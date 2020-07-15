(function() {
    window.addEventListener('scroll', function() {
        debounce(function() {
            scrollHead();
        }(), 100);
    }, { passive: true });

    window.addEventListener('resize', function() {
        debounce(function() {
            scrollHead();
        }(), 100);
    });
    function scrollHead() {
        var title = document.querySelector('.pageTitle:not(.loose)');
        if (title) {
            var w = title.clientWidth;

            if (title.getBoundingClientRect().top > 0 || window.pageYOffset < title.clientHeight) {
                title.classList.remove('pageTitle--sticky');
                title.style.removeProperty('width');
            } else {
                title.classList.add('pageTitle--sticky');
                title.style.width = w + 'px';
            }
        }
    }
    scrollHead();
})();