function resetSearch() {
    window.location.href = '/Admin/Products';
}

function exportToExcel() {
    let table = document.getElementById('productsTable');
    if (!table) return;

    let html = table.outerHTML;

    let blob = new Blob([html], { type: 'application/vnd.ms-excel' });
    let url = URL.createObjectURL(blob);
    let a = document.createElement('a');
    a.href = url;
    a.download = 'danh-sach-san-pham-' + new Date().toISOString().slice(0, 10) + '.xls';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

function printTable() {
    let printWindow = window.open('', '', 'height=600,width=800');
    printWindow.document.write('<html><head><title>Danh sách sản phẩm</title>');
    printWindow.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css" rel="stylesheet">');
    printWindow.document.write('</head><body>');
    printWindow.document.write('<h3 class="text-center mb-4">Danh sách sản phẩm</h3>');
    printWindow.document.write(document.getElementById('productsTable').outerHTML);
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
    printWindow.close();
}

function setupAutoSubmit() {
    document.querySelectorAll('select[name="sortBy"], select[name="sortOrder"]').forEach(select => {
        select.addEventListener('change', function () {
            document.getElementById('searchForm').submit();
        });
    });
}

function setupTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[title]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}



document.addEventListener('DOMContentLoaded', function () {
    setupAutoSubmit();
    setupTooltips();
    setupDeleteConfirmations();
    setupTogglePublishConfirmations();
});