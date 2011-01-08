using System;
using System.Collections.Generic;
using System.Reflection;

namespace Spring.Context.Attributes
{
    public abstract class RequiredConstraintAssemblyTypeScanner : AssemblyTypeScanner
    {
        protected RequiredConstraintAssemblyTypeScanner(string folderScanPath) : base(folderScanPath)
        {
        }

        protected RequiredConstraintAssemblyTypeScanner()
        {
        }

        protected override bool IsCompoundPredicateSatisfiedBy(Type type)
        {
            return IsIncludedType(type) && !IsExcludedType(type) && IsRequiredConstraintSatisfiedBy(type);
        }

        protected abstract bool IsRequiredConstraintSatisfiedBy(Type type);
    }
}