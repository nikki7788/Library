

//این کد کلی است و برای همه مودال ها جواب می دهد نیاز نیست که باری هر مودال جداگانه بنویسیم


$(document).ready(function () {

    var editorContainer = document.querySelector('#BookDescription');

    var check_if_empty = function (val) {

        return $.makeArray($(val)).every(function (el) {

            return el.innerHTML.replace(/&nbsp;|\s/g, '').length === 0;
        });
    };

    var clear_input_if_empty_content = function (input) {

        var $form = $(input).parents('form');

        $form.on('submit', function () {

            if (check_if_empty($(input).val())) {

                $(input).val('');
            }
        });
    };

    ClassicEditor.create(editorContainer)
        .then(function (editor) {
            clear_input_if_empty_content(editorContainer)
        })
        .catch(function (error) {
            console.log(error);
        });
    defaultPrevented();
});


