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

using System.Xml;
using Spring.Context.Attributes;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;

namespace Spring.Context.Config
{
    public class ComponentScanObjectDefinitionParser : IObjectDefinitionParser
    {
        public IObjectDefinition ParseElement(XmlElement element, ParserContext parserContext)
        {
            AssemblyObjectDefinitionScanner scanner = ConfigureScanner(parserContext, element);
            IObjectDefinitionRegistry registry = parserContext.Registry;
            
            // Actually scan for objects definitions and register them.
            registry.Scan(scanner);

            // Register attribute config processors, if necessary.
            bool attributeConfig = true;
            if (element.HasAttribute("attribute-config"))
            {
                attributeConfig = bool.Parse(element.GetAttribute("attribute-config"));
            }
            if (attributeConfig)
            {
                AttributeConfigUtils.RegisterAttributeConfigProcessors(registry);
            }
            return null;
        }

        protected virtual AssemblyObjectDefinitionScanner ConfigureScanner(ParserContext parserContext, XmlElement element)
        {
            XmlReaderContext readerContext = parserContext.ReaderContext;
            bool useDefaultFilters = true;
            if (element.HasAttribute("use-default-filters")) 
            {
                useDefaultFilters = bool.Parse(element.GetAttribute("use-default-filters"));
            }

            //String assemblies = element.GetAttribute("assembly");

            //foldersPath
            AssemblyObjectDefinitionScanner scanner = new AssemblyObjectDefinitionScanner();
            //setBeanDefinitionDefaults
            //setAutowireCandidatePatterns
            
            /*
            	if (element.hasAttribute(RESOURCE_PATTERN_ATTRIBUTE)) {
			        scanner.setResourcePattern(element.getAttribute(RESOURCE_PATTERN_ATTRIBUTE));
		        }

		        try {
			        parseBeanNameGenerator(element, scanner);
		        }
		        catch (Exception ex) {
			        readerContext.error(ex.getMessage(), readerContext.extractSource(element), ex.getCause());
		        }

		        try {
			        parseScope(element, scanner);
		        }
		        catch (Exception ex) {
			        readerContext.error(ex.getMessage(), readerContext.extractSource(element), ex.getCause());
		        }

		        parseTypeFilters(element, scanner, readerContext, parserContext);
            */


            return scanner;
        }


    }
}
