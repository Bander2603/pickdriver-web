window.pickDriverDraftVisibility = {
    register(dotNetReference) {
        const onVisibilityChanged = () => {
            if (document.visibilityState === "visible") {
                dotNetReference.invokeMethodAsync("OnDraftVisibleAsync");
            }
        };

        document.addEventListener("visibilitychange", onVisibilityChanged);
        window.addEventListener("focus", onVisibilityChanged);

        return {
            dispose() {
                document.removeEventListener("visibilitychange", onVisibilityChanged);
                window.removeEventListener("focus", onVisibilityChanged);
            }
        };
    }
};
