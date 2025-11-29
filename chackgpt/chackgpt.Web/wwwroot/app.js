// JavaScript interop functions for ChackGPT Web

// Setup ESC key handler for SlidePopup component
window.setupSlidePopupKeyHandler = function (dotNetHelper) {
    // Remove existing handler if any
    if (window.slidePopupKeyHandler) {
        document.removeEventListener('keydown', window.slidePopupKeyHandler);
    }

    // Create and store the handler
    window.slidePopupKeyHandler = function (e) {
        if (e.key === 'Escape' || e.key === 'Esc') {
            dotNetHelper.invokeMethodAsync('HandleEscapeKey');
        }
    };

    // Add the event listener
    document.addEventListener('keydown', window.slidePopupKeyHandler);
};

// Setup chat input Enter key handler
window.setupChatInput = function (element, dotNetHelper) {
    element.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            // Get current value and send it
            dotNetHelper.invokeMethodAsync('SendFromJS', element.value);
        }
    });
};

// Scroll chat container to bottom
window.scrollToBottom = function () {
    // find a div with class stack-vertical and set to element
    const element = document.querySelector('div.stack-vertical');
    if (element) {
        // Use requestAnimationFrame to ensure DOM is updated before scrolling
        requestAnimationFrame(() => {
            element.scrollTop = element.scrollHeight;
        });
    }
};
