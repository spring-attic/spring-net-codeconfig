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
using System.Reflection;
using NUnit.Framework;
using Spring.Util;

namespace Spring.Context.Attributes
{
    [TestFixture]
    public class AssemblyTypeScannerTests
    {

        private class Scanner : AssemblyTypeScanner
        {
            public Scanner(string folderScanPath)
                : base(folderScanPath)
            { }

            public Scanner()
                : base(null)
            { }

            protected override bool IsCompoundPredicateSatisfiedBy(Type type)
            {
                return IsIncludedType(type) && !IsExcludedType(type);

            }
        }

        private Scanner _scanner;

        private List<Predicate<Type>> ExcludePredicates
        {
            get
            {
                //get at the collection of excludePredicates from the private field
                //(yuck!-- test smell, but at least its wrapped up in a neat private property getter!)
                return (List<Predicate<Type>>)(ReflectionUtils.GetInstanceFieldValue(_scanner, "TypeExclusionPredicates"));
            }
        }

        private List<Predicate<Type>> IncludePredicates
        {
            get
            {
                //get at the collection of includePredicates from the private field
                //(yuck!-- test smell, but at least its wrapped up in a neat private property getter!)
                return (List<Predicate<Type>>)(ReflectionUtils.GetInstanceFieldValue(_scanner, "TypeInclusionPredicates"));
            }
        }

        private List<IEnumerable<Type>> TypeSources
        {
            get
            {
                //get at the collection of typeSources from the private field
                //(yuck!-- test smell, but at least its wrapped up in a neat private property getter!)
                return (List<IEnumerable<Type>>)(ReflectionUtils.GetInstanceFieldValue(_scanner, "TypeSources"));
            }
        }

        [SetUp]
        public void _TestSetup()
        {
            _scanner = new Scanner();
        }

        [Test]
        public void AssemblyHavingType_T_Adds_Assembly()
        {
            _scanner.AssemblyHavingType<Spring.Core.IOrdered>();
            Assert.That(TypeSources.Any(t => t.Contains(typeof(Spring.Core.IOrdered))));
        }

        [Test]
        public void IncludeType_T_Adds_Type()
        {
            _scanner.IncludeType<Spring.Core.IOrdered>();
            _scanner.IncludeType<Spring.Core.IPriorityOrdered>();

            IncludePredicates.Any(p => p(typeof(Spring.Core.IOrdered)));
            IncludePredicates.Any(p => p(typeof(Spring.Core.IPriorityOrdered)));
        }

        [Test]
        public void WithExcludeFilter_Excludes_Type()
        {
            _scanner.IncludeType<TheConfigurationClass>();
            _scanner.IncludeType<TheImportedConfigurationClass>();
            _scanner.WithExcludeFilter(t => t.Name.StartsWith("TheImported"));

            IEnumerable<Type> types = _scanner.Scan();

            Assert.That(types, Contains.Item((typeof(TheConfigurationClass))));
            Assert.False(types.Contains(typeof(TheImportedConfigurationClass)));
        }

        [Test]
        public void WithIncludeFilter_Includes_Types()
        {
            _scanner.WithIncludeFilter(t => t.Name.Contains("ConfigurationClass"));

            Assert.That(_scanner.Scan(), Contains.Item((typeof(TheConfigurationClass))));
            Assert.That(_scanner.Scan(), Contains.Item((typeof(TheImportedConfigurationClass))));
        }

    }
}
