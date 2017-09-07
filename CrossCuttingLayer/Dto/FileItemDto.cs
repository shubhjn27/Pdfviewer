using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingLayer.Dto
{
    public class FileItemDto
    {
        public int DocId { get; set; }
        public string FileName { get; set; }
        public int AppId { get; set; }
        public int FileSource { get; set; }
        public Dictionary<string, string> FileSourceMetadata { get; set; }
    }
}
