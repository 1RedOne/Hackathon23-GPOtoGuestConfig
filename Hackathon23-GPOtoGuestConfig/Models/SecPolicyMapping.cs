using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Hackathon23_GPOtoGuestConfig.Models
{
    public class Mapping
    {
        public string name { get; set; }
        public string url { get; set; }
        public string GuestConfigParamName { get; set; }
    }

    //public class SecPolicyMappingTable
    //{
    //    public List<Mapping> Mappings { get; set; }
    //}

    public class SecPolicyMappingTable
    {
        [JsonPropertyName("Mappings")]
        public List<Mapping> Mappings { get; set; }

        private string[] SupportedSections = new[] { "System Access", "Registry Values", "Group Membership", "Service General Setting", "Version" };

        public void Populate()
        {
            var values = File.ReadAllText("SecPolicyMappings.json");
            //parse to jobject
            var k = JsonObject.Parse(values);

            var k3 = k.AsObject();

            var k5 = k3.Deserialize<SecPolicyMappingTable>();
            this.Mappings = k5.Mappings;
        }

        public bool IsSupported(string sectionName, string PropertyName)
        {
            //if sectionName is in SupportedSections and PropertyName is in Mappings, return true
            var matchingSection = SupportedSections.Where(x => x.Equals(sectionName)).FirstOrDefault();
            if (matchingSection != null)
            {
                var matchingProperty = Mappings.Where(x => x.name.Equals(PropertyName.Trim())).FirstOrDefault();
                if (matchingProperty != null)
                {
                    return true;
                }
            }

            return false;
        }

        public Mapping MatchingValue(string sectionName, string PropertyName)
        {
            //if sectionName is in SupportedSections and PropertyName is in Mappings, return true
            var matchingSection = SupportedSections.Where(x => x.Equals(sectionName)).FirstOrDefault();
            if (matchingSection != null)
            {
                var matchingProperty = Mappings.Where(x => x.name.Equals(PropertyName)).FirstOrDefault();
                if (matchingProperty != null)
                {
                    return matchingProperty;
                }
            }

            return new Mapping();
        }
    }
}