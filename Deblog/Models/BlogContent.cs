using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Deblog.Models
{
	public class BlogContent
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[ValidateNever]
		public string BlogAuthor { get; set; }

		[Required]
		[ValidateNever]
		public string BlogBody { get; set; }

		public bool BlogStatus { get; set; }



	}
}
