// Hospital Management System Client Scripts
$(document).ready(function () {
    // Sidebar toggle for mobile/responsive layout
    $('#sidebarToggle').on('click', function () {
        $('#appSidebar').toggleClass('show');
    });

    // Close sidebar when clicking outside on mobile
    $(document).on('click', function (e) {
        if ($(window).width() < 992) {
            if (!$(e.target).closest('#appSidebar, #sidebarToggle').length) {
                $('#appSidebar').removeClass('show');
            }
        }
    });

    // Avatar selector on Registration & Profile
    $('.avatar-option').on('click', function () {
        $('.avatar-option').removeClass('selected');
        $(this).addClass('selected');
        const selectedSrc = $(this).attr('data-avatar');
        $('#selectedAvatarInput').val(selectedSrc);
        $('#currentAvatarPreview').attr('src', selectedSrc);
    });
});

// Toastr Utility Functions
function notifySuccess(message, title) {
    toastr.success(message, title || 'Success');
}

function notifyError(message, title) {
    toastr.error(message, title || 'Error');
}

function notifyInfo(message, title) {
    toastr.info(message, title || 'Information');
}

function notifyWarning(message, title) {
    toastr.warning(message, title || 'Attention');
}

// AJAX Appointment Status Update
function updateAppointmentStatus(appointmentId, status) {
    if (!confirm('Are you sure you want to mark this appointment as ' + status + '?')) {
        return;
    }

    $.ajax({
        url: '/Appointments/UpdateStatus',
        type: 'POST',
        data: { id: appointmentId, status: status },
        success: function (response) {
            if (response.success) {
                notifySuccess(response.message);
                const badge = $('#status-badge-' + appointmentId);
                if (badge.length) {
                    badge.text(status);
                    badge.removeClass('bg-warning bg-success bg-danger bg-primary bg-secondary');
                    if (status === 'Confirmed') badge.addClass('bg-success');
                    else if (status === 'Cancelled') badge.addClass('bg-danger');
                    else if (status === 'Completed') badge.addClass('bg-primary');
                    else badge.addClass('bg-warning');
                }
            } else {
                notifyError(response.message);
            }
        },
        error: function () {
            notifyError('An error occurred while updating the appointment.');
        }
    });
}

// AJAX Pay Invoice
function payInvoice(invoiceId, totalAmount, paidSoFar) {
    const dueAmount = totalAmount - paidSoFar;
    const amountToPay = prompt('Enter payment amount to process (₹):', dueAmount.toFixed(2));
    
    if (amountToPay === null || amountToPay === '' || isNaN(amountToPay) || Number(amountToPay) <= 0) {
        return;
    }

    const method = prompt('Enter payment method (Cash, Card, UPI, Insurance):', 'Cash') || 'Cash';

    $.ajax({
        url: '/Billing/Pay',
        type: 'POST',
        data: {
            id: invoiceId,
            paidAmount: parseFloat(amountToPay),
            paymentMethod: method
        },
        success: function (response) {
            if (response.success) {
                notifySuccess(response.message);
                setTimeout(function () {
                    location.reload();
                }, 1000);
            } else {
                notifyError(response.message);
            }
        },
        error: function () {
            notifyError('Payment processing failed. Please check details.');
        }
    });
}
