


$(document).ready(function () {

    $("#modal-action-author").on('show.bs.modal', function (e) {
        var link = $(e.relatedTarget);
        $(this).find(".modal-content").load(link.attr("href"));
    });

});