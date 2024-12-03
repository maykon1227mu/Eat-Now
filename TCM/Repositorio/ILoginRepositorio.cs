using TCM.Models;

namespace TCM.Repositorio
{
    public interface ILoginRepositorio
    {
        Task<dynamic> Login(string usuario, string senha);
        Task Logout();
        Usuario AcharUsuario(int id);
        Fornecedor AcharFornecedor(int id);
        IEnumerable<Funcionario> TodosFuncionarios();
        IEnumerable<Fornecedor> TodosFornecedores();
        void DeletarUsuario(int id);
        void EditarUsuario(Usuario user);
        void Cadastrar(string nome, string email, string usuario, string senha);
        void CadastrarFuncionario(string nome, string email, string usuario, string senha, decimal salario);
        void CadastrarFornecedor(string email, string usuario, string senha, string cnpj);

    }
}
