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

/* ---------- Rotating panels ---------- */
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

/* ---------- Scroll reveal ---------- */
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

/* ---------- Search ---------- */
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

/* ---------- Header hides on scroll ---------- */
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

/* ---------- Chat assistant ---------- */
(function () {
    const root = document.getElementById('chatBubble');
    if (!root) return;

    const openBtn = document.getElementById('cbOpen');
    const closeBtn = document.getElementById('cbClose');
    const log = document.getElementById('cbLog');
    const opts = document.getElementById('cbOpts');
    const askForm = document.getElementById('cbAsk');
    const askInput = document.getElementById('cbText');
    const hand = document.getElementById('cbHandoff');
    const err = document.getElementById('cbErr');
    const token = root.querySelector('input[name="__RequestVerificationToken"]');

    let started = false;
    let dept = null;
    let pending = '';

    function setOpen(open) {
        root.classList.toggle('open', open);
        openBtn.setAttribute('aria-expanded', open);
        if (open) {
            if (!started) {
                started = true;
                ask({ choice: 'start' });
            }
            askInput.focus();
        }
    }

    function addBubble(text, mine) {
        const d = document.createElement('div');
        d.className = 'bub ' + (mine ? 'me' : 'them');
        d.textContent = text;
        log.appendChild(d);
        log.scrollTop = log.scrollHeight;
    }

    function post(url, body) {
        return fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token ? token.value : ''
            },
            body: JSON.stringify(body)
        });
    }

    function addLink(url, label) {
        const a = document.createElement('a');
        a.className = 'btn-echt cb-link';
        a.href = url;
        a.textContent = label;
        opts.appendChild(a);
    }

    function showOptions(data) {
        opts.innerHTML = '';

        if (data.url) addLink(data.url, data.linkLabel || 'Open');

        (data.options || []).forEach(function (o) {
            const b = document.createElement('button');
            b.type = 'button';
            b.className = 'cb-opt';
            b.textContent = o.label;
            b.addEventListener('click', function () {
                ask({ choice: o.value }, o.label);
            });
            opts.appendChild(b);
        });
    }

    async function ask(payload, echo) {
        if (echo) addBubble(echo, true);
        opts.innerHTML = '';
        hand.hidden = true;
        err.textContent = '';

        try {
            const res = await post(root.dataset.ask, Object.assign({ dept: dept }, payload));
            if (!res.ok) throw new Error('bad reply');
            const data = await res.json();

            if (data.department) dept = data.department;
            addBubble(data.message, false);

            if (data.handoff) {
                pending = payload.text || pending;
                hand.hidden = false;
                hand.elements['text'].value = pending;
                (hand.elements['name'] || hand.elements['text']).focus();
            } else {
                showOptions(data);
            }
        } catch (ex) {
            addBubble('Sorry, something went wrong. Please try again.', false);
        }
    }

    hand.addEventListener('submit', async function (e) {
        e.preventDefault();
        err.textContent = '';

        const f = hand.elements;
        const body = {
            department: dept || 'General',
            text: f['text'].value.trim(),
            name: f['name'] ? f['name'].value.trim() : null,
            email: f['email'] ? f['email'].value.trim() : null
        };

        try {
            const res = await post(root.dataset.handoff, body);
            const data = await res.json().catch(function () { return {}; });

            if (!res.ok) {
                err.textContent = data.error || 'Something went wrong. Please try again.';
                return;
            }

            hand.hidden = true;
            pending = '';
            opts.innerHTML = '';

            if (data.url) {
                addBubble('Thank you. Your question has been passed to our team. You can follow the reply under Messages.', false);
                addLink(data.url, 'Open my messages');
            } else {
                addBubble('Thank you. Your question has been passed to our team, and a consultant will reply to your email.', false);
            }
        } catch (ex) {
            err.textContent = 'Could not send. Please check your connection.';
        }
    });

    askForm.addEventListener('submit', function (e) {
        e.preventDefault();
        const text = askInput.value.trim();
        if (!text) return;
        askInput.value = '';
        ask({ text: text }, text);
    });

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
})();