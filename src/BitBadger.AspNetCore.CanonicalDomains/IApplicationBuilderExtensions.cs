using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BitBadger.AspNetCore.CanonicalDomains;

/// <summary>
/// Extensions on the <see cref="IApplicationBuilder" /> interface
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Initialize and use the canonical domain middleware
    /// </summary>
    public static IApplicationBuilder UseCanonicalDomains(this IApplicationBuilder app)
    {
        ParseConfiguration(GetService<IConfiguration>(app)!.GetSection("CanonicalDomains"));

        if (CanonicalDomainMiddleware.CanonicalDomains.Count > 0)
        {
            return app.UseMiddleware<CanonicalDomainMiddleware>();
        }
        
        WarnForMissingConfig(app);
        return app;
    }

    /// <summary>
    /// Shorthand for retrieving typed services from the application's service provider
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The requested service, or null if it was not able to be found</returns>
    private static T? GetService<T>(IApplicationBuilder app) =>
        (T?)app.ApplicationServices.GetService(typeof(T));
    
    /// <summary>
    /// Extract the from/to domain paris from the configuration
    /// </summary>
    /// <param name="section">The <tt>CanonicalDomains</tt> configuration section</param>
    private static void ParseConfiguration(IConfigurationSection? section)
    {
        if (section is not null)
        {
            foreach (var item in section.GetChildren())
            {
                var nonCanonical = item["From"];
                var canonical = item["To"];
                if (nonCanonical is not null && canonical is not null)
                {
                    CanonicalDomainMiddleware.CanonicalDomains.Add(nonCanonical, canonical);
                }
            }
        }
    }

    /// <summary>
    /// Generate a warning if no configured domains were found
    /// </summary>
    /// <param name="app">The application builder</param>
    private static void WarnForMissingConfig(IApplicationBuilder app)
    {
        var logger = GetService<ILogger<CanonicalDomainMiddleware>>(app);
        if (logger is not null)
        {
            logger.LogWarning("No canonical domain configuration was found; no domains will be redirected");
        }
    }
}
