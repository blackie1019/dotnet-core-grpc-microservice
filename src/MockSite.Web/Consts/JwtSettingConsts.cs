namespace MockSite.Web.Consts
{
    public static class JwtSettingConsts
    {
        public static readonly string SecretKey = "AppSettings:Jwt:Secret";
        public static readonly string IssuerKey = "AppSettings:Jwt:Issuer";
        public static readonly string AudienceKey = "AppSettings:Jwt:Audience";
    }
}