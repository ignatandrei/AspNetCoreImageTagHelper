# AspNetCoreImageTagHelper

[![Build status](https://ci.appveyor.com/api/projects/status/pkt3xn2wquk1duhd?svg=true)](https://ci.appveyor.com/project/davidrevoledo/aspnetcoreimagetaghelper)
![NuGet](https://img.shields.io/nuget/v/AspNetCore.Mvc.ImageBase64.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)


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
 
 # Support this software

This software is available for free and all of its source code is public domain.  If you want further modifications, or just to show that you appreciate this, money are always welcome.

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://paypal.me/ignatandrei1970/25)

* $5 for a cup of coffee
* $10 for pizza 
* $25 for a lunch or two
* $100+ for upgrading my development environment

