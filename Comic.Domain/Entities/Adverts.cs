namespace Comic.Domain.Entities
{
    public class Adverts : Entity
    {
        public Adverts()
        {
        }

        public Adverts(byte type, string pic, string url)

        {
            Type = type;
            Pic = pic;
            Url = url;
            Rule = 0;
            Order = int.MaxValue;
        }

        public byte Type { get; set; }
        public string Pic { get; set; }
        public string Url { get; set; }
        public byte Rule { get; set; }
        public int Order { get; set; }

        public void UpdateAdvert(string pic, string url)
        {
            this.Pic = string.IsNullOrEmpty(pic) ? this.Pic : pic;
            this.Url = string.IsNullOrEmpty(url) ? this.Url : url;
        }
    }
}
