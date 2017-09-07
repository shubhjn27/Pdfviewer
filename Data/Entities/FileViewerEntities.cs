using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileViewer.Data.Entities
{
    public class FileViewerEntities : DbContext
    {
        static FileViewerEntities()
        {
            Database.SetInitializer<FileViewerEntities>(null);
        }
        public FileViewerEntities() : base("Name=FileViewerEntities")
        {
        }
    }
}
