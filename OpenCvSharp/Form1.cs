using OpenCvSharp.Core;
using OpenCvSharp.Extensions;
using OpenCvSharp.Face;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows.Forms;

//OPENCVSHARP LIVE FACE DETECTION SOURCE CODE BY: ROCAROBIN AKA BLAISEXEN
namespace OpenCvSharp
{
    public delegate void Handler();
    public partial class Form1 : Form
    {
        //自动  中止状态
        EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.AutoReset);
        int g_count = 0;
        VideoCapture capture;
        //frame存储每一帧图像
        Mat frame;
        Bitmap frameimage;
        //人脸检测分类器
        CascadeClassifier cascade;
        bool newlyloaded = true;

        event Handler CountedADozen;

        public Form1()
        {
            InitializeComponent();

            InitFaceDatabase();

        }

        #region DirectShow List Video Devices
        //===================================
        internal static readonly Guid SystemDeviceEnum = new Guid(0x62BE5D10, 0x60EB, 0x11D0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);
        internal static readonly Guid VideoInputDevice = new Guid(0x860BB310, 0x5D01, 0x11D0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        [ComImport, Guid("55272A00-42CB-11CE-8135-00AA004BB851"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IPropertyBag
        {
            [PreserveSig]
            int Read(
                [In, MarshalAs(UnmanagedType.LPWStr)] string propertyName,
                [In, Out, MarshalAs(UnmanagedType.Struct)] ref object pVar,
                [In] IntPtr pErrorLog);
            [PreserveSig]
            int Write(
                [In, MarshalAs(UnmanagedType.LPWStr)] string propertyName,
                [In, MarshalAs(UnmanagedType.Struct)] ref object pVar);
        }

        [ComImport, Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface ICreateDevEnum
        {
            [PreserveSig]
            int CreateClassEnumerator([In] ref Guid type, [Out] out IEnumMoniker enumMoniker, [In] int flags);
        }

        private void ListVideoDevices()
        {
            Object bagObj = null;
            object comObj = null;
            ICreateDevEnum enumDev = null;
            IEnumMoniker enumMon = null;
            IMoniker[] moniker = new IMoniker[100];
            IPropertyBag bag = null;
            try
            {
                // Get the system device enumerator
                Type srvType = Type.GetTypeFromCLSID(SystemDeviceEnum);
                // create device enumerator
                comObj = Activator.CreateInstance(srvType);
                enumDev = (ICreateDevEnum)comObj;
                // Create an enumerator to find filters of specified category
                enumDev.CreateClassEnumerator(VideoInputDevice, out enumMon, 0);
                Guid bagId = typeof(IPropertyBag).GUID;
                while (enumMon.Next(1, moniker, IntPtr.Zero) == 0)
                {
                    // get property bag of the moniker
                    moniker[0].BindToStorage(null, null, ref bagId, out bagObj);
                    bag = (IPropertyBag)bagObj;
                    // read FriendlyName
                    object val = "";
                    bag.Read("FriendlyName", ref val, IntPtr.Zero);
                    //list in box
                    cmbcamera.Items.Add((string)val);
                }

            }
            catch (Exception)
            {
            }
            finally
            {
                bag = null;
                if (bagObj != null)
                {
                    Marshal.ReleaseComObject(bagObj);
                    bagObj = null;
                }
                enumDev = null;
                enumMon = null;
                moniker = null;
            }
            if (cmbcamera.Items.Count > 0) cmbcamera.SelectedIndex = 0;
        }
        #endregion //List Video Devices

        private void cmbcamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!newlyloaded)
            {
                stop_capture();
                initialise_capture(cmbcamera.SelectedIndex);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop_capture();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListVideoDevices();
            cascade = new CascadeClassifier("face.xml");
            initialise_capture(0);
            newlyloaded = false;

            this.CountedADozen = DoSomething;
        }

        internal void stop_capture()
        {
            grabstart = false;
            try
            {
                tvthread.Abort();
            }
            catch { }

            Thread.Sleep(1000);
            capture.Release();
        }


        public void DoSomething()//事件成员被触发时要调用的方法
        {
            imageList1.Images.Add(pictureBox1.Image);
            listView1.Items.Add("face");
        }

        internal bool grabstart = false;
        System.Threading.Thread tvthread;
        internal void initialise_capture(int camndex)
        {
            frame = new Mat();
            capture = new VideoCapture();
            capture.Open(camndex);
            grabstart = true;
            tvthread = new System.Threading.Thread(runthreadedframe);
            tvthread.Priority = ThreadPriority.AboveNormal;
            tvthread.Start();
        }

        internal void runthreadedframe(object State)
        {
            Thread.Sleep(1000);
            timerthreadedframe = new System.Threading.Timer(voidthreadedframe, null, 0, Timeout.Infinite);
        }
        System.Threading.Timer timerthreadedframe;
        internal void voidthreadedframe(object State)
        {
            xTvGrabber();
            if (grabstart)
                timerthreadedframe.Change(0, Timeout.Infinite);
            else
                timerthreadedframe.Change(Timeout.Infinite, Timeout.Infinite);
        }


        OpenCvSharp.Scalar color;
        OpenCvSharp.Rect orig_area;
        //灰度图像
        Mat grayImage = new Mat();
        internal void xTvGrabber()
        {
            GC.AddMemoryPressure(1028);
            capture.Read(frame);
            //ColorConversionCodes.BGR2GRAY
            //图像预处理:CvtColor得到灰度图像
            Cv2.CvtColor(frame, grayImage, ColorConversionCodes.BGRA2GRAY);
            //图像预处理:equalizeHist（直方图均衡化）
            //Cv2.EqualizeHist
            //3*3降噪 （2*3+1)
            //Cv2.Blur(grayImage, grayImage, new OpenCvSharp.Size(7, 7));
            //边缘显示
            //Cv2.Canny(grayImage, grayImage, 0, 30, 3);

            //进行识别
            Rect[] faces = cascade.DetectMultiScale(
                            image: grayImage,
                            scaleFactor: 1.1,
                            minNeighbors: 10,
                            flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                            minSize: new OpenCvSharp.Size(34, 34)
                            );

            if (faces.Length != 0)
            {
                var rnd = new Random();
                color = Scalar.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));

                for (int i = 0; i < faces.Length; i++)
                {
                    orig_area = new Rect(faces[i].X, faces[i].Y, faces[i].Width, faces[i].Height);

                    var detectedFaceImage = new Mat(frame, faces[i]);
                    Image img = detectedFaceImage.ToBitmap();
                    pictureBox2.Image = img;
                    pictureBox2.Tag = detectedFaceImage;
                    //imageList1.Images.Add(img);

                    // CountedADozen();
                    string name = PredictAndReturnName(detectedFaceImage);

                    //添加文字
                    Cv2.PutText(frame, name, new OpenCvSharp.Point(faces[i].X - 5, faces[i].Y - 5),
                    HersheyFonts.HersheyPlain, 2.0, Scalar.DeepSkyBlue);
                    //绘制找到的目标矩形框
                    Cv2.Rectangle(frame, orig_area, color, 3);
                    //截取彩色的人脸保存Cv2.GetValidDisparityROI
                }
            }

            frameimage = BitmapConverter.ToBitmap(frame);
            pictureBox1.Image = frameimage;
            GC.RemoveMemoryPressure(1028);
        }

        private string PredictAndReturnName(Mat sample)
        {
            string name = "Unknown";
            if (!IsTrain) { return name; }
            try
            {
                Cv2.CvtColor(sample, sample, ColorConversionCodes.BGRA2GRAY);
                Cv2.Resize(sample, sample, new OpenCvSharp.Size(128, 128));
                model.Predict(sample, out int label, out double confidence);
                //if (confidence > 50)
                {
                    var people = sampleList.Find(t => t.Label == label);
                    if (people != null)
                    {
                        return people.Name;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return name;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            int label = Convert.ToInt32(this.txtLabel.Text);
            var sample = pictureBox2.Tag as Mat;

            try
            {
                if (sample != null)
                {
                    Cv2.CvtColor(sample, sample, ColorConversionCodes.BGRA2GRAY);
                    Cv2.Resize(sample, sample, new OpenCvSharp.Size(128, 128));

                    Image img = sample.ToBitmap();

                    //string basePath = Path.Combine("D:\\", "FaceDatabase", "my_faces", this.txtName.Text);
                    string basePath = Path.Combine("FaceDatabase", "my_faces", this.txtName.Text);
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    string fileName = Path.Combine(basePath, $"{ Guid.NewGuid()}.bmp");
                    img.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            catch (Exception ex)
            {
            }

            imageList1.Images.Add(frameimage);
            listView1.Items.Add("new", imageList1.Images.Count - 1);

        }


        private void button4_Click(object sender, EventArgs e)
        {
            InitFaceDatabase();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;//你选中的图片的绝对路径
                //pictureBox1.Image = Image.FromFile(path);
                OpencvHelper.DetectMultiScale(path);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FaceHelper faceHelper = new FaceHelper();
            faceHelper.Predict();

            //OpencvHelper.EigenFace();
            //OpencvHelper.FisherFace();
            //OpencvHelper.LBPHFace();

        }

        private bool IsTrain = false;
        private FaceRecognizer model;
        private List<PeopleSampleModel> sampleList = new List<PeopleSampleModel>();
        private void InitFaceDatabase()
        {
            model = EigenFaceRecognizer.Create();

            string basePath = Path.Combine("FaceDatabase", "my_faces");
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            string[] dires = Directory.GetDirectories(basePath);
            int lable = 1;
            foreach (var dirPath in dires)
            {

                var peopleDir = Directory.CreateDirectory(dirPath);

                FileInfo[] fileInfos = peopleDir.GetFiles();
                foreach (var file in fileInfos)
                {
                    var model = new PeopleSampleModel();
                    model.Name = peopleDir.Name;
                    model.Label = lable;
                    model.Image = Cv2.ImRead(file.FullName);
                    sampleList.Add(model);
                }

                //var sss1 = Directory.GetParent(item);
                lable++;
            }

            //, this.txtName.Text
            if (sampleList == null || sampleList.Count == 0)
            {
                return;
            }
            FaceHelper faceHelper = new FaceHelper();
            //训练
            faceHelper.Train(model, sampleList);
            IsTrain = true;
        }

    }
}
