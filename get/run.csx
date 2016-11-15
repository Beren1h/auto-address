using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;

public class Thing
{
    public List<Suggestion> Suggestions { get; set; }
}

public class Suggestion
{
    public string text { get; set; }
    public string street_line { get; set; }
    public string city { get; set; }
    public string state { get; set; }
}

public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    var id = ConfigurationManager.AppSettings["SmartyAuthId"];
    var token = ConfigurationManager.AppSettings["SmartyAuthToken"];
    var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
    var smarty = $"https://us-autocomplete.api.smartystreets.com/suggest?auth-id={id}&auth-token={token}&prefix={primer}";
    
    using(var client = new HttpClient())
    {
        var test = await client.GetAsync(smarty);
        var content = await test.Content.ReadAsStringAsync();
        var test2 = JsonConvert.DeserializeObject<Thing>(content);
        log.Info(test2.Suggestions.Count);
        return test;
    }
    
    return request.CreateResponse(HttpStatusCode.InternalServerError, "too bad so sad");
}