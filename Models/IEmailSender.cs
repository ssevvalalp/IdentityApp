namespace IdentiyApp.Models
{
    public interface IEmailSender //abstract class     --- Uygulama interface'e bağlı olarak çalışır. interfacei hangi concrate versiyon üzerinden çalıştıracaksak onu program.cs'de cagırmamız gerekir. 
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
