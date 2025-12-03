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
    
    elements.comment = document.getElementById("comment");
    elements.commentLists = document.getElementById("comment-lists");
    elements.commentBtn = document.getElementById("add-comment-btn");
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
    const priority = elements.selectPriority.value;
    const assignedTo = elements.assignedTo.value;

    const response = await fetch(API_URL, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            ticketId: ticketId,
            status: status,
            priority: priority,
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

async function initComments() {
    // const commentElements = document.querySelectorAll(".comment-content img");
    //
    // commentElements.forEach(img => {
    //     img.style.cursor = "zoom-in";
    //     img.addEventListener("click", () => openZoom(img));
    // });
    //
    // elements.overlay.addEventListener("click", closeZoom);
}

async function handleCommentAdd(e) {
    e.preventDefault();
    
    const response = await fetch(`/api/comments`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "include": "credentials"
        },
        body: JSON.stringify({
            ticketId: elements.commentBtn.dataset.id,
            content: elements.comment.value
        })
    });
    
    const jsonData = await response.json();
    
    if (response.ok) {
        // showToast({
        //     message: jsonData.message,
        //     type: "success",
        //     duration: 3000
        // });
        
        const data = JSON.parse(jsonData.message);
        
        
        console.log(data);
        
        elements.commentLists.innerHTML += `
            <div class="flex gap-3">
                <img class="w-10 h-10 rounded-full" src="https://placehold.co/100x100/dbeafe/3b82f6?text=A" alt="Admin avatar">
                <div class="flex-1 bg-blue-50 border border-blue-200 rounded-lg p-4">
                    <div class="flex items-center justify-between mb-1">
                        <div class="font-semibold text-gray-900">
                            ${data.User.Username}
                            <span class="ml-2 bg-blue-100 text-blue-800 text-xs font-medium px-2 py-0.5 rounded">${getRoleById(data.User.Roles)}</span>
                        </div>
                        <span class="text-xs text-gray-500">${data.User.CreatedAt}</span>
                    </div>
                    <p class="text-sm text-gray-700">${data.Content}</p>
                </div>
            </div>
        `;

        // setTimeout(() => {
        //     window.location.reload();
        // }, 1000);
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
    
    await initComments();
    
    elements.updateBtn.addEventListener("click", handleUpdateTicket);
    elements.commentBtn.addEventListener("click", handleCommentAdd);
});