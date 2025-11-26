'use strict'

const API_URL = "/api/admin/users";

const state = {
    limit: 10,
    currentPage: 1,
    search: "",
    role: ""
};

const elements = {};

function cacheElements() {
    elements.listContainer = document.getElementById("listContainer");
    elements.totalRows = document.getElementById("total-rows");
    elements.pagination = document.getElementById("pagination");

    elements.searchInput = document.getElementById("search");
    elements.roleFilter = document.getElementById("roleFilter");
}

function getRoleBadge(role)
{
    switch(role)
    {
        case "Admin":
            return `<span class="bg-purple-100 text-purple-800 text-xs font-medium px-2.5 py-0.5 rounded-full">Admin</span>`;
        default:
            return `<span class="bg-gray-100 text-gray-800 text-xs font-medium px-2.5 py-0.5 rounded-full">User</span>`;
    }
}

function createItemHtml(item) {
    return `
            <tr class="bg-white border-b hover:bg-gray-50">
                <td class="px-6 py-4">
                    <div class="font-medium text-gray-900">${item.username}</div>
                    <div class="text-sm text-gray-500">${item.email}</div>
                </td>
                <td class="px-6 py-4">
                    ${getRoleBadge(item.roles)}
                </td>
                <td class="px-6 py-4">
                    <span class="bg-green-100 text-green-800 text-xs font-medium px-2.5 py-0.5 rounded-full">Active</span>
                </td>
                <td class="px-6 py-4">${new Date(item.createdAt).toLocaleDateString()}</td>
                <td class="px-6 py-4">
                    <a href="#" class="font-medium text-blue-600 hover:underline">Edit</a>
                </td>
            </tr>`;
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
async function fetchKnowledgeBase(page) {
    state.currentPage = page;

    const params = new URLSearchParams({
        page: state.currentPage,
        limit: state.limit
    });
    if (state.search) params.append('search', state.search);
    if (state.role) params.append('role', state.role);

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
    const result = await fetchKnowledgeBase(page);
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

async function handleRoleChange(e) {
    state.role = e.target.value === "All Roles" ? "" : e.target.value;
    await loadLists(1);
}

document.addEventListener("DOMContentLoaded", async () => {
    cacheElements();

    elements.searchInput.addEventListener("input", debounce(async (e) => handleSearch(e.target.value), 700));
    elements.roleFilter.addEventListener("change", handleRoleChange);

    await loadLists(state.currentPage);
});