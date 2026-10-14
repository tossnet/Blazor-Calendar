const states = new WeakMap();
export function configure(root, enabled, startHour, showCounts, week) {
    if (!enabled) { dispose(root); return; }
    let state = states.get(root);
    if (!state) {
        state = { frame: 0, signature: null, initialized: false };
        state.schedule = () => {
            if (state.frame) return;
            state.frame = requestAnimationFrame(() => { state.frame = 0; update(root, state); });
        };
        state.click = event => {
            const button = event.target.closest('.week-overflow');
            if (!button || !root.contains(button)) return;
            event.stopPropagation();
            const up = button.classList.contains('week-up');
            const column = button.closest('.day-column');
            const bounds = visibleBounds(root, column);
            const targets = [...column.querySelectorAll('.hour-task')].map(e => e.getBoundingClientRect())
                .filter(r => up ? r.top < bounds.top - 1 : r.bottom > bounds.bottom + 1);
            targets.sort((a,b) => up ? b.top-a.top : a.bottom-b.bottom);
            if (targets.length) root.scrollTo({ top: root.scrollTop +
                (up ? targets[0].top-bounds.top : targets[0].bottom-bounds.bottom), behavior: 'smooth' });
        };
        root.addEventListener('scroll', state.schedule, { passive:true });
        root.addEventListener('click', state.click);
        state.resize = new ResizeObserver(state.schedule); state.resize.observe(root);
        state.mutations = new MutationObserver(state.schedule);
        state.mutations.observe(root, { childList:true, subtree:true, characterData:true });
        states.set(root, state);
    }
    const signature = `${week}/${startHour}`;
    if (signature !== state.signature) { state.signature = signature; state.initialized = false; }
    state.startHour = startHour; state.showCounts = showCounts;
    state.schedule();
}
function visibleBounds(root, column) {
    const r = root.getBoundingClientRect();
    return { top:r.top + column.querySelector('.day-header').getBoundingClientRect().height,
             bottom:r.top + root.clientHeight };
}
function update(root, state) {
    if (!root.isConnected || !root.clientHeight) return;
    const firstHour = root.querySelector('.hour');
    if (!firstHour) return;
    if (!state.initialized) {
        root.scrollTop = state.startHour * firstHour.getBoundingClientRect().height;
        state.initialized = true;
    }
    for (const column of root.querySelectorAll('.day-column')) {
        const bounds = visibleBounds(root, column);
        const tasks = [...column.querySelectorAll('.hour-task')].map(e=>e.getBoundingClientRect());
        for (const [selector,up] of [['.week-up',true],['.week-down',false]]) {
            const button = column.querySelector(selector);
            if (!button) continue;
            const count = state.showCounts ? tasks.filter(r=>up ? r.top < bounds.top-1 : r.bottom > bounds.bottom+1).length : 0;
            button.hidden = count===0;
            const text = `${up?'▲':'▼'} ${count}`;
            if (button.textContent!==text) button.textContent=text;
            button.setAttribute('aria-label', `${count} ${up?'earlier':'later'} events`);
            button.title = `${count} events extend ${up?'above':'below'} the visible hours`;
        }
    }
}
export function dispose(root) {
    const state = states.get(root);
    if (!state) return;
    root.removeEventListener('scroll', state.schedule); root.removeEventListener('click', state.click);
    state.resize.disconnect(); state.mutations.disconnect();
    if (state.frame) cancelAnimationFrame(state.frame);
    states.delete(root);
}
