using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingLayer.Models.Request
{
    /// <summary>
    /// Request object to get the AWS File
    /// </summary>
    public class AWSFileRequest : FileRequest
    {
        //AWS Bucket name
        public string BucketName { get; set; }

        //Key to identify file in a bucket
        public string keyName { get; set; }
    }
}
