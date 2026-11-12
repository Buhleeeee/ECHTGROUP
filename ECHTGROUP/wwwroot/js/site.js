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

(function () {
    const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    document.querySelectorAll('.rot').forEach(function (rot) {
        const slides = rot.querySelectorAll('.slide');
        const dots = rot.querySelectorAll('.dots i');
        let k = 0;

        function show() {
            slides.forEach((s, i) => s.classList.toggle('on', i === k));
            dots.forEach((d, i) => d.classList.toggle('on', i === k));
        }

        show();

        if (!reduce && slides.length > 1) {
            setInterval(function () {
                k = (k + 1) % slides.length;
                show();
            }, 4000);
        }
    });
})();

(function () {
    const items = document.querySelectorAll('.rv');
    if (!items.length) return;

    if (!('IntersectionObserver' in window)) {
        items.forEach(el => el.classList.add('in'));
        return;
    }

    const io = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('in');
                io.unobserve(entry.target);
            }
        });
    }, { threshold: 0.15 });

    items.forEach(el => io.observe(el));
})();