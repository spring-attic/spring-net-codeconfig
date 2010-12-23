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
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Spring.Util;

namespace Spring.Context.Attributes
{
    public abstract class AssemblyTypeScanner : IAssemblyTypeScanner
    {
        protected readonly List<Predicate<Assembly>> AssemblyPredicates = new List<Predicate<Assembly>>();

        protected readonly List<Predicate<Type>> ExcludePredicates = new List<Predicate<Type>>();

        protected string FolderScanPath;

        protected readonly List<Predicate<Type>> IncludePredicates = new List<Predicate<Type>>();

        protected static ILog Logger = LogManager.GetLogger(typeof(AssemblyTypeScanner));

        protected readonly List<IEnumerable<Type>> TypeSources = new List<IEnumerable<Type>>();

        /// <summary>
        /// Initializes a new instance of the AssemblyTypeScanner class.
        /// </summary>
        /// <param name="folderScanPath"></param>
        protected AssemblyTypeScanner(string folderScanPath)
        {
            if (!string.IsNullOrEmpty(folderScanPath))
            {
                FolderScanPath = folderScanPath;
            }
            else
            {
                FolderScanPath = GetCurrentBinDirectoryPath();
            }
        }

        /// <summary>
        /// Initializes a new instance of the AssemblyTypeScanner class.
        /// </summary>
        protected AssemblyTypeScanner()
        {

        }

        public IAssemblyTypeScanner AssemblyHavingType<T>()
        {
            TypeSources.Add(new AssemblyTypeSource((typeof(T).Assembly)));
            return this;
        }

        public IAssemblyTypeScanner ExcludeType<T>()
        {
            ExcludePredicates.Add(t => t == typeof(T));
            return this;
        }

        public IAssemblyTypeScanner IncludeType<T>()
        {
            IncludePredicates.Add(t => t == typeof(T));
            return this;
        }

        public IAssemblyTypeScanner IncludeTypes(IEnumerable<Type> typeSource)
        {
            AssertUtils.ArgumentNotNull(typeSource, "typeSource");
            TypeSources.Add(typeSource);
            IncludePredicates.Add(t => typeSource.Any(t1 => t1 == t));
            return this;
        }

        public virtual IEnumerable<Type> Scan()
        {
            SetDefaultFilters();

            IList<Type> types = new List<Type>();

            foreach (Assembly assembly in GetAllMatchingAssemblies())
            {
                TypeSources.Add(new AssemblyTypeSource(assembly));
            }

            foreach (var typeSource in TypeSources)
            {
                foreach (Type type in typeSource)
                {
                    if (IsIncludedType(type) && !IsExcludedType(type) && FinalRequiredConstraintIsSatisfiedBy(type))
                    {
                        types.Add(type);
                    }
                }
            }

            return types;
        }

        public IAssemblyTypeScanner WithAssemblyFilter(Predicate<Assembly> assemblyPredicate)
        {
            AssemblyPredicates.Add(assemblyPredicate);
            return this;
        }

        public IAssemblyTypeScanner WithExcludeFilter(Predicate<Type> predicate)
        {
            ExcludePredicates.Add(predicate);
            return this;
        }

        public IAssemblyTypeScanner WithIncludeFilter(Predicate<Type> predicate)
        {
            IncludePredicates.Add(predicate);
            return this;
        }

        protected virtual bool IsExcludedType(Type type)
        {
            return ExcludePredicates.Any(exclude => exclude(type));
        }

        protected virtual bool IsIncludedAssembly(Assembly assembly)
        {
            return AssemblyPredicates.Any(include => include(assembly));
        }

        protected virtual bool IsIncludedType(Type type)
        {
            return IncludePredicates.Any(include => include(type));
        }

        protected virtual void SetDefaultFilters()
        {
            if (IncludePredicates.Count == 0)
                IncludePredicates.Add(t => true);

            if (ExcludePredicates.Count == 0)
                ExcludePredicates.Add(t => false);

            if (AssemblyPredicates.Count == 0)
                AssemblyPredicates.Add(a => true);
        }

        protected virtual bool FinalRequiredConstraintIsSatisfiedBy(Type type)
        {
            return true;
        }

        private IEnumerable<Assembly> ApplyAssemblyFiltersTo(IEnumerable<Assembly> assemblyCandidates)
        {
            IList<Assembly> matchingAssemblies = new List<Assembly>();
            foreach (Assembly assemblyCandidate in assemblyCandidates)
                if (IsIncludedAssembly(assemblyCandidate))
                    matchingAssemblies.Add(assemblyCandidate);
            return matchingAssemblies;
        }

        private IEnumerable<Assembly> GetAllAssembliesInPath(string folderPath)
        {
            List<Assembly> assemblies = new List<Assembly>();
            AddFilesForExtension(folderPath, "*.dll", assemblies);
            //AddFilesForExtension(folderPath, "*.exe", assemblies);

            if (Logger.IsDebugEnabled)
            {
                Assembly[] assemblyArray = assemblies.ToArray();                
                Logger.Debug("Assemblies to be scanned: " + StringUtils.ArrayToCommaDelimitedString(assemblyArray));
            }
            return assemblies;
        }



        public void AddFilesForExtension(string folderPath, string extension, IList<Assembly> assemblies)
        {
            IEnumerable<string> files = Directory.GetFiles(folderPath, extension);
            foreach (string file in files)
                try
                {
                    
                    assemblies.Add(Assembly.LoadFrom(file));
                }
                catch (Exception ex)
                {
                    //log and swallow everything that might go wrong here...
                    if (Logger.IsDebugEnabled)
                        Logger.Debug("Failed to load type while scanning Assemblies for Defintions!", ex);
                }
        }


         

        private IEnumerable<Assembly> GetAllMatchingAssemblies()
        {
            IEnumerable<Assembly> assemblyCandidates = GetAllAssembliesInPath(FolderScanPath);
            return ApplyAssemblyFiltersTo(assemblyCandidates);
        }

        private string GetCurrentBinDirectoryPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

    }

}