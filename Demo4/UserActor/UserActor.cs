using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;

using UserActor.Interfaces;

namespace UserActor
{
	/// <remarks>
	/// This class represents an actor.
	/// Every ActorID maps to an instance of this class.
	/// The StatePersistence attribute determines persistence and replication of actor state:
	///  - Persisted: State is written to disk and replicated.
	///  - Volatile: State is kept in memory only and replicated.
	///  - None: State is kept in memory only and not replicated.
	/// </remarks>
	[StatePersistence( StatePersistence.Persisted )]
	internal class UserActor : Actor, IUserActor
	{
		private static readonly string STATE_KEY = "UserState";

		/// <summary>
		/// Initializes a new instance of UserActor
		/// </summary>
		/// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService
		///		that will host this actor instance.</param>
		/// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
		public UserActor( ActorService actorService, ActorId actorId )
			: base( actorService, actorId )
		{
		}

		/// <summary>
		/// This method is called whenever an actor is activated.
		/// An actor is activated the first time any of its methods are invoked.
		/// </summary>
		protected override async Task OnActivateAsync()
		{
			ActorEventSource.Current.ActorMessage( this, "Actor activated." );

			// The StateManager is this actor's private state store.
			// Data stored in the StateManager will be replicated for high-availability for
			// actors that use volatile or persisted state storage.
			// Any serializable object can be saved in the StateManager.
			// For more information, see https://aka.ms/servicefabricactorsstateserialization

			// intialize the actor state if not already initialized
			var state = await StateManager.TryGetStateAsync<ActorState>( STATE_KEY );
			if ( !state.HasValue )
			{
				var initialState = new ActorState( new UserProfile(), string.Empty );
				await SetActorStateAsync( initialState );
			}
		}

		#region IUserActor implementation

		/// <summary>
		/// Creates a new user instance.
		/// </summary>
		/// <param name="firstName">The users first name.</param>
		/// <param name="lastName">The users last name.</param>
		/// <param name="password">The users password.</param>
		/// <returns>
		/// true if the user was created successfully; false otherwise.
		/// </returns>
		public async Task<bool> Create( string firstName, string lastName, string password )
		{
			var state = await GetActorStateAsync();

			if ( !string.IsNullOrEmpty( state.Profile.FirstName ) || !string.IsNullOrEmpty( state.Profile.LastName ) )
			{
				ActorEventSource.Current.ActorMessage( this, "Actor already exists!" );
				return false;
			}

			state.Profile.Email = Id.GetStringId();
			state.Profile.FirstName = firstName;
			state.Profile.LastName = lastName;
			state.Password = password;

			await SetActorStateAsync( state );

			return true;
		}

		/// <summary>
		/// Gets the user profile.
		/// </summary>
		/// <returns>
		/// The users profile data.
		/// </returns>
		public async Task<UserProfile> GetProfile()
		{
			var state = await GetActorStateAsync();

			return state.Profile;
		}

		/// <summary>
		/// Validates that the password supplied by a user matches the password 'on file'.
		/// </summary>
		/// <param name="password">The password supplied by the user attempting to login.</param>
		/// <returns>
		/// true if the password is valid; false otherwise.
		/// </returns>
		public async Task<bool> ValidatePassword( string password )
		{
			var state = await GetActorStateAsync();

			return state.Password.Equals( password );
		}

		/// <summary>
		/// Changes the users password, provided that the <paramref name="oldPassword"/> value
		/// matches the current password.
		/// </summary>
		/// <param name="oldPassword">The users current password.</param>
		/// <param name="newPassword">The new password to use.</param>
		/// <returns>
		/// true if the password has been successfully changed; false otherwise.
		/// </returns>
		public async Task<bool> ChangePassword( string oldPassword, string newPassword )
		{
			var result = false;
			var state = await GetActorStateAsync();

			// TODO: password should be protected (salted/hashed)
			if ( state.Password.Equals( oldPassword ) )
			{
				// TODO: check password history?
				state.Password = newPassword;
				// TODO: add new password to history?
				await SetActorStateAsync( state );
				result = true;
			}

			return result;
		}

		#endregion

		#region Helpers

		private async Task<ActorState> GetActorStateAsync()
		{
			var stateValue = await StateManager.TryGetStateAsync<ActorState>( STATE_KEY );
			return await Task.FromResult( stateValue.Value );
		}

		private async Task SetActorStateAsync( ActorState state )
		{
			await StateManager.AddOrUpdateStateAsync( STATE_KEY, state, ( ke, v ) => state );
		}

		#endregion
	}
}
