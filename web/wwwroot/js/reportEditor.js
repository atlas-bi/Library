(function () {
  function updateId($imagelist) {
    var $hiddenInputs = $imagelist.querySelectorAll(
      'input[type="hidden"][name]:not(.drag)',
    );
    for (var x = 0; x < $hiddenInputs.length; x++) {
      $hiddenInputs[x].setAttribute(
        'name',
        $hiddenInputs[x]
          .getAttribute('name')
          .replace(/\[\d*?\]/, '[' + x + ']'),
      );
    }
  }

  // on reorder events, update id
  (
    document.querySelector('#images .images.reorder') || {
      addEventListener: () => {},
    }
  ).addEventListener('reorder', function (e) {
    updateId(e.target);
  });

  // adding new images
  (
    document.querySelector('.new-image input') || { addEventListener: () => {} }
  ).addEventListener('change', function (e) {
    e.preventDefault();

    // create a new reorder element
    var box = document.createElement('div');
    box.classList.add('box', 'p-0', 'm-3', 'drg');

    var input = document.createElement('input');
    input.type = 'hidden';
    input.setAttribute('name', 'Images[999].Imageid');

    var tools = document.createElement('div');
    tools.classList.add(
      'has-background-white-ter',
      'is-flex',
      'is-justify-content-space-between',
    );

    var drag = document.createElement('button');
    drag.setAttribute('type', 'button');
    drag.classList.add('button', 'is-light', 'drg-hdl');
    drag.innerHTML =
      '<span class="icon"><i class="fas fa-arrows-alt"></i></span>';

    // initial loaded as a spinner icon, the replaced w/ trash once we get a successful ajax response.
    var trash = document.createElement('button');
    trash.setAttribute('type', 'button');
    trash.classList.add('button', 'is-light', 'action-delete');
    trash.innerHTML =
      '<span class="icon"><i class="fas fa-circle-notch fa-spin"></i></span>';

    var figure = document.createElement('figure');
    figure.classList.add('image', 'is-256x256');

    var picture = document.createElement('picture');

    var img = document.createElement('img');
    img.setAttribute('src', window.URL.createObjectURL(e.target.files[0]));
    img.style.width = 'auto';
    img.style.maxWidth = '100%';
    img.style.maxHeight = '100%';

    box.appendChild(input);
    box.appendChild(tools);
    tools.appendChild(drag);
    tools.appendChild(trash);
    box.appendChild(figure);
    figure.appendChild(picture);
    picture.appendChild(img);

    var $this = document
      .querySelector('.images.reorder .new-image')
      .closest('.box');
    $this.parentNode.insertBefore(box, $this);

    // upload image

    var data = new FormData();
    data.append('File', e.target.files[0]);

    var q = new XMLHttpRequest();
    q.open(
      'post',
      '/reports/edit/?handler=AddImage&Id=' + getUrlVars().id,
      true,
    );
    q.withCredentials = true;
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send(data);
    q.onload = function () {
      if (q.responseText == 'error') {
        trash.innerHTML =
          '<span class="icon has-text-danger"><i class="fas fa-exclamation-triangle"></i></span>';
      } else {
        trash.innerHTML =
          '<span class="icon"><i class="fas fa-trash"></i></span>';
        input.value = q.responseText;
        trash.addEventListener('click', function (e) {
          e.preventDefault();
          box.parentNode.removeChild(box);
        });
      }
    };

    // trigger a reorder to get the id
    updateId(document.querySelector('.images.reorder'));
  });

  // add delete action to existing images
  (document.querySelectorAll('.images button.action-delete') || []).forEach(
    ($el) => {
      $el.addEventListener('click', function (e) {
        e.preventDefault();
        var $box = $el.closest('.box');
        $box.parentNode.removeChild($box);
      });
    },
  );
})();
