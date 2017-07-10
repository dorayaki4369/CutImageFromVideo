using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;

namespace CutImageFromVideo {
    public class Detector {
        private SettingData SettingData { get; }
        private CascadeClassifier Cascade { get; }
        private string Outputfile { get; }
        private int Framenum { get; set; }
        private int Imgnum { get; set; }
        private bool IsExitStatus { get; set; }

        public Detector(SettingData settingData) {
            SettingData = settingData;
            Cascade = new CascadeClassifier(SettingData.CascadeFileName.Value);
            Outputfile = new StringBuilder(settingData.OutputDirectryName.Value).Append("\\").ToString();
            Framenum = 0;
            Imgnum = 0;
            IsExitStatus = false;
        }

        public void Stop() {
            IsExitStatus = true;
        }

        public void Run() {
            Console.WriteLine(@"Creating face image...");

            Cv2.NamedWindow("image", WindowMode.FreeRatio);

            foreach (var videoName in SettingData.VideoFileNames) {
                using (var video = VideoCapture.FromFile(videoName)) {
                    while (true) {
                        using (var frame = video.RetrieveMat()) {
                            Framenum++;

                            if (frame.Empty() || IsExitStatus) {
                                Console.WriteLine(@"Cancel");
                                frame.Dispose();
                                IsExitStatus = false;
                                break;
                            }

                            //Detecting every 10 frames because the number of images
                            //increases too much when cutting out all frames
                            if (Framenum % 10 == 0) DetectAndSaveImg(frame);

                            frame.Dispose();
                        }
                    }
                    Console.WriteLine(@"End of video");
                }
            }

            Cv2.DestroyAllWindows();
            Cascade.Dispose();
        }

        //Face detection
        private void DetectAndSaveImg(Mat image) {
            //Image to gray scale
            var grayImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            //Flattening the histogram
            Cv2.EqualizeHist(grayImage, grayImage);

            //Face recognition, Small faces excluded
            var mats = Cascade.DetectMultiScale(grayImage, 1.1, 3, 0, new Size(80, 80))
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
                var sb = new StringBuilder(Outputfile)
                    .Append("image")
                    .AppendFormat("{0:D5}", Imgnum)
                    .Append(".png");
                Imgnum++;

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