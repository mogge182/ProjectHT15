using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.UI.WebControls;

namespace ProjectHT15.Models
{
    public class ReviewModel
    {
        
        public Guid Id { get; set; }
        public Guid CreatorUserId { get; set; }

        [Required]
        [StringLength(Int32.MaxValue, ErrorMessage = "to short.", MinimumLength = 1)]
        public string Title { get; set; }

        [Display(Name = "Review")]
        [Required]
        [StringLength(Int32.MaxValue, ErrorMessage = "to short.", MinimumLength = 6)]
        public string Description { get; set; }
     
        public DateTime CreatedDate { get; set; }
       
        public decimal ReviewRating { get; set; }

        [Required]
        [Range(1,5,ErrorMessage = "You need to rate the review.")]
        public int UserRating { get; set; }

        public int LikeCount { get; set; }

        [Required]
        public string Type { get; set; }
    }
}