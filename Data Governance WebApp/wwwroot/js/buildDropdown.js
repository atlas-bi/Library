/*
    Atlas of Information Management business intelligence library and documentation database.
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
(function(){
	function buildDropdown(b){
		console.log('building input')
		//var inputs = d.querySelectorAll('[type="dynamic-dropdown"]')
		var s,o,d=document;

		/* create elements */
		this.ddCntr = d.createElement('div');
		this.ddWrp = d.createElement('div');
		this.ddHdn = d.createElement(b.tagName);
		this.type = b.tagName;
		this.ddPhdr = d.createElement('div');
		this.ddInpt = d.createElement('div');
		this.ddVsbl = d.createElement('input');
		this.ddRslts = d.createElement('div');

		/* add classes */
		this.ddCntr.classList.add('dd-cntr');
		this.ddWrp.classList.add('dd-wrp');
		this.ddHdn.classList.add('dd-hdn');
		this.ddPhdr.classList.add('dd-phdr');
		this.ddInpt.classList.add('dd-inpt');
		this.ddVsbl.classList.add('dd-vsbl');
		this.ddRslts.classList.add('dd-rslts');

		/* add attributes */
		this.ddHdn.setAttribute('tabindex','-1');
		this.ddHdn.setAttribute('multiple','true');
		if(b.hasAttribute('id')) this.ddHdn.setAttribute('id',b.getAttribute('id'));
		if(b.hasAttribute('name')) this.ddHdn.setAttribute('name',b.getAttribute('name'));
		if(b.hasAttribute('value')) this.ddHdn.setAttribute('value',b.getAttribute('value'));

		this.ddVsbl.setAttribute('autocomplete','off');
		if(b.hasAttribute('search-area')) this.ddVsbl.setAttribute('search-area',b.getAttribute('search-area'));
		this.searchArea = b.getAttribute('search-area');
		if(b.hasAttribute('visible-value')) this.ddVsbl.setAttribute('value',b.getAttribute('visible-value'));
		if(b.hasAttribute('required')) this.ddVsbl.setAttribute('required',b.getAttribute('required'));
		if(b.hasAttribute('method')) this.ddVsbl.setAttribute('method',b.getAttribute('method'));
		this.method = b.getAttribute('method');

		if(!b.hasAttribute('value')) this.ddPhdr.innerHTML = b.hasAttribute('placeholder') ? b.getAttribute('placeholder') : 'type to search..';
		this.ddPhdr.setAttribute('data-placeholder',b.hasAttribute('placeholder') ? b.getAttribute('placeholder') : 'type to search..');
		if(b.hasAttribute('data-head')) this.ddWrp.setAttribute('data-head',b.getAttribute('data-head'));

		/* build html */
		this.ddCntr.appendChild(this.ddWrp);
		this.ddWrp.appendChild(this.ddInpt);
		this.ddWrp.appendChild(this.ddPhdr);
		this.ddWrp.appendChild(this.ddHdn);
		this.ddInpt.appendChild(this.ddRslts);
		this.ddInpt.appendChild(this.ddVsbl);

		s = b.getElementsByTagName('option');

		for(o=0;o<s.length;o++){

			this.ddHdn.appendChild(s[o]);
			var ddItm = document.createElement('div');
			ddItm.classList.add('dd-itm');
			//ddItm.innerHTML = s[o].innerHTML;

			//if(s[o].classList.contains('grp')){
			//	ddItm.classList.add('grp');
		//	}

		//	this.ddWrp.appendChild(ddItm);
		}

		/* replace original */
		b.parentNode.replaceChild(this.ddCntr,b);

		this.flLoad();
		this.stW();

		this.ddVsbl.addEventListener("input", this.search.bind(this));
		this.ddVsbl.addEventListener("input", this.stW.bind(this));
		this.ddVsbl.addEventListener('focus', this.search.bind(this));
		this.ddVsbl.addEventListener('keydown', this.keydn.bind(this));

		this.ddCntr.addEventListener('click', this.click.bind(this));
		this.ddCntr.addEventListener('focusout', this.unfcs.bind(this));
		this.ddRslts.addEventListener('mouseover', this.rhov.bind(this));
	}

	var k = d.requestAnimationFrame || d.setImmediate || function(b) {
        return setTimeout(b, 0);
    };

	buildDropdown.prototype = {
		keydn: function(b) {
			var key = b.keyCode,
				e = b,
				t = this.type,
				h = this.ddHdn,
				i = this.ddVsbl,
				r = this.ddRslts,
				p = this.ddPhdr,
				o,el;

			k(function(){
				if(key == 8 && i.value.length == 0 && t == 'SELECT' && i.previousElementSibling){ // backspace

					o = h.querySelector('option:contains(' + i.previousElementSibling.innerHTML + ')');
					el = r.querySelector('.dd-rslt.hidden[value="'+o.getAttribute('value')+'"]');
					o.parentNode.removeChild(o);
					if(el) el.classList.remove('hidden');


					if(r.querySelectorAll('.dd-rslt:not(.hidden.disabled)').length > 0){
						r.getElementsByClassName('disabled')[0].parentElement.removeChild(r.getElementsByClassName('disabled')[0]);
					}

					i.parentElement.removeChild(i.previousElementSibling);

				} else if(key == 37 && t == 'SELECT' && i.previousElementSibling && i.selectionStart == 0) { // left
					p.innerHTML = p.getAttribute('data-placeholder');
					i.parentElement.insertBefore(i,i.previousElementSibling);
					i.focus();

				} else if (key == 39 && t == 'SELECT' && i.nextElementSibling && i.selectionStart == i.value.length) { // right
					i.parentElement.insertAfter(i,i.nextElementSibling);
					i.focus();

				} else if (key == 46 && t == 'SELECT' && i.nextElementSibling && i.selectionStart == i.value.length){ // delete

					o = h.querySelector('option:contains(' + i.nextElementSibling.innerHTML + ')');
					el = r.querySelector('.dd-rslt.hidden[value="'+o.getAttribute('value')+'"]');
					o.parentNode.removeChild(o);
					if(el) el.classList.remove('hidden');


					if(r.querySelectorAll('.dd-rslt:not(.hidden.disabled)').length > 0){
						r.getElementsByClassName('disabled')[0].parentElement.removeChild(r.getElementsByClassName('disabled')[0]);
					}

					i.parentElement.removeChild(i.nextElementSibling);

				} else if (key == 13 || key == 3){ // enter or enter mac

					e.preventDefault();
					//addItem(input, 'exit');
					i.focus();

				} else if (key == 9){ // tab
					//e.preventDefault();

				} else if (key == 38 ){ // up arrow
					e.preventDefault();

					el = r.getElementsByClassName('active')[0];

					if(el){
						o = Array.prototype.indexOf.call(r.children, el);
						el.classList.remove('active');

						if(o == 0){
							r.children[-1].classList.add('active');
						} else {
							el.previousElementSibling.classList.add('active');
						}
					} else {
						r.children[-1].classList.add('active');
					}

				} else if (e.keyCode == 40){ // down arrow
					e.preventDefault();

					el = r.getElementsByClassName('active')[0];
					if(el){
						o = Array.prototype.indexOf.call(r.children, el);
						el.classList.remove('active');

						if(o == 0){
							r.children[0].classList.add('active');
						} else {
							el.nextElementSibling.classList.add('active');
						}
					} else {
						r.children[0].classList.add('active');
					}
				}
			});
		},
		rhov: function(b) {
			var r = this.ddRslts,
				l = b.target.closest('.dd-rslt'),
				x,e;

			k(function(){
				e = r.getElementsByClassName('active');
				for(x=0;x<e.length;x++){
					e[x].classList.remove('active');
				}
				l.classList.add('active');
			});

		},
		flLoad: function(b) {
			var i = this.ddVsbl,
				c = this.ddCntr,
				rs = this.ddRslts,
				h = this.ddHdn,
				p = this.ddPhdr,
				q,
				sa = this.searchArea,
				url = "/Search?handler=ValueList&s=" + sa,
				data,result = '',i,el, active = [];

			k(function(){

				// get values of items that are alreay selected for select box.
				[].forEach.call(h.getElementsByTagName('option'),function(e){
					active.push(e.getAttribute('value'));
				});

				if(cache.exists(url)) {
					load(cache.get(url));
				} else {

					q = new XMLHttpRequest();
		            q.open('post', url , true);
		            q.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
		            q.setRequestHeader('X-Requested-With','XMLHttpRequest');
		            q.send();

		            q.onload = function() {

				    	var l = q.responseText;
						load(l);

						var ccHeader = q.getResponseHeader('Cache-Control') != null ? (q.getResponseHeader('Cache-Control').match(/\d+/)||[null])[0] : null;

						if(ccHeader) {
				    		cache.set(url,l,ccHeader);
				    	}
					};
				}

				function load(data) {
					data = JSON.parse(data);
					if(data.length === 0){
						rs.innerHTML = '<div class="dd-rslt disabled">No matches found</div>';
					} else {
						for(i=0;i<data.length;i++){
							el = data[i];
							var hiddenClass="";
							if(active.indexOf(el.ObjectId) !== -1){hiddenClass = 'hidden';}
								var id = el.ObjectId || el.Description;
				 				result += '<div class="dd-rslt ' + hiddenClass + '" value="'+ id + '">' + el.Name + '</div>';

					 		if(i == data.length-1){
					 			rs.innerHTML = result;
					 		}
					 	}

				 		if(rs.querySelectorAll('.dd-rslt:not(.hidden)').length < 1){
				 			rs.innerHTML += '<div class="dd-rslt disabled">No matches found</div>';
				 		}

					}
				}

			});
		},
		unfcs: function(b) {
			var i = this.ddVsbl,
				c = this.ddCntr,
				rs = this.ddRslts,
				p = this.ddPhdr;

			k(function(){
				c.classList.remove('show');
				rs.style.removeProperty('display');
				if(i.value.length < 1){
					p.style.removeProperty('display');
				}
			});
		},
		click: function(b){
			var i = this.ddVsbl;
			k(function(){
				i.focus();
			});
		},
		stW: function(b) {
			var v = this.ddVsbl,
				c = this.ddCntr,
				d = document,
				q;
			k(function(){
				q = d.createElement('div');
				q.style.visibility = 'hidden';
				q.style.width = 'auto';
				q.style.zIndex = '-99';
				q.style.whiteSpace = 'nowrap';
				q.style.position = 'absolute';
				q.style.fontSize = v.style.fontSize;
				q.innerHTML = v.value;
				c.appendChild(q);
				v.style.width = Math.max(q.clientWidth+4,4) + 'px';
				q.parentElement.removeChild(q);
			});
		},
		search: function(b) {
			var q,
        		el = this.ddVsbl,
        		rs = this.ddRslts,
        		hi = this.ddHdn,
        		c = this.ddWrp,
        		o,
        		r,
        		x,
        		m = this.method,
        		sa = this.searchArea,
        		p = this.ddPhdr,
        		data,url;

        	k(function(){
        		hi.value = '';
				p.style.display = 'none';

				if(m == 'fullList'){
					c.classList.add('show');
					rs.style.display = 'block';
					/*r = rs.getElementsByClassName('dd-rslt');
					for(x=0;x<r.length;x++){
					   if(el.value.toUpperCase() == r[x].innerHTML.toUpperCase()){
					   		r[x].style.removeProperty('display');
					    } else {
					   		r[x].style.display = 'none';
						}
					}*/
				}

				else {
        			o = hi.getElementsByTagName('option');
					var values = [].forEach.call(o, function(el) {
					    return el.value;
					});
					if (el.value.length > 0){

						data = {s:el.value,e:(typeof values !== 'undefined') ? values.toString() : ''};

					    url = Object.keys(data).map(function (k) {
					      return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
					    }).join('&');

					    if(typeof q !== 'undefined' && q !== null) q.abort();

						q = new XMLHttpRequest();
			            q.open('post', "/Search?handler=" + sa + "&" + url, true);
			            q.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
			            q.setRequestHeader('X-Requested-With','XMLHttpRequest');
			            q.send();

			            q.onload = function() {

							try{
								data = JSON.parse(q.responseText);
							} catch(e){
								return !1;
							}

							if(data.length === 0){
								rs.innerHTML = '<div class="dd-rslt disabled">No matches found</div>';
							} else {
								var result = '';
								for(x=0;x< data.length;x++)
								{
									var id = data[x].ObjectId || data[i].Description;
									var type;
									type = data[x].Type == 'g' ? 'group' : '';
									if(x==0){
										result += '<div class="dd-rslt active '+type+'" value="'+ id + '">'+data[x].Name + '</div>';
									} else {
							 			result += '<div class="dd-rslt'+type+'" value="'+ id + '">'+data[x].Name + '</div>';
							 		}
							 		if(x == data.length-1){
							 			rs.innerHTML = result;
							 		}
								}
							}
							c.classList.add('show');
							rs.style.display = 'block';

							try{
								SimpleScrollbar.initAll();
							}
							catch(e){
								console.log(e);
							}
						};

					} else {
						rs.style.removeProperty('display');
						c.classList.remove('show');
						hi.value = '';
						p.style.removeProperty('display');
					}
				}
        	});
		}
	};
})();