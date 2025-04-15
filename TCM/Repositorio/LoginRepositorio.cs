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
                    else if (tipo == "Colaborador")
                    {
                        Colaborador colaborador = new Colaborador();
                        // Atribuindo dados ao usuário, se encontrados no banco
                        colaborador.usuario = Convert.ToString(dr["usuario"]);
                        colaborador.senha = Convert.ToString(dr["senha"]);
                        colaborador.IdColaborador = Convert.ToInt32(dr["codfor"]);
                        colaborador.CNPJ = Convert.ToString(dr["cnpj"]);
                        colaborador.tipo = Convert.ToString(dr["tipo"]);



                        // Criando a lista de claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, colaborador.usuario),
                            new Claim(ClaimTypes.SerialNumber, Convert.ToString(colaborador.IdColaborador)),
                            new Claim(ClaimTypes.Role, colaborador.tipo == null ? "Colaborador" : colaborador.tipo)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true // Mantém o cookie ao fechar o navegador
                        };

                        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        return colaborador;
                    }
                    else if (tipo == "Administrador")
                    {
                        Administrador administrador = new Administrador();

                        
                            // Atribuindo dados ao usuário, se encontrados no banco
                            administrador.usuario = Convert.ToString(dr["usuario"]);
                            administrador.senha = Convert.ToString(dr["senha"]);
                            administrador.IdAdmin = Convert.ToInt32(dr["codfunc"]);
                            administrador.Salario = Convert.ToInt32(dr["salario"]);
                            administrador.tipo = Convert.ToString(dr["tipo"]);



                            // Criando a lista de claims
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, administrador.usuario),
                                new Claim(ClaimTypes.SerialNumber, Convert.ToString(administrador.IdAdmin)),
                                new Claim(ClaimTypes.Role, administrador.tipo == null ? "Administrador" : administrador.tipo)
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true // Mantém o cookie ao fechar o navegador
                            };

                            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        
                        return administrador;
                    }
                } else
                {
                    return usuarioNaN;
                }
            } 
            return usuarioNaN;
        }

        public IEnumerable<Administrador> TodosAdministradores()
        {
            List<Administrador> adminLista = new List<Administrador>();
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                //Lembrar de fazer o select na tbfuncionario para depois fazer os joins
                MySqlCommand cmd = new MySqlCommand("SELECT tbfuncionario.CodFunc, tbfuncionario.Salario, tbusuario.Nome AS Funcionario, tbusuario.Email AS EmailFuncionario, tbusuario2.Usuario AS Contratante, tbusuario2.Email AS EmailContratante FROM tbfuncionario JOIN tbusuario ON tbfuncionario.CodFunc = tbusuario.CodUsu JOIN tbusuario AS tbusuario2 ON tbfuncionario.UserId = tbusuario2.CodUsu", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach(DataRow dr in dt.Rows)
                {


                    adminLista.Add(
                        new Administrador
                        {
                            IdAdmin = Convert.ToInt32(dr["codfunc"]),
                            email = Convert.ToString(dr["EmailFuncionario"]),
                            Nome = Convert.ToString(dr["Funcionario"]),
                            Salario = Convert.ToDecimal(dr["salario"]),
                        });
                }
            }
            return adminLista;
        }
        public Administrador AcharAdministrador(int id)
        {
            Administrador administrador = new Administrador();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT CodUsu, Nome, Email, Usuario, Senha, Tipo, tbfuncionario.CodFunc,tbFuncionario.Salario, tbFuncionario.UserId, tbfuncionario.DataNasc, tbfuncionario.CPF FROM tbusuario join tbfuncionario on tbfuncionario.CodFunc = tbusuario.CodUsu WHERE CodUsu = @userid", conexao);
                cmd.Parameters.Add("@userid", MySqlDbType.Int32).Value = id;
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    administrador = new Administrador
                    {
                        IdAdmin = Convert.ToInt32(dr["codfunc"]),
                        Nome = Convert.ToString(dr["Nome"]),
                        email = Convert.ToString(dr["Email"]),
                        usuario = Convert.ToString(dr["Usuario"]),
                        senha = Convert.ToString(dr["Senha"]),
                        Salario = Convert.ToDecimal(dr["Salario"]),
                        DataNasc = DateOnly.FromDateTime((DateTime)dr["DataNasc"]),

                    };
                }
                return administrador;
            }
        }


        public IEnumerable<Colaborador> TodosColaboradores()
        {
            List<Colaborador> colaboradorLista = new List<Colaborador>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from tbusuario join tbfornecedor where tipo = 'Fornecedor'", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {


                    colaboradorLista.Add(
                        new Colaborador
                        {
                            IdColaborador = Convert.ToInt32(dr["codfor"]),
                            email = Convert.ToString(dr["email"]),
                            usuario = Convert.ToString(dr["usuario"]),
                            senha = Convert.ToString(dr["senha"]),
                            CNPJ = Convert.ToString(dr["cnpj"]),
                        });
                }
            }
            return colaboradorLista;
        }

        public void EditarUsuario(Usuario user)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("update tbusuario set usuario = @usuario, email = @email, FotoPerfil = @foto where CodUsu = @codusu", conexao);

                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = user.usuario;
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = user.email;
                cmd.Parameters.Add("@foto", MySqlDbType.MediumBlob).Value = user.FotoPerfil;
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

                MySqlCommand cmd = new MySqlCommand("select * from tbusuario join tbcliente where CodUsu = @codusu", conexao);

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
                        FotoPerfil = dr["FotoPerfil"] != DBNull.Value ? (byte[])dr["FotoPerfil"] : null,
                        CPF = (string)dr["CPF"],
                        DataNascimento = DateOnly.FromDateTime((DateTime)dr["DataNasc"]),
                        tipo = (string)dr["tipo"],
                    };
                }
                return user;
            }
        }

        public Colaborador AcharColaborador(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                Colaborador user = new Colaborador();
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select tbusuario.CodUsu, tbusuario.Nome, tbusuario.Usuario, tbusuario.Email, tbfornecedor.CNPJ, tbusuario.tipo from tbusuario join tbfornecedor on tbfornecedor.CodFor = tbusuario.CodUsu where CodUsu = @codfor", conexao);

                cmd.Parameters.Add("@codfor", MySqlDbType.Int32).Value = id;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    user = new Colaborador()
                    {
                        IdColaborador = Convert.ToInt32(dr["codusu"]),
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
        public void Cadastrar(string nome, string email, string usuario, string senha, byte[] fotoPerfil, DateOnly data, string cpf)
        {
            //Instanciando a variavel de conexão
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();

                //Variavel cmd que recebe o comando insert do banco de dados inserindo o usuario e senha
                MySqlCommand cmd = new MySqlCommand("call spCadastrarUsuario(@nome, @email, @usuario, @senha, @foto, @data, @cpf)", conexao);

                //Adicionando os parametros email, usuario e senha
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = nome;
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = senha;
                cmd.Parameters.Add("@foto", MySqlDbType.Blob).Value = fotoPerfil;
                cmd.Parameters.Add("@data", MySqlDbType.Date).Value = data;
                cmd.Parameters.Add("@cpf", MySqlDbType.VarChar).Value = cpf;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void CadastrarAdministrador(string nome, string email, string usuario, string senha, decimal salario, DateOnly data)
        {
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("call spCadastrarAdministrador(@nome, @email, @usuario, @senha, null, @salario, @data)", conexao);
                
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = nome;
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@senha", MySqlDbType.VarString).Value = senha;
                cmd.Parameters.Add("@salario", MySqlDbType.Decimal).Value = salario;
                cmd.Parameters.Add("@data", MySqlDbType.Int32).Value = data;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void CadastrarColaborador(string nome, string email, string usuario, string senha, string cnpj)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("call spCadastrarColaborador(@email, @nome, @usuario, @senha, @cnpj)", conexao);

                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = nome;
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