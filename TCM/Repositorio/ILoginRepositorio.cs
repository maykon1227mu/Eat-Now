using TCM.Models;

namespace TCM.Repositorio
{
    public interface ILoginRepositorio
    {
        Usuario Login(string usuario, string senha);

        void Cadastrar(string email, string usuario, string senha);
    }
}
