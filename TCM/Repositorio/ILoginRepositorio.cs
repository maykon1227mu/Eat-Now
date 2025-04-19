using TCM.Models;

namespace TCM.Repositorio
{
    public interface ILoginRepositorio
    {
        Task<dynamic> Login(string usuario, string senha);
        Task Logout();
        Usuario AcharUsuario(int id);
        Colaborador AcharColaborador(int id);
        IEnumerable<Administrador> TodosAdministradores();
        Administrador AcharAdministrador(int id);
        IEnumerable<Colaborador> TodosColaboradores();
        void DeletarUsuario(int id);
        void EditarUsuario(Usuario user);
        void Cadastrar(string nome, string email, string usuario, string senha, byte[] fotoPerfil, DateOnly data, string cpf);
        void CadastrarAdministrador(string nome, string email, string usuario, string senha);
        void CadastrarColaborador(string nome, string email, string usuario, string senha, string cnpj);
    }
}
