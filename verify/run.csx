#load "..\shared\models.csx"

using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    try{
        var id = ConfigurationManager.AppSettings["SmartyAuthId"];
        var token = ConfigurationManager.AppSettings["SmartyAuthToken"];
        var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
        
        using(var client = new HttpClient())
        {
            var smarty2 = $"https://us-street.api.smartystreets.com/street-address?auth-id={id}&auth-token={token}&canidates=1&street={primer}";
            var response2 = await client.GetAsync(smarty2);
            var content2 = await response2.Content.ReadAsStringAsync();
            var hydrate2 = JsonConvert.DeserializeObject<List<Verification>>(content2);
            foreach(var verification in hydrate2){
                log.Inof(verification.GetType());
            }
            return request.CreateResponse(HttpStatusCode.OK, hydrate2);
        }
    }
    catch(Exception x){
        log.Error($"[ERROR] {x.Message}; {x.StackTrace}");
        return request.CreateResponse(HttpStatusCode.InternalServerError, x.Message);
    }
}