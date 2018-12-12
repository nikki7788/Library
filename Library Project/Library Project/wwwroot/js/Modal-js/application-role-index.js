
$(document).ready(function () {

    // This is need so the content gets replaced correctly.
    $("#modal-action-application-role").on("show.bs.modal", function (e) {

        //المنتی که روی ان کلیک کردیم را برمیگرداند که در اینجا دکمه و
        // a تگ 
        //افزودن نقش جدید است که همه اتریبیپت های ان را نیز انتخاب میکند که 
        //asp-action=AddRole
        //که در واقع همان 
        //است را نیز در برمیگیردومقدار ان را داخل متغیر  href
        // میریزد link
        var link = $(e.relatedTarget);

        //این دستور کلاس 
        //modal-content
        // را پیداکرده و ادرس زیر را  
        //admin/applicationrole/AddRole
        //در داخل
        //href
        //ان قرار میدهد.
        //که این ادرس از روی 
        //href or asp-action =AddRole
        //دکمه و تگ ای ریخته شده در 
        // linkمتغیر 
        //گرفته شده است به این ترتیب 
        //action AddRole
        //اجرا میشود که ان هم پارشال 
        //AddEditApplicationRolePartial
        //را اجرا میکند و مودال به درستی اجرا میشود
        $(this).find(".modal-content").load(link.attr("href"));
    });

    //$("#modal-action-application-role").on("hide.bs.modal", function (e) {
    //    $(this).removeData('bs.modal');
    //});





 
    //------------################################################################################-----------------------
    
    //دستور کامل
    //$(document).ready(function () {
    //    var loadingContent = '<div class="modal-header"><h1>Processing...</h1></div><div class="modal-body"><div class="progress progress-striped active"><div class="bar" style="width: 100%;"></div></div></div>';

    //    // This is need so the content gets replaced correctly.
    //    $("#myModal").on("show.bs.modal", function (e) {
    //        $(this).find(".modal-content").html(loadingContent);
    //        var link = $(e.relatedTarget);
    //        $(this).find(".modal-content").load(link.attr("href"));
    //    });

    //    $("#myModal2").on("hide.bs.modal", function (e) {
    //        $(this).removeData('bs.modal');
    //    });
    //});

});


