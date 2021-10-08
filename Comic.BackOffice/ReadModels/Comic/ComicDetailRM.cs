using System.Collections.Generic;

namespace Comic.BackOffice.ReadModels.Comic
{
    public class ComicDetailRM
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Desc { get; set; }
        public bool IsEnded { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
