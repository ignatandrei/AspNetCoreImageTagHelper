using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace AspNetCore.Mvc.ImageBase64
{
    /// <summary>
    /// <see cref="ITagHelper"/> implementation targeting &lt;img&gt; elements that supports base64 data uri
    /// Made by Andrei Ignat, http://msprogrammer.serviciipeweb.ro/
    /// </summary>
    /// <remarks>
    /// The tag helper won't process for cases with just the 'src' attribute.
    /// </remarks>
    [HtmlTargetElement(
        "img",
        Attributes = RenderBase64AttributeName + "," + SrcAttributeName,
        TagStructure = TagStructure.WithoutEndTag)]
    public class ImageTagBase64Helper : UrlResolutionTagHelper
    {
        private static readonly string Namespace = typeof(ImageTagBase64Helper).Namespace;

        private const string RenderBase64AttributeName = "asp-render-base64";
        private const string SrcAttributeName = "src";

        private FileBase64ContentProvider _fileBase64ContentProvider;

        /// <summary>
        /// Creates a new <see cref="ImageTagBase64Helper"/>.
        /// </summary>
        /// <param name="hostingEnvironment">The <see cref="IHostingEnvironment"/>.</param>
        /// <param name="cache">The <see cref="IMemoryCache"/>.</param>
        /// <param name="htmlEncoder">The <see cref="HtmlEncoder"/> to use.</param>
        /// <param name="urlHelperFactory">The <see cref="IUrlHelperFactory"/>.</param>
        public ImageTagBase64Helper(
            IHostingEnvironment hostingEnvironment,
            IMemoryCache cache,
            HtmlEncoder htmlEncoder,
            IUrlHelperFactory urlHelperFactory)
            : base(urlHelperFactory, htmlEncoder)
        {
            HostingEnvironment = hostingEnvironment;
            Cache = cache;
        }

        /// <inheritdoc />
        public override int Order
        {
            get
            {
                return -1000;
            }
        }

        /// <summary>
        /// Source of the image.
        /// </summary>
        /// <remarks>
        /// Passed through to the generated HTML in all cases.
        /// </remarks>
        [HtmlAttributeName(SrcAttributeName)]
        public string Src { get; set; }

        /// <summary>
        /// Value indicating if file version should be appended to the src urls.
        /// </summary>
        /// <remarks>
        /// If <c>true</c> then a query string "v" with the encoded content of the file is added.
        /// </remarks>
        [HtmlAttributeName(RenderBase64AttributeName)]
        public bool RenderBase64 { get; set; }

        private IHostingEnvironment HostingEnvironment { get; }

        private IMemoryCache Cache { get; }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.CopyHtmlAttribute(SrcAttributeName, context);
            ProcessUrlAttribute(SrcAttributeName, output);

            if (RenderBase64)
            {
                EnsureBase64ContentProvider();

                // Retrieve the TagHelperOutput variation of the "src" attribute in case other TagHelpers in the
                // pipeline have touched the value. If the value is already encoded this ImageTagHelper may
                // not function properly.
                Src = output.Attributes[SrcAttributeName].Value as string;

                output.Attributes.SetAttribute(SrcAttributeName, _fileBase64ContentProvider.RetrieveBase64Data(Src));
            }
        }

        private void EnsureBase64ContentProvider()
        {
            if (_fileBase64ContentProvider == null)
            {
                _fileBase64ContentProvider = new FileBase64ContentProvider(
                    HostingEnvironment.WebRootFileProvider,
                    Cache,
                    ViewContext.HttpContext.Request.PathBase);
            }
        }
    }
}
