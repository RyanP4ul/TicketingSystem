'use strict'

const API_URL = "/api/technician/kb";
const MODAL_FORM_ID = "form-modal";
const MODAL_CONFIRM_ID = "confirm-article-modal";

const state = {
    limit: 10,
    currentPage: 1,
    search: "",
    category: "",
    currentArticleId: null,
    currentAction: null
};

const elements = {};

function cacheElements() {
    elements.listContainer = document.getElementById("listContainer");
    elements.totalRows = document.getElementById("total-rows");
    elements.pagination = document.getElementById("pagination");

    elements.searchInput = document.getElementById("search");
    elements.categoryFilter = document.getElementById("categoryFilter");
    elements.insertBtn = document.querySelector(".insert-btn");

    elements.articleForm = document.getElementById("form");
    elements.confirmBtn = document.querySelector(".confirm-btn");

    elements.formFields = elements.articleForm.elements;

    elements.formModalCloseBtn = document.querySelector(`#${MODAL_FORM_ID} button[data-modal-hide="${MODAL_FORM_ID}"]`);
    elements.confirmModalCloseBtn = document.querySelector(`#${MODAL_CONFIRM_ID} button[data-modal-hide="${MODAL_CONFIRM_ID}"]`);
}

function renderSkeleton()
{
    elements.listContainer.innerHTML = `
        <div class="animate-pulse">
            ${Array.from({ length: 3 }).map(() => `
                <div class="bg-white border border-gray-200 rounded-lg p-6">
                    <div class="flex justify-between items-start mb-4">
                        <div class="h-6 bg-gray-300 rounded w-1/3"></div>
                        <div class="flex gap-2">
                            <div class="h-5 bg-yellow-50 rounded w-16"></div>
                            <div class="h-5 bg-gray-100 rounded w-16"></div>
                        </div>
                    </div>
    
                    <div class="space-y-2 mb-4">
                        <div class="h-4 bg-gray-200 rounded w-3/4"></div>
                        <div class="h-4 bg-gray-200 rounded w-full"></div>
                        <div class="h-4 bg-gray-200 rounded w-1/2"></div>
                    </div>
    
                    <div class="flex gap-2 mb-6">
                        <div class="h-6 bg-blue-50 rounded-full w-20"></div>
                        <div class="h-6 bg-blue-50 rounded-full w-20"></div>
                        <div class="h-6 bg-blue-50 rounded-full w-24"></div>
                    </div>
    
                    <hr class="border-gray-100 mb-4">
    
                    <div class="flex flex-col md:flex-row justify-between items-center gap-4">
                        <div class="h-4 bg-gray-200 rounded w-48"></div>
                        
                        <div class="flex gap-2">
                            <div class="h-9 bg-gray-100 border border-gray-200 rounded-lg w-24"></div> <div class="h-9 bg-gray-100 border border-gray-200 rounded-lg w-16"></div> <div class="h-9 bg-gray-100 border border-gray-200 rounded-lg w-16"></div> </div>
                    </div>
                </div>
            `).join('')}
        </div>
    `;
}

function createTagHtml(tag) {
    return `<span class="bg-blue-100 text-blue-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded dark:bg-blue-900 dark:text-blue-300">#${tag.trim()}</span>`;
}

function createArticleHtml(item) {
    const tags = (item.tags === "none" || !item.tags) ? [] : String(item.tags).split(",");
    const publishStatus = item.isPublished ? "Published" : "Unpublished";

    return `
            <div class="block p-6 bg-white border border-gray-200 rounded-lg shadow dark:bg-gray-800 dark:border-gray-700">
                <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-3">
                    <h5 class="text-2xl font-bold tracking-tight text-gray-900 dark:text-white mb-2 sm:mb-0">${item.title}</h5>
                    <div class="flex-shrink-0 space-x-2">
                        <span class="bg-yellow-100 text-yellow-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded dark:bg-yellow-900 dark:text-yellow-300">${publishStatus}</span>
                        <span class="bg-gray-100 text-gray-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded dark:bg-gray-700 dark:text-gray-300">${item.category.name}</span>
                    </div>
                </div>
                <p class="font-normal text-gray-700 dark:text-gray-400">${item.content.substring(0, 130)}...</p>
                <div class="mt-3">
                    ${tags.map(createTagHtml).join("")}
                </div>
                <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mt-4 pt-4 border-t border-gray-200 dark:border-gray-700">
                    <div class="text-sm text-gray-500 dark:text-gray-400 mb-2 sm:mb-0">
                        <span>by <strong>${item.author.username}</strong></span>
                        <span class="mx-2">|</span>
                        <span>${new Date(item.createdAt).toLocaleDateString('en-US')}</span>
                    </div>
                    <div class="flex-shrink-0 space-x-2">
                        <button type"button" data-id="${item.id}" data-type="publish" data-modal-toggle="${MODAL_CONFIRM_ID}" class="text-blue-700 hover:text-white border border-blue-700 hover:bg-blue-800 focus:ring-4 focus:ring-blue-300 font-medium rounded-lg text-sm px-4 py-2 text-center dark:border-blue-500 dark:text-blue-500 dark:hover:text-white dark:hover:bg-blue-600 dark:focus:ring-blue-800">${publishStatus}</button>
                        <button type="button" data-id="${item.id}" data-type="edit" data-modal-toggle="${MODAL_FORM_ID}" class="edit-btn text-gray-900 hover:text-white border border-gray-800 hover:bg-gray-900 focus:ring-4 focus:ring-gray-300 font-medium rounded-lg text-sm px-4 py-2 text-center dark:border-gray-600 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 dark:focus:ring-gray-800">Edit</button>
                        <button type="button" data-id="${item.id}" data-type="delete" data-modal-toggle="${MODAL_CONFIRM_ID}" class="text-red-700 hover:text-white border border-red-700 hover:bg-red-800 focus:ring-4 focus:ring-red-300 font-medium rounded-lg text-sm px-4 py-2 text-center dark:border-red-500 dark:text-red-500 dark:hover:text-white dark:hover:bg-red-600 dark:focus:ring-red-900">Delete</button>
                    </div>
                </div>
            </div>`;
}

