using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TCM.Libraries.LoginUsuarios;
using TCM.Models;
using TCM.Repositorio;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata;

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
            var produtos = _produtoRepositorio.TodosProdutos();
            //Busca todas as categorias
            ViewBag.Categorias = _produtoRepositorio.TodasCategorias();
            ViewBag.ProdutosPromo = _produtoRepositorio.ProdutosEmPromocao();
            ViewBag.TodosProdutosPromo = _produtoRepositorio.TodosProdutosDaPromocao();
            

            // Converte as imagens para Base64
            foreach (var produto in produtos)
            {

                if (produto.Imagem != null)
                {
                    produto.ImagemBase64 = Convert.ToBase64String(produto.Imagem);
                }
            }

            return View(produtos);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Usuario user)
        {
            dynamic loginUser = await _loginRepositorio.Login(user.usuario, user.senha);
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
            _loginRepositorio.Cadastrar(user.Nome, user.email, user.usuario, user.senha);
            if (user.email != null && user.usuario != null && user.senha != null)
            {
                return new RedirectResult(Url.Action(nameof(Index)));
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            // Chama o método de Logout no repositório
            await _loginRepositorio.Logout();

            // Redireciona o usuário para a página inicial ou para a página de login
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
