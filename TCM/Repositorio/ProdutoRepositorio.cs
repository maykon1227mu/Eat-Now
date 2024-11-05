using MySql.Data.MySqlClient;
using System.Data;
using TCM.Models;

namespace TCM.Repositorio
{
    public class ProdutoRepositorio : IProdutoRepositorio
    {
        //Declarando a variavel de string de conexão
        private readonly string? _conexaoMySQL;

        //Metodo da conexão com banco de dados
        public ProdutoRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

        //Método Adicionar Produto
        public void AdicionarProduto(Produto produto)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("insert into tbproduto (NomeProd, Descricao, Preco) values (@nomeprod, @descricao, @preco)", conexao);

                cmd.Parameters.Add("@nomeprod", MySqlDbType.VarChar).Value = produto.NomeProd;
                cmd.Parameters.Add("@descricao",MySqlDbType.VarChar).Value = produto.Descricao;
                cmd.Parameters.Add("@preco",MySqlDbType.Decimal).Value = produto.Preco;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public void EditarProduto(Produto produto)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("update tbproduto set NomeProd = @nomeprod, Descricao = @descricao, Preco = @preco where CodProd = @codprod", conexao);

                cmd.Parameters.Add("@nomeprod", MySqlDbType.VarChar).Value = produto.NomeProd;
                cmd.Parameters.Add("@descricao", MySqlDbType.VarChar).Value = produto.Descricao;
                cmd.Parameters.Add("@preco", MySqlDbType.Decimal).Value = produto.Preco;
                cmd.Parameters.Add("@codprod", MySqlDbType.Int32).Value = produto.CodProd;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public void DeletarProduto(Produto produto)
        {
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("delete from tbproduto where CodProd = @codprod", conexao);

                cmd.Parameters.Add("@codprod", MySqlDbType.Int32).Value = produto.CodProd;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public Produto AcharProduto(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                Produto produto = new Produto();
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select * from tbproduto where CodProd = @codprod", conexao);

                cmd.Parameters.Add("@codprod", MySqlDbType.Int32).Value = id;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    produto = new Produto() 
                    {
                        CodProd = Convert.ToInt32(dr["codprod"]),
                        NomeProd = ((string)dr["nomeprod"]),
                        Descricao = ((string)dr["descricao"]),
                        Preco = Convert.ToDecimal(dr["preco"]),
                    };
                }

                return produto;
            }
        }

        public IEnumerable<Produto> TodosProdutos()
        {
            //Criando a lista que irá receber todos os produtos[
            List<Produto> Produtoslist = new List<Produto>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();
                //Criando o comando para listar todos os clientes
                MySqlCommand cmd = new MySqlCommand("select * from tbproduto", conexao);

                //Traz a tabela
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Cria a copia da tabela
                DataTable dt = new DataTable();

                //Separa e preenche os dados
                da.Fill(dt);

                //Fechando a conexão com o banco de dados
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    Produtoslist.Add(
                        new Produto()
                        {
                            CodProd = Convert.ToInt32(dr["codprod"]),
                            NomeProd = ((string)dr["nomeprod"]),
                            Descricao = ((string)dr["descricao"]),
                            Preco = Convert.ToDecimal(dr["preco"]),
                        });
                }
                return Produtoslist;
            }
        }
    }
}
