(function(){

  // Dropdowns
  function closeDropdowns() {
    (document.querySelectorAll('.dropdown:not(.is-hoverable)') || []).forEach(function ($el) {
    $el.classList.remove('is-active');
    });
  }
  document.addEventListener('click', function ($e) {
      if (!$e.target.closest('.dropdown.is-active')){
        closeDropdowns();
      }

    });

    // Close dropdowns if ESC pressed
    document.addEventListener('keydown', function (event) {
        var e = event || window.event;
        if (e.keyCode === 27) {
        closeDropdowns();
        }
    });
    document.addEventListener('click', ($e) => {
        if($e.target.closest('.dropdown:not(.is-hoverable)')){
            var $el =$e.target.closest('.dropdown:not(.is-hoverable)');
            $e.stopPropagation();

            if($el.classList.contains('is-active')){
                $el.classList.remove('is-active');
            } else {
                $el.classList.add('is-active');
            }
        }

    })

})();
