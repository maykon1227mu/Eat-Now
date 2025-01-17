namespace TCM.Models
{
    public class Promocao
    {
        public int PromoId { get; set; }
        public string? NomePromo { get; set; }
        public int Porcentagem { get; set; }
        public DateTime Data_Exclusao { get; set; }
        public string? categoria { get; set; }
    }
}
