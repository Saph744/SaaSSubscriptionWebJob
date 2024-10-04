using Newtonsoft.Json;
using SaaSFulfillmentApp.Data;
using SaaSFulfillmentApp.Models;
using SaaSFulfillmentApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Repository.Interface
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;
        private const string ApiVersion = "2018-08-31";

        public SubscriptionRepository(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        public async Task<List<SaaSSubscription>> GetAllSubscriptionsAsync()
        {
            var token = await _tokenService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestUri = $"https://marketplaceapi.microsoft.com/api/saas/subscriptions?api-version={ApiVersion}";

            HttpResponseMessage response = null;
            try
            {
                response = await _httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Content: {content}");
                    var subscriptionWrapper = JsonConvert.DeserializeObject<SaaSSubscriptionWrapper>(content);
                    var subscriptions = subscriptionWrapper?.Subscriptions;
                    if (subscriptions == null)
                    {
                        Console.WriteLine("Deserialization returned null.");
                    }
                    else if (subscriptions.Count == 0)
                    {
                        Console.WriteLine("Deserialization returned an empty list.");
                    }

                    return subscriptions;
                }
                else
                {
                    Console.WriteLine($"Failed with status code: {(int)response.StatusCode} - {response.ReasonPhrase}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error Content: {errorContent}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        token = await _tokenService.GetAccessTokenAsync();
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        response = await _httpClient.GetAsync(requestUri);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Response Content: {content}");
                            return JsonConvert.DeserializeObject<List<SaaSSubscription>>(content);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}");
            }

            Console.WriteLine("The API call was unsuccessful.");
            return null;
        }
    }
}
