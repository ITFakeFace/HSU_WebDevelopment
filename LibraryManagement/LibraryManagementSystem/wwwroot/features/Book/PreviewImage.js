$(function () {
    const inputFile = $("#file");
    const imgArea = $(".img-area");
    const titleArea = $(".title-area");

    inputFile.on("change", function () {
        const images = this.files;
        if (images.length > 0) {
            titleArea.hide();

            $.each(images, function (index, image) {
                if (image.size < 2000000) {
                    const reader = new FileReader();
                    reader.onload = (e) => {
                        // Tạo div bọc ngoài ảnh
                        const imgContainer = $("<div>").addClass("img-item")
                            .attr("data-img", image.name);

                        // Tạo thẻ img và thêm vào div
                        const img = $("<img>").attr({
                            src: e.target.result,
                            alt: image.name,  // Lưu tên ảnh vào thuộc tính alt
                        });

                        // Thêm ảnh vào div
                        imgContainer.append(img);

                        // Thêm div vào imgArea
                        imgArea.append(imgContainer).addClass("active");

                        // Gắn sự kiện click vào div để xóa ảnh
                        imgContainer.on("click", function () {
                            $(this).remove();  // Xóa ảnh khi click vào div
                            inputFile.click();  // Mở hộp thoại chọn ảnh
                        });

                        adjustImageWidth();
                    };
                    reader.readAsDataURL(image);
                } else {
                    alert(`Image ${image.name} size is more than 2MB`);
                }
            });
        }
    });

    function adjustImageWidth() {
        const imgCount = imgArea.find("img").length;
        const imgWidth = imgCount > 0
            ? (imgArea.width() - (imgCount - 1) * 10) / imgCount
            : 0;
        imgArea.find("img").css("width", imgWidth + "px");
    }
});
