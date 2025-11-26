function showToast({
                       message = "",
                       type = "success", // success, danger, warning, info
                       duration = 3000
                   }) {
    const container = document.getElementById("toast-container");

    const colors = {
        success: "bg-green-100 text-green-800 border-green-300",
        danger: "bg-red-100 text-red-800 border-red-300",
        warning: "bg-yellow-100 text-yellow-800 border-yellow-300",
        info: "bg-blue-100 text-blue-800 border-blue-300",
    };

    const icons = {
        success: `<svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7"></path></svg>`,
        danger: `<svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 9v2m0 4h.01M5 13a7 7 0 1 1 14 0 7 7 0 0 1-14 0z"></path></svg>`,
        warning: `<svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 9v2m0 4h.01M9.879 4.879l-6.36 11.04A2 2 0 0 0 5.36 19h13.28a2 2 0 0 0 1.738-3.081l-6.36-11.04a2 2 0 0 0-3.478 0z" /></svg>`,
        info: `<svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M13 16h-1v-4h-1m1-4h.01M12 8v.01m0 4v4m0 4a9 9 0 1 1 0-18 9 9 0 0 1 0 18z"></path></svg>`,
    };

    const toast = document.createElement("div");
    toast.className = `
        flex items-center w-full max-w-xs p-4 mb-2 text-sm border rounded-lg shadow
        ${colors[type] ?? colors.success}
    `;

    toast.innerHTML = `
        <div class="inline-flex items-center justify-center flex-shrink-0 w-6 h-6 me-3">
            ${icons[type] ?? icons.success}
        </div>
        <div class="flex-1">
            ${message}
        </div>
        <button type="button" class="ms-3 text-gray-500 hover:text-gray-700" aria-label="Close">
            &times;
        </button>
    `;

    container.appendChild(toast);

    // Remove on click
    toast.querySelector("button").addEventListener("click", () => {
        toast.remove();
    });

    // Auto remove
    setTimeout(() => {
        toast.classList.add("opacity-0", "transition-opacity", "duration-500");
        setTimeout(() => toast.remove(), 500);
    }, duration);
}


window.showToast = showToast;