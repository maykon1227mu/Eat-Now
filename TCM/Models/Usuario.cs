namespace TCM.Models
{
    public class Usuario
    {
        public int CodUsu { get; set; }
        public string? Nome { get; set; }
        public string? email { get; set; }
        public string? usuario { get; set; }
        public string? senha { get; set; }
        public DateOnly DataNascimento { get; set; }
        public string? CPF { get; set; }
        public byte[]? FotoPerfil { get; set; }
        //ImagemBase64 serve para que depois a imagem que está salva em blob possa ser interpretada para ser mostrada
        public string? FotoPerfilBase64 { get; set; }
        public string? tipo { get; set; }
    }
}
