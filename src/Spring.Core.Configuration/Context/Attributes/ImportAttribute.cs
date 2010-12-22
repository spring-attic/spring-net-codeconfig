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

namespace Spring.Context.Attributes
{
    /// <summary>
    /// Indicates one or more <see cref="Configuration"/> classes to import.
    ///
    /// <para>Provides functionality equivalent to the :lt;import/&gt; element in Spring XML.
    /// Only supported for actual <see cref="Configuration"/>-attributed classes.
    /// </para>
    ///
    /// <para><see cref="Definition"/> definitions declared in imported <see cref="Configuration"/> classes
    /// should be accessed by using <see cref="AutoWired"/> injection.  Either the object
    /// itself can be autowired, or the configuration class instance declaring the object can be
    /// autowired.  The latter approach allows for explicit, IDE-friendly navigation between
    /// <see cref="Configuration"/> class methods.
    /// </para>
    ///
    /// <para>If XML or other non-<see cref="Configuration"/> object definition resources need to be
    /// imported, use <see cref="ImportResource"/>
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ImportAttribute : Attribute
    {
        private Type[] _types;

        /// <summary>
        /// Initializes a new instance of the Import class.
        /// </summary>
        /// <param name="types"></param>
        public ImportAttribute(Type[] types)
        {
            _types = types;
        }

        /// <summary>
        /// The <see cref="Configuration"/> class or classes to import.
        /// </summary>
        /// <value>The type.</value>
        public Type[] Types
        {
            get { return _types; }
            set
            {
                _types = value;
            }
        }

    }
}
