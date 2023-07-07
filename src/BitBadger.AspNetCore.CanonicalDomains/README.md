## BitBadger.AspNetCore.CanonicalDomains

This package provides ASP.NET Core middleware to enforce [canonical domains](https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/Choosing_between_www_and_non-www_URLs).

### What It Does

Having multiple domain names pointing to the same content can lead to inconsistency and diluted search rankings. This middleware intercepts known alternate domains and redirects requests to the canonical domain, ensuring uniformity and unified search rankings.

_If the ASP.NET Core application is running behind a reverse proxy (Nginx, Apache, IIS, etc.), enforcing these domains at that point is the most efficient. This middleware is designed for scenarios where the application is being served directly or in a container, with multiple domains pointed at the running application._

### How to Use

First, install this package.

Second, add the configuration for each domain that needs to be redirected; this middleware will configure itself via the details in the `CanonicalDomains` configuration key. An example:

```json
{
    "CanonicalDomains": [
        {
            "From": "www.example.com",
            "To": "example.com"
        },
        {
            "From": "web.example.com",
            "To": "example.com"
        }
    ]
}
```

Finally, in your main source file (`Program.cs`, `App.fs`, etc.), import the namespace `BitBadger.AspNetCore.CanonicalDomains`, and call `.UseCanonicalDomains()` on the `IApplicationBuilder` instance. It should be placed after `.UseForwardedHeaders()`, if that is used, but should be ahead of `.UseStaticFiles()`, auth config, endpoints, etc. It should be run as close to the start of the pipeline as possible, as no other processing should take place until the request is made on the canonical domain.

### Troubleshooting

This middleware will not throw errors if it cannot parse its configuration properly _(feel free to do the final step before adding configuration to verify!)_. However, if `.UseCanonicalDomains()` is called, and the setup does not find anything to do, it will emit a warning in the log, and will not add the middleware to the pipeline. If redirection is not occurring as you suspect it should, check the top of the log when the application starts.
