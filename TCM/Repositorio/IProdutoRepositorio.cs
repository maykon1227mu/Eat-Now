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
        IEnumerable<Produto> TodosProdutosFornecedor(int id);
        IEnumerable<Produto> Pesquisa(string nome);
        void FinalizarCompra(int userId, int produtoId, int qtd);
        IEnumerable<Pedido> TodosPedidos(int userId);
        IEnumerable<Categoria> TodasCategorias();
        IEnumerable<Produto> ProdutosPorCategoria(int categoriaId);
    }
}
