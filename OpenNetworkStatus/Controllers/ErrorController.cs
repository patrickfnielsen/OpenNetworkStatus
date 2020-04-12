using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenNetworkStatus.Models;

namespace OpenNetworkStatus.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        private readonly Dictionary<int, (string Title, string Message)> _messages;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;

            _messages = new Dictionary<int, (string Title, string Message)>()
            {
                {
                    500,
                    ("Something went wrong", "We are working on fixing it, please try again later.")
                },
                {
                    404,
                    ("Page not found :(", "The requested page could not be found.")
                },
                {
                    401,
                    ("Unauthorized", "Access denied.")
                }
            };
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Code));
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Code(int id = 500)
        {
            if (!_messages.ContainsKey(id))
            {
                id = 500;
            }
            
            var error = new ErrorViewModel(id, _messages[id].Title, _messages[id].Message, HttpContext.TraceIdentifier);

            _logger.LogError("Error: {@error}", error);
            return View(error);
        }
    }
}