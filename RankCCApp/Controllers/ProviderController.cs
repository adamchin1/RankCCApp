using Microsoft.AspNetCore.Mvc;
using RankCCApp.Models;

namespace RankCCApp.Controllers
{
    public class ProviderController : Controller
    {
        public static List<Provider> providers = new List<Provider>{            
            };

        public IActionResult Index()
        {
            ReadWriteController rwc = new ReadWriteController();
            string[] lines = rwc.Read("providers.txt");

            providers.Clear();

            for (int i = 0; i < lines.Length; i++)
            {
                Provider provider = new Provider { Name = lines[i].ToUpper() };
                providers.Add(provider);
            }
            return View(providers);
        }
        public void ValidationError(string errorMessage)
        {
            TempData["ErrorMessage"] = errorMessage;
        }

        public bool ValidateProvider(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ValidationError("Provider field can not be blank.");
                return false;
            }

            bool providerExists = false;
            foreach (var p in providers)
            {
                if (name == p.Name)
                {
                    providerExists = true;
                    ValidationError("Provider already exists in the system.");
                }
            }
            return !providerExists;
        }
        public IActionResult Create()
        {
            string input = Request.Form["input"].ToString().Trim().ToUpper();
            ReadWriteController rwc = new ReadWriteController();

            if (ValidateProvider(input)) {
                rwc.Write("providers.txt", input);     
            }
            return RedirectToAction("Index", "Provider");
        }

        public IActionResult Delete(Provider p)
        {
            ReadWriteController rwc = new ReadWriteController();
            rwc.Delete("providers.txt", p.Name);
            return RedirectToAction("Index", "Provider");
        }
    }
}
