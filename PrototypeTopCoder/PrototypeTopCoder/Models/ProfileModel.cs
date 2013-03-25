using System.ComponentModel.DataAnnotations;

namespace PrototypeTopCoder.Models
{
	public class ProfileModel
	{
		[DataType(DataType.Text)]
		[Display(Name = "Username")]
		public string Username { get; set; }
	}
}