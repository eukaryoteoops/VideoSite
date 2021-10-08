namespace Comic.BackOffice.Commands.Product
{
    public class UpdateProductState
    {
        public int Id { get; set; }
        /// <summary>
        /// 0 : 下架
        /// 1 : 上架
        /// 2 : 封存
        /// </summary>
        public byte State { get; set; }
    }
}
