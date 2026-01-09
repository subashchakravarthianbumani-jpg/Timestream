using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Interface
{
    public interface IFTPHelpers
    {
        public bool UploadFile(FTPModel fTP);
        public bool MakeDirectory(FTPModel fTP);
        public string DownloadFile(FTPModel fTP);
        public Task<byte[]> DownloadFile_bytes(FTPModel fTP);
        public bool UploadFileFromBase64(FTPModel fTP);
        public bool DeleteFile(FTPModel fTP);
    }
}
