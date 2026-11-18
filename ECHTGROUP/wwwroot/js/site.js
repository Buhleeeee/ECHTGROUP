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
(function () {
    const btn = document.getElementById('searchBtn');
    const bar = document.getElementById('searchBar');
    if (!btn || !bar) return;
    const input = bar.querySelector('input');
    const burger = document.getElementById('burger');

    function setSearch(open) {
        document.body.classList.toggle('searching', open);
        btn.setAttribute('aria-expanded', open);
        if (open) {
            document.body.classList.remove('open');
            if (burger) burger.setAttribute('aria-expanded', 'false');
            input.focus();
        }
    }

    btn.addEventListener('click', function () {
        setSearch(!document.body.classList.contains('searching'));
    });

    if (burger) burger.addEventListener('click', function () { setSearch(false); });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') setSearch(false);
    });

    document.addEventListener('click', function (e) {
        if (!e.target.closest('.site-header')) setSearch(false);
    });
})();
(function () {
    const root = document.getElementById('chatBubble');
    if (!root) return;

    const openBtn = document.getElementById('cbOpen');
    const closeBtn = document.getElementById('cbClose');

    function setOpen(open) {
        root.classList.toggle('open', open);
        openBtn.setAttribute('aria-expanded', open);
        if (open) {
            const first = root.querySelector('input, textarea');
            if (first) first.focus();
        }
    }

    openBtn.addEventListener('click', function () {
        setOpen(!root.classList.contains('open'));
    });

    closeBtn.addEventListener('click', function () {
        setOpen(false);
        openBtn.focus();
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') setOpen(false);
    });

    const form = document.getElementById('cbForm');
    if (!form) return;
    const err = document.getElementById('cbErr');

    form.addEventListener('submit', async function (e) {
        e.preventDefault();
        err.textContent = '';
        try {
            const res = await fetch(form.action, { method: 'POST', body: new FormData(form) });
            if (res.ok) {
                form.innerHTML = '<p class="ok">Thanks, we’ll reply as soon as we can.</p>';
            } else {
                const data = await res.json().catch(function () { return {}; });
                err.textContent = data.error || 'Something went wrong. Please try again.';
            }
        } catch (ex) {
            err.textContent = 'Could not send. Please check your connection.';
        }
    });
})();
(function () {
    const header = document.querySelector('.site-header');
    if (!header) return;

    let lastY = window.scrollY;
    let ticking = false;

    function update() {
        const y = window.scrollY;
        const busy = document.body.classList.contains('open') ||
                     document.body.classList.contains('searching');

        if (busy || y < 80) {
            header.classList.remove('hide');
        } else if (y > lastY + 6) {
            header.classList.add('hide');
        } else if (y < lastY - 6) {
            header.classList.remove('hide');
        }

        lastY = y;
        ticking = false;
    }

    window.addEventListener('scroll', function () {
        if (!ticking) {
            window.requestAnimationFrame(update);
            ticking = true;
        }
    }, { passive: true });
})();