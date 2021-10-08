using System;
using Comic.Common.Utilities;

namespace Comic.Domain.Entities
{
    public class Merchants : Entity
    {
        public Merchants()
        {
        }

        public Merchants(string name, string nickName, int bonus)
        {
            Name = name;
            Salt = CryptographyUtility.GenerateSalt();
            Password = CryptographyUtility.Create("coMic666", Salt);
            NickName = nickName;
            Bonus = bonus;
            State = true;
            CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public string Name { get; set; }
        public string NickName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int Bonus { get; set; }
        public bool State { get; set; }
        public long CreatedTime { get; set; }
        public long? UpdatedTime { get; set; }
        public PromoteUrls PromoteUrls { get; set; }

        public void UpdateMerchant(string nickName, int bonus)
        {
            this.NickName = string.IsNullOrEmpty(nickName) ? this.NickName : nickName;
            this.Bonus = bonus == 0 ? this.Bonus : bonus;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void UpdateState(bool state)
        {
            this.State = state;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void UpdatePassword(string password)
        {
            this.Password = CryptographyUtility.Create(password, this.Salt);
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }
    }
}
