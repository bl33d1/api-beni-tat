using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.WebHost.UseUrls("http://+:5000");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}



app.UseRouting();

app.MapGet("/api/pictures", async (HttpContext context) =>
{
    // Directory path where your image files are located
    string directoryPath = "C:/Users/Administrator/Documents/auraAdminPanels/kusheshtebeni/resources/css/img/gallery";

    // Read files from the directory
    string[] files = Directory.GetFiles(directoryPath);

    // Create the pictures array
    var pictures = files.Select(Path.GetFileName).ToArray();

    // Return the pictures as JSON
    await context.Response.WriteAsJsonAsync(pictures);
});

app.MapGet("/api/last-pictures", async (HttpContext context) =>
{
    // Directory path where your image files are located
    string directoryPath = "C:/Users/Administrator/Documents/auraAdminPanels/kusheshtebeni/resources/css/img/gallery";

    // Read files from the directory
    string[] files = Directory.GetFiles(directoryPath);

    // Get the first 8 pictures
    var first8Pictures = files.Select(Path.GetFileName).Take(8).ToArray();

    // Return the first 8 pictures as JSON
    await context.Response.WriteAsJsonAsync(first8Pictures);
});


app.MapGet("/api/get-picture/{encodedImageName}", async (HttpContext context) =>
{
    // Directory path where your image files are located
    string directoryPath = "C:/Users/Administrator/Documents/auraAdminPanels/kusheshtebeni/resources/css/img/gallery";

    // Get the encoded image name from the route parameter
    string encodedImageName = context.Request.RouteValues["encodedImageName"].ToString();

    // Decode the encoded image name
    string imageName = WebUtility.UrlDecode(encodedImageName);

    // Full path to the image file
    string imagePath = Path.Combine(directoryPath, imageName);

    // Check if the file exists
    if (File.Exists(imagePath))
    {
        // Create an instance of MimeTypesMap
        var mimeTypesMap = new FileExtensionContentTypeProvider();

        // Get the content type for the image file
        string contentType;
        if (!mimeTypesMap.TryGetContentType(imagePath, out contentType))
        {
            contentType = "application/octet-stream"; // Default content type if not found
        }

        // Set the content type
        context.Response.ContentType = contentType;

        // Write the file content to the response
        await context.Response.SendFileAsync(imagePath);
    }
    else
    {
        // Return 404 Not Found if the file does not exist
        context.Response.StatusCode = StatusCodes.Status404NotFound;
    }
});

app.Run();
