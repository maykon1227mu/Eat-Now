namespace TCM.Models
{
    public class Pedido
    {
        public int CodPed { get; set; }
        public int ProdutoId { get; set; }
        public string? NomeProd { get; set; }
        public decimal PrecoPed { get; set; }
        public int UserId { get; set; }
        public byte[]? ImagemPed { get; set; }
        public string? ImagemBase64 { get; set; }
        public int QtdPed { get; set; }
        public DateTime DataPed { get; set; }
        public decimal Total => PrecoPed * QtdPed;
    }
}
