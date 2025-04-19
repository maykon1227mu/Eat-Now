using Microsoft.AspNetCore.Authorization;
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
        private IEnderecoRepositorio _enderecoRepositorio;
        public ContaController(ILoginRepositorio loginRepositorio, IProdutoRepositorio produtoRepositorio, IEnderecoRepositorio enderecoRepositorio)
        {
            _loginRepositorio = loginRepositorio;
            _produtoRepositorio = produtoRepositorio;
            _enderecoRepositorio = enderecoRepositorio;
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult MinhaConta()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var user = _loginRepositorio.AcharUsuario(id);
            return View(user);
        }
        [Authorize]
        public IActionResult EditarConta()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var user = _loginRepositorio.AcharUsuario(id);
            return View(user);
        }
        public IActionResult MeusDados()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var user = _loginRepositorio.AcharUsuario(id);
            if (user.FotoPerfil != null) user.FotoPerfilBase64 = Convert.ToBase64String(user.FotoPerfil);
            return View(user);
        }

        [HttpPost]
        public IActionResult EditarConta(Usuario user, IFormFile imagem)
        {
            if (imagem != null && imagem.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    imagem.CopyTo(ms);
                    user.FotoPerfil = ms.ToArray();
                }
            }
            _loginRepositorio.EditarUsuario(user);
            return RedirectToAction("MinhaConta", "Conta");
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult Administradores()
        {
            return View(_loginRepositorio.TodosAdministradores());
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult Colaboradores()
        {
            return View(_loginRepositorio.TodosColaboradores());
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult CadastrarAdministrador()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarAdministrador(Administrador administrador)
        {
            _loginRepositorio.CadastrarAdministrador(administrador.Nome, administrador.email, administrador.usuario, administrador.senha);
            return RedirectToAction("Index", "Conta");
        }

        public IActionResult CadastrarColaborador()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarColaborador(Colaborador colaborador)
        {
            _loginRepositorio.CadastrarColaborador(colaborador.nome, colaborador.email, colaborador.usuario, colaborador.senha, colaborador.CNPJ);
            return RedirectToAction("Index", "Conta");
        }
        [Authorize]
        public IActionResult MeusPedidos()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var pedidos = _produtoRepositorio.TodosPedidos(id);
            return View(pedidos);
        }

        [Authorize(Roles = "Fornecedor")]
        public IActionResult PainelColaborador()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var fornecedor = _loginRepositorio.AcharColaborador(id);
            return View(fornecedor);
        }

        [Authorize]
        public IActionResult NovoEndereco()
        {
            ViewBag.Estado = _enderecoRepositorio.TodosEstados();
            return View();
        }

        [HttpPost]
        public IActionResult NovoEndereco(Endereco endereco)
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            endereco.UserId = id;
            _enderecoRepositorio.AdicionarEndereco(endereco);
            return RedirectToAction("MeusEnderecos", "Conta");
        }

        public IActionResult MeusEnderecos()
        {
            int id = Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            return View(_enderecoRepositorio.TodosEnderecos(id));
        }

        public IActionResult DetalhesPedido(int id)
        {
            var pedido = _produtoRepositorio.AcharPedido(id);
            ViewBag.Endereco = _enderecoRepositorio.AcharEndereco(pedido.IdEndereco);
            var produto = _produtoRepositorio.AcharProduto(pedido.ProdutoId);
            if(produto.Imagem != null)
            {
                produto.ImagemBase64 = Convert.ToBase64String(produto.Imagem);
            }
            ViewBag.Produto = produto;
            ViewBag.Usuario = _loginRepositorio.AcharUsuario(pedido.UserId);

            if (pedido.UserId != Convert.ToInt32(User.FindFirst(ClaimTypes.SerialNumber)?.Value)) return NotFound();

            return View(pedido);
        }
        
        public IActionResult DetalhesPedidoFuncionario(int id)
        {
            var pedido = _produtoRepositorio.AcharPedido(id);
            ViewBag.Endereco = _enderecoRepositorio.AcharEndereco(pedido.IdEndereco);
            var produto = _produtoRepositorio.AcharProduto(pedido.ProdutoId);
            if (produto.Imagem != null)
            {
                pedido.ImagemBase64 = Convert.ToBase64String(produto.Imagem);
            }
            ViewBag.Produto = produto;
            ViewBag.Usuario = _loginRepositorio.AcharUsuario(pedido.UserId);
            return View(pedido);
        }
    }
}
