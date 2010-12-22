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

namespace Spring.Context.Attributes
{
    /// <summary>
    /// Indicates that an object should be given preference when multiple candidates
    /// are qualified to autowire a single-valued dependency. If exactly one 'primary'
    /// object exists among the candidates, it will be the autowired value.
    /// 
    /// <para>Using <see cref="Primary"/> at the class level has no effect unless component-scanning
    /// is being used. If a <see cref="Primary"/>-attributed class is declared via XML,
    /// <see cref="Primary"/> attribute metadata is ignored, and
    /// <code>&lt;object primary="true|false"/&gt;></code> is respected instead.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PrimaryAttribute : Attribute
    {

    }
}
