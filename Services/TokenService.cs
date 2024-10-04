using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Services
{
    public class TokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private DateTime _tokenExpiration;

        public TokenService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken) || IsTokenExpired())
            {
                await FetchNewTokenAsync();
            }
            return _accessToken;
        }

        private async Task FetchNewTokenAsync()
        {
            try
            {
                var clientId = _configuration["SAASClientID"];
                var clientSecret = _configuration["SAASClientSecret"];
                var tenantId = _configuration["SAASTenantID"];
                var scope = _configuration["SAASResource"] + "/.default";

                var request = new HttpRequestMessage(HttpMethod.Post, $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token")
                {
                    Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("client_secret", clientSecret),
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("scope", scope)
                    })
                };

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Token Response Content: {responseContent}");

                response.EnsureSuccessStatusCode();

                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
                _accessToken = tokenResponse.AccessToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 300);
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Request error: {httpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        private bool IsTokenExpired()
        {
            return DateTime.UtcNow >= _tokenExpiration;
        }

        private class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}
