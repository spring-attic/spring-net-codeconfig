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
using System.Security.Permissions;
using System.Security.Policy;
using Common.Logging;
using Spring.Core;
using Spring.Util;

namespace Spring.Context.Attributes
{
    public abstract class AssemblyTypeScanner : IAssemblyTypeScanner
    {
        protected static ILog Logger = LogManager.GetLogger(typeof(AssemblyTypeScanner));
        protected readonly List<Predicate<Assembly>> AssemblyInclusionPredicates = new List<Predicate<Assembly>>();

        protected readonly List<Predicate<Type>> TypeExclusionPredicates = new List<Predicate<Type>>();

        protected readonly List<Predicate<Type>> TypeInclusionPredicates = new List<Predicate<Type>>();

        protected readonly List<IEnumerable<Type>> TypeSources = new List<IEnumerable<Type>>();
        protected string FolderScanPath;

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

        public IAssemblyTypeScanner AssemblyHavingType<T>()
        {
            TypeSources.Add(new AssemblyTypeSource((typeof(T).Assembly)));
            return this;
        }

        public IAssemblyTypeScanner ExcludeType<T>()
        {
            //TypeExclusionPredicates.Add(delegate(Type t) { return t == typeof(T); });
            TypeExclusionPredicates.Add(delegate(Type t) { return t.FullName == typeof(T).FullName; });
            return this;
        }

        public IAssemblyTypeScanner IncludeType<T>()
        {
            //TypeInclusionPredicates.Add(delegate(Type t) { return t == typeof(T); });
            TypeInclusionPredicates.Add(delegate(Type t) { return t.FullName == typeof(T).FullName; });
            return this;
        }

        public IAssemblyTypeScanner IncludeTypes(IEnumerable<Type> typeSource)
        {
            AssertUtils.ArgumentNotNull(typeSource, "typeSource");
            TypeSources.Add(typeSource);
            TypeInclusionPredicates.Add(delegate(Type t) { return typeSource.Any(delegate(Type t1) { return t1.FullName == t.FullName; }); });
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
                    if (IsCompoundPredicateSatisfiedBy(type))
                    {
                        types.Add(type);
                    }
                }
            }

