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
        var which = ConfigurationManager.AppSettings["switch"];
        var url = ConfigurationManager.AppSettings[which];
        var final = $"{url}&primer={primer}";
        return await client.GetAsync(final);
    }
}