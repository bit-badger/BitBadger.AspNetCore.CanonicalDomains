﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace BitBadger.AspNetCore.CanonicalDomains;

public class CanonicalDomainMiddleware
{
    /// <summary>
    /// The domains which should be redirected
    /// </summary>
    internal static readonly IDictionary<string, string> CanonicalDomains = new Dictionary<string, string>();

    /// <summary>
    /// The next middleware in the pipeline to be executed
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">The next middleware in the pipeline to be exectued</param>
    public CanonicalDomainMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        var host = ctx.Request.Host.Host;
        if (CanonicalDomains.ContainsKey(host))
        {
            UriBuilder uri = new(ctx.Request.GetDisplayUrl());
            uri.Host = CanonicalDomains[host];
            ctx.Response.Redirect(uri.Uri.ToString ());
        }
        else
        {
            await _next.Invoke(ctx);
        }
    }
}
