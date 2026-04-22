using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using GLMS_Assignment2.Services;

namespace GLMS_Tests
{
    /// <summary>
    /// Tests for file upload validation.
    /// Verifies that uploading a restricted file type throws an error (only .pdf allowed).
    /// </summary>
    public class FileValidationTests
    {
        private readonly FileService _fileService;

        public FileValidationTests()
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            var mockLogger = new Mock<ILogger<FileService>>();
            _fileService = new FileService(mockEnv.Object, mockLogger.Object);
        }

        private static IFormFile CreateMockFile(string fileName, string contentType, int length = 1024)
        {
            var mock = new Mock<IFormFile>();
            mock.Setup(f => f.FileName).Returns(fileName);
            mock.Setup(f => f.ContentType).Returns(contentType);
            mock.Setup(f => f.Length).Returns(length);
            return mock.Object;
        }

        [Fact]
        public void IsValidPdf_ValidPdfFile_ReturnsTrue()
        {
            var file = CreateMockFile("contract.pdf", "application/pdf");
            Assert.True(_fileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_ExeFile_ReturnsFalse()
        {
            var file = CreateMockFile("malware.exe", "application/octet-stream");
            Assert.False(_fileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_TxtFile_ReturnsFalse()
        {
            var file = CreateMockFile("notes.txt", "text/plain");
            Assert.False(_fileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_DocxFile_ReturnsFalse()
        {
            var file = CreateMockFile("document.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            Assert.False(_fileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_NullFile_ReturnsFalse()
        {
            Assert.False(_fileService.IsValidPdf(null));
        }

        [Fact]
        public void IsValidPdf_EmptyFile_ReturnsFalse()
        {
            var file = CreateMockFile("empty.pdf", "application/pdf", 0);
            Assert.False(_fileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_WrongMimeType_ReturnsFalse()
        {
            // .pdf extension but wrong MIME type
            var file = CreateMockFile("fake.pdf", "text/plain");
            Assert.False(_fileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_NoExtension_ReturnsFalse()
        {
            var file = CreateMockFile("noextension", "application/pdf");
            Assert.False(_fileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_JpgFile_ReturnsFalse()
        {
            var file = CreateMockFile("photo.jpg", "image/jpeg");
            Assert.False(_fileService.IsValidPdf(file));
        }
    }
}