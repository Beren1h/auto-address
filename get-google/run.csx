using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;

public class PredictionContainer
{
    public List<Prediction> Predictions { get; set; }
}

public class Prediction
{
    public string descrption { get; set; }
}

public class ResultContainer
{
    public List<Result> Results { get; set; }
}

public class Result
{
    public List<AddressComponent> AddressComponents { get; set; }
}

public class AddressComponent
{
    public string short_name { get; set; }
    public string long_name { get; set; }
    public List<string> terms { get; set; }
}

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
        var placeId = ConfigurationManager.AppSettings["googlePlaceKey"];
        var geocodeId = ConfigurationManager.AppSettings["googleGeoCodeKey"];
        var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
        var google = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={primer}&type=address&key={placeId}";
        
        using(var client = new HttpClient())
        {
            var response = await client.GetAsync(google);
            var content = await response.Content.ReadAsStringAsync();
            var hydrate = JsonConvert.DeserializeObject<PredictionContainer>(content);
            // if(hydrate.Suggestions != null && hydrate.Suggestions.Count == 1){
            //     var smarty2 = $"https://us-street.api.smartystreets.com/street-address?auth-id={id}&auth-token={token}&canidates=10&street={hydrate.Suggestions[0].street_line}&city={hydrate.Suggestions[0].city}&state={hydrate.Suggestions[0].state}";
            //     var response2 = await client.GetAsync(smarty2);
            //     var content2 = await response2.Content.ReadAsStringAsync();
            //     var hydrate2 = JsonConvert.DeserializeObject<List<Verification>>(content2);
            //     hydrate.Suggestions[0].zipcode = hydrate2[0].components.zipcode;
            //     hydrate.Suggestions[0].primary_number = hydrate2[0].components.primary_number;
            //     hydrate.Suggestions[0].street_name = hydrate2[0].components.street_name;
            //     hydrate.Suggestions[0].street_suffix = hydrate2[0].components.street_suffix;
            //     hydrate.Suggestions[0].street_predirection = hydrate2[0].components.street_predirection;
            // }
            return request.CreateResponse(HttpStatusCode.OK, hydrate);
        }
    }
    catch(Exception x){
        log.Error($"[ERROR] {x.Message}; {x.StackTrace}");
        return request.CreateResponse(HttpStatusCode.InternalServerError, x.Message);
    }
}