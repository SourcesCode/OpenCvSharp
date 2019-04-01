using OpenCvSharp;
using OpenCvSharp.Face;
using System.Collections.Generic;
using System.IO;

namespace OpenCvSharp.Core
{
    /// <summary>
    /// 特别需要强调的是，EigenFace和FisherFace的训练图像和测试图像都必须是灰度图，而且是经过归一化裁剪过的。
    /// </summary>
    public class FaceHelper
    {
        /// <summary>
        /// 对原图归一化
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        void Normal(Mat src, Mat dst)
        {
            if (src.Channels() == 1)//若原图单通道
                Cv2.Normalize(src, dst, 0, 255, NormTypes.MinMax, 1);//CV_8UC1
            else //否则，原图三通道
                Cv2.Normalize(src, dst, 0, 255, NormTypes.MinMax, 3);//CV_8UC3

        }

        //定义保存图片和标签的向量容器
        private List<Mat> images = new List<Mat>();
        private List<int> labels = new List<int>();
        /// <summary>
        /// 测试样本图片
        /// </summary>
        private List<Mat> imagesOfPredictSample = new List<Mat>();
        /// <summary>
        /// 测试样本标签
        /// </summary>
        private List<int> labelsOfPredictSample = new List<int>();
        /// <summary>
        /// 选择人脸数据库
        /// </summary>
        /// <param name="datasetChoose"></param>
        public void SelectFaceDatabase(int datasetChoose)
        {
            images.Clear();
            labels.Clear();
            imagesOfPredictSample.Clear();
            if (datasetChoose == 0)
            {
                //40个人,没人10张图片,一共400张
                string basePath = Path.Combine("FaceDatabase", "att_faces");
                //导入图片，每组前9张作为训练集，最后一张用来测试
                for (int i = 1; i <= 40; i++)
                {
                    for (int j = 1; j <= 10; j++)
                    {
                        string fileName = Path.Combine(basePath, $"s{i}", $"{j}.bmp");
                        Mat tempImage = Cv2.ImRead(fileName, ImreadModes.Grayscale);
                        if (j % 10 != 0)
                        {
                            images.Add(tempImage);
                            labels.Add(i);
                        }
                        else
                        {
                            imagesOfPredictSample.Add(tempImage);
                            labelsOfPredictSample.Add(i);
                        }
                    }
                }
            }
            else if (datasetChoose == 1)
            {
                //15个人,没人11张图片,一共165张
                string basePath = Path.Combine("FaceDatabase", "yale_faces");
                //将文件名编号放入string数组，从1开始
                for (int i = 1; i <= 165; i++)
                {
                    string fileName = Path.Combine(basePath, $"s{i}.bmp");
                    // load images
                    //将每组人脸的前十张用于训练，最后一张用于测试
                    Mat tempImage = Cv2.ImRead(fileName, ImreadModes.Grayscale);
                    if (i % 11 != 0)
                    {
                        images.Add(tempImage);
                        labels.Add(i / 11 + 1);
                    }
                    else
                    {
                        imagesOfPredictSample.Add(tempImage);
                        labelsOfPredictSample.Add(i / 11);
                    }
                }

            }
        }

        /// <summary>
        /// 1 EigenFace;
        /// 2 Fisher;
        /// 3 LBPH;
        /// </summary>
        /// <param name="modelChoose"></param>
        /// <returns></returns>
        public FaceRecognizer SelectFaceRecognizer(int modelChoose)
        {
            FaceRecognizer model;
            switch (modelChoose)
            {
                case 1: model = EigenFaceRecognizer.Create(); break;//特征脸EigenFace
                case 2: model = FisherFaceRecognizer.Create(); break;
                case 3: model = LBPHFaceRecognizer.Create(); break;
                default: model = LBPHFaceRecognizer.Create(); break;
            }
            return model;
        }

        /// <summary>
        /// 特征脸EigenFace
        /// </summary>
        public void Predict(int databaseId = 0, int faceRecognizerModel = 1)
        {
            //读取样本
            SelectFaceDatabase(databaseId);
            //图像大小归一化,默认是处理好的图片,所以这里就不需要处理了
            //Cv2.Resize(src_1, src_1, new Size(128, 128));
            FaceRecognizer model = SelectFaceRecognizer(faceRecognizerModel);
            //训练
            model.Train(images, labels);
            //保存训练的分类器
            //model.Save("faceClass.xml");
            //加载分类器
            //model.Read("faceClass.xml");

            List<int> predictResult = new List<int>();
            foreach (Mat item in imagesOfPredictSample)
            {
                int predict = model.Predict(item);
                predictResult.Add(predict);
            }

        }

        public void Train(FaceRecognizer model, List<PeopleSampleModel> samples)
        {
            //List<Mat> images = new List<Mat>();
            //List<int> labels = new List<int>();
            images.Clear();
            labels.Clear();

            foreach (var item in samples)
            {
                Cv2.Resize(item.Image, item.Image, new Size(128, 128));
                images.Add(item.Image);
                labels.Add(item.Label);
            }
            //训练
            model.Train(images, labels);
        }



    }
}
