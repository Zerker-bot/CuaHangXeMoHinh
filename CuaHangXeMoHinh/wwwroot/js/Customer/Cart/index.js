$(document).ready(function () {
    const CART = {
        selectors: {
            addToCartBtn: '.add-to-cart-btn',
            cartItem: '.cart-item',
            quantityDecrease: '.quantity-decrease',
            quantityIncrease: '.quantity-increase',
            quantityUpdate: '.quantity-update',
            quantityInput: '.quantity-input',
            removeItemBtn: '.remove-item-btn',
            clearCartBtn: '#clear-cart-btn',
            cartCount: '.cart-count',
            cartBadge: '.cart-badge'
        },
        urls: {
            add: '/Cart/Add',
            update: '/Cart/Update',
            remove: '/Cart/Remove',
            clear: '/Cart/Clear',
            getCount: '/Cart/GetCartCount'
        }
    };

    // Hiển thị thông báo từ TempData
    if (typeof showNotificationFromTempData !== 'undefined') {
        showNotificationFromTempData();
    }

    // ============ XỬ LÝ THÊM VÀO GIỎ HÀNG ============
    $(document).on('click', CART.selectors.addToCartBtn, handleAddToCart);

    // ============ XỬ LÝ TĂNG/GIẢM SỐ LƯỢNG TRONG GIỎ HÀNG ============
    $(document).on('click', CART.selectors.quantityDecrease, handleDecreaseQuantity);
    $(document).on('click', CART.selectors.quantityIncrease, handleIncreaseQuantity);
    $(document).on('change', CART.selectors.quantityUpdate, handleQuantityChange);

    // ============ XỬ LÝ TĂNG/GIẢM SỐ LƯỢNG TRONG TRANG DETAIL ============
    $(document).on('click', '.product-details .quantity-decrease', handleDetailDecrease);
    $(document).on('click', '.product-details .quantity-increase', handleDetailIncrease);
    $(document).on('change', '.product-details .quantity-input', handleDetailQuantityChange);

    // ============ XỬ LÝ XÓA SẢN PHẨM ============
    $(document).on('click', CART.selectors.removeItemBtn, handleRemoveItem);
    $(CART.selectors.clearCartBtn).on('click', handleClearCart);

    // ============ KHỞI TẠO ============
    updateCartCount();
    initializeQuantityControls();

    // ============ CÁC HÀM XỬ LÝ CHÍNH ============

    function handleAddToCart(e) {
        e.preventDefault();
        const $button = $(this);
        const productId = $button.data('product-id');
        const productName = $button.data('product-name');
        const isDetailPage = $button.closest('.product-details').length > 0;

        let quantity = 1;

        if (isDetailPage) {
            // Trang detail
            const $quantityInput = $(`#quantity-input-${productId}`);
            quantity = parseInt($quantityInput.val()) || 1;
        } else {
            // Product card
            quantity = parseInt($button.closest('.product-card').find(CART.selectors.quantityInput).val()) || 1;
        }

        if (quantity < 1) {
            showNotification('error', 'Số lượng phải lớn hơn 0');
            return;
        }

        executeAjaxRequest({
            url: CART.urls.add,
            data: { productId, quantity },
            $button,
            originalText: $button.html(),
            successCallback: function (response) {
                showNotification('success', response.message);
                updateCartCount();
            }
        });
    }

    function handleDecreaseQuantity(e) {
        if (!$(this).closest(CART.selectors.cartItem).length) return;
        e.preventDefault();

        const $button = $(this);
        const productId = $button.data('product-id');
        const $input = $(`${CART.selectors.quantityUpdate}[data-product-id="${productId}"]`);
        const currentValue = parseInt($input.val());
        const minValue = parseInt($input.attr('min')) || 1;

        if (currentValue > minValue) {
            $input.val(currentValue - 1);
            updateCartItemQuantity(productId, currentValue - 1);
        }
    }

    function handleIncreaseQuantity(e) {
        if (!$(this).closest(CART.selectors.cartItem).length) return;
        e.preventDefault();

        const $button = $(this);
        const productId = $button.data('product-id');
        const $input = $(`${CART.selectors.quantityUpdate}[data-product-id="${productId}"]`);
        const currentValue = parseInt($input.val());
        const maxValue = parseInt($input.attr('max')) || 999;

        if (currentValue < maxValue) {
            $input.val(currentValue + 1);
            updateCartItemQuantity(productId, currentValue + 1);
        } else {
            showNotification('error', 'Đã đạt số lượng tối đa trong kho');
        }
    }

    // ============ XỬ LÝ TRANG DETAIL ============
    function handleDetailDecrease(e) {
        e.preventDefault();
        const $button = $(this);
        const productId = $button.data('product-id');
        const $input = $(`#quantity-input-${productId}`);
        const currentValue = parseInt($input.val());
        const minValue = parseInt($input.attr('min')) || 1;

        if (currentValue > minValue) {
            $input.val(currentValue - 1);
        }
    }

    function handleDetailIncrease(e) {
        e.preventDefault();
        const $button = $(this);
        const productId = $button.data('product-id');
        const $input = $(`#quantity-input-${productId}`);
        const currentValue = parseInt($input.val());
        const maxValue = parseInt($input.attr('max')) || 999;

        if (currentValue < maxValue) {
            $input.val(currentValue + 1);
        } else {
            showNotification('error', 'Đã đạt số lượng tối đa trong kho');
        }
    }

    function handleDetailQuantityChange() {
        const $input = $(this);
        const productId = $input.data('product-id');
        const quantity = parseInt($input.val());

        if (isNaN(quantity) || quantity < 1) {
            showNotification('error', 'Số lượng phải lớn hơn 0');
            $input.val($input.data('old-value') || 1);
            return;
        }

        const maxValue = parseInt($input.attr('max')) || 999;
        if (quantity > maxValue) {
            showNotification('error', 'Số lượng vượt quá tồn kho');
            $input.val(maxValue);
        }

        $input.data('old-value', quantity);
    }

    function handleQuantityChange() {
        const $input = $(this);
        const productId = $input.data('product-id');
        let quantity = parseInt($input.val());

        if (isNaN(quantity) || quantity < 1) {
            showNotification('error', 'Số lượng phải lớn hơn 0');
            $input.val($input.data('old-value') || 1);
            return;
        }

        const maxValue = parseInt($input.attr('max')) || 999;
        if (quantity > maxValue) {
            showNotification('error', 'Số lượng vượt quá tồn kho');
            quantity = maxValue;
            $input.val(maxValue);
        }

        updateCartItemQuantity(productId, quantity);
    }

    function handleRemoveItem() {
    const $button = $(this);
    const productId = $button.data('product-id');

    // Thêm class loading cho nút
    $button.prop('disabled', true).addClass('loading');

    executeAjaxRequest({
        url: CART.urls.remove,
        data: { productId },
        $button,
        successCallback: function (response) {
            // Xóa item khỏi DOM với hiệu ứng mượt
            const $item = $(`#cart-item-${productId}`);
            $item.fadeOut(300, function() {
                $(this).remove();
                updateCartUI(response);

                // Nếu giỏ hàng trống, hiển thị toast
                if ($(CART.selectors.cartItem).length === 0) {
                    showNotification('success', 'Giỏ hàng đã trống');
                    setTimeout(() => location.reload(), 1000);
                } else {
                    showNotification('success', response.message);
                }
            });
        }
    });
}
    function handleClearCart() {
        Swal.fire({
            title: 'Xác nhận',
            text: 'Bạn có chắc chắn muốn xóa toàn bộ giỏ hàng?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Có, xóa hết!',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                executeAjaxRequest({
                    url: CART.urls.clear,
                    $button: $(CART.selectors.clearCartBtn),
                    successCallback: function (response) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Thành công',
                            text: response.message,
                            timer: 1500,
                            showConfirmButton: false
                        });

                        updateCartCountInLayout(0);
                        setTimeout(() => location.reload(), 1000);
                    }
                });
            }
        });
    }

    // ============ CÁC HÀM HỖ TRỢ ============

    function executeAjaxRequest({ url, data = {}, $button = null, originalText = null, successCallback }) {
        const requestData = {
            ...data,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        if ($button) {
            $button.prop('disabled', true);
            
        }

        $.ajax({
            url,
            type: 'POST',
            data: requestData,
            success: function (response) {
                if (response.success) {
                    successCallback(response);
                } else {
                    showNotification('error', response.message);
                }
                resetButton($button, originalText);
            },
            error: function () {
                showNotification('error', 'Đã xảy ra lỗi kết nối. Vui lòng thử lại.');
                resetButton($button, originalText);
            }
        });
    }

    function updateCartItemQuantity(productId, quantity) {
        if (!productId || !quantity) return;

        executeAjaxRequest({
            url: CART.urls.update,
            data: { productId, quantity },
            successCallback: function (response) {
                showNotification('success', response.message);
                updateCartUI(response, productId); // Thêm productId
                updateQuantityButtonState(productId, quantity);
            }
        });
    }

    function updateCartUI(response, productId   ) {
        // Cập nhật THÀNH TIỀN của từng sản phẩm
        if (productId && response.itemTotal) {
            $(`#item-total-${productId}`).text(response.itemTotal + ' đ');

            // Hiển thị công thức tính (giá x số lượng)
            const $totalCell = $(`#item-total-${productId}`).closest('td');
            const price = $totalCell.find('.item-price').data('price') || 0;
            const quantity = parseInt($(`.quantity-update[data-product-id="${productId}"]`).val()) || 1;

            // Cập nhật công thức tính nếu có element chứa
            const $formulaElement = $totalCell.find('.price-formula');
            if ($formulaElement.length) {
                $formulaElement.text(`${price.toLocaleString('vi-VN')} đ x ${quantity}`);
            }
        }

        // Cập nhật tổng tiền
        $('#cart-subtotal').text(response.subtotal + ' đ');

        // Tính tổng cộng bao gồm phí vận chuyển
        const subtotalNum = parseInt(response.subtotal.replace(/\./g, '') || 0);
        const shippingFee = 30000;
        const totalWithShipping = subtotalNum + shippingFee;
        $('#cart-total').text(totalWithShipping.toLocaleString('vi-VN') + ' đ');

        // Cập nhật số lượng
        updateCartCountInLayout(response.cartItemCount);
    }

    function updateCartCount() {
        $.ajax({
            url: CART.urls.getCount,
            type: 'GET',
            success: function (data) {
                updateCartCountInLayout(data.count);
            },
            error: function () {
                console.error('Không thể cập nhật số lượng giỏ hàng');
            }
        });
    }

    function updateCartCountInLayout(count) {
        $(CART.selectors.cartCount).text(count);
        $(CART.selectors.cartBadge).text(count);

    }

    function updateQuantityButtonState(productId, currentQuantity) {
        const $input = $(`${CART.selectors.quantityUpdate}[data-product-id="${productId}"]`);
        const maxStock = parseInt($input.attr('max')) || 999;

        const $decreaseBtn = $(`${CART.selectors.quantityDecrease}[data-product-id="${productId}"]`);
        const $increaseBtn = $(`${CART.selectors.quantityIncrease}[data-product-id="${productId}"]`);

        if ($decreaseBtn.length) {
            $decreaseBtn.prop('disabled', currentQuantity <= 1);
        }
        if ($increaseBtn.length) {
            $increaseBtn.prop('disabled', currentQuantity >= maxStock);
        }

        $input.data('old-value', currentQuantity);
    }

    function initializeQuantityControls() {
        // Khởi tạo cho giỏ hàng
        $(CART.selectors.quantityUpdate).each(function () {
            const $input = $(this);
            const productId = $input.data('product-id');
            const currentQuantity = parseInt($input.val());

            if (productId) {
                updateQuantityButtonState(productId, currentQuantity);
            }
        });

        // Khởi tạo cho trang detail
        $('.product-details .quantity-input').each(function () {
            const $input = $(this);
            const currentValue = parseInt($input.val());
            $input.data('old-value', currentValue);
        });
    }

    function resetButton($button, originalText) {
        if ($button) {
            $button.prop('disabled', false);
        }
    }

    // ============ HÀM HIỂN THỊ THÔNG BÁO ============
    function showNotification(type, message) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: type,
            title: message,
            showConfirmButton: false,
            timer: 2000,
            timerProgressBar: true
        });
    }



    function showNotificationFromTempData() {
        const errorMessage = $('#error-message').val();
        const successMessage = $('#success-message').val();

        if (errorMessage) showNotification('error', errorMessage);
        if (successMessage) showNotification('success', successMessage);
    }
});