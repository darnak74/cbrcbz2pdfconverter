using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CbrConverter.ViewModels.UI
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        private string _repositoryUrl;

        public AboutViewModel()
        {
            _repositoryUrl = "https://github.com/darnak74/cbrcbz2pdfconverter";
        }

        public string RepositoryUrl
        {
            get { return _repositoryUrl; }
            set { _repositoryUrl = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
