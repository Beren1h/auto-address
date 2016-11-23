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
        var streetLine = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "street", true) == 0).Value;
        var city = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "city", true) == 0).Value;
        var state = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "state", true) == 0).Value;
        //var smarty = $"https://us-autocomplete.api.smartystreets.com/suggest?auth-id={id}&auth-token={token}&prefix={primer}";
        
        using(var client = new HttpClient())
        {
            //for deploy
            //var response = await client.GetAsync(smarty);
            //var content = await response.Content.ReadAsStringAsync();
            //var hydrate = JsonConvert.DeserializeObject<SuggestionContainer>(content);

                var smarty2 = $"https://us-street.api.smartystreets.com/street-address?auth-id={id}&auth-token={token}&canidates=10&street={streetLine}&city={city}&state={state}";
                var response2 = await client.GetAsync(smarty2);
                var content2 = await response2.Content.ReadAsStringAsync();
                var hydrate2 = JsonConvert.DeserializeObject<List<Verification>>(content2);
                var verified = new SuggestionContainer{
                    Suggestions = new List<Suggestion>{
                        new Suggestion{
                            text = streetLine,
                            zipcode = hydrate2[0].components.zipcode,
                            primary_number = hydrate2[0].components.primary_number,
                            city = hydrate2[0].components.city_name,
                            zipcode = hydrate2[0].components.zipcode,
                            state = hydrate2[0].components.state_abbreviation,
                            street_name = hydrate2[0].components.street_name,
                            street_suffix = hydrate2[0].components.street_suffix,
                            street_predirection = hydrate2[0].components.street_predirection
                        }
                    }
                };
                //hydrate.Suggestions[0].text = streetLine;
                //hydrate.Suggestions[0].zipcode = hydrate2[0].components.zipcode;
                //hydrate.Suggestions[0].primary_number = hydrate2[0].components.primary_number;
                //hydrate.Suggestions[0].street_name = hydrate2[0].components.street_name;
                //hydrate.Suggestions[0].street_suffix = hydrate2[0].components.street_suffix;
                //hydrate.Suggestions[0].street_predirection = hydrate2[0].components.street_predirection;

            return request.CreateResponse(HttpStatusCode.OK, verified);
        }
    }
    catch(Exception x){
        log.Error($"[ERROR] {x.Message}; {x.StackTrace}");
        return request.CreateResponse(HttpStatusCode.InternalServerError, x.Message);
    }
}