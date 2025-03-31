let menuToggle = document.querySelector('.menuToggle');
let sidebar = document.querySelector('.navprincipal');
let openNav = document.querySelector('.container-toggle');
menuToggle.onclick = function () {
    menuToggle.classList.toggle('ativo');
    sidebar.classList.toggle('ativo');
    openNav.classList.toggle('ativo');
}


document.addEventListener("DOMContentLoaded", function () {
    const bolinha = document.querySelector(".bolinha");
    const items = document.querySelectorAll(".mobileNav ul li");

    function updateBolinha() {
        const tamanhoTela = window.innerWidth;
        console.log(tamanhoTela);
        const activeItem = document.querySelector(".mobileNav ul li.active");
        if (activeItem) {
            const index = Array.from(items).indexOf(activeItem);

            const position = index * 86; // Multiplica pelo tamanho do item (86px)
            
            bolinha.style.setProperty("--translateX", `${position}px`);
            
        }
    }

    // Atualiza quando a p�gina carrega
    updateBolinha();

    // Atualiza ao clicar em um item do menu
    items.forEach((item) => {
        item.addEventListener("click", function () {
            items.forEach((li) => li.classList.remove("active"));
            this.classList.add("active");
            updateBolinha();
        });
    });
});


const btn = document.getElementById('btn-toggle');
const html = document.getElementById('html');
let imgLogo = document.getElementById('logoNav');


btn.addEventListener('click', function () {
    html.classList.toggle('dark');
    btn.classList.toggle('dark');


    if (html.classList.contains("dark")) {
        imgLogo.src = "/img/LogoDark.png";
    } else {
        imgLogo.src = "/img/LogoLight.png";
    }
});