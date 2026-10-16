using System.Globalization;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Common;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.WebApp.Controllers;
using Gcpe.Hub.WebApp.Models;
using Gcpe.Hub.Website.Models;
using Gcpe.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BusinessTime.Tests;

public class BusinessTimeTests
{
    // A real controller, but no DB context or mail provider: any accidental use fails.
    private static MediaRequestsApiController Controller() =>
        new(null, null, new ConfigurationBuilder().Build());

    private static string Render(string method, object data)
    {
        var culture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var args = method == "GenerateEmail" ? new object[] { data, false } : new[] { data };
            return (string)typeof(MediaRequestsApiController)
                .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(Controller(), args)!;
        }
        finally { CultureInfo.CurrentCulture = culture; }
    }

    private static MediaRequestDto Request(DateTimeOffset? deadline) => new()
    {
        DeadlineAt = deadline,
        RequestedAt = DateTimeOffset.Parse("2026-11-02T00:30:00Z"),
        RespondedAt = DateTimeOffset.Parse("2026-11-02T16:00:00Z"),
        RequestContent = "Offline request", Response = "Offline response",
        RequestTopic = "Offline topic",
        LeadMinistry = new MinistryDto { Abbreviation = "TEST", DisplayAs = "Test ministry" },
        ResponsibleUser = new UserDto { DisplayAs = "Test user" },
        MediaContacts = Array.Empty<MediaContactDto>(), SharedMinistries = Array.Empty<MinistryDto>(),
        ParentRequest = new MediaRequestDto
        {
            RequestedAt = DateTimeOffset.Parse("2026-11-02T00:30:00Z"),
            RequestContent = "Parent request"
        }
    };

    /// <summary>
    /// Request email: render BC deadline, response time, and follow-up parent date.
    /// </summary>
    [Theory(DisplayName = "Request email: render BC deadline, response time, and follow-up parent date.")]
    [InlineData("2026-11-02T09:00:00-08:00", "Monday, 02 November 2026 10:00")]
    [InlineData("2025-12-01T17:00:00Z", "Monday, 01 December 2025 09:00")]
    public void ActualMediaEmailUsesBcDeadlineResponseAndParentDate(string deadline, string expected)
    {
        var html = Render("GenerateEmail", Request(DateTimeOffset.Parse(deadline)));
        Assert.Contains(expected, html);
        Assert.Contains("Monday, 02 November 2026 09:00", html); // RespondedAt
        Assert.Contains("from November 1, 2026.", html); // parent UTC date was Nov 2
        Assert.DoesNotContain("{{Deadline}}", html);
        Assert.DoesNotContain("{{RespondedAt}}", html);
    }

    /// <summary>
    /// Request email: mark past deadlines overdue and leave future deadlines unmarked.
    /// </summary>
    [Theory(DisplayName = "Request email: mark past deadlines overdue and leave future deadlines unmarked.")]
    [InlineData(-2, true)]
    [InlineData(2, false)]
    public void ActualMediaEmailMarksOverdueByInstant(int days, bool overdue)
    {
        var html = Render("GenerateEmail", Request(DateTimeOffset.UtcNow.AddDays(days).ToOffset(TimeSpan.FromHours(-8))));
        Assert.Equal(overdue, html.Contains("OVERDUE"));
    }

    /// <summary>
    /// Request email: display ASAP without an overdue marker when no deadline exists.
    /// </summary>
    [Fact(DisplayName = "Request email: display ASAP without an overdue marker when no deadline exists.")]
    public void ActualMediaEmailWithoutDeadlineKeepsAsap()
    {
        var html = Render("GenerateEmail", Request(null));
        Assert.Contains("ASAP", html);
        Assert.DoesNotContain("OVERDUE", html);
    }

    /// <summary>
    /// EOD email: render BC deadlines under historical and permanent rules.
    /// </summary>
    [Theory(DisplayName = "EOD email: render BC deadlines under historical and permanent rules.")]
    [InlineData("2026-11-02T09:00:00-08:00", "Monday, November 02, 2026 10:00 AM")]
    [InlineData("2025-12-01T17:00:00Z", "Monday, December 01, 2025 9:00 AM")]
    public void ActualEodEmailUsesBcDeadline(string deadline, string expected)
    {
        var request = new MediaRequest { RequestTopic = "Offline EOD", DeadlineAt = DateTimeOffset.Parse(deadline) };
        var html = Render("GenerateEodEmail", new[] { request });
        Assert.Contains(expected, html);
    }

    /// <summary>
    /// EOD summary: filter open requests, midnight boundaries, last-run timestamps, and nulls.
    /// </summary>
    [Theory(DisplayName = "EOD summary: filter open requests, midnight boundaries, last-run timestamps, and nulls.")]
    [InlineData("open", true)]
    [InlineData("before-midnight", false)]
    [InlineData("at-midnight", true)]
    [InlineData("after-midnight", true)]
    [InlineData("after-last-run-yesterday", true)]
    [InlineData("null-last-run-old", false)]
    public async Task ActualMinistrySummaryFiltersResponseTimes(string scenario, bool included)
    {
        using var db = new HubDbContext(new DbContextOptionsBuilder<HubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        // Dynamic BC midnight keeps the real controller clock; fixed-date boundaries are
        // separately covered by TimeZone.Tests. No production clock override is needed.
        var start = BcTime.TodayStart;
        var ministry = new Ministry
        {
            Id = Guid.NewGuid(), IsActive = true, Abbreviation = "GCPEMEDIA",
            DisplayName = "Offline ministry", EodLastRunDateTime = start.AddHours(12)
        };
        DateTimeOffset? response = scenario switch
        {
            "open" => null,
            "before-midnight" => start.AddSeconds(-1),
            "at-midnight" => start,
            "after-midnight" => start.AddMinutes(30),
            _ => start.AddHours(-1)
        };
        if (scenario == "after-last-run-yesterday") ministry.EodLastRunDateTime = start.AddHours(-2);
        if (scenario == "null-last-run-old") ministry.EodLastRunDateTime = null;
        var request = new MediaRequest
        {
            Id = Guid.NewGuid(), LeadMinistryId = ministry.Id, LeadMinistry = ministry,
            RequestedAt = start.AddHours(-3), RespondedAt = response
        };
        db.Ministry.Add(ministry);
        db.MediaRequest.Add(request);
        // Unrelated mandatory CMS text columns are not relevant to this fixture.
        foreach (var entry in db.ChangeTracker.Entries())
            foreach (var property in entry.Properties.Where(p =>
                p.Metadata.ClrType == typeof(string) && !p.Metadata.IsNullable && p.CurrentValue == null))
                property.CurrentValue = "";
        await db.SaveChangesAsync();
        var summary = Assert.Single(await MinistriesApiController.GetEodSummaryData(db));
        Assert.Equal(included ? response ?? request.RequestedAt : (DateTimeOffset?)null, summary.LastActivity);
    }

    /// <summary>
    /// SQL translation: translate the actual summary query with BC midnight and zero connections.
    /// </summary>
    [Fact(DisplayName = "SQL translation: translate the actual summary query with BC midnight and zero connections.")]
    public async Task ActualSummaryQueryTranslatesToSqlServerWithoutOpeningConnection()
    {
        var capture = new CaptureQuery();
        using var memory = new HubDbContext(new DbContextOptionsBuilder<HubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).EnableServiceProviderCaching(false)
            .AddInterceptors(capture).Options);
        await MinistriesApiController.GetEodSummaryData(memory);
        var expression = capture.Queries.Single(e => typeof(IEnumerable<MediaRequest>).IsAssignableFrom(e.Type));
        var blocker = new RejectConnections();
        using var sql = new HubDbContext(new DbContextOptionsBuilder<HubDbContext>()
            .UseSqlServer("Server=offline.invalid;Database=offline;Integrated Security=True;TrustServerCertificate=True")
            .AddInterceptors(blocker).Options);
        var rebound = new SqlRoots(sql).Visit(expression);
        var query = ((IQueryable<MediaRequest>)sql.MediaRequest).Provider.CreateQuery<MediaRequest>(rebound!);
        var text = query.ToQueryString();
        Assert.Contains("JOIN", text);
        Assert.Contains("RespondedAt", text);
        Assert.Contains("EodLastRunDateTime", text);
        Assert.Contains(BcTime.TodayStart.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture), text);
        Assert.Equal(0, blocker.Attempts);
        Assert.Equal(System.Data.ConnectionState.Closed, sql.Database.GetDbConnection().State);
    }

    private sealed class CaptureQuery : IQueryExpressionInterceptor
    {
        public List<Expression> Queries { get; } = new();
        public Expression QueryCompilationStarting(Expression expression, QueryExpressionEventData data)
        { Queries.Add(expression); return expression; }
    }

    private sealed class SqlRoots(HubDbContext db) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node.Name == "__todayStart_0" ? Expression.Constant(BcTime.TodayStart, node.Type) : base.VisitParameter(node);
        protected override Expression VisitExtension(Expression node) => node is EntityQueryRootExpression root
            ? new EntityQueryRootExpression(db.Model.FindEntityType(root.EntityType.ClrType)!)
            : base.VisitExtension(node);
    }

    private sealed class RejectConnections : DbConnectionInterceptor
    {
        public int Attempts;
        public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData data, InterceptionResult result)
        { Attempts++; throw new InvalidOperationException("Offline tests forbid SQL connections."); }
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData data,
            InterceptionResult result, CancellationToken cancellationToken = default)
        { Attempts++; throw new InvalidOperationException("Offline tests forbid SQL connections."); }
    }
}
