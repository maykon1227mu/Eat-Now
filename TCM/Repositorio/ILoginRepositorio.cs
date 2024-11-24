using TCM.Models;

namespace TCM.Repositorio
{
    public interface ILoginRepositorio
    {
        Task<Usuario> Login(string usuario, string senha);
        Task<Funcionario> LoginFuncionario(string usuario, string senha);
        Task<Fornecedor> LoginFornecedor(string usuario, string senha);
        Task Logout();
        Usuario AcharUsuario(int id);
        IEnumerable<Funcionario> TodosFuncionarios();
        IEnumerable<Fornecedor> TodosFornecedores();
        void DeletarUsuario(int id);
        void EditarUsuario(Usuario user);
        void Cadastrar(string nome, string email, string usuario, string senha);
        void CadastrarFuncionario(string nome, string email, string usuario, string senha, decimal salario);
        void CadastrarFornecedor(string email, string usuario, string senha, string cnpj);

    }
}
