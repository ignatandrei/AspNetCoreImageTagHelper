# AspNetCoreImageTagHelper


First Release for rendering base 64 images in ASP.NET Core.

See https://en.wikipedia.org/wiki/Data_URI_scheme  for more details.

Instructions for AspNetCore:

In the _ViewImports @addTagHelper *, AspNetCore.Mvc.ImageBase64 

In the Razor page just put

 &lt;img src='~/ relative path to the image ' asp-render-base64='true' /&gt;
