const SCROLL_CLASSES = ['scroll-win-dos', 'scroll-win-31', 'scroll-win-98', 'scroll-win-xp', 'scroll-win-7', 'scroll-win-10'];

// Swaps the scroll-win-* class of the targets (the whole page when no selector is given).
// An empty className restores the native scrollbar.
export function setScrollbarClass(selector, className) {
    const targets = selector ? document.querySelectorAll(selector) : [document.documentElement];

    for (const el of targets) {
        el.classList.remove(...SCROLL_CLASSES);
        if (className) {
            el.classList.add(className);
        }

        // Chromium keeps drawing the previous ::-webkit-scrollbar until the scroller is laid out again:
        // toggle overflow for one layout, keeping the scroll position.
        const { scrollTop, scrollLeft } = el;
        const overflow = el.style.overflow;
        el.style.overflow = 'hidden';
        void el.offsetWidth;
        el.style.overflow = overflow;
        el.scrollTop = scrollTop;
        el.scrollLeft = scrollLeft;
    }
}
