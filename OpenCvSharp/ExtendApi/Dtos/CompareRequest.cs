using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvSharp.ExtendApi.Dtos
{
    public class CompareRequest
    {
        /// <summary>
        /// 人脸标识face_token
        /// </summary>
        public string FaceToken1 { get; set; }
        /// <summary>
        /// 人脸标识face_token
        /// </summary>
        public string FaceToken2 { get; set; }
        /// <summary>
        /// 人脸图片文件路径，需要用 post multipart/form-data 的方式上传。
        /// </summary>
        public string ImageFilePath1 { get; set; }
        /// <summary>
        /// 人脸图片文件路径，需要用 post multipart/form-data 的方式上传。
        /// </summary>
        public string ImageFilePath2 { get; set; }
    }
}
