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
      .forEach(($child) => {
        var $child_cords = getOffset($child),
          $child_top = $child_cords.top,
          $child_bottom = $child_cords.top + $child.clientHeight,
          $child_left = $child_cords.left,
          $child_hmid = $child_cords.left + $child.clientWidth / 2,
          $child_right = $child_cords.left + $child.clientWidth;

        // ordering on same row
        if ($el_vmid >= $child_top && $el_vmid <= $child_bottom) {
          // dragging to left
          if ($el_hmid <= $child_hmid && $el_hmid >= $child_left) {
            $drag_source.parentNode.insertBefore($drag_source, $child);
          }
          // dragging to right
          else if ($el_hmid >= $child_hmid && $el_hmid <= $child_right) {
            $drag_source.parentNode.insertBefore($child, $drag_source);
          }
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
      .dispatchEvent(new CustomEvent('reorder'));
  }
});
