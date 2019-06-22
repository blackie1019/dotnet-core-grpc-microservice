#region

using MockSite.Common.Logging.Enums;

#endregion

namespace MockSite.Common.Logging.Utilities.LogDetail
{
    public class PerformanceDetail : InfoDetail
    {
        public long Duration { get; set; }

        public DurationRank Rank { get; set; }
    }
}