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
    public string zipcode { get; set; }
    public string primary_number { get; set; }
    public string street_predirection { get; set;}
    public string street_name { get; set; }
    public string street_suffix { get; set; }

}

public class Verification
{
    public string delivery_line_1 { get; set; }
    public string last_line { get; set; }
    public VerificationComponent components { get; set; }
}

public class VerificationComponent
{
    public string zipcode { get; set; }
    public string primary_number { get; set; }
    public string street_predirection { get; set;}
    public string street_name { get; set; }
    public string street_suffix { get; set; }
}


public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    try{
        var test = new Exception("test exception");
        log.Error(test);
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
        log.Error(x.Message);
        return request.CreateResponse(HttpStatusCode.InternalServerError, x.Message);
    }
}