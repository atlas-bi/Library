(function () {
  const dragMoveOrder = function ($event) {
    const $element = $event.detail.el;
    const $x = $event.detail.x;
    const $y = $event.detail.y;

    if ($element.closest('.reorder')) {
      const $elementVmid = $y;
      const $elementHmid = $x;

      // Get all drag elements. Insert dragged at the right place.
      const $dragSource = $element
        .closest('.reorder')
        .querySelector('.drg.drag-source');
      $element
        .closest('.reorder')
        .querySelectorAll('.drg:not(.drag-source):not(.drag)')
        .forEach(function ($child) {
          const $childCords = getOffset($child);
          const $childTop = $childCords.top;
          const $childBottom = $childCords.top + $child.clientHeight;
          const $childLeft = $childCords.left;
          const $childRight = $childCords.left + $child.clientWidth;
          const $childIndex = Array.prototype.indexOf.call(
            $dragSource.parentNode.children,
            $child,
          );
          const $dragSourceIndex = Array.prototype.indexOf.call(
            $dragSource.parentNode.children,
            $dragSource,
          );

          // => right can only happen with h siblings
          if (
            $childLeft <= $elementHmid &&
            $elementVmid >= $childTop &&
            $elementVmid <= $childBottom &&
            $childIndex > $dragSourceIndex
          ) {
            $dragSource.parentNode.insertBefore($child, $dragSource);
          }

          // <= left can only happen with h siblings
          else if (
            $childRight >= $elementHmid &&
            $elementVmid >= $childTop &&
            $elementVmid <= $childBottom &&
            $childIndex < $dragSourceIndex
          ) {
            $dragSource.parentNode.insertBefore($dragSource, $child);
          }

          // ^ up
          if ($childTop > $elementVmid && $childIndex < $dragSourceIndex) {
            $dragSource.parentNode.insertBefore($dragSource, $child);
          }

          // ⌄ down
          else if (
            $childBottom < $elementVmid &&
            $childIndex > $dragSourceIndex
          ) {
            $dragSource.parentNode.insertBefore($child, $dragSource);
          }
        });
    }
  };

  document.addEventListener('dragMove', function (event) {
    event = event || window.event;
    if (event.detail === undefined || event.detail.el === undefined) {
      return !1;
    }

    debounce(dragMoveOrder(event), 250);
  });

  document.addEventListener('dragEnd', function (event) {
    event = event || window.event;
    if (event.detail === undefined || event.detail.el === undefined) {
      return !1;
    }

    if (event.detail.el.closest('.reorder')) {
      event.detail.el
        .closest('.reorder')
        .dispatchEvent(new CustomEvent('reorder', { bubbles: true }));
    }
  });
})();
