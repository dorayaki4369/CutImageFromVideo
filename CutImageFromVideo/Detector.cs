using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;

namespace CutImageFromVideo {
    public class Detector {
        private readonly SettingData settingData;
        private int _framenum = 0;
        private int _imgnum = 0;

        public Detector(SettingData settingData) {
            this.settingData = settingData;
        }

        public void Run() {
            Console.WriteLine(@"Creating face image...");

            foreach (var videoName in settingData.VideoFileNames) {
                using (var video = VideoCapture.FromFile(videoName)) {
                    while (true) {
                        using (var frame = video.RetrieveMat()) {
                            _framenum++;
                            if (frame.Empty()) {
                                Console.WriteLine(@"End of video");
                                break;
                            }

                            //Detecting every 10 frames because the number of images
                            //increases too much when cutting out all frames
                            if (_framenum % 10 == 0) DetectAndSaveImg(frame);
                        }
                    }
                }
            }
        }

        //Face detection
        private void DetectAndSaveImg(Mat image) {
            //Image to gray scale
            var grayImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            //Flattening the histogram
            Cv2.EqualizeHist(grayImage, grayImage);

            //Face recognition, Small faces excluded
            using (var cascade = new CascadeClassifier(settingData.CascadeFileName.Value)) {
                var mats = cascade.DetectMultiScale(grayImage, 1.1, 3, 0, new Size(80, 80))
                    .Select(rect => new Rect(rect.X, rect.Y, rect.Width,
                        rect.Height)) //Make rects focusing on facial parts
                    .Select(image.Clone) //Imaged cut out
                    .ToList(); //Listing

                SaveImg(mats);
            }
        }

        //Save image
        private void SaveImg(IEnumerable<Mat> mats) {
            Cv2.NamedWindow("image", WindowMode.FreeRatio);

            var sb = new StringBuilder();
            sb.Append(settingData.OutputDirectryName.Value).Append("\\")
                .Append("image");
            foreach (var mat in mats) {
                sb.AppendFormat("{0:D5}", _imgnum)
                    .Append(".png");

                //Save
                Cv2.ImWrite(sb.ToString(), mat);
                _imgnum++;

                //Show cuted image
                Cv2.ImShow("image", mat);
                Cv2.WaitKey(1);
            }
        }
    }
}