let menuToggle = document.querySelector('.menuToggle');
let sidebar = document.querySelector('.navprincipal');

menuToggle.onclick = function () {
    menuToggle.classList.toggle('ativo');
    sidebar.classList.toggle('ativo');
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

    // Atualiza quando a página carrega
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

btn.addEventListener('click', function () {
    html.classList.toggle('dark');
});