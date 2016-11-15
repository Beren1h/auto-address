using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;

public class SuggestionContainer
{
    public List<Suggestion> Suggestions { get; set; }
}

public class Suggestion
{
    public string text { get; set; }
    public string street_line { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string zip { get; set; }
}

public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    var id = ConfigurationManager.AppSettings["SmartyAuthId"];
    var token = ConfigurationManager.AppSettings["SmartyAuthToken"];
    var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
    var smarty = $"https://us-autocomplete.api.smartystreets.com/suggest?auth-id={id}&auth-token={token}&prefix={primer}";
    
    using(var client = new HttpClient())
    {
        var response = await client.GetAsync(smarty);
        var content = await response.Content.ReadAsStringAsync();
        var hydrate = JsonConvert.DeserializeObject<SuggestionContainer>(content);

        if(hydrate.Suggestions.Count == 1){
            return request.CreateResponse(HttpStatusCode.OK, "There can be only one!!!")
        }

        var final = JsonConvert.SerializeObject(hydrate);
        return request.CreateResponse(HttpStatusCode.OK, final)

    }
    
    return request.CreateResponse(HttpStatusCode.InternalServerError, "too bad so sad");
}