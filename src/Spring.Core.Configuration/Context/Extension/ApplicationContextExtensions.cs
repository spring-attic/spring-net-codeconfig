#region License

/*
 * Copyright © 2010-2011 the original author or authors.
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

using Spring.Objects.Factory;

#if(! DotNetVersion35)

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Manufactured Extension Attribute to permit .NET 2.0 to support extension methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class ExtensionAttribute : Attribute
    {
    }
}

#endif


namespace Spring.Context
{
    /// <summary>
    /// Generic extensions for IApplicationContext
    /// </summary>
    public static class ApplicationContextExtensions
    {
        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <typeparam name="T">Type of Object to return.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T GetObject<T>(this IApplicationContext context, string name)
        {
            return (T)context.GetObject(name, typeof(T));
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <typeparam name="T">Type of Object to return.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static T GetObject<T>(this IApplicationContext context)
        {
            string[] objectNamesForType = context.GetObjectNamesForType(typeof(T));
            if ((objectNamesForType == null) || (objectNamesForType.Length == 0))
            {
                throw new NoSuchObjectDefinitionException(typeof(T).FullName, "Requested Type not Defined in the Context.");
            }
            return context.GetObject<T>(objectNamesForType[0]);
        }
    }
}

namespace Spring.Context.Support
{
}
