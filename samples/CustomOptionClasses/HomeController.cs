using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CustomOptionClasses
{
    public class HomeController : Controller
    {
        private readonly IOptions<MyOptions> _myOptions;

        public HomeController(IOptions<MyOptions> myOptions)
        {
            _myOptions = myOptions;
        }

        public ContentResult Index() => Content($"MyOptions in the HomeController: {_myOptions.Value.StringOption} {_myOptions.Value.IntegerOption}");
    }
}