using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Spring.Context.Attributes;
using Spring.Context.Config;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Xml;
using Spring.Context.Support;
using Spring.Stereotype;

namespace Spring.Context.Attributes
{
    [TestFixture]
    public class XmlAssemblyTypeScannerTest
    {
        private IApplicationContext _applicationContext;

        [SetUp]
        public void Setup()
        {
            NamespaceParserRegistry.RegisterParser(typeof(ContextNamespaceParser));
        }

        [Test]
        public void IncludeRegExpressionFilter()
        {
            _applicationContext = new XmlApplicationContext(ReadOnlyXmlTestResource.GetFilePath("ConfigFiles.TypeScannerTestRegExInclude.xml", GetType()));

            Assert.That(_applicationContext.GetObjectDefinitionNames().Count, Is.EqualTo(3));
            Assert.That(_applicationContext.GetObject("SomeIncludeType1"), Is.Not.Null);
            Assert.That(delegate { _applicationContext.GetObject("SomeExcludeType"); }, Throws.Exception.TypeOf<NoSuchObjectDefinitionException>());
        }

        [Test]
        public void IncludeMultipleRegExpressionFilter()
        {
            _applicationContext = new XmlApplicationContext(ReadOnlyXmlTestResource.GetFilePath("ConfigFiles.TypeScannerTestRegExInclude2.xml", GetType()));

            Assert.That(_applicationContext.GetObjectDefinitionNames().Count, Is.EqualTo(5));
            Assert.That(_applicationContext.GetObject("SomeIncludeType1"), Is.Not.Null);
            Assert.That(_applicationContext.GetObject("SomeIncludeType2"), Is.Not.Null);
            Assert.That(delegate { _applicationContext.GetObject("SomeExcludeType"); }, Throws.Exception.TypeOf<NoSuchObjectDefinitionException>());
        }

        [Test]
        public void ExcludeRegExpressionFilter()
        {
            _applicationContext = new XmlApplicationContext(ReadOnlyXmlTestResource.GetFilePath("ConfigFiles.TypeScannerTestRegExExclude.xml", GetType()));

            Assert.That(_applicationContext.GetObjectDefinitionNames().Count, Is.EqualTo(5));
            Assert.That(_applicationContext.GetObject("SomeIncludeType1"), Is.Not.Null);
            Assert.That(delegate { _applicationContext.GetObject("SomeExcludeType"); }, Throws.Exception.TypeOf<NoSuchObjectDefinitionException>());
        }

        [Test]
        public void IncludeAttributeExpressionFilter()
        {
            _applicationContext = new XmlApplicationContext(ReadOnlyXmlTestResource.GetFilePath("ConfigFiles.TypeScannerTestAttributeInclude.xml", GetType()));

            Assert.That(_applicationContext.GetObjectDefinitionNames().Count, Is.EqualTo(3));
            Assert.That(_applicationContext.GetObject("SomeIncludeType1"), Is.Not.Null);
            Assert.That(delegate { _applicationContext.GetObject("SomeExcludeType"); }, Throws.Exception.TypeOf<NoSuchObjectDefinitionException>());
        }

        [Test]
        public void ExcludeAttributeExpressionFilter()
        {
            _applicationContext = new XmlApplicationContext(ReadOnlyXmlTestResource.GetFilePath("ConfigFiles.TypeScannerTestAttributeExclude.xml", GetType()));

            Assert.That(_applicationContext.GetObjectDefinitionNames().Count, Is.EqualTo(5));
            Assert.That(_applicationContext.GetObject("SomeIncludeType2"), Is.Not.Null);
            Assert.That(_applicationContext.GetObject("SomeExcludeType"), Is.Not.Null);
            Assert.That(delegate { _applicationContext.GetObject("SomeIncludeType1"); }, Throws.Exception.TypeOf<NoSuchObjectDefinitionException>());
        }


    }
}

namespace XmlAssemblyTypeScanner.Test.Include1
{
    [Service]
    [Configuration]
    public class SomeIncludeConfiguration
    {
        [ObjectDef]
        public virtual SomeIncludeType1 SomeIncludeType1()
        {
            return new SomeIncludeType1();           
        }
    }

    public class SomeIncludeType1
    {
    }
}

namespace XmlAssemblyTypeScanner.Test.Include2
{
    [Repository]
    [Configuration]
    public class SomeIncludeConfiguration
    {
        [ObjectDef]
        public virtual SomeIncludeType2 SomeIncludeType2()
        {
            return new SomeIncludeType2();
        }
    }

    public class SomeIncludeType2
    {
    }
}

namespace XmlAssemblyTypeScanner.Test.Include
{
    [Configuration]
    public class SomeExcludeConfiguration
    {
        
        [ObjectDef]
        public virtual SomeExcludeType SomeExcludeType()
        {
            return new SomeExcludeType();
        }
    }

    public class SomeExcludeType
    {
    }
}
