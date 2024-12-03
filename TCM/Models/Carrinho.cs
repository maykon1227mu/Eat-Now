namespace TCM.Models
{
    public class Carrinho
    {
        public Produto Produto { get; set; }
        public int ProdutoId { get; set; }
        public string? NomeProduto { get; set; }
        public decimal? PrecoProduto { get; set; }
        public byte[]? ImagemProd { get; set; }
        public string? ImagemBase64 { get; set; }
        public int Quantidade { get; set; }
        public decimal Total => PrecoProduto.Value * Quantidade;
    }
}
