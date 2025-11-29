class BookingManager {
    constructor() {
        this.selectedRoom = null;
        this.bookingData = {
            roomId: null,
            checkIn: null,
            checkOut: null,
            guests: 1
        };
        this.init();
    }

    init() {
        this.initializeDatePickers();
        this.setupEventListeners();
        this.loadRoomDetails();
    }

    initializeDatePickers() {
        const today = new Date().toISOString().split('T')[0];

        // Set minimum dates
        document.querySelectorAll('#CheckInDate').forEach(input => {
            input.min = today;
        });

        // Handle check-in date changes
        document.querySelectorAll('#CheckInDate').forEach(input => {
            input.addEventListener('change', (e) => {
                this.bookingData.checkIn = e.target.value;
                const checkOutInput = document.querySelector('#CheckOutDate');
                if (checkOutInput) {
                    checkOutInput.min = e.target.value;
                    if (checkOutInput.value && checkOutInput.value < e.target.value) {
                        checkOutInput.value = '';
                        this.bookingData.checkOut = null;
                    }
                }
                this.updatePriceCalculation();
                this.checkAvailability();
            });
        });

        // Handle check-out date changes
        document.querySelectorAll('#CheckOutDate').forEach(input => {
            input.addEventListener('change', (e) => {
                this.bookingData.checkOut = e.target.value;
                this.updatePriceCalculation();
                this.checkAvailability();
            });
        });

        // Handle guest number changes
        document.querySelectorAll('#NumberOfGuests').forEach(input => {
            input.addEventListener('change', (e) => {
                this.bookingData.guests = parseInt(e.target.value);
                this.checkCapacity();
            });
        });
    }

    setupEventListeners() {
        // Room selection
        document.querySelectorAll('.select-room-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const roomId = e.target.dataset.roomId;
                this.selectRoom(roomId);
            });
        });

        // Payment method selection
        document.querySelectorAll('input[name="paymentMethod"]').forEach(radio => {
            radio.addEventListener('change', (e) => {
                this.togglePaymentDetails(e.target.value);
            });
        });

        // Form submission
        document.querySelectorAll('.booking-form').forEach(form => {
            form.addEventListener('submit', (e) => {
                if (!this.validateBooking()) {
                    e.preventDefault();
                    this.showError('Please fill in all required fields correctly.');
                }
            });
        });
    }

    async loadRoomDetails() {
        const urlParams = new URLSearchParams(window.location.search);
        const roomId = urlParams.get('roomId');

        if (roomId) {
            await this.selectRoom(roomId);
        }
    }

    async selectRoom(roomId) {
        try {
            const response = await fetch(`/api/rooms/${roomId}`);
            const room = await response.json();

            this.selectedRoom = room;
            this.bookingData.roomId = roomId;

            this.updateRoomDetails(room);
            this.updatePriceCalculation();

            // Scroll to booking form
            document.getElementById('booking-form').scrollIntoView({
                behavior: 'smooth'
            });

        } catch (error) {
            console.error('Error loading room details:', error);
            this.showError('Error loading room details. Please try again.');
        }
    }

    updateRoomDetails(room) {
        document.querySelectorAll('.room-name').forEach(el => {
            el.textContent = room.name;
        });

        document.querySelectorAll('.room-price').forEach(el => {
            el.textContent = `$${room.price}/night`;
        });

        document.querySelectorAll('.room-capacity').forEach(el => {
            el.textContent = room.capacity;
        });

        // Update hidden field
        document.querySelectorAll('#RoomId').forEach(input => {
            input.value = room.id;
        });
    }

    updatePriceCalculation() {
        if (!this.bookingData.checkIn || !this.bookingData.checkOut || !this.selectedRoom) {
            return;
        }

        const checkIn = new Date(this.bookingData.checkIn);
        const checkOut = new Date(this.bookingData.checkOut);
        const nights = Math.ceil((checkOut - checkIn) / (1000 * 60 * 60 * 24));

        if (nights <= 0) {
            return;
        }

        const basePrice = this.selectedRoom.price * nights;
        const tax = basePrice * 0.1; // 10% tax
        const total = basePrice + tax;

        // Update display
        document.querySelectorAll('.nights-count').forEach(el => {
            el.textContent = nights;
        });

        document.querySelectorAll('.base-price').forEach(el => {
            el.textContent = `$${basePrice.toFixed(2)}`;
        });

        document.querySelectorAll('.tax-amount').forEach(el => {
            el.textContent = `$${tax.toFixed(2)}`;
        });

        document.querySelectorAll('.total-price').forEach(el => {
            el.textContent = `$${total.toFixed(2)}`;
        });

        // Update hidden total field if exists
        document.querySelectorAll('#TotalPrice').forEach(input => {
            input.value = total.toFixed(2);
        });
    }

    async checkAvailability() {
        if (!this.bookingData.roomId || !this.bookingData.checkIn || !this.bookingData.checkOut) {
            return;
        }

        try {
            const response = await fetch(
                `/api/rooms/${this.bookingData.roomId}/availability?checkIn=${this.bookingData.checkIn}&checkOut=${this.bookingData.checkOut}`
            );
            const data = await response.json();

            const availabilityElement = document.getElementById('room-availability');
            if (availabilityElement) {
                if (data.available) {
                    availabilityElement.innerHTML = '<span class="text-success"><i class="fas fa-check"></i> Room is available</span>';
                    availabilityElement.classList.remove('text-danger');
                    availabilityElement.classList.add('text-success');
                } else {
                    availabilityElement.innerHTML = '<span class="text-danger"><i class="fas fa-times"></i> Room is not available for selected dates</span>';
                    availabilityElement.classList.remove('text-success');
                    availabilityElement.classList.add('text-danger');
                }
            }
        } catch (error) {
            console.error('Error checking availability:', error);
        }
    }

    checkCapacity() {
        if (!this.selectedRoom) return;

        const capacityElement = document.getElementById('capacity-check');
        if (capacityElement && this.bookingData.guests > this.selectedRoom.capacity) {
            capacityElement.innerHTML = `<span class="text-warning"><i class="fas fa-exclamation-triangle"></i> Maximum capacity is ${this.selectedRoom.capacity} guests</span>`;
        } else if (capacityElement) {
            capacityElement.innerHTML = '<span class="text-success"><i class="fas fa-check"></i> Within room capacity</span>';
        }
    }

    togglePaymentDetails(paymentMethod) {
        // Hide all payment detail forms
        document.querySelectorAll('.payment-details').forEach(form => {
            form.style.display = 'none';
        });

        // Show relevant payment form
        const detailsForm = document.getElementById(`${paymentMethod.toLowerCase().replace(' ', '-')}-details`);
        if (detailsForm) {
            detailsForm.style.display = 'block';
        }
    }

    validateBooking() {
        if (!this.bookingData.roomId) {
            this.showError('Please select a room.');
            return false;
        }

        if (!this.bookingData.checkIn || !this.bookingData.checkOut) {
            this.showError('Please select check-in and check-out dates.');
            return false;
        }

        const checkIn = new Date(this.bookingData.checkIn);
        const checkOut = new Date(this.bookingData.checkOut);
        if (checkOut <= checkIn) {
            this.showError('Check-out date must be after check-in date.');
            return false;
        }

        if (this.bookingData.guests < 1) {
            this.showError('Number of guests must be at least 1.');
            return false;
        }

        return true;
    }

    showError(message) {
        // Remove existing error messages
        document.querySelectorAll('.booking-error').forEach(el => el.remove());

        // Create new error message
        const errorDiv = document.createElement('div');
        errorDiv.className = 'alert alert-danger booking-error';
        errorDiv.innerHTML