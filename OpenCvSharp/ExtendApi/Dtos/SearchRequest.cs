using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvSharp.ExtendApi.Dtos
{
    public class SearchRequest
    {
        /// <summary>
        /// FaceSet 的标识
        /// </summary>
        public string FaceSetToken { get; set; }
        /// <summary>
        /// 人脸的 face_token，优先使用该参数
        /// </summary>
        public string FaceToken { get; set; }
        /// <summary>
        /// 人脸图片文件路径，需要用 post multipart/form-data 的方式上传。
        /// </summary>
        public string ImageFilePath { get; set; }
    }
}
