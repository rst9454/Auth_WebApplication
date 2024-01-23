namespace Auth_WebApplication.Repostory.Interface
{
    public interface IEmailSender
    {
        Task<bool> EmailSendAsync(string email,string Subject,string message);
    }
}
