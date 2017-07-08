using System.Collections.ObjectModel;

namespace CutImageFromMovie {
    public class SettingData {
        public ObservableCollection<string> MovieFileNames { get; private set; }

        public SettingData() {
            MovieFileNames = new ObservableCollection<string>();
        }
    }
}