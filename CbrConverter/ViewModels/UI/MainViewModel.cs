using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CbrConverter.ViewModels.UI
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _sourcePath;
        private string _destinationPath;
        private double _fileProgress;
        private double _totalProgress;
        private string _status;

        public MainViewModel()
        {
            _status = "Pret";
        }

        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; OnPropertyChanged(); }
        }

        public string DestinationPath
        {
            get { return _destinationPath; }
            set { _destinationPath = value; OnPropertyChanged(); }
        }

        public double FileProgress
        {
            get { return _fileProgress; }
            set { _fileProgress = value; OnPropertyChanged(); }
        }

        public double TotalProgress
        {
            get { return _totalProgress; }
            set { _totalProgress = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(); }
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
