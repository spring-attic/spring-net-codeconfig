using System;
using System.Linq;
using NUnit.Framework;

namespace Spring.Context.Attributes
{
    [TestFixture]
    public class AttributeConfigUtilsTests
    {
        [Test]
        public void CanVerifyAttributeExists()
        {
            Assert.That(AttributeConfigUtils.ReflectionOnlyTypeHasAttribute(typeof(ClassWithConfigurationAttribute), typeof(ConfigurationAttribute)), Is.True);
        }

        [Test]
        public void CanVerifyAttributeDoesNotExist()
        {
            Assert.That(AttributeConfigUtils.ReflectionOnlyTypeHasAttribute(typeof(object), typeof(ConfigurationAttribute)), Is.Not.True);
        }

        //[Test]
        //public void CanRetrieveNamedConstructorArgValue()
        //{
        //    var types = AttributeConfigUtils.ReflectionOnlyTypeGetValueFromAttributeConstructor<Type[]>(
        //        typeof (ClassWithConfigurationAttribute), typeof (ImportAttribute), 0);

        //    Assert.That(types[0], Is.EqualTo(typeof(ClassToImport)));
        //    Assert.That(types[1], Is.EqualTo(typeof(object)));
        //}
    }

    [Configuration]
    [Import(new Type[]{typeof(ClassToImport), typeof(object)})]
    public class ClassWithConfigurationAttribute
    {
        
    }

    public class ClassToImport
    {

    }
}