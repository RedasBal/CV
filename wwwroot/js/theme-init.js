// Applies the saved theme before the body paints to avoid a flash of the wrong
// theme. Loaded synchronously in <head> so no inline script is needed — this
// lets the Content-Security-Policy forbid inline scripts entirely.
(function () {
    try {
        var t = localStorage.getItem('theme');
        if (t) document.documentElement.setAttribute('data-theme', t);
    } catch (e) { /* localStorage blocked — fall back to default theme */ }
})();
