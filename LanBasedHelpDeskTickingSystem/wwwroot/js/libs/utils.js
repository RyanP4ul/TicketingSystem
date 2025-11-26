function debounce(func, delay) {
    let timeout;
    return function(...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), delay);
    };
}

function toggleSkeleton(isLoading)
{
    const skeleton = document.getElementById("skeleton");
    const listContainer = document.getElementById("listContainer");

    if (isLoading) {
        skeleton.classList.remove("hidden");
        listContainer.classList.add("hidden");
    } else {
        skeleton.classList.add("hidden");
        listContainer.classList.remove("hidden");
    }
}

function getStatusBadge(status)
{
    switch (status)
    {
        case "open": return '<span class="bg-blue-100 text-blue-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-blue-900 dark:text-blue-300">Open</span>';
        case "in_progress": return '<span class="bg-yellow-100 text-yellow-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-yellow-900 dark:text-yellow-300">In Progress</span>';
        case "pending": return '<span class="bg-gray-100 text-gray-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-gray-700 dark:text-gray-300">Pending</span>';
        case "resolved": return '<span class="bg-green-100 text-green-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-green-900 dark:text-green-300">Resolved</span>';
        case "closed": return '<span class="bg-red-100 text-red-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-red-900 dark:text-red-300">Closed</span>';
    }
}

function getPriorityBadge(priority)
{
    switch (priority)
    {
        case "low": return '<span class="bg-green-100 text-green-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-green-900 dark:text-green-300">Low</span>';
        case "medium": return '<span class="bg-yellow-100 text-yellow-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-yellow-900 dark:text-yellow-300">Medium</span>';
        case "high": return '<span class="bg-red-100 text-red-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-red-900 dark:text-red-300">High</span>';
    }
}

window.debounce = debounce;
window.toggleSkeleton = toggleSkeleton;
window.getStatusBadge = getStatusBadge;
window.getPriorityBadge = getPriorityBadge;