(function () {
    const burger = document.getElementById('burger');
    const menu = document.getElementById('menu');
    if (!burger || !menu) return;

    function setOpen(open) {
        document.body.classList.toggle('open', open);
        burger.setAttribute('aria-expanded', open);
    }

    burger.addEventListener('click', function () {
        setOpen(!document.body.classList.contains('open'));
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') setOpen(false);
    });

    document.addEventListener('click', function (e) {
        if (!e.target.closest('.site-header')) setOpen(false);
    });
})();