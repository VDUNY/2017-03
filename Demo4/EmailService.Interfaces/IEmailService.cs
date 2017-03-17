using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Remoting;

namespace EmailService.Interfaces
{
	/// <summary>
	/// Defines the remote interface for interacting with the Email service.
	/// </summary>
    public interface IEmailService : IService
	{
		/// <summary>
		/// Sends an email to one or more users.
		/// </summary>
		/// <param name="fromAddress"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="toAddresses"></param>
		/// <returns></returns>
		Task<bool> SendEmail( string fromAddress, string subject, string body,  IEnumerable<string> toAddresses );
    }
}
