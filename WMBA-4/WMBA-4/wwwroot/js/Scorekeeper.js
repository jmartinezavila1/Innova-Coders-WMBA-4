$(document).ready(function () {
    function checkVisibility() {
        // 스크린 폭이 990px 이하인 경우에만 실행
        if ($(window).width() <= 990) {
            $('nav.sb-topnav, footer').hide();
            // main-padding 클래스의 padding-top을 0으로 설정
            $('.main-padding').css('padding-top', '0');
        } else {
            $('nav.sb-topnav, footer').show();
            // 스크린 폭이 990px 초과인 경우 원래대로 설정
            $('.main-padding').css('padding-top', '2rem');
        }
    }

    // 페이지 로드시 가시성 확인
    checkVisibility();

    // 창 크기 변경시 가시성 확인
    $(window).resize(function () {
        checkVisibility();
    });
});

