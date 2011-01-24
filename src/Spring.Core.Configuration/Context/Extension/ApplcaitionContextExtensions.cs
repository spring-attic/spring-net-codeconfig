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

using Spring.Objects.Factory;


namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
    }
}





namespace Spring.Context
{
    public static class ApplcaitionContextExtensions
    {
        public static T GetObject<T>(this IApplicationContext context, string name)
        {
            return (T)context.GetObject(name, typeof(T));
        }

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
