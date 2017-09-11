using FileViewer.CrossCuttingLayer.Models.Request;
using FileViewer.CrossCuttingLayer.Models.Response;
using System;
using System.IO;

namespace FileViewer.Dependency.Files
{
    public interface IFileItemService<TFileRequest,TResponse>
    {
        TResponse ViewFileItem(TFileRequest viewFile);
        FileStream GetFile();
    }
}
