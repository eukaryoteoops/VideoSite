using System;
using System.ComponentModel.DataAnnotations;

namespace Comic.BackOffice.Commands.Member
{
    public class CompensateMember
    {
        public string Name { get; set; }
        /// <summary>
        ///     1 : 點數
        ///     2 : 天數,9999視為終身
        /// </summary>
        [Range(1, 2)]
        public byte Type { get; set; }
        public int Value { get; set; }
    }
}
