using System.Runtime.Serialization;

using UserActor.Interfaces;

namespace UserActor
{
	/// <summary>
	/// The 'state' for a user that gets replicated to other instances/replicas.
	/// </summary>
	[DataContract]
	internal class ActorState
	{
		public ActorState( UserProfile profile, string password )
		{
			Profile = profile;
			Password = password;
		}

		[DataMember]
		public UserProfile Profile { get; set; }

		// TODO: ensure that the password is protected (salted/hashed)
		[DataMember]
		public string Password { get; set; }
	}
}
