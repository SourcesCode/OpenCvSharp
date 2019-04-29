namespace OpenCvSharp.ExtendApi
{
    //[TestClass]
    public class FacePlusPlusApiHelperUnitTest //: BaseUnitTest
    {
        //[TestMethod]
        public void TestDetect()
        {
            var result = FacePlusPlusApiHelper.Detect();
        }

        //[TestMethod]
        public void TestCreateFaceSet()
        {
            var result = FacePlusPlusApiHelper.CreateFaceSet();
        }

        //[TestMethod]
        public void TestAddFace()
        {
            var result = FacePlusPlusApiHelper.AddFace();
        }

        //[TestMethod]
        public void TestSearch()
        {
            var result = FacePlusPlusApiHelper.Search();
        }


    }
}
