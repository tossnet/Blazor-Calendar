export function clampContextMenu(el) {
    const vw = window.innerWidth;
    const vh = window.innerHeight;
    const w = el.offsetWidth;
    const h = el.offsetHeight;
    let left = parseFloat(el.style.left) || 0;
    let top = parseFloat(el.style.top) || 0;
    if (left + w > vw) left = Math.max(0, vw - w);
    if (top + h > vh) top = Math.max(0, vh - h);
    el.style.left = left + 'px';
    el.style.top = top + 'px';
}

export function positionSubmenu(el) {
    if (!el) return;

    // Repart d'un état neutre à chaque appel pour re-mesurer la position "naturelle" :
    // évite qu'une classe flip posée avant un redimensionnement de fenêtre ne reste
    // "collée" lors d'une réouverture ultérieure.
    el.classList.remove('flip-h', 'flip-v');

    const rect = el.getBoundingClientRect();
    const vw = window.innerWidth;
    const vh = window.innerHeight;

    if (rect.right > vw) el.classList.add('flip-h');
    if (rect.bottom > vh) el.classList.add('flip-v');
}

export function resetPosition(el) {
    el.style.position = '';
    el.style.left = '';
    el.style.top = '';
    el.style.margin = '';
    el.style.zIndex = '';
}

// Sur mobile, si la page déborde horizontalement, le navigateur élargit le "layout viewport" :
// position:fixed/inset:0 et 100vw le suivent, et une fenêtre centrée sort de l'écran à droite.
// On expose donc la zone réellement visible (visualViewport) en variables CSS sur <html>,
// utilisées par les backdrops. Compteur partagé : plusieurs fenêtres peuvent être ouvertes.
const VV_VARS = ['--winold-vv-left', '--winold-vv-top', '--winold-vv-width', '--winold-vv-height'];
let vvUsers = 0;

function updateVisualViewport() {
    const vv = window.visualViewport;
    if (!vv) return;
    const s = document.documentElement.style;
    s.setProperty('--winold-vv-left', vv.offsetLeft + 'px');
    s.setProperty('--winold-vv-top', vv.offsetTop + 'px');
    s.setProperty('--winold-vv-width', vv.width + 'px');
    s.setProperty('--winold-vv-height', vv.height + 'px');
}

export function trackVisualViewport() {
    if (!window.visualViewport) return;
    if (vvUsers++ === 0) {
        updateVisualViewport();
        window.visualViewport.addEventListener('resize', updateVisualViewport);
        window.visualViewport.addEventListener('scroll', updateVisualViewport);
    }
}

export function untrackVisualViewport() {
    if (!window.visualViewport || vvUsers === 0) return;
    if (--vvUsers === 0) {
        window.visualViewport.removeEventListener('resize', updateVisualViewport);
        window.visualViewport.removeEventListener('scroll', updateVisualViewport);
        VV_VARS.forEach(v => document.documentElement.style.removeProperty(v));
    }
}

// Zone visible dans les coordonnées de position:fixed (layout viewport).
function visibleArea() {
    const vv = window.visualViewport;
    return vv
        ? { left: vv.offsetLeft, top: vv.offsetTop, width: vv.width, height: vv.height }
        : { left: 0, top: 0, width: window.innerWidth, height: window.innerHeight };
}

export function initDraggable(windowEl, titleBarEl) {
    // Bloque le scroll natif sur la barre de titre (pour tactile)
    titleBarEl.style.touchAction = 'none';

    const hasFinePointer = window.matchMedia('(pointer: fine)').matches;
    if (hasFinePointer) titleBarEl.style.cursor = 'grab';

    let isDragging = false, offsetX = 0, offsetY = 0;

    function onDown(e) {
        if (e.target.closest('button, a, input, select')) return;

        // Ne fige la position (sortie du flux flex de centrage) qu'au moment
        // où l'utilisateur commence réellement à dragger, pour ne jamais figer
        // une position calculée avant que le contenu (ex: liste scrollable) ait
        // atteint sa hauteur finale.
        const r = windowEl.getBoundingClientRect();
        windowEl.style.position = 'fixed';
        windowEl.style.zIndex = '999999';
        windowEl.style.left = r.left + 'px';
        windowEl.style.top = r.top + 'px';
        windowEl.style.margin = '0';

        isDragging = true;
        titleBarEl.setPointerCapture(e.pointerId);
        if (hasFinePointer) titleBarEl.style.cursor = 'grabbing';
        offsetX = e.clientX - r.left;
        offsetY = e.clientY - r.top;
        e.preventDefault();
    }

    function onMove(e) {
        if (!isDragging) return;
        const a = visibleArea();
        const w = windowEl.offsetWidth, h = windowEl.offsetHeight;
        windowEl.style.left = Math.max(a.left, Math.min(e.clientX - offsetX, a.left + a.width - w)) + 'px';
        windowEl.style.top = Math.max(a.top, Math.min(e.clientY - offsetY, a.top + a.height - h)) + 'px';
    }

    function onUp() {
        isDragging = false;
        if (hasFinePointer) titleBarEl.style.cursor = 'grab';
    }

    titleBarEl.addEventListener('pointerdown', onDown);
    titleBarEl.addEventListener('pointermove', onMove);
    titleBarEl.addEventListener('pointerup', onUp);
    titleBarEl.addEventListener('pointercancel', onUp);

    return {
        dispose: () => {
            titleBarEl.removeEventListener('pointerdown', onDown);
            titleBarEl.removeEventListener('pointermove', onMove);
            titleBarEl.removeEventListener('pointerup', onUp);
            titleBarEl.removeEventListener('pointercancel', onUp);
        }
    };
}