using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvSharp.ExtendApi.Dtos
{
    public class DetectResponse
    {
        public string Request_Id { get; set; }
        public string Image_Id { get; set; }
        public List<DetectResponseFaceModel> Faces { get; set; }
        public int Time_Used { get; set; }
        public string Error_Message { get; set; }
    }

    public class DetectResponseFaceModel
    {
        public string Face_Token { get; set; }
    }
}
