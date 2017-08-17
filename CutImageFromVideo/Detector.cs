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
        private string ZeroFillFormat { get; }
        private int ImgNum { get; set; }
        private bool IsExitStatus { get; set; }

        public Detector(SettingData settingData) {
            SettingData = settingData;
            Cascade = new CascadeClassifier(SettingData.CascadeFileName);
            Outputfile = new StringBuilder(settingData.OutputDirectryName).Append("\\").ToString();
            ImgNum = 0;
            ZeroFillFormat = new StringBuilder("{0:D").Append(settingData.ZeroNum).Append("}").ToString();
            IsExitStatus = false;
        }

        public void Stop() {
            IsExitStatus = true;
        }

        public void Run() {
            Console.WriteLine(@"Creating face image...");
            SettingData.TotalFrameNum = 0;
            SettingData.CurrentFrameNum = 0;

            foreach (var videoName in SettingData.VideoFileNames) {
                using (var video = new VideoCapture(videoName)) {
                    SettingData.TotalFrameNum += video.FrameCount;
                }
            }

            Cv2.NamedWindow("image", WindowMode.FreeRatio);

            foreach (var videoName in SettingData.VideoFileNames) {
                using (var video = VideoCapture.FromFile(videoName)) {
                    var frameNum = 0;
                    while (true) {
                        using (var frame = video.RetrieveMat()) {
                            if (frame.Empty() || IsExitStatus) {
                                if (IsExitStatus) {
                                    Console.WriteLine(@"Cancel");
                                    IsExitStatus = false;
                                    goto CANCEL;
                                }
                                break;
                            }

                            frameNum++;
                            SettingData.CurrentFrameNum++;

                            //Detecting every 10 frames because the number of images
                            //increases too much when cutting out all frames
                            if (frameNum % SettingData.FrameRateNum == 0) DetectAndSaveImg(frame);
                        }
                    }
                    Console.WriteLine(@"End of video");
                }
            }
            CANCEL:

            Cv2.DestroyAllWindows();
            Cascade.Dispose();
        }

        //Face detection
        private void DetectAndSaveImg(Mat image) {
            //Image to gray scale
            using (var grayImage = new Mat()) {
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
            }
        }

        //Save image
        private void SaveImg(IEnumerable<Mat> mats) {
            foreach (var mat in mats) {
                var imgName = new StringBuilder(Outputfile)
                    .Append(SettingData.ImageName)
                    .AppendFormat(ZeroFillFormat, ImgNum)
                    .Append(SettingData.ImageExtention)
                    .ToString();
                ImgNum++;

                //Save
                Cv2.ImWrite(imgName, mat);

                //Show cuted image
                Cv2.ImShow("image", mat);
                Cv2.WaitKey(1);

                mat.Dispose();
            }
        }
    }
}