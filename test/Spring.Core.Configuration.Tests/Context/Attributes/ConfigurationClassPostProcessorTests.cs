using System;
using NUnit.Framework;
using Spring.Context.Config;
using Spring.Context.Support;
using Spring.Objects.Factory.Support;
using Spring.Context.Attributes;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Xml;

namespace Spring.Context.Attributes
{
    [TestFixture]
    public class ConfigurationClassPostProcessorTests : AbstractConfigurationClassPostProcessorTests
    {

        protected override void CreateApplicationContext()
        {
            GenericApplicationContext ctx = new GenericApplicationContext();

            var configDefinitionBuilder = ObjectDefinitionBuilder.GenericObjectDefinition(typeof(TheConfigurationClass));
            ctx.RegisterObjectDefinition(configDefinitionBuilder.ObjectDefinition.ObjectTypeName, configDefinitionBuilder.ObjectDefinition);

            var postProcessorDefintionBuilder = ObjectDefinitionBuilder.GenericObjectDefinition(typeof(ConfigurationClassPostProcessor));
            ctx.RegisterObjectDefinition(postProcessorDefintionBuilder.ObjectDefinition.ObjectTypeName, postProcessorDefintionBuilder.ObjectDefinition);

            Assert.That(ctx.ObjectDefinitionCount, Is.EqualTo(2));

            ctx.Refresh();

            _ctx = ctx;
        }

    }

}
