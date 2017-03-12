using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    public class UserController : Controller
    {
		// GET: /<controller>/Login
        public IActionResult Login()
        {
            return View();
        }

		// GET: /<controller>/SignOut
		public IActionResult SignOut()
		{
			// TODO: kill the session

			return RedirectToAction("Index", "Home");
		}

		// GET: /<controller>/Register
		public IActionResult Register()
		{
			return View();
		}
    }
}