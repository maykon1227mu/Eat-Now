using Microsoft.AspNetCore.Authorization;
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
        private readonly IEnderecoRepositorio _enderecoRepositorio;

        public CarrinhoController(IProdutoRepositorio produtoRepositorio, ICarrinhoRepositorio carrinhoRepositorio, IEnderecoRepositorio enderecoRepositorio)
        {
            _produtoRepositorio = produtoRepositorio;
            _carrinhoRepositorio = carrinhoRepositorio;
            _enderecoRepositorio = enderecoRepositorio;
        }
        [Authorize]
        public IActionResult Index()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var carrinho = _carrinhoRepositorio.ObterCarrinhoPorUsuario(id);
            ViewBag.Enderecos = _enderecoRepositorio.TodosEnderecos(id);
            return View(carrinho);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Adicionar()
        {
            int id = Convert.ToInt32(Request.Form["produtoId"]);
            int qtd = Convert.ToInt32(Request.Form["qtd"]);
            var produto = _produtoRepositorio.AcharProduto(id);
            _carrinhoRepositorio.SalvarItemCarrinho(Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value), produto, qtd);
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Remover(int id, int qtd)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            _carrinhoRepositorio.RemoverItemCarrinho(userId, id, qtd);
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult LimparCarrinho()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var carrinho = _carrinhoRepositorio.ObterCarrinhoPorUsuario(userId);
            foreach (var item in carrinho)
            {
                _carrinhoRepositorio.RemoverItemCarrinho(userId, item.ProdutoId, item.Quantidade);
            }
            return RedirectToAction("Index");
        }
    }
}
