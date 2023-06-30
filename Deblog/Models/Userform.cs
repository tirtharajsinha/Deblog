using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deblog.Models
{
	public class Userform
	{
		[Key]
		[ValidateNever]
		public string Id { get; set; }

		
		[ValidateNever]
		[NotMapped]
		public IFormFile Image { get; set; }


		[Required]
		public String Fullname { get; set; }

		[Required]
		public String UserDesc { get; set; }

	}
}
