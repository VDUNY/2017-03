using System.ComponentModel.DataAnnotations;

namespace Website.Models
{
	public class RegistrationViewModel
    {
		[DataType(DataType.EmailAddress)]
		[Required]
		public string Email { get; set; }

		[Display(Name = "First Name")]
		[Required]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		[Required]
		public string LastName { get; set; }

		[Compare( "ConfirmPassword" )]
		[DataType(DataType.Password)]
		[Required]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[Required]
		public string ConfirmPassword { get; set; }
	}
}
