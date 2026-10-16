using System;
namespace Gcpe.Time
{
    public static class ChangeFreezeWindow
    {
        public static bool IsFrozen(DateTimeOffset nowUtc, bool isHeadquarters, BcTimeConverter time)
        {
            var clock = time.ToLocal(nowUtc).TimeOfDay;
            return !isHeadquarters && clock > new TimeSpan(16, 5, 0) && clock < new TimeSpan(17, 0, 0);
        }
    }
}
