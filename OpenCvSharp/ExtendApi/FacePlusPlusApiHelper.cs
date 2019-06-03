using OpenCvSharp.Core;
using OpenCvSharp.ExtendApi.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace OpenCvSharp.ExtendApi
{
    public class FacePlusPlusApiHelper
    {
        static dynamic _setting;
        static FacePlusPlusApiHelper()
        {
            _setting = new
            {
                AccessKeyId = "UKClbXP9uWUP-qWYz77oc_6rFUogj2sc",
                AccessKeySecret = "ydpVgEBTBTSAmJCBukYiPjkr1cOpNiEp"
            };
        }

        /// <summary>
        /// 人脸检测和人脸分析
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static CommonExtendApiResult Detect(DetectRequest request)
        {
            /*
             
             传入图片进行人脸检测和人脸分析。

            可以检测图片内的所有人脸，对于每个检测出的人脸，会给出其唯一标识 face_token，
            可用于后续的人脸分析、人脸比对等操作。对于正式 API Key，
            支持指定图片的某一区域进行人脸检测。

            本 API 支持对检测到的人脸直接进行分析，获得人脸的关键点和各类属性信息。
            对于试用 API Key，最多只对人脸框面积最大的 5 个人脸进行分析，
            其他检测到的人脸可以使用 Face Analyze API 进行分析。对于正式 API Key，
            支持分析所有检测到的人脸。
             */
            var apiPath = "https://api-cn.faceplusplus.com/facepp/v3/detect";
            var result = new CommonExtendApiResult();

            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            verifyPostParameters.Add("return_landmark", "1");//是否检测并返回人脸关键点。
            verifyPostParameters.Add("return_attributes", "gender,age,smiling,headpose,facequality,blur,eyestatus,emotion,ethnicity,beauty,mouthstatus,eyegaze,skinstatus");

            byte[] fileImage;
            using (Stream stream = File.Open(request.ImageFilePath, FileMode.Open))// 图片地址
            {
                fileImage = new byte[stream.Length];
                stream.Read(fileImage, 0, fileImage.Length);
                // 设置当前流的位置为流的开始
                stream.Seek(0, SeekOrigin.Begin);
            }

            //添加图片参数
            verifyPostParameters.Add("image_file", new HttpHelper4MultipartForm.FileParameter(fileImage, "1.jpg", "application/octet-stream"));
            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost(apiPath, "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);


            return result;
        }

        /// <summary>
        /// 人脸进行比对
        /// 将两个人脸进行比对，来判断是否为同一个人，返回比对结果置信度和不同误识率下的阈值。
        /// 支持传入图片或 face_token 进行比对。使用图片时会自动选取图片中检测到人脸尺寸最大的一个人脸。
        /// </summary>
        /// <returns></returns>
        public static CommonExtendApiResult Compare(CompareRequest request)
        {
            var apiPath = "https://api-cn.faceplusplus.com/facepp/v3/compare";
            var result = new CommonExtendApiResult();

            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);

            if (!string.IsNullOrWhiteSpace(request.FaceToken1) &&
                !string.IsNullOrWhiteSpace(request.FaceToken2))
            {
                #region FaceToken

                //添加图片参数
                verifyPostParameters.Add("face_token1", request.FaceToken1);
                //添加图片参数
                verifyPostParameters.Add("face_token2", request.FaceToken2);

                #endregion
            }
            else
            {
                #region ImageFilePath

                byte[] fileImage1;
                using (Stream stream = File.Open(request.ImageFilePath1, FileMode.Open))// 图片地址
                {
                    fileImage1 = new byte[stream.Length];
                    stream.Read(fileImage1, 0, fileImage1.Length);
                    // 设置当前流的位置为流的开始
                    stream.Seek(0, SeekOrigin.Begin);
                }

                //添加图片参数
                verifyPostParameters.Add("image_file1", new HttpHelper4MultipartForm.FileParameter(fileImage1, "1.jpg", "application/octet-stream"));
                byte[] fileImage2;
                using (Stream stream = File.Open(request.ImageFilePath2, FileMode.Open))// 图片地址
                {
                    fileImage2 = new byte[stream.Length];
                    stream.Read(fileImage2, 0, fileImage2.Length);
                    // 设置当前流的位置为流的开始
                    stream.Seek(0, SeekOrigin.Begin);
                }
                //添加图片参数
                verifyPostParameters.Add("image_file2", new HttpHelper4MultipartForm.FileParameter(fileImage2, "2.jpg", "application/octet-stream"));

                #endregion 
            }

            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost(apiPath, "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);

            return result;
        }

        /// <summary>
        /// 人脸搜索
        /// 
        /// 在一个已有的 FaceSet 中找出与目标人脸最相似的一张或多张人脸，
        /// 返回置信度和不同误识率下的阈值。
        /// 支持传入图片或 face_token 进行人脸搜索。
        /// 使用图片进行搜索时会选取图片中检测到人脸尺寸最大的一个人脸。
        /// </summary>
        /// <returns></returns>
        public static CommonExtendApiResult Search(SearchRequest request)
        {
            var result = new CommonExtendApiResult();
            string apiPath = $"https://api-cn.faceplusplus.com/facepp/v3/search";
            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            if (string.IsNullOrWhiteSpace(request.FaceSetToken))
            {
                verifyPostParameters.Add("faceset_token", "1da59b4c728cadb43092bfa0cdfa4b76");
            }
            else
            {
                verifyPostParameters.Add("faceset_token", request.FaceSetToken);
            }
            byte[] fileImage;
            using (Stream stream = File.Open("tmp/3.jpg", FileMode.Open))// 图片地址
            {
                fileImage = new byte[stream.Length];
                stream.Read(fileImage, 0, fileImage.Length);
                // 设置当前流的位置为流的开始
                stream.Seek(0, SeekOrigin.Begin);
            }

            //添加图片参数
            verifyPostParameters.Add("image_file", new HttpHelper4MultipartForm.FileParameter(fileImage, "1.jpg", "application/octet-stream"));
            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost(apiPath, "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);

            return result;
            //a12e6b8330b1a65a5f211f6793691b22
        }

        #region FaceSet
        /// <summary>
        /// 创建一个人脸的集合 FaceSet
        /// 用于存储人脸标识 face_token。一个 FaceSet 能够存储 10,000 个 face_token。
        /// </summary>
        /// <returns></returns>
        public static CommonExtendApiResult CreateFaceSet()
        {
            var result = new CommonExtendApiResult();
            string apiPath = $"https://api-cn.faceplusplus.com/facepp/v3/faceset/create";

            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            verifyPostParameters.Add("display_name", "demoFaceSet");
            verifyPostParameters.Add("face_tokens", "cdd88d7f918f8d1a7757a37fd42c0adc,25e0cfd15ec6abb3d5ece864f2b927f2,72f2f7fa47cd29b6a09d61ee079d8bd7");

            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost(apiPath, "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);

            return result;
            //"faceset_token":"1da59b4c728cadb43092bfa0cdfa4b76",
        }

        /// <summary>
        /// 添加人脸标识 face_token
        /// 一个 FaceSet 最多存储10,000个 face_token。
        /// </summary>
        /// <returns></returns>
        public static CommonExtendApiResult AddFace(AddFaceRequest request)
        {
            var result = new CommonExtendApiResult();
            string apiPath = $"https://api-cn.faceplusplus.com/facepp/v3/faceset/addface";

            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            if (string.IsNullOrWhiteSpace(request.FaceSetToken))
            {
                verifyPostParameters.Add("faceset_token", "1da59b4c728cadb43092bfa0cdfa4b76");
            }
            else
            {
                verifyPostParameters.Add("faceset_token", request.FaceSetToken);
            }

            //verifyPostParameters.Add("face_tokens", "cdd88d7f918f8d1a7757a37fd42c0adc,25e0cfd15ec6abb3d5ece864f2b927f2,72f2f7fa47cd29b6a09d61ee079d8bd7");
            verifyPostParameters.Add("face_tokens", request.FaceTokens);
            //16e0bd620a10929b330f7abf28027267
            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost(apiPath, "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);

            return result;
            //"faceset_token":"1da59b4c728cadb43092bfa0cdfa4b76",
        }

        #endregion

        #region Face
        /// <summary>
        /// 为检测出的某一个人脸添加标识信息，该信息会在Search接口结果中返回，用来确定用户身份。
        /// </summary>
        /// <returns></returns>
        public static CommonExtendApiResult SetFaceUserId(SetFaceUserIdRequest request)
        {
            var result = new CommonExtendApiResult();
            string apiPath = $"https://api-cn.faceplusplus.com/facepp/v3/face/setuserid";

            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            verifyPostParameters.Add("face_token", request.FaceToken);
            verifyPostParameters.Add("user_id", request.UserId);

            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost(apiPath, "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);

            return result;
        }


        #endregion


        private static string GetResponseString(HttpWebResponse httpResponse)
        {
            string responseString = null;


            using (Stream responseStream = httpResponse.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
                {
                    responseString = sr.ReadToEnd();
                }
            }

            return responseString;
        }

    }
}
