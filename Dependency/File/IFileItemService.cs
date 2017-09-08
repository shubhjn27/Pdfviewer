using FileViewer.CrossCuttingLayer.Models.Request;
using FileViewer.CrossCuttingLayer.Models.Response;
using System;
using System.IO;

namespace FileViewer.Dependency.File
{
    public interface IFileItemService
    {
        FileItemResponse ViewFileItem(FileRequest viewFile);
        FileStream GetFile()
    }
}
