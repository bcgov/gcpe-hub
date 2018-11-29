using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RM = Gcpe.Hub.News.ReleaseManagement.ReleaseManagementModel;

namespace Gcpe.Hub.Tests
{
    [TestClass]
    public class SlugUnitTest
    {
        [TestMethod]
        public void SlugTestMethod()
        {
            Assert.AreEqual("province-celebrates-first-nation-metis-inuit-employees", RM.GenerateSlug("Province celebrates First Nation, Métis, Inuit employees"));
            Assert.AreEqual("bcs-skills-for-jobs-blueprint-enewsletter", RM.GenerateSlug("B.C.'s Skills for Jobs Blueprint eNewsletter"));
            Assert.AreEqual("opinion-editorial-as-komoks-signs-aip-treaty-process-is-going-strong", RM.GenerateSlug("OPINION-EDITORIAL: As K'ómoks signs AIP, treaty process is going strong"));
            Assert.AreEqual("bc-liquor-stores-collect-over-208000-for-nepal-relief", RM.GenerateSlug("BC Liquor Stores collect over $208,000 for Nepal Relief"));
            Assert.AreEqual("canada-and-british-columbia-sign-agreement-in-principle", RM.GenerateSlug("Canada and British Columbia sign Agreement-in-Principle"));
            Assert.AreEqual("factsheet-bc-stats-report-profile-of-the-british-columbia-high-tech-sector-2014-edition", RM.GenerateSlug("FACTSHEET: BC Stats Report - Profile of the British Columbia High Tech Sector 2014 Edition"));
            Assert.AreEqual("tsilhqotin-title-land-access", RM.GenerateSlug("Tsilhqot’in title land access"));
            Assert.AreEqual("et-si-labsenteisme-revelait-un-mal-etre-au-travail", RM.GenerateSlug("Et si l’absentéisme révélait un mal être au travail ?"));
        }
    }
}
