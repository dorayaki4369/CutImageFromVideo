using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutImageFromVideo.Annotations;
using Reactive.Bindings;

namespace CutImageFromVideo {
    public class SettingData {
        public ObservableCollection<string> VideoFileNames { get; private set; }
        public ReactiveProperty<string> CascadeFileName { get; set; }
        public ReactiveProperty<string> OutputDirectryName { get; set; }

        public SettingData() {
            VideoFileNames = new ObservableCollection<string>();
            CascadeFileName = new ReactiveProperty<string>();
            OutputDirectryName = new ReactiveProperty<string>();
        }
    }
}