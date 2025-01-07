using MySql.Data.MySqlClient;
using System.Data;
using TCM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace TCM.Repositorio
{
    public class LoginRepositorio : ILoginRepositorio
    {
        //Declarando a variavel de string de conexão com o banco de dados
        private readonly string? _conexaoMySQL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginRepositorio(IConfiguration conf, IHttpContextAccessor httpContextAccessor)
        {
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
            _httpContextAccessor = httpContextAccessor; 
        }

        public async Task<dynamic> Login(string usuario, string senha)
        {
            Usuario usuarioNaN = new Usuario();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("call spLogin(@usuario, @senha)", conexao);
                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = senha;
                

                MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                
                if (dr.Read())
                {
                    string tipo = Convert.ToString(dr["tipo"]);
                    if (tipo == "Cliente")
                    {
                        Usuario user = new Usuario();

                            // Atribuindo dados ao usuário, se encontrados no banco
                            user.usuario = Convert.ToString(dr["usuario"]);
                            user.senha = Convert.ToString(dr["senha"]);
                            user.CodUsu = Convert.ToInt32(dr["codusu"]);
                            user.tipo = Convert.ToString(dr["tipo"]);



                            // Criando a lista de claims
                            //Claims são um tipo de identificadores do usuario
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.usuario),
                                new Claim(ClaimTypes.SerialNumber, Convert.ToString(user.CodUsu)),
                                new Claim(ClaimTypes.Role, user.tipo == null ? "Cliente" : user.tipo)
                            };

                            //Criando o Claim de identidade do usuario, juntamente de coockies
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            //Permite que o usuario continue logado mesmo se fechar o navegador
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true // Mantém o cookie ao fechar o navegador
                            };
                            //Vai logar o usuario com o HTTP usando tanto os coockies quanto a identidade do usuario
                            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        
                        return user;
                    }
                    else if (tipo == "Fornecedor" || tipo == "Administrador")
                    {
                        Fornecedor fornecedor = new Fornecedor();
                        // Atribuindo dados ao usuário, se encontrados no banco
                        fornecedor.usuario = Convert.ToString(dr["usuario"]);
                        fornecedor.senha = Convert.ToString(dr["senha"]);
                        fornecedor.CodFor = Convert.ToInt32(dr["codfor"]);
                        fornecedor.CNPJ = Convert.ToString(dr["cnpj"]);
                        fornecedor.tipo = Convert.ToString(dr["tipo"]);



                        // Criando a lista de claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, fornecedor.usuario),
                            new Claim(ClaimTypes.SerialNumber, Convert.ToString(fornecedor.CodFor)),
                            new Claim(ClaimTypes.Role, fornecedor.tipo == null ? "Fornecedor" : fornecedor.tipo)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true // Mantém o cookie ao fechar o navegador
                        };

                        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        return fornecedor;
                    }
                    else if (tipo == "Funcionario")
                    {
                        Funcionario funcionario = new Funcionario();

                        
                            // Atribuindo dados ao usuário, se encontrados no banco
                            funcionario.usuario = Convert.ToString(dr["usuario"]);
                            funcionario.senha = Convert.ToString(dr["senha"]);
                            funcionario.CodFunc = Convert.ToInt32(dr["codfunc"]);
                            funcionario.Salario = Convert.ToInt32(dr["salario"]);
                            funcionario.tipo = Convert.ToString(dr["tipo"]);



                            // Criando a lista de claims
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, funcionario.usuario),
                                new Claim(ClaimTypes.SerialNumber, Convert.ToString(funcionario.CodFunc)),
                                new Claim(ClaimTypes.Role, funcionario.tipo == null ? "Fornecedor" : funcionario.tipo)
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true // Mantém o cookie ao fechar o navegador
                            };

                            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        
                        return funcionario;
                    }
                } else
                {
                    return usuarioNaN;
                }
            } 
            return usuarioNaN;
        }

        public IEnumerable<Funcionario> TodosFuncionarios()
        {
            List<Funcionario> funcionarioLista = new List<Funcionario>();
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from tbfuncionario", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach(DataRow dr in dt.Rows)
                {


                    funcionarioLista.Add(
                        new Funcionario
                        {
                            CodFunc = Convert.ToInt32(dr["codfunc"]),
                            email = Convert.ToString(dr["email"]),
                            usuario = Convert.ToString(dr["usuario"]),
                            senha = Convert.ToString(dr["senha"]),
                            Salario = Convert.ToDecimal(dr["salario"])
                        });
                }
            }
            return funcionarioLista;
        }

        public IEnumerable<Fornecedor> TodosFornecedores()
        {
            List<Fornecedor> fornecedorLista = new List<Fornecedor>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from tbfornecedor where tipo = 'Fornecedor'", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {


                    fornecedorLista.Add(
                        new Fornecedor
                        {
                            CodFor = Convert.ToInt32(dr["codfor"]),
                            email = Convert.ToString(dr["email"]),
                            usuario = Convert.ToString(dr["usuario"]),
                            senha = Convert.ToString(dr["senha"]),
                            CNPJ = Convert.ToString(dr["cnpj"]),
                        });
                }
            }
            return fornecedorLista;
        }

        public void EditarUsuario(Usuario user)
        {
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("update tbusuario set usuario = @usuario, email = @email where CodUsu = @codusu", conexao);

                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = user.usuario;
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = user.email;
                cmd.Parameters.Add("@codusu", MySqlDbType.Int32).Value = user.CodUsu;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public void DeletarUsuario(int id)
        {
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("delete from tbusuario where CodUsu = @codusu", conexao);

                cmd.Parameters.Add("@codusu", MySqlDbType.Int32 ).Value = id;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public Usuario AcharUsuario(int id)
        {
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                Usuario user = new Usuario();
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select * from tbusuario where CodUsu = @codusu", conexao);

                cmd.Parameters.Add("@codusu", MySqlDbType.Int32).Value = id;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    user = new Usuario()
                    {
                        CodUsu = Convert.ToInt32(dr["codusu"]),
                        Nome = (string)dr["nome"],
                        usuario = ((string)dr["usuario"]),
                        email = ((string)dr["email"]),
                        senha = (string)dr["senha"],
                        tipo = (string)dr["tipo"],
                    };
                }
                return user;
            }
        }

        public Fornecedor AcharFornecedor(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                Fornecedor user = new Fornecedor();
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select * from tbfornecedor where CodFor = @codfor", conexao);

                cmd.Parameters.Add("@codfor", MySqlDbType.Int32).Value = id;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    user = new Fornecedor()
                    {
                        CodFor = Convert.ToInt32(dr["codusu"]),
                        usuario = ((string)dr["usuario"]),
                        email = ((string)dr["email"]),
                        CNPJ = (string)dr["cnpj"],
                        tipo = (string)dr["tipo"],
                    };
                }
                return user;
            }
        }

        public async Task Logout()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }


        //Metodo de cadastro
        public void Cadastrar(string nome, string email, string usuario, string senha)
        {
            //Instanciando a variavel de conexão
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();

                //Variavel cmd que recebe o comando insert do banco de dados inserindo o usuario e senha
                MySqlCommand cmd = new MySqlCommand("insert into tbusuario (nome, email, usuario, senha) values (@nome, @email, @usuario, @senha)", conexao);

                //Adicionando os parametros email, usuario e senha
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = nome;
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = senha;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void CadastrarFuncionario(string nome, string email, string usuario, string senha, decimal salario)
        {
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("insert into tbfuncionario (Nome, Email, Usuario, Senha, Salario) values (@nome, @email, @usuario, @senha, @salario)", conexao);
                
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = nome;
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@senha", MySqlDbType.VarString).Value = senha;
                cmd.Parameters.Add("@salario", MySqlDbType.Decimal).Value = salario;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void CadastrarFornecedor(string email, string usuario, string senha, string cnpj)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("insert into tbfornecedor (Email, Usuario, Senha, CNPJ) values (@email, @usuario, @senha, @cnpj)", conexao);

                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = senha;
                cmd.Parameters.Add("@cnpj", MySqlDbType.VarChar).Value = cnpj;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
    }
}