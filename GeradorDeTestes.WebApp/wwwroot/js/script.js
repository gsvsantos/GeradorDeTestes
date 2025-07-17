const hamburguer = document.querySelector('.toggle-btn');
const toggler = document.querySelector('.toggle-btn-icon');
var minWidth = window.matchMedia("(min-width: 1200px)")
console.log(minWidth);

function sidebarShow() {
    document.querySelector('.sidebar').classList.toggle('expand');
    toggler.classList.toggle('bi-chevron-double-left');
    toggler.classList.toggle('bi-chevron-double-right');
}

function sidebarHide() {
    document.querySelector('.sidebar').classList.remove('expand');
}

if (!minWidth.matches) {
    sidebarHide();
}
else {
    hamburguer.addEventListener('click', sidebarShow);
}

minWidth.onchange = (e) => {
    if (!e.matches) {
        sidebarHide();
    }
    else {
        sidebarShow()
        hamburguer.addEventListener('click', sidebarShow);
    }
}