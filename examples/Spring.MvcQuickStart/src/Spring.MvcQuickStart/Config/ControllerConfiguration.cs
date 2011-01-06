using Spring.Context.Attributes;
using Spring.MvcQuickStart.Controllers;

namespace Spring.MvcQuickStart.Config
{
    [Configuration]
    public class ControllerConfiguration
    {
        [Definition]
        public virtual HomeController Home()
        {
            HomeController controller = new HomeController();
            controller.Message = "Welcome to ASP.NET MVC powered by Spring.NET (Code-Config)!";
            return controller;
        }
    }
}