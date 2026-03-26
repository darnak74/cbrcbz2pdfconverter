namespace CbrConverter.Models
{
    public class ConversionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int ProcessedItems { get; set; }
    }
}
