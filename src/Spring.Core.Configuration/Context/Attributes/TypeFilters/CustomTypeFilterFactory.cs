#region License

/*
 * Copyright © 2010-2011 the original author or authors.
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
using Common.Logging;
using Spring.Context.Attributes.TypeFilters;
using Spring.Core.TypeResolution;
using Spring.Util;

namespace Spring.Context.Attributes.TypeFilters
{
    /// <summary>
    /// Creates a new Type Filter based on given type name
    /// </summary>
    public static class CustomTypeFilterFactory
    {
        private static readonly ILog Logger = LogManager.GetLogger<AbstractLoadTypeFilter>();

        /// <summary>
        /// Creates a new instance of given Type Filter name
        /// </summary>
        /// <param name="expression">Custom type filter to create</param>
        /// <returns>An instance of ITypeFilter or NULL if no instance can be created</returns>
        public static ITypeFilter GetCustomTypeFilter(string expression)
        {
            var customTypeFilterType = LoadTypeFilter(expression);
            if (customTypeFilterType == null)
                return null;

            try
            {
                var typeFilter = ObjectUtils.InstantiateType(customTypeFilterType) as ITypeFilter;
                return typeFilter;
            }
            catch
            {
                Logger.Error(string.Format("Can't instatiate {0}. Type needs to have a non arg constructor.", expression));
            }

            return null;
        }


        private static Type LoadTypeFilter(string typeToLoad)
        {
            try
            {
                return TypeResolutionUtils.ResolveType(typeToLoad);
            }
            catch (Exception)
            {
                Logger.Error("Can't load type defined in exoression:" + typeToLoad);
            }

            return null;
        }
    }
}
