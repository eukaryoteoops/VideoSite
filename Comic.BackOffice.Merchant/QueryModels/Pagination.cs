using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Comic.BackOffice.Merchant.QueryModels
{
    public class Pagination
    {
        public Pagination()
        {
            this.PageNo = 1;
            this.PageSize = 20;
        }

        [FromQuery(Name = "pageNo")]
        [Range(1, int.MaxValue)]
        public int PageNo { get; set; }

        [FromQuery(Name = "pageSize")]
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; }
    }
}