            return EnsureAllTypesLoadedInAppDomain(types);
        }

        public IAssemblyTypeScanner WithAssemblyFilter(Predicate<Assembly> assemblyPredicate)
        {
            AssemblyInclusionPredicates.Add(assemblyPredicate);
            return this;
        }

        public IAssemblyTypeScanner WithExcludeFilter(Predicate<Type> predicate)
        {
            TypeExclusionPredicates.Add(predicate);
            return this;
        }

        public IAssemblyTypeScanner WithIncludeFilter(Predicate<Type> predicate)
        {
            TypeInclusionPredicates.Add(predicate);
            return this;
        }

        #endregion

        protected abstract bool IsCompoundPredicateSatisfiedBy(Type type);

        protected virtual bool IsExcludedType(Type type)
        {
            return TypeExclusionPredicates.Any(delegate(Predicate<Type> exclude) { return exclude(type); });
        }

        protected virtual bool IsIncludedAssembly(Assembly assembly)
        {
            return AssemblyInclusionPredicates.Any(delegate(Predicate<Assembly> include) { return include(assembly); });
        }

        protected virtual bool IsIncludedType(Type type)
        {
            return TypeInclusionPredicates.Any(delegate(Predicate<Type> include) { return include(type); });
        }

        protected virtual void SetDefaultFilters()
        {

            if (TypeInclusionPredicates.Count == 0)
                TypeInclusionPredicates.Add(delegate { return true; });

            if (TypeExclusionPredicates.Count == 0)
                TypeExclusionPredicates.Add(delegate { return false; });

            if (AssemblyInclusionPredicates.Count == 0)
                AssemblyInclusionPredicates.Add(delegate { return true; });
        }


        protected virtual IEnumerable<Assembly> ApplyAssemblyFiltersTo(IEnumerable<Assembly> assemblyCandidates)
        {
            return assemblyCandidates.Where(delegate(Assembly assembly) { return IsIncludedAssembly(assembly); }).AsEnumerable();
        }

        private IEnumerable<Assembly> GetAllAssembliesInPath(string folderPath)
        {
            var assemblies = new List<Assembly>();
            AddFilesForExtension(folderPath, "*.dll", assemblies);
            AddFilesForExtension(folderPath, "*.exe", assemblies);

            if (Logger.IsDebugEnabled)
            {
                Assembly[] assemblyArray = assemblies.ToArray();
                Logger.Debug("Assemblies to be scanned: " + StringUtils.ArrayToCommaDelimitedString(assemblyArray));
            }
            return assemblies;
        }



        private Assembly MyReflectionOnlyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            AssemblyName name = new AssemblyName(args.Name);

            String asmToCheck = Path.GetDirectoryName(FolderScanPath) + "\\" + name.Name + ".dll";

            if (File.Exists(asmToCheck))
            {
                return Assembly.ReflectionOnlyLoadFrom(asmToCheck);
            }
            return ReflectionOnlyLoadWithPartialName(name.Name);
        }

        #region ReflectionLoad
        private Assembly ReflectionOnlyLoadWithPartialName(string partialName)
        {
            return ReflectionOnlyLoadWithPartialName(partialName, null);
        }

        private Assembly ReflectionOnlyLoadWithPartialName(string partialName, Evidence securityEvidence)
        {
            if (securityEvidence != null)
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();

            AssemblyName fileName = new AssemblyName(partialName);

            var assembly = nLoad(fileName, null, securityEvidence, null, null, false, true);

            if (assembly != null)
                return assembly;

            var assemblyRef = EnumerateCache(fileName);

            if (assemblyRef != null)
                return InternalLoad(assemblyRef, securityEvidence, null, true);

            return assembly;
        }

        private Assembly nLoad(params object[] args)
        {
            return (Assembly)typeof(Assembly)
                .GetMethod("nLoad", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, args);
        }

        private AssemblyName EnumerateCache(params object[] args)
        {
            return (AssemblyName)typeof(Assembly)
                .GetMethod("EnumerateCache", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, args);
        }

        private Assembly InternalLoad(params object[] args)
        {
            // Easiest to query because the StackCrawlMark type is internal
            /*
            return (Assembly)
                typeof(Assembly).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .First(m => m.Name == "InternalLoad" &&
                            m.GetParameters()[0].ParameterType == typeof (AssemblyName))
                .Invoke(null, args);
             */
            IEnumerable<MethodInfo> methods =
                typeof(Assembly).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(
                    delegate(MethodInfo m)
                    {
                        return m.Name == "InternalLoad" &&
                               m.GetParameters()[0].ParameterType == typeof(AssemblyName);
                    });

            foreach (MethodInfo methodInfo in methods)
            {
                return (Assembly)methodInfo.Invoke(null, args);
            }

            return null;
        }

        #endregion





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
            return string.IsNullOrEmpty(AppDomain.CurrentDomain.DynamicDirectory)
                       ? AppDomain.CurrentDomain.BaseDirectory
                       : AppDomain.CurrentDomain.DynamicDirectory;
        }

        private IEnumerable<Type> EnsureAllTypesLoadedInAppDomain(IEnumerable<Type> potentialReflectionOnlyTypes)
        {
            List<Type> actualAppDomainTypes = new List<Type>();

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
                        throw new CannotLoadObjectTypeException(string.Format("Unable to load type {0} from assembly {1}", type.FullName, type.Assembly.Location));
                    }
                    
                }
                
                actualAppDomainTypes.Add(Type.GetType(type.FullName + "," + type.Assembly.FullName));
            }

            return actualAppDomainTypes;
        }
    }
}