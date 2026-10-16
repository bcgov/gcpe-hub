using System;
using Gcpe.Time;
using Xunit;
namespace TimeZone.Tests
{
    public class FreezeWindowTests
    {
        /// <summary>
        /// Server freeze: enforce strict BC afternoon boundaries and headquarters exemption.
        /// </summary>
        [Theory(DisplayName = "Server freeze: enforce strict BC afternoon boundaries and headquarters exemption.")]
        [InlineData(4,5,false)]
        [InlineData(4,30,false)]
        [InlineData(16,4,false)]
        [InlineData(16,5,false)]
        [InlineData(16,6,true)]
        [InlineData(16,59,true)]
        [InlineData(17,0,false)]
        [InlineData(17,1,false)]
        public void ServerAppliesOriginalAfternoonBoundaries(int hour,int minute,bool expected)
        {
            var time=TimeFixture.Fallback();
            var instant=time.FromLocal(new DateTime(2026,11,2,hour,minute,0));
            Assert.Equal(expected,ChangeFreezeWindow.IsFrozen(instant,false,time));
            Assert.False(ChangeFreezeWindow.IsFrozen(instant,true,time));
        }
        /// <summary>
        /// Historical freeze: UTC winter time maps to the previous BC afternoon.
        /// </summary>
        [Fact(DisplayName = "Historical freeze: UTC winter time maps to the previous BC afternoon.")]
        public void WinterBeforeMigrationUsesHistoricalHour()
        {
            Assert.True(ChangeFreezeWindow.IsFrozen(TimeFixture.Instant("2025-12-02T00:30:00Z"),false,TimeFixture.Fallback()));
        }
    }
}
