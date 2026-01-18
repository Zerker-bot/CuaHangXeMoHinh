(function () {
    'use strict';

    const AdminReview = {

        /**
         */
        initReviewModals: function () {
            document.querySelectorAll('.review-modal form').forEach(form => {
                form.addEventListener('submit', function (e) {

                    const modal = this.closest('.modal');
                    const submitBtn = this.querySelector('button[type="submit"]');

                    if (submitBtn) {
                        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang xử lý...';
                        submitBtn.disabled = true;
                    }
                });
            });
        },

        /**
         */
        initReviewDetailActions: function () {
            const approveForm = document.getElementById('approveForm');
            const rejectForm = document.getElementById('rejectForm');

            if (approveForm) {
                const approveBtn = approveForm.querySelector('button[type="button"]');
                if (approveBtn) {
                    approveBtn.addEventListener('click', () => {
                        this.confirmReviewAction('approveForm', 'duyệt');
                    });
                }
            }

            if (rejectForm) {
                const rejectBtn = rejectForm.querySelector('button[type="button"]');
                if (rejectBtn) {
                    rejectBtn.addEventListener('click', () => {
                        this.confirmReviewAction('rejectForm', 'từ chối');
                    });
                }
            }
        },

        /**
         */
        confirmReviewAction: function (formId, actionName) {
            const form = document.getElementById(formId);
            if (!form) return;

            Swal.fire({
                title: `Xác nhận ${actionName}?`,
                text: `Bạn có chắc chắn muốn ${actionName} đánh giá này?`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: actionName === 'duyệt' ? '#198754' : '#dc3545', 
                cancelButtonColor: '#6c757d',
                cancelButtonText: 'Hủy bỏ',
                confirmButtonText: `${actionName.charAt(0).toUpperCase() + actionName.slice(1)} ngay!`,
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    const submitBtn = form.querySelector('button[type="button"]');
                    if (submitBtn) {
                        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang xử lý...';
                        submitBtn.disabled = true;
                    }
                    // Submit form 
                    form.submit();
                }
            });
        },

        /**
         */
        initTooltips: function () {
            const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        },

        /**
         */
        init: function () {
            if (typeof Swal === 'undefined') {
                console.warn('SweetAlert2 chưa được load, tính năng Review có thể không chạy.');
                return;
            }

            this.initReviewModals();
            this.initReviewDetailActions();
            this.initTooltips();
        }
    };

    document.addEventListener('DOMContentLoaded', function () {
        AdminReview.init();
    });

    window.AdminReview = AdminReview;

})();