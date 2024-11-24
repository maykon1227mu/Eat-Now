using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCM.Models;
using TCM.Repositorio;

namespace TCM.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly ICarrinhoRepositorio _carrinhoRepositorio;

        public CarrinhoController(IProdutoRepositorio produtoRepositorio, ICarrinhoRepositorio carrinhoRepositorio)
        {
            _produtoRepositorio = produtoRepositorio;
            _carrinhoRepositorio = carrinhoRepositorio;
        }

        public IActionResult Index(int id)
        {
            var carrinho = _carrinhoRepositorio.ObterCarrinhoPorUsuario(id);
            return View(carrinho);
        }

        [HttpPost]
        public IActionResult Adicionar()
        {
            int id = Convert.ToInt32(Request.Form["produtoId"]);
            int qtd = Convert.ToInt32(Request.Form["qtd"]);
            var produto = _produtoRepositorio.AcharProduto(id);
            _carrinhoRepositorio.SalvarItemCarrinho(Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value), produto, qtd);
            return RedirectToAction("Index", new { id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value) });
        }

        public IActionResult Remover(int id, int qtd)
        {
            _carrinhoRepositorio.RemoverItemCarrinho(Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value), id, qtd);
            return RedirectToAction("Index", new { id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value) });
        }

        
    }
}
