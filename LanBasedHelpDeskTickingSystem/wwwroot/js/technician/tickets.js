'use strict'

const API_URL = "/api/technician/tickets";

const state = {
    limit: 10,
    currentPage: 1,
    search: "",
    category: "",
    status: "open",
    priority: ""
};

const elements = {};

function cacheElements() {
    elements.listContainer = document.getElementById("listContainer");
    elements.totalRows = document.getElementById("total-rows");
    elements.pagination = document.getElementById("pagination");

    elements.searchInput = document.getElementById("search");
    elements.categoryFilter = document.getElementById("categoryFilter");
    elements.statusFilter = document.getElementById("statusFilter");
    elements.priorityFilter = document.getElementById("priorityFilter");
}

function renderSkeleton() {
    elements.listContainer.innerHTML = `
        <tr class="bg-white border-b">
            <th scope="row" class="px-6 py-4">
                <div class="h-4 bg-gray-300 rounded w-3/4 mb-2.5"></div>
                <div class="h-3 bg-gray-200 rounded w-100"></div>
            </th>
            <td class="px-6 py-4">
                <div class="h-4 bg-gray-200 rounded w-24"></div>
            </td>
            <td class="px-6 py-4">
                <div class="h-6 bg-blue-100 rounded w-16"></div>
            </td>
            <td class="px-6 py-4">
                <div class="h-6 bg-gray-100 rounded w-20"></div>
            </td>
            <td class="px-6 py-4">
                <div class="h-6 bg-green-100 rounded w-16"></div>
            </td>
            <td class="px-6 py-4">
                <div class="h-4 bg-gray-200 rounded w-24"></div>
            </td>
            <td class="px-6 py-4 text-right">
                <div class="h-9 bg-gray-100 rounded border border-gray-200 w-24 ml-auto"></div>
            </td>
        </tr>
    
        <tr class="bg-white border-b">
            <td class="px-6 py-4">
                <div class="h-4 bg-gray-300 rounded w-2/3 mb-2.5"></div>
                <div class="h-3 bg-gray-200 rounded w-full"></div>
            </td>
            <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-24"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-blue-100 rounded w-16"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-gray-100 rounded w-20"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-green-100 rounded w-16"></div></td>
            <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-24"></div></td>
            <td class="px-6 py-4 text-right"><div class="h-9 bg-gray-100 rounded border border-gray-200 w-24 ml-auto"></div></td>
        </tr>
    
        <tr class="bg-white border-b">
            <td class="px-6 py-4">
                <div class="h-4 bg-gray-300 rounded w-1/2 mb-2.5"></div>
                <div class="h-3 bg-gray-200 rounded w-3/4"></div>
            </td>
            <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-24"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-blue-100 rounded w-16"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-gray-100 rounded w-20"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-red-100 rounded w-16"></div></td>
            <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-24"></div></td>
            <td class="px-6 py-4 text-right"><div class="h-9 bg-gray-100 rounded border border-gray-200 w-24 ml-auto"></div></td>
        </tr>
    
        <tr class="bg-white border-b">
            <td class="px-6 py-4">
                <div class="h-4 bg-gray-300 rounded w-1/4 mb-2.5"></div>
                <div class="h-3 bg-gray-200 rounded w-1/3"></div>
            </td>
            <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-24"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-blue-100 rounded w-16"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-gray-100 rounded w-24"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-red-100 rounded w-16"></div></td>
            <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-24"></div></td>
            <td class="px-6 py-4 text-right"><div class="h-9 bg-gray-100 rounded border border-gray-200 w-24 ml-auto"></div></td>
        </tr>
         <tr class="bg-white border-b">
            <td class="px-6 py-4">
                <div class="h-4 bg-gray-300 rounded w-1/3 mb-2.5"></div>
                <div class="h-3 bg-gray-200 rounded w-1/3"></div>
            </td>
            <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-24"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-blue-100 rounded w-16"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-gray-100 rounded w-20"></div></td>
            <td class="px-6 py-4"><div class="h-6 bg-green-100 rounded w-16"></div></td>
            <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-24"></div></td>
            <td class="px-6 py-4 text-right"><div class="h-9 bg-gray-100 rounded border border-gray-200 w-24 ml-auto"></div></td>
        </tr>
    `;
}

