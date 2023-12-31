﻿using Deblog.Areas.Identity.Data;
using Deblog.Data;
using Deblog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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

		//public IActionResult Index()
		//{
		//	return RedirectToAction("Index", "Home");
		//}

		public IActionResult Index(int id)
		{
			if (id == 0 || id == null)
			{
				return RedirectToAction("Index", "Home");
			}
			Blog obj = _db.Blogs.FirstOrDefault(x => x.Id == id);


			if (User.Identity.IsAuthenticated)
			{
				var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
				Bookmark bookmark = _db.Bookmarks.FirstOrDefault(x => x.UserId == _userId && x.BlogId == id);
				if (bookmark != null)
				{
					TempData["bookmark"] = "Yes";
				}
				else
				{
					TempData["bookmark"] = "No";
				}
			}



			Userdata AuthorData = _db.Userdata.FirstOrDefault(x => x.Id == obj.BlogAuthor);
			int countAuthorBlogs = _db.Blogs
				.Where(p => p.BlogAuthor == obj.BlogAuthor)
				.Count();


			if (obj == null && AuthorData==null)
			{
				TempData["Message"] = "This id does not Exist";
				return NotFound("This id does not Exist");
			}

			if (countAuthorBlogs == 1)
			{
				TempData["AuthorBlogCount"] = $"{countAuthorBlogs} blog.";
			}
			else
			{
				TempData["AuthorBlogCount"] = $"{countAuthorBlogs} blogs.";
			}
			
			var data = new Tuple<Blog, Userdata>(obj, AuthorData);

			return View("viewblog", data);
		}

		public IActionResult SearchBlogs()
		{
			string query = HttpContext.Request.Query["query"];
			if(query == null || query=="")
			{
				return Json(null);
			}

			var objCategoryList = _db.Blogs
				.Where(p => p.BlogType.Equals("public") && p.BlogStatus)
				.Where(p => p.BlogTitle.StartsWith(query))
				.OrderByDescending(m => m.BlogDatetime)
				.Select(t => new { t.Id, t.BlogTitle })
				.ToList();

			var objCategoryListContains = _db.Blogs
				.Where(p => p.BlogType.Equals("public") && p.BlogStatus)
				.Where(p => !p.BlogTitle.StartsWith(query) && p.BlogTitle.Contains(query))
				.OrderByDescending(m => m.BlogDatetime)
				.Select(t => new {t.Id, t.BlogTitle})
				.ToList();

			objCategoryList.AddRange(objCategoryListContains);

			if(objCategoryList.Count > 5)
			{
				objCategoryList = objCategoryList.GetRange(0, 6);
			}
			

			return Json(objCategoryList);
		}



		[Authorize]
		public IActionResult YourBlogs()
		{
			var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
			List<Blog> objCategoryList = _db.Blogs
				.Where(p => p.BlogAuthor == _userId)
				.OrderByDescending(m => m.BlogDatetime)
				.ToList();
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

				return RedirectToAction("BlogWriter", "Blog", new { id = obj.Id });
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
				return RedirectToAction("BlogWriter", "Blog", new { id = obj.Id });
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

		[Authorize]
		public IActionResult Bookmark(int id)
		{
			if (id == 0)
			{
				return NotFound();
			}
			var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
			Blog obj = _db.Blogs.FirstOrDefault(x => x.Id == id);
			if (obj == null)
			{
				return NotFound();
			}
			Bookmark bookmark = _db.Bookmarks.FirstOrDefault(x => x.UserId == _userId && x.BlogId == id);
			Hashtable responsedata;
			if (bookmark == null)
			{
				Bookmark newbook = new Bookmark();
				newbook.UserId = _userId;
				newbook.BlogId = id;
				_db.Bookmarks.Add(newbook);
				_db.SaveChanges();
				responsedata = new Hashtable(){
					{"state", true}
			};
			}
			else
			{
				_db.Bookmarks.Remove(bookmark);
				_db.SaveChanges();
				responsedata = new Hashtable(){
					{"state", false}
			};
			}


			return Json(responsedata);
		}

		[Authorize]
		public IActionResult MyBookmarks()
		{
			var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


			List<int> bookmarkedId = _db.Bookmarks.Where(p => p.UserId == _userId).Select(bp => bp.BlogId).ToList();

			//List<Blog> bookmarked = _db.Blogs.Where(p => bookmarkedId.Contains(p.Id)).ToList();
			List<Blog> bookmarked = _db.Blogs.ToList();
			List<Blog> savedblogs = new List<Blog>();
			foreach (var blg in bookmarked)
			{
				if (bookmarkedId.Contains(blg.Id))
				{
					savedblogs.Add(blg);
				}
			}



			return View(savedblogs);
		}


	}
}


