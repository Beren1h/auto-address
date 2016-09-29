using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    var primer = "";
    try
    {
        primer = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "primer", true) == 0).Value;
        return request.CreateResponse(HttpStatusCode.OK, primer);
    }
    catch(Exception ex)
    {
        log.Info($"{ex.Message}");
    }
    finally
    {
        return primer;
    }

}