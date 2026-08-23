// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(() => {
    const aplicarTema = (tema) => {
        document.documentElement.setAttribute('data-bs-theme', tema);
    };

    aplicarTema(localStorage.getItem('theme') === 'dark' ? 'dark' : 'light');

    const control = document.getElementById('theme-toggle');
    if (!control) {
        return;
    }

    control.addEventListener('click', () => {
        const oscuroActivo = document.documentElement.getAttribute('data-bs-theme') === 'dark';
        const siguiente = oscuroActivo ? 'light' : 'dark';
        localStorage.setItem('theme', siguiente);
        aplicarTema(siguiente);
    });
})();
