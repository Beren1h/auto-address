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
        var smarty = $"https://us-autocomplete.api.smartystreets.com/suggest?auth-id={id}&auth-token={token}&prefix={primer}";
        
        using(var client = new HttpClient())
        {
            var response = await client.GetAsync(smarty);
            var content = await response.Content.ReadAsStringAsync();
            var hydrate = JsonConvert.DeserializeObject<SuggestionContainer>(content);

            if(hydrate.Suggestions != null && hydrate.Suggestions.Count == 1){
                var smarty2 = $"https://us-street.api.smartystreets.com/street-address?auth-id={id}&auth-token={token}&canidates=10&street={hydrate.Suggestions[0].street_line}&city={hydrate.Suggestions[0].city}&state={hydrate.Suggestions[0].state}";
                var response2 = await client.GetAsync(smarty2);
                var content2 = await response2.Content.ReadAsStringAsync();
                var hydrate2 = JsonConvert.DeserializeObject<List<Verification>>(content2);
                hydrate.Suggestions[0].zipcode = hydrate2[0].components.zipcode;
                hydrate.Suggestions[0].primary_number = hydrate2[0].components.primary_number;
                hydrate.Suggestions[0].street_name = hydrate2[0].components.street_name;
                hydrate.Suggestions[0].street_suffix = hydrate2[0].components.street_suffix;
                hydrate.Suggestions[0].street_predirection = hydrate2[0].components.street_predirection;
            }
            return request.CreateResponse(HttpStatusCode.OK, hydrate);
        }
    }
    catch(Exception x){
        log.Error($"[ERROR] {x.Message}; {x.StackTrace}");
        return request.CreateResponse(HttpStatusCode.InternalServerError, x.Message);
    }
}