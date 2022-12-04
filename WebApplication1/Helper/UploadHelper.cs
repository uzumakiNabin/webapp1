namespace WebApplication1.Helper
{
    public class UploadHelper
    {
        private readonly IConfiguration _config;
        public UploadHelper(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<string> UploadImage(string folderPath, IFormFile file)
        {

            var fileType = file.ContentType;
            if (fileType == "image/jpeg" || fileType == "image/png" || fileType == "application/pdf" || fileType == "application/jpg")
            {
                var docPath = _config["ApplicationConfigurations:DocumentPath"];
                //var docPath = Path.Combine("E:\\FileServerDocuments", "\\AgentPortal");
                if (!Directory.Exists(docPath))
                    Directory.CreateDirectory(docPath);

                if (!Directory.Exists(docPath + "\\" + folderPath))
                    Directory.CreateDirectory(docPath + "\\" + folderPath);
                folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

                string serverFolder = Path.Combine(docPath, folderPath);

                await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                return "/" + folderPath;
            }
            return "/" + folderPath;
        }
    }
}
