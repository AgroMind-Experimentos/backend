namespace EcotrackPlatform.API.Shared.Infrastructure.Configuration.Utils;

public static class EnvVarUtils
{
    public static string GetRequiredEnvVar(string variableName)
    {
        var value = Environment.GetEnvironmentVariable(variableName);

        return string.IsNullOrWhiteSpace(value) 
            ? throw new InvalidOperationException($"CRITICAL CONFIGURATION MISSING: Environment variable '{variableName}' is not set.")
            : value;
    }

    public static string GetOptionalEnvVar(string variableName, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(variableName);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }
}