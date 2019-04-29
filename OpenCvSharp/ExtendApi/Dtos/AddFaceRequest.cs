using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvSharp.ExtendApi.Dtos
{
    public class AddFaceRequest
    {
        /// <summary>
        /// FaceSet 的标识
        /// </summary>
        public string FaceSetToken { get; set; }
        /// <summary>
        /// 人脸标识 face_token 组成的字符串，可以是一个或者多个，用逗号分隔。
        /// 最多不超过5个face_token
        /// </summary>
        public string FaceTokens { get; set; }
    }
}
