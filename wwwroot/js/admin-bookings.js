// Admin Bookings Management
document.addEventListener('DOMContentLoaded', function () {
    // Initialize
    initializeFilters();
    initializeDropdowns();

    // Auto-hide success message
    const alertBanner = document.querySelector('.alert-banner');
    if (alertBanner) {
        setTimeout(() => {
            alertBanner.style.animation = 'slideUp 0.5s ease-out';
            setTimeout(() => alertBanner.remove(), 500);
        }, 5000);
    }
});

// Filter Functions
function initializeFilters() {
    const statusFilter = document.getElementById('statusFilter');
    const searchInput = document.getElementById('searchInput');
    const dateFilter = document.getElementById('dateFilter');

    if (statusFilter) {
        statusFilter.addEventListener('change', applyFilters);
    }

    if (searchInput) {
        searchInput.addEventListener('input', debounce(applyFilters, 300));
    }

    if (dateFilter) {
        dateFilter.addEventListener('change', applyFilters);
    }
}

function applyFilters() {
    const statusFilter = document.getElementById('statusFilter').value.toLowerCase();
    const searchText = document.getElementById('searchInput').value.toLowerCase();
    const dateFilter = document.getElementById('dateFilter').value;

    const rows = document.querySelectorAll('.booking-row');
    let visibleCount = 0;

    rows.forEach(row => {
        let showRow = true;

        // Status filter
        if (statusFilter && row.dataset.status !== statusFilter) {
            showRow = false;
        }

        // Search filter
        if (searchText) {
            const rowText = row.textContent.toLowerCase();
            if (!rowText.includes(searchText)) {
                showRow = false;
            }
        }

        // Date filter
        if (dateFilter) {
            const checkInCell = row.cells[3]; // Check-in column
            if (checkInCell) {
                const checkInDate = checkInCell.textContent.trim();
                const filterDate = formatDateForComparison(dateFilter);
                const rowDate = formatDateForComparison(checkInDate);

                if (rowDate !== filterDate) {
                    showRow = false;
                }
            }
        }

        if (showRow) {
            row.style.display = '';
            visibleCount++;
            // Add fade in animation
            row.style.animation = 'fadeIn 0.3s ease-out';
        } else {
            row.style.display = 'none';
        }
    });

    // Show/hide no results message
    updateNoResultsMessage(visibleCount);
}

function resetFilters() {
    document.getElementById('statusFilter').value = '';
    document.getElementById('searchInput').value = '';
    document.getElementById('dateFilter').value = '';

    const rows = document.querySelectorAll('.booking-row');
    rows.forEach(row => {
        row.style.display = '';
        row.style.animation = 'fadeIn 0.3s ease-out';
    });

    const noResultsMessage = document.querySelector('.no-results-message');
    if (noResultsMessage) {
        noResultsMessage.remove();
    }

    showNotification('Đã đặt lại bộ lọc', 'info');
}

function updateNoResultsMessage(visibleCount) {
    const tbody = document.querySelector('.bookings-table tbody');
    let noResultsMessage = document.querySelector('.no-results-message');

    if (visibleCount === 0 && document.querySelectorAll('.booking-row').length > 0) {
        if (!noResultsMessage) {
            noResultsMessage = document.createElement('tr');
            noResultsMessage.className = 'no-results-message';
            noResultsMessage.innerHTML = `
                <td colspan="9" class="text-center">
                    <div class="no-data">
                        <i class="fas fa-search"></i>
                        <p>Không tìm thấy kết quả phù hợp</p>
                    </div>
                </td>
            `;
            tbody.appendChild(noResultsMessage);
        }
    } else if (noResultsMessage) {
        noResultsMessage.remove();
    }
}

