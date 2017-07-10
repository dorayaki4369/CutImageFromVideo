using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;

namespace CutImageFromVideo {
    public class Detector {
        private readonly SettingData _settingData;
        private readonly CascadeClassifier _cascade;
        private readonly string _outputfile;
        private int _framenum;
        private int _imgnum;
        private bool _isExitStatus;

        public Detector(SettingData settingData) {
            _settingData = settingData;
            _cascade = new CascadeClassifier(_settingData.CascadeFileName.Value);
            _outputfile = new StringBuilder(settingData.OutputDirectryName.Value).Append("\\").ToString();
            _framenum = 0;
            _imgnum = 0;
            _isExitStatus = false;
        }

        public void Stop() {
            _isExitStatus = true;
        }

        public void Run() {
            Console.WriteLine(@"Creating face image...");

            Cv2.NamedWindow("image", WindowMode.FreeRatio);

            foreach (var videoName in _settingData.VideoFileNames) {
                using (var video = VideoCapture.FromFile(videoName)) {
                    while (true) {
                        using (var frame = video.RetrieveMat()) {
                            _framenum++;

                            if (frame.Empty() || _isExitStatus) {
                                Console.WriteLine(@"Cancel");
                                frame.Dispose();
                                _isExitStatus = false;
                                break;
                            }

                            //Detecting every 10 frames because the number of images
                            //increases too much when cutting out all frames
                            if (_framenum % 10 == 0) DetectAndSaveImg(frame);

                            frame.Dispose();
                        }
                    }
                    Console.WriteLine(@"End of video");
                }
            }

            Cv2.DestroyAllWindows();
            _cascade.Dispose();
        }

        //Face detection
        private void DetectAndSaveImg(Mat image) {
            //Image to gray scale
            var grayImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            //Flattening the histogram
            Cv2.EqualizeHist(grayImage, grayImage);

            //Face recognition, Small faces excluded
            var mats = _cascade.DetectMultiScale(grayImage, 1.1, 3, 0, new Size(80, 80))
                //Make rects focusing on facial parts
                .Select(rect => new Rect(rect.X, rect.Y, rect.Width, rect.Height))
                //Imaged cut out
                .Select(image.Clone)
                //Listing
                .ToList();

            SaveImg(mats);

            grayImage.Dispose();
            mats.Clear();
        }

        //Save image
        private void SaveImg(IEnumerable<Mat> mats) {
            foreach (var mat in mats) {
                var sb = new StringBuilder(_outputfile)
                    .Append("image")
                    .AppendFormat("{0:D5}", _imgnum)
                    .Append(".png");
                _imgnum++;

                //Save
                Cv2.ImWrite(sb.ToString(), mat);

                //Show cuted image
                Cv2.ImShow("image", mat);
                Cv2.WaitKey(1);

                mat.Dispose();
            }
        }
    }
}