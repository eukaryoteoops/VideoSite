namespace Comic.Api.Commands.MemberSecurity
{
    public class VerifyShiftingEmail
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string DeviceId { get; set; }
    }
}
