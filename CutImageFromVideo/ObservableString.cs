using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutImageFromVideo.Annotations;

namespace CutImageFromVideo {
    public class ObservableString : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public string String {
            get { return String; }
            set {
                if (value == null) throw new ArgumentNullException(nameof(value));
                String = value;
                OnPropertyChanged(String);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(String));
        }
    }
}