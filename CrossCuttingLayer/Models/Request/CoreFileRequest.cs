using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingLayer.Models.Request
{
    /// <summary>
    /// Request object to get the Core File
    /// </summary>
    public class CoreFileRequest : FileRequest
    {
        //The file id in Core Db
        public Guid DocId { get; set; }
    }
}
