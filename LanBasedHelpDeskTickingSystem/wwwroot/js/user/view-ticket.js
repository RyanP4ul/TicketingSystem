'use strict'

const API_URL = "/api/user/tickets";
const elements = {};
let ticketId = -1;

function cacheElements() {
    elements.confirmBtn = document.querySelector(".confirm-btn");
    elements.updateTicketBtn = document.getElementById("update-ticket-button");
    
    elements.overlay = document.getElementById("zoom-overlay");
    elements.zoomedImg = document.getElementById("zoomed-image");
    
    elements.form = document.getElementById("form");
    
    elements.removeAttachment = document.getElementById("remove-attachment");
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

async function handleRemoveAttachment() {
    
    const ticketId = elements.form.dataset.id || -1;
    const attachmentId = elements.removeAttachment.dataset.id;

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
        elements.removeAttachment.parentElement.remove();

        showToast({
            message: "Attachment removed successfully.",
            type: "success",
            duration: 3000
        });
    }
}

document.addEventListener("DOMContentLoaded", async () => {
    cacheElements();

    ticketId = elements.form.dataset.id || -1;
    
    elements.updateTicketBtn.addEventListener("click", handleFormSubmit);
    elements.confirmBtn.addEventListener("click", handleDeleteClick);
    elements.removeAttachment.addEventListener("click", handleRemoveAttachment);
});