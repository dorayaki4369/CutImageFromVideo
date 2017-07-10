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
        private int _framenum = 0;
        private int _imgnum = 0;

        public Detector(SettingData settingData) {
            _settingData = settingData;
            _cascade = new CascadeClassifier(_settingData.CascadeFileName.Value);
            _outputfile = new StringBuilder(settingData.OutputDirectryName.Value).Append("\\").ToString();
        }

        public void Run() {
            Console.WriteLine(@"Creating face image...");

            Cv2.NamedWindow("image", WindowMode.FreeRatio);

            foreach (var videoName in _settingData.VideoFileNames) {
                using (var video = VideoCapture.FromFile(videoName)) {
                    while (true) {
                        using (var frame = video.RetrieveMat()) {
                            _framenum++;

                            if (frame.Empty()) break;

                            //Detecting every 10 frames because the number of images
                            //increases too much when cutting out all frames
                            if (_framenum % 10 == 0) DetectAndSaveImg(frame);
                        }
                    }
                    Console.WriteLine(@"End of video");
                }
            }

            Cv2.DestroyAllWindows();
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