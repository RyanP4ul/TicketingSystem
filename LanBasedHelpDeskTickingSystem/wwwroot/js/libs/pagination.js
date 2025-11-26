function buildPagination(current, total) {
    let html = "";

    function pageLink(page, label = page, disable = false) {
        return `
            <li>
                 <a href="#" ${disable ? 'aria-disabled="true"' : `data-page="${page}"`} class="flex items-center justify-center px-3 h-8 ms-0 ${page === current ? 'text-blue-600 border border-gray-300 bg-blue-50 hover:bg-blue-100 hover:text-blue-700 dark:border-gray-700 dark:bg-gray-700 dark:text-white' : 'leading-tight text-gray-500 bg-white border border-gray-300 rounded-e-lg hover:bg-gray-100 hover:text-gray-700 dark:bg-gray-800 dark:border-gray-700 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white'}">${label}</a>
           </li>
        `;
    }

    html += pageLink(current - 1, "Previous", current < 2);

    if (total <= 5) {
        for (let i = 1; i <= total; i++) html += pageLink(i);
    } else {
        if (current > 3) {
            html += pageLink(1);
            if (current > 4) html += `<li class="px-2 py-1">...</li>`;
        }

        const start = Math.max(1, current - 1);
        const end = Math.min(total, current + 1);
        for (let i = start; i <= end; i++) html += pageLink(i);

        if (current < total - 2) {
            if (current < total - 3) html += `<li class="px-2 py-1">...</li>`;
            html += pageLink(total);
        }
    }

    html += pageLink(current + 1, "Next", current === total);

    document.getElementById("pagination").innerHTML = html;

    document.querySelectorAll("#pagination a").forEach(a => {
        a.addEventListener("click", function(e) {
            e.preventDefault();
            const page = parseInt(this.dataset.page);
            if (!isNaN(page)) loadLists(page);
        });
    });
}

window.buildPagination = buildPagination;