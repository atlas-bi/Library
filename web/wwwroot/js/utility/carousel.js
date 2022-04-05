(function () {
  /*
    Example usage
    div.carousel
      div.carousel-itm
      div.carousel-itm
      div.carousel-ind
        span.carousel-dot
        span.carousel-dot
      a.carousel-prev
      a.carousel-next
    */
  const d = document;

  function l() {
    const carousel = document.querySelectorAll('.carousel');
    let x;

    for (x = 0; x < carousel.length; x++) {
      if (carousel[x].classList.contains('carousel-p')) {
        continue;
      }

      // eslint-disable-next-line no-new
      new CarBuilder(carousel[x]);
    }
  }

  function CarBuilder(b) {
    this.target = b;
    this.next = b.querySelector('.carousel-next');
    this.prev = b.querySelector('.carousel-prev');
    this.dots = b.querySelectorAll('.carousel-dot');
    this.slides = b.querySelectorAll('.carousel-itm');
    this.target.classList.add('carousel-p');
    this.next.addEventListener('click', this.showSlides.bind(this, 1), false);
    this.prev.addEventListener('click', this.showSlides.bind(this, -1), false);

    for (let q = 0; q < this.dots.length; q++) {
      const element = this.dots[q];
      element.addEventListener('click', this.showSlides.bind(this, 0), false);
    }
  }

  const k =
    d.requestAnimationFrame ||
    d.setImmediate ||
    function (b) {
      return setTimeout(b, 0);
    };

  CarBuilder.prototype = {
    showSlides(n, b) {
      let i;
      const slides = this.slides;
      let aslide;
      const dots = this.dots;
      k(function () {
        if (b.target.matches('.carousel-dot')) {
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
