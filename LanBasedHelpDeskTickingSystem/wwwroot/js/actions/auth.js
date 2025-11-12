async function authForm(formElementId, buttonElementId, formData, endpointUrl, timerHandler) {
    document.querySelectorAll("[id^='error-']").forEach(el => el.textContent = "");
    document.querySelectorAll("input").forEach(input => {
        input.classList.remove("border-red-600");
        input.classList.add("border-gray-300");
    });

    const login_button = document.getElementById(buttonElementId);
    const temp = login_button.textContent;

    login_button.innerHTML = "Please Wait...";

        // login_button.innerHTML = `
    //     <div class="flex items-center justify-center w-full space-x-2">
    //         <svg aria-hidden="true" class="w-4 h-4 text-gray-200 animate-spin fill-blue-900" viewBox="0 0 100 101">
    //             <path d="M100 50.6C100 78.2 77.6 100.6 50 100.6S0 78.2 0 50.6 22.4 0.6 50 0.6 100 22.9 100 50.6z" fill="currentColor"/>
    //             <path d="M93.97 39.04c2.42-.64 3.89-3.13 3.04-5.49..." fill="currentFill"/>
    //         </svg>
    //         <div>Please Wait...</div>
    //     </div>`;
    
    login_button.disabled = true;

    // ✅ FIX: Send actual values, not DOM elements
    const jsonData = {};
    for (const [key, input] of Object.entries(formData)) {
        jsonData[key] = input.value;
    }

    const response = await fetch(endpointUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData)
    });

    const data = await response.json();
    
    console.log(data);

    if (!response.ok && data.errors) {
        Object.keys(data.errors).forEach(key => {
            const errorDiv = document.getElementById(`error-${key.toLowerCase()}`);
            const inputField = document.getElementById(key);
            if (errorDiv) errorDiv.textContent = data.errors[key][0];
            if (inputField) {
                inputField.classList.remove("border-gray-300");
                inputField.classList.add("border-red-600");
            }
        });
    } else {
        document.getElementById(formElementId).reset();
    }

    setTimeout(() => {
        timerHandler();
        login_button.textContent = temp;
        login_button.disabled = false;
    }, 500);
}

// ✅ Make available globally
window.authForm = authForm;
