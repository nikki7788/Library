$(document).ready(function () {
    $(".menu li.first").children('ul').show();     //به طور پیش فرض پنل مدیریت سیستم باز باشد وبقیه پنل ها بته باشد

    $(".menu li.main-li > a").click(function () {
        var getli = $(this).parent('li');
        //   getli.find('ul').slideToggle(200);

        getli.children('ul').slideToggle(200)                                 //نمایش و محو ایتم های پنل ها
            .siblings('a').children('i').toggleClass("fa-times").toggleClass("fa-plus");           //تعویض ایکن ضربدر به بعلاوه و بالعکس
    });
});