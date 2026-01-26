window.clickElement = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        // Safari requires showPicker() for color inputs
        if (typeof element.showPicker === 'function') {
            try {
                element.showPicker();
                return;
            } catch (e) {
                // Fallback if showPicker fails
            }
        }
        // Fallback for older browsers
        element.click();
    }
};