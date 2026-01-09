using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utils.Interface;

namespace Utils
{
    public class FTPModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FtpSite { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FolderName { get; set; } = string.Empty;
        public string FileFromBase64 { get; set; } = string.Empty;
        public IFormFile? file { get; set; } = null;
    }
    public class FTPHelpers : IFTPHelpers
    {
        private readonly string _FtpSite;
        private readonly string _UserName;
        private readonly string _Password;
        public FTPHelpers(IConfiguration configuration)
        {
            _FtpSite = configuration.GetSection("FTP:FtpSite").Value.ToString();
            _UserName = configuration.GetSection("FTP:UserName").Value.ToString();
            _Password = configuration.GetSection("FTP:Password").Value.ToString();
        }
        public bool UploadFile(FTPModel fTP)
        {
            try
            {
                bool result;

                string urlString = "";
                if (!string.IsNullOrWhiteSpace(fTP.FolderName) && false)
                {
                    urlString = _FtpSite + "/" + fTP.FolderName + "/" + fTP.FileName;
                }
                else
                {
                    urlString = _FtpSite + "/" + fTP.FileName;
                }
                Uri uri = new Uri(urlString);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UsePassive = true;
                request.Credentials = new NetworkCredential(_UserName, _Password);
                byte[] fileContents;
                using (var ms = new MemoryStream())
                {
                    fTP?.file?.CopyTo(ms);
                    fileContents = ms.ToArray();
                    //string s = Convert.ToBase64String(fileBytes);
                }
                request.ContentLength = fileContents.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    result = response.StatusCode == FtpStatusCode.CommandOK ? true : false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool MakeDirectory(FTPModel fTP)
        {
            try
            {
                bool result;
                string urlString = "";
                if (!string.IsNullOrWhiteSpace(fTP.FolderName) && false)
                {
                    urlString = _FtpSite + "/" + fTP.FolderName + "/" + fTP.FileName;
                }
                else
                {
                    urlString = _FtpSite + "/" + fTP.FileName;
                }
                Uri uri = new Uri(urlString);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(_UserName, _Password);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    result = response.StatusCode == FtpStatusCode.CommandOK ? true : false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return true;
            }
        }
        public string DownloadFile(FTPModel fTP)
        {
            try
            {
                string base64;
                string urlString = "";
                if (!string.IsNullOrWhiteSpace(fTP.FolderName) && false)
                {
                    urlString = _FtpSite + "/" + fTP.FolderName + "/" + fTP.FileName;
                }
                else
                {
                    urlString = _FtpSite + "/" + fTP.FileName;
                }
                Uri uri = new Uri(urlString);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_UserName, _Password);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        responseStream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    base64 = Convert.ToBase64String(bytes);
                    reader.Close();
                    responseStream.Close();
                }
                return base64;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return "";
            }
        }
        public async Task<byte[]> DownloadFile_bytes(FTPModel fTP)
        {
            try
            {
                string urlString = "";
                byte[] bytes;
                if (!string.IsNullOrWhiteSpace(fTP.FolderName) && false)
                {
                    urlString = _FtpSite + "/" + fTP.FolderName + "/" + fTP.FileName;
                }
                else
                {
                    urlString = _FtpSite + "/" + fTP.FileName;
                }
                Uri uri = new Uri(urlString);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_UserName, _Password);
                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);

                    using (var memoryStream = new MemoryStream())
                    {
                        await responseStream.CopyToAsync(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                }
                return bytes;
            }
            catch
            {
                throw;
            }
        }
        public bool UploadFileFromBase64(FTPModel fTP)
        {
            try
            {
                bool result;
                string urlString = "";
                if (!string.IsNullOrWhiteSpace(fTP.FolderName) && false)
                {
                    urlString = _FtpSite + "/" + fTP.FolderName + "/" + fTP.FileName;
                }
                else
                {
                    urlString = _FtpSite + "/" + fTP.FileName;
                }
                Uri uri = new Uri(urlString);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(_UserName, _Password);
                byte[] fileContents = Convert.FromBase64String(fTP.FileFromBase64);
                request.ContentLength = fileContents.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    result = response.StatusCode == FtpStatusCode.CommandOK ? true : false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool DeleteFile(FTPModel fTP)
        {
            try
            {
                string urlString = "";
                if (!string.IsNullOrWhiteSpace(fTP.FolderName) && false)
                {
                    urlString = _FtpSite + "/" + fTP.FolderName + "/" + fTP.FileName;
                }
                else
                {
                    urlString = _FtpSite + "/" + fTP.FileName;
                }
                Uri uri = new Uri(urlString);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(_UserName, _Password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                if (response.StatusDescription != null && response.StatusDescription.Contains("250 DELE command successful."))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }




}
