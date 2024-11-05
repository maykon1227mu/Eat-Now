using MySql.Data.MySqlClient;
using System.Data;
using TCM.Models;

namespace TCM.Repositorio
{
    public class LoginRepositorio : ILoginRepositorio
    {
        //Declarando a variavel de string de conexão com o banco de dados
        private readonly string? _conexaoMySQL;

        //Metodo da conexão com o banco de dados

        public LoginRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

        public Usuario Login(string usuario, string senha)
        {
            //Instanciando a variavel conexao
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();

                //Variavel cmd recebe o select do banco de dados trazendo o usuario e a senha
                MySqlCommand cmd = new MySqlCommand("select * from tbusuario where usuario = @usuario and senha = @senha", conexao);

                //Pegando os parametros do usuario e senha do banco
                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = senha;

                //Le os dados usuario e senha
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Guarda os dados usuario e senha lidos
                MySqlDataReader dr;

                //Instanciando a model Usuario
                Usuario user = new Usuario();

                //Executando os comandos do MySql para a variavel dr
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                //Verifica todos os usuarios que foram pegos e lidos no banco

                while (dr.Read())
                {
                    user.usuario = Convert.ToString(dr["usuario"]);
                    user.senha = Convert.ToString(dr["senha"]);
                }
                //Retorna para o usuario
                return user;
            }

        }

        //Metodo de cadastro
        public void Cadastrar(string email, string usuario, string senha)
        {
            //Instanciando a variavel de conexão
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();

                //Variavel cmd que recebe o comando insert do banco de dados inserindo o usuario e senha
                MySqlCommand cmd = new MySqlCommand("insert into tbusuario (email, usuario, senha) values (@email, @usuario, @senha)", conexao);

                //Adicionando os parametros email, usuario e senha
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = senha;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
    }
}
