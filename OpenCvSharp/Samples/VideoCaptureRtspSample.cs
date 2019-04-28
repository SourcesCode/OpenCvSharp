using SamplesCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvSharp.Samples
{
    public class VideoCaptureRtspSample : ISample
    {
        public void Run()
        {
            string videoStreamAddress = "rtsp://admin:dahua@192.168.1.108/cam/realmonitor?channel=1&subtype=0";

            var capture = new VideoCapture();

            capture.Open("");
        }
    }
}
