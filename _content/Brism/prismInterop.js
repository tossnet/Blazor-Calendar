export function highlightAll() {
    return new Promise((resolve, reject) => {
        if (typeof Prism !== 'undefined') {
            Prism.highlightAll();
            resolve();
        } else {
            // Wait for Prism to be available
            const checkPrism = setInterval(() => {
                if (typeof Prism !== 'undefined') {
                    clearInterval(checkPrism);
                    Prism.highlightAll();
                    resolve();
                }
            }, 100);

            // Timeout after 5 seconds
            setTimeout(() => {
                clearInterval(checkPrism);
                reject('Prism not loaded after 5 seconds');
            }, 5000);
        }
    });
}