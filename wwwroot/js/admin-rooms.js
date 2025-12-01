// Admin Rooms Management
document.addEventListener('DOMContentLoaded', function () {
    // Initialize
    initializeFilters();

    // Auto-hide alerts
    const alertBanners = document.querySelectorAll('.alert-banner');
    alertBanners.forEach(alert => {
        setTimeout(() => {
            alert.style.animation = 'slideUp 0.5s ease-out';
            setTimeout(() => alert.remove(), 500);
        }, 5000);
    });

    // Add confirmation animations
    addConfirmationAnimations();
});

// Filter Functions
function initializeFilters() {
    const statusFilter = document.getElementById('statusFilter');
    const typeFilter = document.getElementById('typeFilter');
    const searchInput = document.getElementById('searchInput');

    if (statusFilter) {
        statusFilter.addEventListener('change', applyFilters);
    }

    if (typeFilter) {
        typeFilter.addEventListener('change', applyFilters);
    }

    if (searchInput) {
        searchInput.addEventListener('input', debounce(applyFilters, 300));
    }
}

function applyFilters() {
    const statusFilter = document.getElementById('statusFilter').value.toLowerCase();
    const typeFilter = document.getElementById('typeFilter').value;
    const searchText = document.getElementById('searchInput').value.toLowerCase();

    const rows = document.querySelectorAll('.room-row');
    let visibleCount = 0;

    rows.forEach(row => {
        let showRow = true;

        // Status filter
        if (statusFilter && row.dataset.status !== statusFilter) {
            showRow = false;
        }

        // Type filter
        if (typeFilter && row.dataset.type !== typeFilter) {
            showRow = false;
        }

        // Search filter
        if (searchText) {
            const rowText = row.textContent.toLowerCase();
            if (!rowText.includes(searchText)) {
                showRow = false;
            }
        }

        if (showRow) {
            row.style.display = '';
            visibleCount++;
            row.style.animation = 'fadeIn 0.3s ease-out';
        } else {
            row.style.display = 'none';
        }
    });

    // Update no results message
    updateNoResultsMessage(visibleCount);

    // Show notification
    if (statusFilter || typeFilter || searchText) {
        showNotification(`Tìm thấy ${visibleCount} phòng`, 'info');
    }
}

function resetFilters() {
    document.getElementById('statusFilter').value = '';
    document.getElementById('typeFilter').value = '';
    document.getElementById('searchInput').value = '';

    const rows = document.querySelectorAll('.room-row');
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
    const tbody = document.querySelector('.rooms-table tbody');
    let noResultsMessage = document.querySelector('.no-results-message');

    if (visibleCount === 0 && document.querySelectorAll('.room-row').length > 0) {
        if (!noResultsMessage) {
            noResultsMessage = document.createElement('tr');
            noResultsMessage.className = 'no-results-message';
            noResultsMessage.innerHTML = `
                <td colspan="7" class="text-center">
                    <div class="no-data">
                        <i class="fas fa-search"></i>
                        <p>Không tìm thấy phòng phù hợp</p>
                    </div>
                </td>
            `;
            tbody.appendChild(noResultsMessage);
        }
    } else if (noResultsMessage) {
        noResultsMessage.remove();
    }
}

// Confirmation Animations
function addConfirmationAnimations() {
    // Delete confirmation with animation
    const deleteForms = document.querySelectorAll('form[action*="DeleteRoom"]');
    deleteForms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const confirmed = confirm('Bạn có chắc muốn xóa phòng này?');
            if (confirmed) {
                // Show loading notification
                showNotification('Đang xóa phòng...', 'info');

                // Add fade out animation to row
                const row = this.closest('tr');
                if (row) {
                    row.style.animation = 'fadeOut 0.5s ease-out';
                }
            } else {
                e.preventDefault();
            }
        });
    });

    // Toggle status with animation
    const toggleForms = document.querySelectorAll('form[action*="ToggleRoomStatus"]');
    toggleForms.forEach(form => {
        form.addEventListener('submit', function (e) {
            showNotification('Đang cập nhật trạng thái...', 'info');
        });
    });
}

// Stats Update
function updateStats() {
    const rows = document.querySelectorAll('.room-row');
    let availableCount = 0;
    let unavailableCount = 0;

    rows.forEach(row => {
        if (row.dataset.status === 'available') {
            availableCount++;
        } else {
            unavailableCount++;
        }
    });

    // Update stat displays
    const statValues = document.querySelectorAll('.stat-value');
    if (statValues.length >= 3) {
        statValues[0].textContent = availableCount;
        statValues[1].textContent = unavailableCount;
        statValues[2].textContent = rows.length;
    }
}

// Quick Stats View
function showQuickStats() {
    const rows = document.querySelectorAll('.room-row');
    const stats = {
        total: rows.length,
        available: 0,
        unavailable: 0,
        types: {}
    };

    rows.forEach(row => {
        if (row.dataset.status === 'available') {
            stats.available++;
        } else {
            stats.unavailable++;
        }

        const type = row.dataset.type;
        stats.types[type] = (stats.types[type] || 0) + 1;
    });

    let message = `
        <strong>Thống kê phòng:</strong><br>
        Tổng: ${stats.total} | 
        Có sẵn: ${stats.available} | 
        Không có sẵn: ${stats.unavailable}<br>
        <strong>Theo loại:</strong> ${Object.entries(stats.types).map(([type, count]) => `${type}: ${count}`).join(', ')}
    `;

    showNotification(message, 'info', 8000);
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

function showNotification(message, type = 'info', duration = 3000) {
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
        max-width: 400px;
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
    }, duration);
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
    
    @keyframes fadeOut {
        from {
            opacity: 1;
            transform: scale(1);
        }
        to {
            opacity: 0;
            transform: scale(0.9);
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

    // Ctrl/Cmd + N: Add new room
    if ((e.ctrlKey || e.metaKey) && e.key === 'n') {
        e.preventDefault();
        const addButton = document.querySelector('.btn-add');
        if (addButton) {
            window.location.href = addButton.href;
        }
    }

    // Ctrl/Cmd + I: Show stats
    if ((e.ctrlKey || e.metaKey) && e.key === 'i') {
        e.preventDefault();
        showQuickStats();
    }
});

// Print shortcuts info on load
console.log('%c🎯 Keyboard Shortcuts:', 'font-size: 16px; font-weight: bold; color: #3498db;');
console.log('Ctrl/Cmd + F: Focus search');
console.log('Ctrl/Cmd + R: Reset filters');
console.log('Ctrl/Cmd + N: Add new room');
console.log('Ctrl/Cmd + I: Show quick stats');

// Initialize on load
window.addEventListener('load', function () {
    updateStats();
});