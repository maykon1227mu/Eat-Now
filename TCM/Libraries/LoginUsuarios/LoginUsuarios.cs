using Newtonsoft.Json;
using TCM.Models;

namespace TCM.Libraries.LoginUsuarios
{
    public class LoginUsuarios
    {
        //Injeção de dependencia
        private string Key = "Login.Usuario";
        private Sessao.Sessao _sessao;

        //Construtor
        public LoginUsuarios(Sessao.Sessao sessao)
        {
            _sessao = sessao;
        }

        public void Login(Usuario usuarios)
        {
            //Serializar - Com a serialização é possivel salvar objetos em arquivos de dados
            string usuarioJSONString = JsonConvert.SerializeObject(usuarios);
        }

        public Usuario GetUsuarios()
        {
            /*Desserializar - A Desserialização permite que os objetos persistidos em arquivos possam ser recuperados e seus valores recriados na memória*/
            if (_sessao.Existe(Key))
            {
                string usuarioJSONString = _sessao.Consultar(Key);
                return JsonConvert.DeserializeObject<Usuario>(usuarioJSONString);
            }
            else
            {
                return null;
            }
        }

        public void Logout()
        {
            _sessao.RemoverTodos();
        }
    }
}
