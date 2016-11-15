using System.Net;
using System.Net.Http;
using System.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    var id = ConfigurationManager.AppSettings["SmartyKey"];
    var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
    var smarty = $"https://us-street.api.smartystreets.com/street-address?auth-id={id}&prefix={primer}";
    var google = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={primer}&type=address&key=AIzaSyAXJB9uEy1TU1uzGEJGJzfzkj1zSZtYJOI";
    
    using(var client = new HttpClient())
    {
        return await client.GetAsync(smarty);
    }
    
    return request.CreateResponse(HttpStatusCode.InternalServerError, "too bad so sad");
}