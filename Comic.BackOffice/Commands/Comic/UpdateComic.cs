using System.Collections.Generic;

namespace Comic.BackOffice.Commands.Comic
{
    public class UpdateComic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte Channel { get; set; }
        public string Author { get; set; }
        public string Desc { get; set; }
        public bool IsEnded { get; set; }
        public List<int> Tags { get; set; }
    }
}
