using System;

namespace Spring.Context.Attributes
{
    public abstract class RequiredConstraintAssemblyTypeScanner : AssemblyTypeScanner
    {
        public RequiredConstraintAssemblyTypeScanner(string folderScanPath) : base(folderScanPath)
        {
        }

        public RequiredConstraintAssemblyTypeScanner()
        {
        }

        protected override bool IsCompoundPredicateSatisfiedBy(Type type)
        {
            return IsIncludedType(type) && !IsExcludedType(type) && IsRequiredConstraintSatisfiedBy(type);
        }

        protected abstract bool IsRequiredConstraintSatisfiedBy(Type type);
    }
}