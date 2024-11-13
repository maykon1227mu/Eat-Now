using TCM.Models;

namespace TCM.Repositorio
{
    public interface IProdutoRepositorio
    {
        //Adiciona Produto
        void AdicionarProduto(Produto produto);
        //Edita o produto
        void EditarProduto(Produto produto);

        //Deleta o produto
        void DeletarProduto(int id);

        Produto AcharProduto(int id);
        //Buscar todos os produtos
        IEnumerable<Produto> TodosProdutos();

        IEnumerable<Produto> Pesquisa(string nome);
    }
}
