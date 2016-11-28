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
        var smarty = $"https://us-autocomplete.api.smartystreets.com/suggest?auth-id={id}&auth-token={token}&candidates=5&prefix={primer}";
        
        using(var client = new HttpClient())
        {
            var response = await client.GetAsync(smarty);
            var content = await response.Content.ReadAsStringAsync();
            var hydrate = JsonConvert.DeserializeObject<SuggestionContainer>(content);
            return request.CreateResponse(HttpStatusCode.OK, hydrate);
        }
    }
    catch(Exception x){
        log.Error($"[ERROR] {x.Message}; {x.StackTrace}");
        return request.CreateResponse(HttpStatusCode.InternalServerError, x.Message);
    }
}