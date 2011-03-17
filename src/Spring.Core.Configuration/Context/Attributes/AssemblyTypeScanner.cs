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
using System.Reflection;
using Common.Logging;
using Spring.Core;
using Spring.Util;

namespace Spring.Context.Attributes
{
    /// <summary>
    /// Scans Assebmlies for Types that satisfy a given set of constraints.
    /// </summary>
    public abstract class AssemblyTypeScanner : IAssemblyTypeScanner
    {
        /// <summary>
        /// Logger Instance.
        /// </summary>
        protected static ILog Logger = LogManager.GetLogger(typeof (AssemblyTypeScanner));
        
        /// <summary>
        /// Assembly Inclusion Predicates.
        /// </summary>
        protected readonly List<Predicate<Assembly>> AssemblyInclusionPredicates = new List<Predicate<Assembly>>();

        /// <summary>
        /// Type Exclusion Predicates.
        /// </summary>
        protected readonly List<Predicate<Type>> TypeExclusionPredicates = new List<Predicate<Type>>();

        /// <summary>
        /// Type Inclusion Predicates.
        /// </summary>
        protected readonly List<Predicate<Type>> TypeInclusionPredicates = new List<Predicate<Type>>();

        /// <summary>
        /// Assemblies to scan.
        /// </summary>
        protected readonly List<IEnumerable<Type>> TypeSources = new List<IEnumerable<Type>>();
        
        /// <summary>
        /// Fully-qualified path to the root folder from which to begin the recursive scan.
        /// </summary>
        protected string FolderScanPath;

        /// <summary>
        /// Initializes a new instance of the AssemblyTypeScanner class.
        /// </summary>
        /// <param name="folderScanPath"></param>
        protected AssemblyTypeScanner(string folderScanPath)
        {
            FolderScanPath = !string.IsNullOrEmpty(folderScanPath) ? folderScanPath : GetCurrentBinDirectoryPath();
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += MyReflectionOnlyResolveEventHandler;
        }

        /// <summary>
        /// Initializes a new instance of the AssemblyTypeScanner class.
        /// </summary>
        protected AssemblyTypeScanner()
            : this(string.Empty)
        {
        }

        #region IAssemblyTypeScanner Members

        /// <summary>
        /// Assemblies the type of the having.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IAssemblyTypeScanner AssemblyHavingType<T>()
        {
            TypeSources.Add(new AssemblyTypeSource((typeof (T).Assembly)));
            return this;
        }

        /// <summary>
        /// Excludes the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IAssemblyTypeScanner ExcludeType<T>()
        {
            TypeExclusionPredicates.Add(delegate(Type t) { return t.FullName == typeof (T).FullName; });
            return this;
        }

        /// <summary>
        /// Includes the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IAssemblyTypeScanner IncludeType<T>()
        {
            TypeInclusionPredicates.Add(delegate(Type t) { return t.FullName == typeof (T).FullName; });
            return this;
        }

        /// <summary>
        /// Includes the types.
        /// </summary>
        /// <param name="typeSource">The type source.</param>
        /// <returns></returns>
        public IAssemblyTypeScanner IncludeTypes(IEnumerable<Type> typeSource)
        {
            AssertUtils.ArgumentNotNull(typeSource, "typeSource");
            TypeSources.Add(typeSource);
            TypeInclusionPredicates.Add(
                delegate(Type t) { return typeSource.Any(delegate(Type t1) { return t1.FullName == t.FullName; }); });
            return this;
        }

