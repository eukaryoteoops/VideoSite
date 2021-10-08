using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Comic.Cache;
using Comic.Common.BaseClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Comic.Api
{
    public class AuthorizationFilter : AuthorizeFilter
    {
        private readonly CacheTokenProvider _cache;
        private readonly IMemoryCache _memoryCache;

        public AuthorizationFilter(CacheTokenProvider cache, IMemoryCache memoryCache) : base(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
        {
            _cache = cache;
            _memoryCache = memoryCache;
        }

        public override Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!HasAllowAnonymous(context))
            {
                var memberId = context.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value;
                if (memberId == null)
                {
                    context.Result = new UnauthorizedResult();
                    return Task.FromResult(0);
                }
                var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", string.Empty);
                var watch = new Stopwatch();
                watch.Start();
                var cached = _memoryCache.Get<string>($"{CacheKeys.MemberToken}{memberId}");
                //var cached = _cache.GetStringAndRefresh($"{CacheKeys.MemberToken}{memberId}", TimeSpan.FromHours(6));
                watch.Stop();
                context.HttpContext.Response.Headers["X-GetTokenCacheTime-ms"] = watch.ElapsedMilliseconds.ToString();
                if (cached != null)
                    if (token == cached)
                    {
                        return base.OnAuthorizationAsync(context);
                    }
                context.Result = new UnauthorizedResult();
            }
            return base.OnAuthorizationAsync(context);
        }

        private static bool HasAllowAnonymous(AuthorizationFilterContext context)
        {
            var filters = context.Filters;
            for (var i = 0; i < filters.Count; i++)
            {
                if (filters[i] is IAllowAnonymousFilter)
                {
                    return true;
                }
            }

            // When doing endpoint routing, MVC does not add AllowAnonymousFilters for AllowAnonymousAttributes that
            // were discovered on controllers and actions. To maintain compat with 2.x,
            // we'll check for the presence of IAllowAnonymous in endpoint metadata.
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return true;
            }

            return false;
        }
    }
}
