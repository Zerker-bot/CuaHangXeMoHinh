document.addEventListener('DOMContentLoaded', function () {
    const container = document.getElementById('productsContainer');
    const btns = document.querySelectorAll('.view-btn');

    const viewMode = getCookie('ViewMode') || 'grid';
    setViewMode(viewMode);

    const sortSelect = document.getElementById('sortSelect');
    if (sortSelect) {
        sortSelect.addEventListener('change', function () {
            const currentUrl = new URL(window.location.href);
            currentUrl.searchParams.set('sortBy', this.value);
            currentUrl.searchParams.set('page', '1');
            window.location.href = currentUrl.toString();
        });
    }
    function setViewMode(mode) {
        if (mode === 'list') {
            container.classList.remove('products-grid');
            container.classList.add('products-list');
        } else {
            container.classList.remove('products-list');
            container.classList.add('products-grid');
        }

        btns.forEach(btn => btn.classList.remove('active'));
        const activeBtn = document.querySelector(`.view-btn[data-mode='${mode}']`);
        if (activeBtn) activeBtn.classList.add('active');

        setCookie('ViewMode', mode, 30);
    }

    btns.forEach(btn => {
        btn.addEventListener('click', function () {
            const mode = this.dataset.mode;
            setViewMode(mode);
        });
    });

    function setCookie(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }

    function getCookie(name) {
        const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
        return match ? match[2] : null;
    }
});

document.addEventListener("DOMContentLoaded", function () {
    var slider = document.getElementById('slider-range');
    var minInput = document.getElementById('minPriceInput');
    var maxInput = document.getElementById('maxPriceInput');
    var priceDisplay = document.getElementById('priceDisplay'); 

    if (slider && minInput && maxInput) {
        var minVal = parseInt(minInput.value) || 0;
        var maxVal = parseInt(maxInput.value) || 10000000;

        const formatCurrency = (value) => {
            return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
        }

        const updateTextDisplay = (min, max) => {
            if (priceDisplay) {
                priceDisplay.textContent = `Khoảng giá: ${formatCurrency(min)} - ${formatCurrency(max)}`;
            }
        }

        noUiSlider.create(slider, {
            start: [minVal, maxVal],
            connect: true,
            step: 100000,
            range: {
                'min': 0,
                'max': 20000000
            },
            format: {
                to: function (value) { return Math.round(value); },
                from: function (value) { return Number(value); }
            }
        });

        slider.noUiSlider.on('update', function (values, handle) {
            var currentMin = values[0];
            var currentMax = values[1];

            if (handle === 0) minInput.value = currentMin;
            if (handle === 1) maxInput.value = currentMax;

            updateTextDisplay(currentMin, currentMax);
        });

        minInput.addEventListener('change', function () {
            var val = this.value;
            slider.noUiSlider.set([val, null]);
        });

        maxInput.addEventListener('change', function () {
            var val = this.value;
            slider.noUiSlider.set([null, val]);
        });

        updateTextDisplay(minVal, maxVal);
    }
});