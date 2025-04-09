namespace TCM.Models
{
    public class Funcionario
    {
        public int CodFunc { get; set; }
        public string? Nome { get; set; }
        public string? email { get; set; }
        public string? usuario { get; set; }
        public string? senha { get; set; }
        public decimal Salario { get; set; }
        public string? CPF { get; set; }
        public DateOnly DataNasc { get; set; }
        public int UserId { get; set; }
        public string? Contratante { get; set; }
        public string? tipo { get; set; }
    }
}
