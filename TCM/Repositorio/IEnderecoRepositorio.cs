using TCM.Models;

namespace TCM.Repositorio
{
    public interface IEnderecoRepositorio
    {
        public void AdicionarEndereco(Endereco endereco);
        public Endereco AcharEndereco(int id);
        public IEnumerable<Endereco> TodosEnderecos(int id);
        public void AlterarEndereco(Endereco endereco);
        public void ApagarEndereco(int id);
        public bool ExisteEndereco(int userId);
    }
}
