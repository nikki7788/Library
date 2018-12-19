
$(document).ready(function () {

    $("#modal-action-book").on("show.bs.modal", function (e) {
        var link = $(e.relatedTarget);
        $(this).find(".modal-content").load(link.attr("href"));
    });

});