using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Deblog.Models
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BlogTitle { get; set; }

        [Required]
		[ValidateNever]
		public string BlogBody { get; set;}

        public string BlogImageUrl { get; set;}

        [Required]
		[ValidateNever]
		public string BlogAuthor { get; set;}

        [Required]
        public string BlogType { get; set;}

        public string BlogTopic { get; set;}

        public bool BlogStatus { get; set;}

        [Required]
        public DateTime BlogDatetime { get; set;}

        [Required]
        public int BlogReadtime { get; set;}

    }
}