        /// <summary>
        /// Performs the Scan, respecting all filter settings.
        /// </summary>
        /// <returns></returns>
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
                    if (IsCompoundPredicateSatisfiedBy(type))
                    {
                        types.Add(type);
                    }
                }
            }

            return EnsureAllTypesLoadedInAppDomain(types);
        }

        /// <summary>
        /// Adds the assembly filter.
        /// </summary>
        /// <param name="assemblyPredicate">The assembly predicate.</param>
        /// <returns></returns>
        public IAssemblyTypeScanner WithAssemblyFilter(Predicate<Assembly> assemblyPredicate)
        {
            AssemblyInclusionPredicates.Add(assemblyPredicate);
            return this;
        }

        /// <summary>
        /// Adds the exclude filter.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IAssemblyTypeScanner WithExcludeFilter(Predicate<Type> predicate)
        {
            TypeExclusionPredicates.Add(predicate);
            return this;
        }

        /// <summary>
        /// Adds the include filter.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IAssemblyTypeScanner WithIncludeFilter(Predicate<Type> predicate)
        {
            TypeInclusionPredicates.Add(predicate);
            return this;
        }

        #endregion

        private IEnumerable<Type> EnsureAllTypesLoadedInAppDomain(IEnumerable<Type> potentialReflectionOnlyTypes)
        {
            var actualAppDomainTypes = new List<Type>();

            foreach (Type type in potentialReflectionOnlyTypes)
            {
                if (type.Assembly.ReflectionOnly)
                {
                    try
                    {
                        Assembly.LoadFrom(type.Assembly.Location);
                    }
                    catch (Exception)
                    {
                        throw new CannotLoadObjectTypeException(
                            string.Format("Unable to load type {0} from assembly {1}", type.FullName,
                                          type.Assembly.Location));
                    }
                }

                actualAppDomainTypes.Add(Type.GetType(type.FullName + "," + type.Assembly.FullName));
            }

            return actualAppDomainTypes;
        }

        private IEnumerable<Assembly> GetAllAssembliesInPath(string folderPath)
        {
            var assemblies = new List<Assembly>();
            AddFilesForExtension(folderPath, "*.dll", assemblies);
            AddFilesForExtension(folderPath, "*.exe", assemblies);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(string.Format("Assemblies to be scanned: {0}", StringUtils.ArrayToCommaDelimitedString(assemblies.ToArray())));
            }
            return assemblies;
        }

        private IEnumerable<Assembly> GetAllMatchingAssemblies()
        {
            IEnumerable<Assembly> assemblyCandidates = GetAllAssembliesInPath(FolderScanPath);
            return ApplyAssemblyFiltersTo(assemblyCandidates);
        }

        private string GetCurrentBinDirectoryPath()
        {
            return string.IsNullOrEmpty(AppDomain.CurrentDomain.DynamicDirectory)
                       ? AppDomain.CurrentDomain.BaseDirectory
                       : AppDomain.CurrentDomain.DynamicDirectory;
        }

        private Assembly MyReflectionOnlyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);

            String asmToCheck = Path.GetDirectoryName(FolderScanPath) + "\\" + name.Name + ".dll";

            if (File.Exists(asmToCheck))
            {
                return Assembly.ReflectionOnlyLoadFrom(asmToCheck);
            }

            return ReflectionOnlyUtils.ReflectionOnlyLoadWithPartialName(name.Name);
        }

        /// <summary>
        /// Applies the assembly filters to the assembly candidates.
        /// </summary>
        /// <param name="assemblyCandidates">The assembly candidates.</param>
        /// <returns></returns>
        protected virtual IEnumerable<Assembly> ApplyAssemblyFiltersTo(IEnumerable<Assembly> assemblyCandidates)
        {
            return
                assemblyCandidates.Where(IsIncludedAssembly).
                    AsEnumerable();
        }

        /// <summary>
        /// Determines whether the compound predicate is satisfied by the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the compound predicate is satisfied by the specified type; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsCompoundPredicateSatisfiedBy(Type type);

        /// <summary>
        /// Determines whether [is excluded type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is excluded type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsExcludedType(Type type)
        {
            return TypeExclusionPredicates.Any(delegate(Predicate<Type> exclude) { return exclude(type); });
        }

        /// <summary>
        /// Determines whether [is included assembly] [the specified assembly].
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// 	<c>true</c> if [is included assembly] [the specified assembly]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsIncludedAssembly(Assembly assembly)
        {
            return AssemblyInclusionPredicates.Any(delegate(Predicate<Assembly> include) { return include(assembly); });
        }

        /// <summary>
        /// Determines whether [is included type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is included type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsIncludedType(Type type)
        {
            return TypeInclusionPredicates.Any(delegate(Predicate<Type> include) { return include(type); });
        }


        /// <summary>
        /// Sets the default filters.
        /// </summary>
        protected virtual void SetDefaultFilters()
        {
            if (TypeInclusionPredicates.Count == 0)
                TypeInclusionPredicates.Add(delegate { return true; });

            if (TypeExclusionPredicates.Count == 0)
                TypeExclusionPredicates.Add(delegate { return false; });

            if (AssemblyInclusionPredicates.Count == 0)
                AssemblyInclusionPredicates.Add(delegate { return true; });
        }

        /// <summary>
        /// Adds the files found in the recursive search path for the given extension.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="assemblies">The assemblies.</param>
        public void AddFilesForExtension(string folderPath, string extension, IList<Assembly> assemblies)
        {
            IEnumerable<string> files = Directory.GetFiles(folderPath, extension, SearchOption.AllDirectories);
            foreach (string file in files)
                try
                {
                    assemblies.Add(Assembly.ReflectionOnlyLoadFrom(file));
                }
                catch (Exception ex)
                {
                    //log and swallow everything that might go wrong here...
                    if (Logger.IsDebugEnabled)
                        Logger.Debug(
                            string.Format("Failed to load assembly {0} to inspect for [Configuration] types!", file), ex);
                }
        }
    }
}