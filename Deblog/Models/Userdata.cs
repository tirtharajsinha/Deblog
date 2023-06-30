using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deblog.Models
{
	public class Userdata
	{
		[Key]
		[ValidateNever]
		public string Id { get; set; }

		[Required]
		public string Username { get; set; }


		[ValidateNever]
		public String ImageUrl { get; set; }

		[Required]
		public String Fullname { get; set; }

		[Required]
		public String UserDesc { get; set; }


		public DateTime DOJ { get; set; }
	}
}
