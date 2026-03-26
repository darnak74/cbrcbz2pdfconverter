using System;
using System.Collections.Generic;
using System.Drawing;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;

namespace CbrConverter
{
    public class PdfFunctions
    {
        public static event UpdateCurrentBar evnt_UpdateCurBar;
        public delegate void UpdateCurrentBar();


        public static PdfExtractionResult PDF_ExportImage(string filename, string dirForExtractions, int divider, bool checkResult, bool joinImages)
        {
            var details = new PdfExtractionResult();

            DataAccess.Instance.g_curProgress = 0;
            evnt_UpdateCurBar();

            var imagesList = new Dictionary<PageImageIndex, Image>();          

            // Ask itextsharp to extract image
            var pdfReader = new PdfReader(filename);
            var pdfParser = new PdfReaderContentParser(pdfReader);
            var pdfListener = new PDFImageListener(dirForExtractions);

            double tem0 = divider;
            double pgc = pdfReader.NumberOfPages;
            double CurOneStep = (double)(tem0 / pgc);

            details.Pages = (int)pgc;

            for (int i = 1; i <= pgc; i++)
            {
                pdfListener.PageIndex = i;
                // itextsharp send response to listener
                pdfParser.ProcessContent(i, pdfListener);

                DataAccess.Instance.g_curProgress += CurOneStep;
                evnt_UpdateCurBar();
            }

            imagesList = pdfListener.ImagesList; 
            details.ImagesBeforeMerge = pdfListener.ImagesList.Count;
            details.ImagesAfterMerge = details.ImagesBeforeMerge;

            if (checkResult && pdfReader.NumberOfPages != details.ImagesBeforeMerge)
            {
                if (joinImages)
                {
                    ImageJoiner cp = new ImageJoiner();
                    imagesList = cp.Merge(pdfListener.ImagesList, dirForExtractions);
                }

                details.ImagesAfterMerge = imagesList.Count;

                if(pdfReader.NumberOfPages != imagesList.Count)
                {                    
                    //Directory.Delete(dirForExtractions, true);
                    //throw new Exception(string.Format("Error extracting {0} : {1} images for {2} pages", Path.GetFileName(filename), pdfListener.ImagesList.Count, pdfReader.NumberOfPages));
                }
            }

            if (pdfReader != null)
                pdfReader.Close();

            return details;
        }
    }
}
