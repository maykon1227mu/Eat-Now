namespace TCM.Models
{
    public class Produto
    {
        public int CodProd {  get; set; }
        public string? NomeProd { get; set; }
        public string? Descricao { get; set; }
        public decimal? Preco { get; set; }
        public int? Qtd { get; set; }
        public int? UserId { get; set; }
        public string? Usuario { get; set; }
        public int Avaliacoes { get; set; }
        public double Nota { get; set; }
        public byte[]? Imagem { get; set; }
        //ImagemBase64 serve para que depois a imagem que está salva em blob possa ser interpretada para ser mostrada
        public string? ImagemBase64 { get; set; }
        public int CategoriaId { get; set; }
        public string? NomeCategoria { get; set; }
        public int QuantidadeDoPedido { get; set; }
    }
}
