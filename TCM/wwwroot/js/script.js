let menuToggle = document.querySelector('.menuToggle');
let menuClose = document.querySelector('.menuClose');
let sidebar = document.querySelector('.sidebar');



menuToggle.onclick = function() {
    menuToggle.classList.toggle('active');
    sidebar.classList.toggle('active');
    menuClose.classList.toggle('active');   
}

let Menulist = document.querySelectorAll('.Menulist li');

function activeLink() {
    Menulist.forEach((item) =>
    item.classList.remove('active'));
    this.classList.add('active')
}
Menulist.forEach((item) =>
item.addEventListener('click',activeLink));

let ative = 1;

function closeNav() {
    if (ative === 1) {
        sidebar.style.width = "0px";
        sidebar.style.opacity = "0";
        menuClose.style.transform = "rotate(180deg)";
        ative = 2;
    }
     else {
        sidebar.style.width = "80px";
        sidebar.style.opacity = "1";
        menuClose.style.transform = "rotate(0deg)";
        ative = 1;
    }
}

const myObserver = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
        if(entry.isIntersecting){
            entry.target.classList.add('show')
        }
        else {
            entry.target.classList.remove('show')
        }
    })
})

const elements = document.querySelectorAll('.hidden')
elements.forEach( (element) => myObserver.observe(element))