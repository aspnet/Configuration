using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CustomOptionClasses
{
    public class HomeController : Controller
    {
        private readonly IOptions<MyOptions> _myOptionsProvider;

        public HomeController(IOptions<MyOptions> myOptionsProvider)
        {
            _myOptionsProvider = myOptionsProvider;
        }

        public ContentResult Index()
        {
            var myOptions = _myOptionsProvider.Value;
            return Content($"MyOptions in the HomeController: {myOptions.StringOption} {myOptions.IntegerOption}");
        }
    }
}