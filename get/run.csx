using System.Net;
using System.Net.Http;
using System.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    var id = ConfigurationManager.AppSettings["SmartyAuthId"];
    var token = ConfigurationManager.AppSettings["SmartyAuthToken"];
    var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
    var smarty = $"https://us-street.api.smartystreets.com/street-address?auth-id={id}&auth-token={token}prefix={primer}";
    log.Info(smarty);
    using(var client = new HttpClient())
    {
        return await client.GetAsync(smarty);
    }
    
    return request.CreateResponse(HttpStatusCode.InternalServerError, "too bad so sad");
}