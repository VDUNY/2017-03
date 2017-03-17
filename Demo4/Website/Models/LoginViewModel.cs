using System.ComponentModel.DataAnnotations;

namespace Website.Models
{
	public class LoginViewModel
    {
		[DataType(DataType.EmailAddress)]
		[Required]
		public string Email { get; set; }

		[DataType(DataType.Password)]
		[Required]
		public string Password { get; set; }
	}
}
