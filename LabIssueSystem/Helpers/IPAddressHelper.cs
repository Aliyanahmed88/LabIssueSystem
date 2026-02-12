using Microsoft.AspNetCore.Http;
using System;

namespace LabIssueSystem.Helpers
{
    public static class IPAddressHelper
    {
        public static string GetClientIPAddress(HttpContext httpContext)
        {
            var request = httpContext.Request;
            
            // Check for forwarded headers (when behind proxy)
            var forwardedFor = request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For format: client, proxy1, proxy2
                var ip = forwardedFor.Split(',').First().Trim();
                if (!string.IsNullOrEmpty(ip))
                    return ip;
            }

            var realIP = request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIP))
                return realIP;

            // Fallback to UserHostAddress
            return request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        public static bool IsValidIPAddress(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;

            // Basic IPv4 validation
            var parts = ipAddress.Split('.');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int value))
                    return false;
                if (value < 0 || value > 255)
                    return false;
            }

            return true;
        }
    }
}
