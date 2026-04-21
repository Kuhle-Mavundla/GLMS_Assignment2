using GLMS.Web.Services.Interfaces;

namespace GLMS.Web.Services
{
    // This service handles PDF file uploads.
    // It validates that only .pdf files are accepted, saves them with unique names,
    // and stores them in the wwwroot/uploads folder on the server.
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileService> _logger;

        // Only these file extensions are allowed (strict PDF-only policy)
        private static readonly string[] AllowedExtensions = { ".pdf" };
        private static readonly string[] AllowedMimeTypes = { "application/pdf" };

        public FileService(IWebHostEnvironment env, ILogger<FileService> logger)
        {
            _env = env;
            _logger = logger;
        }

        // Checks if the uploaded file is a valid PDF.
        // Returns false for null files, wrong extensions, or wrong MIME types.
        public bool IsValidPdf(IFormFile? file)
        {
            // Reject null or empty files
            if (file == null || file.Length == 0)
                return false;

            // Check the file extension (must be .pdf)
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                return false;

            // Check the MIME type (must be application/pdf)
            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }

        // Saves the uploaded PDF file to the server with a unique filename.
        // Returns the relative path to be stored in the database.
        public async Task<string> SaveFileAsync(IFormFile file)
        {
            // Double-check validation before saving
            if (!IsValidPdf(file))
                throw new InvalidOperationException("Only PDF files are allowed.");

            // Generate a unique filename using a GUID to prevent name collisions
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // Build the full path to the uploads folder
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            // Create the folder if it does not exist yet
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Save the file to disk
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            _logger.LogInformation("Saved file {Original} as {Unique}", file.FileName, uniqueFileName);

            // Return the relative path (this gets stored in the database)
            return $"/uploads/{uniqueFileName}";
        }

        // Converts a relative path (from the database) to a full server file path
        public string GetFilePath(string relativePath)
        {
            return Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
        }
    }
}