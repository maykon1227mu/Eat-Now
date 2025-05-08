namespace TCM.Models
{
    public class Categoria
    {
        public int CodCat { get; set; }
        public string? Nome { get; set; }

        public int CategoriaId { get; set; }

        // Adicione esta propriedade se ainda não existir:
        public virtual ICollection<Produto> Produtos { get; set; }


    }
}
