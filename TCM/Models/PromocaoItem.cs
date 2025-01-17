namespace TCM.Models
{
    public class PromocaoItem
    {
        public int PromoItemId { get; set; }
        public int ProdutoId { get; set; }
        public int PromoId { get; set; }
        public decimal PrecoPromo { get; set; }
        public int Porcentagem { get; set; }
        public Produto Produto { get; set; }
        public Promocao Promocao { get; set; }
    }
}
