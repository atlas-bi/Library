(function () {
  function updateId($imagelist) {
    const $hiddenInputs = $imagelist.querySelectorAll(
      'input[type="hidden"][name]:not(.drag)',
    );
    for (let x = 0; x < $hiddenInputs.length; x++) {
      $hiddenInputs[x].setAttribute(
        'name',
        $hiddenInputs[x].getAttribute('name').replace(/\[\d*?]/, '[' + x + ']'),
      );
    }
  }

  // On reorder events, update id
  (
    document.querySelector('#images .images.reorder') || {
      addEventListener() {},
    }
  ).addEventListener('reorder', function (event) {
    updateId(event.target);
  });

  // Adding new images
  (
    document.querySelector('.new-image input') || { addEventListener() {} }
  ).addEventListener('change', function (event) {
    event.preventDefault();

    // Create a new reorder element
    const box = document.createElement('div');
    box.classList.add('box', 'p-0', 'm-3', 'drg');

    const input = document.createElement('input');
    input.type = 'hidden';
    input.setAttribute('name', 'Images[999].Imageid');

    const tools = document.createElement('div');
    tools.classList.add(
      'has-background-white-ter',
      'is-flex',
      'is-justify-content-space-between',
    );

    const drag = document.createElement('button');
    drag.setAttribute('type', 'button');
    drag.classList.add('button', 'is-light', 'drg-hdl');
    drag.innerHTML =
      '<span class="icon"><i class="fas fa-up-down-left-right"></i></span>';

    // Initial loaded as a spinner icon, the replaced w/ trash once we get a successful ajax response.
    const trash = document.createElement('button');
    trash.setAttribute('type', 'button');
    trash.classList.add('button', 'is-light', 'action-delete');
    trash.innerHTML =
      '<span class="icon"><i class="fas fa-circle-notch fa-spin"></i></span>';

    const figure = document.createElement('figure');
    figure.classList.add('image', 'is-256x256');

    const picture = document.createElement('picture');

    const img = document.createElement('img');
    img.setAttribute('src', window.URL.createObjectURL(event.target.files[0]));
    img.style.width = 'auto';
    img.style.maxWidth = '100%';
    img.style.maxHeight = '100%';

    box.append(input);
    box.append(tools);
    tools.append(drag);
    tools.append(trash);
    box.append(figure);
    figure.append(picture);
    picture.append(img);

    const $this = document
      .querySelector('.images.reorder .new-image')
      .closest('.box');
    $this.parentNode.insertBefore(box, $this);

    // Upload image

    const data = new FormData();
    data.append('File', event.target.files[0]);

    const q = new XMLHttpRequest();
    q.open(
      'post',
      '/reports/edit/?handler=AddImage&Id=' + getUrlVars().id,
      true,
    );
    q.withCredentials = true;
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send(data);
    q.addEventListener('load', function () {
      if (q.responseText === 'error') {
        trash.innerHTML =
          '<span class="icon has-text-danger"><i class="fas fa-exclamation-triangle"></i></span>';
      } else {
        trash.innerHTML =
          '<span class="icon"><i class="fas fa-trash"></i></span>';
        input.value = q.responseText;
        trash.addEventListener('click', function (event) {
          event.preventDefault();
          box.remove();
        });
      }
    });

    // Trigger a reorder to get the id
    updateId(document.querySelector('.images.reorder'));
  });

  // Add delete action to existing images
  (document.querySelectorAll('.images button.action-delete') || []).forEach(
    ($element) => {
      $element.addEventListener('click', function (event) {
        event.preventDefault();
        const $box = $element.closest('.box');
        $box.remove();
      });
    },
  );
})();
