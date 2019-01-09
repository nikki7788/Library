

//این کد کلی است و برای همه مودال ها جواب می دهد نیاز نیست که باری هر مودال جداگانه بنویسیم
$(document).ready(function () {

    $("a").on('click', function (e) {
        var item = $(this);

        //data-target
        var id = $(this).data("target");
        $(id).on("show.bs.modal", function (e) {
            //$(this) = index view
            $(this).find(".modal-content").load(item.attr("href"));
        });
    });

});