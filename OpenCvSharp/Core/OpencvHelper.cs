using OpenCvSharp.Face;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenCvSharp.Core
{
    public class OpencvHelper
    {
        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="fileName"></param>
        public static void DetectMultiScale(string fileName)
        {
            //
            //
            //String face_cascade_name = "face.xml";
            String face_cascade_name = "data/haarcascades/haarcascade_frontalface_alt.xml";
            //String eyes_cascade_name = "haarcascade_eye_tree_eyeglasses.xml";
            CascadeClassifier face_cascade = new CascadeClassifier();
            if (!face_cascade.Load(face_cascade_name))
            {
                //加载脸部分类器失败！
                return;
            }
            Mat img = Cv2.ImRead(fileName);
            Mat gray = new Mat();
            //Mat facesROI;
            //图像缩放，采用双线性插值。
            //Cv2.Resize(src, src, new Size(128, 128), 0, 0, InterpolationFlags.Linear);
            //图像灰度化。
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            //直方图均衡化，图像增强，暗的变亮，亮的变暗。
            Cv2.EqualizeHist(gray, gray);

            //存储找到的脸部矩形。
            // Rect型的容器，存放所有检测出的人脸，每个人脸是一个矩形
            Rect[] faces = face_cascade.DetectMultiScale(
                image: img,//要检测的图片，一般为灰度图
                scaleFactor: 1.1,//缩放因子，对图片进行缩放，默认为1.1
                minNeighbors: 3,//最小邻居数，默认为3
                flags: 0 | HaarDetectionType.ScaleImage,//兼容老版本的一个参数，在3.0版本中没用处。默认为0
                minSize: new Size(10, 10),//最小尺寸，检测出的人脸最小尺寸
                maxSize: new Size(100, 100)//最大尺寸，检测出的人脸最大尺寸
            );


            for (int i = 0; i < faces.Length; ++i)
            {
                //画出脸部矩形,绘制矩形 BGR。
                Cv2.Rectangle(img, faces[i], new Scalar(0, 0, 255), 2);
            }
            //Cv2.NamedWindow("faces", WindowMode.AutoSize);
            Cv2.ImShow("faces", img);
            //Esc
            //while (true)
            //{
            //    if (Cv2.WaitKey(1) == 27)
            //    {
            //        break;
            //    }
            //}
            Cv2.WaitKey();
            Cv2.DestroyWindow("faces");

        }

        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="dst"></param>
        public void FindFaces(ref Mat dst)
        {
            Mat src = Cv2.ImRead("src.jpg");
            //Mat src = Mat.FromStream(File.OpenRead("src.jpg"), ImreadModes.Unchanged);
            Mat frame = src.Clone();
            Mat facesROI;
            //图像缩放，采用双线性插值。
            //Cv2.Resize(src, src, new Size(128, 128), 0, 0, InterpolationFlags.Linear);
            //图像灰度化。
            Cv2.CvtColor(src, src, ColorConversionCodes.BGR2GRAY);
            //直方图均衡化，图像增强，暗的变亮，亮的变暗。
            Cv2.EqualizeHist(src, src);
            //
            String face_cascade_name = "haarcascade_frontalface_alt.xml";
            String eyes_cascade_name = "haarcascade_eye_tree_eyeglasses.xml";
            CascadeClassifier face_cascade = new CascadeClassifier();
            CascadeClassifier eyes_cascade = new CascadeClassifier();
            if (!face_cascade.Load(face_cascade_name))
            {
                //加载脸部分类器失败！
                return;
            }
            if (!eyes_cascade.Load(eyes_cascade_name))
            {
                //加载眼睛分类器失败！
                return;
            }
            //存储找到的脸部矩形。
            //VectorOfRect faces;
            // Rect型的容器，存放所有检测出的人脸，每个人脸是一个矩形
            Rect[] faces = face_cascade.DetectMultiScale(
                image: src,//要检测的图片，一般为灰度图
                scaleFactor: 1.1,//缩放因子，对图片进行缩放，默认为1.1
                minNeighbors: 3,//最小邻居数，默认为3
                flags: 0 | HaarDetectionType.ScaleImage,//兼容老版本的一个参数，在3.0版本中没用处。默认为0
                minSize: new Size(30, 30),//最小尺寸，检测出的人脸最小尺寸
                maxSize: new Size(100, 100)//最大尺寸，检测出的人脸最大尺寸
            );


            for (int i = 0; i < faces.Length; ++i)
            {
                //绘制矩形 BGR。
                Cv2.Rectangle(frame, faces[i], new Scalar(0, 0, 255), 1);

                //获取矩形中心点。
                //Point center( faces[i].x + faces[i].width/2, faces[i].y + faces[i].height/2 );
                //Point center = new Point(faces[i].X + faces[i].Width / 2, faces[i].Y + faces[i].Height / 2);
                //绘制圆形。
                //ellipse(frame, center, Size( faces[i].width/2, faces[i].height/2 ), 0, 0, 360, Scalar( 255, 0, 255 ), 4, 8, 0 );
                //Cv2.Ellipse(frame, center, new Size(faces[i].Width / 2, faces[i].Height / 2), 0, 0, 360, new Scalar(255, 0, 255), 4, LineTypes.Link8, 0);
                //获取脸部矩形区域。
                //Mat faceROI = src(faces[i]);
                //Mat faceROI = new Mat(src, faces[i]);
                //存储找到的眼睛矩形。
                //std::vector<Rect> eyes;
                //eyes_cascade.detectMultiScale(faceROI,eyes,1.1,2,0 |CASCADE_SCALE_IMAGE,Size(30,30));
                //Rect[] eyes = eyes_cascade.DetectMultiScale(faceROI, 1.1, 2, 0 | HaarDetectionType.ScaleImage, new Size(30, 30));
                //for (int j = 0; j < eyes.Length; ++j)
                //{
                //    Point eye_center = new Point(faces[i].X + eyes[j].X + eyes[j].Width / 2, faces[i].Y + eyes[j].Y + eyes[j].Height / 2);
                //    //半径
                //    int radius = cvRound((eyes[j].Width + eyes[j].Height) * 0.25);
                //    Cv2.Circle(frame, eye_center, radius, new Scalar(255, 0, 0), 4, LineTypes.Link8, 0);
                //}

                //截取人脸。
                //facesROI = frame(faces[i]);
                facesROI = new Mat(frame, faces[i]);

                //图像缩放。
                Cv2.Resize(facesROI, facesROI, new Size(128, 128), 0, 0, InterpolationFlags.Linear);
                //保存图像。
                dst = facesROI;
                Cv2.ImWrite("dst.jpg", facesROI);

            }

        }

        public static void OpencvCamera()
        {
            //获取视频流
            Cv2.NamedWindow("Win7x64", WindowMode.Normal);
            VideoCapture capture = new VideoCapture();
            Mat camera = new Mat();
            //采用 Directshow 的方式打开第一个摄像头设备。
            capture.Open(CaptureDevice.DShow, 0);
            if (!capture.IsOpened())
            {
                return;
            }
            //capture.set(CAP_PROP_SETTINGS,0);//调出 Directshow 摄像头属性设置栏
            while (true)
            {
                //读取一帧图像
                capture.Read(camera);
                if (camera.Empty())
                {
                    continue;
                }
                Cv2.ImShow("Win7x64", camera);
                //Esc
                if (Cv2.WaitKey(1) == 27)
                {
                    break;
                }
            }
            Cv2.DestroyWindow("Win7x64");

        }

        #region Recognizer
        /* 
         * 
         * 特别需要强调的是，EigenFace和FisherFace的训练图像和测试图像都必须是灰度图，而且是经过归一化裁剪过的。
         * 
         * 
         */

        /// <summary>
        /// 特征脸EigenFace
        /// </summary>
        public static void EigenFace()
        {
            //定义保存图片和标签的向量容器
            List<Mat> images = new List<Mat>();
            List<int> labels = new List<int>();
            //VectorOfMat images = new VectorOfMat();
            //VectorOfInt32 labels = new VectorOfInt32();
            //读取样本
            Mat src_1 = Cv2.ImRead("persons/person0/0.jpg", ImreadModes.Grayscale);
            Mat src_2 = Cv2.ImRead("persons/person0/1.jpg", ImreadModes.Grayscale);
            Mat src_3 = Cv2.ImRead("persons/person1/0.jpg", ImreadModes.Grayscale);
            Mat src_4 = Cv2.ImRead("persons/person1/1.jpg", ImreadModes.Grayscale);

            //图像大小归一化
            Cv2.Resize(src_1, src_1, new Size(128, 128));
            Cv2.Resize(src_2, src_2, new Size(128, 128));
            Cv2.Resize(src_3, src_3, new Size(128, 128));
            Cv2.Resize(src_4, src_4, new Size(128, 128));
            //加入图像
            images.Add(src_1);
            //加入标签
            labels.Add(0);
            images.Add(src_2);
            labels.Add(0);
            images.Add(src_3);
            labels.Add(1);
            images.Add(src_4);
            labels.Add(1);

            //特征脸EigenFace
            EigenFaceRecognizer faceClass = EigenFaceRecognizer.Create();

            //训练
            faceClass.Train(images, labels);
            //保存训练的分类器
            //faceClass.Save("faceClass.xml");
            //加载分类器
            //faceClass.Read("faceClass.xml");
            //fisherClass.Read("fisherClass.xml");
            //lpbhClass.Read("lpbhClass.xml");

            //预测样本                                       
            Mat src_5 = Cv2.ImRead("persons/person1/2.jpg", ImreadModes.Grayscale);
            Cv2.Resize(src_5, src_5, new Size(128, 128));

            //预测样本并获取标签和置信度
            //使用训练好的分类器进行预测。
            int faceResult = faceClass.Predict(src_5);
            switch (faceResult)
            {
                case 0:
                    //张三
                    break;
                case 1:
                    //李四
                    break;
                default:
                    //未知
                    break;
            }

            return;
        }
        /// <summary>
        /// Fisher脸FisherFace
        /// </summary>
        public static void FisherFace()
        {
            //定义保存图片和标签的向量容器
            List<Mat> images = new List<Mat>();
            List<int> labels = new List<int>();
            //VectorOfMat images = new VectorOfMat();
            //VectorOfInt32 labels = new VectorOfInt32();
            //读取样本
            Mat src_1 = Cv2.ImRead("persons/person0/0.jpg", ImreadModes.Grayscale);
            Mat src_2 = Cv2.ImRead("persons/person0/1.jpg", ImreadModes.Grayscale);
            Mat src_3 = Cv2.ImRead("persons/person1/0.jpg", ImreadModes.Grayscale);
            Mat src_4 = Cv2.ImRead("persons/person1/1.jpg", ImreadModes.Grayscale);

            //图像大小归一化
            Cv2.Resize(src_1, src_1, new Size(128, 128));
            Cv2.Resize(src_2, src_2, new Size(128, 128));
            Cv2.Resize(src_3, src_3, new Size(128, 128));
            Cv2.Resize(src_4, src_4, new Size(128, 128));
            //加入图像
            images.Add(src_1);
            //加入标签
            labels.Add(0);
            images.Add(src_2);
            labels.Add(0);
            images.Add(src_3);
            labels.Add(1);
            images.Add(src_4);
            labels.Add(1);

            //Fisher脸FisherFace
            FisherFaceRecognizer fisherClass = FisherFaceRecognizer.Create();

            //训练
            fisherClass.Train(images, labels);
            //保存训练的分类器
            //fisherClass.Save("fisherClass.xml");
            //加载分类器
            //faceClass.Read("faceClass.xml");
            //fisherClass.Read("fisherClass.xml");
            //lpbhClass.Read("lpbhClass.xml");

            //预测样本                                       
            Mat src_5 = Cv2.ImRead("persons/person1/2.jpg", ImreadModes.Grayscale);
            Cv2.Resize(src_5, src_5, new Size(128, 128));

            //预测样本并获取标签和置信度
            //标签
            int fisherResult = -1;
            //置信度
            double fisherConfidence = 0.0;
            fisherClass.Predict(src_5, out fisherResult, out fisherConfidence);

            return;
        }
        /// <summary>
        /// LBP直方图LBPHFace
        /// </summary>
        public static void LBPHFace()
        {
            //定义保存图片和标签的向量容器
            List<Mat> images = new List<Mat>();
            List<int> labels = new List<int>();
            //VectorOfMat images = new VectorOfMat();
            //VectorOfInt32 labels = new VectorOfInt32();
            //读取样本
            Mat src_1 = Cv2.ImRead("persons/person0/0.jpg", ImreadModes.Grayscale);
            Mat src_2 = Cv2.ImRead("persons/person0/1.jpg", ImreadModes.Grayscale);
            Mat src_3 = Cv2.ImRead("persons/person1/0.jpg", ImreadModes.Grayscale);
            Mat src_4 = Cv2.ImRead("persons/person1/1.jpg", ImreadModes.Grayscale);

            //图像大小归一化
            Cv2.Resize(src_1, src_1, new Size(128, 128));
            Cv2.Resize(src_2, src_2, new Size(128, 128));
            Cv2.Resize(src_3, src_3, new Size(128, 128));
            Cv2.Resize(src_4, src_4, new Size(128, 128));
            //加入图像
            images.Add(src_1);
            //加入标签
            labels.Add(0);
            images.Add(src_2);
            labels.Add(0);
            images.Add(src_3);
            labels.Add(1);
            images.Add(src_4);
            labels.Add(1);

            //LBP直方图LBPHFace
            LBPHFaceRecognizer lpbhClass = LBPHFaceRecognizer.Create();

            //训练
            lpbhClass.Train(images, labels);
            //保存训练的分类器
            //lpbhClass.Save("lpbhClass.xml");
            //加载分类器
            //faceClass.Read("faceClass.xml");
            //fisherClass.Read("fisherClass.xml");
            //lpbhClass.Read("lpbhClass.xml");

            //预测样本                                       
            Mat src_5 = Cv2.ImRead("persons/person1/2.jpg", ImreadModes.Grayscale);
            Cv2.Resize(src_5, src_5, new Size(128, 128));

            //标签类别
            int lpbhResult = lpbhClass.Predict(src_5);

            return;
        }

        #endregion
    }



    //1.人脸检测 CascadeClassifier
    //加载 Opencv 自带的人脸检测 haarcascade_frontalface_alt.xml 分类器。
    //图像预处理 cvtColor（灰度化） equalizeHist（直方图均衡化）。
    //使用 detectMultiScale 函数进行识别。
    //使用 rectangle 函数绘制找到的目标矩形框。
    //在原图像上 ROI 截取彩色的人脸保存。
    //2.人脸识别 FaceRecognizer FisherFaceRecognizer LBPHFaceRecognizer
    //2.1 人脸识别分类器训练
    //样本归一化，即图像大小一致、灰度化、直方图均衡化等。
    //Ptr<FaceRecognizer> face = EigenFaceRecognizer::create()。
    //创建样本和标签向量 std::vector<Mat> images std::vector<int> labels 并 push_back 加入样本和标签。
    //进行训练 face->train(images,labels)。
    //保存训练的分类器 face->save("face.xml")。
    //2.2 人脸识别分类器加载和使用
    //加载训练好的分类器 face->load(face.xml)。
    //待识别图像预处理 resize cvtColor 等。
    //预测输入图像获取标签值 int label = face->predict(Mat src)，当然也可以获取 置信度 来调整 阈值。
    //根据标签值，绘制 putText 对应的角色名。
    //Note:

    //1.训练和预测的图像大小必须一样且为灰度图像。

    //2.使用 FisherFaceRecognizer 标签必须大于2类



}