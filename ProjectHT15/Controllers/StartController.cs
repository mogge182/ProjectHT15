using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using PagedList;
using ProjectHT15.Helpers;
using ProjectHT15.Models;
using ProjectHT15.Models.Context;

namespace ProjectHT15.Controllers
{
    public class StartController : Controller
    {
        // GET: Start
        public ActionResult Index(int? page)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var reviewItems = new List<ListReviewsModel>();
            if (ModelState.IsValid)
            {
                try
                {
                    using (var context = new HermodsProjektEntities())
                    {
                        foreach (var review in context.Reviews)
                        {
                            var newReview = new ListReviewsModel();

                            foreach (var user in context.Users)
                            {
                                if (user.Id == review.CreatorUserId)
                                    newReview.Username = user.Username;
                            }

                            newReview.Id = review.Id;
                            newReview.Title = review.Title;
                            newReview.UserId = review.CreatorUserId;
                            newReview.Likes = review.LikeCount;

                            newReview.OverallRating = review.ReviewRating;
                            newReview.Type = review.Type;
                            newReview.Date = review.CreatedDate;
                            newReview.UserRating = review.UserRating;
                            reviewItems.Add(newReview);
                        }
                    }
                }
                catch (Exception)
                {
                    return View("_Error");
                }
            }
            var pageNumber = (page ?? 1);
            return View("Index", reviewItems.ToPagedList(pageNumber, 5));


        }

        public ActionResult MyPage()
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }


        public ActionResult UpdateMyPage(EditMyInfo myInfo)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var sessionUser = (LoggedInUserModel)Session["loggedIn"];

            if (ValidationHelper.CheckUserName(myInfo.UserName, sessionUser.UserId))
            {
                ViewBag.UsernameError = "";
                if (ValidationHelper.CheckEmailAdress(myInfo.Email, sessionUser.UserId))
                {
                    try
                    {
                        User updateInfo;
                        using (var context = new HermodsProjektEntities())
                        {
                            updateInfo = context.Users.Where(x => x.Id == sessionUser.UserId).FirstOrDefault<User>();
                        }
                        if (updateInfo != null)
                        {
                            updateInfo.Username = myInfo.UserName;
                            updateInfo.FirstName = myInfo.FirstName;
                            updateInfo.LastName = myInfo.LastName;
                            updateInfo.Email = myInfo.Email.ToLower();
                            updateInfo.Password =
                                HashAndSaltHelper.PasswordHash(new LoginModel()
                                {
                                    Password = myInfo.Password,
                                    Salt = myInfo.UserName
                                });
                            updateInfo.Salt = myInfo.UserName;
                        }
                        using (var context = new HermodsProjektEntities())
                        {
                            context.Entry(updateInfo).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        var loginModel = new LoggedInUserModel()
                        {
                            UserName = updateInfo.Username,
                            Email = updateInfo.Email,
                            UserId = updateInfo.Id,
                            FirstName = updateInfo.FirstName,
                            LastName = updateInfo.LastName
                        };

                        Session["LoggedIn"] = loginModel;

                    }
                    catch (Exception e)
                    {
                        return View("_Error");
                    }
                }
                else
                {
                    ViewBag.EmailError = "is invalid.";
                }
            }
            else
            {
                ViewBag.UsernameError = "is invalid.";
            }
            return View("MyPage");
        }

        public ActionResult _ReadReview(Guid? reviewId)
        {

            using (var context = new HermodsProjektEntities())
            {
                var model = context.Reviews.FirstOrDefault(x => x.Id == reviewId);
                return RedirectToAction("Index", "Start", model);

            }
        }


        public ActionResult DeleteUser(Guid id)
        {

            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    foreach (var review in context.Reviews)
                    {
                        if (review.CreatorUserId == id)
                        {
                            context.UserToReviews.RemoveRange(review.UserToReviews);
                            context.CommentToReviews.RemoveRange(review.CommentToReviews);
                            context.Reviews.Remove(review);
                        }
                    }

                    context.UserToReviews.RemoveRange(context.UserToReviews.Where(x => x.UserId == id));
                    context.CommentToReviews.RemoveRange(context.CommentToReviews.Where(x => x.UserId == id));
                    context.Users.Remove(context.Users.FirstOrDefault(x => x.Id == id));
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return View("_Error");
            }


            Session["LoggedIn"] = null;
            return RedirectToAction("Login", "Home");
        }

        public ActionResult SearchResult()
        {
            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    var reviewList = new List<string>();
                    var dReviewList = new List<string>();
                    foreach (var review in context.Reviews)
                    {
                        reviewList.Add(review.Title);
                    }

                    dReviewList = reviewList.Distinct().ToList();

                    return PartialView("_SearchPartial", dReviewList);
                }
            }
            catch (Exception)
            {
                return View("_Error");
            }
        }

        [HttpPost]
        public ActionResult SearchTitle(List<string> title)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Home");
            }


            var searchResult = title.FirstOrDefault();
            var srl = new List<Review>();
            var urm = new List<MultiResultModel>();

            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    srl.AddRange(context.Reviews.Where(review => review.Title == searchResult));
                    if (srl.Count == 0)
                    {
                        return Redirect("Index");
                    }

                    if (srl.Count > 1)
                    {
                        foreach (var review in srl)
                        {
                            var userName = context.Users.FirstOrDefault(x => x.Id == review.CreatorUserId);
                            urm.Add(new MultiResultModel()
                            {
                                Title = review.Title,
                                Id = review.Id,
                                Description = review.Description,
                                Type = review.Type,
                                UserRating = review.UserRating,
                                Likes = review.LikeCount,
                                CreatedDate = review.CreatedDate,
                                UserName = userName.Username

                            });

                        }
                        return View("MultiResult", urm);
                    }
                    else
                    {
                        var s = srl.FirstOrDefault();
                        return RedirectToAction("ReadReview", "Review", new { id = s.Id });
                    }

                }

            }
            catch (Exception e)
            {
                return View("_Error");
            }
           
        }
    }
}