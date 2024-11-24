using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCM.Models;
using TCM.Repositorio;

namespace TCM.Controllers
{
    public class ContaController : Controller
    {
        private ILoginRepositorio _loginRepositorio;
        private IProdutoRepositorio _produtoRepositorio;
        public ContaController(ILoginRepositorio loginRepositorio, IProdutoRepositorio produtoRepositorio)
        {
            _loginRepositorio = loginRepositorio;
            _produtoRepositorio = produtoRepositorio;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult MinhaConta()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var user = _loginRepositorio.AcharUsuario(id);
            return View(user);
        }

        public IActionResult EditarConta()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var user = _loginRepositorio.AcharUsuario(id);
            return View(user);
        }

        [HttpPost]
        public IActionResult EditarConta(Usuario user)
        {
            _loginRepositorio.EditarUsuario(user);
            return RedirectToAction("Index", "Conta");
        }

        public IActionResult Funcionarios()
        {
            return View(_loginRepositorio.TodosFuncionarios());
        }

        public IActionResult Fornecedores()
        {
            return View(_loginRepositorio.TodosFornecedores());
        }

        public IActionResult CadastrarFuncionario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarFuncionario(Funcionario funcionario)
        {
            _loginRepositorio.CadastrarFuncionario(funcionario.Nome, funcionario.email, funcionario.usuario, funcionario.senha, funcionario.Salario);
            return RedirectToAction("Index", "Conta");
        }

        public IActionResult CadastrarFornecedor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarFornecedor(Fornecedor fornecedor)
        {
            _loginRepositorio.CadastrarFornecedor(fornecedor.email, fornecedor.usuario, fornecedor.senha, fornecedor.CNPJ);
            return RedirectToAction("Index", "Conta");
        }

        public IActionResult MeusPedidos()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var pedidos = _produtoRepositorio.TodosPedidos(id);
            return View(pedidos);
        }
    }
}
