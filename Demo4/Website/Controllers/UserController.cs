using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

using UserActor.Interfaces;
using Website.Models;

namespace Website.Controllers
{
	public class UserController : Controller
	{
		// GET: /<controller>/Login
		public IActionResult Login()
		{
			return View( new LoginViewModel() );
		}

		// POST: /<controller>/Login
		[HttpPost]
		public async Task<IActionResult> Login( LoginViewModel model )
		{
			if ( ModelState.IsValid )
			{
				var userProfile = await ValidateUser( model );
				if ( userProfile != null )
				{
					BuildUserSesion( userProfile );
					return RedirectToAction( "Index", "Home" );
				}
				else
				{
					ModelState.AddModelError( string.Empty, "Login unsuccessful. Please try again." );
				}
			}

			return View( model );
		}

		// GET: /<controller>/SignOut
		public async Task<IActionResult> SignOut()
		{
			var username = User.Identity.Name;
			await HttpContext.Authentication.SignOutAsync( "Forms" );

			ServiceEventSource.Current.SuccessfulLogout( username );

			return RedirectToAction( "Index", "Home" );
		}

		// GET: /<controller>/Register
		public IActionResult Register()
		{
			return View();
		}

		// POST: /<controller>/Register
		[HttpPost]
		public async Task<IActionResult> Register( RegistrationViewModel model )
		{
			if ( ModelState.IsValid )
			{
				var success = await CreateUser( model );
				if ( success )
				{
					return RedirectToAction( "RegistrationSuccess" );
				}
			}

			return View( model );
		}

		// GET: /<controller>/RegistrationSuccess
		public IActionResult RegistrationSuccess()
		{
			return View();
		}

		#region Helpers

		private async Task<UserProfile> ValidateUser( LoginViewModel model )
		{
			UserProfile result = null;

			var actorId = new ActorId( model.Email );
			var userActor = ActorProxy.Create<IUserActor>( actorId );   // serviceUri is optional when called from the same cluster
			var profile = await userActor.GetProfile();

			if ( !string.IsNullOrEmpty( profile.FirstName ) && await userActor.ValidatePassword( model.Password ) )
			{
				result = profile;
			}

			return result;
		}

		private void BuildUserSesion( UserProfile profile )
		{
			var claims = new List<Claim>
			{
				new Claim( ClaimTypes.Email, profile.Email ),
				new Claim( ClaimTypes.Name, profile.Email ),
				new Claim( ClaimTypes.GivenName, profile.FirstName ),
				new Claim( ClaimTypes.Surname, profile.LastName ),
				new Claim( ClaimTypes.AuthenticationMethod, "Forms" )
			};

			var identity = new ClaimsIdentity( claims, "Forms" );
			var principal = new ClaimsPrincipal( identity );

			HttpContext.Authentication.SignInAsync( "Forms", principal ).GetAwaiter().GetResult();

			ServiceEventSource.Current.SuccessfulLogin( profile.Email );
		}

		private async Task<bool> CreateUser( RegistrationViewModel model )
		{
			var actorId = new ActorId( model.Email );
			var userActor = ActorProxy.Create<IUserActor>( actorId );
			var success = await userActor.Create( model.FirstName, model.LastName, model.Password );

			return success;
		}

		#endregion
	}
}