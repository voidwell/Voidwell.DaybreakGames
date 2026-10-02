using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Api.Authentication
{
    /// <summary>
    /// Adds role claims from Voidwell.UserManagement to authenticated users, replacing the
    /// role lookup that previously happened in Voidwell.Api.
    /// </summary>
    public class RoleClaimsTransformation : IClaimsTransformation
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(2);

        private readonly IUserRolesClient _userRolesClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RoleClaimsTransformation> _logger;

        public RoleClaimsTransformation(IUserRolesClient userRolesClient, IMemoryCache cache,
            ILogger<RoleClaimsTransformation> logger)
        {
            _userRolesClient = userRolesClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal?.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            {
                return principal;
            }

            var subject = principal.FindFirst("sub")?.Value ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(subject, out var userId))
            {
                // Client credential tokens have no user, so there are no roles to add
                return principal;
            }

            var roles = await GetRolesAsync(userId);

            foreach (var role in roles.Where(role => !principal.IsInRole(role)))
            {
                identity.AddClaim(new Claim(identity.RoleClaimType, role));
            }

            return principal;
        }

        private async Task<IEnumerable<string>> GetRolesAsync(Guid userId)
        {
            if (_cache.TryGetValue(userId, out IEnumerable<string> cached))
            {
                return cached;
            }

            try
            {
                var roles = (await _userRolesClient.GetRolesAsync(userId) ?? Enumerable.Empty<string>()).ToArray();
                _cache.Set(userId, roles, CacheDuration);
                return roles;
            }
            catch (Exception ex)
            {
                // Fail closed: the user stays authenticated but without roles
                _logger.LogError(ex, "Failed to retrieve roles for user {UserId}", userId);
                return Enumerable.Empty<string>();
            }
        }
    }
}
