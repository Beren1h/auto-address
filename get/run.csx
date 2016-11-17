using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage request, TraceWriter log)
{
    var primer = request.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;

    using(var client = new HttpClient())
    {
        var url = $"{ConfigurationManager.AppSettings["googleUrl"]}?primer={primer}";
        log.Info(url);
        return await client.GetAsync(url);
    }
}