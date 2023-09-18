using Hackathon23_GPOtoGuestConfig.Models;
using Hackathon23_GPOtoGuestConfig.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hackathon23_GPOtoGuestConfig.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult Upload(IFormFile file)
        {
            var result = new List<SecPolicyConversionResult>();
            if (file != null && file.Length > 0)
            {
                var service = new GuestConfigConversionUtility();

                //read the file content to string
                var reader = new StreamReader(file.OpenReadStream());
                var fileAsString = reader.ReadToEnd();

                result = service.ConvertSecPolicytoGuestConfig(fileAsString, file.FileName);

                var template = service.GenerateCustomizedTemplate(result);
                var request = service.GenerateCustomizedHttpRequest(result);

                var viewModel = new ResultViewModel()
                {
                    Results = result,
                    CustomizedTemplateValue = template,
                    WebRequest = request
                };
                return View("ResultView", viewModel);

                // Do something with the uploaded file
            }
            return View("ResultView", result);
        }
    }
}