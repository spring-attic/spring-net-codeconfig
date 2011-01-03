using System;

namespace Spring.Context.Attributes
{
    public class RequiredConstraintAssemblyTypeScanner : AssemblyTypeScanner
    {
        public RequiredConstraintAssemblyTypeScanner(string folderScanPath) : base(folderScanPath)
        {
        }

        public RequiredConstraintAssemblyTypeScanner()
        {
        }

        protected override bool PredicateIsSatisfiedBy(Type type)
        {
            return IsIncludedType(type) && !IsExcludedType(type) && RequiredConstraintIsSatisfiedBy(type);
        }

        protected virtual bool RequiredConstraintIsSatisfiedBy(Type type)
        {
            return true;
        }

    }
}