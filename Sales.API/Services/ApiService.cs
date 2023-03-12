﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sales.Shared.Responses;

namespace Sales.API.Services
{
      public class ApiService : IApiService
        {
            private readonly IConfiguration _configuration;
            private readonly string _urlBase;
            private readonly string _tokenName;
            private readonly string _tokenValue;
            public ApiService(IConfiguration configuration)
            {
                _configuration = configuration;
                _urlBase = _configuration["CountriesAPI:urlBase"]!;
                _tokenName = _configuration["CountriesAPI:tokenName"]!;
                _tokenValue = _configuration["CountriesAPI:tokenValue"]!;
            }

        public async Task<Response>GetListAsync<T>(string servicePrefix, string controller)
        {
                try
                {
                    HttpClient client = new()
                    {
                        BaseAddress = new Uri(_urlBase),
                    };

                    client.DefaultRequestHeaders.Add(_tokenName, _tokenValue);
                    string url = $"{servicePrefix}{controller}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    string result = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        return new Response
                        {
                            IsSucess = true,
                            Message = result,
                        };
                    }

                    List<T> list = JsonConvert.DeserializeObject<List<T>>(result)!;
                    return new Response
                    {
                        IsSucess = true,
                        Result = list,
                    };
                }
                catch (Exception ex)
                {
                    return new Response
                    {
                        IsSucess = false,
                        Message = ex.Message,
                    };
                }
        }
      }
 
}