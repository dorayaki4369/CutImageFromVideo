using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace CutImageFromVideo {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        /**
         * Video : Drag&Drop
         */
        private void VideoList_Drop(object sender, DragEventArgs e) {
            var list = DataContext as SettingData;
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files == null) return;
            foreach (var s in files) {
                list?.VideoFileNames.Add(s);
            }

            VideoScrollViewer.ScrollToRightEnd();
        }

        private void VideoList_PreviewDragOver(object sender, DragEventArgs e) {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop, true)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
            e.Handled = true;
        }

        /**
         * Video : Browse
         */
        private void VideoBrowseButton_Click(object sender, RoutedEventArgs e) {
            var ofd = new OpenFileDialog {
                Title = "Select video file",
                FileName = "*.avi",
                Filter = VideoContainerFilter(),
                DefaultExt = "*.*"
            };

            var list = DataContext as SettingData;
            if (ofd.ShowDialog() == true) {
                list?.VideoFileNames.Add(ofd.FileName);
            }

            VideoScrollViewer.ScrollToRightEnd();
        }

        //Create video container format
        private static string VideoContainerFilter() {
            var filter = new StringBuilder()
                .Append("AVI|*.avi").Append("|")
                .Append("MP4|*.mp4; *.m4a").Append("|")
                .Append("MOV|*.mov; .qt").Append("|")
                .Append("MPEG2-TS|*.m2ts; *.ts").Append("|")
                .Append("MPEG2-PS|*.mpeg; *.mpg").Append("|")
                .Append("MKV|*.mkv").Append("|")
                .Append("WMV|*.wmv").Append("|")
                .Append("FLV|*.flv").Append("|")
                .Append("ASF|*.asf").Append("|")
                .Append("VOB|*.wmv").Append("|")
                .Append("WebM|*.webm").Append("|")
                .Append("OGM|*.ogm").Append("|")
                .Append("All files|*.*");

            return filter.ToString();
        }

        /**
         * Video : SelectAll
         */
        private void VideoSelectAllButton_Click(object sender, RoutedEventArgs e) {
            VideoList.SelectAll();
        }

        /**
         * Video : Delete
         */
        private void VideoDeleteButton_Click(object sender, RoutedEventArgs e) {
            //var deleteList = new string[20];
            //VideoList.SelectedItems.CopyTo(deleteList, 0);
            //var list = DataContext as SettingData;
            //if (list == null) return;
            //foreach (var item in list.VideoFileNames) {
            //    foreach (var element in deleteList) {
            //        if (element == null) return;
            //        if (element.Equals(item)) {
            //            list.VideoFileNames.Remove(item);
            //        }
            //    }
            //}

            var list = DataContext as SettingData;
            list?.VideoFileNames.RemoveAt(VideoList.SelectedIndex);
        }
    }
}