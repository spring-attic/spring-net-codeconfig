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
using System.Reflection;
using Spring.Objects.Factory.Parsing;

namespace Spring.Context.Attributes
{
    /// <summary>
    /// Represents a [Configuration] class method marked with the [Definition] attribute.
    /// </summary>
    public class ConfigurationClassMethod
    {
        private readonly ConfigurationClass _configurationClass;

        private readonly MethodInfo _methodInfo;

        /// <summary>
        /// Initializes a new instance of the ConfigurationClassMethod class.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="configurationClass"></param>
        public ConfigurationClassMethod(MethodInfo methodInfo, ConfigurationClass configurationClass)
        {
            _methodInfo = methodInfo;
            _configurationClass = configurationClass;
        }

        public ConfigurationClass ConfigurationClass
        {
            get { return _configurationClass; }
        }

        public MethodInfo MethodMetadata
        {
            get { return _methodInfo; }
        }

        public Location ResourceLocation
        {
            get { return new Location(_configurationClass.Resource, _methodInfo); }
        }

        public override string ToString()
        {
            return string.Format("{0}:name={1},declaringClass={2}", GetType().Name, _methodInfo.Name,
                                 _methodInfo.DeclaringType.FullName);
        }

        public void Validate(IProblemReporter problemReporter)
        {
            //TODO: shouldn't this be "if method has Definition attribute" instead of "if class has Configuration attribute" --????
            if (
                Attribute.GetCustomAttribute(ConfigurationClass.ConfigurationClassType, typeof (ConfigurationAttribute)) !=
                null)
            {
                if (!MethodMetadata.IsVirtual)
                {
                    problemReporter.Error(new NonVirtualMethodError(MethodMetadata.Name, ResourceLocation));
                }

                if (MethodMetadata.IsStatic)
                {
                    problemReporter.Error(new StaticMethodError(MethodMetadata.Name, ResourceLocation));
                }

                if (MethodMetadata.GetParameters().Length != 0)
                {
                    problemReporter.Error(new MethodWithParametersError(MethodMetadata.Name, ResourceLocation));
                }
            }
        }

        #region Nested type: MethodWithParametersError

        private class MethodWithParametersError : Problem
        {
            public MethodWithParametersError(string methodName, Location location)
                : base(
                    String.Format(
                        "Method '{0}' must not accept parameters; remove the method's parameters to continue.",
                        methodName), location)
            {
            }
        }

        #endregion

        #region Nested type: NonVirtualMethodError

        private class NonVirtualMethodError : Problem
        {
            public NonVirtualMethodError(string methodName, Location location)
                : base(String.Format("Method '{0}' must be public virtual; change the method's modifiers to continue.",
                                     methodName), location)
            {
            }
        }

        #endregion

        #region Nested type: StaticMethodError

        private class StaticMethodError : Problem
        {
            public StaticMethodError(string methodName, Location location)
                : base(
                    String.Format("Method '{0}' must not be static; remove the method's static modifier to continue.",
                                  methodName), location)
            {
            }
        }

        #endregion
    }
}