
$(document).ready(function () {

    $(".menu li.first").children('ul').show();     //به طور پیش فرض پنل مدیریت سیستم باز باشد وبقیه پنل ها بته باشد

   //-------------------------------------
      //var num = 0;
    //var num1 = 0;
    //getLocalStorage();
    //if (num % 2 !== 0) {  //اگر روی دکمه اخبار کلیک شده بود لیت زیر ان باز بماند بعد رفرش صفحه
    //    $(".cllo ul").show();
    //}
    //------------------------------------

    $(".menu li.main-li > a").click(function () {
        var getli = $(this).parent('li');
        //   getli.find('ul').slideToggle(200);
        getli.children('ul').slideToggle(200)                                 //نمایش و محو ایتم های پنل ها
            .siblings('a').children('i').toggleClass("fa-times").toggleClass("fa-plus");           //تعویض ایکن ضربدر به بعلاوه و بالعکس
    });


    //-----------------------------------------------------------
    ////اگر روی دکمه اخبار کلیک شده بود لیت زیر ان باز بماند بعد رفرش صفحه
    //$(".cllo ul").click(function () {
    //    //روی مدیریت خبرها که کلیک شد تا صفحه ان باز شود شمارنده یکی اضافه میشود و در شرط بالای بررسی میکنیم که اگر شمارنده فرد بود یعنی روی مدیریت خب لیک شده و لیست باز شو ان را باز نگه یدارد
    //    //اگر دوباره روی مدیریت خبر کلیک کنیم 
    //    num++;
    //    saveLocalStorage();
    //});

    //-----------------local storage-----------------------
    // save
    //function saveLocalStorage() {

    //    if (localStorage !== "undefined") {
    //        localStorage.setItem("num_st", num);
    //        localStorage.setItem("num1_st", num1);
    //        getLocalStorage();
    //    }
    //}
    ////load
    //function getLocalStorage() {

    //    if (typeof localStorage !== "undefined") {
    //        if (localStorage.num_st) {
    //            num = Number(localStorage.getItem("num_st"));

    //        }
    //        if (localStorage.num_st) {
    //            num = Number(localStorage.getItem("num1_st"));
    //        }

    //    }
    //}
    //-----------------local storage-----------------------

});