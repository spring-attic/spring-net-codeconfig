using System;
using NUnit.Framework;
using Spring.Context.Config;
using Spring.Context.Support;
using Spring.Objects.Factory.Xml;

namespace Spring.Context.Attributes
{
    [TestFixture]
    public class ScanningConfigurationClassPostProcessorTests : AbstractConfigurationClassPostProcessorTests
    {
        protected override void CreateApplicationContext()
        {
            NamespaceParserRegistry.RegisterParser(typeof(ContextNamespaceParser));
            this._ctx = new XmlApplicationContext(ReadOnlyXmlTestResource.GetFilePath("SimpleScanTest.xml", GetType()));
    
        }

        [Test]
        public void ContextNotNull()
        {
            Assert.That(_ctx, Is.Not.Null);
        }
    }

}
