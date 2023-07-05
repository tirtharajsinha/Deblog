using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deblog.Models
{
	public class Bookmark
	{
		[Required]
		public string UserId { get; set; }

		[Required]
		public int BlogId { get; set; }
	}
}
