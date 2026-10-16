using System;
using System.Collections.Generic;
using Gcpe.Time;
using Xunit;

namespace TimeZone.Tests
{
    public static class TimeFixture
    {
        public static readonly TimeZoneInfo OldPacific = CreatePacific(false);
        public static readonly TimeZoneInfo UpdatedBc = CreatePacific(true);
        private static TimeZoneInfo CreatePacific(bool updated)
        {
            var march = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1,1,1,2,0,0),3,2,DayOfWeek.Sunday);
            var november = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1,1,1,2,0,0),11,1,DayOfWeek.Sunday);
            var rules = new List<TimeZoneInfo.AdjustmentRule>();
            rules.Add(TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2007,1,1),new DateTime(updated ? 2025 : 2099,12,31),TimeSpan.FromHours(1),march,november));
            if (updated)
            {
                var yearEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1,1,1,23,59,59),12,31);
                var yearStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1,1,1),1,1);
                // Framework treats the annual Jan 1 marker as a gap. Exact New Year
                // coverage below uses real system rules and the permanent-offset fallback.
                rules.Add(TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2026,1,1),new DateTime(2026,12,31),TimeSpan.FromHours(1),march,yearEnd));
                rules.Add(TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2027,1,1),new DateTime(2099,12,31),TimeSpan.FromHours(1),yearStart,yearEnd));
            }
            return TimeZoneInfo.CreateCustomTimeZone(updated ? "Updated BC test" : "Old Pacific test",TimeSpan.FromHours(-8),"Test Pacific","Test PST","Test PDT",rules.ToArray());
        }
        public static BcTimeConverter Fallback(Action<string> warning = null)
        {
            return new BcTimeConverter(id => id == "old" ? OldPacific : throw new TimeZoneNotFoundException(id),new[]{"new"},new[]{"old"},warning);
        }
        public static BcTimeConverter Current()
        {
            return new BcTimeConverter(id => UpdatedBc,new[]{"new"},new[]{"old"},null);
        }
        public static BcTimeConverter Missing()
        {
            return new BcTimeConverter(id => { throw new TimeZoneNotFoundException(id); },new[]{"new"},new[]{"old"},null);
        }
        public static DateTimeOffset Instant(string value) { return DateTimeOffset.Parse(value); }
    }

    public class BcTimeTests
    {
        /// <summary>
        /// UTC to BC: updated and fallback rules preserve the instant and historical offsets.
        /// </summary>
        [Theory(DisplayName = "UTC to BC: updated and fallback rules preserve the instant and historical offsets.")]
        [InlineData("2025-12-01T17:00:00Z","2025-12-01 09:00 -08:00")]
        [InlineData("2025-07-01T16:00:00Z","2025-07-01 09:00 -07:00")]
        [InlineData("2026-03-08T09:59:00Z","2026-03-08 01:59 -08:00")]
        [InlineData("2026-03-08T10:00:00Z","2026-03-08 03:00 -07:00")]
        [InlineData("2026-11-01T06:59:59Z","2026-10-31 23:59 -07:00")]
        [InlineData("2026-11-01T07:00:00Z","2026-11-01 00:00 -07:00")]
        [InlineData("2026-11-01T08:59:59Z","2026-11-01 01:59 -07:00")]
        [InlineData("2026-11-01T09:00:00Z","2026-11-01 02:00 -07:00")]
        [InlineData("2026-11-02T16:00:00Z","2026-11-02 09:00 -07:00")]
        [InlineData("2027-01-15T16:00:00Z","2027-01-15 09:00 -07:00")]
        [InlineData("2027-07-01T16:00:00Z","2027-07-01 09:00 -07:00")]
        public void UpdatedRulesAndFallbackPreserveBcClockAndInstant(string utc, string expected)
        {
            var input = TimeFixture.Instant(utc);
            foreach (var time in new[]{TimeFixture.Current(),TimeFixture.Fallback()})
            {
                var actual = time.ToLocal(input);
                Assert.Equal(expected,actual.ToString("yyyy-MM-dd HH:mm zzz"));
                Assert.Equal(input.UtcDateTime,actual.UtcDateTime);
            }
        }
        /// <summary>
        /// BC input: all DateTimeKind values are interpreted as BC wall time.
        /// </summary>
        [Theory(DisplayName = "BC input: all DateTimeKind values are interpreted as BC wall time.")]
        [InlineData(2025,12,1,17)]
        [InlineData(2025,7,1,16)]
        [InlineData(2026,11,2,16)]
        [InlineData(2027,7,1,16)]
        public void WallTimeInputIsIndependentOfHostAndKind(int year,int month,int day,int utcHour)
        {
            foreach (var time in new[]{TimeFixture.Current(),TimeFixture.Fallback()})
                foreach (DateTimeKind kind in Enum.GetValues(typeof(DateTimeKind)))
                    Assert.Equal(new DateTime(year,month,day,utcHour,0,0,DateTimeKind.Utc),
                        time.FromLocal(new DateTime(year,month,day,9,0,0,kind)).UtcDateTime);
        }
        /// <summary>
        /// Permanent-rule boundary: BC wall time does not repeat an hour after the switch.
        /// </summary>
        [Theory(DisplayName = "Permanent-rule boundary: BC wall time does not repeat an hour after the switch.")]
        [InlineData("2026-10-31T23:59:59","2026-11-01T06:59:59Z")]
        [InlineData("2026-11-01T00:00:00","2026-11-01T07:00:00Z")]
        [InlineData("2026-11-01T01:30:00","2026-11-01T08:30:00Z")]
        [InlineData("2026-11-01T02:00:00","2026-11-01T09:00:00Z")]
        [InlineData("2027-01-02T00:00:00","2027-01-02T07:00:00Z")]
        public void WallTimeCrossesPermanentRuleBoundaryWithoutRepeatingAnHour(string wall,string utc)
        {
            foreach (var time in new[]{TimeFixture.Current(),TimeFixture.Fallback()})
            {
                var actual = time.FromLocal(DateTime.Parse(wall));
                Assert.Equal(TimeFixture.Instant(utc),actual);
                Assert.Equal(TimeSpan.FromHours(-7),actual.Offset);
            }
        }
        /// <summary>
        /// EOD day start: use the offset at BC midnight, including historical DST days.
        /// </summary>
        [Theory(DisplayName = "EOD day start: use the offset at BC midnight, including historical DST days.")]
        [InlineData("2025-12-01T12:00:00-08:00","2025-12-01T08:00:00Z")]
        [InlineData("2025-11-02T12:00:00-08:00","2025-11-02T07:00:00Z")]
        [InlineData("2026-03-08T12:00:00-07:00","2026-03-08T08:00:00Z")]
        [InlineData("2026-11-02T12:00:00-07:00","2026-11-02T07:00:00Z")]
        public void EodDayStartUsesBcMidnightOffsetRatherThanHostOrNoonOffset(string now,string midnightUtc)
        {
            foreach (var time in new[]{TimeFixture.Current(),TimeFixture.Fallback()})
            {
                var localNow = time.ToLocal(TimeFixture.Instant(now));
                Assert.Equal(TimeFixture.Instant(midnightUtc),time.FromLocal(localNow.Date));
            }
        }
        /// <summary>
        /// Historical repeated time: interpret the ambiguous wall time using standard time.
        /// </summary>
        [Fact(DisplayName = "Historical repeated time: interpret the ambiguous wall time using standard time.")]
        public void HistoricalRepeatedWallTimeKeepsStandardTimeInterpretation()
        {
            foreach (var time in new[]{TimeFixture.Current(),TimeFixture.Fallback()})
            {
                var actual = time.FromLocal(new DateTime(2025,11,2,1,30,0));
                Assert.Equal(TimeSpan.FromHours(-8),actual.Offset);
                Assert.Equal(TimeFixture.Instant("2025-11-02T09:30:00Z"),actual);
            }
        }
        /// <summary>
        /// New Year midnight: system and fallback rules map midnight to 07:00 UTC.
        /// </summary>
        [Fact(DisplayName = "New Year midnight: system and fallback rules map midnight to 07:00 UTC.")]
        public void PermanentNewYearMidnightIsValidWithSystemRulesAndFallback()
        {
            foreach (var time in new[]{BcTimeConverter.CreateSystem(),TimeFixture.Fallback()})
            {
                var actual = time.FromLocal(new DateTime(2027,1,1));
                Assert.Equal(TimeFixture.Instant("2027-01-01T07:00:00Z"),actual);
                Assert.Equal(TimeSpan.FromHours(-7),actual.Offset);
            }
        }
        /// <summary>
        /// Schedule roundtrip: displaying and parsing BC input preserves the stored instant.
        /// </summary>
        [Theory(DisplayName = "Schedule roundtrip: displaying and parsing BC input preserves the stored instant.")]
        [InlineData("2026-11-02T09:00:00-08:00","2026-11-02 10:00")]
        [InlineData("2026-11-02T00:30:00Z","2026-11-01 17:30")]
        [InlineData("2025-12-01T17:00:00Z","2025-12-01 09:00")]
        public void EditingStoredScheduleThroughBcWallTimePreservesItsInstant(string stored,string expected)
        {
            foreach (var time in new[]{TimeFixture.Current(),TimeFixture.Fallback()})
            {
                var input = TimeFixture.Instant(stored);
                var text = time.ToLocal(input).ToString("yyyy-MM-dd HH:mm",System.Globalization.CultureInfo.InvariantCulture);
                Assert.Equal(expected,text);
                var saved = time.FromLocal(DateTime.ParseExact(text,"yyyy-MM-dd HH:mm",System.Globalization.CultureInfo.InvariantCulture));
                Assert.Equal(input.UtcDateTime,saved.UtcDateTime);
            }
        }
        /// <summary>
        /// Outdated rules: use the fallback and issue only one initialization warning.
        /// </summary>
        [Fact(DisplayName = "Outdated rules: use the fallback and issue only one initialization warning.")]
        public void ExistingButOldVancouverRulesAlsoTriggerFallback()
        {
            var warnings = new List<string>();
            var time = new BcTimeConverter(id => TimeFixture.OldPacific,new[]{"Vancouver"},new[]{"old"},warnings.Add);
            Assert.Equal(TimeSpan.FromHours(-7),time.ToLocal(TimeFixture.Instant("2026-12-01T16:00:00Z")).Offset);
            Assert.Equal(TimeSpan.FromHours(-8),time.ToLocal(TimeFixture.Instant("2025-12-01T17:00:00Z")).Offset);
            for (int i=0;i<20;i++) time.ToLocal(DateTimeOffset.UtcNow);
            Assert.Single(warnings);
        }
        /// <summary>
        /// Valid BC rules: preserve historical and future offsets without a fallback warning.
        /// </summary>
        [Fact(DisplayName = "Valid BC rules: preserve historical and future offsets without a fallback warning.")]
        public void ValidNewRulesAreUsedWithoutWarning()
        {
            var warnings = new List<string>();
            var time = new BcTimeConverter(id => TimeFixture.UpdatedBc,new[]{"new"},new[]{"old"},warnings.Add);
            Assert.True(BcTimeConverter.HasBcRules(TimeFixture.UpdatedBc));
            Assert.Empty(warnings);
            Assert.Equal(TimeSpan.FromHours(-8),time.ToLocal(TimeFixture.Instant("2025-12-01T17:00:00Z")).Offset);
            Assert.Equal(TimeSpan.FromHours(-7),time.ToLocal(TimeFixture.Instant("2026-12-01T16:00:00Z")).Offset);
        }
        /// <summary>
        /// Corrupt zone: invalid candidate data does not prevent fallback conversion.
        /// </summary>
        [Fact(DisplayName = "Corrupt zone: invalid candidate data does not prevent fallback conversion.")]
        public void CorruptZoneDoesNotPreventFallback()
        {
            var time = new BcTimeConverter(id => { if(id=="new") throw new InvalidTimeZoneException(); return TimeFixture.OldPacific; },new[]{"new"},new[]{"old"},null);
            Assert.Equal(9,time.ToLocal(TimeFixture.Instant("2026-11-02T16:00:00Z")).Hour);
        }
        /// <summary>
        /// Missing rules: retain future UTC-7, historical UTC display fallback, and input failure.
        /// </summary>
        [Fact(DisplayName = "Missing rules: retain future UTC-7, historical UTC display fallback, and input failure.")]
        public void MissingAllDataStillSupportsFutureAndReturnsUtcForHistoricalDisplay()
        {
            var time = TimeFixture.Missing();
            DateTimeOffset result;
            Assert.False(time.TryToLocal(TimeFixture.Instant("2025-12-01T17:00:00Z"),out result));
            Assert.Equal(TimeSpan.Zero,result.Offset);
            Assert.Equal(TimeFixture.Instant("2025-12-01T17:00:00Z"),time.ToLocal(TimeFixture.Instant("2025-12-01T17:00:00Z")));
            Assert.True(time.TryToLocal(TimeFixture.Instant("2026-11-02T16:00:00Z"),out result));
            Assert.Equal(9,result.Hour);
            Assert.False(time.TryFromLocal(new DateTime(2025,12,1,9,0,0),out result));
            Assert.Throws<ArgumentException>(() => time.FromLocal(new DateTime(2025,12,1,9,0,0)));
            Assert.True(time.TryFromLocal(new DateTime(2026,11,2,9,0,0),out result));
            Assert.Equal(16,result.UtcDateTime.Hour);
        }
        /// <summary>
        /// Spring gap: reject a nonexistent BC wall time instead of silently shifting it.
        /// </summary>
        [Fact(DisplayName = "Spring gap: reject a nonexistent BC wall time instead of silently shifting it.")]
        public void NonexistentSpringTimeIsNotSilentlyShifted()
        {
            foreach (var time in new[]{TimeFixture.Current(),TimeFixture.Fallback()})
            {
                DateTimeOffset result;
                Assert.False(time.TryFromLocal(new DateTime(2026,3,8,2,30,0),out result));
            }
        }
        /// <summary>
        /// Resolver errors: propagate unexpected exceptions instead of hiding them.
        /// </summary>
        [Fact(DisplayName = "Resolver errors: propagate unexpected exceptions instead of hiding them.")]
        public void UnexpectedResolverFailuresAreNotSwallowed()
        {
            Assert.Throws<InvalidOperationException>(() => new BcTimeConverter(id => {throw new InvalidOperationException();},new[]{"new"},new[]{"old"},null));
        }
        /// <summary>
        /// System resolver: preserve historical UTC-8 and future UTC-7 offsets.
        /// </summary>
        [Fact(DisplayName = "System resolver: preserve historical UTC-8 and future UTC-7 offsets.")]
        public void ProductionSystemResolverPreservesHistoricalAndFutureOffsets()
        {
            var time = BcTimeConverter.CreateSystem();
            Assert.Equal(TimeSpan.FromHours(-8),time.ToLocal(TimeFixture.Instant("2025-12-01T17:00:00Z")).Offset);
            Assert.Equal(TimeSpan.FromHours(-7),time.ToLocal(TimeFixture.Instant("2026-12-01T16:00:00Z")).Offset);
        }
        /// <summary>
        /// Publication fallback: retain the stored scheduled instant when historical rules are missing.
        /// </summary>
        [Fact(DisplayName = "Publication fallback: retain the stored scheduled instant when historical rules are missing.")]
        public void MissingHistoricalRulesKeepStoredApiInstantInsteadOfFailing()
        {
            var scheduled=TimeFixture.Instant("2025-12-01T17:00:00Z");
            Assert.Equal(scheduled,BcTime.PublishDate(new DateTime(2025,12,1,9,0,0),scheduled,TimeFixture.Missing()));
        }
        /// <summary>
        /// Old stored offset: display the BC hour without rewriting the absolute instant.
        /// </summary>
        [Fact(DisplayName = "Old stored offset: display the BC hour without rewriting the absolute instant.")]
        public void StoredWrongFutureOffsetIsNotSilentlyRewritten()
        {
            var stored=TimeFixture.Instant("2026-11-02T09:00:00-08:00");
            var displayed=TimeFixture.Fallback().ToLocal(stored);
            Assert.Equal(10,displayed.Hour);
            Assert.Equal(stored.UtcDateTime,displayed.UtcDateTime);
        }
        /// <summary>
        /// BC calendar day: change the date at BC midnight instead of UTC midnight.
        /// </summary>
        [Theory(DisplayName = "BC calendar day: change the date at BC midnight instead of UTC midnight.")]
        [InlineData("2026-11-02T06:59:59Z",1)]
        [InlineData("2026-11-02T07:00:00Z",2)]
        public void TodayChangesAtBcMidnightRatherThanUtcMidnight(string now,int expectedDay)
        {
            Assert.Equal(expectedDay,TimeFixture.Fallback().ToLocal(TimeFixture.Instant(now)).Day);
        }
        /// <summary>
        /// Publication dates: interpret BC wall time and select the later instant with null handling.
        /// </summary>
        [Fact(DisplayName = "Publication dates: interpret BC wall time and select the later instant with null handling.")]
        public void ReleaseDateAndMaxDateDoNotUseHostOffset()
        {
            var scheduled = TimeFixture.Instant("2026-11-02T16:00:00Z");
            Assert.Equal(scheduled,BcTime.PublishDate(new DateTime(2026,11,2,9,0,0),scheduled));
            Assert.Equal(scheduled,BcTime.PublishDate(null,scheduled));
            Assert.Equal(TimeFixture.Instant("2026-11-02T17:00:00Z"),BcTime.MaxPublishDate(new DateTime(2026,11,2,10,0,0),scheduled));
            Assert.Null(BcTime.MaxPublishDate(null,null));
        }
    }
}
