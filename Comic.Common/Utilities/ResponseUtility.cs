using Comic.Common.BaseClasses;

namespace Comic.Common.Utilities
{
    public class ResponseUtility
    {
        public static CustomResponse CreateSuccessResopnse()
        {
            return new CustomResponse("0000", "success");
        }

        public static CustomResponse<T> CreateSuccessResopnse<T>(T data) where T : class
        {
            return new CustomResponse<T>("0000", "success", data);
        }

        public static CustomPagingResponse<T> CreateSuccessResopnse<T>(T data, int page, int size, int total) where T : class
        {
            return new CustomPagingResponse<T>("0000", "success", data, page, size, total);
        }

        public static CustomResponse CreateErrorResopnse(string errorCode, string errorDesc)
        {
            return new CustomResponse(errorCode, errorDesc);
        }
    }
}