#region License

/*
 * Copyright 2002-2010 the original author or authors.
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
using System.Linq;
using NUnit.Framework;
using Spring.Context.Support;
using Spring.Context.Attributes;

namespace Spring.Objects.Factory.Support
{
    [TestFixture]
    public class CodeConfigApplicationContextTests
    {
        private CodeConfigApplicationContext _context;

        [SetUp]
        public void _TestSetup()
        {
            _context = new CodeConfigApplicationContext();
        }

        [Test]
        public void Can_Filter_For_Assembly_Based_On_Assembly_Metadata()
        {
            _context.ScanWithAssemblyFilter(a => a.GetName().Name.StartsWith("Spring.Core.Configuration."));
            _context.Refresh();

            AssertExpectedObjectsAreRegisteredWith(_context);
        }

        [Test]
        public void Can_Filter_For_Assembly_Containing_Specific_Type_But_Having_NO_Definitions()
        {
            //specifically filter assemblies for one that we *know* will result in NO [Configuration] types in it
            _context.ScanWithAssemblyFilter(assy => assy.GetTypes().Any(type => type.FullName.Contains(typeof(Spring.Core.IOrdered).Name)));
            _context.Refresh();

            Assert.That(_context.DefaultListableObjectFactory.ObjectDefinitionCount, Is.EqualTo(0));
        }

        [Test]
        public void Can_Filter_For_Assembly_Containing_Specific_Type()
        {
            _context.ScanWithAssemblyFilter(assy => assy.GetTypes().Any(type => type.FullName.Contains(typeof(MarkerTypeForScannerToFind).Name)));
            _context.Refresh();

            AssertExpectedObjectsAreRegisteredWith(_context);
        }

        [Test]
        public void Can_Filter_For_Specific_Type()
        {
            _context.ScanWithTypeFilter(type => ((Type)type).FullName.Contains(typeof(TheImportedConfigurationClass).Name));
            _context.Refresh();

            Assert.That(_context.DefaultListableObjectFactory.ObjectDefinitionCount, Is.EqualTo(4));
        }

        [Test]
        public void Can_Filter_For_Specific_Types_With_Compound_Predicate()
        {
            _context.ScanWithTypeFilter(type => ((Type)type).FullName.Contains(typeof(TheImportedConfigurationClass).Name) || ((Type)type).FullName.Contains(typeof(TheConfigurationClass).Name));
            _context.Refresh();

            AssertExpectedObjectsAreRegisteredWith(_context);
        }

        [Test]
        public void Can_Filter_For_Specific_Types_With_Multiple_Include_Filters()
        {
            var scanner = new AssemblyObjectDefinitionScanner();
            scanner.WithIncludeFilter(type => type.FullName.Contains(typeof(TheImportedConfigurationClass).Name));
            scanner.WithIncludeFilter(type => type.FullName.Contains(typeof(TheConfigurationClass).Name));

            _context.Scan(scanner);
            _context.Refresh();

            AssertExpectedObjectsAreRegisteredWith(_context);
        }

        [Test]
        public void Scanner()
        {
            AssemblyObjectDefinitionScanner scanner = new AssemblyObjectDefinitionScanner();
            scanner.AssemblyHavingType<TheConfigurationClass>();

        }

        [Test]
        public void Can_Perform_Scan_With_No_Filtering()
        {
            _context.ScanAllAssemblies();
            _context.Refresh();

            AssertExpectedObjectsAreRegisteredWith(_context);
        }

        private void AssertExpectedObjectsAreRegisteredWith(GenericApplicationContext context)
        {
            Assert.That(context.DefaultListableObjectFactory.ObjectDefinitionCount, Is.EqualTo(13));
        }

    }

    public class MarkerTypeForScannerToFind
    {

    }
}
