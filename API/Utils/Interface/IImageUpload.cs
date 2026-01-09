using Microsoft.AspNetCore.Http;

namespace Utils.Interface
{
    public interface IImageUpload
    {
        string UploadimageInBase64(string ImageString, string ImageNamePrefix);
        public string UploadFile(IFormFile? formFile);
        public bool DeleteFile(string fileNameWithExt);
    }
}
