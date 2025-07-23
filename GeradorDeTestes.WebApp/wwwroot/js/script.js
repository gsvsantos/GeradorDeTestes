const hamburguer = document.querySelector('.toggle-btn');
const toggler = document.querySelector('.toggle-btn-icon');
var minWidth = window.matchMedia("(min-width: 1200px)")
const sidebar = document.querySelector('.sidebar');
console.log(minWidth);

function toggleSidebar() {
    sidebar.classList.toggle('expand');
    updateTogglerIcon();
}

function sidebarHide() {
    sidebar.classList.remove('expand');
    updateTogglerIcon();

    document.querySelectorAll('.nav-dropdown.collapse.show')
        .forEach(el => bootstrap.Collapse.getInstance(el)?.hide());
}

function updateTogglerIcon() {
    if (!toggler) return;
    const expanded = sidebar.classList.contains('expand');
    toggler.classList.toggle('bi-chevron-double-left', expanded);
    toggler.classList.toggle('bi-chevron-double-right', !expanded);
}

hamburguer?.addEventListener('click', () => {
    if (minWidth.matches) {
        toggleSidebar();
    }
});

if (!minWidth.matches) {
    sidebarHide();
}

minWidth.onchange = (e) => {
    if (!e.matches) {
        sidebarHide();
    } else {
        sidebar.classList.add('expand');
        updateTogglerIcon();
    }
};