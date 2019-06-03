using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvSharp.ExtendApi.Dtos
{
    public class DetectRequest
    {
        /// <summary>
        /// 人脸图片文件路径，需要用 post multipart/form-data 的方式上传。
        /// </summary>
        public string ImageFilePath { get; set; }
    }
}
