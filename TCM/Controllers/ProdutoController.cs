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
        //Trazendo a interface a instanciando
        private IProdutoRepositorio _produtoRepositorio;
        private ICarrinhoRepositorio _carrinhoRepositorio;
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
                //Cria uma memoria temporaria para ler e escrever dados diretamente na memoria
                using (var ms = new MemoryStream())
                {
                    imagem.CopyTo(ms);
                    produto.Imagem = ms.ToArray();
                }
            }

            _produtoRepositorio.AdicionarProduto(produto);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Administrador")]
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
                //Cria uma memoria temporaria para ler e escrever dados diretamente na memoria
                using (var ms = new MemoryStream())
                {
                    imagem.CopyTo(ms);
                    produto.Imagem = ms.ToArray();
                }
            }
            _produtoRepositorio.EditarProduto(produto);
            return RedirectToAction("Index", "Produto");
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult DeletarProduto(int id)
        {
            _produtoRepositorio.DeletarProduto(id);
            return RedirectToAction("Index", "Produto");
        }

        public IActionResult Comprar(int id)
        {
            var produto = _produtoRepositorio.AcharProduto(id);
            //Verifica se a imagem existe
            if (produto.Imagem != null)
            {
                //Transforma o blob para um jeito que dê para a imagem ser interpretada
                produto.ImagemBase64 = Convert.ToBase64String(produto.Imagem);
            }
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
    }
}
