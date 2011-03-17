using System;
using System.Reflection;
using Spring.Context.Attributes;
using Spring.Objects.Factory.Support;

namespace Spring.Context.Support
{
    /// <summary>
    /// Extensions to enable scanning on any AbstractApplicationContext-derived type.
    /// </summary>
    public static class AbstractApplicationContextExtensions
    {
        /// <summary>
        /// Scans for types using the provided scanner.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scanner">The scanner.</param>
        public static void Scan(this AbstractApplicationContext context, AssemblyObjectDefinitionScanner scanner)
        {
            scanner.ScanAndRegisterTypes((IObjectDefinitionRegistry)context.ObjectFactory);
        }

        /// <summary>
        /// Scans for types that satisfy specified predicates located in the specified scan path.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblyScanPath">The assembly scan path.</param>
        /// <param name="assemblyPredicate">The assembly predicate.</param>
        /// <param name="typePredicate">The type predicate.</param>
        public static void Scan(this AbstractApplicationContext context, string assemblyScanPath, Predicate<Assembly> assemblyPredicate,
                                Predicate<Type> typePredicate)
        {
            //create a scanner instance using the scan path
            var scanner = new AssemblyObjectDefinitionScanner(assemblyScanPath);

            //configure the scanner per the provided constraints
            scanner.WithAssemblyFilter(assemblyPredicate).WithIncludeFilter(typePredicate);

            //pass the scanner to primary Scan method to actually do the work
            Scan(context, scanner);
        }

        /// <summary>
        /// Scans for types that satisfy specified predicates.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblyPredicate">The assembly predicate.</param>
        /// <param name="typePredicate">The type predicate.</param>
        public static void Scan(this AbstractApplicationContext context, Predicate<Assembly> assemblyPredicate, Predicate<Type> typePredicate)
        {
            Scan(context, null, assemblyPredicate, typePredicate);
        }

        /// <summary>
        /// Scans for types using the default scanner.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void ScanAllAssemblies(this AbstractApplicationContext context)
        {
            Scan(context, new AssemblyObjectDefinitionScanner());
        }


        /// <summary>
        /// Scans the with assembly filter.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblyPredicate">The assembly predicate.</param>
        public static void ScanWithAssemblyFilter(this AbstractApplicationContext context, Predicate<Assembly> assemblyPredicate)
        {
            Scan(context, null, assemblyPredicate, delegate { return true; });
        }

        /// <summary>
        /// Scans the with type filter.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="typePredicate">The type predicate.</param>
        public static void ScanWithTypeFilter(this AbstractApplicationContext context, Predicate<Type> typePredicate)
        {
            Scan(context, null, delegate { return true; }, typePredicate);
        }

        
    }
}