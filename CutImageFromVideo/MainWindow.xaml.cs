﻿using System.Text;
using System.Windows;
using System.Windows.Forms;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

using Forms = System.Windows.Forms;

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
         * Video : Delete
         */
        private void VideoDeleteButton_Click(object sender, RoutedEventArgs e) {
            var list = DataContext as SettingData;
            if (VideoList.SelectedIndex == -1) return;
            list?.VideoFileNames?.RemoveAt(VideoList.SelectedIndex);
        }

        /**
         * Video : DeleteAll
         */
        private void VideoDeleteAllButton_Click(object sender, RoutedEventArgs e) {
            var list = DataContext as SettingData;
            list?.VideoFileNames.Clear();
        }

        /**
         * Cascade : Browse
         */
        private void CascadeBrowseButton_Click(object sender, RoutedEventArgs e) {
            var ofd = new OpenFileDialog {
                Title = "Select Cascade file",
                FileName = "*.xml",
                Filter = "Cascade file|*.xml",
                DefaultExt = "*.*"
            };

            var list = DataContext as SettingData;
            if (ofd.ShowDialog() != true) return;
            if (list != null) list.CascadeFileName.Value = ofd.FileName;
            CascadeBox.Text = ofd.FileName;

            //Right justified
            CascadeBox.CaretIndex = CascadeBox.Text.Length;
            var rect = CascadeBox.GetRectFromCharacterIndex(CascadeBox.CaretIndex);
            CascadeBox.ScrollToHorizontalOffset(rect.Right);
        }

        /**
         * OutputDirectry : Browse
         */
        private void DirectryBrowseButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new FolderBrowserDialog {
                Description = @"Select Oputput directry"
            };

            if (dialog.ShowDialog() != Forms.DialogResult.OK) return;
            var list = DataContext as SettingData;
            if (list != null) list.OutputDirectryName.Value = dialog.SelectedPath;
            DirectryBox.Text = dialog.SelectedPath;

            //Right justified
            DirectryBox.CaretIndex = DirectryBox.Text.Length;
            var rect = DirectryBox.GetRectFromCharacterIndex(DirectryBox.CaretIndex);
            DirectryBox.ScrollToHorizontalOffset(rect.Right);
        }
    }
}