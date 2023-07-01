using Deblog.Areas.Identity.Data;
using Deblog.Data;
using Deblog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Deblog.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
       

        public BlogController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult YourBlogs()
        {
            var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Blog> objCategoryList = _db.Blogs.Where(p => p.BlogAuthor == _userId).ToList();
            return View(objCategoryList);
        }

        [Authorize]
        public IActionResult NewBlog()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult NewBlog(Blog obj)
        {
            var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            


            obj.BlogAuthor = _userId;
            obj.BlogBody = obj.BlogTitle;
            obj.BlogStatus = false;
            obj.BlogDatetime = DateTime.Now;

            if(ModelState.IsValid)
            {
                _db.Blogs.Add(obj);
                _db.SaveChanges();

                return RedirectToAction("Index","User");
            }
            return View();
        }

        [Authorize]
		public IActionResult BlogEditor(int id)
		{
			var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Blog obj = _db.Blogs.FirstOrDefault(x => x.Id == id);

            if(obj == null)
            {
                TempData["Message"] = "This id does not Exist";
                return NotFound("This id does not Exist");
            }
            if(obj.BlogAuthor != _userId) {
				TempData["Message"] = "You are not authorized";
				return NotFound("You are not authorized");
			}


			return View(obj);
		}


		[Authorize]
        [HttpPost]
		public IActionResult BlogEditor(Blog obj)
		{
			var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if(ModelState.IsValid && obj.BlogAuthor == _userId)
            {
				obj.BlogDatetime = DateTime.Now;
                _db.Blogs.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "User");
            }

			return View(obj);
		}

        [Authorize]
        [HttpPost]
        public IActionResult Publish(Blog obj)
        {
            var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid && obj.BlogAuthor == _userId)
            {
                obj.BlogDatetime = DateTime.Now;
                obj.BlogStatus = true;
                _db.Blogs.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "User");
            }

            return View(obj);
        }


        [Authorize]
        [HttpPost]
        public IActionResult Unpublish(Blog obj)
        {
            var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid && obj.BlogAuthor == _userId)
            {
                obj.BlogDatetime = DateTime.Now;
                obj.BlogStatus = false;
                _db.Blogs.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "User");
            }

            return View(obj);
        }

        [Authorize]
        public IActionResult RemoveBlog(int id)
        {
            var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Blog obj = _db.Blogs.FirstOrDefault(x => x.Id == id);

            if (obj == null)
            {
                TempData["Message"] = "This id does not Exist";
                return NotFound("This id does not Exist");
            }
            if (obj.BlogAuthor != _userId)
            {
                TempData["Message"] = "You are not authorized";
                return NotFound("You are not authorized");
            }

            _db.Blogs.Remove(obj);
            _db.SaveChanges();

            return RedirectToAction("Index", "User");
        }


    }
}
