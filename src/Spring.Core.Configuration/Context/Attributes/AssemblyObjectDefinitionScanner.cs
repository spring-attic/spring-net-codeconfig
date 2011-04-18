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
using Spring.Objects.Factory.Support;

namespace Spring.Context.Attributes
{
    /// <summary>
    /// AssemblyTypeScanner that only accepts types that also meet the requirements of being ObjectDefintions.
    /// </summary>
    public class AssemblyObjectDefinitionScanner : RequiredConstraintAssemblyTypeScanner
    {
        private readonly List<Predicate<Assembly>> _assemblyExclusionPredicates = new List<Predicate<Assembly>>();

        private readonly IEnumerable<string> _springAssemblies = new List<string>
                                                                     {
                                                                         "Spring.Core",
                                                                         "Spring.Aop",
                                                                         "Spring.Data",
                                                                         "Spring.Services",
                                                                         "Spring.Messaging",
                                                                         "Spring.Messaging.Ems",
                                                                         "Spring.Messaging.Nms",
                                                                         "Spring.Template.Velocity",
                                                                         "Spring.Messaging.Quartz",
                                                                         "Spring.Testing.Microsoft",
                                                                         "Spring.Testing.Nunit",
                                                                         "Spring.Data.NHibernate12",
                                                                         "Spring.Data.NHibernate21",
                                                                         "Spring.Data.NHibernate20",
                                                                         "Spring.Data.NHibernate30",
                                                                         "Spring.Web",
                                                                         "Spring.Web.Extensions",
                                                                         "Spring.Web.Mvc",
                                                                     };


        /// <summary>
        /// Registers the defintions for types.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="typesToRegister">The types to register.</param>
        private void RegisterDefinitionsForTypes(IObjectDefinitionRegistry registry, IEnumerable<Type> typesToRegister)
        {
            foreach (Type type in typesToRegister)
            {
                ObjectDefinitionBuilder definition = ObjectDefinitionBuilder.GenericObjectDefinition(type);
                registry.RegisterObjectDefinition(definition.ObjectDefinition.ObjectTypeName,
                                                  definition.ObjectDefinition);
            }
        }


        /// <summary>
        /// Applies the assembly filters to the assembly candidates.
        /// </summary>
        /// <param name="assemblyCandidates">The assembly candidates.</param>
        /// <returns></returns>
        protected override IEnumerable<Assembly> ApplyAssemblyFiltersTo(IEnumerable<Assembly> assemblyCandidates)
        {
            return assemblyCandidates.Where(
                delegate(Assembly candidate) { return IsIncludedAssembly(candidate) && !IsExcludedAssembly(candidate); });
        }

        /// <summary>
        /// Determines whether the specified candidate is and excluded assembly.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// 	<c>true</c> if the specified candidate is an excluded assembly ; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsExcludedAssembly(Assembly candidate)
        {
            return _assemblyExclusionPredicates.Any(delegate(Predicate<Assembly> exclude) { return exclude(candidate); });
        }

        /// <summary>
        /// Determines whether the required constraint is satisfied by the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the required constraint is satisfied by the specified type; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsRequiredConstraintSatisfiedBy(Type type)
        {
            if (!type.Assembly.ReflectionOnly)
            {
                return Attribute.GetCustomAttribute(type, typeof (ConfigurationAttribute), true) != null &&
                       !type.IsAbstract;
            }

            bool satisfied = false;

            foreach (CustomAttributeData customAttributeData in CustomAttributeData.GetCustomAttributes(type))
            {
                if (customAttributeData.Constructor.DeclaringType.FullName == typeof (ConfigurationAttribute).FullName &&
                    !type.IsAbstract)
                {
                    satisfied = true;
                    break;
                }
            }

            return satisfied;
        }

        /// <summary>
        /// Sets the default filters.
        /// </summary>
        protected override void SetDefaultFilters()
        {
            //set the built-in defaults
            base.SetDefaultFilters();

            //add the desired assembly exclusions to the list
            _assemblyExclusionPredicates.Add(
                delegate(Assembly a) { return _springAssemblies.Contains(a.GetName().Name); });
            _assemblyExclusionPredicates.Add(delegate(Assembly a) { return a.GetName().Name.StartsWith("System."); });
            _assemblyExclusionPredicates.Add(delegate(Assembly a) { return a.GetName().Name.StartsWith("Microsoft."); });
            _assemblyExclusionPredicates.Add(delegate(Assembly a) { return a.GetName().Name == "mscorlib"; });
            _assemblyExclusionPredicates.Add(delegate(Assembly a) { return a.GetName().Name == "System"; });
        }

        /// <summary>
        /// Scans the and register types.
        /// </summary>
        /// <param name="registry">The registry within which to register the types.</param>
        public virtual void ScanAndRegisterTypes(IObjectDefinitionRegistry registry)
        {
            IEnumerable<Type> configTypes = base.Scan();

            //if we have at least one config class, ensure the post-processor is registered
            if (configTypes.Count() > 0)
            {
                AttributeConfigUtils.RegisterAttributeConfigProcessors(registry);
            }

            RegisterDefinitionsForTypes(registry, configTypes);
        }
    }
}