namespace TCM.Libraries.Sessao
{
    public class Sessao
    {
        //Interface com um biblioteca para manipular a sessão

        /* A interface IHttpContextAccessor é usada para acessar o contexto HTTP em . NET, e por
          utilizar injeção de dependência, é extremamente útil quando precisamos de 
          alguma informação em camadas de serviço*/


        IHttpContextAccessor _context;
        public Sessao(IHttpContextAccessor context)
        {
            _context = context;
        }


        //Consultar sessão
        public string Consultar(string Key)
        {
            return _context.HttpContext.Session.GetString(Key);
        }


        public bool Existe(string Key)
        {
            /*  HttpContext- Items pode ser usada para armazenar dados durante o processamento de uma única solicitação.
             * O conteúdo da coleção é descartado após o processamento de uma solicitação*/
            if (_context.HttpContext.Session.GetString(Key) == null)
            {
                return false;
            }

            return true;
        }

        //Remover todas as sessões
        public void RemoverTodos()
        {
            _context.HttpContext.Session.Clear();
        }
    }
}