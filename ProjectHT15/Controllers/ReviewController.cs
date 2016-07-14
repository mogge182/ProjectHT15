using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using ProjectHT15.Models;
using ProjectHT15.Models.Context;
using PagedList;
using ProjectHT15.Helpers;

namespace ProjectHT15.Controllers
{
    public class ReviewController : Controller
    {
        // GET: Review
        public ActionResult Index()
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        public ActionResult NewReview(ReviewModel rw)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (rw.UserRating == 0)
            {
                rw.UserRating = 3;
            }
            
            return View(rw);

        }

        [HttpPost]
        public ActionResult AddReview(ReviewModel rw)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View("NewReview",rw);
            }

            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    var loggedInUser = (LoggedInUserModel)Session["LoggedIn"];

                    var newReview = new Review();
                  
                    if (rw.Id == new Guid())
                    {
                        newReview.Id = Guid.NewGuid();
                    }
                    else
                    {
                        newReview.Id = rw.Id;
                    }

                    newReview.CreatorUserId = loggedInUser.UserId;
                    newReview.Description = rw.Description;
                    newReview.Title = rw.Title;
                    newReview.CreatedDate = DateTime.Now;
                    newReview.Type = rw.Type;
                    newReview.UserRating = rw.UserRating;
                    context.Reviews.AddOrUpdate(newReview);
                    context.SaveChanges();
                }

                return View("NewReview");
            }
            catch (Exception)
            {
                return View("_Error");
            }
        }

        public ActionResult ReviewResult(int? page)
        {

            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var reviewItems = new List<Review>();
           
                var session = (LoggedInUserModel)Session["LoggedIn"];
                var userId = session.UserId;
                try
                {
                    using (var context = new HermodsProjektEntities())
                    {
                        reviewItems.AddRange(context.Reviews.Where(review => review.CreatorUserId == userId));
                    }
                }
                catch (Exception)
                {
                return View("_Error");
            }
            
           
            return PartialView("_ReviewPartial", reviewItems);
        }

        public ActionResult EditReview(Guid? reviewId)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                using (var context = new HermodsProjektEntities())
                {

                    var review = context.Reviews.FirstOrDefault(x => x.Id == reviewId);
                    var edit = new ReviewModel()
                    {
                        Title = review.Title,
                        Id = review.Id,
                        CreatorUserId = review.CreatorUserId,
                        Description = review.Description,
                        Type = review.Type,
                        LikeCount = review.LikeCount,
                        UserRating = review.UserRating,
                        ReviewRating = review.ReviewRating,
                        CreatedDate = review.CreatedDate
                    };

                    return View("NewReview", edit);

                }
            }
            catch (Exception)
            {
                return View("_Error");
            }

        }


        public ActionResult ReadReview(Guid? id)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
           
           

            if (id != null)
            {

                var reviewModel = new ReadReviewModel();
                try
                {

                    using (var context = new HermodsProjektEntities())
                    {
                        var session = (LoggedInUserModel)Session["LoggedIn"];
                        var utr = context.UserToReviews.FirstOrDefault(x => x.ReviewId == id && x.UserId == session.UserId);

                        if (utr == null)
                        {
                            reviewModel.HasLiked = false;
                        }
                        else
                        {
                            reviewModel.HasLiked = utr.HasLiked;
                        }


                        var review = context.Reviews.FirstOrDefault(x => x.Id == id);

                        reviewModel.Title = review.Title;
                        reviewModel.CreatedDate = review.CreatedDate;
                        reviewModel.CreatorUserId = review.CreatorUserId;
                        reviewModel.Description = review.Description;
                        reviewModel.Id = review.Id;
                        reviewModel.LikeCount = review.LikeCount;
                        reviewModel.ReviewRating = review.ReviewRating;
                        reviewModel.Type = review.Type;

                        return View(reviewModel);
                    }

                }
                catch (Exception)
                {
                    return View("_Error");
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteReview(List<Guid> reviews )
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (reviews == null) return View("NewReview");
            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    foreach (var review in reviews)
                    {
                        context.UserToReviews.RemoveRange(context.UserToReviews.Where(x => x.ReviewId == review));
                        context.CommentToReviews.RemoveRange(context.CommentToReviews.Where(x => x.ReviewId == review));
                        context.Reviews.Remove(context.Reviews.FirstOrDefault(x => x.Id == review));


                    }
                    context.SaveChanges();


                    return View("NewReview");
                }
                   
            }
            catch (Exception)
            {
                return View("_Error");
            }
        }

        public ActionResult LikeOrNot(Guid id)
        {

            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var user = (LoggedInUserModel)Session["LoggedIn"];
            try
            {
                using (var context = new HermodsProjektEntities())
                {

                    var review = context.Reviews.FirstOrDefault(x => x.CreatorUserId == user.UserId && x.Id == id);

                    if (review == null)
                    {
                        var utr = context.UserToReviews.FirstOrDefault(x => x.ReviewId == id && x.UserId == user.UserId);
                        if (utr != null)
                        {
                            if (utr.HasLiked)
                            {
                                utr.HasLiked = false;
                                context.Reviews.AddOrUpdate(LikeOrNotHelper.LikeOrNot(utr));
                                context.UserToReviews.AddOrUpdate(utr);
                                context.SaveChanges();
                            }
                            else
                            {
                                utr.HasLiked = true;
                                context.Reviews.AddOrUpdate(LikeOrNotHelper.LikeOrNot(utr));
                                context.UserToReviews.AddOrUpdate(utr);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            utr = new UserToReview()
                            {
                                Id = Guid.NewGuid(),
                                UserId = user.UserId,
                                ReviewId = id,
                                HasLiked = true,
                                Rating = 0
                            };

                            context.UserToReviews.Add(utr);


                            var t = LikeOrNotHelper.LikeOrNot(utr);
                            context.Reviews.AddOrUpdate(t);
                            context.SaveChanges();
                        }
                    }

              
                }
            }

            catch (Exception)
            {
                return View("_Error"); ;
            }

            return RedirectToAction("ReadReview", new { id });
        }

        public ActionResult UsersReviews(Guid id, int? page)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
          
           
            try
            {

                using (var context = new HermodsProjektEntities())
                {
                    var userInfo = new UserReviewsModel();
                    var userList = new List<Review>();



                    foreach (var review in context.Reviews)
                    {
                        if (review.CreatorUserId == id)
                        {
                            if (userInfo.UserName == null || userInfo.FirstName == null || userInfo.LastName == null || userInfo.Email == null)
                            {
                                userInfo.UserName = review.User.Username;
                                userInfo.FirstName = review.User.FirstName;
                                userInfo.LastName = review.User.LastName;
                                userInfo.Email = review.User.Email;
                            }
                            userList.Add(review);
                        }
                        userInfo.List = userList;
                    }

                    return View(userInfo);
                }
            }
            catch (Exception e)
            {
                return View("_Error");
            }


        }

        public PartialViewResult Comments(Guid id)
        {
            
            using (var context = new HermodsProjektEntities())
            {

                var list = new List<CommentToReviewModel>();
                try
                {
                    foreach (var comment in context.CommentToReviews)
                    {
                        if (comment.ReviewId == id)
                            list.Add(new CommentToReviewModel()
                            {
                                UserName = comment.User.Username,
                                Id = comment.Id,
                                UserId = comment.UserId,
                                ReviewId = comment.ReviewId,
                                CreatedDate = comment.CreatedDate,
                                Comment = comment.Comment
                            });
                    }
                }
                catch (Exception)
                {
                    return PartialView("_Error");
                }
                return PartialView("_Comments", list.OrderBy(x => x.CreatedDate).ToList());
            }
        }

        public ActionResult AddComment(string comment, Guid id)
        {

            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    var user = (LoggedInUserModel)Session["LoggedIn"];
                    var addComment = new CommentToReview()
                    {
                        Id = Guid.NewGuid(),
                        ReviewId = id,
                        Comment = comment,
                        CreatedDate = DateTime.Now,
                        UserId = user.UserId
                       
                    };
                    context.CommentToReviews.Add(addComment);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                return View("_Error");
            }

            return RedirectToAction("ReadReview", new { id });
        }

        public ActionResult RateReview(int rating, Guid id)
        {

            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    var user = (LoggedInUserModel) Session["LoggedIn"];
                    var urw = context.Reviews.FirstOrDefault(x => x.CreatorUserId == user.UserId && x.Id == id);

                    if (urw == null)
                    {


                        var utr = context.UserToReviews.FirstOrDefault(x => x.UserId == user.UserId && x.ReviewId == id);

                        if (utr == null)
                        {
                            var newUtr = new UserToReview()
                            {
                                Id = Guid.NewGuid(),
                                ReviewId = id,
                                UserId = user.UserId,
                                HasLiked = false,
                                Rating = rating
                            };

                            context.UserToReviews.AddOrUpdate(newUtr);
                            context.SaveChanges();
                        }
                        else
                        {
                            utr.Rating = rating;
                            context.UserToReviews.AddOrUpdate(utr);
                            context.SaveChanges();
                        }
                        var r = new List<decimal?>();

                        foreach (var rates in context.UserToReviews)
                        {
                            if (rates.ReviewId == id)
                            {
                                if (rates.Rating != null)
                                {
                                    r.Add(rates.Rating);
                                }

                            }
                        }
                        var sum = r.Sum()/r.Count;
                        var review = context.Reviews.FirstOrDefault(x => x.Id == id);
                        review.ReviewRating = sum.Value;
                       
                        context.Reviews.AddOrUpdate(review);
                        context.SaveChanges();
                        return RedirectToAction("ReadReview", "Review", new { id = review.Id });
                    }
                    else
                    {
                        return RedirectToAction("ReadReview", "Review", new {id = urw.Id});
                    }
                }
                  
            }
            catch (Exception e)
            {
                return View("_Error");
            }

           
        }

        public ActionResult WhoHasRated(Guid id)
        {

            var list = new List<UserToReview>();
            var whrl = new List<WhoHasRatedModel>();
            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    list.AddRange(context.UserToReviews.Where(utr => utr.ReviewId == id && utr.Rating > 0));
                
                foreach (var utr in list)
                {
                   var whr = new WhoHasRatedModel()
                   {
                       UserId = utr.User.Id,
                       UserName = utr.User.Username,
                       Rating =  (decimal)utr.Rating
                       
                   };
                    whrl.Add(whr);
                }
                    return View(whrl);
                }
            }
            catch (Exception)
            {
                return View("_Error");
            }

        }
    }
}