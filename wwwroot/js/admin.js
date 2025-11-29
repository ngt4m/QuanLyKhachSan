// Admin Dashboard functionality
class AdminDashboard {
    constructor() {
        this.initCharts();
        this.initDataTables();
        this.initRealTimeUpdates();
    }

    initCharts() {
        // Revenue Chart
        const revenueCtx = document.getElementById('revenueChart');
        if (revenueCtx) {
            const revenueChart = new Chart(revenueCtx, {
                type: 'line',
                data: {
                    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                    datasets: [{
                        label: 'Revenue',
                        data: [12000, 19000, 15000, 25000, 22000, 30000],
                        borderColor: '#007bff',
                        backgroundColor: 'rgba(0, 123, 255, 0.1)',
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                        }
                    }
                }
            });
        }

        // Booking Status Chart
        const statusCtx = document.getElementById('statusChart');
        if (statusCtx) {
            const statusChart = new Chart(statusCtx, {
                type: 'doughnut',
                data: {
                    labels: ['Confirmed', 'Pending', 'Checked In', 'Checked Out', 'Cancelled'],
                    datasets: [{
                        data: [30, 10, 5, 40, 15],
                        backgroundColor: [
                            '#17a2b8',
                            '#ffc107',
                            '#28a745',
                            '#6c757d',
                            '#dc3545'
                        ]
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'bottom',
                        }
                    }
                }
            });
        }
    }

    initDataTables() {
        const tables = document.querySelectorAll('table[data-table="datatable"]');
        tables.forEach(table => {
            new simpleDatatables.DataTable(table, {
                searchable: true,
                fixedHeight: true,
                perPage: 10
            });
        });
    }

    initRealTimeUpdates() {
        // Simulate real-time updates
        setInterval(() => {
            this.updateStats();
        }, 30000);
    }

    async updateStats() {
        try {
            const response = await fetch('/admin/api/stats');
            const stats = await response.json();

            document.getElementById('totalBookings').textContent = stats.totalBookings;
            document.getElementById('totalUsers').textContent = stats.totalUsers;
            document.getElementById('totalRooms').textContent = stats.totalRooms;
            document.getElementById('totalRevenue').textContent = `$${stats.totalRevenue.toLocaleString()}`;
        } catch (error) {
            console.error('Error updating stats:', error);
        }
    }

    // Room management
    async updateRoomStatus(roomId, isAvailable) {
        try {
            const response = await fetch(`/admin/rooms/${roomId}/status`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ isAvailable })
            });

            if (response.ok) {
                showAlert('Room status updated successfully!', 'success');
            } else {
                throw new Error('Failed to update room status');
            }
        } catch (error) {
            showAlert('Error updating room status', 'danger');
            console.error('Error:', error);
        }
    }

    // Booking management
    async updateBookingStatus(bookingId, status) {
        try {
            const response = await fetch(`/admin/bookings/${bookingId}/status`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ status })
            });

            if (response.ok) {
                showAlert('Booking status updated successfully!', 'success');
                // Refresh the table or update the row
                location.reload();
            } else {
                throw new Error('Failed to update booking status');
            }
        } catch (error) {
            showAlert('Error updating booking status', 'danger');
            console.error('Error:', error);
        }
    }
}

// Initialize admin dashboard when ready
document.addEventListener('DOMContentLoaded', function () {
    if (document.querySelector('.admin-dashboard')) {
        new AdminDashboard();
    }
});