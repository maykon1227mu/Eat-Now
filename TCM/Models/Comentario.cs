namespace TCM.Models
{
    public class Comentario
    {
        public int ComentId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public byte[]? FotoPerfil { get; set; }
        public string? FotoPerfilBase64 { get; set; }
        public int ProdutoId { get; set; }
        public string? comentario { get; set; }
        public DateTime DataComent { get; set; }
        public int Avaliacao { get; set; }
    }
}
