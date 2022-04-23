using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Azure.Data.Tables;
using Azure.Identity;

namespace LocationService
{
    [Route("[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {

        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<LocationController> _logger;
        private readonly IConfiguration _config;

        public LocationController(ILogger<LocationController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public IEnumerable<Location> Get()
        {
            return Enumerable.Range(1, 1).Select(index => new Location
            {
                longitude = "52.07963511282511",
                latitude= ", 4.86157988871688"
            }
                ).ToArray();
            
        }

        [HttpPost]
        public async Task<IEnumerable<Location>> PostAsync([FromBody] InputAddress address)
        {
            var key = _config["OpenCageKey"];
            var data = HttpUtility.UrlEncode(address.address);
            var url = "https://api.opencagedata.com/geocode/v1/json?key="+ key + "&pretty=1&q=" + data;

            var response =  await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<JObject>(response);
            
            var resList = res["results"];
            
            var lat = resList[0]["bounds"]["northeast"]["lat"].Value<string>();
            var lng = resList[0]["bounds"]["northeast"]["lng"].Value<string>();

            var result = new Location();
            result.longitude = lng;
            result.latitude = lat;
            StoreResultInTable(result);
            return Enumerable.Range(1, 1).Select(index => result
                ).ToArray();
        }

        private void StoreResultInTable(Location result)
        {
            var uri = new Uri("https://locationstoragemli.table.core.windows.net");
            var tableName = "locationresponses";
            var SASConnectionString = "BlobEndpoint=https://locationstoragemli.blob.core.windows.net/;QueueEndpoint=https://locationstoragemli.queue.core.windows.net/;FileEndpoint=https://locationstoragemli.file.core.windows.net/;TableEndpoint=https://locationstoragemli.table.core.windows.net/;SharedAccessSignature=sv=2020-08-04&ss=bfqt&srt=o&sp=rwdlacupitfx&se=2022-04-23T22:27:01Z&st=2022-04-23T14:27:01Z&spr=https&sig=3ThYP9dScUxzKVfFFDERWQaITRh2o0%2FhiQ%2Bq8OEjMiA%3D";
            // TableUriBuilder tb =  new TableUriBuilder(new Uri("https://locationstoragemli.table.core.windows.net/locationresponses"));

            TableClient tc;
            
            tc = new TableClient(uri, tableName, new DefaultAzureCredential());
            // tc = new TableClient(SASConnectionString, tableName);
            var entity = new TableEntity("locationresult",  
               Guid.NewGuid().ToString())
            {
                { "Timestamp", DateTime.Now.ToString("yyyyMMddHHmmss")},
                { "lng", result.longitude },
                { "lat", result.latitude}
            };

            tc.AddEntity(entity);
        }
    }
}
