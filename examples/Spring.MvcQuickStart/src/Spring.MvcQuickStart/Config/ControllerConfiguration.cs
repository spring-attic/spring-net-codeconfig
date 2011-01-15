using Spring.Context.Attributes;
using Spring.MvcQuickStart.Controllers;
using Spring.Objects.Factory.Support;

namespace Spring.MvcQuickStart.Config
{
    [Configuration]
    public class ControllerConfiguration
    {
        [Definition]
        [Scope(ObjectScope.Prototype)]
        public virtual HomeController HomeController()
        {
            HomeController controller = new HomeController();
            controller.Message = "Welcome to ASP.NET MVC powered by Spring.NET (Code-Config)!";
            return controller;
        }
    }
}