// Site JavaScript

// Toggle sidebar on mobile
function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    sidebar.classList.toggle('show');
}

// Close sidebar when clicking outside on mobile
document.addEventListener('click', function(event) {
    const sidebar = document.getElementById('sidebar');
    const toggleBtn = document.querySelector('.btn-toggle-sidebar');
    
    if (window.innerWidth <= 992 && 
        !sidebar.contains(event.target) && 
        !toggleBtn.contains(event.target) && 
        sidebar.classList.contains('show')) {
        sidebar.classList.remove('show');
    }
});

// Auto-hide alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function() {
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function(alert) {
        setTimeout(function() {
            const closeBtn = alert.querySelector('.btn-close');
            if (closeBtn) {
                closeBtn.click();
            }
        }, 5000);
    });
});

// Confirm before logout
function confirmLogout(event) {
    event.preventDefault();
    if (confirm('Are you sure you want to logout?')) {
        event.target.submit();
    }
}

// Add loading state to forms
document.querySelectorAll('form').forEach(function(form) {
    form.addEventListener('submit', function() {
        const submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing...';
        }
    });
});

// Character counter for textareas
document.querySelectorAll('textarea[maxlength]').forEach(function(textarea) {
    textarea.addEventListener('input', function() {
        const counterId = this.id + 'Count';
        let counter = document.getElementById(counterId);
        if (!counter) {
            counter = document.createElement('small');
            counter.className = 'text-muted';
            counter.id = counterId;
            this.parentNode.appendChild(counter);
        }
        counter.textContent = this.value.length + '/' + this.maxlength;
    });
});
