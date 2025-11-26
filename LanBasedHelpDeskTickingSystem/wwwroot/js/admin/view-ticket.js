'use strict'

const API_URL = "/api/admin/tickets";
const elements = {};

function cacheElements() {
    elements.selectStatus = document.getElementById("select-status");
    elements.selectPriority = document.getElementById("select-priority");
    elements.assignedTo = document.getElementById("select-assigned-to");
    
    elements.updateBtn = document.getElementById("update-ticket-btn");
    elements.rejectBtn = document.getElementById("reject-ticket-btn");

    elements.overlay = document.getElementById("zoom-overlay");
    elements.zoomedImg = document.getElementById("zoomed-image");
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

async function handleUpdateTicket(e) {
    e.preventDefault();
    
    const ticketId = elements.updateBtn.dataset.id;
    const status = elements.selectStatus.value;
    const assignedTo = elements.assignedTo.value;

    const response = await fetch(API_URL, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            ticketId: ticketId,
            status: status,
            assigned: assignedTo,
            notes: "none"
        })
    });
    
    const jsonData = await response.json();
    
    if (response.ok) {
        showToast({
            message: jsonData.message,
            type: "success",
            duration: 3000
        });

        setTimeout(() => {
            window.location.href = "/Admin/Tickets";
        }, 1000);
    } else {
        showToast({
            message: jsonData.message,
            type: "danger",
            duration: 3000
        });
    }
}

document.addEventListener("DOMContentLoaded", async () => {
    cacheElements();
    
    elements.updateBtn.addEventListener("click", handleUpdateTicket);
});