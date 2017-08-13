namespace ForeignExchangeWin1.Services
{
    using System.Collections.Generic;
    using Models;
    using System;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    public class ApiService
    {
        public async Task<Response> GetRates()
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://apiexchangerates.azurewebsites.net");
                var controller = "/api/Rates";
                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();
                if(!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var list = JsonConvert.DeserializeObject<List<Rate>>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
