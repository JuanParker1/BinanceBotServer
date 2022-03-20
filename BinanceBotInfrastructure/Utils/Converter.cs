using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Net.Http;
using System.Security.Cryptography;

namespace BinanceBotInfrastructure.Utils
{
    public static class Converter
    {
        public static IDictionary<string, string> ToDictionary<T>(T dto,
            IEnumerable<string> paramsToRemove)
        {
            var resultDict = new Dictionary<string, string>
            {
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var typeFieldsNames = dto.GetType().GetMembers()
                .Where(m => m.MemberType == MemberTypes.Property)
                .Select(f => f.Name);

            foreach (var name in typeFieldsNames)
            {
                var camelCasedKey = char.ToLower(name[0]) + name[1..];
                var value = dto.GetType()?.GetProperty(name)?.GetValue(dto);

                if (string.IsNullOrEmpty($"{value}") || Verifier.IsNumericDefault(value))
                    continue;
                
                resultDict.Add(camelCasedKey, $"{value}");
            }

            foreach (var removeParam in paramsToRemove)
                resultDict.Remove(removeParam);

            return resultDict;
        }
        
        public static string ToParamsString(IDictionary<string, string> qParams = default)
        {
            var queryString = string.Empty;

            if (qParams == default || !qParams.Any()) 
                return queryString;
            
            queryString = string.Join("&", qParams.Select(kvp =>
                $"{kvp.Key}={kvp.Value}"));

            return queryString;
        }
        
        public static FormUrlEncodedContent ToUrlEncodedContent(
            IDictionary<string, string> qParams = default)
        {
            qParams ??= new Dictionary<string, string>();
            
            var content = new FormUrlEncodedContent(qParams);
            return content;
        }
        
        public static Uri ToValidUri(Uri endpoint, IDictionary<string, string> qParams = default)
        {
            var queryString = Converter.ToParamsString(qParams);
            var resultQueryString = $"{endpoint}";
            
            if(!string.IsNullOrEmpty(queryString))
                resultQueryString = $"{endpoint}?{queryString}";
            
            var uri = new Uri(resultQueryString);
            return uri;
        }

        public static Uri ToValidSignedUri(Uri endpoint, string secretKey, 
            IDictionary<string, string> qParams = default)
        {
            if (qParams == default)
                return endpoint;
            
            var queryParamsString = Converter.ToParamsString(qParams);
            var hmacResult = ToHmacSignature(secretKey, queryParamsString);
            
            var resultUri = new Uri($"{endpoint.AbsoluteUri}?" +
                                    $"{queryParamsString}&signature={hmacResult}");
            return resultUri;
        }
        
        public static string ToHmacSignature(string secretKey, string totalParams)
        {
            var messageBytes = Encoding.UTF8.GetBytes(totalParams);
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var hash = new HMACSHA256(keyBytes);
            var computedHash = hash.ComputeHash(messageBytes);
            
            return BitConverter.ToString(computedHash)
                .Replace("-", "").ToLower();
        }
    }
}