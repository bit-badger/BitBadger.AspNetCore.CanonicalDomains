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
        void warnForMissingConfig() {
            var logger = (ILogger<CanonicalDomainMiddleware>?)app.ApplicationServices
                .GetService(typeof(ILogger<CanonicalDomainMiddleware>));
            if (logger is not null)
            {
                logger.LogWarning("No canonical domain configuration was found; no domains will be redirected");
            }
        }

        var config = (IConfiguration)app.ApplicationServices.GetService(typeof(IConfiguration))!;

        var section = config.GetSection("CanonicalDomains");
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

            if (CanonicalDomainMiddleware.CanonicalDomains.Count > 0)
            {
                app.UseMiddleware<CanonicalDomainMiddleware> ();
            }
            else
            {
                warnForMissingConfig();
            }
        }
        else
        {
            warnForMissingConfig();
        }
        
        return app;
    }
}