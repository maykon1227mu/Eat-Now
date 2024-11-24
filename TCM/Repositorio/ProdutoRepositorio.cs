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

                MySqlCommand cmd = new MySqlCommand("insert into tbproduto (NomeProd, Descricao, Preco, Qtd, UserId, Imagem) values (@nomeprod, @descricao, @preco, @qtd, @userid, @imagem)", conexao);

                cmd.Parameters.Add("@nomeprod", MySqlDbType.VarChar).Value = produto.NomeProd;
                cmd.Parameters.Add("@descricao",MySqlDbType.VarChar).Value = produto.Descricao;
                cmd.Parameters.Add("@preco",MySqlDbType.Decimal).Value = produto.Preco;
                cmd.Parameters.Add("@qtd", MySqlDbType.Int32).Value = produto.Qtd;
                cmd.Parameters.Add("@userid", MySqlDbType.Int32).Value = produto.UserId;
                cmd.Parameters.Add("@imagem", MySqlDbType.Blob).Value = produto.Imagem;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public void EditarProduto(Produto produto)
        {
            
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("update tbproduto set NomeProd = @nomeprod, Descricao = @descricao, Preco = @preco, Qtd = @qtd, Imagem = @imagem where CodProd = @codprod", conexao);

                cmd.Parameters.Add("@nomeprod", MySqlDbType.VarChar).Value = produto.NomeProd;
                cmd.Parameters.Add("@descricao", MySqlDbType.VarChar).Value = produto.Descricao;
                cmd.Parameters.Add("@preco", MySqlDbType.Decimal).Value = produto.Preco;
                cmd.Parameters.Add("@qtd", MySqlDbType.Int32).Value = produto.Qtd;
                cmd.Parameters.Add("@imagem", MySqlDbType.Blob).Value = produto.Imagem;
                cmd.Parameters.Add("@codprod", MySqlDbType.Int32).Value = produto.CodProd;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public void DeletarProduto(int id)
        {
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("delete from tbproduto where CodProd = @codprod", conexao);

                cmd.Parameters.Add("@codprod", MySqlDbType.Int32).Value = id;

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

                MySqlCommand cmd = new MySqlCommand("select CodProd, NomeProd, Descricao, Preco, Qtd, Usuario, Imagem from tbproduto join tbfornecedor on tbproduto.UserId = tbfornecedor.CodFor where CodProd = @codprod;", conexao);

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
                        Qtd = Convert.ToInt32(dr["qtd"]),
                        Usuario = Convert.ToString(dr["usuario"]),
                        Imagem = (byte[])dr["imagem"],
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
                            Qtd = Convert.ToInt32(dr["qtd"]),
                            UserId = Convert.ToInt32(dr["userid"]),
                            Imagem = (byte[])dr["imagem"],
                        });
                }
                return Produtoslist;
            }
        }

        public IEnumerable<Produto> Pesquisa(string nome)
        {
            //Criando a lista que irá receber todos os produtos[
            List<Produto> Produtoslist = new List<Produto>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();
                //Criando o comando para listar todos os clientes
                MySqlCommand cmd = new MySqlCommand("select * from tbproduto where NomeProd like @nome", conexao);

                string nsei = nome + "%";

                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = nsei;

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
                            Qtd = Convert.ToInt32(dr["qtd"]),
                            UserId = Convert.ToInt32(dr["userid"]),
                            Imagem = ((byte[])dr["imagem"]),
                        });
                }
                return Produtoslist;
            }
        }

        public void FinalizarCompra(int userId, int produtoId, int qtd)
        {
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("insert into tbpedido (ProdutoId, UserId, QtdPed) values (@produtoId, @userId, @qtd)", conexao);

                cmd.Parameters.Add("@produtoId", MySqlDbType.Int32).Value = produtoId;
                cmd.Parameters.Add("@userId", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("@qtd", MySqlDbType.Int32).Value = qtd;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public IEnumerable<Pedido> TodosPedidos(int userId)
        {
            List<Pedido> pedidoLista = new List<Pedido>();
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select tbpedido.CodPed, tbpedido.ProdutoId, tbpedido.UserId, tbpedido.DataPed, tbproduto.NomeProd, tbproduto.Preco, tbpedido.QtdPed from tbpedido join tbproduto on tbpedido.ProdutoId = tbproduto.CodProd where tbpedido.UserId = @userId", conexao);
                
                cmd.Parameters.Add("@userId", MySqlDbType.Int32).Value = userId;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    pedidoLista.Add(
                        new Pedido()
                        {
                            CodPed = Convert.ToInt32(dr["codped"]),
                            ProdutoId = Convert.ToInt32(dr["produtoid"]),
                            UserId = Convert.ToInt32(dr["userid"]),
                            DataPed = Convert.ToDateTime(dr["dataped"]),
                            NomeProd = Convert.ToString(dr["nomeprod"]),
                            PrecoProd = Convert.ToDecimal(dr["preco"]),
                            QtdPed = Convert.ToInt32(dr["qtdped"]),
                        });
                }
            }
            return pedidoLista;
        }
    }
}
