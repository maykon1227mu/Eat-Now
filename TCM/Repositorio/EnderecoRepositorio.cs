using MySql.Data.MySqlClient;
using System.Data;
using TCM.Models;

namespace TCM.Repositorio
{
    public class EnderecoRepositorio : IEnderecoRepositorio
    {
        private readonly string? _conexaoMySQL;

        public EnderecoRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
        public void AdicionarEndereco(Endereco endereco)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO tbEndereco(Logradouro, Numero, Complemento, Bairro, Cidade, IdEstado, UserId, CEP) values (@log, @numero, @comp, @bairro, @cidade, @idest, @usrid, @cep)", conexao);
                cmd.Parameters.Add("@log", MySqlDbType.VarChar).Value = endereco.Logradouro;
                cmd.Parameters.Add("@numero", MySqlDbType.VarChar).Value = endereco.Numero;
                cmd.Parameters.Add("@comp", MySqlDbType.VarChar).Value = endereco.Complemento;
                cmd.Parameters.Add("@bairro", MySqlDbType.VarChar).Value = endereco.Bairro;
                cmd.Parameters.Add("@cidade", MySqlDbType.VarChar).Value = endereco.Cidade;
                cmd.Parameters.Add("@idest", MySqlDbType.Int32).Value = endereco.IdEstado;
                cmd.Parameters.Add("@usrid", MySqlDbType.Int32).Value = endereco.UserId;
                cmd.Parameters.Add("@cep", MySqlDbType.VarChar).Value = endereco.CEP;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public Endereco AcharEndereco(int id)
        {
            Endereco endereco = new Endereco();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT IdEndereco, Logradouro, Numero, Complemento, Bairro, Cidade, IdEstado, tbEstado.Estado, UserId, CEP FROM tbEndereco join tbEstado on tbEndereco.IdEstado = tbEstado.IdEstado WHERE IdEndereco = @idend", conexao);
                cmd.Parameters.Add("@idend", MySqlDbType.Int32).Value = id;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    endereco = new Endereco()
                    {
                        IdEndereco = Convert.ToInt32(dr["IdEndereco"]),
                        Logradouro = Convert.ToString(dr["Logradouro"]),
                        Numero = Convert.ToString(dr["Numero"]),
                        Complemento = Convert.ToString(dr["Complemento"]),
                        Bairro = Convert.ToString(dr["Bairro"]),
                        Cidade = Convert.ToString(dr["Cidade"]),
                        IdEstado = Convert.ToInt32(dr["IdEstado"]),
                        Estado = Convert.ToString(dr["Estado"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        CEP = Convert.ToString(dr["CEP"])
                    };
                }
                return endereco;
            }
        }

        public IEnumerable<Endereco> TodosEnderecos(int id)
        {
            List<Endereco> enderecos = new List<Endereco>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT IdEndereco, Logradouro, Numero, Complemento, Bairro, Cidade, tbEndereco.IdEstado, tbEstado.SiglaEstado, UserId, CEP FROM tbEndereco join tbEstado on tbEndereco.IdEstado = tbEstado.IdEstado WHERE UserId = @userid", conexao);
                cmd.Parameters.Add("@userid", MySqlDbType.Int32).Value = id;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    enderecos.Add(new Endereco()
                    {
                        IdEndereco = Convert.ToInt32(dr["IdEndereco"]),
                        Logradouro = Convert.ToString(dr["Logradouro"]),
                        Numero = Convert.ToString(dr["Numero"]),
                        Complemento = Convert.ToString(dr["Complemento"]),
                        Bairro = Convert.ToString(dr["Bairro"]),
                        Cidade = Convert.ToString(dr["Cidade"]),
                        IdEstado = Convert.ToInt32(dr["IdEstado"]),
                        Estado = Convert.ToString(dr["SiglaEstado"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        CEP = Convert.ToString(dr["CEP"])
                    });
                }
                return enderecos;
            }
        }
        public void AlterarEndereco(Endereco endereco)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE tbEndereco SET Logradouro = @log, Numero = @numero, Complemento = @comp, Bairro = @bairro, Cidade = @cidade, IdEstado = @idest, CEP = @cep WHERE IdEndereco = @idend", conexao);
                cmd.Parameters.Add("@log", MySqlDbType.VarChar).Value = endereco.Logradouro;
                cmd.Parameters.Add("@numero", MySqlDbType.VarChar).Value = endereco.Numero;
                cmd.Parameters.Add("@comp", MySqlDbType.VarChar).Value = endereco.Complemento;
                cmd.Parameters.Add("@bairro", MySqlDbType.VarChar).Value = endereco.Bairro;
                cmd.Parameters.Add("@cidade", MySqlDbType.VarChar).Value = endereco.Cidade;
                cmd.Parameters.Add("@idest", MySqlDbType.Int32).Value = endereco.IdEstado;
                cmd.Parameters.Add("@cep", MySqlDbType.VarChar).Value = endereco.CEP;
                cmd.Parameters.Add("@idend", MySqlDbType.Int32).Value = endereco.IdEndereco;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public void ApagarEndereco(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM tbEndereco WHERE IdEndereco = @idend", conexao);
                cmd.Parameters.Add("@idend", MySqlDbType.Int32 ).Value = id;
                cmd.ExecuteReader();
                
                conexao.Close();
            }
        }

        public bool ExisteEndereco(int userId)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM tbEndereco WHERE UserId = @userid", conexao);
                cmd.Parameters.Add("@userid", MySqlDbType.Int32).Value = userId;

                int enderecos = Convert.ToInt32(cmd.ExecuteScalar());

                conexao.Close();

                if (enderecos > 0) return true;
                
                return false;
            }
        }

        public IEnumerable<Estado> TodosEstados()
        {
            List<Estado> estados = new List<Estado>();
            using(var conexao =  new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tbEstado", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    estados.Add(new Estado()
                    {
                        IdEstado = Convert.ToInt32(dr["IdEstado"]),
                        SiglaEstado = Convert.ToString(dr["SiglaEstado"])
                    });
                }
                return estados;
            }
        }

    }
}
