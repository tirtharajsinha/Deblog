using Deblog.Areas.Identity.Data;
using Deblog.Data;
using Deblog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
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

		public IActionResult Index(int id)
		{
			Blog obj = _db.Blogs.FirstOrDefault(x => x.Id == id);
			Userdata AuthorData = _db.Userdata.FirstOrDefault(x => x.Id == obj.BlogAuthor);

			if (obj == null)
			{
				TempData["Message"] = "This id does not Exist";
				return NotFound("This id does not Exist");
			}

			var data = new Tuple<Blog, Userdata>(obj, AuthorData);

			return View("viewblog",data);
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

			if (ModelState.IsValid)
			{
				_db.Blogs.Add(obj);
				_db.SaveChanges();

				return RedirectToAction("Index", "User");
			}
			return View();
		}

		[Authorize]
		public IActionResult BlogEditor(int id)
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


			return View(obj);
		}


		[Authorize]
		[HttpPost]
		public IActionResult BlogEditor(Blog obj)
		{
			var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

			if (ModelState.IsValid && obj.BlogAuthor == _userId)
			{
				obj.BlogDatetime = DateTime.Now;

				_db.Blogs.Update(obj);
				_db.SaveChanges();
				return RedirectToAction("Index", "User");
			}

			return View(obj);
		}


		[Authorize]
		public IActionResult BlogWriter(int id)
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

			BlogContent blogContent = new BlogContent();
			blogContent.Id = obj.Id;
			blogContent.BlogAuthor = obj.BlogAuthor;
			blogContent.BlogBody = obj.BlogBody;
			blogContent.BlogStatus = obj.BlogStatus;

			string blogtitle = obj.BlogTitle;
			if (blogtitle.Length > 40)
			{
				blogtitle = blogtitle.Substring(0, 40) + "...";
			}

			TempData["blogtitle"] = blogtitle;

			return View(blogContent);
		}


		[Authorize]
		[HttpPost]
		public IActionResult BlogWriter(BlogContent blogContent)
		{
			var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

			Blog obj = _db.Blogs.FirstOrDefault(x => x.Id == blogContent.Id);
			Hashtable responsedata;
			if (ModelState.IsValid && blogContent.BlogAuthor == _userId && _userId == obj.BlogAuthor)
			{

				obj.BlogDatetime = DateTime.Now;
				obj.BlogBody = blogContent.BlogBody;

				_db.Blogs.Update(obj);
				_db.SaveChanges();
				responsedata = new Hashtable(){
					{"Status", 200}
				};
				return Json(responsedata);
			}

			responsedata = new Hashtable(){
					{"Status", 404}
			};

			return Json(responsedata);
		}


		// if there is error of max-packet-allowed, follow this video https://www.youtube.com/watch?v=zDaaG8hFYlk


		[Authorize]
		public IActionResult Publish(int id)
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

			obj.BlogStatus = true;


			_db.Blogs.Update(obj);
			_db.SaveChanges();

			string next = HttpContext.Request.Query["next"];

			if (next != null && next == "BlogWriter")
			{
				return RedirectToAction("BlogWriter", new { id = id });
			}
			return RedirectToAction("Index", "User");
		}


		[Authorize]
		public IActionResult Unpublish(int id)
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

			obj.BlogStatus = false;
			_db.Blogs.Update(obj);
			_db.SaveChanges();


			string next = HttpContext.Request.Query["next"];

			if (next != null && next == "BlogWriter")
			{
				return RedirectToAction("BlogWriter", new { id = id });
			}

			return RedirectToAction("Index", "User");
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


