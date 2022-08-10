using CreditCardValidator;
using Microsoft.AspNetCore.Mvc;
using RankCCApp.Models;
using RankCCApp.ViewModels;

namespace RankCCApp.Controllers
{
    public class CardController : Controller
    {
        static List<Card> cards = new List<Card> { };
        static List<Provider> providers = new List<Provider>{ };

        public IActionResult Index()
        {
            ReadWriteController rwc = new ReadWriteController();
            string[] providerLines = rwc.Read("providers.txt");

            providers.Clear();

            for (int i = 0; i < providerLines.Length; i++)
            {
                Provider provider = new Provider { Name = providerLines[i].ToUpper() };
                providers.Add(provider);
            }

            string[] cardLines = rwc.Read("cards.txt");
            cards.Clear();
            
                for (int i = 0; i < cardLines.Length; i++)
                {
                    if (cardLines[i] != "")
                    {
                        string[] values = cardLines[i].Split("||");
                        Provider provider = new Provider { Name = values[1].ToUpper() };
                        long number = long.Parse(values[0]);
                        Card card = new Card { Number = number, Provider = provider };
                        cards.Add(card);
                    }
                } 
            
            CardProviderViewModel cardProviderViewModel = new CardProviderViewModel
            {
                Cards = cards,
                Providers = providers
            };
            return View(cardProviderViewModel);
        }

        public void ValidationError(string errorMessage)
        {
            TempData["ErrorMessage"] = errorMessage;
        }

        public IActionResult Save()
        {
            string input = Request.Form["input"].ToString().Trim();
            if (input.Length <= 18) // long data type does not support numbers with more than 18 digits
            {
                bool isNumeric = long.TryParse(input, out long number);

                if (isNumeric && number > 0)
                {
                    CreditCardDetector detector = new CreditCardDetector(input);                  
                    string providerName = detector.BrandName.ToUpper();

                    if (ValidateCard(detector, number, providerName, providers))
                    {
                        ReadWriteController rwc = new ReadWriteController();
                        string output = number + "||" + providerName;
                        rwc.Write("cards.txt", output);
                    }
                } else { ValidationError("Card number must be a sequence of numeric characters."); }
            } else { ValidationError("Card number can not exceed 18 digits."); }              
            return RedirectToAction("Index");
        }
        public IActionResult Delete(Card card)
        {
            string cardString = card.Number.ToString();
            ReadWriteController rwc = new ReadWriteController();
            rwc.Delete("cards.txt", cardString);
            return RedirectToAction("Index", "Card");
        }
        public bool ValidateCard(CreditCardDetector detector, long number, string providerName, List<Provider> providers)
        {
            bool isValidProvider = false;
            bool isValidCard = false;

            // Check if provider is in list of configured providers
            foreach (var p in providers)
            {
                if (providerName == p.Name)
                {
                    isValidProvider = true;
                }
            }

            // Check if card number is already in the system
            if (cards.Count != 0)
            {
                foreach (var card in cards)
                {
                    if (card.Number == number)
                    {
                        isValidCard = false;
                        break;

                    }
                    else
                    {
                        isValidCard = true;
                    }
                }
            }
            else
            {
                isValidCard = true;
            }

            if (!isValidProvider)
            {
                ValidationError(providerName + " provided cards are not configured.");
            }

            if (!detector.IsValid())
            {
                ValidationError("Card number is invalid.");
            }

            if (!isValidCard)
            {
                ValidationError("Card number already exists in system.");
            }

            return isValidCard && isValidProvider && detector.IsValid();
        }
    }
}
