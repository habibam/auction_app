using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Controllers
{
    
    public class UserController : Controller
    {
        private AuctionsContext _context;
        
        public UserController(AuctionsContext context)
        {
            _context = context;
        }

//****************************************************************************************************/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {

            // Lazy login.  remove this when completed
            // LoginUser dustin = new LoginUser();
            // dustin.LogUsername = "amalfushi";
            // dustin.LogPassword = "password";

            // return Login(dustin);
            return View();
        }

//****************************************************************************************************/
        [HttpPost]
        [Route("CreateUser")]
        public IActionResult CreateUser(RegisterUser u)
        {
            if(ModelState.IsValid){
                List<User> users = _context.users.Where(nu => nu.Username == u.Username).ToList();
                if(users.Count > 0){
                    ViewBag.InvalidUsername = true;
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();

                User nUser = new User();
                nUser.Password = Hasher.HashPassword(nUser, u.Password);
                nUser.FirstName = u.FirstName;
                nUser.LastName = u.LastName;
                nUser.Username = u.Username;
                nUser.Wallet = 1000.00;

                _context.Add(nUser);
                _context.SaveChanges();
                
                HttpContext.Session.SetInt32("UserId", nUser.UserId);
                return Redirect("/dashboard");
            }
            return View("Index");
        }

//****************************************************************************************************/
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginUser lu)
        {
            if(ModelState.IsValid)
            {
                User user = _context.users.Where(u => u.Username == lu.LogUsername).SingleOrDefault();
                if(user != null)
                {   
                    PasswordHasher<User> hasher = new PasswordHasher<User>();
                    if(hasher.VerifyHashedPassword(user, user.Password, lu.LogPassword) != 0)
                    {
                        HttpContext.Session.SetInt32("UserId", user.UserId);
                        return Redirect("/dashboard");
                    }
                }
                ViewBag.InvalidLogin = true;
                return View ("Index");
            }
            return View("Index");
        }

//****************************************************************************************************/
        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}