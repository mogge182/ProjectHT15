using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;
using ProjectHT15.Models.Context;

namespace ProjectHT15.Models
{
    public class UserReviewsModel
    {

       
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Review> List { get; set; }

       


    }
}