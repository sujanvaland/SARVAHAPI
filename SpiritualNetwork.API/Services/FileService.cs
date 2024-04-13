using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using System.Net;

namespace SpiritualNetwork.API.Services
{
    public class FileService : IFileService
    {
        private IRepository<Entities.File> _fileRepository;
        private IRepository<PostFiles> _postFiles;
        private IWebHostEnvironment _webHostEnvironment;

        public FileService(
            IRepository<Entities.File> attachmentRepository,
            IRepository<PostFiles> postFiles,
            IWebHostEnvironment webHostEnvironment)
        {
            _fileRepository = attachmentRepository;
            _postFiles = postFiles;
            _webHostEnvironment = webHostEnvironment;
        }

        private List<string> SaveFileToImagesFolder(byte[] fileContents, string filename)
        {
            string rootPath = _webHostEnvironment.ContentRootPath;
            string imagesFolderPath = Path.Combine(rootPath, "posts/images");

            // Ensure 'images' directory exists, if not create it
            Directory.CreateDirectory(imagesFolderPath);

            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // Unique timestamp
            string tempfilename = "K4M2A_" + timestamp;
            string filePath = Path.Combine(imagesFolderPath, tempfilename); // Your file name

            try
            {
                string url = filePath;
                List<string> urlstring = new List<string>();
                System.IO.File.WriteAllBytes(filePath, fileContents);
                urlstring.Add(tempfilename);
                urlstring.Add(filePath);
                urlstring.Add(filePath);
                return urlstring;
                // Console.WriteLine("File saved successfully.");
            }
            catch (Exception ex)
            {
                return null;
                // Console.WriteLine("Error saving file: " + ex.Message);
            }
        }

        public string UploadImagesToFtp(IFormFile imgfile, string filename, string path, string ftpServerUrl = "ftp://45.114.245.207", string ftpUsername = "k4m2a.co_dv2lwekpooa", string ftpPassword = "Iry?jwu84Bhq^8Sq")
        {
            try
            {
                // string remoteDirectory = "httpdocs/post/images"
                // Get the file bytes from the IFormFile
                using (MemoryStream ms = new MemoryStream())
                {

                    imgfile.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();

                    var remoteDirectory = "httpdocs/" + path;
                    // Create a FTP request object
                    FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create($"{ftpServerUrl}/{remoteDirectory}/{filename}");

                    // Set credentials
                    ftpRequest.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                    // Specify the FTP command
                    ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                    // Write the file data to the request stream
                    using (Stream requestStream = ftpRequest.GetRequestStream())
                    {
                        requestStream.Write(fileBytes, 0, fileBytes.Length);
                    }

                    // Get FTP server's response
                    using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())
                    {
                        return filename;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Entities.File>> UploadFile(IFormCollection form)
        {
            try
            {
                var path = form["path"].ToString();
                List<Entities.File> filearr = new List<Entities.File>();
                
                foreach (var item in form.Files)
                {
                    Entities.File file = new Entities.File();
                    byte[] TempContent;

                    using (var memory = new MemoryStream())
                    {
                        item.CopyTo(memory);
                        TempContent = memory.ToArray();
                    }


                    string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // Unique timestamp

                    string tempfilename = "K4M2A_" + timestamp + Path.GetExtension(item.FileName);

                    string tempname = UploadImagesToFtp(item, tempfilename,path);

                    byte[] emptybyte = new byte[0];

                    file.ContentType = item.ContentType;
                    file.Content = emptybyte;
                    file.FileExtension = Path.GetExtension(item.FileName);
                    file.FileName = tempfilename;
                    file.ThumbnailUrl = "https://www.k4m2a.com/"+ path + tempfilename;
                    file.ActualUrl = "https://www.k4m2a.com/"+path + tempfilename;
                    filearr.Add(file);
                }

                await _fileRepository.InsertRangeAsync(filearr);
                return filearr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
