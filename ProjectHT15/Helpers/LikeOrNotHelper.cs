using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web;
using ProjectHT15.Models.Context;

namespace ProjectHT15.Helpers
{
    public class LikeOrNotHelper
    {
        public static Review LikeOrNot(UserToReview like)
        {

            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    var review = context.Reviews.FirstOrDefault(x => x.Id == like.ReviewId);
                    if (like.HasLiked)
                    {
                        review.LikeCount += 1;
                        return review;
                    }
                    else
                    {
                        review.LikeCount -= 1;
                        return review;
                    }
                    
                }
            }
            catch (Exception)
            {
                
                throw;
            }


            return null;
        }
    }
}