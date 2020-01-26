#region

using MockSite.Common.Logging.Enums;

#endregion

namespace MockSite.Common.Logging.Utilities.LogDetail
{
    public class PerformanceDetail : InfoDetail
    {

        public DurationRank Rank
        {
            get
            {
                if (this.Duration <= 0 || this.Rank != DurationRank.Unknown) return DurationRank.Unknown;
                if (this.Duration > 0 && this.Duration < 3 * 1000) // < 3 Seconds
                    return DurationRank.Normal;
                if (this.Duration >= 3000 && this.Duration <= 10000) // 3~10 Seconds
                    return DurationRank.Slow;
                if (this.Duration > 10000 && this.Duration <= 30000) // 10~30 Seconds
                    return DurationRank.DamnSlow;                    // > 30 Seconds
                return this.Duration > 30000 ? DurationRank.FxxxSlow : DurationRank.Unknown;
            }
        }
    }
}