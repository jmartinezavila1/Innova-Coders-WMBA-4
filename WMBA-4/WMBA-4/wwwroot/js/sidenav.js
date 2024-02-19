window.addEventListener('DOMContentLoaded', event => {

    // Toggle the side navigation
    const sidebarToggle = document.body.querySelector('#sidebarToggle');
    const layoutSidenavContent = document.getElementById('layoutSidenav_content');
    const sidebarCloseButton = document.getElementById('sidebar-close');
    const sidenavMenuHeading = document.querySelector('.sb-sidenav-close');

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', event => {
            event.preventDefault();
            const isSidebarToggled = document.body.classList.toggle('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', isSidebarToggled);

            // Toggle the visibility of sidenav menu heading
            if (isSidebarToggled) {
                sidenavMenuHeading.classList.remove('invisible');
            } else {
                sidenavMenuHeading.classList.add('invisible');
            }
        });
    }

    layoutSidenavContent.addEventListener('click', event => {
        if (document.body.classList.contains('sb-sidenav-toggled')) {
            document.body.classList.remove('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', false);
        }
    });

    if (sidebarCloseButton) {
        sidebarCloseButton.addEventListener('click', event => {
            event.preventDefault();
            const isSidebarToggled = document.body.classList.toggle('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', isSidebarToggled);

            // Toggle the visibility of sidenav menu heading
            if (isSidebarToggled) {
                sidenavMenuHeading.classList.remove('invisible');
            } else {
                sidenavMenuHeading.classList.add('invisible');
            }
        });
    }

    // Check screen width on page load
    if (window.innerWidth >= 992) {
        sidenavMenuHeading.classList.add('invisible');
    }

    // Check screen width on window resize
    window.addEventListener('resize', () => {
        if (window.innerWidth >= 992) {
            sidenavMenuHeading.classList.add('invisible');
        } else {
            sidenavMenuHeading.classList.remove('invisible');
        }
    });
});
