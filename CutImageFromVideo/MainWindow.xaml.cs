/**
 * Copyright (c) 2017 Ryuya Hirayama All rights reserved.
 * 
 * The source is:
 * 
 * Copyright (c) 2016 MahApps
 * https://github.com/MahApps/MahApps.Metro/blob/master/LICENSE
 * 
 * Copyright (c) James Willock,  Mulholland Software and Contributors
 * https://github.com/ButchersBoy/MaterialDesignInXamlToolkit/blob/master/LICENSE
 * 
 * Copyright (c) 2017, shimat All rights reserved.
 * https://github.com/shimat/opencvsharp/blob/master/LICENSE
 */

using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using MahApps.Metro.Controls.Dialogs;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Forms = System.Windows.Forms;

namespace CutImageFromVideo {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();

            ExtensionCombo.ItemsSource = new[] {
                ".bmp", ".dib",
                ".jpeg", ".jpg", ".jpe",
                ".jp2",
                ".png",
                ".sr", ".ras",
                ".tiff", ".tif",
                ".exr",
                ".webp",
                ".pbm", ".pgm", ".ppm"
            };

            ProgressBar.Maximum = 1;
            ProgressBar.Value = 0;
            ImageName.Text = "image";
            ExtensionCombo.SelectedIndex = 6;//.png
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

            CheckStartButtonEnabled();
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

            CheckStartButtonEnabled();
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

            CheckStartButtonEnabled();
        }

        /**
         * Video : DeleteAll
         */
        private void VideoDeleteAllButton_Click(object sender, RoutedEventArgs e) {
            var list = DataContext as SettingData;
            list?.VideoFileNames.Clear();

            StartButton.IsEnabled = false;
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
            if (list != null) list.CascadeFileName = ofd.FileName;
            CascadeBox.Text = ofd.FileName;

            //Right justified
            CascadeBox.CaretIndex = CascadeBox.Text.Length;
            var rect = CascadeBox.GetRectFromCharacterIndex(CascadeBox.CaretIndex);
            CascadeBox.ScrollToHorizontalOffset(rect.Right);

            CheckStartButtonEnabled();
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
            if (list != null) list.OutputDirectryName = dialog.SelectedPath;
            DirectryBox.Text = dialog.SelectedPath;

            //Right justified
            DirectryBox.CaretIndex = DirectryBox.Text.Length;
            var rect = DirectryBox.GetRectFromCharacterIndex(DirectryBox.CaretIndex);
            DirectryBox.ScrollToHorizontalOffset(rect.Right);

            CheckStartButtonEnabled();
        }

        /**
         * StartButton : Check
         */
        private void CheckStartButtonEnabled() {
            var list = DataContext as SettingData;
            if (list != null
                && list.VideoFileNames.Count != 0
                && list.CascadeFileName != null
                && list.OutputDirectryName != null) {
                StartButton.IsEnabled = true;
            }
            else {
                StartButton.IsEnabled = false;
            }
        }

        private Detector _detector;

        /**
         * StartButton : Click
         */
        private async void StartButton_Click(object sender, RoutedEventArgs e) {
            ChangeEnabledInButtons(false);

            _detector = new Detector(DataContext as SettingData);
            _detector.Run();
            var list = DataContext as SettingData;
            await this.ShowMessageAsync("Finish!", GenDialogMessage());

            ChangeEnabledInButtons(true);
        }

        private void ChangeEnabledInButtons(bool b) {
            //button
            VideoBrowseButton.IsEnabled = b;
            VideoDeleteButton.IsEnabled = b;
            VideoBrowseButton.IsEnabled = b;
            VideoDeleteAllButton.IsEnabled = b;
            CascadeBrowseButton.IsEnabled = b;
            DirectryBrowseButton.IsEnabled = b;
            StartButton.IsEnabled = b;
            CancelButton.IsEnabled = !b; //Attention!

            //list and box
            VideoList.IsEnabled = b;
            CascadeBox.IsEnabled = b;
            DirectryBox.IsEnabled = b;
        }

        private string GenDialogMessage() {
            var list = DataContext as SettingData;
            return new StringBuilder()
                .Append("Total frame number   : ").Append(list?.TotalFrameNum.ToString()).Append("\n")
                .Append("Current frame number : ").Append(list?.CurrentFrameNum.ToString())
                .ToString();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            //As you can see from the Detector,
            //there is no need to do anything particularly
            //because button activation is done after this.
            _detector.Stop();
        }
    }
}