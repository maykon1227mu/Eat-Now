using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCM.Libraries.LoginUsuarios;
using TCM.Models;
using TCM.Repositorio;

namespace TCM.Controllers
{

    public class ProdutoController : Controller
    {
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly ICarrinhoRepositorio _carrinhoRepositorio;

        public ProdutoController(IProdutoRepositorio produtoRepositorio, ICarrinhoRepositorio carrinhoRepositorio)
        {
            _produtoRepositorio = produtoRepositorio;
            _carrinhoRepositorio = carrinhoRepositorio;
        }


        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View(_produtoRepositorio.TodosProdutos());
        }


        [Authorize(Roles = "Administrador, Fornecedor")]
        public IActionResult CadastrarProduto()
        {
            ViewBag.Categorias = _produtoRepositorio.TodasCategorias();
            return View();
        }


        [HttpPost]
        public IActionResult CadastrarProduto(Produto produto, IFormFile imagem)
        {
            if (imagem != null && imagem.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    imagem.CopyTo(ms);
                    produto.Imagem = ms.ToArray();
                }
            }

            _produtoRepositorio.AdicionarProduto(produto);
            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = "Administrador, Fornecedor")]
        public IActionResult EditarProduto(int id)
        {
            var produto = _produtoRepositorio.AcharProduto(id);
            ViewBag.Categorias = _produtoRepositorio.TodasCategorias();
            return View(produto);
        }


        [HttpPost]
        public IActionResult EditarProduto(Produto produto, IFormFile imagem)
        {
            if (imagem != null && imagem.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    imagem.CopyTo(ms);
                    produto.Imagem = ms.ToArray();
                }
            }
            _produtoRepositorio.EditarProduto(produto);
            return RedirectToAction("Index", "Produto");
        }


        [Authorize(Roles = "Administrador, Fornecedor")]
        public IActionResult DeletarProduto(int id)
        {
            _produtoRepositorio.DeletarProduto(id);
            return RedirectToAction("Index", "Produto");
        }


        public IActionResult Comprar(int id)
        {
            var produto = _produtoRepositorio.AcharProduto(id);

            ViewBag.ProdutosPromo = _produtoRepositorio.ProdutosEmPromocao();
            ViewBag.ProdutoPromo = _produtoRepositorio.ProdutoDaPromocao(produto.CodProd);
            ViewBag.Avaliacoes = _produtoRepositorio.TotalAvaliacoes(produto.CodProd);
            var comentarios = _produtoRepositorio.ComentariosProduto(produto.CodProd);

            if (produto.Imagem != null)
            {
                produto.ImagemBase64 = Convert.ToBase64String(produto.Imagem);
            }
            foreach(var comentario in comentarios)
            {
                if(comentario.FotoPerfil != null)
                {
                    comentario.FotoPerfilBase64 = Convert.ToBase64String(comentario.FotoPerfil);
                }
            }
            ViewBag.Comentarios = comentarios;
            return View(produto);
        }


        [HttpPost]
        public IActionResult Pesquisar()
        {
            string nome = Request.Form["txtpesq"];
            if (nome == "") return RedirectToAction("Index", "Home");
            var produtos = _produtoRepositorio.Pesquisa(nome);
            foreach (var produto in produtos)
            {
                if (produto.Imagem != null)
                {
                    produto.ImagemBase64 = Convert.ToBase64String(produto.Imagem);
                }
            }
            return View(produtos);
        }


        [Authorize]
        public IActionResult Finalizar()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var carrinho = _carrinhoRepositorio.ObterCarrinhoPorUsuario(id);
            foreach (var item in carrinho)
            {
                _produtoRepositorio.FinalizarCompra(id, item.ProdutoId, item.Quantidade);
                _carrinhoRepositorio.RemoverItemCarrinho(id, item.ProdutoId, item.Quantidade);
            }
            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = "Fornecedor")]
        public IActionResult ProdutosFornecedor()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var produtos = _produtoRepositorio.TodosProdutosFornecedor(id);
            return View(produtos);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult PainelPromocoes()
        {
            return View(_produtoRepositorio.TodasPromocoes());
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult NovaPromocao()
        {
            ViewBag.Categorias = _produtoRepositorio.TodasCategorias();
            return View();
        }

        [HttpPost]
        public IActionResult NovaPromocao(Promocao promocao)
        {
            
            string categoria = Request.Form["CategoriaId"];
            _produtoRepositorio.NovaPromocao(promocao.NomePromo, promocao.Porcentagem, categoria, promocao.Data_Exclusao);
            return RedirectToAction("PainelPromocoes", "Produto");
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult DeletarPromocao(int promoId)
        {
            _produtoRepositorio.DeletarPromocao(promoId);
            return RedirectToAction("PainelPromocoes", "Produto");
        }

        public IActionResult Vendas()
        {
            ViewBag.TotalVendas = _produtoRepositorio.TotalVendasSite();
            ViewBag.Lucro = _produtoRepositorio.LucroSite();
            return View();
        }

        public IActionResult VendasFornecedor()
        {
            ViewBag.TotalVendas = _produtoRepositorio.TotalVendas(Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value));
            ViewBag.Lucro = _produtoRepositorio.ValorTotalVendas(Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value));
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Comentar(Comentario comentarios)
        {
            comentarios.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            _produtoRepositorio.Comentar(comentarios);
            return RedirectToAction("Comprar", "Produto", new { id = comentarios.ProdutoId });
        }
    }
}