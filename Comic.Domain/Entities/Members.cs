using System;
using Comic.Common.Utilities;

namespace Comic.Domain.Entities
{
    public class Members : NonAutoIncrementEntity
    {
        public Members()
        {
        }

        public Members(string name, string password, string hash, string deviceId, int? merchantId = null, string source = "web")
        {
            Id = new Random().Next(1000000, 9999999);
            Name = name;
            Salt = CryptographyUtility.GenerateSalt();
            Password = CryptographyUtility.Create(password, Salt);
            Hash = hash;
            MerchantId = merchantId ?? 11003;
            PackageTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
            IsPremium = false;
            IsPurchased = false;
            DeviceId = deviceId;
            Source = source;
            State = true;
            CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
        public int MerchantId { get; set; }
        public int Point { get; set; }
        public long PackageTime { get; set; }
        public bool IsPremium { get; set; }
        public bool IsPurchased { get; set; }
        public string DeviceId { get; set; }
        public string Source { get; set; }
        public bool State { get; set; }
        public long CreatedTime { get; set; }
        public long? LoginTime { get; set; }
        public long? UpdatedTime { get; set; }

        public void UpdateMember(bool state)
        {
            this.State = state;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void ResetPassword(string password)
        {
            this.Password = CryptographyUtility.Create(password, this.Salt);
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void SetPremium()
        {
            this.IsPremium = true;
            this.IsPurchased = true;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void UpdatePoint(int value)
        {
            this.Point += value;
            this.IsPurchased = true;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void UpdatePackage(int value)
        {
            if (this.PackageTime >= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds())
                this.PackageTime += value * 24 * 60 * 60;
            else
                this.PackageTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds() + value * 24 * 60 * 60;
            this.IsPurchased = true;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void UpdatePassword(string password)
        {
            this.Password = CryptographyUtility.Create(password, this.Salt);
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void DisableMember()
        {
            this.State = false;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void ChangeDevice(string deviceId)
        {
            this.DeviceId = deviceId;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void UpdatePhone(string phone)
        {
            this.Phone = phone;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void UpdateEmail(string email)
        {
            this.Email = email;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void UpdateLoginTime()
        {
            this.LoginTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }
    }
}