// Export to Excel
function exportToExcel() {
    showNotification('Đang chuẩn bị xuất file Excel...', 'info');

    const table = document.getElementById('bookingsTable');
    const rows = [];

    // Get headers
    const headers = [];
    table.querySelectorAll('thead th').forEach((th, index) => {
        if (index < 8) { // Exclude the Actions column
            headers.push(th.textContent.trim());
        }
    });
    rows.push(headers);

    // Get visible rows data
    table.querySelectorAll('tbody tr.booking-row').forEach(row => {
        if (row.style.display !== 'none') {
            const rowData = [];
            for (let i = 0; i < 8; i++) { // Exclude the Actions column
                const cell = row.cells[i];
                if (cell) {
                    rowData.push(cell.textContent.trim());
                }
            }
            rows.push(rowData);
        }
    });

    // Create CSV content
    let csvContent = 'data:text/csv;charset=utf-8,\uFEFF';
    rows.forEach(row => {
        csvContent += row.join(',') + '\r\n';
    });

    // Create download link
    const encodedUri = encodeURI(csvContent);
    const link = document.createElement('a');
    link.setAttribute('href', encodedUri);
    link.setAttribute('download', `bookings_${new Date().toISOString().split('T')[0]}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    showNotification('Đã xuất file Excel thành công!', 'success');
}

// Dropdown Handlers
function initializeDropdowns() {
    // Add confirmation for status updates
    const dropdownForms = document.querySelectorAll('.dropdown-form');
    dropdownForms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const statusInput = this.querySelector('input[name="status"]');
            const status = statusInput ? statusInput.value : '';

            if (status !== 'Cancelled') {
                // Show loading notification
                showNotification('Đang cập nhật trạng thái...', 'info');
            }
        });
    });
}

// Utility Functions
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

function formatDateForComparison(dateString) {
    // Convert date to DD/MM/YYYY format for comparison
    if (!dateString) return '';

    // If it's already in DD/MM/YYYY format
    if (dateString.includes('/')) {
        return dateString;
    }

    // If it's in YYYY-MM-DD format (from date input)
    const parts = dateString.split('-');
    if (parts.length === 3) {
        return `${parts[2]}/${parts[1]}/${parts[0]}`;
    }

    return dateString;
}

function showNotification(message, type = 'info') {
    // Remove existing notifications
    const existingNotifications = document.querySelectorAll('.notification-toast');
    existingNotifications.forEach(n => n.remove());

    // Create notification
    const notification = document.createElement('div');
    notification.className = 'notification-toast';

    const colors = {
        info: '#3498db',
        success: '#27ae60',
        warning: '#f39c12',
        error: '#e74c3c'
    };

    const icons = {
        info: 'fa-info-circle',
        success: 'fa-check-circle',
        warning: 'fa-exclamation-triangle',
        error: 'fa-times-circle'
    };

    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${colors[type]};
        color: white;
        padding: 15px 25px;
        border-radius: 12px;
        box-shadow: 0 8px 25px rgba(0, 0, 0, 0.2);
        z-index: 9999;
        display: flex;
        align-items: center;
        gap: 12px;
        animation: slideInRight 0.3s ease-out;
        max-width: 350px;
    `;

    notification.innerHTML = `
        <i class="fas ${icons[type]}" style="font-size: 1.3rem;"></i>
        <span>${message}</span>
    `;

    document.body.appendChild(notification);

    // Auto remove
    setTimeout(() => {
        notification.style.animation = 'slideOutRight 0.3s ease-out';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// Add animations to stylesheet
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeIn {
        from {
            opacity: 0;
            transform: translateY(10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
    
    @keyframes slideUp {
        from {
            transform: translateY(0);
            opacity: 1;
        }
        to {
            transform: translateY(-20px);
            opacity: 0;
        }
    }
    
    @keyframes slideInRight {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

// Keyboard shortcuts
document.addEventListener('keydown', function (e) {
    // Ctrl/Cmd + F: Focus search
    if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
        e.preventDefault();
        document.getElementById('searchInput').focus();
    }

    // Ctrl/Cmd + R: Reset filters
    if ((e.ctrlKey || e.metaKey) && e.key === 'r') {
        e.preventDefault();
        resetFilters();
    }

    // Ctrl/Cmd + E: Export to Excel
    if ((e.ctrlKey || e.metaKey) && e.key === 'e') {
        e.preventDefault();
        exportToExcel();
    }
});

// Print shortcuts info on load
console.log('%c🎯 Keyboard Shortcuts:', 'font-size: 16px; font-weight: bold; color: #3498db;');
console.log('Ctrl/Cmd + F: Focus search');
console.log('Ctrl/Cmd + R: Reset filters');
console.log('Ctrl/Cmd + E: Export to Excel');