namespace Comic.Domain.Entities
{
    public class VideoChannels : Entity
    {
        public VideoChannels()
        {
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool State { get; set; }
        public int Order { get; set; }

        public void UpdateState(bool state)
        {
            this.State = state;
        }
    }

    public enum ChannelEnum
    {
        無碼 = 1,
        歐美 = 2,
        有碼 = 3,
        動畫 = 4,
        自拍 = 5,
        三級 = 6,
        中文 = 7,
        韓國 = 8,
        VR = 9,
        素人 = 10,
        無碼中文 = 11,
        高清 = 12,
        免費 = 13,
        獨家 = 14,
        水印 = 15,
        動漫 = 16
    }
}
