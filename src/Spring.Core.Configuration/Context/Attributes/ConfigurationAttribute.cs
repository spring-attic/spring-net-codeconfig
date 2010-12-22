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
using Spring.Stereotype;

namespace Spring.Context.Attributes
{
    /// <summary>
    /// Indicates that a class declares one or more <see cref="Definition"/> methods and may be processed
    /// by the Spring container to generate object definitions and service requests for those objects
    /// at runtime.
    ///
    /// <para>Configuration is meta-annotated as a {@link Component}, therefore Configuration
    /// classes are candidates for component-scanning and may also take advantage of
    /// <see cref="AutoWired"/> at the field and method but not at the constructor level.
    /// </para>
    /// <para>May be used in conjunction with the <see cref="Lazy"/> attribute to indicate that all object
    /// methods declared within this class are by default lazily initialized.
    ///</para>
    /// <h3>Constraints</h3>
    /// <ul>
    ///    <li>Configuration classes must be non-final</li>
    ///    <li>Configuration classes must be non-local (may not be declared within a method)</li>
    ///    <li>Configuration classes must have a default/no-arg constructor and may not use
    ///        {@link Autowired} constructor parameters</li>
    /// </ul>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationAttribute : ComponentAttribute
    {

        /// <summary>
        /// Initializes a new instance of the ConfigurationAttribute class.
        /// </summary>
        public ConfigurationAttribute()
        {
            
        }

        private string _name;

        /// <summary>
        /// Initializes a new instance of the Configuration class.
        /// </summary>
        /// <param name="name"></param>
        public ConfigurationAttribute(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Explicitly specify the name of the Spring object definition associated
        /// with this Configuration class.  If left unspecified (the common case),
        /// a object name will be automatically generated.
        ///
        /// <para>The custom name applies only if the Configuration class is picked up via
        /// component scanning or supplied directly to a <see cref="AnnotationConfigApplicationContext"/>.
        /// If the Configuration class is registered as a traditional XML object definition,
        /// the name/id of the object element will take precedence.
        /// </para>
        /// <see cref="Spring.Objects.Factory.Support.DefaultObjectNameGenerator"/>
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

    }
}
