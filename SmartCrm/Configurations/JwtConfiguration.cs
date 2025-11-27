namespace SmartCrm.Configurations;

public static class JwtConfig
{
    public static string Issuer => "http://SmartCRM.uz";
    public static string Audience => "SmartCRM";
    public static string SecurityKey => "BU_YERGA_JUDA_UZUN_VA_MAXFIY_KALIT_SO'Z_YOZING_MINIMUM_32_BELGI";

    // Lifetime in minutes
    public static int Lifetime => 30;
}
