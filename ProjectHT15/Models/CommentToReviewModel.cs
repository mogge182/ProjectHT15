using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectHT15.Models
{
    public class CommentToReviewModel
    {
        public string UserName { get; set; }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ReviewId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}