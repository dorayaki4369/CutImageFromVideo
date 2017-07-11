using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutImageFromVideo.Annotations;

namespace CutImageFromVideo {
    public class SettingData : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>([NotNull] ref T field, [NotNull] T value,
                                    [CallerMemberName] string propertyName = null) {
            //if (field == null) throw new ArgumentNullException(nameof(field));

            field = value;
            var h = PropertyChanged;
            h?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<string> VideoFileNames { get; private set; }

        private string _cascadeFileName;

        public string CascadeFileName {
            get { return _cascadeFileName; }
            set { SetProperty(ref _cascadeFileName, value); }
        }

        private string _outputDirectryName;

        public string OutputDirectryName {
            get { return _outputDirectryName; }
            set { SetProperty(ref _outputDirectryName, value); }
        }

        private int _totalFrameNum;

        public int TotalFrameNum {
            get { return _totalFrameNum; }
            set { SetProperty(ref _totalFrameNum, value); }
        }

        private int _currentFrameNum;

        public int CurrentFrameNum {
            get { return _currentFrameNum; }
            set { SetProperty(ref _currentFrameNum, value); }
        }

        public SettingData() {
            VideoFileNames = new ObservableCollection<string>();
        }
    }
}