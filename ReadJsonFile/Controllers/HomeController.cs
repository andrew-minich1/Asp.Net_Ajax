using Newtonsoft.Json;
using ReadJsonFile.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ReadJsonFile.Controllers
{
    public class HomeController : Controller
    {
        private List<Comment> Comments { get; set; }
        private static List<Comment> CommentsNew { get; set; }

        static HomeController()
        {
            CommentsNew = new List<Comment>();
        }
        public HomeController()
        {
            this.Comments = new List<Comment>();
        }

        public async Task<ActionResult> Index()
        {
            if (this.HttpContext.Cache["comments"] != null)
            {
                Comments = this.HttpContext.Cache["comments"] as List<Comment>;
            }
            else
            {
                this.Comments = await ReadFile();
                this.HttpContext.Cache.Insert("comments", Comments, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);
            }

            var count = Math.Ceiling(Convert.ToDouble(this.Comments.Count) / 10.0);
            CommentViewModel result = new CommentViewModel();
            result.Comments = this.Comments.Take(10).ToList();
            result.Pages = countPages(1, Convert.ToInt32(count));
            ViewBag.CurrentPage = 1;
            return View(result);
        }

        [HttpPost]
        public ActionResult AddComment(Comment comment)
        {
            if (null != comment)
            {
                if(User.Identity.Name == null || string.Empty == User.Identity.Name)
                {
                    comment.UserName = "Аnonym";
                }
                else
                {
                    comment.UserName = User.Identity.Name;
                }
                CommentsNew.Add(comment);
            }
            if (Request.IsAjaxRequest())
            {
                return Json(comment);
            }
            else
            {
                return Redirect("Index");
            }
        }

        public ActionResult LoadComments(int pageNumber)
        {
            this.Comments = this.HttpContext.Cache["comments"] as List<Comment>;
            int numberCommentsOnThePage = int.Parse(WebConfigurationManager.AppSettings["numberCommentsOnThePage"]);
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            CommentViewModel result = new CommentViewModel();
            double count = Convert.ToDouble(Comments.Count) / 10.0;
            var comments = Comments.Skip((pageNumber - 1) * 10).Take(10);
            result.Comments = comments.ToList();
            result.Pages = countPages(pageNumber, Convert.ToInt32(count));

            if (Request.IsAjaxRequest())
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View("Index", result);
            }
        }

        public ActionResult GenerateJson()
        {
            List<Comment> temp = new List<Comment>();
            for (int i = 0; i < 1000; i++)
            {
                temp.Add(new Comment { Text = "Text " + i, UserName = "User Name " + i });

            }
            string path = Server.MapPath("~/Content/comments.json");
            if (null == path || path == string.Empty)
                throw new Exception("Null or Empty path");
            try
            {
                using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, FileMode.OpenOrCreate)))
                {
                    string comments = JsonConvert.SerializeObject(temp);
                    sw.Write(comments);
                }
            }
            catch (Exception e)
            {
                throw new IOException("Can not open file", e);
            }

            return RedirectToAction("Index");

        }

        private async Task<List<Comment>> ReadFile()
        {
            List<Comment> commentsResult;
            string path = Server.MapPath(WebConfigurationManager.AppSettings["filePath"]);

            if (null == path || path == string.Empty)
                throw new Exception("Null or Empty path");

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string comments = await sr.ReadToEndAsync();
                    commentsResult = JsonConvert.DeserializeObject<List<Comment>>(comments);
                }
            }
            catch (Exception e)
            {
                throw new IOException("Can not open file", e);
            }

            return commentsResult;
        }

        private int[] countPages(int currentPage, int totalPage)
        {
            int[] pages = new int[7];
            if (totalPage <= 5)
            {
                for (int i = 1; i < totalPage; i++)
                {
                    pages[i] = i + 1;
                }
                return pages;
            }

            else
            {
                if (currentPage <= 3)
                {
                    for (int i = 1; i < pages.Length - 1; i++)
                    {
                        pages[i] = i;
                    }
                    pages[pages.Length - 1] = totalPage;
                    return pages;
                }

                if (currentPage >= totalPage - 2)
                {
                    pages[0] = 1;
                    for (int i = 1; i < pages.Length - 1; i++)
                    {
                        pages[i] = totalPage - 5 + i;
                    }
                    return pages;
                }

                if (currentPage >= 4 && currentPage <= totalPage - 3)
                {
                    pages[0] = 1;
                    for (int i = 1; i < pages.Length - 1; i++)
                    {
                        pages[i] = currentPage - 3 + i;
                    }
                    pages[pages.Length - 1] = totalPage;
                    return pages;
                }
            }
            return pages;
        }

        [Authorize]
        public ActionResult AboutUs()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult GetComments()
        {
            return PartialView(CommentsNew);
        }

    }
}
