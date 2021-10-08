using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.BackOffice.ReadModels.Home
{
    public class ComicCounterStatisticRM
    {
        public ComicCounterStatisticRM(string date, List<ComicCounters> counters)
        {
            Date = date;
            Ch1 = counters.Where(o => o.Comic.Channel == ComicChannelEnum.韓漫).Count();
            Ch2 = counters.Where(o => o.Comic.Channel == ComicChannelEnum.同人誌).Count();
        }
        public string Date { get; set; }
        public int Ch1 { get; set; }
        public int Ch2 { get; set; }
    }
}
