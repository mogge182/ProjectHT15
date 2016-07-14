using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectHT15.Models
{
    public class MultiResultModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserRating { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
    }
}