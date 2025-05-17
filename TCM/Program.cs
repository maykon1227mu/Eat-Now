using TCM.Libraries.LoginUsuarios;
using TCM.Models;
using TCM.Repositorio;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Adicionando autentica��o com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login"; // Caminho para a p�gina de login
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

// Adicionando HttpContextAccessor (necess�rio para acessar o contexto HTTP)
builder.Services.AddHttpContextAccessor();

// Adicionando a interface de login e outras depend�ncias
builder.Services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
builder.Services.AddScoped<ICarrinhoRepositorio, CarrinhoRepositorio>();
builder.Services.AddScoped<ILoginRepositorio, LoginRepositorio>();
builder.Services.AddScoped<IEnderecoRepositorio, EnderecoRepositorio>();
builder.Services.AddScoped<Usuario>();
builder.Services.AddScoped<TCM.Libraries.Sessao.Sessao>();
builder.Services.AddScoped<LoginUsuarios>();

// Adicionando o servi�o de sess�o
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Define o tempo de expira��o da sess�o
    options.Cookie.HttpOnly = true;  // Aumenta a seguran�a
    options.Cookie.IsEssential = true;  // Necess�rio para conformidade com a GDPR
});

var app = builder.Build();

// Ativar autentica��o e autoriza��o
app.UseAuthentication(); // Ativa a autentica��o com cookies
app.UseAuthorization();  // Ativa a autoriza��o

// Configure o pipeline de requisi��es HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ativar o middleware de sess�o
app.UseSession();  // Necess�rio para a sess�o funcionar

// Ativar autentica��o e autoriza��o
app.UseAuthentication(); // Adicione esta linha para garantir que a autentica��o funcione
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();