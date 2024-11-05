using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TCM.Libraries.LoginUsuarios;
using TCM.Models;
using TCM.Repositorio;

namespace TCM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //Trazendo a interface e instanciando
        private ILoginRepositorio _loginRepositorio;
        //Chamando a model e instanciando
        private LoginUsuarios _loginUsuarios;
        //Trazendo a interface a instanciando
        private IProdutoRepositorio _produtoRepositorio;

        //Criando o construtor
        public HomeController(ILogger<HomeController> logger, ILoginRepositorio loginRepositorio, LoginUsuarios loginUsuarios,IProdutoRepositorio produtoRepositorio)
        {
            _logger = logger;
            _loginRepositorio = loginRepositorio;
            _produtoRepositorio = produtoRepositorio;
            _loginUsuarios = loginUsuarios;
        }

        public IActionResult Index()
        {
            return View(_produtoRepositorio.TodosProdutos());
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Usuario user)
        {
            Usuario loginUser = _loginRepositorio.Login(user.usuario, user.senha);
            if (loginUser.usuario != null && loginUser.senha != null)
            {
                return new RedirectResult(Url.Action(nameof(Index)));
            }
            else
            {
                ViewData["msg"] = "Usuario/Senha inválidos";
                return View();
            }
        }

        public IActionResult CadastrarUsuario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarUsuario(Usuario user)
        {
            _loginRepositorio.Cadastrar(user.email, user.usuario, user.senha);
            if (user.email != null && user.usuario != null && user.senha != null)
            {
                return new RedirectResult(Url.Action(nameof(Index)));
            }
            else
            {
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
