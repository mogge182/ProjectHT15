using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectHT15.Models
{
    public class WhoHasRatedModel
    {
        public Guid UserId { get; set; }
        public decimal Rating { get; set; }
        public string UserName { get; set; }
    }
}