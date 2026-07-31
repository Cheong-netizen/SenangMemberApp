using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace SenangMemberApp.Services
{
    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            if (string.IsNullOrWhiteSpace(jwt)) return claims;

            var parts = jwt.Split('.');
            if (parts.Length < 2) return claims;

            var jsonBytes = ParseBase64WithoutPadding(parts[1]);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
                }
            }
            return claims;
        }

        public static DateTime? GetTokenExpiration(string? jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
                return null;

            try
            {
                var parts = jwt.Split('.');
                if (parts.Length < 2)
                    return null;

                var jsonBytes = ParseBase64WithoutPadding(parts[1]);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                if (keyValuePairs != null && keyValuePairs.TryGetValue("exp", out var expObj))
                {
                    if (long.TryParse(expObj.ToString(), out long expSeconds))
                    {
                        return DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public static bool IsTokenExpired(string? jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
                return true;

            var expiration = GetTokenExpiration(jwt);
            if (!expiration.HasValue)
            {
                return false;
            }

            return expiration.Value <= DateTime.UtcNow;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
