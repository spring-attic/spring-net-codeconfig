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
using Spring.Objects.Factory.Xml;
using Spring.Objects.Factory.Support;
using Spring.Util;
using Spring.Context.Attributes;
using Spring.Objects.Factory.Config;

namespace Spring.Context.Config
{
    public class AttributeConfigObjectDefinitionParser : IObjectDefinitionParser
    {
        /// <summary>
        ///  The object name of the internally managed configuration attribure processor
        /// </summary>
        public static readonly string CONFIGURATION_ATTRIBUTE_PROCESSOR_OBJECT_NAME = "Spring.Context.Attributes.InternalConfigurationAttributeProcessor";


        private static readonly Type ConfigurationClassPostProcessorType = typeof(ConfigurationClassPostProcessor);

        public IObjectDefinition ParseElement(System.Xml.XmlElement element, ParserContext parserContext)
        {
            IObjectDefinitionRegistry registry = parserContext.ReaderContext.Registry;
            
            AssertUtils.ArgumentNotNull(registry, "registry");

            if (!registry.ContainsObjectDefinition(CONFIGURATION_ATTRIBUTE_PROCESSOR_OBJECT_NAME))
            {
                RootObjectDefinition objectDefinition = new RootObjectDefinition(ConfigurationClassPostProcessorType);
                objectDefinition.Role = ObjectRole.ROLE_INFRASTRUCTURE;
                registry.RegisterObjectDefinition(CONFIGURATION_ATTRIBUTE_PROCESSOR_OBJECT_NAME, objectDefinition);
            }
            
            return null;
        }
    }
}
