using System.Net;
using System.Net.Http;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    //AIzaSyAXJB9uEy1TU1uzGEJGJzfzkj1zSZtYJOI
    //log.Info($"C# HTTP trigger function processed a request. RequestUri={request.RequestUri}");

    // parse query parameter
    //string primer = reqquest.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
    var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;

    return primer;

    //HttpResponseMessage response;

    //private var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={primer}&type=address&key=AIzaSyAXJB9uEy1TU1uzGEJGJzfzkj1zSZtYJOI";
    //using (var client = new HttpClient())
    //{
    //    response = await client.GetAsync(url);
    //};

    // Get request body
    //dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    //name = name ?? data?.name;

    //return name == null
    //    ? request.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
    //    : request.CreateResponse(HttpStatusCode.OK, "Hello " + name);
}