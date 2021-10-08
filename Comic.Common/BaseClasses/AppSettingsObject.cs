namespace Comic.Common.BaseClasses
{
    public class AppSettingsObject
    {
        public JwtObject Jwt { get; set; }
        public class JwtObject
        {
            public string Issuer { get; set; }
            public string Key { get; set; }
            public int ExpiredIn { get; set; }
        }
    }
}
