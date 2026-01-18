
(function () {
    'use strict';

    const AdminCommon = {
        swalConfig: {
            delete: {
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc3545',
                cancelButtonColor: '#6c757d',
                cancelButtonText: 'Hủy bỏ',
                reverseButtons: true
            },
            toggleStatus: {
                icon: 'question',
                showCancelButton: true,
                cancelButtonColor: '#6c757d',
                cancelButtonText: 'Hủy bỏ',
                reverseButtons: true
            }
        },

        /**
         */
        initDeleteConfirmation: function () {
            document.querySelectorAll('.js-delete-form').forEach(form => {
                form.addEventListener('submit', (e) => {
                    e.preventDefault();
                    this.confirmDelete(form);
                });
            });
        },

        /**
         * @param {HTMLFormElement} form - Form cần xác nhận
         */
        confirmDelete: function (form) {
            const itemName = form.getAttribute('data-item-name') || 'mục này';
            const itemType = form.getAttribute('data-item-type') || 'item';
            const customMessage = form.getAttribute('data-custom-message');

            const config = {
                ...this.swalConfig.delete,
                title: `Xóa ${itemType}?`,
                text: customMessage || `Bạn có chắc chắn muốn xóa "${itemName}"? Thao tác này không thể hoàn tác!`,
                confirmButtonText: form.getAttribute('data-confirm-text') || 'Xóa ngay!'
            };

            Swal.fire(config).then((result) => {
                if (result.isConfirmed) {
                    const submitBtn = form.querySelector('button[type="submit"]');
                    if (submitBtn) {
                        const originalText = submitBtn.innerHTML;
                        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang xóa...';
                        submitBtn.disabled = true;

                        setTimeout(() => form.submit(), 500);
                    } else {
                        form.submit();
                    }
                }
            });
        },

        /**
         */
        initToggleStatusConfirmation: function () {
            document.querySelectorAll('.js-toggle-status-form').forEach(form => {
                form.addEventListener('submit', (e) => {
                    e.preventDefault();
                    this.confirmToggleStatus(form);
                });
            });
        },

        /**
         */
        confirmToggleStatus: function (form) {
            const actionName = form.getAttribute('data-action-name') || 'thực hiện';
            const userName = form.getAttribute('data-user-name') || 'người dùng này';

            const config = {
                ...this.swalConfig.toggleStatus,
                title: `${actionName} tài khoản?`,
                text: `Bạn có chắc chắn muốn ${actionName.toLowerCase()} tài khoản của "${userName}"?`,
                confirmButtonColor: actionName === 'Khóa' ? '#dc3545' : '#198754',
                confirmButtonText: `${actionName} ngay!`
            };

            Swal.fire(config).then((result) => {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
        },

        /**
         */
        initActionForms: function () {
            this.initDeleteConfirmation();
            this.initToggleStatusConfirmation();
            this.initCustomActions();
        },

        /**
         */
        initCustomActions: function () {
            document.querySelectorAll('.js-toggle-publish-form').forEach(form => {
                form.addEventListener('submit', (e) => {
                    e.preventDefault();
                    this.confirmTogglePublish(form);
                });
            });
        },

        /**
         */
        confirmTogglePublish: function (form) {
            const action = form.getAttribute('data-action');
            const itemName = form.getAttribute('data-item-name') || 'sản phẩm';
            const statusText = action === 'publish' ? 'công khai' : 'ẩn';

            Swal.fire({
                title: `${action === 'publish' ? 'Công khai' : 'Ẩn'} ${itemName}?`,
                text: `Bạn có chắc muốn ${statusText} ${itemName} này?`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: action === 'publish' ? '#198754' : '#2c3e50',
                cancelButtonColor: '#6c757d',
                cancelButtonText: 'Hủy bỏ',
                confirmButtonText: action === 'publish' ? 'Công khai' : 'Ẩn đi',
                reverseButtons: true
             
            }).then((result) => {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
        },

        /**
         */
      

        /**
         */
        init: function () {
            if (typeof Swal === 'undefined') {
                console.error('SweetAlert2 is not loaded');
                return;
            }

            this.initActionForms();

            this.initAutoDismissAlerts();
        },

        /**
         */
        initAutoDismissAlerts: function () {
            const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
            alerts.forEach(alert => {
                setTimeout(() => {
                    const bsAlert = new bootstrap.Alert(alert);
                    bsAlert.close();
                }, 5000);
            });
        },

        /**
         */
        showError: function (message) {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: message,
                confirmButtonColor: '#dc3545'
            });
        },

        /**
         */
        showSuccess: function (message) {
            Swal.fire({
                icon: 'success',
                title: 'Thành công',
                text: message,
                timer: 2000,
                showConfirmButton: false
            });
        }
    };

    document.addEventListener('DOMContentLoaded', function () {
        AdminCommon.init();
    });

    window.AdminCommon = AdminCommon;


})();