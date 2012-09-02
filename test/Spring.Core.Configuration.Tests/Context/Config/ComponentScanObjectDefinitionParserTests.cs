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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Spring.Context.Attributes;
using Spring.Context.Support;
using Spring.Objects.Factory.Xml;

namespace Spring.Context.Config
{
    [TestFixture]
    public class ComponentScanObjectDefinitionParserTests
    {
        private XmlApplicationContext _applicationContext;

        [SetUp]
        public void Setup()
        {
            NamespaceParserRegistry.RegisterParser(typeof(ContextNamespaceParser));
        }

        [Test]
        public void DontRegisterAttributeConfig()
        {
            _applicationContext = new XmlApplicationContext(ReadOnlyXmlTestResource.GetFilePath("ConfigFiles.ComponentScanAttributeConfigFalse.xml", GetType()));
            var objectDefintionNames = _applicationContext.ObjectFactory.GetObjectDefinitionNames();

            Assert.That(objectDefintionNames.Count, Is.EqualTo(0));
            Assert.That(objectDefintionNames.Contains(AttributeConfigUtils.CONFIGURATION_ATTRIBUTE_PROCESSOR_OBJECT_NAME), Is.False);
            Assert.That(objectDefintionNames.Contains(AttributeConfigUtils.AUTOWIRED_ATTRIBUTE_PROCESSOR_OBJECT_NAME), Is.False);
            Assert.That(objectDefintionNames.Contains(AttributeConfigUtils.REQUIRED_ATTRIBUTE_PROCESSOR_OBJECT_NAME), Is.False);
            Assert.That(objectDefintionNames.Contains(AttributeConfigUtils.INITDESTROY_ATTRIBUTE_PROCESSOR_OBJECT_NAME), Is.False);
        }

        [Test]
        public void RegisterAttributeConfig()
        {
            _applicationContext = new XmlApplicationContext(ReadOnlyXmlTestResource.GetFilePath("ConfigFiles.ComponentScanAttributeConfigTrue.xml", GetType()));
            var objectDefintionNames = _applicationContext.ObjectFactory.GetObjectDefinitionNames();

            Assert.That(objectDefintionNames.Count, Is.EqualTo(4));
            Assert.That(objectDefintionNames.Contains(AttributeConfigUtils.CONFIGURATION_ATTRIBUTE_PROCESSOR_OBJECT_NAME), Is.True);
            Assert.That(objectDefintionNames.Contains(AttributeConfigUtils.AUTOWIRED_ATTRIBUTE_PROCESSOR_OBJECT_NAME), Is.True);
            Assert.That(objectDefintionNames.Contains(AttributeConfigUtils.REQUIRED_ATTRIBUTE_PROCESSOR_OBJECT_NAME), Is.True);
            Assert.That(objectDefintionNames.Contains(AttributeConfigUtils.INITDESTROY_ATTRIBUTE_PROCESSOR_OBJECT_NAME), Is.True);
        }

    }
}
