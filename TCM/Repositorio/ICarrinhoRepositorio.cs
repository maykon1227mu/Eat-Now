using TCM.Models;

namespace TCM.Repositorio
{
    public interface ICarrinhoRepositorio
    {
        void SalvarItemCarrinho(int userId, Produto item, int qtd);
        IEnumerable<CarrinhoItem> ObterCarrinhoPorUsuario(int userId);
        void RemoverItemCarrinho(int userId, int produtoId, int qtd);
    }
}
