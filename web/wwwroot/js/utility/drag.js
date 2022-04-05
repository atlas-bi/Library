(function () {
  const d = document;
  let d1 = 0;
  let d2 = 0;
  let dragElement;
  let dragSElement;
  d.addEventListener(
    'mousedown',
    function (event) {
      if (event.target.closest('.drg-hdl')) {
        dragSElement = event.target.closest('.drg');
      } else {
        return;
      }

      dragElement = dragSElement.cloneNode(true);
      dragSElement.classList.add('drag-source');
      dragElement.classList.add('drag');
      dragElement.style.width = dragSElement.offsetWidth + 1 + 'px';

      // Margin is not included in x and y.
      const style = getComputedStyle(dragSElement);

      d1 =
        event.clientY -
        getOffset(dragSElement).top +
        parseInt(style.marginTop, 10);
      d2 =
        event.clientX -
        getOffset(dragSElement).left +
        parseInt(style.marginLeft, 10);
      dragSElement.parentElement.style.position = 'relative';
      dragSElement.parentElement.append(dragElement);
      dragElement.style.top = event.clientY - d1 + 'px';
      dragElement.style.left = event.clientX - d2 + 'px';

      dragMouseDown(event);
    },
    false,
  );

  const dragMouseDown = function (event) {
    event = event || window.event;
    event.preventDefault();
    d.addEventListener('mouseup', dragMouseUp);
    d.addEventListener('mousemove', dragMouseMove);
  };

  const dragMouseMove = function (event) {
    event = event || window.event;
    event.preventDefault();
    dragElement.style.top = event.clientY - d1 + 'px';
    dragElement.style.left = event.clientX - d2 + 'px';
    dragElement.dispatchEvent(
      new CustomEvent('dragMove', {
        cancelable: true,
        bubbles: true,
        detail: {
          el: dragElement,
          x: event.clientX,
          y: event.clientY,
        },
      }),
    );
  };

  const dragMouseUp = function (event) {
    dragSElement.parentElement.replaceChild(dragElement, dragSElement);
    dragElement.classList.remove('drag');
    dragElement.dispatchEvent(
      new CustomEvent('dragEnd', {
        cancelable: true,
        bubbles: true,
        detail: {
          el: dragElement,
          x: event.clientX,
          y: event.clientY,
        },
      }),
    );

    dragElement.style.width = '';
    dragElement.style.top = '';
    dragElement.style.left = '';
    d.removeEventListener('mouseup', dragMouseUp);
    d.removeEventListener('mousemove', dragMouseMove);
  };
})();
