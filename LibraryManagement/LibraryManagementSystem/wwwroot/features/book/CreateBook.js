const baseUrl = "https://localhost:50282/api";

async function callAPI(path, method) {
    const url = baseUrl + path;
    method = method.toUpperCase();

    return new Promise((resolve, reject) => {
        $.ajax({
            url: url,
            type: method,
            dataType: "json",
            success: function (data) {
                if (data.IsSuccess) {
                    resolve(data.Data); // Thành công, trả về data
                } else {
                    reject(data.Message); // Không thành công, trả về lỗi
                }
            },
            error: function (xhr, status, error) {
                reject(error); // Lỗi trong quá trình request
            },
        });
    });
}

class ApiBuilder {
    constructor() {
        this.controller = "";
        this.action = "";
        this.param = null;
    }

    setController(controller) {
        this.controller = controller;
        return this;
    }

    setAction(action) {
        this.action = action;
        return this;
    }

    setParam(param) {
        this.param = param;
        return this;
    }

    build() {
        return this.param
            ? `/${this.controller}/${this.action}/${this.param}`
            : `/${this.controller}/${this.action}`;
    }
}
async function initializeSelect2(htmlElement, controller, placeholder) {
    try {
        const builder = new ApiBuilder();
        const api = builder.setController(controller).setAction("getAll").build();
        const data = await callAPI(api, "get");

        $(htmlElement).select2({
            placeholder: `Select ${placeholder}`,
            width: "100%",
            dropdownCssClass: "custom-dropdown",
            selectionCssClass: "custom-selection",
            data: data.map((item) => ({
                id: item.Id,
                text: item.Name,
            })),
        });
    } catch (error) {
        console.error(`Error initializing Select2 for ${htmlElement}:`, error);
    }
}

document.addEventListener("DOMContentLoaded", function () {
    initializeSelect2("#author", "author", "Author");
    initializeSelect2("#publisher", "publisher", "Publisher");
    initializeSelect2("#vendor", "vendor", "Vendor");
    initializeSelect2("#series", "series", "Series");

});
