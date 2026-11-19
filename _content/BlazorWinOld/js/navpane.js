const SCROLLER_SELECTOR = '.navpane-group-scroller';
const ITEMS_SELECTOR = ':scope > .navpane-group-items';
const BUTTON_SELECTOR = '.navpane-scroll-btn';
const EPSILON = 1; // subpixel tolerance at both ends of the scroll range

// Office 97 Outlook bar: instead of a scrollbar, small arrow buttons appear over the open group
// only when items are hidden on that side, and each click scrolls by one item.
export function initNavPaneScroll(paneEl) {
    let scheduled = false;

    function schedule() {
        if (scheduled) return;
        scheduled = true;
        requestAnimationFrame(() => {
            scheduled = false;
            update();
        });
    }

    function update() {
        for (const scroller of paneEl.querySelectorAll(SCROLLER_SELECTOR)) {
            const items = scroller.querySelector(ITEMS_SELECTOR);
            const up = scroller.querySelector(':scope > .navpane-scroll-up');
            const down = scroller.querySelector(':scope > .navpane-scroll-down');
            if (!items || !up || !down) continue;

            // observe() is idempotent: picks up the items container of a newly opened group.
            resizeObserver.observe(items);
            up.hidden = items.scrollTop <= EPSILON;
            down.hidden = items.scrollTop + items.clientHeight >= items.scrollHeight - EPSILON;
        }
    }

    function onClick(e) {
        const btn = e.target.closest(BUTTON_SELECTOR);
        if (!btn || !paneEl.contains(btn)) return;

        const items = btn.parentElement.querySelector(ITEMS_SELECTOR);
        if (!items) return;

        // Snap to the next (or previous) item so it sits at the top, whatever the item heights.
        const top = items.getBoundingClientRect().top + (parseFloat(getComputedStyle(items).paddingTop) || 0);
        const offsets = Array.from(items.children, child => child.getBoundingClientRect().top - top);
        const offset = btn.classList.contains('navpane-scroll-up')
            ? offsets.filter(o => o < -EPSILON).pop()
            : offsets.find(o => o > EPSILON);
        if (offset !== undefined) items.scrollBy({ top: offset, behavior: 'smooth' });
    }

    const resizeObserver = new ResizeObserver(() => schedule());
    resizeObserver.observe(paneEl);

    // Attributes are deliberately NOT observed: the `hidden` toggles above would loop otherwise.
    const mutationObserver = new MutationObserver(() => schedule());
    mutationObserver.observe(paneEl, { childList: true, subtree: true, characterData: true });

    // scroll doesn't bubble: listen in the capture phase to catch the groups' own scrolling.
    paneEl.addEventListener('scroll', schedule, true);
    paneEl.addEventListener('click', onClick);
    document.fonts?.ready.then(schedule);

    schedule();

    return {
        dispose: () => {
            resizeObserver.disconnect();
            mutationObserver.disconnect();
            paneEl.removeEventListener('scroll', schedule, true);
            paneEl.removeEventListener('click', onClick);
        }
    };
}

// Narrow screens (e.g. a phone in portrait): reports whether the viewport is below maxWidthPx now,
// then calls back only when the breakpoint is crossed, so a manual toggle is never overridden.
export function initNavPaneAutoCollapse(dotNetRef, maxWidthPx) {
    const mql = window.matchMedia(`(max-width: ${maxWidthPx - 0.02}px)`);
    const onChange = e => dotNetRef.invokeMethodAsync('OnBreakpointChanged', e.matches);
    mql.addEventListener('change', onChange);

    return {
        matches: () => mql.matches,
        dispose: () => mql.removeEventListener('change', onChange)
    };
}
