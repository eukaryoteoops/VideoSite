namespace Comic.Api.ReadModels.Member
{
    public class RegisterRM
    {
        public RegisterRM(string name, string hash)
        {
            Name = name;
            Hash = hash;
        }

        public string Name { get; set; }
        public string Hash { get; set; }
    }
}
