using System.Runtime.Serialization;

namespace UserActor.Interfaces
{
	/// <summary>
	/// The public profile data for a user that can be used by other services.
	/// </summary>
	[DataContract]
	public class UserProfile
	{
		[DataMember]
		public string Email { get; set; }

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string LastName { get; set; }
	}
}
