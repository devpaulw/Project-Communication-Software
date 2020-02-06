//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Text;
//using FluentFTP;

//namespace PCS
//{
//    // DISCONTINUED, used FluentFtp instead
//    //class SdnFtpClient : IDisposable
//    //{
//    //    private readonly FtpClient ftpClient;

//    //    public IPAddress IP { get; }
//    //    public ushort Port { get; }

//    //    public SdnFtpClient(IPAddress ip, ushort port = 21)
//    //    {
//    //        IP = ip ?? throw new ArgumentNullException(nameof(ip));
//    //        Port = port;

//    //        ftpClient = new FtpClient();
//    //        ftpClient.Host = ip.MapToIPv4().ToString();
//    //        ftpClient.Port = PcsFtpServer.Port;
//    //        ftpClient.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//    //        ftpClient.Connect();
//    //    }

//    //    public void UploadFile(string localFilePath, string path)
//    //    {
//    //        byte[] buffer;

//    //        using (FileStream stream = File.OpenRead(localFilePath))
//    //        {
//    //            buffer = new byte[stream.Length];
//    //            stream.Read(buffer, 0, buffer.Length);
//    //        }

//    //        using (var uploadStream = GetUploadStream(path))
//    //        {
//    //            uploadStream.Write(buffer, 0, buffer.Length);
//    //        }
//    //    }

//    //    public Stream GetDownloadStream(string path)
//    //    {
//    //        var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//    //        request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//    //        request.Method = WebRequestMethods.Ftp.DownloadFile;

//    //        var response = request.GetResponse() as FtpWebResponse;

//    //        return response.GetResponseStream();
//    //    }

//    //    public Stream GetUploadStream(string path)
//    //    {
//    //        DefineMissingDirectories(path);

//    //        var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//    //        request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs"); // DOLATER: Find cleaner, use passwords, and SSL
//    //        request.Method = WebRequestMethods.Ftp.UploadFile;
//    //        request.GetResponse();

//    //        return request.GetRequestStream();
//    //    }

//    //    public Stream GetAppendStream(string path)
//    //    {
//    //        DefineMissingDirectories(path);

//    //        var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//    //        request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs"); // DOLATER: Find cleaner, use passwords, and SSL
//    //        request.Method = WebRequestMethods.Ftp.AppendFile;

//    //        return request.GetRequestStream();
//    //    }

//    //    public void DefineMissingDirectories(string path)
//    //    {
//    //        var parentDirs = GetParentDirectories(path, true);

//    //        foreach (var parentDir in parentDirs)
//    //        {
//    //            if (!DirectoryExists(parentDir))
//    //                MakeDirectory(parentDir);
//    //        }
//    //    }

//    //    public void MakeDirectory(string path)
//    //    {
//    //        var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//    //        request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//    //        request.Method = WebRequestMethods.Ftp.MakeDirectory;
//    //        request.GetResponse();

//    //    }

//    //    public void CreateFile(string path)
//    //    {
//    //        var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//    //        request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//    //        request.Method = WebRequestMethods.Ftp.UploadFile;
//    //        request.GetRequestStream().Close();
//    //        request.GetResponse().Close();
//    //    }

//    //    public bool DirectoryExists(string path)
//    //    {
//    //        var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}/") as FtpWebRequest;
//    //        request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//    //        request.Method = WebRequestMethods.Ftp.ListDirectory;

//    //        try
//    //        {
//    //            request.GetResponse();

//    //            return true; // The file exists because there are not errors when wishing get infos of it
//    //        }
//    //        catch (WebException)
//    //        {
//    //            return false;
//    //        }
//    //    }

//    //    public bool FileExists(string path)
//    //    {
//    //        return ftpClient.FileExists(path);
//    //    }

//    //    private static IEnumerable<string> GetParentDirectories(string path, bool isFilePath)
//    //    {
//    //        var parentDirs = new List<string>();

//    //        string directory = path;

//    //        if (!isFilePath)
//    //            parentDirs.Add(path);

//    //        while (true)
//    //        {
//    //            directory = Path.GetDirectoryName(directory);

//    //            if (!string.IsNullOrEmpty(directory))
//    //                parentDirs.Add(directory);
//    //            else
//    //                break;
//    //        }

