// Admin editor: add/remove repeatable rows and renumber field names so the
// indexed collections bind correctly on the server. No inline script (CSP-safe).
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        // Add row
        document.querySelectorAll('[data-add]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var section = btn.getAttribute('data-add');
                var tpl = document.getElementById('tpl-' + section);
                var container = document.querySelector('.adm-rows[data-section="' + section + '"]');
                if (!tpl || !container) return;
                container.appendChild(tpl.content.cloneNode(true));
                reindex(section);
            });
        });

        // Remove row (event delegation)
        document.addEventListener('click', function (ev) {
            var rm = ev.target.closest('[data-remove]');
            if (!rm) return;
            var row = rm.closest('.adm-row');
            if (!row) return;
            var section = row.getAttribute('data-section');
            row.remove();
            reindex(section);
        });

        // Reindex everything right before submit so names are sequential.
        var form = document.getElementById('cvForm');
        if (form) {
            form.addEventListener('submit', function () {
                ['Experience', 'Projects', 'Skills', 'Languages', 'Education'].forEach(reindex);
            });
        }
    });

    function reindex(section) {
        var container = document.querySelector('.adm-rows[data-section="' + section + '"]');
        if (!container) return;
        var rows = container.querySelectorAll('.adm-row');
        rows.forEach(function (row, i) {
            row.querySelectorAll('[data-field]').forEach(function (input) {
                var field = input.getAttribute('data-field');
                input.name = 'Input.' + section + '[' + i + '].' + field;
            });
        });
    }
})();
