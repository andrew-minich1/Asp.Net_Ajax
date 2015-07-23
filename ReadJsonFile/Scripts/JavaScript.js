
$(function () {

    var showLoginForm = false;
    var commentError = true;
    $(document.body).on('click', 'a.login_link', function (event) {
        event.preventDefault();
        if (showLoginForm) {
            $("div.login_form_container").slideUp("fast");
            $('.login_form').trigger('reset');
            showLoginForm = false;
        }
        else {
            $("div.login_form_container").slideDown("fast");
            $('.login_form button:submit').addClass("login_submit");
            showLoginForm = true;
        }
    });


    $(document.body).on('click', '.pageNumber', function (event) {
        event.preventDefault();
        var pageNumber = this.innerHTML;
        var data = { pageNumber: pageNumber };
        $.ajax({
            type: "GET",
            url: '/Home/LoadComments',
            data: data,
            cache: false,
            success: function (response) {
                $("#productCommentsContainer").html($("#showComment").tmpl(response.Comments));
                $("#paging").html($("#showPaging").tmpl(response));
            }
        })
    });


    $('.add_comment_form textarea').blur(function (event) {
        if (this.value.length < 3) {
            this.classList.remove("field-validation-valid");
            this.classList.add("field-validation-error");
            commentError = true;
        }
        else {
            this.classList.remove("field-validation-error");
            this.classList.add("field-validation-valid");
            commentError = false;
        }
    });


    $("#addCommentButton").click(function (event) {
        event.preventDefault();
        if (commentError) return;
        var data = $('form').serialize();
        $.ajax({
            type: "POST",
            url: '/Home/AddComment',
            data: data,
            cache: false,
            dataType: "html",
            success: function (response) {
                var comment = $.parseJSON(response);
                $("#newComments").prepend($("#showComment").tmpl(comment));
                $('form').trigger('reset');
            }
        });
    });


    $(document.body).on('click', '.login_submit', function (event) {
        event.preventDefault();
        var fields = $('.login_form').serialize();
        $.ajax({
            type: "POST",
            url: '/Account/Login',
            data: fields,
            cache: false,
            error: function () {
                window.location = "/Account/Login";
            },
            success: function (response) {
                if (response == "") return;
                var result ='<li><a href="#">' + response + '</a><ul><li><a href="/Account/LogOff" class="log_off">Exit</a></li></ul></li>';
                $("#mainNavbar").html(result);
                $("div.login_form_container").slideUp("fast");
                $('.login_form').trigger('reset');
                showLoginForm = false;
            }
        })
    });


    $(document.body).on('click', '.log_off', function (event) {
        event.preventDefault();
        $.ajax({
            type: "GET",
            url: '/Account/LogOff',
            cache: false,
            success: function () {
                var result = '<li><a href="/Account/Registration">Registration</a></li><li><a class="login_link" href="/Account/Login">Login</a></li>';
                $("#mainNavbar").html(result);
            }
        })
    });

});