//    //        parentDirs.Reverse();

//    //        return parentDirs;
//    //    }

//    //    #region IDisposable Support
//    //    private bool disposedValue = false; // To detect redundant calls

//    //    protected virtual void Dispose(bool disposing)
//    //    {
//    //        if (!disposedValue)
//    //        {
//    //            if (disposing)
//    //            {
//    //            }

//    //            disposedValue = true;
//    //        }
//    //    }
//    //    public void Dispose()
//    //    {
//    //        Dispose(true);
//    //        GC.SuppressFinalize(this);
//    //    }
//    //    #endregion
//    //}

//    class SdnFtpClient
//    {
//        public IPAddress IP { get; }
//        public ushort Port { get; }

//        public SdnFtpClient(IPAddress ip, ushort port = 21)
//        {
//            IP = ip ?? throw new ArgumentNullException(nameof(ip));
//            Port = port;
//        }

//        public void UploadFile(string localFilePath, string path)
//        {
//            byte[] buffer;

//            using (FileStream stream = File.OpenRead(localFilePath))
//            {
//                buffer = new byte[stream.Length];
//                stream.Read(buffer, 0, buffer.Length);
//            }

//            using (var uploadStream = GetUploadStream(path))
//            {
//                uploadStream.Write(buffer, 0, buffer.Length);
//            }
//        }

//        public Stream GetDownloadStream(string path)
//        {
//            var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//            request.Method = WebRequestMethods.Ftp.DownloadFile;

//            var response = request.GetResponse() as FtpWebResponse;

//            return response.GetResponseStream();
//        }

//        public Stream GetUploadStream(string path)
//        {
//            DefineMissingDirectories(path);

//            var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs"); // DOLATER: Find cleaner, use passwords, and SSL
//            request.Method = WebRequestMethods.Ftp.UploadFile;
//            request.GetResponse();

//            return request.GetRequestStream();
//        }

//        public Stream GetAppendStream(string path)
//        {
//            DefineMissingDirectories(path);

//            var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs"); // DOLATER: Find cleaner, use passwords, and SSL
//            request.Method = WebRequestMethods.Ftp.AppendFile;

//            return request.GetRequestStream();
//        }

//        public void DefineMissingDirectories(string path)
//        {
//            var parentDirs = GetParentDirectories(path, true);

//            foreach (var parentDir in parentDirs)
//            {
//                if (!DirectoryExists(parentDir))
//                    MakeDirectory(parentDir);
//            }
//        }

//        public void MakeDirectory(string path)
//        {
//            var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//            request.Method = WebRequestMethods.Ftp.MakeDirectory;
//            request.GetResponse();

//        }

//        public void CreateFile(string path)
//        {
//            var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//            request.Method = WebRequestMethods.Ftp.UploadFile;
//            request.GetRequestStream().Close();
//            request.GetResponse().Close();
//        }

//        public bool DirectoryExists(string path)
//        {
//            var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}/") as FtpWebRequest;
//            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//            request.Method = WebRequestMethods.Ftp.ListDirectory;

//            try
//            {
//                request.GetResponse();

//                return true; // The file exists because there are not errors when wishing get infos of it
//            }
//            catch (WebException)
//            {
//                return false;
//            }
//        }

//        public bool FileExists(string path)
//        {
//            var request = WebRequest.Create($"ftp://{IP.MapToIPv4()}:{Port}/{path}") as FtpWebRequest;
//            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
//            request.Method = WebRequestMethods.Ftp.GetFileSize;

//            try
//            {
//                request.GetResponse();

//                return true;
//            }
//            catch (WebException)
//            {
//                return false;
//            }
//        }

//        private static IEnumerable<string> GetParentDirectories(string path, bool isFilePath)
//        {
//            var parentDirs = new List<string>();

//            string directory = path;

//            if (!isFilePath)
//                parentDirs.Add(path);

//            while (true)
//            {
//                directory = Path.GetDirectoryName(directory);

//                if (!string.IsNullOrEmpty(directory))
//                    parentDirs.Add(directory);
//                else
//                    break;
//            }

//            parentDirs.Reverse();

//            return parentDirs;
//        }
//    }
//}
