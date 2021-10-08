using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.BackOffice.ReadModels.Home
{
    public class VideoCounterStatisticRM
    {
        public VideoCounterStatisticRM(string date, List<VideoCounters> counters)
        {
            Date = date;
            Ch1 = counters.Where(o => o.Video.Channel == ChannelEnum.無碼).Count();
            Ch2 = counters.Where(o => o.Video.Channel == ChannelEnum.歐美).Count();
            Ch3 = counters.Where(o => o.Video.Channel == ChannelEnum.有碼).Count();
            Ch4 = counters.Where(o => o.Video.Channel == ChannelEnum.動畫).Count();
            Ch5 = counters.Where(o => o.Video.Channel == ChannelEnum.自拍).Count();
            Ch6 = counters.Where(o => o.Video.Channel == ChannelEnum.三級).Count();
            Ch7 = counters.Where(o => o.Video.Channel == ChannelEnum.中文).Count();
            Ch8 = counters.Where(o => o.Video.Channel == ChannelEnum.韓國).Count();
            Ch10 = counters.Where(o => o.Video.Channel == ChannelEnum.素人).Count();
            Ch11 = counters.Where(o => o.Video.Channel == ChannelEnum.無碼中文).Count();
            Ch13 = counters.Where(o => o.Video.Channel == ChannelEnum.免費).Count();
            Ch14 = counters.Where(o => o.Video.Channel == ChannelEnum.獨家).Count();
            Ch16 = counters.Where(o => o.Video.Channel == ChannelEnum.動漫).Count();

        }
        public string Date { get; set; }
        public int Ch1 { get; set; }
        public int Ch2 { get; set; }
        public int Ch3 { get; set; }
        public int Ch4 { get; set; }
        public int Ch5 { get; set; }
        public int Ch6 { get; set; }
        public int Ch7 { get; set; }
        public int Ch8 { get; set; }
        public int Ch10 { get; set; }
        public int Ch11 { get; set; }
        public int Ch13 { get; set; }
        public int Ch14 { get; set; }
        public int Ch16 { get; set; }
    }

}
