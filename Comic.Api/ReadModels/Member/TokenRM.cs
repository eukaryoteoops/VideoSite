namespace Comic.Api.ReadModels.Member
{
    public class TokenRM
    {
        public TokenRM(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}
