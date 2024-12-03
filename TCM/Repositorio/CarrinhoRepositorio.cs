using MySql.Data.MySqlClient;
using System.Data;
using TCM.Models;
namespace TCM.Repositorio
{
    public class CarrinhoRepositorio : ICarrinhoRepositorio
    {
        private readonly string? _conexaoMySQL;

        public CarrinhoRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

        public void SalvarItemCarrinho(int userId, Produto item, int qtd)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                var cmd = new MySqlCommand(
                    "call spInserirCarrinho(@userId, @produtoId, @quantidade)", conexao);

                cmd.Parameters.Add("@userId", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("@produtoId", MySqlDbType.Int32).Value = item.CodProd;
                cmd.Parameters.Add("@quantidade", MySqlDbType.Int32).Value = qtd;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public IEnumerable<Carrinho> ObterCarrinhoPorUsuario(int userId)
        {
            List<Carrinho> carrinho = new List<Carrinho>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                var cmd = new MySqlCommand("select tbcarrinho.ProdutoId, tbcarrinho.Quantidade, tbproduto.NomeProd, tbproduto.Imagem, tbproduto.Preco from tbcarrinho join tbproduto on tbcarrinho.ProdutoId = tbproduto.CodProd where tbcarrinho.UserId = @userId", conexao);

                cmd.Parameters.Add("@userId", MySqlDbType.Int32).Value = userId;
                
                //Traz a tabela
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Cria a copia da tabela
                DataTable dt = new DataTable();

                //Separa e preenche os dados
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    carrinho.Add(new Carrinho
                    {
                        ProdutoId = Convert.ToInt32(dr["ProdutoId"]),
                        NomeProduto = dr["NomeProd"].ToString(),
                        PrecoProduto = Convert.ToDecimal(dr["Preco"]),
                        ImagemProd = (byte[])dr["imagem"],
                        Quantidade = Convert.ToInt32(dr["Quantidade"]),
                    });
                }
                

                /*
                Outras formas de fazer!!
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    carrinho.Add(new CarrinhoItem
                    {
                        ProdutoId = Convert.ToInt32(dr["ProdutoId"]),
                        NomeProduto = dr["NomeProd"].ToString(),
                        PrecoProduto = Convert.ToDecimal(dr["Preco"]),
                        Quantidade = Convert.ToInt32(dr["Quantidade"])
                    });
                }
                */

                /*
                 using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        carrinho.Add(new CarrinhoItem
                        {
                            ProdutoId = Convert.ToInt32(reader["ProdutoId"]),
                            NomeProduto = reader["NomeProd"].ToString(),
                            PrecoProduto = Convert.ToDecimal(reader["Preco"]),
                            Quantidade = Convert.ToInt32(reader["Quantidade"])
                        });
                    }
                }
                 */
            }
            return carrinho;
        }

        public void RemoverItemCarrinho(int userId, int produtoId, int qtd)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                var cmd = new MySqlCommand("call spExcluirDoCarrinho(@userId, @produtoId, @qtd); call spZerosCarrinho();", conexao);

                cmd.Parameters.Add("@userId", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("@produtoId", MySqlDbType.Int32).Value = produtoId;
                cmd.Parameters.Add("@qtd", MySqlDbType.Int32).Value = qtd;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
    }
}
