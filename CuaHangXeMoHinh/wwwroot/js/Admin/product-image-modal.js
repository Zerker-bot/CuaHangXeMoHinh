(function () {
    'use strict';

    let modalInitialized = false;
    let currentProductId = 0;

    function initProductImageModal() {
        if (modalInitialized) return;

        const modalElement = document.getElementById('productImageModal');
        if (!modalElement) return;

        modalElement.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            currentProductId = button.getAttribute('data-product-id');
            document.getElementById('productId').value = currentProductId;

            resetForm();
        });

        const imageFileInput = document.getElementById('imageFile');
        const imagePreview = document.getElementById('imagePreview');
        const previewImage = document.getElementById('previewImage');

        if (imageFileInput) {
            imageFileInput.addEventListener('change', function () {
                if (this.files && this.files[0]) {
                    const reader = new FileReader();

                    reader.onload = function (e) {
                        previewImage.src = e.target.result;
                        imagePreview.style.display = 'block';
                    }

                    reader.readAsDataURL(this.files[0]);
                } else {
                    imagePreview.style.display = 'none';
                }
            });
        }

        const form = document.getElementById('productImageForm');
        if (form) {
            form.addEventListener('submit', async function (e) {
                e.preventDefault();
                console.log('FormData contents:');

                const formData = new FormData(this);
                const saveBtn = document.getElementById('saveImageBtn');
                const originalBtnText = saveBtn.innerHTML;

                const imageFile = document.getElementById('imageFile').files[0];
                const imageUrl = document.getElementById('imageUrl').value;

                if (!imageFile && !imageUrl) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Thiếu thông tin',
                        text: 'Vui lòng chọn file ảnh hoặc nhập URL',
                        confirmButtonColor: '#3085d6'
                    });
                    return;
                }

                saveBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang tải lên...';
                saveBtn.disabled = true;

                try {
                    const response = await fetch('/api/ProductImages/upload', {
                        method: 'POST',
                        body: formData
                    });

                    const result = await response.json();

                    if (result.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Thành công!',
                            text: result.message,
                            timer: 1500,
                            showConfirmButton: false
                        });

                        resetForm();

                        if (currentProductId) {
                            loadProductImages(currentProductId);
                        }
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Lỗi',
                            text: result.message,
                            confirmButtonColor: '#d33'
                        });
                    }
                } catch (error) {
                    console.error('Error:', error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi kết nối',
                        text: 'Không thể kết nối đến server',
                        confirmButtonColor: '#d33'
                    });
                } finally {
                    saveBtn.innerHTML = originalBtnText;
                    saveBtn.disabled = false;
                }
            });
        

        const uploadArea = document.querySelector('.image-upload-area');
        if (uploadArea) {
            ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
                uploadArea.addEventListener(eventName, preventDefaults, false);
            });

            function preventDefaults(e) {
                e.preventDefault();
                e.stopPropagation();
            }

            ['dragenter', 'dragover'].forEach(eventName => {
                uploadArea.addEventListener(eventName, highlight, false);
            });

            ['dragleave', 'drop'].forEach(eventName => {
                uploadArea.addEventListener(eventName, unhighlight, false);
            });

            function highlight() {
                uploadArea.classList.add('highlight');
            }

            function unhighlight() {
                uploadArea.classList.remove('highlight');
            }

            uploadArea.addEventListener('drop', handleDrop, false);

            function handleDrop(e) {
                const dt = e.dataTransfer;
                const files = dt.files;
                document.getElementById('imageFile').files = files;

                const event = new Event('change');
                document.getElementById('imageFile').dispatchEvent(event);
            }
        }

        modalInitialized = true;
    }

    function resetForm() {
        const form = document.getElementById('productImageForm');
        if (form) {
            document.getElementById('imageFile').value = '';

            document.getElementById('imageUrl').value = '';

            document.getElementById('imagePreview').style.display = 'none';
            document.getElementById('previewImage').src = '#';

            const errorElements = form.querySelectorAll('.text-danger');
            errorElements.forEach(el => el.style.display = 'none');
        }
    }

    async function loadProductImages(productId) {
        try {
            if (!productId) {
                productId = document.getElementById('Id')?.value;
            }

            if (!productId) {
                console.error('Không tìm thấy ID sản phẩm');
                return;
            }

            const timestamp = new Date().getTime();
            const response = await fetch(`/Admin/Products/GetProductImages/${productId}?_=${timestamp}`);

            if (response.ok) {
                const html = await response.text();
                const container = document.getElementById('productImagesContainer');
                if (container) {
                    container.innerHTML = html;
                    bindImageActions();
                }
            } else {
                console.error('Lỗi khi tải ảnh:', response.statusText);
            }
        } catch (error) {
            console.error('Lỗi:', error);
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        initProductImageModal();

        const productIdInput = document.getElementById('Id');
        if (productIdInput) {
            window.productImageModal.setCurrentProductId(productIdInput.value);
        }

        bindImageActions();
    });
    }

    function bindImageActions() {
        document.querySelectorAll('.btn-delete-image').forEach(btn => {
            btn.addEventListener('click', function () {
                const imageId = this.getAttribute('data-image-id');
                const imageUrl = this.getAttribute('data-image-url');

                Swal.fire({
                    title: 'Xóa ảnh này?',
                    text: "Bạn có chắc chắn muốn xóa ảnh này?",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#3085d6',
                    confirmButtonText: 'Xóa',
                    cancelButtonText: 'Hủy'
                }).then(async (result) => {
                    if (result.isConfirmed) {
                        try {
                            const response = await fetch(`/api/ProductImages/${imageId}`, {
                                method: 'DELETE'
                            });

                            const data = await response.json();

                            if (data.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Đã xóa!',
                                    text: data.message,
                                    timer: 1500,
                                    showConfirmButton: false
                                });

                                const imageCard = btn.closest('.col-md-3');
                                if (imageCard) {
                                    imageCard.remove();
                                }

                                if (document.querySelectorAll('.image-card').length === 0) {
                                    document.getElementById('productImagesContainer').innerHTML =
                                        '<div class="alert alert-warning"><i class="bi bi-exclamation-triangle me-2"></i>Sản phẩm chưa có ảnh. Vui lòng thêm ít nhất một ảnh.</div>';
                                }
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Lỗi',
                                    text: data.message
                                });
                            }
                        } catch (error) {
                            Swal.fire({
                                icon: 'error',
                                title: 'Lỗi',
                                text: 'Không thể xóa ảnh'
                            });
                        }
                    }
                });
            });
        });

        document.querySelectorAll('.btn-set-primary').forEach(btn => {
            btn.addEventListener('click', async function () {
                const imageId = this.getAttribute('data-image-id');
                const productId = currentProductId || document.getElementById('productId')?.value;

                const originalHTML = this.innerHTML;
                this.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';
                this.disabled = true;

                try {
                    const response = await fetch(`/api/ProductImages/${imageId}/set-primary`, {
                        method: 'POST'
                    });

                    const data = await response.json();

                    if (data.success) {
                        document.querySelectorAll('.btn-set-primary').forEach(b => {
                            b.classList.remove('active');
                            b.innerHTML = '<i class="bi bi-star"></i>';
                            b.disabled = false;
                        });

                        this.classList.add('active');
                        this.innerHTML = '<i class="bi bi-star-fill"></i>';
                        this.disabled = true;

                        document.querySelectorAll('.image-card').forEach(card => {
                            card.classList.remove('border-primary');
                        });
                        this.closest('.image-card').classList.add('border-primary');

                        document.querySelectorAll('.image-card small').forEach(label => {
                            label.classList.remove('text-primary', 'fw-bold');
                            label.innerHTML = '';
                        });
                        this.closest('.image-card').querySelector('small').innerHTML = '<span>Ảnh chính</span>';
                        this.closest('.image-card').querySelector('small').classList.add('text-primary', 'fw-bold');

                        Swal.fire({
                            icon: 'success',
                            title: 'Thành công!',
                            text: data.message,
                            timer: 1500,
                            showConfirmButton: false
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Lỗi',
                            text: data.message
                        });
                        this.innerHTML = originalHTML;
                        this.disabled = false;
                    }
                } catch (error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Không thể cập nhật'
                    });
                    this.innerHTML = originalHTML;
                    this.disabled = false;
                }
            });
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        initProductImageModal();

        bindImageActions();
    });

    window.productImageModal = {
        init: initProductImageModal,
        show: function (productId) {
            currentProductId = productId;
            document.getElementById('productId').value = productId;
            const modal = new bootstrap.Modal(document.getElementById('productImageModal'));
            modal.show();
        },
        hide: function () {
            const modal = bootstrap.Modal.getInstance(document.getElementById('productImageModal'));
            modal?.hide();
        },
        setCurrentProductId: function (productId) {
            currentProductId = productId;
        },
        reloadImages: function () {
            if (currentProductId) {
                loadProductImages(currentProductId);
            }
        }
    };

})();