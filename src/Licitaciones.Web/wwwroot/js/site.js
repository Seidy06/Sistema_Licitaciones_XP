(() => {
    const control = document.getElementById('theme-toggle');
    if (!control) {
        return;
    }

    const icono = control.querySelector('span');

    function aplicarIcono(tema) {
        if (icono) {
            icono.textContent = tema === 'dark' ? '\u263E' : '\u2600';
        }
    }

    control.addEventListener('click', () => {
        const oscuroActivo = document.documentElement.getAttribute('data-bs-theme') === 'dark';
        const siguienteTema = oscuroActivo ? 'light' : 'dark';

        localStorage.setItem('theme', siguienteTema);
        document.documentElement.setAttribute('data-bs-theme', siguienteTema);
        aplicarIcono(siguienteTema);
    });

    aplicarIcono(document.documentElement.getAttribute('data-bs-theme') || 'light');
})();
