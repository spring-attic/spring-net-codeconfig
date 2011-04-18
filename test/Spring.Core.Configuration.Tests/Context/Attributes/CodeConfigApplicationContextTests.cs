using NUnit.Framework;
using Spring.Context.Support;

namespace Spring.Context.Attributes
{
    [TestFixture]
    public class CodeConfigApplicationContextTests : AbstractConfigurationClassPostProcessorTests
    {

        protected override void CreateApplicationContext()
        {
            GenericApplicationContext ctx = new GenericApplicationContext();

            ctx.ScanAllAssemblies();

            ctx.Refresh();

            _ctx = ctx;
        }

    }

}
