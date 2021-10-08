using System;

namespace Comic.Common.BaseClasses
{
    public class CustomResponse
    {
        public CustomResponse(string code, string desc)
        {
            Code = code;
            Desc = desc;
            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public string Code { get; set; }
        public string Desc { get; set; }
        public long Time { get; set; }

    }

    public class CustomResponse<T> : CustomResponse where T : class
    {
        public CustomResponse(string code, string desc, T data) : base(code, desc)
        {
            Data = data;
        }

        public T Data { get; set; }
    }

    public class CustomPagingResponse<T> : CustomResponse<T> where T : class
    {
        public CustomPagingResponse(string code, string desc, T data, int page, int size, int total) : base(code, desc, data)
        {
            Page = page;
            Size = size;
            Total = total;
        }

        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
    }
}
