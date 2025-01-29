using Microsoft.AspNetCore.Mvc;
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

                MySqlCommand cmd = new MySqlCommand("insert into tbproduto (NomeProd, Descricao, Preco, Qtd, UserId, CategoriaId, Imagem) values (@nomeprod, @descricao, @preco, @qtd, @userid, @categoriaid, @imagem)", conexao);

                cmd.Parameters.Add("@nomeprod", MySqlDbType.VarChar).Value = produto.NomeProd;
                cmd.Parameters.Add("@descricao", MySqlDbType.VarChar).Value = produto.Descricao;
                cmd.Parameters.Add("@preco", MySqlDbType.Decimal).Value = produto.Preco;
                cmd.Parameters.Add("@qtd", MySqlDbType.Int32).Value = produto.Qtd;
                cmd.Parameters.Add("@userid", MySqlDbType.Int32).Value = produto.UserId;
                cmd.Parameters.Add("@categoriaid", MySqlDbType.Int32).Value = produto.CategoriaId;
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

                MySqlCommand cmd = new MySqlCommand("update tbproduto set NomeProd = @nomeprod, Descricao = @descricao, Preco = @preco, Qtd = @qtd, Imagem = @imagem, CategoriaId = @categoriaid where CodProd = @codprod", conexao);

                cmd.Parameters.Add("@nomeprod", MySqlDbType.VarChar).Value = produto.NomeProd;
                cmd.Parameters.Add("@descricao", MySqlDbType.VarChar).Value = produto.Descricao;
                cmd.Parameters.Add("@preco", MySqlDbType.Decimal).Value = produto.Preco;
                cmd.Parameters.Add("@qtd", MySqlDbType.Int32).Value = produto.Qtd;
                cmd.Parameters.Add("@imagem", MySqlDbType.Blob).Value = produto.Imagem;
                cmd.Parameters.Add("@categoriaid", MySqlDbType.Int32).Value = produto.CategoriaId;
                cmd.Parameters.Add("@codprod", MySqlDbType.Int32).Value = produto.CodProd;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public void DeletarProduto(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
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

                MySqlCommand cmd = new MySqlCommand("select CodProd, NomeProd, Descricao, Preco, Qtd, Usuario, Categoria, CategoriaId, Imagem from tbproduto join tbfornecedor on tbproduto.UserId = tbfornecedor.CodFor join tbcategoria on tbproduto.CategoriaId = tbcategoria.CodCat where CodProd = @codprod;", conexao);

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
                        CategoriaId = Convert.ToInt32(dr["categoriaid"]),
                        NomeCategoria = (string)dr["categoria"],
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
                MySqlCommand cmd = new MySqlCommand("select CodProd, NomeProd, Descricao, Preco, Qtd, Usuario, UserId, CategoriaId, Categoria, Imagem from tbproduto join tbfornecedor on tbproduto.UserId = tbfornecedor.CodFor join tbcategoria on tbproduto.CategoriaId = tbcategoria.CodCat", conexao);

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
                            Usuario = (string)dr["usuario"],
                            UserId = Convert.ToInt32(dr["userid"]),
                            CategoriaId = Convert.ToInt32(dr["categoriaid"]),
                            NomeCategoria = (string)dr["categoria"],
                            Imagem = (byte[])dr["imagem"],
                        });
                }
                return Produtoslist;
            }
        }
        public IEnumerable<Produto> TodosProdutosFornecedor(int id)
        {
            //Criando a lista que irá receber todos os produtos[
            List<Produto> Produtoslist = new List<Produto>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();
                //Criando o comando para listar todos os clientes
                MySqlCommand cmd = new MySqlCommand("select CodProd, NomeProd, Descricao, Preco, Qtd, Usuario, UserId, CategoriaId, Categoria, Imagem from tbproduto join tbfornecedor on tbproduto.UserId = tbfornecedor.CodFor join tbcategoria on tbproduto.CategoriaId = tbcategoria.CodCat where UserId = @id", conexao);

                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

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
                            Usuario = (string)dr["usuario"],
                            UserId = Convert.ToInt32(dr["userid"]),
                            CategoriaId = Convert.ToInt32(dr["categoriaid"]),
                            NomeCategoria = (string)dr["categoria"],
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
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("call spFinalizarCompra(@userId, @produtoId, @qtd, @qtdvenda)", conexao);

                cmd.Parameters.Add("@userId", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("@produtoId", MySqlDbType.Int32).Value = produtoId;
                cmd.Parameters.Add("@qtd", MySqlDbType.Int32).Value = qtd;
                cmd.Parameters.Add("@qtdvenda", MySqlDbType.Int32).Value = qtd;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public IEnumerable<Pedido> TodosPedidos(int userId)
        {
            List<Pedido> pedidoLista = new List<Pedido>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select tbpedido.CodPed, tbpedido.ProdutoId, tbpedido.UserId, tbpedido.DataPed, tbproduto.NomeProd, tbpedido.PrecoPed, tbpedido.QtdPed, tbproduto.Imagem from tbpedido join tbproduto on tbpedido.ProdutoId = tbproduto.CodProd where tbpedido.UserId = @userId", conexao);

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
                            ImagemPed = (byte[])(dr["imagem"]),
                            DataPed = Convert.ToDateTime(dr["dataped"]),
                            NomeProd = Convert.ToString(dr["nomeprod"]),
                            PrecoPed = Convert.ToDecimal(dr["precoped"]),
                            QtdPed = Convert.ToInt32(dr["qtdped"]),
                        });
                }
            }
            return pedidoLista;
        }

        public IEnumerable<Categoria> TodasCategorias()
        {
            List<Categoria> categoriaLista = new List<Categoria>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                // Vincular a conexão ao comando
                MySqlCommand cmd = new MySqlCommand("select * from tbcategoria", conexao);

                // Vincular o comando ao DataAdapter
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                // Preencher o DataTable com os dados da consulta
                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    categoriaLista.Add(new Categoria()
                    {
                        CodCat = Convert.ToInt32(dr["codcat"]),
                        Nome = dr["categoria"].ToString() ?? string.Empty
                    });
                }
            }

            return categoriaLista;
        }

        public IEnumerable<Produto> ProdutosPorCategoria(int categoriaId)
        {
            //Criando a lista que irá receber todos os produtos
            List<Produto> Produtoslist = new List<Produto>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();
                //Criando o comando para listar todos os clientes
                MySqlCommand cmd = new MySqlCommand("select CodProd, NomeProd, Descricao, Preco, Qtd, Usuario, UserId, CategoriaId, Categoria, Imagem from tbproduto join tbfornecedor on tbproduto.UserId = tbfornecedor.CodFor join tbcategoria on tbproduto.CategoriaId = tbcategoria.CodCat where CategoriaId = @categoriaid", conexao);

                cmd.Parameters.Add("@categoriaid", MySqlDbType.Int32).Value = categoriaId;

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
                            Usuario = (string)dr["usuario"],
                            UserId = Convert.ToInt32(dr["userid"]),
                            CategoriaId = Convert.ToInt32(dr["categoriaid"]),
                            NomeCategoria = (string)dr["categoria"],
                            Imagem = (byte[])dr["imagem"],
                        });
                }
                return Produtoslist;
            }
        }

        public void NovaPromocao(string nomepromo, int procentagem, string categoria, DateTime data)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("call spInserirPromocao(@nomepromo, @procentagem, @categoria, @data)", conexao);

                cmd.Parameters.Add("@nomepromo", MySqlDbType.VarChar).Value = nomepromo;
                cmd.Parameters.Add("@procentagem", MySqlDbType.Int32).Value = procentagem;
                cmd.Parameters.Add("@categoria", MySqlDbType.VarChar).Value = categoria;
                cmd.Parameters.Add("@data", MySqlDbType.DateTime).Value = data;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public IEnumerable<Promocao> TodasPromocoes()
        {
            List<Promocao> promocaoLista = new List<Promocao>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                // Vincular a conexão ao comando
                MySqlCommand cmd = new MySqlCommand("select PromoId, NomePromo, Porcentagem, Data_Exclusao, CategoriaId, Categoria from tbpromocao left join tbcategoria on tbpromocao.CategoriaId = tbcategoria.CodCat", conexao);

                // Vincular o comando ao DataAdapter
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                // Preencher o DataTable com os dados da consulta
                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    promocaoLista.Add(new Promocao()
                    {
                        PromoId = Convert.ToInt32(dr["promoid"]),
                        NomePromo = dr["nomepromo"].ToString() ?? string.Empty,
                        Porcentagem = Convert.ToInt32(dr["porcentagem"]),
                        Data_Exclusao = Convert.ToDateTime(dr["data_exclusao"]),
                        Categoria = Convert.ToString(dr["categoria"])
                    });
                }
            }

            return promocaoLista;
        }

        public IEnumerable<int> ProdutosEmPromocao()
        {
            List<int> Produtoslist = new List<int>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();
                //Criando o comando para listar todos os clientes
                MySqlCommand cmd = new MySqlCommand("select ProdutoId from tbpromocaoitem", conexao);

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
                    Produtoslist.Add(Convert.ToInt32(dr["ProdutoId"]));
                }
                return Produtoslist;
            }
        }

        public PromocaoItem ProdutoDaPromocao(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                PromocaoItem produtoEmPromocao = new PromocaoItem();
                //Abrindo a conexão com o banco de dados
                conexao.Open();
                //Criando o comando para listar todos os clientes
                MySqlCommand cmd = new MySqlCommand("select * from tbpromocaoitem where produtoid = @id", conexao);

                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

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
                    
                        produtoEmPromocao = new PromocaoItem()
                        {
                            PromoItemId = Convert.ToInt32(dr["promoiditem"]),
                            PromoId = Convert.ToInt32(dr["promoid"]),
                            ProdutoId = Convert.ToInt32(dr["produtoid"]),
                            PrecoPromo = Convert.ToDecimal(dr["precopromo"]),
                            Porcentagem = Convert.ToInt32(dr["porcentagem"])
                        };
                }
                return produtoEmPromocao;
            }
        }

        public IEnumerable<PromocaoItem> TodosProdutosDaPromocao()
        {
            List<PromocaoItem> ItensEmPromo = new List<PromocaoItem>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //Abrindo a conexão com o banco de dados
                conexao.Open();
                //Criando o comando para listar todos os clientes
                MySqlCommand cmd = new MySqlCommand("select * from tbpromocaoitem", conexao);
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
                    ItensEmPromo.Add(new PromocaoItem()
                    {
                        PromoItemId = Convert.ToInt32(dr["promoiditem"]),
                        PromoId = Convert.ToInt32(dr["promoid"]),
                        ProdutoId = Convert.ToInt32(dr["produtoid"]),
                        PrecoPromo = Convert.ToDecimal(dr["precopromo"]),
                        Porcentagem = Convert.ToInt32(dr["porcentagem"])
                    });
                }
                return ItensEmPromo;
            }
        }

        public void DeletarPromocao(int promoId)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("call spExcluirPromocao(@promoid)", conexao);

                cmd.Parameters.Add("@promoid", MySqlDbType.Int32).Value = promoId;

                cmd.ExecuteReader();
                conexao.Close();
            }
        }

        public int TotalVendas(int userId)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select sum(vendas) from tbproduto where UserId = @userid", conexao);

                cmd.Parameters.Add("@userid", MySqlDbType.Int32).Value = userId;

                int total = Convert.ToInt32(cmd.ExecuteScalar());

                conexao.Close();

                return total;
            }
        }

        public int TotalVendasSite()
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select sum(vendas) from tbproduto", conexao);

                int total = Convert.ToInt32(cmd.ExecuteScalar());

                conexao.Close();

                return total;
            }
        }

        public decimal ValorTotalVendas(int userId)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select sum(vendas * preco) from tbproduto where UserId = @userid", conexao);

                cmd.Parameters.Add("@userid", MySqlDbType.Int32).Value = userId;

                decimal total = Convert.ToDecimal(cmd.ExecuteScalar());

                conexao.Close();

                return total;
            }
        }

        public decimal LucroSite()
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select sum(vendas * preco) from tbproduto", conexao);

                decimal total = Convert.ToDecimal(cmd.ExecuteScalar());

                conexao.Close();

                return total;
            }
        }
    }
}
