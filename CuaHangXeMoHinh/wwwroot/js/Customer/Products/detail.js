$(document).ready(function () {
    // Hàm đồng bộ số lượng từ input sang form mua ngay
    function syncQuantityToBuyNow() {
        const productId = window.productDetail?.productId;
        if (!productId) return;

        const quantity = $(`#quantity-input-${productId}`).val();
        $(`#buynow-quantity-${productId}`).val(quantity);
    }

    // Hàm thay đổi ảnh sản phẩm
    window.changeImage = function (src, element) {
        $('#main-image').attr('src', src);
        $('.thumbnail').removeClass('active border-primary');
        $(element).addClass('active border-primary');
    }

    // Xử lý sự kiện số lượng
    $(document).on('click', '.quantity-decrease, .quantity-increase', function () {
        setTimeout(syncQuantityToBuyNow, 100);
    });

    $(document).on('change', '.quantity-input', syncQuantityToBuyNow);

    // Xử lý submit form mua ngay
    $(document).on('submit', '[id^="buynow-form-"]', function (e) {
        const formId = $(this).attr('id');
        const productId = formId.replace('buynow-form-', '');
        const quantity = parseInt($(`#quantity-input-${productId}`).val()) || 1;
        const stock = window.productDetail?.stock || 0;

        if (quantity < 1) {
            e.preventDefault();
            showNotification('error', 'Số lượng phải lớn hơn 0');
            return false;
        }

        if (quantity > stock) {
            e.preventDefault();
            showNotification('error', 'Số lượng vượt quá tồn kho');
            return false;
        }

        $(`#buynow-quantity-${productId}`).val(quantity);
        return true;
    });
    document.addEventListener('DOMContentLoaded', function () {
        const inputs = document.querySelectorAll('.stars input');
        const displayVal = document.getElementById('rating-value');

        inputs.forEach(input => {
            input.addEventListener('change', function () {
                displayVal.innerText = this.value;
            });
        });
    });
  
    // Khởi tạo
    syncQuantityToBuyNow();
    $('.thumbnail:first').addClass('active border-primary');
});