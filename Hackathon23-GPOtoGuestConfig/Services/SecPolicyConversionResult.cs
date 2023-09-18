namespace Hackathon23_GPOtoGuestConfig.Services
{
    public class SecPolicyConversionResult
    {
        public string SecurityPolicyName { get; set; }
        public string SectionHeading { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public string GuestConfigValue { get; set; }
        public string GuestConfigParamName { get; set; }
        public bool IsMatch { get; set; }
    }
}