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
using Spring.Core.IO;
using Spring.Collections.Generic;
using Spring.Objects.Factory.Parsing;

namespace Spring.Context.Attributes
{
    public class ConfigurationClass
    {
        private Type _configurationClassType;

        private IDictionary<string, Type> _importedResources = new Dictionary<string, Type>();

        private ISet<ConfigurationClassMethod> _methods = new HashedSet<ConfigurationClassMethod>();

        private string _objectName;

        //TODO: determine how (and to what value!) this should be set during parsing
        // the *only* place this value is ultimately later used is in the ProblemReporter to help construct meaningful
        // error messages from the .Location property
        // (in JAVA impl its set to the location on the classpath the config class was found but we've no similar
        // setting in .NET worth capturing...unless maybe we want to just capture the Assembly name itself?)
        private IResource _resource;

        /// <summary>
        /// Initializes a new instance of the ConfigurationClass class.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="type"></param>
        public ConfigurationClass(string objectName, Type type)
        {
            _objectName = objectName;
            _configurationClassType = type;
        }

        public Type ConfigurationClassType
        {
            get
            {
                return _configurationClassType;
            }
        }

        public IDictionary<string, Type> ImportedResources
        {
            get
            {
                return _importedResources;
            }
        }

        public ISet<ConfigurationClassMethod> Methods
        {
            get
            {
                return _methods;
            }
        }

        public string ObjectName
        {
            get
            {
                return _objectName;
            }
            set
            {
                _objectName = value;
            }
        }

        public IResource Resource
        {
            get
            {
                return _resource;
            }
        }

        public string SimpleName
        {
            get { return ConfigurationClassType.Name; }
        }

        public void AddImportedResource(string importedResource, Type readerClass)
        {
            _importedResources.Add(importedResource, readerClass);
        }

        public override bool Equals(object other)
        {
            return this == other || (other is ConfigurationClass && ConfigurationClassType.Name.Equals(((ConfigurationClass)other).ConfigurationClassType.Name));
        }

        public override int GetHashCode()
        {
            return ConfigurationClassType.Name.GetHashCode() * 14;
        }

        public void Validate(IProblemReporter problemReporter)
        {
            // A [Definition] method may only be overloaded through inheritance. No single
            // [Configuration] class may declare two [Definition] methods with the same name.
            const char hashDelim = '#';
            Dictionary<String, int> methodNameCounts = new Dictionary<String, int>();
            foreach (ConfigurationClassMethod method in _methods)
            {
                String dClassName = method.MethodMetadata.DeclaringType.FullName;
                String methodName = method.MethodMetadata.Name;
                String fqMethodName = dClassName + hashDelim + methodName;
                if (!methodNameCounts.ContainsKey(fqMethodName))
                {
                    methodNameCounts.Add(fqMethodName, 1);
                }
                else
                {
                    int currentCount = methodNameCounts[fqMethodName];
                    methodNameCounts.Add(fqMethodName, currentCount++);
                }
            }

            foreach (String methodName in methodNameCounts.Keys)
            {
                int count = methodNameCounts[methodName];
                if (count > 1)
                {
                    String shortMethodName = methodName.Substring(methodName.IndexOf(hashDelim) + 1);
                    problemReporter.Error(new ObjectMethodOverloadingProblem(shortMethodName, count, Resource, ConfigurationClassType));
                }
            }

            if (Attribute.GetCustomAttribute(_configurationClassType, typeof(ConfigurationAttribute)) != null)
            {

                if (ConfigurationClassType.IsSealed)
                {
                    problemReporter.Error(new SealedConfigurationProblem(SimpleName, Resource, ConfigurationClassType));

                }

                foreach (ConfigurationClassMethod method in _methods)
                {
                    method.Validate(problemReporter);
                }
            }
        }

        private class SealedConfigurationProblem : Problem
        {
            public SealedConfigurationProblem(string name, IResource resource, Type configurationClassType)
                : base(String.Format("[Configuration] class '{0}' may not be sealed. Remove the sealed modifier to continue.", name), new Location(resource, configurationClassType))
            { }

        }

        private class ObjectMethodOverloadingProblem : Problem
        {
            public ObjectMethodOverloadingProblem(string methodName, int count, IResource resource, Type configurationClassType)
                : base(String.Format("[Configuration] class '{0}' has {1} overloaded [Definiton] methods named '{2}'. " +
                    "Only one [Definition] method of a given name is allowed within each [Configuration] class.", configurationClassType.Name, count, methodName), new Location(resource, configurationClassType))
            { }

        }

    }
}
