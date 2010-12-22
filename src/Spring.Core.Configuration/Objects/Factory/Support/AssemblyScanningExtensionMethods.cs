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
using System.Reflection;
using System.Linq;
using Spring.Context.Attributes;

namespace Spring.Objects.Factory.Support
{
    public static class AssemblyScanningExtensionMethods
    {
        public static void Scan(this IObjectDefinitionRegistry registry, IAssemblyTypeScanner scanner)
        {
            IEnumerable<Type> configTypes = scanner.Scan();

            //if we have at least one config class, ensure the post-processor is registered
            if (configTypes.Count() > 0)
            {
                //TODO more fine grained registration of just
                AttributeConfigUtils.RegisterAttributeConfigProcessors(registry);                
            }

            RegisiterDefintionsForTypes(configTypes, registry);
        }

        public static void Scan(this IObjectDefinitionRegistry registry)
        {
            Scan(registry, new AssemblyObjectDefinitionScanner());
        }

        public static void Scan(this IObjectDefinitionRegistry registry, Predicate<Type> typePredicate)
        {
            Scan(registry, null, ta => true, typePredicate);
        }

        public static void Scan(this IObjectDefinitionRegistry registry, string assemblyScanPath, Predicate<Assembly> assemblyPredicate, Predicate<Type> typePredicate)
        {
            //create a scanner instance using the scan path
            IAssemblyTypeScanner scanner = new AssemblyObjectDefinitionScanner(assemblyScanPath);

            //configure the scanner per the provided constraints
            scanner.WithAssemblyFilter(assemblyPredicate).WithIncludeFilter(typePredicate);

            //pass the scanner to primary Scan method to actually do the work
            Scan(registry, scanner);
        }

        public static void Scan(this IObjectDefinitionRegistry registry, Predicate<Assembly> assemblyPredicate, Predicate<Type> typePredicate)
        {
            Scan(registry, null, assemblyPredicate, typePredicate);
        }

        public static void Scan(this IObjectDefinitionRegistry registry, Predicate<Assembly> assemblyPredicate)
        {
            Scan(registry, null, assemblyPredicate, t => true);
        }

        #region Obsolete
        /// <summary>
        /// Ensures the configuration class post processor is registered for.
        /// </summary>
        /// <param name="registry">The registry.</param>
        private static void EnsureConfigurationClassPostProcessorIsRegisteredFor(IObjectDefinitionRegistry registry)
        {
            var postProcessorBuilder = ObjectDefinitionBuilder.GenericObjectDefinition(typeof(ConfigurationClassPostProcessor));
            if (!registry.ContainsObjectDefinition(postProcessorBuilder.ObjectDefinition.ObjectTypeName))
            {
                registry.RegisterObjectDefinition(postProcessorBuilder.ObjectDefinition.ObjectTypeName, postProcessorBuilder.ObjectDefinition);
            }
        }
        #endregion

        /// <summary>
        /// Regisiters the defintions for types.
        /// </summary>
        /// <param name="typesToRegister">The types to register.</param>
        /// <param name="registry">The registry.</param>
        private static void RegisiterDefintionsForTypes(IEnumerable<Type> typesToRegister, IObjectDefinitionRegistry registry)
        {
            foreach (Type type in typesToRegister)
            {
                ObjectDefinitionBuilder definition = ObjectDefinitionBuilder.GenericObjectDefinition(type);
                registry.RegisterObjectDefinition(definition.ObjectDefinition.ObjectTypeName, definition.ObjectDefinition);
            }
        }

    }
}
