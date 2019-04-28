using SamplesCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpenCvSharp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Test();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void Test()
        {
            ISample sample =
            //new BgSubtractorMOG();
            //new BinarizerSample();
            //new BRISKSample();
            //new CaffeSample();
            //new ClaheSample();
            //new ConnectedComponentsSample();
            //new DFT();
            //new FaceDetection();
            //new FASTSample();
            //new FlannSample(); 
            //new FREAKSample();
            //new HistSample();
            //new HOGSample();
            //new HoughLinesSample();
            //new KAZESample2();
            //new KAZESample();
            //new MatOperations();
            //new MatToBitmap();
            //new MDS();
            //new MSERSample();
            //new NormalArrayOperations();
            //new PhotoMethods();
            //new MergeSplitSample();
            //new MorphologySample();
            //new PixelAccess();
            //new SeamlessClone();
            //new SiftSurfSample();
            //new SolveEquation();
            //new StarDetectorSample();
            //new Stitching();
            //new Subdiv2DSample();
            //new SuperResolutionSample();
            //new SVMSample();
            //new VideoWriterSample();
            new VideoCaptureSample();

            sample.Run();

        }

    }
}
