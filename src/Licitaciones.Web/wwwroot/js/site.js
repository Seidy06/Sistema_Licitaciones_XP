(() => {
    const control = document.getElementById('theme-toggle');
    if (!control) return;

    const icono = control.querySelector('span');

    function aplicarIcono(tema) {
        if (!icono) return;
        icono.textContent = tema === 'dark' ? '\u263E' : '\u2600';
        control.setAttribute('aria-label',
            tema === 'dark' ? 'Cambiar a modo claro' : 'Cambiar a modo oscuro');
    }

    control.addEventListener('click', () => {
        const actual = document.documentElement.getAttribute('data-bs-theme') === 'dark';
        const siguiente = actual ? 'light' : 'dark';
        localStorage.setItem('theme', siguiente);
        document.documentElement.setAttribute('data-bs-theme', siguiente);
        aplicarIcono(siguiente);
    });

    aplicarIcono(document.documentElement.getAttribute('data-bs-theme') || 'light');
})();
