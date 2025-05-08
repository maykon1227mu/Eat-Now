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

            const position = index * 102; // Multiplica pelo tamanho do item (86px)

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



const html = document.getElementById('html');
const imgLogo = document.getElementById('logoNav');
const imgLogin = document.getElementById('logoLogin');
const btn = document.getElementById('btn-toggle');

// Aplica o tema salvo ao carregar a página
document.addEventListener('DOMContentLoaded', function () {
    const tema = localStorage.getItem('tema');

    if (tema === "dark") {
        html.classList.add('dark');
        btn.classList.add('dark');
        if (imgLogo) imgLogo.src = "/img/LogoDark.png";
        if (imgLogin) imgLogin.src = "/img/LogoLight.png";
    } else {
        html.classList.remove('dark');
        btn.classList.remove('dark');
        if (imgLogo) imgLogo.src = "/img/LogoLight.png";
        if (imgLogin) imgLogin.src = "/img/LogoDark.png";
    }
});

btn.addEventListener('click', function () {
    html.classList.toggle('dark');
    btn.classList.toggle('dark');

    if (html.classList.contains("dark")) {
        localStorage.setItem('tema', "dark");
        if (imgLogo) imgLogo.src = "/img/LogoDark.png";
        if (imgLogin) imgLogin.src = "/img/LogoLight.png";
    } else {
        localStorage.setItem('tema', "light");
        if (imgLogo) imgLogo.src = "/img/LogoLight.png";
        if (imgLogin) imgLogin.src = "/img/LogoDark.png";
    }
});




function verificaIdade() {
    console.log("foi");

    const dataNascimento = new Date(document.getElementById("data").value);
    const dataAtual = new Date();


    let idade = dataAtual.getFullYear() - dataNascimento.getFullYear();
    const mesAtual = dataAtual.getMonth();
    const diaAtual = dataAtual.getDate();

    if (mesAtual < dataNascimento.getMonth() || (mesAtual === dataNascimento.getMonth() && diaAtual < dataNascimento.getDate())) {
        idade--;
    }

    if (idade <= 18) {
        document.getElementById("idadeX").innerHTML = "A idade é menor que 18 anos.";
        document.getElementById("btnCad").style.display = "none";
    } else {
        document.getElementById("idadeX").innerHTML = "";
        document.getElementById("btnCad").style.display = "block";
    }

}

function mascara(input) {
    let cpf = input.value.replace(/\D/g, "");
    if (cpf.length > 3 && cpf.length <= 6) {
        cpf = cpf.replace(/(\d{3})(\d+)/, "$1.$2");
    } else if (cpf.length > 6 && cpf.length <= 9) {
        cpf = cpf.replace(/(\d{3})(\d{3})(\d+)/, "$1.$2.$3");
    } else if (cpf.length > 9) {
        cpf = cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{1,2})/, "$1.$2.$3-$4");
    }

    input.value = cpf;
}

async function verificaCEP() {
    let cep = document.getElementById("cep").value.replace(/\D/g, '');

    if (cep.length == 8) {
        try {
            const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
            const data = await response.json();

            if (data.erro) {
                alert("CEP não encontrado.");
                limparCampos();
            } else {
                document.getElementById("logradouro").value = data.logradouro || '';
                document.getElementById("bairro").value = data.bairro || '';
                document.getElementById("cidade").value = data.localidade || '';
            }
        } catch {
            console.log("Erro");
        }
    }



}