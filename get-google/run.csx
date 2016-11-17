#load "..\shared\models.csx"

using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;

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
            var conversion = new SuggestionContainer{
                Suggestions = new List<Suggestion>()
            };
            foreach (var prediction in hydrate.Predictions)
            {
                var googleP = $"https://maps.googleapis.com/maps/api/geocode/json?address={hydrate.Predictions[0].description}&key={geocodeId}";
                var responseP = await client.GetAsync(googleP);
                var contentP = await responseP.Content.ReadAsStringAsync();
                var hydrateP = JsonConvert.DeserializeObject<ResultContainer>(contentP);
                var suggestion = new Suggestion{
                     text = prediction.description
                 };

                 conversion.Suggestions.Add(AddMoreBits(suggestion, hydrateP));
            }
            return request.CreateResponse(HttpStatusCode.OK, conversion);
        }
    }
    catch(Exception x){
        log.Error($"[ERROR] {x.Message}; {x.StackTrace}");
        return request.CreateResponse(HttpStatusCode.InternalServerError, x.Message);
    }
}

public static Suggestion AddMoreBits(Suggestion suggestion, ResultContainer container)
{
    foreach(var result in container.Results)
    {  
        foreach(var component in result.address_components)
        {
            if(component.types.Count == 1 && component.types.Contains("postal_code"))
            {
                suggestion.zipcode = component.short_name;
            }

            if (component.types.Count == 2 && component.types.Contains("administrative_area_level_1") && component.types.Contains("political"))
            {
                suggestion.state = component.short_name;
            }

            if (component.types.Count == 2 && component.types.Contains("locality") && component.types.Contains("political"))
            {
                suggestion.city = component.short_name;
            }

            if (component.types.Count == 1 && component.types.Contains("street_number"))
            {
                suggestion.primary_number = component.short_name;
            }

            if (component.types.Count == 1 && component.types.Contains("route"))
            {
                suggestion.street_line = component.short_name;
                var split = component.short_name.Split(' ');

                switch (split.Length)
                {
                    case 1:
                        suggestion.street_name = split[0];
                        break;
                    case 2:
                        suggestion.street_name = split[0];
                        suggestion.street_suffix = split[1];
                        break;
                    case 3:
                        suggestion.street_predirection = split[0];
                        suggestion.street_name = split[1];
                        suggestion.street_suffix = split[2];
                        break;
                }
            }


        }
    }

    return suggestion;

}
