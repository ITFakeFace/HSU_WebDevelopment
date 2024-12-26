$(document).ready(function () {
    $('#data-table').DataTable();
});

$(document).on("click", ".btn-delete", function () {
    let libId = $(this).parent().data("id")
    $(`#action-box-${libId}`).hide();
    $(`#delete-confirm-box-${libId}`).show();
});

$(document).on("click", ".btn-cancel", function () {
    let libId = $(this).parent().data("id")
    $(`#action-box-${libId}`).show();
    $(`#delete-confirm-box-${libId}`).hide();
});