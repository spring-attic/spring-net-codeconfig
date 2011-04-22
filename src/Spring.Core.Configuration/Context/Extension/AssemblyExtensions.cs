using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace Spring.Context.Extension
{
	/// <summary>
	/// Extension methods for the <see cref="System.Reflection.Assembly"/> class.
	/// </summary>
	public static class AssemblyExtensions
	{
		/// <summary>
		/// Scans a list of assemblies in a remote sandbox <see cref="System.AppDomain"/>.
		/// </summary>
		/// <typeparam name="TInput">The type of <paramref name="scanParameters" /> that the <paramref name="scanner" /> accepts.</typeparam>
		/// <typeparam name="TOutput">The type of value returned by the <paramref name="scanner" /> delegate.</typeparam>
		/// <param name="files">
		/// An <see cref="System.Collections.Generic.IEnumerable{T}"/> with a set
		/// of strings, each of which contains the path to an <see cref="System.Reflection.Assembly"/>
		/// to load and scan.
		/// </param>
		/// <param name="scanParameters">
		/// The parameters to pass to the <paramref name="scanner" /> delegate.
		/// These parameters must be serializable since they will be passed to
		/// the scanner in the remote <see cref="System.AppDomain"/>. If your
		/// <paramref name="scanner" /> does not need parameters, pass
		/// <c>(object)null</c>.
		/// </param>
		/// <param name="scanner">
		/// The delegate used to scan the <see cref="System.Reflection.Assembly"/>
		/// in the remote <see cref="System.AppDomain"/>.
		/// </param>
		/// <returns>
		/// An <see cref="System.Collections.Generic.IEnumerable{U}"/> containing
		/// the set of results from the scan.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If you get a <see cref="System.Runtime.Serialization.SerializationException"/>
		/// when scanning, it indicates that either the <paramref name="scanParameters" />
		/// are not serializable (and, thus, cannot be marshaled into the remote
		/// <see cref="System.AppDomain"/>), you have a closure problem in
		/// your <paramref name="scanner" />, or the object you're trying to return
		/// from the <paramref name="scanner" /> is not serializable (and, thus,
		/// cannot be marshaled back into the primary <see cref="System.AppDomain"/>).
		/// </para>
		/// <para>
		/// Make sure that your <paramref name="scanParameters" /> are simple
		/// serializable objects like <see cref="System.String"/>, <see cref="System.Boolean"/>,
		/// etc. For example, rather than passing in a <see cref="System.Type"/>
		/// as an input, pass in a <see cref="System.String"/> containing the
		/// type's name and then use <see cref="System.Type.GetType(string)"/>
		/// inside your <paramref name="scanner" /> to load the type in the
		/// other <see cref="System.AppDomain"/>.
		/// </para>
		/// <para>
		/// Your <paramref name="scanner" /> delegate should be a simple delegate
		/// that doesn't have any nested delegates or closures. The reason is that
		/// if the <paramref name="scanner" /> has a closure or nested delegate,
		/// the compiler will create a small temporary class containing the logic
		/// for the closure/delegate and that class is not serializable, so you
		/// will get an exception.
		/// </para>
		/// <para>
		/// Finally, just as you should make sure your <paramref name="scanParameters" />
		/// are simple serializable objects, you should make sure the return value
		/// from the <paramref name="scanner" /> is also a simple serializable object
		/// like an array of <see cref="System.String"/>.
		/// </para>
		/// </remarks>
		/// <example>
		/// <para>
		/// The following example shows scanning a directory full of assemblies
		/// to get a list of types implementing a specific interface.
		/// </para>
		/// <code lang="C#">
		/// string targetInterfaceName = "ISomeInterface";
		/// string directoryToScan = @"C:\path\to\assemblies";
		/// var implementors =
		///   Directory
		///   .GetFiles(directoryToScan)
		///   .ScanAssembliesInSandbox(
		///     targetInterfaceName,
		///     (asm, sp) =&gt;
		///     {
		///       var foundTypes = new List&lt;string&gt;();
		///       foreach (var t in asm.GetTypes())
		///       {
		///         if (t.GetInterface(sp) != null)
		///         {
		///           foundTypes.Add(t.AssemblyQualifiedName);
		///         }
		///       }
		///       return foundTypes.ToArray();
		///     });
		/// </code>
		/// </example>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="files" /> or <paramref name="scanner" /> is <see langword="null" />.
		/// </exception>
		/// <seealso cref="RemoteAssemblyScanningExtensions.AssemblyExtensions.ScanAssemblyInSandbox"/>
		public static IEnumerable<TOutput> ScanAssembliesInSandbox<TInput, TOutput>(this IEnumerable<string> files, TInput scanParameters, Func<Assembly, TInput, TOutput> scanner)
		{
			if (files == null)
			{
				throw new ArgumentNullException("files");
			}
			if (scanner == null)
			{
				throw new ArgumentNullException("scanner");
			}
			foreach (var path in files)
			{
				var info = new FileInfo(path);
				yield return info.ScanAssemblyInSandbox(scanParameters, scanner);
			}
		}

		/// <summary>
		/// Scans a list of assemblies in a remote sandbox <see cref="System.AppDomain"/>.
		/// </summary>
		/// <typeparam name="TInput">The type of <paramref name="scanParameters" /> that the <paramref name="scanner" /> accepts.</typeparam>
		/// <typeparam name="TOutput">The type of value returned by the <paramref name="scanner" /> delegate.</typeparam>
		/// <param name="files">
		/// An <see cref="System.Collections.Generic.IEnumerable{T}"/> with a set
		/// of <see cref="System.IO.FileInfo"/>, each of which contains the path
		/// to an <see cref="System.Reflection.Assembly"/> to load and scan.
		/// </param>
		/// <param name="scanParameters">
		/// The parameters to pass to the <paramref name="scanner" /> delegate.
		/// These parameters must be serializable since they will be passed to
		/// the scanner in the remote <see cref="System.AppDomain"/>. If your
		/// <paramref name="scanner" /> does not need parameters, pass
		/// <c>(object)null</c>.
		/// </param>
		/// <param name="scanner">
		/// The delegate used to scan the <see cref="System.Reflection.Assembly"/>
		/// in the remote <see cref="System.AppDomain"/>.
		/// </param>
		/// <returns>
		/// An <see cref="System.Collections.Generic.IEnumerable{U}"/> containing
		/// the set of results from the scan.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If you get a <see cref="System.Runtime.Serialization.SerializationException"/>
		/// when scanning, it indicates that either the <paramref name="scanParameters" />
		/// are not serializable (and, thus, cannot be marshaled into the remote
		/// <see cref="System.AppDomain"/>), you have a closure problem in
		/// your <paramref name="scanner" />, or the object you're trying to return
		/// from the <paramref name="scanner" /> is not serializable (and, thus,
		/// cannot be marshaled back into the primary <see cref="System.AppDomain"/>).
		/// </para>
		/// <para>
		/// Make sure that your <paramref name="scanParameters" /> are simple
		/// serializable objects like <see cref="System.String"/>, <see cref="System.Boolean"/>,
		/// etc. For example, rather than passing in a <see cref="System.Type"/>
		/// as an input, pass in a <see cref="System.String"/> containing the
		/// type's name and then use <see cref="System.Type.GetType(string)"/>
		/// inside your <paramref name="scanner" /> to load the type in the
		/// other <see cref="System.AppDomain"/>.
		/// </para>
		/// <para>
		/// Your <paramref name="scanner" /> delegate should be a simple delegate
		/// that doesn't have any nested delegates or closures. The reason is that
		/// if the <paramref name="scanner" /> has a closure or nested delegate,
		/// the compiler will create a small temporary class containing the logic
		/// for the closure/delegate and that class is not serializable, so you
		/// will get an exception.
		/// </para>
		/// <para>
		/// Finally, just as you should make sure your <paramref name="scanParameters" />
		/// are simple serializable objects, you should make sure the return value
		/// from the <paramref name="scanner" /> is also a simple serializable object
		/// like an array of <see cref="System.String"/>.
		/// </para>
		/// </remarks>
		/// <example>
		/// <para>
		/// The following example shows scanning a directory full of assemblies
		/// to get a list of types implementing a specific interface.
		/// </para>
		/// <code lang="C#">
		/// string targetInterfaceName = "ISomeInterface";
		/// DirectoryInfo directoryToScan = new DirectoryInfo(@"C:\path\to\assemblies");
		/// var implementors =
		///   directoryToScan
		///   .GetFiles()
		///   .ScanAssembliesInSandbox(
		///     targetInterfaceName,
		///     (asm, sp) =&gt;
		///     {
		///       var foundTypes = new List&lt;string&gt;();
		///       foreach (var t in asm.GetTypes())
		///       {
		///         if (t.GetInterface(sp) != null)
		///         {
		///           foundTypes.Add(t.AssemblyQualifiedName);
		///         }
		///       }
		///       return foundTypes.ToArray();
		///     });
		/// </code>
		/// </example>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="files" /> or <paramref name="scanner" /> is <see langword="null" />.
		/// </exception>
		/// <seealso cref="RemoteAssemblyScanningExtensions.AssemblyExtensions.ScanAssemblyInSandbox"/>
		public static IEnumerable<TOutput> ScanAssembliesInSandbox<TInput, TOutput>(this IEnumerable<FileInfo> files, TInput scanParameters, Func<Assembly, TInput, TOutput> scanner)
		{
			if (files == null)
			{
				throw new ArgumentNullException("files");
			}
			if (scanner == null)
			{
				throw new ArgumentNullException("scanner");
			}
			foreach (var info in files)
			{
				yield return info.ScanAssemblyInSandbox(scanParameters, scanner);
			}
		}

		/// <summary>
		/// Scans an individual assembly in a remote sandbox <see cref="System.AppDomain"/>.
		/// </summary>
		/// <typeparam name="TInput">The type of <paramref name="scanParameters" /> that the <paramref name="scanner" /> accepts.</typeparam>
		/// <typeparam name="TOutput">The type of value returned by the <paramref name="scanner" /> delegate.</typeparam>
		/// <param name="file">
		/// A <see cref="System.IO.FileInfo"/> which contains the path
		/// to an <see cref="System.Reflection.Assembly"/> to load and scan.
		/// </param>
		/// <param name="scanParameters">
		/// The parameters to pass to the <paramref name="scanner" /> delegate.
		/// These parameters must be serializable since they will be passed to
		/// the scanner in the remote <see cref="System.AppDomain"/>. If your
		/// <paramref name="scanner" /> does not need parameters, pass
		/// <c>(object)null</c>.
		/// </param>
		/// <param name="scanner">
		/// The delegate used to scan the <see cref="System.Reflection.Assembly"/>
		/// in the remote <see cref="System.AppDomain"/>.
		/// </param>
		/// <returns>
		/// An instance of <typeparamref name="TOutput"/> containing the results from the scan.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If you get a <see cref="System.Runtime.Serialization.SerializationException"/>
		/// when scanning, it indicates that either the <paramref name="scanParameters" />
		/// are not serializable (and, thus, cannot be marshaled into the remote
		/// <see cref="System.AppDomain"/>), you have a closure problem in
		/// your <paramref name="scanner" />, or the object you're trying to return
		/// from the <paramref name="scanner" /> is not serializable (and, thus,
		/// cannot be marshaled back into the primary <see cref="System.AppDomain"/>).
		/// </para>
		/// <para>
		/// Make sure that your <paramref name="scanParameters" /> are simple
		/// serializable objects like <see cref="System.String"/>, <see cref="System.Boolean"/>,
		/// etc. For example, rather than passing in a <see cref="System.Type"/>
		/// as an input, pass in a <see cref="System.String"/> containing the
		/// type's name and then use <see cref="System.Type.GetType(string)"/>
		/// inside your <paramref name="scanner" /> to load the type in the
		/// other <see cref="System.AppDomain"/>.
		/// </para>
		/// <para>
		/// Your <paramref name="scanner" /> delegate should be a simple delegate
		/// that doesn't have any nested delegates or closures. The reason is that
		/// if the <paramref name="scanner" /> has a closure or nested delegate,
		/// the compiler will create a small temporary class containing the logic
		/// for the closure/delegate and that class is not serializable, so you
		/// will get an exception.
		/// </para>
		/// <para>
		/// Finally, just as you should make sure your <paramref name="scanParameters" />
		/// are simple serializable objects, you should make sure the return value
		/// from the <paramref name="scanner" /> is also a simple serializable object
		/// like an array of <see cref="System.String"/>.
		/// </para>
		/// </remarks>
		/// <example>
		/// <para>
		/// The following example shows scanning a directory full of assemblies
		/// to get a list of types implementing a specific interface.
		/// </para>
		/// <code lang="C#">
		/// string targetInterfaceName = "ISomeInterface";
		/// FileInfo fileToScan = new FileInfo(@"C:\path\to\assemblies\myasm.dll");
		/// var implementors =
		///   fileToScan
		///   .ScanAssemblyInSandbox(
		///     targetInterfaceName,
		///     (asm, sp) =&gt;
		///     {
		///       var foundTypes = new List&lt;string&gt;();
		///       foreach (var t in asm.GetTypes())
		///       {
		///         if (t.GetInterface(sp) != null)
		///         {
		///           foundTypes.Add(t.AssemblyQualifiedName);
		///         }
		///       }
		///       return foundTypes.ToArray();
		///     });
		/// </code>
		/// </example>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="file" /> or <paramref name="scanner" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="System.IO.FileNotFoundException">
		/// Thrown if <paramref name="file" /> does not exist.
		/// </exception>
		/// <seealso cref="AppDomainExtensions.CloneDomain"/>
		public static TOutput ScanAssemblyInSandbox<TInput, TOutput>(this FileInfo file, TInput scanParameters, Func<Assembly, TInput, TOutput> scanner)
		{
			/* The body of this method looks a lot like the AppDomainExtensions.CloneDomainAndRemoteExecute
			 * method but we needed to do a little copy/paste due to serialization
			 * problems.
			 * 
			 * The issue is that if you try to do this...
			 * 
			 * AppDomain.CurrentDomain.CloneDomainAndRemoteExecute(
			 *   "AssemblyScanDomain",
			 *   new object[]{ fileInfo, scanner },
			 *   p =>
			 *   {
			 *     var fi = (FileInfo)p[0];
			 *     var scanner = (Func<Assembly, T>)p[1];
			 *     var asm = Assembly.LoadFrom(fi.FullName);
			 *     return scanner(asm);
			 *   });
			 * 
			 * ...then the compiler actually makes a tiny generated class that contains
			 * the body of the lambda and tries to marshal that across to the other
			 * AppDomain... except the generated class is not marked Serializable
			 * and isn't a MarshalByRefObject, so it fails.
			 * 
			 * In order to get the strongly-typed syntax and assembly loading
			 * convenience in place, we have to introduce a custom MarshalByRefObject
			 * that has the loading code in place - that's what the
			 * RemoteAssemblyScanner type is for.
			 * 
			 * You could potentially get around this by creating custom expression
			 * trees and compiling them down to a new lambda with a closure
			 * on the incoming scanner lambda and file FileInfo object... but
			 * that's a reasonably complex task that can be addressed in a far
			 * more readable and maintainable fashion this way.
			 */

			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (scanner == null)
			{
				throw new ArgumentNullException("scanner");
			}

			// Edge case here - if file.Exists was checked earlier, the value
			// is cached. Since the last Exists check, the file may have been
			// deleted, but the FileInfo object still thinks it exists. We could
			// call Refresh on it, but that's not a free operation, so we'll assume
			// the FileInfo data is valid.
			if (!file.Exists)
			{
				throw new FileNotFoundException("Unable to scan assembly that does not exist.", file.FullName);
			}

			var appDomain = AppDomain.CurrentDomain.CloneDomain("ScanAssemblyInSandbox-Domain");
			try
			{
				var remoteExecutor = appDomain.CreateRemoteObject<RemoteAssemblyScanner>();
				return remoteExecutor.Execute(file, scanParameters, scanner);
			}
			finally
			{
				AppDomain.Unload(appDomain);
			}
		}

		/// <summary>
		/// Internal class used only for remote scanning of an assembly.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This type is a workaround for serialization issues that occur when
		/// you try to inline a lambda that contains a call to another lambda.
		/// It's only needed for adding the strongly-typed syntax around assembly
		/// scanning, so it's marked private - developers outside
		/// shouldn't be using it or directly relying on it.
		/// </para>
		/// </remarks>
		[ExcludeFromCodeCoverage]
		[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "This class is instantiated via AppDomain.CreateInstanceAndUnwrap, which FxCop does not properly detect.")]
        private class RemoteAssemblyScanner : MarshalByRefObject
		{
			/// <summary>
			/// Executes a remote assembly scan.
			/// </summary>
			/// <typeparam name="TInput">The type of input parameter for the scan.</typeparam>
			/// <typeparam name="TOutput">The type of return value coming back from the scan.</typeparam>
			/// <param name="assemblyFile">The assembly (dll) file to scan.</param>
			/// <param name="scanParameters">Incoming parameters that will be marshaled into the scan action.</param>
			/// <param name="action">
			/// The actual action to execute in a remote sandbox to scan the assembly.
			/// </param>
			/// <returns>
			/// The value returned from the scan action.
			/// </returns>
			[SuppressMessage("Microsoft.Reliability", "CA2001", Justification = "The point of this method is to allow a user to load an assembly from a file for scanning. Use of Assembly.LoadFrom is intentional and unavoidable.")]
			[SuppressMessage("Microsoft.Performance", "CA1822", Justification = "This method needs to be an instance method to ensure proper marshaling and execution across AppDomain boundaries.")]
			public TOutput Execute<TInput, TOutput>(FileInfo assemblyFile, TInput scanParameters, Func<Assembly, TInput, TOutput> action)
			{
				var asm = Assembly.LoadFrom(assemblyFile.FullName);
				return action(asm, scanParameters);
			}

			/// <summary>
			/// Obtains a lifetime service object to control the lifetime policy for this instance.
			/// </summary>
			/// <returns>
			/// Always returns <see langword="null" />.
			/// </returns>
			/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
			public override object InitializeLifetimeService()
			{
				return null;
			}
		}
	}
}
