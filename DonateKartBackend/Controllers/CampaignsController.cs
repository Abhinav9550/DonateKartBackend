using DonateKartBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DonateKartBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CampaignsController
    {

        [HttpGet]
        public async Task<IActionResult> getCampaigns()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://testapi.donatekart.com/api/campaign");
                    var response1 = await client.SendAsync(request);
                    if (response1.IsSuccessStatusCode)
                    {
                        var responseStream = await response1.Content.ReadAsStreamAsync();
                        StreamReader reader = new StreamReader(responseStream);
                        string text = reader.ReadToEnd();
                        var resp = JsonConvert.DeserializeObject<List<Campaigns_Api_Response>>(text);
                        return new OkObjectResult(resp.Select(c=>new { c.backersCount, c.title ,c.totalAmount, c.endDate}).ToList().OrderByDescending(c => c.totalAmount));
                    }
                    else
                        return new BadRequestObjectResult("Call to api failed");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        [HttpGet]
        [Route("{type}")]
        public async Task<IActionResult> getCampaignsByInput(string type)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://testapi.donatekart.com/api/campaign");
                    var response1 = await client.SendAsync(request);
                    if (response1.IsSuccessStatusCode)
                    {
                        var responseStream = await response1.Content.ReadAsStreamAsync();
                        StreamReader reader = new StreamReader(responseStream);
                        string text = reader.ReadToEnd();
                        var resp = JsonConvert.DeserializeObject<List<Campaigns_Api_Response>>(text);
                        if (type == "active")
                            resp = resp.Where(c => c.endDate >= DateTime.UtcNow && c.created != null && ((DateTime.UtcNow  - c.created )?.TotalDays ?? 0) <= 30).ToList();
                        else if (type == "closed")
                            resp = resp.Where(c => c.endDate < DateTime.UtcNow && c.procuredAmount >= c.totalAmount).ToList();
                        else
                            return new OkObjectResult("please enter either 'active' or 'closed' instead of => " + type);
                        return new OkObjectResult(resp);

                    }
                    else
                        return new BadRequestObjectResult("Call to api failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
