namespace Hackathon23_GPOtoGuestConfigTests
{
    public class SecPolicyContent
    {
        public string FileName = "SecPolicy.inf";

        public string Content = @"
[System Access]
MinimumPasswordAge = 30
MaximumPasswordAge = -1
PasswordComplexity = 1
LockoutBadCount = 10
ResetLockoutCount = 5
LockoutDuration = 5
AllowAdministratorLockout = 1
[Registry Values]
[Group Membership]
*S-1-5-32-544__Memberof =
*S-1-5-32-544__Members = *S-1-5-21-3818945699-900446794-3716848007-1103
[Service General Setting]
""AppReadiness"",4,""""
[Version]
[SomethingUnsupported]
SomethingUnsupported = 1
";
    }
}