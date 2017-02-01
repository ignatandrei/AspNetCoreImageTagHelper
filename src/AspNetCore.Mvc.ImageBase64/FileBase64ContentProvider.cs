using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
namespace AspNetCore.Mvc.ImageBase64
{
    /// <summary>
    /// Provides base64 content a specified file.
    ///</summary>
    public class FileBase64ContentProvider
    {
        private const string VersionKey = "v";
        private static readonly char[] QueryStringAndFragmentTokens = new[] { '?', '#' };
        private readonly IFileProvider _fileProvider;
        private readonly IMemoryCache _cache;
        private readonly PathString _requestPathBase;

        /// <summary>
        /// Creates a new instance of <see cref="FileBase64ContentProvider"/>.
        /// </summary>
        /// <param name="fileProvider">The file provider to get and watch files.</param>
        /// <param name="cache"><see cref="IMemoryCache"/> where versioned urls of files are cached.</param>
        /// <param name="requestPathBase">The base path for the current HTTP request.</param>
        public FileBase64ContentProvider(
            IFileProvider fileProvider,
            IMemoryCache cache,
            PathString requestPathBase)
        {
            if (fileProvider == null)
            {
                throw new ArgumentNullException(nameof(fileProvider));
            }

            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            _fileProvider = fileProvider;
            _cache = cache;
            _requestPathBase = requestPathBase;
        }


        /// <summary>
        /// Adds version query parameter to the specified file path.
        /// </summary>
        /// <param name="path">The path of the file to which version should be added.</param>
        /// <returns>Path containing the version query string.</returns>
        /// <remarks>
        /// The version query string is appended with the key "v".
        /// </remarks>
        public string RetrieveBase64Data(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var resolvedPath = path;

            var queryStringOrFragmentStartIndex = path.IndexOfAny(QueryStringAndFragmentTokens);
            if (queryStringOrFragmentStartIndex != -1)
            {
                resolvedPath = path.Substring(0, queryStringOrFragmentStartIndex);
            }

            Uri uri;
            if (Uri.TryCreate(resolvedPath, UriKind.Absolute, out uri) && !uri.IsFile)
            {
                // Don't append version if the path is absolute.
                return path;
            }

            string value;
            if (!_cache.TryGetValue(path, out value))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions();
                cacheEntryOptions.AddExpirationToken(_fileProvider.Watch(resolvedPath));
                var fileInfo = _fileProvider.GetFileInfo(resolvedPath);

                if (!fileInfo.Exists &&
                    _requestPathBase.HasValue &&
                    resolvedPath.StartsWith(_requestPathBase.Value, StringComparison.OrdinalIgnoreCase))
                {
                    var requestPathBaseRelativePath = resolvedPath.Substring(_requestPathBase.Value.Length);
                    cacheEntryOptions.AddExpirationToken(_fileProvider.Watch(requestPathBaseRelativePath));
                    fileInfo = _fileProvider.GetFileInfo(requestPathBaseRelativePath);
                }

                if (fileInfo.Exists)
                {
                    value = string.Format("{0}{1}", RetrieveBase64Prefix(fileInfo), GetContentsForFile(fileInfo));
                }
                else
                {
                    // if the file is not in the current server.
                    value = path;
                }

                value = _cache.Set<string>(path, value, cacheEntryOptions);
            }

            return value;
        }

        private static string GetContentsForFile(IFileInfo fileInfo)
        {

            var numberBytes = fileInfo.Length;
            int iNumberBytes = (int)numberBytes;
            var contentFile = new byte[numberBytes];
            using (var readStream = fileInfo.CreateReadStream())
            {
                var bytes = readStream.Read(contentFile, 0, (int)numberBytes);

                return Convert.ToBase64String(contentFile);
            }

        }
        private static string RetrieveBase64Prefix(IFileInfo fileInfo)
        {

            string extension = Path.GetExtension(fileInfo.Name);
            if (!extension.StartsWith("."))
            {
                throw new NotSupportedException(fileInfo.Name);
            }
            extension = extension.Substring(1).ToLowerInvariant();//remove .

            return string.Format("data:image/{0};base64,", extension);
        }
    }
}
