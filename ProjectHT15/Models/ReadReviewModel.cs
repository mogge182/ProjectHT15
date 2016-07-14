using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectHT15.Models
{
    public class ReadReviewModel
    {
        public Guid Id { get; set; }
        public Guid CreatorUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal ReviewRating { get; set; }
        public int UserRating { get; set; }
        public int LikeCount { get; set; }
        public bool HasLiked { get; set; }
        public string Type { get; set; }
    }
}