using System;
using Comic.Common.Utilities;

namespace Comic.Domain.Entities
{
    public class Managers : Entity
    {
        public Managers()
        {
        }

        public Managers(string name, string password)
        {
            Name = name;
            Salt = CryptographyUtility.GenerateSalt();
            Password = CryptographyUtility.Create(password, Salt);
            State = true;
            CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool State { get; set; }
        public long CreatedTime { get; set; }
        public long? UpdatedTime { get; set; }

        public void UpdateManager(string password)
        {
            this.Password = string.IsNullOrEmpty(password) ? this.Password : CryptographyUtility.Create(password, this.Salt);
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public void UpdateState(bool state)
        {
            this.State = state;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }
    }
}
