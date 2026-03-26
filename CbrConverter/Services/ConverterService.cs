using CbrConverter.Models;

namespace CbrConverter.Services
{
    public class ConverterService : IConverterService
    {
        private readonly CbrToPdfConverter _cbrToPdfConverter = new CbrToPdfConverter();

        public ConversionResult Convert(ConversionJob job)
        {
            return _cbrToPdfConverter.Convert(job);
        }
    }
}
