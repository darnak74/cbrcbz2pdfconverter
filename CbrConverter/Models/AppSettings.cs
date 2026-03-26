using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace CbrConverter.Models
{
    [DataContract]
    public class AppSettings
    {
        [DataMember]
        public bool MergeImagesByDefault { get; set; }

        [DataMember]
        public bool CompressImagesByDefault { get; set; }

        [DataMember]
        public bool RecursiveByDefault { get; set; }

        public AppSettings()
        {
            MergeImagesByDefault = true;
            CompressImagesByDefault = false;
            RecursiveByDefault = true;
        }

        public static AppSettings Load(string filePath)
        {
            if (!File.Exists(filePath))
                return new AppSettings();

            var serializer = new DataContractJsonSerializer(typeof(AppSettings));
            using (var stream = File.OpenRead(filePath))
            {
                return (AppSettings)serializer.ReadObject(stream);
            }
        }

        public void Save(string filePath)
        {
            var serializer = new DataContractJsonSerializer(typeof(AppSettings));
            using (var stream = File.Create(filePath))
            {
                serializer.WriteObject(stream, this);
            }
        }
    }
}
