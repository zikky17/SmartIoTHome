using Azure;
using Azure.Communication.Email;
namespace SharedResources.Communication;

public class EmailCommunication
{

    private readonly EmailClient _client;
    private readonly string _fromAddress;

    public EmailCommunication(string connectionString = "endpoint=https://gurras-cs.europe.communication.azure.com/;accesskey=5JlLR28TmgV5apDmLYMdOI4bbzvBerRMHO9UCcz0x5NtZVyRh2SKJQQJ99AIACULyCpeJNWdAAAAAZCSzOeH", string fromAddress = "DoNotReply@57aeddb5-21a3-4998-9b51-e7734f344d5c.azurecomm.net")
    {
        _client = new EmailClient(connectionString);
        _fromAddress = fromAddress;
    }

    public void Send(string toAddress, string subject, string body, string plainText)
    {
        EmailSendOperation emailSendOperation = _client.Send(
            WaitUntil.Completed,
            senderAddress: _fromAddress,
            recipientAddress: toAddress,
            subject: subject,
            htmlContent: body,
            plainTextContent: plainText);
    }
}
