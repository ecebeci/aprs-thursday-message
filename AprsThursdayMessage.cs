using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Aprs.Message;

public class AprsThursdayMessage
{
    private static readonly HttpClient httpClient = new();

    [Function(nameof(AprsThursdayMessage) + "Run")]
    public async Task TriggerTimer([TimerTrigger("0 12 * * 4")] TimerInfo timerInfo, FunctionContext context)
    {
        await Execute(context);
    }

    [Function((nameof(AprsThursdayMessage) + "TriggerManually"))]
    public async Task<HttpResponseData> TriggerManually(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext context)
    {
        var logger = context.GetLogger("TriggerThursdayMessage");
        logger.LogInformation("Manual trigger received.");

        await Execute(context);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Manual trigger executed.");
        return response;
    }

    private static async Task Execute(FunctionContext context)
    {
        var logger = context.GetLogger(nameof(AprsThursdayMessage));
        var url = Environment.GetEnvironmentVariable("APRS_SERVER_URL") ?? throw new InvalidOperationException("APRS_SERVER_URL is not configured.");
        var callsign = Environment.GetEnvironmentVariable("CALLSIGN") ?? throw new InvalidOperationException("CALLSIGN is not configured.");
        var passcode = Environment.GetEnvironmentVariable("PASSCODE") ?? throw new InvalidOperationException("PASSCODE is not configured.");
        var version = Environment.GetEnvironmentVariable("VERSION") ?? throw new InvalidOperationException("VERSION is not configured.");
        var aprsThursdayMessageTo = Environment.GetEnvironmentVariable("APRS_THURSDAY_MESSAGE_TO") ?? throw new InvalidOperationException("APRS_THURSDAY_MESSAGE_TO is not configured.");
        var aprsThursdayMessageText = Environment.GetEnvironmentVariable("APRS_THURSDAY_MESSAGE_TEXT") ?? throw new InvalidOperationException("APRS_THURSDAY_MESSAGE_TEXT is not configured.");

        var loginLine = $"user {callsign} pass {passcode} vers {version}\n";
        if (aprsThursdayMessageTo.Length < 9)
        {
            aprsThursdayMessageTo = aprsThursdayMessageTo.PadRight(9, ' ');
        }

        var message = $"{callsign}>APRS::{aprsThursdayMessageTo}:{aprsThursdayMessageText}\n";

        var fullPayload = loginLine + message;
        var payloadBytes = Encoding.ASCII.GetBytes(fullPayload);

        var content = new ByteArrayContent(payloadBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        try
        {
            var response = await httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Message sent successfully, status: {StatusCode}, response: {ResponseBody}", response.StatusCode, responseBody);
            }
            else
            {
                logger.LogError("Failed to send message, status: {StatusCode}, response: {ResponseBody}", response.StatusCode, responseBody);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while sending message.");
        }
    }
}