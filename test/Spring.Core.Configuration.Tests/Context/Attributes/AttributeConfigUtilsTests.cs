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
            Assert.That(AttributeConfigUtils.ReflectionOnlyTypeHasAttribute(typeof(TheConfigurationClass), typeof(ConfigurationAttribute)), Is.True);
        }

        [Test]
        public void CanVerifyAttributeDoesNotExist()
        {
            Assert.That(AttributeConfigUtils.ReflectionOnlyTypeHasAttribute(typeof(object), typeof(ConfigurationAttribute)), Is.Not.True);
        }
    }
    
}