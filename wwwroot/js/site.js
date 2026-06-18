// CV / portfolio — interactivity: theme toggle, mobile nav, scroll reveals,
// animated skill bars and active-section highlighting.
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        initTheme();
        initNav();
        initReveal();
    });

    // ----- Theme toggle (persisted in localStorage) -----
    function initTheme() {
        var toggle = document.getElementById('themeToggle');
        if (!toggle) return;
        toggle.addEventListener('click', function () {
            var root = document.documentElement;
            var next = root.getAttribute('data-theme') === 'light' ? 'dark' : 'light';
            root.setAttribute('data-theme', next);
            localStorage.setItem('theme', next);
        });
    }

    // ----- Mobile nav + auto-close on link click -----
    function initNav() {
        var burger = document.getElementById('navToggle');
        var links = document.getElementById('navLinks');
        if (!burger || !links) return;
        burger.addEventListener('click', function () {
            links.classList.toggle('open');
        });
        links.querySelectorAll('a').forEach(function (a) {
            a.addEventListener('click', function () { links.classList.remove('open'); });
        });
    }

    // ----- Reveal-on-scroll + skill bar fill -----
    function initReveal() {
        var items = document.querySelectorAll('.reveal');

        if (!('IntersectionObserver' in window)) {
            items.forEach(function (el) { el.classList.add('visible'); });
            fillBars(document);
            return;
        }

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    fillBars(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12 });

        items.forEach(function (el) { observer.observe(el); });
    }

    function fillBars(scope) {
        scope.querySelectorAll('.bar-fill').forEach(function (bar) {
            var level = bar.getAttribute('data-level');
            if (level) bar.style.width = level + '%';
        });
    }
})();
