namespace Core.ServicesContracts
{
    public interface IEmailSender
    {
        void SendEmail(string from, string to, string sub, string body);
    }
}
