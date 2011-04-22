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
using System.Reflection;
using AopAlliance.Intercept;
using Common.Logging;
using Spring.Objects.Factory.Config;
using Spring.Context.Attributes;

namespace Spring.Context.Advice
{
    /// <summary>
    /// Intercepts calls to methods within the configuration object
    /// </summary>
    /// <author>Mark Pollack</author>
    /// <author>Erich Eichinger</author>
    public class SpringObjectMethodInterceptor : IMethodInterceptor
    {
        #region Logging Definition

        private static readonly ILog LOG = LogManager.GetLogger(typeof(SpringObjectMethodInterceptor));

        #endregion

        private readonly IConfigurableListableObjectFactory _configurableListableObjectFactory;


        /// <summary>
        /// Initializes a new instance of the <see cref="SpringObjectMethodInterceptor"/> class.
        /// </summary>
        /// <param name="configurableListableObjectFactory">The configurable listable object factory.</param>
        public SpringObjectMethodInterceptor(IConfigurableListableObjectFactory configurableListableObjectFactory)
        {
            _configurableListableObjectFactory = configurableListableObjectFactory;
            
        }

        #region IMethodInterceptor Members

        /// <summary>
        /// Implement this method to perform extra treatments before and after
        /// the call to the supplied <paramref name="invocation"/>.
        /// </summary>
        /// <param name="invocation">The method invocation that is being intercepted.</param>
        /// <returns>
        /// The result of the call to the
        /// <see cref="M:AopAlliance.Intercept.IJoinpoint.Proceed"/> method of
        /// the supplied <paramref name="invocation"/>; this return value may
        /// well have been intercepted by the interceptor.
        /// </returns>
        /// <remarks>
        /// 	<p>
        /// Polite implementations would certainly like to invoke
        /// <see cref="M:AopAlliance.Intercept.IJoinpoint.Proceed"/>.
        /// </p>
        /// </remarks>
        /// <exception cref="T:System.Exception">
        /// If any of the interceptors in the chain or the target object itself
        /// throws an exception.
        /// </exception>
        public object Invoke(IMethodInvocation invocation)
        {
            MethodInfo m = invocation.Method;
            if (m.Name.StartsWith("set_") || m.Name.StartsWith("get_"))
                return invocation.Proceed();

            string name = m.Name;

            object[] attribs = m.GetCustomAttributes(typeof(DefinitionAttribute), true);
            if (attribs.Length > 0)
            {

            }

            if (IsCurrentlyInCreation(name))
            {
                if (LOG.IsDebugEnabled)
                {
                    LOG.Debug(name + " currently in creation, created one.");
                }
                return invocation.Proceed();                
            }
            if (LOG.IsDebugEnabled)
            {
                LOG.Debug(name + " not in creation, asked the application context for one");
            }

            return _configurableListableObjectFactory.GetObject(name);
        }

        private bool IsCurrentlyInCreation(string name)
        {
            return _configurableListableObjectFactory.IsCurrentlyInCreation(name);
        }

        #endregion
    }
}