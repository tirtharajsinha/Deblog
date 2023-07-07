﻿using Deblog.Areas.Identity.Data;
using Deblog.Data;
using Deblog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Deblog.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;




		public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
		{
			_logger = logger;
			_db = db;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			List<Blog> objCategoryList = _db.Blogs
				.OrderByDescending(m => m.BlogDatetime)
				.ToList();


			List<Userdata> authors = _db.Userdata.ToList();

			Dictionary<string, Userdata> authormap = new Dictionary<string, Userdata>();
			foreach (Userdata author in authors)
			{
				authormap[author.Id] = author;
			}

			var data = new Tuple<List<Blog>, Dictionary<string, Userdata>>(objCategoryList, authormap);

			if (User.Identity.IsAuthenticated)
			{
				return View("IndexAuthenticated",data);
			}
			return View(data);
		}





		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


	}


}