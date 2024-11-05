using Microsoft.AspNetCore.Mvc;
using TCM.Libraries.LoginUsuarios;
using TCM.Models;
using TCM.Repositorio;

namespace TCM.Controllers
{
    public class ProdutoController : Controller
    {
        //Trazendo a interface a instanciando
        private IProdutoRepositorio _produtoRepositorio;
        public ProdutoController(IProdutoRepositorio produtoRepositorio)
        {
            _produtoRepositorio = produtoRepositorio;
        }
        public IActionResult Index()
        {
            return View(_produtoRepositorio.TodosProdutos());
        }

        public IActionResult CadastrarProduto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarProduto(Produto produto)
        {
            _produtoRepositorio.AdicionarProduto(produto);
            return RedirectToAction("Index", "Produto");
        }


        public IActionResult EditarProduto(int id)
        {
            var produto = _produtoRepositorio.AcharProduto(id);
            return View(produto);
        }

        [HttpPost]
        public IActionResult EditarProduto(Produto produto)
        {
            _produtoRepositorio.EditarProduto(produto);
            return RedirectToAction("Index", "Produto");
        }

        public IActionResult DeletarProduto(int id)
        {
            var produto = _produtoRepositorio.AcharProduto(id);
            _produtoRepositorio.DeletarProduto(produto);
            return RedirectToAction("Index", "Produto");
        }

        public IActionResult Comprar(int id)
        {
            var produto = _produtoRepositorio.AcharProduto(id);
            return View(produto);
        }
    }
}
