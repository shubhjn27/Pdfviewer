using System.IO;

namespace FileViewer.Dependency.Implementation.Files.PdfTron
{
    internal class FileStreamWithDelete : FileStream
    {
        public FileStreamWithDelete(string path, FileMode mode = FileMode.Create)
            : base(path, mode)
        { }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            File.Delete(Name);
        }
    }
}
