namespace TCM.Models
{
    public class Administrador
    {
        public int IdAdmin { get; set; }
        public string? Nome { get; set; }
        public string? email { get; set; }
        public string? usuario { get; set; }
        public string? senha { get; set; }
        public DateOnly DataAdmissao { get; set; }
        public string? Estado { get; set; }
        public string? tipo { get; set; }
    }
}
