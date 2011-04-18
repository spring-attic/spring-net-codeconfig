using System;
using System.Collections.Generic;
using System.Reflection;

namespace Spring.Context.Attributes
{
    /// <summary>
    /// AssemblyTypeScanner that provides for applying a final hard-coded Required Constraint to all types found in the the scanned assemblies
    /// in addition to respecting the constraints passed to it during its configuration.
    /// </summary>
    public abstract class RequiredConstraintAssemblyTypeScanner : AssemblyTypeScanner
    {

        /// <summary>
        /// Determines whether the compound predicate is satisfied by the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the compound predicate is satisfied by the specified type; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsCompoundPredicateSatisfiedBy(Type type)
        {
            return IsRequiredConstraintSatisfiedBy(type) && IsIncludedType(type) && !IsExcludedType(type);
        }

        /// <summary>
        /// Determines whether the required constraint is satisfied by the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the required constraint is satisfied by the specified type; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsRequiredConstraintSatisfiedBy(Type type);
    }
}