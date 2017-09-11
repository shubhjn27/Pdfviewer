using FileViewer.CrossCuttingLayer.Enums;
using FileViewer.CrossCuttingLayer.Models.Watermark;
using pdftron.PDF;
using pdftron.SDF;
using System.IO;

namespace FileViewer.Dependency.Implementation.Files.PdfTron
{
    public class FileStreamWithPdftron : Stream
    {
        private readonly FileStreamWithDelete _wrappedStream;

        public FileStreamWithPdftron(string pdfFilePath, FileItemStreamType streamType, WaterMarkInfo watermarkInfo)
        {
            var tempFilePath = string.Empty;

            try
            {
                tempFilePath = Path.GetTempFileName();

                pdftron.PDFNet.Initialize(Properties.Settings.Default.PDFTronLicense);

                using (var doc = new PDFDoc(pdfFilePath))
                {
                    if (null != watermarkInfo)
                       WatermarkFile(doc, watermarkInfo);

                    if (streamType == FileItemStreamType.Downloadable)
                        doc.Save(tempFilePath, SDFDoc.SaveOptions.e_compatibility);
                    else
                        Convert.ToXod(doc, tempFilePath);

                    doc.Close();
                }

                _wrappedStream = new FileStreamWithDelete(tempFilePath, FileMode.Open);
            }
            catch
            {

                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
                throw;
            }
            finally
            {
                if (File.Exists(pdfFilePath))
                    File.Delete(pdfFilePath);
            }
        }

        private static void WatermarkFile(PDFDoc doc, WaterMarkInfo watermarkInfo)
        {
            using (var stamp = new Stamper(Stamper.SizeType.e_relative_scale, 1, 1))
            {
                doc.InitSecurityHandler();

                stamp.SetAlignment(Stamper.HorizontalAlignment.e_horizontal_center, Stamper.VerticalAlignment.e_vertical_center);
                stamp.SetFontColor(new ColorPt(0, 0, 0)); // set text color to red   
                stamp.SetOpacity(0.1);
                stamp.SetRotation(-67);
                stamp.ShowsOnPrint(true);
                stamp.ShowsOnScreen(true);
                stamp.SetAsBackground(false);
                stamp.StampText(doc, watermarkInfo.CustomMessage + "\n"
                                     + watermarkInfo.UserIPAddress,
                                     new PageSet(1, doc.GetPageCount()));
            }
        }

        #region Unsupported Stream Ops


        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        #endregion // Unsupported Stream Ops


        #region Wrapper Methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _wrappedStream.Dispose();
        }

        public override void Flush()
        {
            _wrappedStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _wrappedStream.Seek(offset, origin);
        }


        public override int Read(byte[] buffer, int offset, int count)
        {
            return _wrappedStream.Read(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return _wrappedStream.CanRead; }
        }


        public override bool CanSeek
        {
            get { return _wrappedStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _wrappedStream.Length; }
        }

        public override long Position
        {
            get { return _wrappedStream.Position; }
            set { _wrappedStream.Position = value; }
        }

        #endregion //Wrapper Methods

    }
}
