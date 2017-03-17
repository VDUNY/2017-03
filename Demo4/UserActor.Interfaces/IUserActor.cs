using System.Threading.Tasks;

using Microsoft.ServiceFabric.Actors;

namespace UserActor.Interfaces
{
	/// <summary>
	/// This interface defines the methods exposed by an actor.
	/// Clients use this interface to interact with the actor that implements it.
	/// </summary>
	public interface IUserActor : IActor
	{
		/// <summary>
		/// Creates a new user instance.
		/// </summary>
		/// <param name="firstName">The users first name.</param>
		/// <param name="lastName">The users last name.</param>
		/// <param name="password">The users password.</param>
		/// <returns>
		/// true if the user was created successfully; false otherwise.
		/// </returns>
		Task<bool> Create( string firstName, string lastName, string password );

		/// <summary>
		/// Gets the user profile.
		/// </summary>
		/// <returns>
		/// The users profile data.
		/// </returns>
		Task<UserProfile> GetProfile();

		/// <summary>
		/// Validates that the password supplied by a user matches the password 'on file'.
		/// </summary>
		/// <param name="password">The password supplied by the user attempting to login.</param>
		/// <returns>
		/// true if the password is valid; false otherwise.
		/// </returns>
		Task<bool> ValidatePassword( string password );

		/// <summary>
		/// Changes the users password, provided that the <paramref name="oldPassword"/> value
		/// matches the current password.
		/// </summary>
		/// <param name="oldPassword">The users current password.</param>
		/// <param name="newPassword">The new password to use.</param>
		/// <returns>
		/// true if the password has been successfully changed; false otherwise.
		/// </returns>
		Task<bool> ChangePassword( string oldPassword, string newPassword );
	}
}
