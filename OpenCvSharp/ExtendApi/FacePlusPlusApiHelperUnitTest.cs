using OpenCvSharp.ExtendApi.Dtos;

namespace OpenCvSharp.ExtendApi
{
    //[TestClass]
    public class FacePlusPlusApiHelperUnitTest //: BaseUnitTest
    {
        //[TestMethod]
        public void TestDetect()
        {
            var request = new DetectRequest
            {
                ImageFilePath = "tmp/1.jpg"
            };
            var result = FacePlusPlusApiHelper.Detect(request);
        }

        //[TestMethod]
        public void TestCreateFaceSet()
        {
            var result = FacePlusPlusApiHelper.CreateFaceSet();
        }

        //[TestMethod]
        public void TestAddFace()
        {
            var request = new AddFaceRequest
            {
                FaceSetToken = "1da59b4c728cadb43092bfa0cdfa4b76",
                FaceTokens = ""
            };
            var result = FacePlusPlusApiHelper.AddFace(request);
        }

        //[TestMethod]
        public void TestSearch()
        {
            var request = new SearchRequest
            {
                FaceSetToken = "1da59b4c728cadb43092bfa0cdfa4b76",
                FaceToken = ""
                //ImageFilePath = "tmp/1.jpg"
            };
            var result = FacePlusPlusApiHelper.Search(request);
        }


    }
}
