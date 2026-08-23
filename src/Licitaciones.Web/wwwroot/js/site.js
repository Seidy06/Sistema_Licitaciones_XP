(() => {
    const control = document.getElementById('theme-toggle');
    if (!control) {
        return;
    }

    control.addEventListener('click', () => {
        const oscuroActivo = document.documentElement.getAttribute('data-bs-theme') === 'dark';
        const siguienteTema = oscuroActivo ? 'light' : 'dark';

        localStorage.setItem('theme', siguienteTema);
        document.documentElement.setAttribute('data-bs-theme', siguienteTema);
    });
})();
