using Hackathon23_GPOtoGuestConfig.Models;
using Hackathon23_GPOtoGuestConfigTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hackathon23_GPOtoGuestConfig.Services.Tests
{
    [TestClass()]
    public class GuestConfigConversionUtilityTests
    {
        private string[] sectionHeadings = new[] { "System Access", "Registry Values", "Group Membership", "Service General Setting", "Version" };
        private string[] sectionValues = new[] { "System Access", "Registry Values", "Group Membership", "Service General Setting", "Version" };

        [TestMethod()]
        public void ConvertGPOtoGuestConfigTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertSecPolicytoGuestConfigTest()
        {
            var actual = new SecPolicyContent();
            var k = new GuestConfigConversionUtility();
            var inf = k.ParseInf(actual.Content, actual.FileName);

            var sections = inf.GetSections();
            //can read these values
            foreach (var section in sectionHeadings)
            {
                var matchingSection = sections.Where(x => x.Equals(section)).FirstOrDefault();
                Assert.IsNotNull(matchingSection);
            }

            var k2 = new SecPolicyMappingTable();
            k2.Populate();

            Assert.IsNotNull(k2.Mappings);
            //values

            foreach (var section in sectionHeadings)
            {
                var matchingKeys = inf.GetKeys(section);
                Assert.IsNotNull(matchingKeys);

                //get each value
                foreach (var key in matchingKeys)
                {
                    var matchingValue = inf.GetValue(key, section);
                    Assert.IsNotNull(matchingValue);
                }
            }
        }

        [TestMethod()]
        public void ConvertSecPolicytoGuestConfigReturnsTestResult()
        {
            var actual = new SecPolicyContent();
            var k = new GuestConfigConversionUtility();
            var inf = k.ParseInf(actual.Content, actual.FileName);

            var result = k.ConvertSecPolicytoGuestConfig(actual.Content, actual.FileName);
            //can read these values

            Assert.IsNotNull(result);
        }
    }
}