(function () {
    'use strict';

    // Small helpers (used by pages; keeps code organized)
    window.SiteAjax = {
        postJson: function (url, obj) {
            return fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(obj)
            }).then(function (r) { return r.json(); });
        },
        getJson: function (url) {
            return fetch(url).then(function (r) { return r.json(); });
        }
    };
})();