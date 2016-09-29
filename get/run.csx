using System.Net;
using System.Net.Http;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
    var google = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={primer}&type=address&key=AIzaSyAXJB9uEy1TU1uzGEJGJzfzkj1zSZtYJOI";
    
    using(var client = new HttpClient())
    {
        return await client.GetAsync(google);
    }
    
    return request.CreateResponse(HttpStatusCode.InternalServerError, "too bad so sad");
}