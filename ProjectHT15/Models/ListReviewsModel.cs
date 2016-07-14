using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectHT15.Models
{
    public class ListReviewsModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public int Likes { get; set; }
        public int UserRating { get; set; }
        public string Type { get; set; }
        public decimal OverallRating { get; set; }
        public DateTime Date { get; set; }
    }
}