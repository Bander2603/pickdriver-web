window.pickdriverGoogleAuth = (() => {
  let dotNetRef = null;
  let clientId = null;
  let isInitialized = false;

  const waitForGoogle = (onReady, attempts = 0) => {
    if (window.google && google.accounts && google.accounts.id) {
      onReady();
      return;
    }

    if (attempts > 20) {
      console.error("Google Identity Services no esta disponible.");
      return;
    }

    setTimeout(() => waitForGoogle(onReady, attempts + 1), 200);
  };

  const handleCredential = (response) => {
    if (!dotNetRef || !response || !response.credential) {
      return;
    }

    dotNetRef.invokeMethodAsync("OnGoogleCredential", response.credential);
  };

  const initialize = (newClientId, dotNetHelper) => {
    if (!newClientId) {
      return;
    }

    clientId = newClientId;
    dotNetRef = dotNetHelper;

    waitForGoogle(() => {
      if (isInitialized) {
        return;
      }

      google.accounts.id.initialize({
        client_id: clientId,
        callback: handleCredential,
      });

      isInitialized = true;
    });
  };

  const renderButton = (elementId, options) => {
    if (!elementId) {
      return;
    }

    waitForGoogle(() => {
      const target = document.getElementById(elementId);
      if (!target) {
        return;
      }

      target.innerHTML = "";
      google.accounts.id.renderButton(target, options || { theme: "outline", size: "large" });
    });
  };

  return {
    initialize,
    renderButton,
  };
})();
