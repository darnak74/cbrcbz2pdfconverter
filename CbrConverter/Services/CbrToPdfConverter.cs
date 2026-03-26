using CbrConverter.Models;

namespace CbrConverter.Services
{
    public class CbrToPdfConverter
    {
        public ConversionResult Convert(ConversionJob job)
        {
            return new ConversionResult { Success = true };
        }
    }
}
