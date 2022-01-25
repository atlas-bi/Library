(function () {
  /*
      div.crsl
        div.crsl-itm
        div.crsl-itm
        div.crsl-ind
          span.crsl-dot
          span.crsl-dot
        a.crsl-prev
        a.crsl-next
    */
  var d = document;

  function l() {
    var crsl = document.getElementsByClassName('crsl'),
      x;

    for (x = 0; x < crsl.length; x++) {
      if (crsl[x].classList.contains('crsl-p')) {
        continue;
      }

      new c(crsl[x]);
    }
  }

  function c(b) {
    this.target = b;
    this.next = b.getElementsByClassName('crsl-next')[0];
    this.prev = b.getElementsByClassName('crsl-prev')[0];
    this.dots = b.getElementsByClassName('crsl-dot');
    this.slides = b.getElementsByClassName('crsl-itm');
    this.target.classList.add('crsl-p');
    this.next.addEventListener('click', this.showSlides.bind(this, 1), false);
    this.prev.addEventListener('click', this.showSlides.bind(this, -1), false);

    for (var q = 0; q < this.dots.length; q++) {
      var el = this.dots[q];
      el.addEventListener('click', this.showSlides.bind(this, 0), false);
    }
  }

  var k =
    d.requestAnimationFrame ||
    d.setImmediate ||
    function (b) {
      return setTimeout(b, 0);
    };

  c.prototype = {
    showSlides: function showSlides(n, b) {
      var i,
        slides = this.slides,
        aslide,
        dots = this.dots;
      k(function () {
        if (b.target.matches('.crsl-dot')) {
          n = b.target.getAttribute('slide');
        } else {
          for (i = 0; i < slides.length; i++) {
            if (slides[i].offsetHeight > 0 && slides[i].offsetWidth > 0) {
              aslide = slides[i];
              break;
            }
          }

          n += Array.prototype.indexOf.call(slides, aslide);
        }

        if (n >= slides.length) {
          n = 0;
        }

        if (n < 0) {
          n = slides.length - 1;
        }

        for (i = 0; i < slides.length; i++) {
          slides[i].style.display = 'none';
        }

        for (i = 0; i < dots.length; i++) {
          dots[i].classList.remove('active');
        }

        slides[n].style.display = 'block';
        dots[n].classList.add('active');
      });
    },
  };
  l();
  d.addEventListener('load-carousel', function () {
    l();
  });
})();
