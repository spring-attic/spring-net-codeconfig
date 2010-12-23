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
using System.Reflection;

namespace Spring.Context.Attributes
{

    /// <summary>
    /// Scanner that can filter types from assemblies based on constraints.
    /// </summary>
    public interface IAssemblyTypeScanner
    {
        IAssemblyTypeScanner AssemblyHavingType<T>();
        IAssemblyTypeScanner WithAssemblyFilter(Predicate<Assembly> assemblyPredicate);

        IAssemblyTypeScanner WithIncludeFilter(Predicate<Type> predicate);
        IAssemblyTypeScanner WithExcludeFilter(Predicate<Type> predicate);

        IAssemblyTypeScanner IncludeTypes(IEnumerable<Type> typeSource);
        IAssemblyTypeScanner IncludeType<T>();
        IAssemblyTypeScanner ExcludeType<T>();

        IEnumerable<Type> Scan();
    }
}