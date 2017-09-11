using FileViewer.CrossCuttingLayer.Enums;
using FileViewer.CrossCuttingLayer.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace FileService.Controllers
{
    [RoutePrefix("api/file")]
    public class FileController : ApiController
    {

        [Route("view/aws")]
        [HttpGet]
        public async Task<FileRequest> ViewFile(AWSFileRequest file)
        {
            FileRequest fileResponse = new FileRequest();
            //var enumsSelected = WaterMarkInfoFields.AccessDateTime | WaterMarkInfoFields.CustomMessage | WaterMarkInfoFields.EmailAddress;
            return fileResponse;
        }

    }
}
