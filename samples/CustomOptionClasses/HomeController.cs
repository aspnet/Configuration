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

        public ContentResult Index() => Content(_myOptions?.Value?.Option1 ?? "Option1 is null");
    }
}