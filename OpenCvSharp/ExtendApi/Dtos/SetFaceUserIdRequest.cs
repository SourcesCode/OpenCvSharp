using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvSharp.ExtendApi.Dtos
{
    public class SetFaceUserIdRequest
    {
        /// <summary>
        /// 人脸标识face_token
        /// </summary>
        public string FaceToken { get; set; }
        /// <summary>
        /// 用户自定义的user_id，不超过255个字符，不能包括^@,&=*'"
        /// 建议将同一个人的多个face_token设置同样的user_id。
        /// </summary>
        public string UserId { get; set; }

    }
}
