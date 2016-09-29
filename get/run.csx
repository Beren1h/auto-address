using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
    return request.CreateResponse(HttpStatusCode.OK, primer);
}