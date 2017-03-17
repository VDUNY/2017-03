using System.Collections.Generic;
using System.Net.Mail;
using System.Runtime.Serialization;

namespace EmailService
{
	/// <summary>Represents the various attributes of an email.</summary>
	[DataContract]
	internal class Email
	{
		/// <summary>Adress of who the email was sent from.</summary>
		[DataMember]
		public string From { get; set; }

		/// <summary>Subject line of the email.</summary>
		[DataMember]
		public string Subject { get; set; }

		/// <summary>Main content of the message.</summary>
		[DataMember]
		public string Body { get; set; }

		/// <summary>The list of recipients.</summary>
		[DataMember]
		public IEnumerable<string> ToList { get; set; }


		/// <summary>
		/// Initializes a new email instance.
		/// </summary>
		/// <param name="fromAddress"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="toAddresses"></param>
		public Email( string fromAddress, string subject, string body, IEnumerable<string> toAddresses )
		{
			From = fromAddress;
			Subject = subject;
			Body = body;
			ToList = toAddresses;
		}
	}
}
