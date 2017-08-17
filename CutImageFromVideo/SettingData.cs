using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using CutImageFromVideo.Annotations;

namespace CutImageFromVideo {
    public class SettingData : INotifyPropertyChanged, INotifyDataErrorInfo {
        /**
         * INotifyPropertyChanged
        */
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>([NotNull] ref T field, [NotNull] T value,
                                    [CallerMemberName] string propertyName = null) {
            //if (field == null) throw new ArgumentNullException(nameof(field));

            field = value;
            var h = PropertyChanged;
            h?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /**
         * INotifyErrorsInfo
         */
        private Dictionary<string, IEnumerable> errors = new Dictionary<string, IEnumerable>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void OnErrorsChanged([CallerMemberName] string propertyName = null) {
            var h = ErrorsChanged;
            h?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName) {
            IEnumerable error;
            errors.TryGetValue(propertyName, out error);
            return error;
        }

        public bool HasErrors {
            get { return errors.Values.Any(e => e != null); }
        }

        /**
         * Fields
         */
        public ObservableCollection<string> VideoFileNames { get; set; }

        private string _cascadeFileName;

        public string CascadeFileName {
            get { return _cascadeFileName; }
            set {
                SetProperty(ref _cascadeFileName, value);
                if (string.IsNullOrEmpty(value)) {
                    errors["CascadeFileName"] = new[] {"Please enter a cascade file path"};
                }
                else if (!value.Contains(".xml")) {
                    errors["CascadeFileName"] = new[] {"The file is not a cascade file"};
                }
                else if (!File.Exists(value)) {
                    errors["CascadeFileName"] = new[] {"The file does not exist"};
                }
                else {
                    errors["CascadeFileName"] = null;
                }
                OnErrorsChanged();
            }
        }

        private string _outputDirectryName;

        public string OutputDirectryName {
            get { return _outputDirectryName; }
            set {
                SetProperty(ref _outputDirectryName, value);
                if (string.IsNullOrEmpty(value)) {
                    errors["OutputDirectryName"] = new[] {"Please enter a directry path"};
                }
                else if (!Directory.Exists(value)) {
                    errors["OutputDirectryName"] = new[] {"The directry does not exist"};
                }
                else {
                    errors["OutputDirectryName"] = null;
                }
                OnErrorsChanged();
            }
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

        private string _imageName;

        public string ImageName {
            get { return _imageName; }
            set {
                SetProperty(ref _imageName, value);
                if (string.IsNullOrEmpty(value)) {
                    errors["ImageName"] = new[] {"Please enter a image name"};
                }
                else {
                    errors["ImageName"] = null;
                }
                OnErrorsChanged();
            }
        }

        private bool _enableZeroFill;

        public bool EnableZeroFill {
            get { return _enableZeroFill; }
            set {
                SetProperty(ref _enableZeroFill, value);
                if (!_enableZeroFill) {
                    SetProperty(ref _zeroNum, 0);
                }
            }
        }

        private int _zeroNum;

        public int ZeroNum {
            get { return _zeroNum; }
            set { SetProperty(ref _zeroNum, value); }
        }

        private string _imageExtention;

        public string ImageExtention {
            get { return _imageExtention; }
            set { SetProperty(ref _imageExtention, value); }
        }

        public SettingData() {
            VideoFileNames = new ObservableCollection<string>();
        }
    }
}