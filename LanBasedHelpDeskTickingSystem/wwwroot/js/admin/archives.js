'use strict'

const API_URL = "/api/admin/archives";

const state = {
    limit: 10,
    currentPage: 1,
    search: "",
    type: ""
};

const elements = {};

function cacheElements() {
    elements.listContainer = document.getElementById("listContainer");
    elements.totalRows = document.getElementById("total-rows");
    elements.pagination = document.getElementById("pagination");

    elements.searchInput = document.getElementById("search");
    elements.typeFilter = document.getElementById("typeFilter");
}

function renderSkeleton() {
    elements.listContainer.innerHTML = `
        ${Array(5).fill().map(() => `
            <tr class="bg-white border-b border-gray-100">
                <td class="px-6 py-4">
                    <div class="h-4 bg-gray-300 rounded w-1/3 mb-2"></div>
                    <div class="h-3 bg-gray-200 rounded w-1/4"></div>
                </td>
                <td class="px-6 py-4">
                    <div class="h-6 bg-blue-100 rounded-md w-32"></div>
                </td>
                <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-20"></div></td>
                <td class="px-6 py-4"><div class="h-4 bg-gray-200 rounded w-20"></div></td>
                <td class="px-6 py-4">
                    <div class="flex gap-4">
                        <div class="h-4 bg-blue-50 rounded w-12"></div>
                        <div class="h-4 bg-red-50 rounded w-12"></div>
                    </div>
                </td>
            </tr>
        `)}
    `;
}

function getTypeBadge(type) {
    switch (type) {
        case "Kb":
            return `<span class="bg-blue-100 text-blue-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded dark:bg-blue-700 dark:text-blue-300">Knowledge Base</span>`;
        case "Ticket":
            return `<span class="bg-green-100 text-green-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded dark:bg-green-700 dark:text-green-300">Ticket</span>`;
        default:
            return `<span class="bg-gray-100 text-gray-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded dark:bg-gray-700 dark:text-gray-300">Unknown</span>`;
    }
}

function createItemHtml(item) {
    return `
            <tr class="bg-white border-b dark:bg-gray-800 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
                <th scope="row" class="px-6 py-4 font-medium text-gray-900 whitespace-nowrap dark:text-white">
                    ${item.type === "Kb" ? `
                        ${item.title}
                        <p class="text-xs font-normal text-gray-500 dark:text-gray-400">${String(item.content)}</p>
                    ` : `
                        ${item.title}
                        <p class="text-xs font-normal text-gray-500 dark:text-gray-400">${item.description}</p>
                    `}
                </th>
                <td class="px-6 py-4">${getTypeBadge(item.type)}</td>
                <td class="px-6 py-4">${new Date(item.createdAt).toLocaleDateString()}</td>
                <td class="px-6 py-4">${new Date(item.updatedAt).toLocaleDateString()}</td>
                <td class="px-6 py-4 flex gap-4">
                    <button data-id="${item.id}" data-type="${item.type}" onclick="handleActionClick(this)" class="font-medium text-blue-600 hover:underline">Restore</button>
                    <button data-id="${item.id}" data-type="${item.type}" onclick="handleActionClick(this)" class="font-medium text-red-600 hover:underline">Delete</button>
                </td>
            </tr>
`;
}

function renderEmpty() {
    return `
        <tr>
            <td colspan="5">
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

async function fetchAllArchives(page) {
    state.currentPage = page;

    const params = new URLSearchParams({
        page: state.currentPage,
        limit: state.limit
    });
    if (state.search) params.append('search', state.search);
    if (state.type) params.append('type', state.type);

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
    
    const result = await fetchAllArchives(page);
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

async function handleArchiveChange(e) {
    state.type = e.target.value === "All Types" ? "" : e.target.value;
    await loadLists(1);
}

async function handleActionClick(e)
{
    const id = e.dataset.id || -1;
    const type = e.dataset.type;

    const response = await fetch(API_URL, {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        credentials: "include",
        body: JSON.stringify({ 
            Id: id, 
            Type: type,
            Action: e.innerText
        })
    });

    const jsonData = await response.json();

    if (response.ok) {
        showToast({
            message: jsonData.message,
            type: "success",
            duration: 3000
        });

        await loadLists(1);
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

    elements.searchInput.addEventListener("input", debounce(async (e) => handleSearch(e.target.value), 700));
    elements.typeFilter.addEventListener("change", handleArchiveChange);

    await loadLists(state.currentPage);
});