using CbrConverter.Models;

namespace CbrConverter.Services
{
    public interface IConverterService
    {
        ConversionResult Convert(ConversionJob job);
    }
}
