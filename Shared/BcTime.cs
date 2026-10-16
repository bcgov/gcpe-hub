using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gcpe.Time
{
    // Keep this source compatible with .NET Framework 4.6.2 as well as modern .NET.
    /// <summary>
    /// Converts absolute instants and BC wall times for gcpe-hub's Windows/IIS applications.
    /// Use BcTime when a business date or displayed time must be BC time regardless of the host zone.
    /// </summary>
    /// <remarks>
    /// CreateSystem tries BC_TIME_ZONE_ID, then installed Windows zones whose ID or display name
    /// contains "British Columbia". Candidates must preserve the 2025 winter/summer offsets and
    /// use UTC-7 in December 2026. This copy has no Linux-specific time zone lookup.
    /// If no candidate passes, conversion uses fixed UTC-7 from 2026-11-01 BC and historical
    /// "Pacific Standard Time" rules before that date, with a warning at initialisation.
    /// Rules are selected by the date being converted, not by the current date or host zone.
    /// If historical data is also missing, TryToLocal returns UTC/false and FromLocal throws for
    /// earlier dates; callers of ToLocal must account for that UTC display fallback.
    /// PublishDate and MaxPublishDate keep their stored scheduled-instant fallback in that case.
    /// </remarks>
    public sealed class BcTimeConverter
    {
        public static readonly TimeSpan PermanentOffset = TimeSpan.FromHours(-7);
        private static readonly DateTime PermanentLocalStart = new DateTime(2026, 11, 1);
        private static readonly DateTimeOffset PermanentUtcStart =
            new DateTimeOffset(2026, 11, 1, 0, 0, 0, PermanentOffset);
        private readonly TimeZoneInfo currentZone;
        private readonly TimeZoneInfo historicalZone;

        public BcTimeConverter(Func<string, TimeZoneInfo> findZone,
            IEnumerable<string> bcIds, IEnumerable<string> historicalIds, Action<string> warn)
        {
            foreach (var id in bcIds)
            {
                var zone = Find(findZone, id);
                if (zone != null && HasBcRules(zone))
                {
                    currentZone = historicalZone = zone;
                    return;
                }
            }

            foreach (var id in historicalIds)
            {
                var zone = Find(findZone, id);
                if (zone != null && HasHistoricalRules(zone))
                {
                    historicalZone = zone;
                    break;
                }
            }
            if (warn != null)
                warn("BC time zone is unavailable or has old rules. Using UTC-7 from " +
                    "2026-11-01 and " + (historicalZone == null ? "UTC for historical display." :
                    historicalZone.Id + " for historical dates."));
        }

        private static TimeZoneInfo Find(Func<string, TimeZoneInfo> findZone, string id)
        {
            try { return findZone(id); }
            catch (TimeZoneNotFoundException) { return null; }
            catch (InvalidTimeZoneException) { return null; }
        }

        public static bool HasBcRules(TimeZoneInfo zone)
        {
            return HasHistoricalRules(zone) &&
                zone.GetUtcOffset(new DateTimeOffset(2026, 12, 1, 12, 0, 0, TimeSpan.Zero)) == PermanentOffset;
        }

        private static bool HasHistoricalRules(TimeZoneInfo zone)
        {
            return zone.GetUtcOffset(new DateTimeOffset(2025, 12, 1, 12, 0, 0, TimeSpan.Zero)) == TimeSpan.FromHours(-8) &&
                zone.GetUtcOffset(new DateTimeOffset(2025, 7, 1, 12, 0, 0, TimeSpan.Zero)) == PermanentOffset;
        }

        public bool TryToLocal(DateTimeOffset instant, out DateTimeOffset local)
        {
            if (currentZone != null)
            {
                local = TimeZoneInfo.ConvertTime(instant, currentZone);
                return true;
            }
            if (instant >= PermanentUtcStart)
            {
                local = instant.ToOffset(PermanentOffset);
                return true;
            }
            if (historicalZone != null)
            {
                local = TimeZoneInfo.ConvertTime(instant, historicalZone);
                return true;
            }
            local = instant.ToUniversalTime();
            return false;
        }

        public DateTimeOffset ToLocal(DateTimeOffset instant)
        {
            DateTimeOffset local;
            TryToLocal(instant, out local);
            return local;
        }

        public bool TryFromLocal(DateTime wallTime, out DateTimeOffset instant)
        {
            // Inputs here are BC wall times, never the host's local time zone.
            wallTime = DateTime.SpecifyKind(wallTime, DateTimeKind.Unspecified);
            var zone = currentZone;
            if (zone == null && wallTime >= PermanentLocalStart)
            {
                instant = new DateTimeOffset(wallTime, PermanentOffset);
                return true;
            }
            zone = zone ?? historicalZone;
            if (zone == null || zone.IsInvalidTime(wallTime))
            {
                instant = default(DateTimeOffset);
                return false;
            }
            instant = new DateTimeOffset(wallTime, zone.GetUtcOffset(wallTime));
            return true;
        }

        public DateTimeOffset FromLocal(DateTime wallTime)
        {
            DateTimeOffset instant;
            if (!TryFromLocal(wallTime, out instant))
                throw new ArgumentException("The BC local time does not exist, or historical time zone data is unavailable.", "wallTime");
            return instant;
        }

        public static BcTimeConverter CreateSystem()
        {
            var bcIds = new List<string>();
            var configuredId = Environment.GetEnvironmentVariable("BC_TIME_ZONE_ID");
            if (!string.IsNullOrWhiteSpace(configuredId)) bcIds.Add(configuredId);
            // Discover the actual installed Windows ID rather than guessing it.
            foreach (var zone in TimeZoneInfo.GetSystemTimeZones())
                if (zone.Id.IndexOf("British Columbia", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    zone.DisplayName.IndexOf("British Columbia", StringComparison.OrdinalIgnoreCase) >= 0)
                    bcIds.Add(zone.Id);

            return new BcTimeConverter(TimeZoneInfo.FindSystemTimeZoneById, bcIds,
                new[] { "Pacific Standard Time" },
                message => Trace.TraceWarning(message));
        }
    }

    public static class BcTime
    {
        public static readonly BcTimeConverter Default = BcTimeConverter.CreateSystem();
        public static DateTimeOffset Now { get { return ToLocal(DateTimeOffset.UtcNow); } }
        public static DateTime Today { get { return Now.Date; } }
        public static DateTimeOffset TodayStart { get { return FromLocal(Today); } }
        public static DateTimeOffset ToLocal(DateTimeOffset instant) { return Default.ToLocal(instant); }
        public static DateTimeOffset FromLocal(DateTime wallTime) { return Default.FromLocal(wallTime); }
        public static DateTimeOffset PublishDate(DateTime? released, DateTimeOffset scheduled)
        {
            return PublishDate(released, scheduled, Default);
        }
        public static DateTimeOffset PublishDate(DateTime? released, DateTimeOffset scheduled, BcTimeConverter time)
        {
            DateTimeOffset instant;
            // A stored instant remains truthful if historical wall-time data is unavailable.
            return released.HasValue && time.TryFromLocal(released.Value, out instant) ? instant : scheduled;
        }
        public static DateTimeOffset? MaxPublishDate(DateTime? released, DateTimeOffset? scheduled)
        {
            if (!released.HasValue) return scheduled;
            DateTimeOffset releaseInstant;
            if (!Default.TryFromLocal(released.Value, out releaseInstant)) return scheduled;
            return !scheduled.HasValue || releaseInstant > scheduled.Value ? releaseInstant : scheduled;
        }
    }
}
