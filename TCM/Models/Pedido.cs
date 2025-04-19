namespace TCM.Models
{
    public class Pedido
    {
        public int CodPed { get; set; }
        public decimal PrecoPed { get; set; }
        public int UserId { get; set; }
        public int IdEndereco { get; set; }
        public int QtdPed { get; set; }
        public DateTime DataPed { get; set; }
        public decimal Total => PrecoPed * QtdPed;
    }
}
