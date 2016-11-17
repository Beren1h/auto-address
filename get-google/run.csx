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
    public string description { get; set; }
}

public class ResultContainer
{
    public List<Result> Results { get; set; }
}

public class Result
{
    public string formatted_address { get; set; }
    public List<AddressComponent> address_components { get; set; }
}

public class AddressComponent
{
    public string short_name { get; set; }
    public string long_name { get; set; }
    public List<string> types { get; set; }
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

                 conversion.Suggestions.Add(SillyString(suggestion, hydrateP));

            }

            // foreach(var prediction in hydrate.Predictions)
            // {
            //     var googleP = $"https://maps.googleapis.com/maps/api/geocode/json?address={hydrate.Predictions[0].description}&key={geocodeId}";
            //     var responseP = await client.GetAsync(googleP);
            //     var contentP = await responseP.Content.ReadAsStringAsync();
            //     var hydrateP = JsonConvert.DeserializeObject<ResultContainer>(contentP);
            //     //conversion.Suggestions[0].text = hydrateP.Results[0].formatted_address;
            //     //conversion.Suggestions[0].street_line = SillyString();
            //     SillyString(hydrateP.Results, conversion);
            // }
            // return request.CreateResponse(HttpStatusCode.OK, conversion);

            if(hydrate.Predictions != null && hydrate.Predictions.Count == 1){
                var google2 = $"https://maps.googleapis.com/maps/api/geocode/json?address={hydrate.Predictions[0].description}&key={geocodeId}";
                var response2 = await client.GetAsync(google2);
                var content2 = await response2.Content.ReadAsStringAsync();
                var hydrate2 = JsonConvert.DeserializeObject<ResultContainer>(content2);

                conversion.Suggestions[0].text = hydrate2.Results[0].formatted_address;

                foreach (var component in hydrate2.Results[0].address_components)
                {
                    if (component.types.Count == 1 && component.types.Contains("post_code"))
                    {
                        conversion.Suggestions[0].zipcode = component.short_name;
                    }

                    if (component.types.Count == 2 && component.types.Contains("administrative_area_level_1") && component.types.Contains("political"))
                    {
                        conversion.Suggestions[0].state = component.short_name;
                    }

                    if (component.types.Count == 2 && component.types.Contains("locality") && component.types.Contains("political"))
                    {
                        conversion.Suggestions[0].city = component.short_name;
                    }

                    if (component.types.Count == 1 && component.types.Contains("street_number"))
                    {
                        conversion.Suggestions[0].primary_number = component.short_name;
                    }

                    if (component.types.Count == 1 && component.types.Contains("route"))
                    {
                        conversion.Suggestions[0].street_line = component.short_name;
                        var split = component.short_name.Split(' ');

                        switch (split.Length)
                        {
                            case 1:
                                conversion.Suggestions[0].street_name = split[0];
                                break;
                            case 2:
                                conversion.Suggestions[0].street_name = split[0];
                                conversion.Suggestions[0].street_suffix = split[1];
                                break;
                            case 3:
                                conversion.Suggestions[0].street_predirection = split[0];
                                conversion.Suggestions[0].street_name = split[1];
                                conversion.Suggestions[0].street_suffix = split[2];
                                break;
                        }
                    }
                }
                //return request.CreateResponse(HttpStatusCode.OK, conversion);
            }
            return request.CreateResponse(HttpStatusCode.OK, conversion);
        }
    }
    catch(Exception x){
        log.Error($"[ERROR] {x.Message}; {x.StackTrace}");
        return request.CreateResponse(HttpStatusCode.InternalServerError, x.Message);
    }
}


public static Suggestion SillyString(Suggestion suggestion, ResultContainer container)
{
    foreach(var result in container.Results)
    {  
        log.Info($"results = ");
        foreach(var component in result.address_components)
        {
            log.Info($"component = ");
            if(component.types.Count == 1 && component.types.Contains("post_code"))
            {
                suggestion.zipcode = component.short_name;
            }
        }
    }

    return suggestion;
    // foreach (var result in results)
    // {
    //     foreach(var component in result.address_components)
    //     {
    //         if (component.types.Count == 1 && component.types.Contains("post_code"))
    //         {
    //             conversion.Suggestions[0].zipcode = component.short_name;
    //         }

    //         if (component.types.Count == 2 && component.types.Contains("administrative_area_level_1") && component.types.Contains("political"))
    //         {
    //             conversion.Suggestions[0].state = component.short_name;
    //         }

    //         if (component.types.Count == 2 && component.types.Contains("locality") && component.types.Contains("political"))
    //         {
    //             conversion.Suggestions[0].city = component.short_name;
    //         }

    //         if (component.types.Count == 1 && component.types.Contains("street_number"))
    //         {
    //             conversion.Suggestions[0].primary_number = component.short_name;
    //         }

    //         if (component.types.Count == 1 && component.types.Contains("route"))
    //         {
    //             conversion.Suggestions[0].street_line = component.short_name;
    //             var split = component.short_name.Split(' ');

    //             switch (split.Length)
    //             {
    //                 case 1:
    //                     conversion.Suggestions[0].street_name = split[0];
    //                     break;
    //                 case 2:
    //                     conversion.Suggestions[0].street_name = split[0];
    //                     conversion.Suggestions[0].street_suffix = split[1];
    //                     break;
    //                 case 3:
    //                     conversion.Suggestions[0].street_predirection = split[0];
    //                     conversion.Suggestions[0].street_name = split[1];
    //                     conversion.Suggestions[0].street_suffix = split[2];
    //                     break;
    //             }
    //         }
    //     }
    // }



    //return suggestions;
}
