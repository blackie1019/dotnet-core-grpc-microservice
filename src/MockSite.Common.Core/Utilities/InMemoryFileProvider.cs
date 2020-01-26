using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace MockSite.Common.Core.Utilities
{
    public class InMemoryFileProvider : IFileProvider
    {
        private readonly IFileInfo _fileInfo;

        public InMemoryFileProvider(string json)
        {
            _fileInfo = new InMemoryFile(json);
        }

        public IFileInfo GetFileInfo(string subPath)
        {
            return _fileInfo;
        }

        [SuppressMessage("ReSharper", "S1168")]
        public IDirectoryContents GetDirectoryContents(string subPath)
        {
            return null;
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        private class InMemoryFile : IFileInfo
        {
            private readonly byte[] _data;

            public InMemoryFile(string json)
            {
                _data = Encoding.UTF8.GetBytes(json);
            }

            public Stream CreateReadStream()
            {
                return new MemoryStream(_data);
            }

            public bool Exists { get; } = true;

            public long Length
            {
                get { return _data.Length; }
            }

            public string PhysicalPath { get; } = string.Empty;

            public string Name { get; } = string.Empty;

            public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;

            public bool IsDirectory { get; } = false;
        }
    }
}