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
using System.Collections.Generic;
using System.Linq;
using Spring.Objects.Factory.Support;

namespace Spring.Context.Attributes
{
    public class AssemblyObjectDefinitionScanner : RequiredConstraintAssemblyTypeScanner
    {
        /// <summary>
        /// Initializes a new instance of the AssemblyObjectDefinitionScanner class.
        /// </summary>
        /// <param name="folderScanPath">The folder scan path.</param>
        public AssemblyObjectDefinitionScanner(string folderScanPath)
            : base(folderScanPath)
        { }

        /// <summary>
        /// Initializes a new instance of the AssemblyObjectDefinitionScanner class.
        /// </summary>
        public AssemblyObjectDefinitionScanner()
            : base(null)
        { }

        protected override bool IsRequiredConstraintSatisfiedBy(Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(ConfigurationAttribute), true) != null;
        }


        public void ScanAndRegisterTypes(IObjectDefinitionRegistry registry)
        {
            IEnumerable<Type> configTypes = base.Scan();

            //if we have at least one config class, ensure the post-processor is registered
            if (configTypes.Count() > 0)
            {
                AttributeConfigUtils.RegisterAttributeConfigProcessors(registry);
            }

            RegisiterDefintionsForTypes(registry, configTypes);

        }


        /// <summary>
        /// Regisiters the defintions for types.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="typesToRegister">The types to register.</param>
        private void RegisiterDefintionsForTypes(IObjectDefinitionRegistry registry, IEnumerable<Type> typesToRegister)
        {
            foreach (Type type in typesToRegister)
            {
                ObjectDefinitionBuilder definition = ObjectDefinitionBuilder.GenericObjectDefinition(type);
                registry.RegisterObjectDefinition(definition.ObjectDefinition.ObjectTypeName, definition.ObjectDefinition);
            }
        }

    }
}
