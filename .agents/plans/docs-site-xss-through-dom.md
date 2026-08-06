# Docs site - fix CodeQL alert #7, DOM text reinterpreted as HTML

**Status:** Not started. `docs/` is off limits to a coding session - this file is the brief for the docs session.

## The problem

GitHub CodeQL alert #7 (`js/xss-through-dom`, open on `main`) flags `docs/_js/main.js:12`, inside the
version-select change handler:

```js
const versionSelects = document.querySelectorAll('[data-versionselect]');
versionSelects.forEach(versionSelect => {
    versionSelect.addEventListener('change', function (event) {
        const url = versionSelect.dataset.versionselect;
        const selectedVersion = event.target.value;
        if (selectedVersion) {
            window.location.href = url + selectedVersion;
        }
    });
});
```

`url` and `selectedVersion` both come from the DOM (a data attribute and a `<select>` value) and are
concatenated straight into `window.location.href`. A browser will run a `javascript:` URI assigned to
`location.href` as script, so CodeQL treats any DOM-sourced string reaching that sink as a possible DOM-based
XSS - it does not know that, today, both pieces are actually safe:

- `url` is rendered by `docs/_includes/versions/sidebar.html` as `{{ '/version/' | relative_url }}` - a
  build-time constant, not visitor input.
- `selectedVersion` is one of the `<option value="...">` entries generated from `site.data.versions.list`,
  a YAML file only the maintainer edits.

So this is not exploitable as the code stands. It is still worth closing, because the fix is cheap and it
removes the sink pattern CodeQL and Copilot Autofix are both flagging - if either input source ever changes
(e.g. a query-string-driven version picker), the current code would become a real DOM XSS with no other
warning.

## The fix

In the same handler, resolve the concatenated string to a `URL` against the current origin and only navigate
if it actually resolves to that origin. This keeps today's behavior identical (the result is always a
same-origin relative path) while making a `javascript:` URI or an off-origin value fail closed instead of
navigating:

```js
const url = versionSelect.dataset.versionselect;
const selectedVersion = event.target.value;
if (selectedVersion) {
    const target = new URL(url + selectedVersion, window.location.origin);
    if (target.origin === window.location.origin) {
        window.location.href = target.href;
    }
}
```

`docs/_js/main.js` is the webpack source; it compiles to `docs/js/main.js` (watched by `just serve docs`).
Edit only the `_js/` source - the compiled copy is generated, not hand-maintained.

## Done when

- The handler in `docs/_js/main.js` uses the `URL`-based same-origin check above.
- The docs site rebuilds clean and the version selector still navigates correctly between versions.
- CodeQL alert #7 closes on the next scan after the fix merges.
