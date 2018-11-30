using Dapper.Contrib.Extensions;

namespace MockSite.Core.Entities
{
    [Table("User")]
    public class User
    {
        [ExplicitKey] 
        public int Code { get; set; }
        public string DisplayKey { get; set; }
        public int OrderNo { get; set; }
    }
}