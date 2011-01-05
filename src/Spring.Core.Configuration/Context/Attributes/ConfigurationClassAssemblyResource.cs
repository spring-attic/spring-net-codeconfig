using System;
using System.IO;
using System.Reflection;
using Spring.Core.IO;

namespace Spring.Context.Attributes
{
    public class ConfigurationClassAssemblyResource : IResource
    {
        private readonly string _containingAssemblyFileName;
        private readonly Type _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ConfigurationClassAssemblyResource(Type type)
        {
            _type = type;
            _containingAssemblyFileName = Assembly.GetAssembly(_type.GetType()).Location;
        }

        #region IResource Members

        public IResource CreateRelative(string relativePath)
        {
            throw new InvalidOperationException();
        }

        public bool IsOpen
        {
            get { return false; }
        }

        public Uri Uri
        {
            get { return new Uri(_containingAssemblyFileName); }
        }

        public FileInfo File
        {
            get { return new FileInfo(_containingAssemblyFileName); }
        }

        public string Description
        {
            get { return _type.FullName; }
        }

        public bool Exists
        {
            get { return System.IO.File.Exists(_containingAssemblyFileName); }
        }

        public Stream InputStream
        {
            get { throw new InvalidOperationException(); }
        }

        #endregion
    }
}