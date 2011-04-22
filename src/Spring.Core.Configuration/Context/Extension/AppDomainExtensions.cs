using System;

namespace Spring.Context.Extension
{
	/// <summary>
	/// Extension methods for the <see cref="System.AppDomain"/> class.
	/// </summary>
	public static class AppDomainExtensions
	{
		/// <summary>
		/// Clones an <see cref="System.AppDomain"/> with the same settings as
		/// a provided base <see cref="System.AppDomain"/>.
		/// </summary>
		/// <param name="baseDomain">
		/// The <see cref="System.AppDomain"/> that contains the base set of
		/// settings (directory, search paths, security evidence) that should
		/// be cloned.
		/// </param>
		/// <param name="friendlyName">
		/// The friendly name of the cloned domain.
		/// </param>
		/// <returns>
		/// A new <see cref="System.AppDomain"/> that has the same settings as
		/// the provided <paramref name="baseDomain" />.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="baseDomain" /> or <paramref name="friendlyName" /> is <see langword="null" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// It is up to the caller to unload the returned <see cref="System.AppDomain"/>
		/// using <see cref="System.AppDomain.Unload"/> when use of it is no longer
		/// required.
		/// </para>
		/// <para>
		/// The information that gets copied to the cloned domain includes:
		/// </para>
		/// <list type="bullet">
		/// <item>
		/// <term><see cref="System.AppDomain.BaseDirectory"/></term>
		/// </item>
		/// <item>
		/// <term><see cref="System.AppDomain.RelativeSearchPath"/></term>
		/// </item>
		/// <item>
		/// <term><see cref="System.AppDomain.Evidence"/></term>
		/// </item>
		/// </list>
		/// </remarks>
		public static AppDomain CloneDomain(this AppDomain baseDomain, string friendlyName)
		{
			if (baseDomain == null)
			{
				throw new ArgumentNullException("baseDomain");
			}
			if (friendlyName == null)
			{
				throw new ArgumentNullException("friendlyName");
			}
			var appDomainSetup = new AppDomainSetup();
			appDomainSetup.ApplicationBase = baseDomain.BaseDirectory;
			appDomainSetup.PrivateBinPath = baseDomain.RelativeSearchPath;
			var appDomain = AppDomain.CreateDomain(friendlyName, baseDomain.Evidence, appDomainSetup);
			return appDomain;
		}

		/// <summary>
		/// Creates an object of a specified type
		/// inside the provided <see cref="System.AppDomain"/>.
		/// </summary>
		/// <typeparam name="T">
		/// The type of object to create in the <paramref name="domain" />.
		/// </typeparam>
		/// <param name="domain">
		/// The domain in which the remote object should be created.
		/// </param>
		/// <returns>
		/// An object of the specified type, unwrapped inside the target <paramref name="domain" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default parameterless constructor is used to create the object.
		/// </para>
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="domain" /> is <see langword="null" />.
		/// </exception>
		/// <seealso cref="System.AppDomain.CreateInstanceAndUnwrap(string, string)"/>
		public static T CreateRemoteObject<T>(this AppDomain domain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			var assemblyName = typeof(T).Assembly.GetName().Name;
			var typeName = typeof(T).FullName;
			var remoteObject = (T)domain.CreateInstanceAndUnwrap(assemblyName, typeName);
			return remoteObject;
		}
	}
}
