using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Utils.Interface;

namespace Utils
{
    public class ImageUpload : IImageUpload
    {
        private readonly IConfiguration _configuration;
        public ImageUpload(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string UploadimageInBase64(string ImageString, string ImageNamePrefix)
        {
            try
            {
                string base64 = ImageString.Replace("data:image/jpeg;base64,", "");

                string Name = ImageNamePrefix + "_" + DateTime.Now.ToString("yyyyMMddHHmmssf");
                string fileName = Name + ".jpeg";
                string pathstring = _configuration["ImageUpload:Path"];
                string destFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathstring));
                if (!Directory.Exists(destFile))
                {
                    Directory.CreateDirectory(destFile);
                }
                string destFile1 = Path.Combine(destFile, fileName);

                // convert string to stream
                byte[] byteArray = Encoding.UTF8.GetBytes(base64);

                //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                MemoryStream stream = new MemoryStream(byteArray);

                if (System.IO.File.Exists(destFile1))
                {
                    System.IO.File.Delete(destFile1);
                }
                using (FileStream fs = new FileStream(destFile1, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] bytes = Convert.FromBase64String(base64);
                        bw.Write(bytes, 0, bytes.Length);
                        bw.Close();
                    }
                }
                //compressimagesize(0.5, stream, destFile1);
                return Name;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        private void compressimagesize(double scaleFactor, Stream sourcePath, string targetPath)
        {
            using (var image = System.Drawing.Image.FromStream(sourcePath))
            {
                var imgnewwidth = (int)(image.Width * scaleFactor);
                var imgnewheight = (int)(image.Height * scaleFactor);
                var imgthumnail = new Bitmap(imgnewwidth, imgnewheight);
                var imgthumbgraph = Graphics.FromImage(imgthumnail);
                imgthumbgraph.CompositingQuality = CompositingQuality.HighQuality;
                imgthumbgraph.SmoothingMode = SmoothingMode.HighQuality;
                imgthumbgraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, imgnewwidth, imgnewheight);
                imgthumbgraph.DrawImage(image, imageRectangle);
                imgthumnail.Save(targetPath, image.RawFormat);
            }
        }
        public string UploadFile(IFormFile? formFile)
        {
            try
            {
                if (formFile is not null)
                {
                    string fileName = "Report_" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
                    string extension = Path.GetExtension(formFile.FileName);
                    fileName += extension;

                    string pathstring = _configuration["ImageUpload:Path"];
                    string destFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathstring));

                    if (!Directory.Exists(destFile))
                    {
                        Directory.CreateDirectory(destFile);
                    }

                    string destFile1 = Path.Combine(destFile, fileName);
                    if (System.IO.File.Exists(destFile1))
                    {
                        System.IO.File.Delete(destFile1);
                    }

                    using (FileStream stream = new FileStream(Path.Combine(destFile, fileName), FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    return fileName;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public bool DeleteFile(string fileNameWithExt)
        {
            try
            {
                if (string.IsNullOrEmpty(fileNameWithExt))
                {
                    return false;
                }
                else
                {
                    string pathstring = _configuration["ImageUpload:Path"];
                    string destFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, pathstring));
                    string destFile1 = Path.Combine(destFile, fileNameWithExt);
                    if (System.IO.File.Exists(destFile1))
                    {
                        System.IO.File.Delete(destFile1);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }
    }
}
