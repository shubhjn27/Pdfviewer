using System.Collections.Generic;

namespace FileViewer.CrossCuttingLayer.Dto
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
