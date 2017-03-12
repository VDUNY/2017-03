using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailService.Interfaces
{
    public interface IEmailService
    {
		Task<bool> SendEmail( MailAddressCollection toAddresses, MailAddress fromAddress, string subject, string body );
    }
}
