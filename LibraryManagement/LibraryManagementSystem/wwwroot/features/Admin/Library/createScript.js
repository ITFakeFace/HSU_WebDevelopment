$(document).ready(function () {
    $('#manager-select').select2({
        placeholder: "Select a manager", // Placeholder
        allowClear: true, // Hiển thị nút xóa
        width: '100%' // Đảm bảo full-width
    });

    // Cài đặt màu sắc cho các option dựa trên data-role
    $('#manager-select').on('select2:open', function () {
        $('#manager-select option').each(function () {
            const role = $(this).data('role');
            const color = role === "ADMINISTRATOR" ? "red" : "green";
            $(this).css('color', color);
        });
    });

    // Initialize Select2
    $('#city-select, #district-select, #ward-select, #street-select').select2({
        placeholder: "Chọn một tùy chọn",
        allowClear: true,
        tags: true // Cho phép thêm giá trị mới
    });

    // Load cities on page load
    $.ajax({
        url: 'http://localhost:5292/api/Address/cities', // API endpoint for cities
        method: 'GET',
        success: function (data) {
            const cities = JSON.parse(data).Data;
            $('#city-select').empty().append('<option></option>');
            cities.forEach(city => {
                $('#city-select').append(new Option(`${city.Name}`, city.Id));
            });
        },
        error: function () {
            console.log('Không thể tải danh sách thành phố.');
        }
    });

    // On City change, load districts
    $('#city-select').on('change', function () {
        const cityId = $(this).val();
        if (cityId) {
            $('#district-select').prop('disabled', false);
            $.ajax({
                url: `http://localhost:5292/api/Address/districts/city/${cityId}`,
                method: 'GET',
                success: function (data) {
                    const districts = JSON.parse(data).Data;
                    $('#district-select').empty().append('<option></option>');
                    districts.forEach(district => {
                        $('#district-select').append(new Option(`${district.Name}`, district.Id));
                    });
                },
                error: function () {
                    console.log('Không thể tải danh sách quận/huyện.');
                }
            });
        } else {
            $('#district-select, #ward-select, #street-select').prop('disabled', true).empty();
        }
    });

    // On District change, load wards
    $('#district-select').on('change', function () {
        const districtId = $(this).val();
        if (districtId) {
            $('#ward-select').prop('disabled', false);
            $.ajax({
                url: `http://localhost:5292/api/Address/wards/district/${districtId}`,
                method: 'GET',
                success: function (data) {
                    const wards = JSON.parse(data).Data;
                    $('#ward-select').empty().append('<option></option>');
                    wards.forEach(ward => {
                        $('#ward-select').append(new Option(`${ward.Name}`, ward.Id));
                    });
                },
                error: function () {
                    console.log('Không thể tải danh sách phường/xã.');
                }
            });
        } else {
            $('#ward-select, #street-select').prop('disabled', true).empty();
        }
    });

    // On Ward change, load streets
    $('#ward-select').on('change', function () {
        const wardId = $(this).val();
        if (wardId) {
            $('#street-select').prop('disabled', false);
            $.ajax({
                url: `http://localhost:5292/api/Address/streets/ward/${wardId}`,
                method: 'GET',
                success: function (data) {
                    const streets = JSON.parse(data).Data;
                    $('#street-select').empty().append('<option></option>');
                    streets.forEach(street => {
                        $('#street-select').append(new Option(`${street.Name}`, street.Id));
                    });
                },
                error: function () {
                    console.log('Không thể tải danh sách đường.');
                }
            });
        } else {
            $('#street-select').prop('disabled', true).empty();
        }
    });

    // Submit form and retrieve the text of selected options
    $('#form-create').submit(function (e) {
        e.preventDefault();

        // Lấy text của các lựa chọn được chọn
        const cityText = $('#city-select option:selected').text();
        const districtText = $('#district-select option:selected').text();
        const wardText = $('#ward-select option:selected').text();
        const streetText = $('#street-select option:selected').text();
        const addressText = $('#address-select option:selected').text();

        // Gán các giá trị text vào các trường input ẩn (nếu cần)
        $(this).find('input[id="CityText"]').val(cityText);
        $(this).find('input[id="DistrictText"]').val(districtText);
        $(this).find('input[id="WardText"]').val(wardText);
        $(this).find('input[id="StreetText"]').val(streetText);
        $(this).find('input[id="AddressText"]').val(addressText);

        // Sau khi gán text vào input, gửi form
        this.submit();
    });
});
