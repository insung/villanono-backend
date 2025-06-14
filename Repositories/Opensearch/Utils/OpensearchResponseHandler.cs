using System.Net;

internal static class OpensearchResponseHandler
{
    internal static void CheckResponseFailed(
        int? httpStatusCode,
        string? debugInformation,
        string failedMessage
    )
    {
        if (
            !tryGetElasticSearchApiResponseCode(
                httpStatusCode,
                debugInformation,
                out HttpStatusCode responseCode,
                out Exception? innerException
            )
        )
        {
            throw new HttpRequestException(failedMessage, innerException, responseCode);
        }
    }

    private static bool tryGetElasticSearchApiResponseCode(
        int? httpStatusCode,
        string? debugInformation,
        out HttpStatusCode responseCode,
        out Exception? innerException
    )
    {
        innerException = null;
        var apiCallResponseCode = httpStatusCode ?? 500;
        responseCode = (HttpStatusCode)apiCallResponseCode;
        if (responseCode != HttpStatusCode.OK)
        {
            innerException = new Exception(debugInformation ?? "");
        }
        return responseCode == HttpStatusCode.OK;
    }
}
