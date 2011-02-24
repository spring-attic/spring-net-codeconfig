#region License

/*
 * Copyright © 2002-2010 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Xml;
using Spring.Context.Attributes;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;
using Spring.Util;

namespace Spring.Context.Config
{
    /// <summary>
    /// Object Defintion Parser for interpreting <see cref="ConfigurationAttribute"/> classes when primary configuration is peformed via XML.
    /// </summary>
    public class AttributeConfigObjectDefinitionParser : IObjectDefinitionParser
    {
        /// <summary>
        ///  The object name of the internally managed configuration attribute processor
        /// </summary>
        public static readonly string CONFIGURATION_ATTRIBUTE_PROCESSOR_OBJECT_NAME =
            "Spring.Context.Attributes.InternalConfigurationAttributeProcessor";

        private static readonly Type ConfigurationClassPostProcessorType = typeof (ConfigurationClassPostProcessor);

        /// <summary>
        /// Parse the specified XmlElement and register the resulting
        /// ObjectDefinitions with the <see cref="P:Spring.Objects.Factory.Xml.ParserContext.Registry"/> IObjectDefinitionRegistry
        /// embedded in the supplied <see cref="T:Spring.Objects.Factory.Xml.ParserContext"/>
        /// </summary>
        /// <param name="element">The element to be parsed.</param>
        /// <param name="parserContext">TThe object encapsulating the current state of the parsing process.
        /// Provides access to a IObjectDefinitionRegistry</param>
        /// <returns>The primary object definition.</returns>
        /// <remarks>
        /// 	<p>
        /// This method is never invoked if the parser is namespace aware
        /// and was called to process the root node.
        /// </p>
        /// </remarks>
        public IObjectDefinition ParseElement(XmlElement element, ParserContext parserContext)
        {
            IObjectDefinitionRegistry registry = parserContext.ReaderContext.Registry;

            AssertUtils.ArgumentNotNull(registry, "registry");

            if (!registry.ContainsObjectDefinition(CONFIGURATION_ATTRIBUTE_PROCESSOR_OBJECT_NAME))
            {
                var objectDefinition = new RootObjectDefinition(ConfigurationClassPostProcessorType);
                objectDefinition.Role = ObjectRole.ROLE_INFRASTRUCTURE;
                registry.RegisterObjectDefinition(CONFIGURATION_ATTRIBUTE_PROCESSOR_OBJECT_NAME, objectDefinition);
            }

            return null;
        }
    }
}