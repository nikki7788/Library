
//as soon as the page is loaded and then if  the id=submitdata is clicked ,run this function ...
$(document).on("click", "#submitdata", function (ev) {
    //ev parameter is contained in the event of clicking on the #submitdata

    //the default action of the event(=ev) will not be triggered
    ev.preventDefault();

    //for sending data from a form or a Model to the server
    //we should append all data(=files + input values + slectList values)from form to the myData
    var myData = new FormData();

    //----------###########-------- دریافت مقادیر اپلودی   -----#######---------------

    //for recieving value(=file) and data from the  tag with  id= files
    var fileUpload = $("#files").get(0);

    //we need this code whenever there are more than a file for uploading
    //all files in this tag with  id= files ,will be put into the myFiles
    var myFiles = fileUpload.files;

    //this code appends values of the myFiles  to the myData
    for (var i = 0; i < myFiles.length; i++) {
       
        //append("the name attr of the tag ",the tag value )
        myData.append("files", myFiles[i]);
    }
    //--------------------#####################------------------------------------------------

    //----############---- دریافت مقادیر اینپوت ها و کمبوباکس ها  ---##############----------------
    //getting the values of the inputs and selects
    $("input,select").each(function (x, y) {

        //x is indexer that means it is a counter      
        //y = the selcted tag by each method in each loop    ورودی فانکشن باربر است با تگ انتخاب شده توسط حلقه در هربار حلقه زدن و لوپ
        // متغیر اول شمارنده و ایندکسر است و متغیر دوم تگ متناظر با شماذنده را می اورد
        //append("the name attr of the tag = e.g. asp-for="pageCount" ",the tag value = e.g. pages of book )
        myData.append($(y).attr("name"), $(y).val());

    });

    //-------------------------##########################--------------------------------------

    //----############----CKEDITOR دریافت مقادیر   ---##############----------------
    var editorVal = myEditor.getData();
    myData.append("BookDescription", editorVal);

    //-------------------------##########################--------------------------------------

    $.ajax({

        type: "post",
        url: '@Url.Action("AddEditBook","Book", new { area = "Admin"})',
       // url: 'Book/AddEditBook',
        contentType: false,
        processData: false,    //false : don't proccess any action on my data
        data: myData          //sending data


        //when data is sent into the server and after that its response recieved from the server
        //status and message are defined in action
    }).done(function (res) {
        //checking ajax result (in data type and being successful )
        if (res.status === "success") {
            alert(res.message);
        } else {
            alert("در ثبت اطلاعات مشکلی بجود امده است");
        }
        //when data isn't sent into the server and occured an error in the client side or at the beginig of entry into the sever
    }).fail(function (xhr, b, error) {
        alert(error);
    });

});
