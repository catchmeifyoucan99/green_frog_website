$(document).ready(function () {
    $("#bars").click(function () {
        $("#bars").toggleClass("bars_active");
    }),

        $("#searchBtn").click(function () {
            $(".search").toggleClass("searchBtn");
        }),

        window.addEventListener("scroll", function () {
            $(this).scrollTop() >= 700 ? $(".go-to-top").fadeIn() : $(".go-to-top").fadeOut();
        }),

        $(".go-to-top").click(function () {
            $("html, body").animate({ scrollTop: 0 });
        });

    $('#op1').click(function () {
        $('.momo').addClass('active');
        $('.cod').removeClass('active');
    })

    $('#op2').click(function () {
        $('.momo').removeClass('active');
        $('.cod').addClass('active');
    })

    //for new product
    $('#newpro').click(function () {
        $(this).addClass('activeColor');
        $('.newlist').addClass('active');

        $('.newstyle').removeClass('active');
        $('.newequi').removeClass('active');

        $('#newpro2').removeClass('activeColor');
        $('#newpro3').removeClass('activeColor');
    })

    $('#newpro2').click(function () {
        $(this).addClass('activeColor');
        $('.newstyle').addClass('active');

        $('.newlist').removeClass('active');
        $('.newequi').removeClass('active');

        $('#newpro').removeClass('activeColor');
        $('#newpro3').removeClass('activeColor');
    })

    $('#newpro3').click(function () {
        $(this).addClass('activeColor');
        $('.newequi').addClass('active');

        $('.newstyle').removeClass('active');
        $('.newlist').removeClass('active');

        $('#newpro').removeClass('activeColor');
        $('#newpro2').removeClass('activeColor');
    })

    //for dis product
    $('#dispro').click(function () {
        $(this).addClass('activeColor');
        $('.dislist').addClass('active');

        $('.disstyle').removeClass('active');
        $('.disequi').removeClass('active');

        $('#dispro2').removeClass('activeColor');
        $('#dispro3').removeClass('activeColor');
    })

    $('#dispro2').click(function () {
        $(this).addClass('activeColor');
        $('.disstyle').addClass('active');

        $('.dislist').removeClass('active');
        $('.disequi').removeClass('active');

        $('#dispro').removeClass('activeColor');
        $('#dispro3').removeClass('activeColor');
    })

    $('#dispro3').click(function () {
        $(this).addClass('activeColor');
        $('.disequi').addClass('active');

        $('.disstyle').removeClass('active');
        $('.dislist').removeClass('active');

        $('#dispro').removeClass('activeColor');
        $('#dispro2').removeClass('activeColor');
    })

    //con giong
    $('#clickCat').click(function () {
        $(this).addClass('activeColor');

        $('.cat').addClass('active');

        $('.dog').removeClass('active');

        $('#clickDog').removeClass('activeColor');
    })

    $('#clickDog').click(function () {
        $(this).addClass('activeColor');

        $('.dog').addClass('active');

        $('.cat').removeClass('active');

        $('#clickCat').removeClass('activeColor');
    })


    //upload anh dai dien
    let input = document.getElementById('fileUpload');
    let span = document.getElementById('fileUploadSpan');
    //input.addEventListener('change', () => {
    $('#fileUpload').change(function () {
        let files = input.files;

        if (files.length > 0) {
            if (files[0].size > 3000 * 1024) {
                span.innerText = 'Ảnh tối đa không được quá 3mb';
                $('#pictureUpload').attr('src', null);
                $('#hinh').attr("required", true);
                $('#hinh').val(null);
                return;
            }

            else {
                $('#hinh').attr("required", false);
                var picture = new FileReader();
                picture.readAsDataURL(this.files[0]);
                picture.addEventListener('load', function (event) {
                    document.getElementById('pictureUpload').setAttribute('src', event.target.result);
                    document.getElementById('pictureUpload').style.display = 'block';
                });
            }
        }


        span.innerText = '';
    });

    //upload anh thu vien

    $('#uploadhinh').change(function () {
        let input1 = document.getElementById('uploadhinh');
        let span1 = document.getElementById('fileUploadSpan1');

        let filess = input1.files;

        if (filess.length > 0) {
            if (filess[0].size > 1000 * 1024) {
                span1.innerText = 'Ảnh tối đa không được quá 1mb';
                $('#hinh1').attr("required", true);
                $('#hinh1').attr("value", null);
                return;
            } 
        }
        if (window.FormData !== undefined){
            $('#hinh1').attr("required", false);
        }


        span1.innerText = '';
    });

    $('#uploadhinh1').change(function () {
        let input1 = document.getElementById('uploadhinh1');
        let span1 = document.getElementById('fileUploadSpan2');

        let filess = input1.files;

        if (filess.length > 0) {
            if (filess[0].size > 1000 * 1024) {
                span1.innerText = 'Ảnh tối đa không được quá 1mb';
                $('#hinh2').attr("required", true);
                $('#hinh2').attr("value", null);
                return;
            } else {
                var picture1 = new FileReader();
                picture1.readAsDataURL(this.files[0]);
                picture1.addEventListener('load', function (event) {
                    document.getElementById('imguploadhinh1').setAttribute('src', event.target.result);
                });
            }
        }
        if (window.FormData !== undefined) {
            $('#hinh2').attr("required", false);
        }

        span1.innerText = '';
    });

    $('#uploadhinh2').change(function () {
        let input1 = document.getElementById('uploadhinh2');
        let span1 = document.getElementById('fileUploadSpan3');

        let filess = input1.files;

        if (filess.length > 0) {
            if (filess[0].size > 1000 * 1024) {
                span1.innerText = 'Ảnh tối đa không được quá 1mb';
                $('#hinh3').attr("required", true);
                $('#hinh3').attr("value", null);
                return;
            }
            else {
                var picture1 = new FileReader();
                picture1.readAsDataURL(this.files[0]);
                picture1.addEventListener('load', function (event) {
                    document.getElementById('imguploadhinh2').setAttribute('src', event.target.result);
                });
            }
        }
        if (window.FormData !== undefined) {
            $('#hinh3').attr("required", false);
        }

        span1.innerText = '';
    });


})

