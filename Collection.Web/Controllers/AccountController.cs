using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Collection.Data.Models;
using System.Threading.Tasks;
using Collection.Data;
using Collection.Business.Services;
using System.Web.Security;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Net;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;

namespace Collection.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly CollectionDbContext dbContext;
        private readonly IUserService userService;
        private readonly ICollectionTypeService collectionTypeService;
        public AccountController()
        {
            this.dbContext = new CollectionDbContext();
            this.userService = new UserService(this.dbContext);
            this.collectionTypeService = new CollectionTypeService(this.dbContext);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //GET: Account Register
        public ActionResult Register()
        {
            return View();
        }
    
        //POST: Account Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User model)
        {
            userService.Create(model);
            dbContext.SaveChanges();
           

            //CollectionType type = new CollectionType();
            //type.UserId = model.UserId;
            //type.Sales = true;
            //type.Type = "Sales";

            //collectionTypeService.Create(type);
            //dbContext.SaveChanges();

            //CollectionType expense = new CollectionType();
            //expense.UserId = model.UserId;
            //expense.Sales = false;
            //expense.Type = "Expense";

            //collectionTypeService.Create(expense);
            //dbContext.SaveChanges();
            var user = User.Identity;
            if (user != null)
            {
                string userId = user.GetUserId();
                if (userId == "-1")
                {
                    return RedirectToAction("Admin", "Account");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        // GET: Account Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string password, bool remember = false)
        {
            var user = await userService.CheckUser(email,password);
            if (user != null)
            {
                bool active = user.ActiveDate > DateTime.Now;
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, active.ToString()));
                //claims.Add(new Claim(ClaimTypes.Expiration, active.ToString()));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));

                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                AuthenticationManager.SignIn(new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                }, identity);

                return RedirectToAction("Index", "Home");
            }
            else if (email.Equals("superadmin"))
            {
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, "-1"));

                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                AuthenticationManager.SignIn(new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                }, identity);
                return RedirectToAction("Admin", "Account");
            }
            TempData["msg"] = "<span class='red'>Email or Password is incorrect. Please try again.</span>";
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> FBLogin(string fname, string lname, string email, string id)
        {
            var user = await userService.CheckUserFB(id,email);
            if (user != null)
            {
                bool active = user.ActiveDate > DateTime.Now;
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, active.ToString()));
                //claims.Add(new Claim(ClaimTypes.Expiration, active.ToString()));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));

                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                AuthenticationManager.SignIn(new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                }, identity);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                User model = new User();
                model.Name = fname + " " + lname;
                model.UserName = fname + lname;
                model.Password = model.Name;
                model.FBid = id;
                model.Email = email;
                userService.Create(model);
                dbContext.SaveChanges();


                ////CollectionType type = new CollectionType();
                ////type.UserId = model.UserId;
                ////type.Sales = true;
                ////type.Type = "Sales";

                ////collectionTypeService.Create(type);
                ////dbContext.SaveChanges();

                ////CollectionType expense = new CollectionType();
                ////expense.UserId = model.UserId;
                ////expense.Sales = false;
                ////expense.Type = "Expense";

                //collectionTypeService.Create(expense);
                //dbContext.SaveChanges();
                bool active = model.ActiveDate > DateTime.Now;
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, active.ToString()));
                //claims.Add(new Claim(ClaimTypes.Expiration, active.ToString()));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, model.UserId.ToString()));

                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                AuthenticationManager.SignIn(new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                }, identity);

                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                                  DefaultAuthenticationTypes.ExternalCookie);
            return RedirectToAction("Login", "Account");
        }

        public async Task<ActionResult> Delete(int id)
        {
            await userService.Delete(id);
            dbContext.SaveChanges();
            return RedirectToAction("Admin", "Account");
        }

        public async Task<ActionResult> Edit( int id)
        {

            var model = await userService.GetByIdAsync(id);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Update(User model)
        {

           await userService.Update(model.UserId, model);
            dbContext.SaveChanges();
            return RedirectToAction("Admin","Account");
        }
        public ActionResult Admin()
        {

            int userId = int.Parse(User.Identity.GetUserId());
            if (userId == -1)
            {
                var model = userService.GetAll();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
          
        }

        public async Task<bool> CheckUserName(string userName)
        {
            var user = await userService.CheckUserName(userName);

            return user == null ? false : true;
        }

        public async Task<bool> CheckEmail(string email)
        {
            var user = await userService.CheckEmail(email);

            return user == null ? false : true;
        }

    }
}