using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAdsAuth.Data;
using SimpleAdsAuth.Web.Models;
using System.Diagnostics;

namespace SimpleAdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=Homework; Integrated Security=true;";

        public IActionResult Index()
        {
            AdViewModel vm = new AdViewModel();
            AdRepository repo = new AdRepository(_connectionString);
            vm.Ads = repo.GetAllAds();

            if (User.Identity.IsAuthenticated)
            {
                var user = repo.GetByEmail(User.Identity.Name);
                vm.CurrentId = user.Id;
            }
            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            AdRepository repo = new AdRepository(_connectionString);
            ad.UserId = repo.GetByEmail(User.Identity.Name).Id;
            repo.AddAd(ad);

            return Redirect("/home/index");
        }

        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            AdRepository repo = new AdRepository(_connectionString);
            repo.DeleteAd(id);
            return Redirect("/home/index");
        }

        public IActionResult MyAccount(int id)
        {
            var repo = new AdRepository(_connectionString);
            var vm = new MyAccountViewModel
            {
                Ads = repo.GetAllAdsForMyAccount(User.Identity.Name)
            };
            return View(vm);
        }

    }
}