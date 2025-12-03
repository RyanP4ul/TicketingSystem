'use strict'

let allFiles = [];
const API_URL = "/api/user/tickets";
const elements = {};
let ticketId = -1;

function cacheElements() {
    elements.confirmBtn = document.querySelector(".confirm-btn");
    elements.updateTicketBtn = document.getElementById("update-ticket-button");
    
    elements.overlay = document.getElementById("zoom-overlay");
    elements.zoomedImg = document.getElementById("zoomed-image");
    
    elements.form = document.getElementById("form");

    elements.removeAttachments = document.querySelectorAll("#remove-attachment");
    elements.fileInput = document.getElementById('Files');
    elements.fileListContainer = document.getElementById('file-list-container');
}

function openZoom(element) {
    elements.zoomedImg.src = element.src;
    elements.overlay.classList.remove('hidden');
    elements.overlay.classList.add('flex');
    
    setTimeout(() => {
        elements.overlay.classList.remove('opacity-0');
        elements.zoomedImg.classList.remove('scale-95');
        elements.zoomedImg.classList.add('scale-100');
    }, 10);
}

function closeZoom() {
    elements.overlay.classList.add('opacity-0');
    elements.zoomedImg.classList.remove('scale-100');
    elements.zoomedImg.classList.add('scale-95');

    setTimeout(() => {
        elements.overlay.classList.remove('flex');
        elements.overlay.classList.add('hidden');
        elements.zoomedImg.src = '';
    }, 300);
}

async function handleFormSubmit(e)
{
    e.preventDefault();
    
    const formData = new FormData(elements.form);
    
    const response = await fetch(`${API_URL}/${ticketId}`, {
        method: "PUT",
        credentials: "include",
        body: formData
    });

    const jsonData = await response.json();

    if (!response.ok) {
        elements.form.querySelectorAll('input, textarea, select').forEach(field => {
            const errorDiv = document.getElementById(`error-${field.getAttribute("name").toLowerCase()}`);

            if (errorDiv) errorDiv.textContent = "";

            field.classList.remove("border-red-600");
            field.classList.add("border-gray-300");
        });

        if (jsonData.errors && typeof jsonData.errors === 'object' && !Array.isArray(jsonData.errors)) {
            Object.keys(jsonData.errors).forEach(key => {
                const errorDiv = document.getElementById(`error-${key.toLowerCase()}`);
                const inputField = elements.formFields[key];
                if (errorDiv) errorDiv.textContent = jsonData.errors[key][0];
                if (inputField) {
                    inputField.classList.remove("border-gray-300");
                    inputField.classList.add("border-red-600");
                }
            });
        } else {
            showToast({
                message: jsonData.message,
                type: "danger",
                duration: 3000
            });
        }

    } else {
        showToast({
            message: jsonData.message,
            type: "success",
            duration: 3000
        });

        setTimeout(() => {
            window.location.href = '/User/Tickets';
        }, 500);
    }
}

async function handleDeleteClick() {
    const response = await fetch(`${API_URL}/${ticketId}`, {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
        credentials: "include"
    });

    if (response.ok) {
        setTimeout(() => {
            window.location.href = "/User/Tickets";
        }, 500);
    }
}

async function handleRemoveAttachment(e) {
    const ticketId = elements.form.dataset.id || -1;
    const attachmentId = e.target.dataset.id;

    const response = await fetch(`${API_URL}/attachment`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({
            TicketId: ticketId,
            AttachmentId: attachmentId
        })
    });

    if (response.ok) {
        e.target.parentElement.remove();

        showToast({
            message: "Attachment removed successfully.",
            type: "success",
            duration: 3000
        });
    }
}

const handleFileInputChange = (e) => {
    const newFiles = Array.from(elements.fileInput.files);
    const maxFileSize = 5 * 1024 * 1024;
    const allowedTypes = ['image/jpeg', 'image/png'];

    newFiles.forEach(file => {

        if (!allowedTypes.includes(file.type)) {
            showToast({
                message: `Skipped "${file.name}": Invalid Type.`,
                type: "danger",
                duration: 3000
            });
            return;
        }

        if (file.size > maxFileSize) {
            showToast({
                message: `Skipped "${file.name}": Too big.`,
                type: "danger",
                duration: 3000
            });
            return;
        }

        const exists = allFiles.some(f => f.name === file.name && f.size === file.size);

        if (!exists) {
            allFiles.push(file);
        }
    });

    renderFileList();
}

function renderFileList() {
    elements.fileListContainer.innerHTML = '';
    
    allFiles.forEach((file, index) => {
        const imgUrl = URL.createObjectURL(file);

        const li = document.createElement('li');
        li.className = "flex items-center justify-between p-3 bg-gray-50 border border-gray-200 rounded-lg shadow-sm";
        li.innerHTML = `
                    <div class="flex items-center space-x-3 overflow-hidden">
                        <img src="${imgUrl}" class="h-10 w-10 object-cover rounded" />
                        <div class="flex flex-col min-w-0">
                            <span class="text-sm font-medium text-gray-900 truncate block max-w-xs">${file.name}</span>
                        </div>
                    </div>
                    <button type="button" onclick="removeFile(${index})" class="text-gray-400 hover:text-red-600 transition p-1">
                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
                    </button>
                `;
        elements.fileListContainer.appendChild(li);


    });
}

window.removeFile = function(index) {
    allFiles.splice(index, 1);
    renderFileList();
};

document.addEventListener("DOMContentLoaded", async () => {
    cacheElements();

    ticketId = elements.form.dataset.id || -1;
    
    elements.removeAttachments.forEach(btn => btn.addEventListener("click", handleRemoveAttachment));
    elements.updateTicketBtn.addEventListener("click", handleFormSubmit);
    elements.confirmBtn.addEventListener("click", handleDeleteClick);
    elements.fileInput.addEventListener("change", handleFileInputChange);
});