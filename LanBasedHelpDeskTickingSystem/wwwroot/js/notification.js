'use strict'

// const clearNotification = document.getElementById("notification-clear")

function renderEmpty() {
    return `
    <div class="flex px-4 py-3">
        <div class="w-full ps-3">
            <div class="text-body text-sm mb-1.5">No new notifications</div>
        </div>
    </div>
    `;
}

function timeAgo(date) {
    const now = new Date();
    const seconds = Math.floor((now - date) / 1000);
    const intervals = [
        { label: 'year', seconds: 31536000 },
        { label: 'month', seconds: 2592000 },
        { label: 'week', seconds: 604800 },
        { label: 'day', seconds: 86400 },
        { label: 'hour', seconds: 3600 },
        { label: 'minute', seconds: 60 },
        { label: 'second', seconds: 1 }
    ];

    for (const interval of intervals) {
        const count = Math.floor(seconds / interval.seconds);
        if (count >= 1) {
            return `${count} ${interval.label}${count > 1 ? 's' : ''} ago`;
        }
    }
    return 'just now';
}

document.addEventListener("DOMContentLoaded", async () => {

    const notificationLists = document.getElementById("notification-lists");
    const notificationBadge = document.getElementById("notification-badge");
    const clearNotification = document.getElementById("notification-clear");

    notificationLists.innerHTML = "";
    
    clearNotification.addEventListener("click", async () => {
        const clearResponse = await fetch('/api/notifications', {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });
        
        if (clearResponse.ok) {
            notificationBadge.classList.add("hidden");
            notificationLists.innerHTML = renderEmpty();
        }
    });
    
    const response = await fetch('/api/notifications', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: 'include'
    });
    
    if (response.ok) {
        const data = await response.json();
        
        if (data.length > 0)
        {
            notificationBadge.classList.remove("hidden");
            data.forEach(notification => {
                notificationLists.innerHTML += `
            <a href="/api/notifications/redirect/${notification.id}" class="flex px-4 py-3 hover:bg-neutral-secondary-medium">
                <div class="w-full ps-3">
                    <div class="text-body text-sm mb-1.5">${notification.message}</div>
                    <div class="text-xs text-fg-brand">${timeAgo(new Date(notification.created_at))}</div>
                </div>
                ${notification.is_read ? '<svg class="w-4 h-4 text-green-600 dark:text-green-300 mr-2" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 16 12">\n' +
                    '                                <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M1 5.917 5.724 10.5 15 1.5"/>\n' +
                    '                            </svg>' : ''}
            </a>
            `;
            });
        }
        else
        {
            notificationBadge.classList.add("hidden");
            notificationLists.innerHTML = renderEmpty();
        }
        
        
    } else {
        console.error('Failed to fetch notifications');
    }
    
});