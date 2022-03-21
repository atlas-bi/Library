(function () {
  var dragMoveOrder = function ($event) {
    var $el = $event.detail.el;
    var $x = $event.detail.x;
    var $y = $event.detail.y;

    if ($el.closest('.reorder')) {
      var $el_vmid = $y,
        $el_hmid = $x;

      // get all drag elements. Insert dragged at the right place.
      var $drag_source = $el
        .closest('.reorder')
        .querySelector('.drg.drag-source');
      $el
        .closest('.reorder')
        .querySelectorAll('.drg:not(.drag-source):not(.drag)')
        .forEach(function ($child) {
          var $child_cords = getOffset($child),
            $child_top = $child_cords.top,
            $child_bottom = $child_cords.top + $child.clientHeight,
            $child_left = $child_cords.left,
            $child_right = $child_cords.left + $child.clientWidth,
            $child_index = Array.prototype.indexOf.call(
              $drag_source.parentNode.children,
              $child,
            ),
            $drag_source_index = Array.prototype.indexOf.call(
              $drag_source.parentNode.children,
              $drag_source,
            );

          // => right can only happen with h siblings
          if (
            $child_left <= $el_hmid &&
            $el_vmid >= $child_top &&
            $el_vmid <= $child_bottom &&
            $child_index > $drag_source_index
          ) {
            $drag_source.parentNode.insertBefore($child, $drag_source);
          }

          // <= left can only happen with h siblings
          else if (
            $child_right >= $el_hmid &&
            $el_vmid >= $child_top &&
            $el_vmid <= $child_bottom &&
            $child_index < $drag_source_index
          ) {
            $drag_source.parentNode.insertBefore($drag_source, $child);
          }

          // ^ up
          if ($child_top > $el_vmid && $child_index < $drag_source_index) {
            $drag_source.parentNode.insertBefore($drag_source, $child);
          }

          // ⌄ down
          else if (
            $child_bottom < $el_vmid &&
            $child_index > $drag_source_index
          ) {
            $drag_source.parentNode.insertBefore($child, $drag_source);
          }
        });
    }
  };

  document.addEventListener('dragMove', function (e) {
    var $event = e || window.event;
    if ($event.detail == undefined || $event.detail.el == undefined) {
      return !1;
    }
    debounce(dragMoveOrder($event), 250);
  });

  document.addEventListener('dragEnd', function (e) {
    var $event = e || window.event;
    if ($event.detail == undefined || $event.detail.el == undefined) {
      return !1;
    }

    if ($event.detail.el.closest('.reorder')) {
      $event.detail.el
        .closest('.reorder')
        .dispatchEvent(new CustomEvent('reorder', { bubbles: true }));
    }
  });
})();
