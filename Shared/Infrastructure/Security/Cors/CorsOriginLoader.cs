using EcotrackPlatform.API.Shared.Infrastructure.Configuration.Utils;

namespace EcotrackPlatform.API.Shared.Infrastructure.Security.Cors;

public static class CorsOriginLoader
{
    private const string EnvFrontendUrl = "FRONTEND_URL";
    private const string ProductionUrl = "https://agrotrackfront.vercel.app";

    public static string[] GetAllowedOrigins(bool isProduction)
    {
        string originsStr;

        if (isProduction)
        {
            originsStr = ProductionUrl;
        }
        else
        {
            originsStr = "http://localhost:5173";
        }

        return originsStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    public static string GetPolicyName(bool isProduction)
    {
        return isProduction ? "AllowWebPolicy" : "AllowDevPolicy";
    }
}