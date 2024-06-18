// Controllers/HomeController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

public class HomeController : Controller
{
    private readonly string _videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"); // Path to store videos

    public HomeController()
    {
        FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official).Wait();
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var filePath = Path.Combine(_videoPath, file.FileName);

            // Ensure the directory exists
            if (!Directory.Exists(_videoPath))
            {
                Directory.CreateDirectory(_videoPath);
            }

            // Save the file temporarily to check its duration
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Check video duration
            var mediaInfo = await FFmpeg.GetMediaInfo(filePath);
            var duration = mediaInfo.Duration;

            if (duration.TotalSeconds > 10)
            {
                // Delete the temporary file
                System.IO.File.Delete(filePath);
                return BadRequest("Video duration exceeds 10 seconds.");
            }

            // Proceed with your processing logic if video duration is valid

            var testFilePath = Path.Combine(_videoPath, "test.txt");
            await System.IO.File.WriteAllTextAsync(testFilePath, "This is a test file.");

            return RedirectToAction("Index");
        }

        return BadRequest("Invalid file.");
    }
}




