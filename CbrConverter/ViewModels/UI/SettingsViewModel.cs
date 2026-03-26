using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CbrConverter.ViewModels.UI
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool _mergeImages;
        private bool _compressImages;
        private bool _recursive;

        public SettingsViewModel()
        {
            _mergeImages = true;
            _recursive = true;
        }

        public bool MergeImages
        {
            get { return _mergeImages; }
            set { _mergeImages = value; OnPropertyChanged(); }
        }

        public bool CompressImages
        {
            get { return _compressImages; }
            set { _compressImages = value; OnPropertyChanged(); }
        }

        public bool Recursive
        {
            get { return _recursive; }
            set { _recursive = value; OnPropertyChanged(); }
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
