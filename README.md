# AspNetCoreImageTagHelper


First Release for rendering base 64 images in ASP.NET Core.

See https://en.wikipedia.org/wiki/Data_URI_scheme  for more details.

Instructions for AspNetCore:

In the _ViewImports.cshtml  put

@addTagHelper *, AspNetCore.Mvc.ImageBase64 

In the Razor page just put

 &lt;img src='~/ relative path to the image ' asp-render-base64='true' /&gt;

 NuGet Package at https://www.nuget.org/packages/AspNetCore.Mvc.ImageBase64/ 
 
 ( to generate the nuget package run\

 dotnet pack -c Release
 
 in the folder )