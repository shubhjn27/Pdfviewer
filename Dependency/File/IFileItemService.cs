using FileViewer.CrossCuttingLayer.Models.Request;
using System;

namespace FileViewer.Dependency.File
{
    internal interface IFileItemService
    {
        public FileItemResponse ViewFileItem(FileRequest);
    }
}
