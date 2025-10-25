using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace TestingExample.Website.FunctionalTests;

public class TestConfiguration
{
    private static readonly Lock ConfigLock = new();
    private static IConfiguration? s_configuration = null;

    public static IConfiguration GetConfiguration()
    {
        // double-if pattern ensures no unnecessary locks while reading the config.
        // As soon as the config contains a value, everybody is free to use it.
        if (s_configuration is null)
        {
            lock (ConfigLock)
            {
                s_configuration ??= new ConfigurationBuilder()
                        .AddEnvironmentVariables()
                        .AddUserSecrets(Assembly.GetExecutingAssembly())
                        .Build();
            }
        }

        return s_configuration;
    }

    public static T GetRequiredValue<T>(string section)
    {
        var config = GetConfiguration();
        return config.GetSection(section).Get<T>() ?? throw new InvalidOperationException($"Unable to bind section {section} to object of type {typeof(T)}");
    }
}
