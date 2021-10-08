namespace Comic.Api.Commands.MemberSecurity
{
    public class VerifyShiftingSms
    {
        public string Phone { get; set; }
        public string Code { get; set; }
        public string DeviceId { get; set; }
    }
}
