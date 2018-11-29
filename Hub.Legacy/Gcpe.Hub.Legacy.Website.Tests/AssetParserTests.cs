using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gcpe.Hub.News.ReleaseManagement;


namespace Gcpe.Hub.Tests
{
    [TestClass]
    public class AssetParserTest
    {
        [TestMethod]
        public void AssetTest()
        {

           string testhtml = "<p><asset>https://www.youtube.com/watch?v=CJzOAZQW5fk</asset></p><asset>B</asset><asset>C</asset>";
           var resulthtml = AssetEmbedManager.RenderAssetsInHtml(testhtml);
        }
    }
}
