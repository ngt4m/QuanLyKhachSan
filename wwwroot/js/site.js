// Global functions
function showAlert(message, type = 'success') {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    document.querySelector('main').insertBefore(alertDiv, document.querySelector('main').firstChild);

    setTimeout(() => {
        alertDiv.remove();
    }, 5000);
}

// Date validation
function validateDates(checkInId, checkOutId) {
    const checkIn = document.getElementById(checkInId);
    const checkOut = document.getElementById(checkOutId);

    if (checkIn && checkOut) {
        const checkInDate = new Date(checkIn.value);
        const checkOutDate = new Date(checkOut.value);
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        if (checkInDate < today) {
            showAlert('Check-in date cannot be in the past', 'danger');
            checkIn.value = '';
            return false;
        }

        if (checkOutDate <= checkInDate) {
            showAlert('Check-out date must be after check-in date', 'danger');
            checkOut.value = '';
            return false;
        }

        return true;
    }
}

// Image preview
function previewImage(input, previewId) {
    const preview = document.getElementById(previewId);
    const file = input.files[0];
    const reader = new FileReader();

    reader.onloadend = function () {
        preview.src = reader.result;
        preview.style.display = 'block';
    }

    if (file) {
        reader.readAsDataURL(file);
    } else {
        preview.src = '';
        preview.style.display = 'none';
    }
}

// Price calculation
function calculateRoomPrice(pricePerNight, checkIn, checkOut) {
    const start = new Date(checkIn);
    const end = new Date(checkOut);
    const nights = Math.ceil((end - start) / (1000 * 60 * 60 * 24));
    return nights * pricePerNight;
}

// Form validation
// Form validation (continued)
function validateForm(formId) {
    const form = document.getElementById(formId);
    const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
    let isValid = true;

    inputs.forEach(input => {
        if (!input.value.trim()) {
            input.classList.add('is-invalid');
            isValid = false;
        } else {
            input.classList.remove('is-invalid');
        }
    });

    return isValid;
}

// Search functionality
function searchRooms() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();
    const roomCards = document.querySelectorAll('.room-card');

    roomCards.forEach(card => {
        const roomName = card.querySelector('.card-title').textContent.toLowerCase();
        const roomDescription = card.querySelector('.card-text').textContent.toLowerCase();

        if (roomName.includes(searchTerm) || roomDescription.includes(searchTerm)) {
            card.style.display = 'block';
        } else {
            card.style.display = 'none';
        }
    });
}

// Filter rooms by price
function filterByPrice() {
    const minPrice = parseFloat(document.getElementById('minPrice').value) || 0;
    const maxPrice = parseFloat(document.getElementById('maxPrice').value) || Infinity;
    const roomCards = document.querySelectorAll('.room-card');

    roomCards.forEach(card => {
        const priceText = card.querySelector('.price').textContent;
        const price = parseFloat(priceText.replace(/[^0-9.]/g, ''));

        if (price >= minPrice && price <= maxPrice) {
            card.style.display = 'block';
        } else {
            card.style.display = 'none';
        }
    });
}

// Initialize datepickers
function initializeDatepickers() {
    const today = new Date().toISOString().split('T')[0];
    const checkInInputs = document.querySelectorAll('input[type="date"][id*="CheckIn"]');
    const checkOutInputs = document.querySelectorAll('input[type="date"][id*="CheckOut"]');

    checkInInputs.forEach(input => {
        input.min = today;
        input.addEventListener('change', function () {
            const checkOutInput = document.getElementById(this.id.replace('CheckIn', 'CheckOut'));
            if (checkOutInput) {
                checkOutInput.min = this.value;
                if (checkOutInput.value && checkOutInput.value < this.value) {
                    checkOutInput.value = '';
                }
            }
        });
    });
}

// Room availability check
async function checkRoomAvailability(roomId, checkIn, checkOut) {
    try {
        const response = await fetch(`/api/rooms/${roomId}/availability?checkIn=${checkIn}&checkOut=${checkOut}`);
        const data = await response.json();
        return data.available;
    } catch (error) {
        console.error('Error checking room availability:', error);
        return false;
    }
}

// Initialize when document is ready
document.addEventListener('DOMContentLoaded', function () {
    initializeDatepickers();

    // Auto-dismiss alerts
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.remove();
        }, 5000);
    });

    // Add loading states to buttons
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function () {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Loading...';
            }
        });
    });
});