function renderEmpty() {
    return `
        <tr>
            <td colspan="4">
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
    if (state.category) params.append('category', state.category);

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

    const result = await fetchKnowledgeBase(page);
    if (!result) return;

    if (result.data && result.data.length > 0) {
        elements.listContainer.innerHTML = result.data.map(createArticleHtml).join('');
    } else {
        elements.listContainer.innerHTML = renderEmpty();
    }

    elements.totalRows.innerText = result.data.length;

    if (typeof initFlowbite === "function") initFlowbite();
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

function handleInsertClick() {
    state.currentArticleId = null;
    state.currentAction = 'insert';
    elements.articleForm.reset();
}

async function handleListClick(e) {
    const button = e.target.closest('button[data-id]');
    if (!button) return;

    const id = button.dataset.id;
    const type = button.dataset.type;

    if (!id || !type) return;

    state.currentArticleId = id;
    state.currentAction = type;

    if (type === 'edit') {
        try {
            const response = await fetch(`${API_URL}/${id}`, {
                method: "GET",
                headers: {"Content-Type": "application/json"},
                credentials: "include"
            });
            const result = await response.json();

            if (result.data) {
                elements.formFields.Title.value = result.data.title;
                elements.formFields.Content.value = result.data.content;
                elements.formFields.CategoryId.value = result.data.categoryId;
                elements.formFields.Tags.value = result.data.tags;
            }
        } catch (error) {
            console.error('Failed to fetch article for editing:', error);
        }
    }
}

async function handleFormSubmit(e) {
    e.preventDefault();

    const data = {
        Title: elements.formFields.Title.value,
        Content: elements.formFields.Content.value,
        CategoryId: parseInt(elements.formFields.CategoryId.value),
        Tags: elements.formFields.Tags.value
    };

    let method = 'POST';
    if (state.currentAction === 'edit') {
        data.id = state.currentArticleId;
        method = 'PUT';
    }

    const response = await fetch(API_URL, {
        method: method,
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify(data),
        credentials: "include"
    });

    const jsonData = await response.json();

    if (!response.ok) {


        elements.articleForm.querySelectorAll('input, textarea, select').forEach(field => {
            const errorDiv = document.getElementById(`error-${String(field.attributes.name).toLowerCase()}`);

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
        if (elements.formModalCloseBtn) elements.formModalCloseBtn.click();

        await loadLists(1);

        showToast({
            message: jsonData.message,
            type: "success",
            duration: 3000
        });
    }
}

async function handleConfirm() {
    const {currentAction, currentArticleId} = state;
    if (!currentAction || !currentArticleId) return;

    let url = '';
    let method = '';

    if (currentAction === 'delete') {
        url = `${API_URL}/${currentArticleId}`;
        method = 'DELETE';
    } else if (currentAction === 'publish') {
        url = `${API_URL}/publish/${currentArticleId}`;
        method = 'PUT';
    } else {
        return;
    }

    try {
        const response = await fetch(url, {
            method: method,
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        });

        const jsonData = await response.json();

        if (!response.ok) {
            showToast({
                message: jsonData.message,
                type: "danger",
                duration: 3000
            });
        } else {
            if (elements.confirmModalCloseBtn) elements.confirmModalCloseBtn.click();

            await loadLists(state.currentPage);

            showToast({
                message: jsonData.message,
                type: "success",
                duration: 3000
            });
        }
    } catch (error) {
        showToast({
            message: "Action failed",
            type: "danger",
            duration: 3000
        });
    }
}

document.addEventListener("DOMContentLoaded", async () => {
    cacheElements();

    const categoryModalEl  = document.getElementById(MODAL_FORM_ID);
    const confirmModalEl  = document.getElementById(MODAL_CONFIRM_ID);

    window.modalForm = new Modal(categoryModalEl, { backdrop: 'dynamic' });
    window.modalConfirm = new Modal(confirmModalEl, { backdrop: 'dynamic' });

    elements.searchInput.addEventListener("input", debounce(async (e) => handleSearch(e.target.value), 700));
    elements.categoryFilter.addEventListener("change", handleCategoryChange);
    elements.insertBtn.addEventListener("click", handleInsertClick);
    elements.articleForm.addEventListener("submit", handleFormSubmit);
    elements.confirmBtn.addEventListener("click", handleConfirm);
    elements.listContainer.addEventListener("click", handleListClick);

    await loadLists(state.currentPage);
});