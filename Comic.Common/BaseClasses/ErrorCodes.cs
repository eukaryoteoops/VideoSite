using System.Collections.Generic;

namespace Comic.Common.BaseClasses
{
    public class ErrorCodes
    {
        public static Dictionary<string, string> ErrorDesc => new Dictionary<string, string>
        {
            { AccountDuplicated , "account duplicate." },
            { NotEnougnMoney , "not enough money." },
            { AccountOrPwdError , "account or password error." },
            { FeeInfoError , "fee info error." },
            { ChapterNotExist , "chapter not exist." },
            { AccountDisabled , "account disabled." },
            { UnknownDevice , "device unknown." },
            { PhoneDuplicated , "phone duplicate." },
            { VerifyCodeError , "verify code error." },
            { PhoneNotExist , "phone not exist." },
            { EmailNotExist , "email not exist." },
            { EmailDuplicated , "email duplicate." },
            { SystemError , "Not defined error." },
        };

        public static string AccountDuplicated => "1000";
        public static string NotEnougnMoney => "1001";
        public static string AccountOrPwdError => "1002";
        public static string FeeInfoError => "1003";
        public static string ChapterNotExist => "1004";
        public static string AccountDisabled => "1005";
        public static string UnknownDevice => "1006";
        public static string PhoneDuplicated => "1007";
        public static string VerifyCodeError => "1008";
        public static string PhoneNotExist => "1009";
        public static string EmailNotExist => "1010";
        public static string EmailDuplicated => "1011";
        public static string SystemError => "9999";
    }
}
