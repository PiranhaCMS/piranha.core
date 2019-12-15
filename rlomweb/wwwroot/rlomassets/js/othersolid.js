/*======= Navbar Transparent to Solid =======*/
$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 10) {
            $('.navbar').addClass('solid');
        } else {
            $('.navbar').removeClass('solid');
        }
    });
});

$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 10) {
            $('#logo-image1').attr('src', '/rlomassets/img/rlom_logo.png');
            $(".navbar-nav, .nav-link").css("color", "white").hover(
                function () {
                    $(this).css({ color: '#e9bfff' }); // Mouseover
                },
                function () {
                    $(this).css({ color: 'white' }); //Mouseout
                });
            
        } else {
            $('#logo-image1').attr('src', '/rlomassets/img/rlom_logo1.png');
            $(".navbar-nav, .nav-link").css("color", "black").hover(
                function () {
                    $(this).css({ color: '#A703FE' }); // Mouseover
                },
                function () {
                    $(this).css({ color: 'black' }); //Mouseout
                });
        }
    });
});
//window.addEventListener('scroll', function () {
//    if (document.body.scrollTop > 10 || document.documentElement.scrollTop > 10) {
//        document.getElementById('logo-image1').src = '/rlomassets/img/rlom_logo.png';

//        var items = document.getElementsByClassName('navbar-nav nav-link');

//        for (i = 0; i < items.length; i++) {
            
//        }

//    } else {
//        document.getElementById('logo-image1').src = '/rlomassets/img/rlom_logo1.png';

//        var items = document.getElementsByClassName("navbar-nav nav-link");
//        for (i = 0; i < items.length; i++) {
//            items[i].setAttribute('style', 'color:black;');
//        }
//    }
//});
