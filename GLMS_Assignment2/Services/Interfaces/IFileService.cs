using Microsoft.AspNetCore.Http;
namespace GLMS.Web.Services.Interfaces
{ public interface IFileService { bool IsValidPdf(IFormFile? file); Task<string> SaveFileAsync(IFormFile file); string GetFilePath(string relativePath); } }