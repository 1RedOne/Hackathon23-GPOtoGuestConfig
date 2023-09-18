using Hackathon23_GPOtoGuestConfig.Models;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace Hackathon23_GPOtoGuestConfig.Services
{
    public class GuestConfigConversionUtility
    {
        private SecPolicyMappingTable mappingTable;

        public GuestConfigConversionUtility()
        {
            var secPolicyMappingTable = new SecPolicyMappingTable();
            secPolicyMappingTable.Populate();

            this.mappingTable = secPolicyMappingTable;
        }

        public string ConvertGPOtoGuestConfig(string gpo)
        {
            //return the string "GPO to Guest Config Conversion"
            return "GPO to Guest Config Conversion";
        }

        public List<SecPolicyConversionResult> ConvertSecPolicytoGuestConfig(string secPolicy, string polName)
        {
            //parse .inf file to our infType
            var secPolicyValues = this.ParseInf(secPolicy, polName);

            //retrieve all sections, skipping sections of Unicode and Version so remove them from sections
            var sections = secPolicyValues.GetSections();//.Where(x=>x != "Unicode" || x !="Version");

            //build return object
            var returnObject = new List<SecPolicyConversionResult>();

            foreach (var section in sections)
            {
                var matchingKeys = secPolicyValues.GetKeys(section).Select(x => x.Trim());

                //get each value
                foreach (var key in matchingKeys)
                {
                    var matchingValue = secPolicyValues.GetValue(key, section).Trim();
                    var thisSupported = mappingTable.IsSupported(section, key);

                    if (thisSupported)
                    {
                        var matchingMapping = mappingTable.MatchingValue(section, key);
                        var result = new SecPolicyConversionResult()
                        {
                            SecurityPolicyName = polName,
                            GuestConfigValue = matchingMapping.url,
                            SectionHeading = section,
                            IsMatch = true,
                            PropertyName = key,
                            PropertyValue = matchingValue,
                            GuestConfigParamName = matchingMapping.GuestConfigParamName
                        };

                        returnObject.Add(result);
                    }
                    else
                    {
                        var result = new SecPolicyConversionResult()
                        {
                            SecurityPolicyName = polName,
                            GuestConfigValue = string.Empty,
                            SectionHeading = section,
                            IsMatch = false,
                            PropertyName = key,
                            PropertyValue = matchingValue,
                            GuestConfigParamName = string.Empty,
                        };

                        returnObject.Add(result);
                    }
                }
            }

            return returnObject;
        }

        public string GenerateCustomizedTemplate(List<SecPolicyConversionResult> results)
        {
            //for each result in results, get any sections with status of IsMatch = true
            var matchingResults = results.Where(x => x.IsMatch == true);

            //for each of these settings, get the value and make a jObject with the value

            //get our base template
            var values = File.ReadAllText("template.json");
            var k = JsonObject.Parse(values);

            //dig down to k properties.guestConfiguration.configurationParameter, which is an array
            var k2 = k["resources"][0]["properties"]?["guestConfiguration"]?["configurationParameter"];

            k2 = new System.Text.Json.Nodes.JsonArray();
            //discard the starting items in k2
            //for each of these settings, get the value and make a jObject with the value
            //{
            foreach (var templateSetting in matchingResults)
            {
                var expected = new JsonObject
                    {
                        {"name", string.Concat(templateSetting.GuestConfigParamName, ";", "ExpectedValue")},
                        {"value", templateSetting.PropertyValue}
                    };

                var remediate = new JsonObject
                    {
                        {"name", string.Concat(templateSetting.GuestConfigParamName, ";", "RemediateValue")},
                        {"value", templateSetting.PropertyValue}
                    };

                k2.AsArray().Add(expected);
                k2.AsArray().Add(remediate);
            }

            Console.WriteLine(k2.ToString());
            var o = k.AsObject();
            o["resources"]![0]!["properties"]!["guestConfiguration"]!["configurationParameter"] = k2;
            return PrettifyJson(o.ToJsonString());
        }

        public string GenerateCustomizedHttpRequest(List<SecPolicyConversionResult> results)
        {
            //for each result in results, get any sections with status of IsMatch = true
            var matchingResults = results.Where(x => x.IsMatch == true);

            //for each of these settings, get the value and make a jObject with the value

            //get our base template
            var values = File.ReadAllText("jsonResquest.json");
            var k = JsonObject.Parse(values);

            //dig down to k properties.guestConfiguration.configurationParameter, which is an array
            var k2 = k["properties"]?["guestConfiguration"]?["configurationParameter"];

            k2 = new System.Text.Json.Nodes.JsonArray();
            //discard the starting items in k2
            //for each of these settings, get the value and make a jObject with the value
            //{
            foreach (var templateSetting in matchingResults)
            {
                var expected = new JsonObject
            {
                {"name", string.Concat(templateSetting.GuestConfigParamName, ";", "ExpectedValue")},
                {"value", templateSetting.PropertyValue}
            };

                var remediate = new JsonObject
            {
                {"name", string.Concat(templateSetting.GuestConfigParamName, ";", "RemediateValue")},
                {"value", templateSetting.PropertyValue}
            };

                k2.AsArray().Add(expected);
                k2.AsArray().Add(remediate);
            }

            Console.WriteLine(k2.ToString());
            var o = k.AsObject();
            o["properties"]!["guestConfiguration"]!["configurationParameter"] = k2;
            return PrettifyJson(o.ToJsonString());
        }

        //should parse a text file, looking for lines that begin with "[Unicode]" and end with "[Version]"
        public IniReader ParseInf(string secPolicy, string polName)
        {
            //should parse a text file, looking for lines that begin with "[Unicode]" and end with "[Version]"
            var parser = new IniReader(polName, secPolicy, false);
            return parser;
        }

        private static string PrettifyJson(string jsonString)
        {
            using (JsonDocument jsonDocument = JsonDocument.Parse(jsonString))
            {
                // Convert the JsonDocument back to a pretty-printed JSON string
                string prettifiedJson =
                System.Text.Json.JsonSerializer.Serialize(jsonDocument.RootElement, new
                    JsonSerializerOptions
                {
                    WriteIndented = true
                });
                return prettifiedJson;
            }
        }
    }
}