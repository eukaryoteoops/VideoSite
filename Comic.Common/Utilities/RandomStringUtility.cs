using System;
using System.Linq;
using System.Text;

namespace Comic.Common.Utilities
{
    public class RandomStringUtility
    {
        public static string Create(int length, bool upperCase = true, bool lowerCase = true)
        {
            StringBuilder builder = new StringBuilder();
            var list = Enumerable.Range(0, 10).Select(e => e.ToString());
            if (lowerCase)
                list = list.Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()));
            if (upperCase)
                list = list.Concat(Enumerable.Range(65, 26).Select(e => ((char)e).ToString()));
            list.OrderBy(e => Guid.NewGuid()).Take(length).ToList().ForEach(e => builder.Append(e));
            return builder.ToString();
        }
    }
}
