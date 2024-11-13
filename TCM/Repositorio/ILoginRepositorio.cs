using TCM.Models;

namespace TCM.Repositorio
{
    public interface ILoginRepositorio
    {
        Task<Usuario> Login(string usuario, string senha);

        Task Logout();

        Usuario AcharUsuario(int id);

        void DeletarUsuario(int id);

        void EditarUsuario(Usuario usuario);

        void Cadastrar(string email, string usuario, string senha);

    }
}
