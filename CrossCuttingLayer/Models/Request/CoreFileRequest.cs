using FileViewer.CrossCuttingLayer.Enums;
using System;

namespace FileViewer.CrossCuttingLayer.Models.Request
{
    /// <summary>
    /// Request object to get the Core File
    /// </summary>
    public class CoreFileRequest : FileRequest<FileSource>
    {
        //The file id in Core Db
        public Guid DocId { get; set; }
    }
}
