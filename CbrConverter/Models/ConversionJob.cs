namespace CbrConverter.Models
{
    public class ConversionJob
    {
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public bool MergeImages { get; set; }
        public bool CompressImages { get; set; }
        public bool Recursive { get; set; }
    }
}
