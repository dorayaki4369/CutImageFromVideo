using System.Collections.ObjectModel;

namespace CutImageFromMovie {
    public class MovieList {
        public ObservableCollection<string> FileNames { get; private set; }

        public MovieList() {
            FileNames = new ObservableCollection<string>();
        }
    }
}