using TCM.Libraries.LoginUsuarios;
using TCM.Models;
using TCM.Repositorio;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Adicionando autenticação com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login"; // Caminho para a página de login
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

// Adicionando HttpContextAccessor (necessário para acessar o contexto HTTP)
builder.Services.AddHttpContextAccessor();

// Adicionando a interface de login e outras dependências
builder.Services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
builder.Services.AddScoped<ICarrinhoRepositorio, CarrinhoRepositorio>();
builder.Services.AddScoped<ILoginRepositorio, LoginRepositorio>();
builder.Services.AddScoped<Usuario>();
builder.Services.AddScoped<TCM.Libraries.Sessao.Sessao>();
builder.Services.AddScoped<LoginUsuarios>();

// Adicionando o serviço de sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Define o tempo de expiração da sessão
    options.Cookie.HttpOnly = true;  // Aumenta a segurança
    options.Cookie.IsEssential = true;  // Necessário para conformidade com a GDPR
});

var app = builder.Build();

// Ativar autenticação e autorização
app.UseAuthentication(); // Ativa a autenticação com cookies
app.UseAuthorization();  // Ativa a autorização

// Configure o pipeline de requisições HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ativar o middleware de sessão
app.UseSession();  // Necessário para a sessão funcionar

// Ativar autenticação e autorização
app.UseAuthentication(); // Adicione esta linha para garantir que a autenticação funcione
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();