function createItemHtml(item) {
    return `
            <tr class="bg-white border-b dark:bg-gray-800 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
                <th scope="row" class="px-6 py-4 font-medium text-gray-900 whitespace-nowrap dark:text-white">
                    ${item.title}
                    <p class="text-xs font-normal text-gray-500 dark:text-gray-400">${item.description}</p>
                </th>
                <td class="px-6 py-4">${item.requester?.username ?? "Unknown User"}</td>
                <td class="px-6 py-4">
                    ${getStatusBadge(item.status)}
                </td>
                <td class="px-6 py-4">
                    <span class="bg-gray-100 text-gray-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded dark:bg-gray-700 dark:text-gray-300">${item.category.name}</span>
                </td>
                <td class="px-6 py-4">
                    ${getPriorityBadge(item.priority)}
                </td>
                <td class="px-6 py-4">${new Date(item.createdAt).toLocaleDateString()}</td>
                <td class="px-6 py-4">
                    <div>
                        <a href="/Technician/Tickets/${item.id}" class="text-gray-900 bg-white border border-gray-300 focus:outline-none hover:bg-gray-100 focus:ring-4 focus:ring-gray-100 font-medium rounded-lg text-sm px-5 py-2 dark:bg-gray-800 dark:text-white dark:border-gray-600 dark:hover:bg-gray-700 dark:hover:border-gray-600 dark:focus:ring-gray-700">
                            View Details
                        </a>
                    </div>
                </td>
            </tr>
`;
}

function renderEmpty() {
    return `
        <tr>
            <td colspan="7">
                <div class="flex flex-col items-center py-8">
                <svg class="w-12 h-12 mb-3 text-gray-400" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 20 20">
                    <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 11V6m0 8h.01M19 10a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z"/>
                </svg>
                
                <h5 class="mb-2 text-2xl font-bold text-gray-900 dark:text-white">
                    No Items Found
                </h5>
                
                <p class="text-base text-gray-500 dark:text-gray-400 text-center">
                    It looks like there are no items in this list yet.
                </p>
            </div>
            </td>
        </tr>
        `;
}

async function fetchAllTickets(page) {
    state.currentPage = page;

    const params = new URLSearchParams({
        page: state.currentPage,
        limit: state.limit
    });
    if (state.search) params.append('search', state.search);
    if (state.category) params.append('category', state.category);
    if (state.status) params.append('status', state.status);
    if (state.priority) params.append('priority', state.priority);

    try {
        const response = await fetch(`${API_URL}?${params.toString()}`, {
            method: "GET",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        });

        if (!response.ok) return;

        return await response.json();
    } catch (error) {
        console.error(error);
        return null;
    }
}

async function loadLists(page) {
    renderSkeleton();

    const result = await fetchAllTickets(page);
    if (!result) return;

    if (result.data && result.data.length > 0) {
        elements.listContainer.innerHTML = result.data.map(createItemHtml).join('');
    } else {
        elements.listContainer.innerHTML = renderEmpty();
    }

    elements.totalRows.innerText = result.data.length;

    if (typeof buildPagination === "function") buildPagination(page, result.totalPages);
}

async function handleSearch(value) {
    state.search = value;
    await loadLists(1);
}

async function handleCategoryChange(e) {
    state.category = e.target.value === "All Categories" ? "" : e.target.value;
    await loadLists(1);
}

async function handleStatusChange(e) {
    state.status = e.target.value === "All Status" ? "" : e.target.value;
    await loadLists(1);
}

async function handlePriorityChange(e) {
    state.priority = e.target.value === "All Priorities" ? "" : e.target.value;
    await loadLists(1);
}

document.addEventListener("DOMContentLoaded", async () => {
    cacheElements();

    elements.searchInput.addEventListener("input", debounce(async (e) => handleSearch(e.target.value), 700));
    elements.categoryFilter.addEventListener("change", handleCategoryChange);
    elements.statusFilter.addEventListener("change", handleStatusChange);
    elements.priorityFilter.addEventListener("change", handlePriorityChange);

    await loadLists(state.currentPage);
});