using System.Collections.Generic;

namespace Comic.Api.ReadModels.Advert
{
    public class AdvertRM
    {

        public IEnumerable<AdvertBox> Carousel { get; set; }
        public IEnumerable<AdvertBox> Member { get; set; }
        public IEnumerable<AdvertBox> Comic { get; set; }
        public IEnumerable<AdvertBox> WebCover { get; set; }
        public IEnumerable<AdvertBox> AppCover { get; set; }
        public IEnumerable<AdvertBox> PopUp { get; set; }
        public class AdvertBox
        {
            public string Pic { get; set; }
            public string Url { get; set; }
            /// <summary>
            ///     0 : All
            ///     1 : NotPurchased
            ///     2 : IsPurchased
            /// </summary>
            public byte Rule { get; set; }
        }
    }
}
