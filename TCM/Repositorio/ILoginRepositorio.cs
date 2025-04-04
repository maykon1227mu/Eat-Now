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
        void Cadastrar(string nome, string email, string usuario, string senha, byte[] fotoPerfil, DateOnly data, string cpf);
        void CadastrarFuncionario(string nome, string email, string usuario, string senha, decimal salario, int userid);
        void CadastrarFornecedor(string nome, string email, string usuario, string senha, string cnpj);
        void CadastrarAdministrador(string nome, string email, string usuario, string senha, byte[] fotoPerfil);

    }
}
