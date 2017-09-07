using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingLayer.Models.Request
{
    /// <summary>
    /// Request object to get the File
    /// </summary>
    public class FileRequest
    {
        //the user vieweing the document for adding annotations
        public Guid RequestedUserId { get; set; }
        //optional see if required
        public Guid CreatedUserId { get; set; }
        public int AppId { get; set; }
    }
}
