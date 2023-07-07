using Deblog.Areas.Identity.Data;
using Deblog.Data;
using Deblog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Deblog.Controllers
{
	[Authorize]
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		//private string _userId;
		//private string _userName;

		public UserController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
		{
			_db = db;
			_userManager = userManager;
			//_userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
			//_userName = User.FindFirst(ClaimTypes.Name).Value;
		}
		public IActionResult Index()
		{
            var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var _userName = User.FindFirst(ClaimTypes.Name).Value;

            Userdata UserObj = _db.Userdata.FirstOrDefault(x => x.Id == _userId);

			if (UserObj == null )
			{
				Userdata newuser = new Userdata();
				newuser.Id = _userId;
				newuser.Username = _userName;
				newuser.DOJ = DateTime.Now;
				newuser.Fullname = _userName;
				newuser.UserDesc = "Hi there, nice to meet you.";
				newuser.ImageUrl = "/images/users.png";

				_db.Userdata.Add(newuser);
				_db.SaveChanges();

				return RedirectToAction("Settings");
			}

			TempData["fullname"] = UserObj.Fullname;
			TempData["userimage"] = UserObj.ImageUrl;
			TempData["username"] = _userName;

			



			return View();
		}

		public IActionResult Settings()
		{
            var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var _userName = User.FindFirst(ClaimTypes.Name).Value;

            Userdata UserObj = _db.Userdata.FirstOrDefault(x => x.Id == _userId);
			if (UserObj == null)
			{ 
				return RedirectToAction("Index");
			}

            Userform formObj = new Userform();
			formObj.Id = _userId;
			formObj.Fullname = UserObj.Fullname;
			formObj.UserDesc= UserObj.UserDesc;
			TempData["userimage"] = UserObj.ImageUrl;
			return View(formObj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Settings(Userform obj)
		{
            var _userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var _userName = User.FindFirst(ClaimTypes.Name).Value;

            Userdata UserObj = _db.Userdata.FirstOrDefault(x => x.Id == _userId);
            if (UserObj == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
			{
                UserObj.Fullname= obj.Fullname;
				UserObj.UserDesc= obj.UserDesc;

				if (obj.Image != null)
				{
					string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Projectdata/ProjectImage");

					//create folder if not exist
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);
					var imageId = 1;

					if (UserObj.ImageUrl.EndsWith("1.png"))
					{
						imageId= 2;
					}
					var newfilename = $"UserImage-{obj.Id}-{imageId}.png";

                    var filePath = Path.Combine(path, newfilename);

					filePath = filePath.Replace("\\", "/");

					using (FileStream fs = System.IO.File.Create(filePath))
					{
						obj.Image.CopyTo(fs);
					}
					UserObj.ImageUrl = Path.Combine("/images/Projectdata/ProjectImage", newfilename);
				}

				_db.Userdata.Update(UserObj);
				_db.SaveChanges();

				return RedirectToAction("Index");
			}
            TempData["userimage"] = UserObj.ImageUrl;
            return View(obj);
		}
	}
}
