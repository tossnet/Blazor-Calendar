const BTN_SELECTOR = ':scope > .toolbar-btn-win';
const COLLAPSED_CLASS = 'toolbar-btn-collapsed-win';
const ICON_ONLY_CLASS = 'toolbar-btn-icon-only-win';
const EPSILON = 0.5; // subpixel tolerance to avoid float-jitter thrash at the fit boundary

export function initToolbarCollapse(toolbarEl) {
    let scheduled = false;

    function schedule() {
        if (scheduled) return;
        scheduled = true;
        requestAnimationFrame(() => {
            scheduled = false;
            recalc();
        });
    }

    function recalc() {
        const buttons = Array.from(toolbarEl.querySelectorAll(BTN_SELECTOR));

        // Un-collapse everything first: gives labels back once the container regains
        // room, and makes every pass idempotent regardless of the previous state.
        for (const btn of buttons) btn.classList.remove(COLLAPSED_CLASS);

        let overflow = toolbarEl.scrollWidth - toolbarEl.clientWidth;
        if (overflow <= EPSILON) return;

        // Trailing-first candidates. Buttons already icon-only (own or ancestor
        // IconOnly="true") have nothing left to give up, so they're skipped.
        for (let i = buttons.length - 1; i >= 0 && overflow > EPSILON; i--) {
            const btn = buttons[i];
            if (btn.classList.contains(ICON_ONLY_CLASS)) continue;

            // Once the row overflows, every button already sits at its flex-shrink
            // floor (min-width), so a before/after measurement around this single
            // class toggle captures exactly what this button gives back, without
            // the script needing to know any theme's pixel constants.
            const before = btn.getBoundingClientRect().width;
            btn.classList.add(COLLAPSED_CLASS);
            const after = btn.getBoundingClientRect().width;
            overflow -= (before - after);
        }
        // If every eligible button is now collapsed and overflow is still > 0,
        // the toolbar's own overflow-x:auto (set in WinOldToolbar.razor.css) is
        // the fallback — no further JS action here.
    }

    const resizeObserver = new ResizeObserver(() => schedule());
    resizeObserver.observe(toolbarEl);

    // Attributes is deliberately NOT observed: it's what keeps this module from
    // ever reacting to (and looping on) its own classList.add/remove above.
    const mutationObserver = new MutationObserver(() => schedule());
    mutationObserver.observe(toolbarEl, { childList: true, subtree: true, characterData: true });

    schedule(); // initial pass, in case the toolbar already overflows on first paint

    return {
        dispose: () => {
            resizeObserver.disconnect();
            mutationObserver.disconnect();
        }
    };
}
