using System.Collections.ObjectModel;

namespace CutImageFromVideo {
    public class SettingData {
        public ObservableCollection<string> VideoFileNames { get; private set; }

        public SettingData() {
            VideoFileNames = new ObservableCollection<string>();
        }
    }
}