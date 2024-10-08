using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var subscriptions = new List<SaaSSubscription>();
            string nextLink = requestUri;

            do
            {
                HttpResponseMessage response = null;
                try
                {
                    response = await _httpClient.GetAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response Content: {content}");
                        var subs = JsonConvert.DeserializeObject(content);
                        nextLink = GetNextLink((JObject)subs);

                        var subscriptionWrapper = JsonConvert.DeserializeObject<SaaSSubscriptionWrapper>(content);

                        if (subscriptionWrapper != null)
                        {
                            if (subscriptionWrapper.Subscriptions != null && subscriptionWrapper.Subscriptions.Any())
                            {
                                subscriptions.AddRange(subscriptionWrapper.Subscriptions);
                            }

                            //nextLink = subscriptionWrapper.NextLink;
                            if (string.IsNullOrEmpty(nextLink))
                            {
                                Console.WriteLine("NextLink is null or empty, exiting the loop.");
                            }
                            else
                            {
                                Console.WriteLine($"NextLink: {nextLink}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Deserialization returned null.");
                            break;
                        }
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
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e.Message}");
                    break;
                }
            } while (!string.IsNullOrEmpty(nextLink));

            return subscriptions;
        }
        private static string GetNextLink(JObject pjobjResult)
        {
            JProperty jtLastToken = (JProperty)pjobjResult.Last;

            if (jtLastToken.Name.Equals(@"@nextLink"))
            {
                return jtLastToken.Value.ToString();
            } 
            else
            {
                return "";
            }
        }
    }
}
