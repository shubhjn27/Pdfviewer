using FileViewer.Dependency.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileViewer.Dependency.Implementation.Files
{
    public abstract class FileItem<TFileRequest, TResponse> : IFileItemService<FileStream, FileStream>
    {
        public FileStream GetFile()
        {
            throw new NotImplementedException();
        }

        public FileStream ViewFileItem(FileStream viewFile)
        {
            throw new NotImplementedException();
        }
    }
}
