namespace CenIT.DegreeManagement.CoreAPI.Processor.UploadFile
{
    public interface IFileService
    {
        public Tuple<int, string> SaveImage(IFormFile imageFile, string folderName);
        public Tuple<int, string> SaveFileImage(IFormFile imageFile, string folderName);

        public Tuple<int, string> SaveFilePDFOrWorld(IFormFile imageFile, string folderName);
        public Tuple<int, string> SaveFile(IFormFile imageFile, string folderName);

        public bool DeleteImage(string imageFileName);
    }
}
