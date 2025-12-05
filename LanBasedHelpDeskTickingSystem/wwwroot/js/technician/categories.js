'use strict'

const API_URL = "/api/technician/categories";
const MODAL_FORM_ID = "form-modal";
const MODAL_CONFIRM_ID = "confirm-modal";

const state = {
    limit: 10,
    currentPage: 1,
    search: "",
    currentArticleId: null,
    currentAction: null
};

const elements = {};

function cacheElements() {
    elements.listContainer = document.getElementById("listContainer");
    elements.totalRows = document.getElementById("total-rows");
    elements.pagination = document.getElementById("pagination");

    elements.searchInput = document.getElementById("search");
    elements.insertBtn = document.querySelector(".insert-btn");

    elements.articleForm = document.getElementById("form");
    elements.confirmBtn = document.querySelector(".confirm-btn");

    elements.formFields = elements.articleForm.elements;

    elements.formModalCloseBtn = document.querySelector(`#${MODAL_FORM_ID} button[data-modal-hide="${MODAL_FORM_ID}"]`);
    elements.confirmModalCloseBtn = document.querySelector(`#${MODAL_CONFIRM_ID} button[data-modal-hide="${MODAL_CONFIRM_ID}"]`);
}

function renderSkeleton() {
    elements.listContainer.innerHTML = `
        ${Array(5).fill().map(() => `
            <tr class="bg-white border-b border-gray-100">
                <td class="px-6 py-4">
                    <div class="h-4 bg-gray-300 rounded w-1/3 mb-2"></div>
                    <div class="h-3 bg-gray-200 rounded w-1/4"></div>
                </td>
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

// <td className="px-6 py-4">
//     <a href="#" data-id="${item.id}" data-type="edit" data-modal-toggle="${MODAL_FORM_ID}" className="edit-btn font-medium text-blue-600 hover:underline">Edit</a>
//     <a href="#" data-id="${item.id}" data-type="delete" data-modal-toggle="${MODAL_CONFIRM_ID}" className="font-medium text-blue-600 hover:underline">Delete</a>
// </td>

function createItemHtml(item)
{
    return `
            <tr class="bg-white border-b dark:bg-gray-800 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
                <th scope="row" class="px-6 py-4 font-medium text-gray-900 whitespace-nowrap dark:text-white">
                    ${item.name}
                    <p class="text-xs font-normal text-gray-500 dark:text-gray-400">${item.description}</p>
                </th>
                <td class="px-6 py-4">${new Date(item.createdAt).toLocaleDateString()}</td>
                
                <td class="px-6 py-4">
                <button type="button" data-id="${item.id}" data-type="edit" data-modal-toggle="${MODAL_FORM_ID}" class="edit-btn font-medium text-blue-600 hover:underline mr-2">Edit</button>
                        <button type="button" data-id="${item.id}" data-type="delete" data-modal-toggle="${MODAL_CONFIRM_ID}" class="font-medium text-red-600 hover:underline">Delete</button>

                </td>
            </tr>
            `;
}

function renderEmpty() {
    return `
        <tr>
            <td colspan="3">
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

async function fetchCategories(page) {
    state.currentPage = page;

    const params = new URLSearchParams({
        page: state.currentPage,
        limit: state.limit
    });
    if (state.search) params.append('search', state.search);

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

    const result = await fetchCategories(page);
    if (!result) return;

    if (result.data && result.data.length > 0) {
        elements.listContainer.innerHTML = result.data.map(createItemHtml).join('');
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

function handleInsertClick() {
    state.currentArticleId = null;
    state.currentAction = 'insert';
    elements.articleForm.reset();
}

async function handleListClick(e) {
    const button = e.target.closest('button[data-id]');
    if (!button) {
        console.log("error button;");
        return;
    }

    const id = button.dataset.id;
    const type = button.dataset.type;

    if (!id || !type) {
        console.log("Invalid action or article ID 123");
        return;
    }

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
                elements.formFields.Name.value = result.data.name;
                elements.formFields.Description.value = result.data.description;
            }
        } catch (error) {
            console.error('Failed to fetch article for editing:', error);
        }
    }
}

async function handleFormSubmit(e) {
    e.preventDefault();

    const data = {
        Name: elements.formFields.Name.value,
        Description: elements.formFields.Description.value
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
            const errorDiv = document.getElementById(`error-${String(field.attributes.name.value).toLowerCase()}`);

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
    if (!currentAction || !currentArticleId) {
        console.log("Invalid action or article ID");
        return;
    }

    let url = '';
    let method = '';

    if (currentAction === 'delete') {
        url = `${API_URL}/${currentArticleId}`;
        method = 'DELETE';
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
    elements.insertBtn.addEventListener("click", handleInsertClick);
    elements.articleForm.addEventListener("submit", handleFormSubmit);
    elements.confirmBtn.addEventListener("click", handleConfirm);
    elements.listContainer.addEventListener("click", handleListClick);

    await loadLists(state.currentPage);
});