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

(function () {
  var d = document,
    q;

  function insertAfter(newElement, referenceElement) {
    referenceElement.parentNode.insertBefore(
      newElement,
      referenceElement.nextSibling
    );
  }

  // when everything looses focus close all drops
  window.addEventListener("blur", function (event) {
    if (document.activeElement) {
      try {
        document.activeElement.blur();
      } catch (e) {}
    }
    var el = document.querySelector(":focus");
    if (el) {
      try {
        el.blur();
      } catch (e) {}
    }
  });

  function load(el) {
    if (typeof el !== "undefined") {
      new build(el);
    } else {
      var l = d.querySelectorAll('[type="dynamic-dropdown"]');
      for (var e = 0; e < l.length; e++) {
        new build(l[e]);
      }
    }
  }

  /*
    	div.dd-ctnr
    		div.dd-wrp
    			input.dd-hdn (or select.dd-hdn > option.dd-optn)
    			div.dd-itm (item selected)
    			div.dd-phdr
    			div.dd-inpt
    				input.dd-vsbl
    				div.dd-rslts
    					div.dd-rslt
    */

  function build(b) {
    //var inputs = d.querySelectorAll('[type="dynamic-dropdown"]')
    var s, o;

    /* create elements */
    this.ddCntr = d.createElement("div");
    this.ddWrp = d.createElement("div");
    this.type = b.tagName;
    this.ddHdn = d.createElement(this.type);
    this.ddPhdr = d.createElement("div");
    this.ddInpt = d.createElement("div");
    this.ddVsbl = d.createElement("input");
    this.ddRslts = d.createElement("div");
    //this.ddRsltsLnr = d.createElement('div');

    /* add classes */
    this.ddCntr.classList.add("dd-cntr");
    this.ddWrp.classList.add("dd-wrp");
    if (b.classList.contains("slim")) this.ddWrp.classList.add("slim");
    this.ddHdn.classList.add("dd-hdn");
    this.ddPhdr.classList.add("dd-phdr");
    this.ddInpt.classList.add("dd-inpt");
    this.ddVsbl.classList.add("dd-vsbl");
    this.ddRslts.classList.add("dd-rslts");
    this.ddRslts.setAttribute("ss-container", "true");
    //this.ddRsltsLnr.classList.add('dd-rsltsLnr');

    /* add attributes */
    this.ddCntr.setAttribute("tabindex", "0");
    this.ddHdn.setAttribute("tabindex", "-1");
    this.ddHdn.setAttribute("multiple", "true");
    if (b.hasAttribute("id"))
      this.ddHdn.setAttribute("id", b.getAttribute("id"));
    if (b.hasAttribute("name"))
      this.ddHdn.setAttribute("name", b.getAttribute("name"));
    if (b.hasAttribute("value"))
      this.ddHdn.setAttribute("value", b.getAttribute("value"));

    this.ddVsbl.setAttribute("autocomplete", "off");
    if (b.hasAttribute("search-area"))
      this.ddVsbl.setAttribute("search-area", b.getAttribute("search-area"));
    this.searchArea = b.getAttribute("search-area");
    if (b.hasAttribute("visible-value"))
      this.ddVsbl.setAttribute("value", b.getAttribute("visible-value"));
    if (b.hasAttribute("required"))
      this.ddVsbl.setAttribute("required", b.getAttribute("required"));
    if (b.hasAttribute("method"))
      this.ddVsbl.setAttribute("method", b.getAttribute("method"));
    this.method = b.getAttribute("method");

    if (!b.hasAttribute("value"))
      this.ddPhdr.innerHTML = b.hasAttribute("placeholder")
        ? b.getAttribute("placeholder")
        : "type to search..";
    this.ddPhdr.setAttribute(
      "data-placeholder",
      b.hasAttribute("placeholder")
        ? b.getAttribute("placeholder")
        : "type to search.."
    );
    if (b.hasAttribute("data-head"))
      this.ddWrp.setAttribute("data-head", b.getAttribute("data-head"));

    /* build html */
    this.ddCntr.appendChild(this.ddWrp);
    this.ddWrp.appendChild(this.ddInpt);

    this.ddWrp.appendChild(this.ddHdn);
    this.ddInpt.appendChild(this.ddRslts);

    //this.ddRslts.appendChild(this.ddRsltsLnr)
    this.ddInpt.appendChild(this.ddVsbl);
    this.ddInpt.appendChild(this.ddPhdr);
    s = Array.prototype.slice.call(b.getElementsByTagName("option"));

    for (o = 0; o < s.length; o++) {
      this.ddHdn.appendChild(s[o]);
      var ddItm = document.createElement("div");
      ddItm.classList.add("dd-itm");
      if (s[o].classList.contains("group")) {
        ddItm.classList.add("group");
      }

      var userDrop = "";
      if (
        b.hasAttribute("search-area") &&
        b.getAttribute("search-area") == "UserSearchMail"
      ) {
        var split = s[o].classList.contains("group")
          ? '<div class="mlbx-usrDrpItm">Expand Group</div>'
          : "";
        userDrop =
          '<div class="mlbx-usrDrp"><i class="fas fa-angle-down fa-sm"></i><div class="mlbx-usrDrpCtr"><div class="mlbx-usrDrpItm mail-new-message" data-from="Analytics" data-fromid="45781">New Mail</div>' +
          split +
          '<div class="separator"></div><div class="mlbx-usrDrpItm"><a class="ajax" href="/users?id=45781">Open Profile</a></div></div></div>';
      }

      ddItm.innerHTML = s[o].innerHTML + userDrop;

      this.ddWrp.insertBefore(ddItm, this.ddWrp.firstChild);
    }

    /* replace original */
    b.parentNode.replaceChild(this.ddCntr, b);

    this.flLoad();
    this.stW();

    this.fcsTmr = null;

    this.ddVsbl.addEventListener("input", this.search.bind(this));
    this.ddVsbl.addEventListener("input", this.stW.bind(this));
    this.ddVsbl.addEventListener("setWidth", this.stW.bind(this));
    // also set width if a tab was changed
    document.addEventListener("tab-opened", this.stW.bind(this));
    this.ddVsbl.addEventListener("focus", this.search.bind(this));
    this.ddVsbl.addEventListener("keydown", this.keydn.bind(this));

    this.ddCntr.addEventListener("click", this.click.bind(this), true);
    this.ddCntr.addEventListener("focusout", this.unfcs.bind(this));
    this.ddCntr.addEventListener("blur", this.unfcs.bind(this));
    this.ddRslts.addEventListener("mouseover", this.rhov.bind(this));
  }

  var k =
    d.requestAnimationFrame ||
    d.setImmediate ||
    function (b) {
      return setTimeout(b, 0);
    };

  build.prototype = {
    keydn: function (b) {
      var key = b.keyCode,
        e = b,
        t = this.type,
        h = this.ddHdn,
        i = this.ddVsbl,
        r = this.ddRslts,
        p = this.ddPhdr,
        dd = this.ddWrp,
        ipt = this.ddInpt,
        o,
        el,
        newEl;

      if (
        key == 8 &&
        i.value.length == 0 &&
        t == "SELECT" &&
        ipt.previousElementSibling
      ) {
        // backspace
        k(function () {
          o = h.getElementsByTagName("option");
          for (var g = 0; g < o.length; g++) {
            if (
              o[g].childNodes[0].nodeValue ==
              ipt.previousElementSibling.childNodes[0].nodeValue
            ) {
              o = o[g];
              el = r.querySelector(
                '.dd-rslt.hidden[value="' + o.getAttribute("value") + '"]'
              );
              o.parentNode.removeChild(o);
              if (el) el.classList.remove("hidden");
              break;
            }
          }

          //if(r.querySelectorAll('.dd-rslt:not(.hidden):not(.disabled)').length > 0){
          //	r.getElementsByClassName('disabled')[0].parentElement.removeChild(r.getElementsByClassName('disabled')[0]);
          //}

          ipt.parentElement.removeChild(ipt.previousElementSibling);
        });
      } else if (
        key == 37 &&
        t == "SELECT" &&
        ipt.previousElementSibling &&
        ipt.previousElementSibling.matches(".dd-itm") &&
        i.selectionStart == 0
      ) {
        // left arrow
        k(function () {
          p.innerHTML = p.getAttribute("data-placeholder");
          ipt.parentElement.insertBefore(ipt, ipt.previousElementSibling);
          setTimeout(function () {
            i.focus();
          }, 0);
        });
      } else if (
        key == 39 &&
        t == "SELECT" &&
        ipt.nextElementSibling &&
        ipt.nextElementSibling.matches(".dd-itm") &&
        i.selectionStart == i.value.length
      ) {
        // right arrow
        k(function () {
          insertAfter(ipt, ipt.nextElementSibling);
          setTimeout(function () {
            i.focus();
          }, 0);
        });
      } else if (
        key == 46 &&
        t == "SELECT" &&
        ipt.nextElementSibling &&
        ipt.nextElementSibling.matches(".dd-itm") &&
        i.selectionStart == i.value.length
      ) {
        // delete
        k(function () {
          o = h.getElementsByTagName("option");

          for (var g = 0; g < o.length; g++) {
            if (o[g].innerHtml == ipt.nextElementSibling.innerHTML) {
              o = o[g];
              el = r.querySelector(
                '.dd-rslt.hidden[value="' + o.getAttribute("value") + '"]'
              );
              o.parentNode.removeChild(o);
              if (el) el.classList.remove("hidden");
              break;
            }
          }

          if (
            r.querySelectorAll(".dd-rslt:not(.hidden):not(.disabled)").length >
              0 &&
            r.getElementsByClassName("disabled") !== null
          ) {
            r.getElementsByClassName("disabled")[0].parentElement.removeChild(
              r.getElementsByClassName("disabled")[0]
            );
          }

          ipt.parentElement.removeChild(ipt.nextElementSibling);
        });
      } else if (key == 13 || key == 3) {
        // enter or enter mac
        b.preventDefault();
        k(function () {
          if (r.getElementsByClassName("active").length > 0) {
            r.getElementsByClassName("active")[0].click();
            dd.classList.remove("dd-wrp-show");
            r.style.removeProperty("display");
          }
          setTimeout(function () {
            i.focus();
          }, 0);
        });
      } else if (key == 9) {
        // tab
        e.preventDefault();
        if (r.getElementsByClassName("active").length > 0) {
          r.getElementsByClassName("active")[0].click();
        } else {
          dd.classList.add("dd-wrp-invalid");
        }

        var isHidden = function (el) {
          var style = window.getComputedStyle(el);
          return style.display === "none" || style.visibility === "hidden";
        };

        var inputs = document.querySelectorAll(
          'input, select, div[contenteditable="true"]'
        );
        var vInputs = [];

        for (var x = 0; x < inputs.length; x++) {
          if (isHidden(inputs[x]) == false) {
            vInputs.push(inputs[x]);
          }
        }

        if (e.shiftKey) {
          if (Array.prototype.indexOf.call(vInputs, i) == 0) {
            setTimeout(function () {
              vInputs[vInputs.length - 1].focus();
            }, 0);
          } else {
            setTimeout(function () {
              vInputs[Array.prototype.indexOf.call(vInputs, i) - 1].focus();
            }, 0);
          }
        } else {
          if (Array.prototype.indexOf.call(vInputs, i) == vInputs.length - 1) {
            setTimeout(function () {
              vInputs[0].focus();
            }, 0);
          } else {
            setTimeout(function () {
              vInputs[Array.prototype.indexOf.call(vInputs, i) + 1].focus();
            }, 0);
          }
        }
      } else if (key == 38) {
        // up arrow
        b.preventDefault();
        k(function () {
          el = r.getElementsByClassName("active")[0];
          if (el) {
            o = Array.prototype.indexOf.call(el.parentElement.children, el);
            el.classList.remove("active");
            if (o == 0) {
              newEl =
                el.parentElement.children[el.parentElement.children.length - 1];
            } else {
              newEl = el.previousElementSibling;
            }
          } else {
            newEl = r.children[r.children.length - 1];
          }
          newEl.classList.add("active");
          if (newEl.offsetTop < newEl.parentElement.scrollTop) {
            newEl.parentElement.scrollTop -= newEl.clientHeight;
          } else if (
            newEl.offsetTop >
            newEl.parentElement.scrollTop + newEl.parentElement.clientHeight + 3
          ) {
            newEl.parentElement.scrollTop = newEl.offsetTop + 3;
          }
        });
      } else if (e.keyCode == 40) {
        // down arrow
        b.preventDefault();
        k(function () {
          el = r.getElementsByClassName("active")[0];
          if (el) {
            o = Array.prototype.indexOf.call(el.parentElement.children, el);
            el.classList.remove("active");
            if (o == el.parentElement.children.length - 1) {
              newEl = el.parentElement.children[0];
            } else {
              newEl = el.nextElementSibling;
            }
          } else {
            newEl = r.children[0];
          }
          newEl.classList.add("active");
          if (
            newEl.offsetTop + newEl.clientHeight >
            newEl.parentElement.scrollTop + newEl.parentElement.clientHeight
          ) {
            newEl.parentElement.scrollTop += newEl.clientHeight;
          } else if (
            newEl.offsetTop + newEl.clientHeight <
            newEl.parentElement.scrollTop + 3
          ) {
            newEl.parentElement.scrollTop = newEl.offsetTop - 3;
          }
        });
      }
    },
    rhov: function (b) {
      var r = this.ddRslts,
        l = b.target.closest(".dd-rslt"),
        x,
        e;

      k(function () {
        e = r.getElementsByClassName("active");
        for (x = 0; x < e.length; x++) {
          e[x].classList.remove("active");
        }
        if (!!l) {
          l.classList.add("active");
        }
      });
    },
    flLoad: function (b) {
      var i = this.ddVsbl,
        c = this.ddCntr,
        rs = this.ddRslts,
        h = this.ddHdn,
        p = this.ddPhdr,
        q,
        sa = this.searchArea,
        url = "/Search?handler=ValueList&s=" + sa,
        data,
        result = "",
        el,
        active = [];

      k(function () {
        // get values of items that are alreay selected for select box.
        [].forEach.call(h.getElementsByTagName("option"), function (e) {
          active.push(e.getAttribute("value"));
        });

        if (cache.exists(url)) {
          load(cache.get(url));
        } else {
          q = new XMLHttpRequest();
          q.open("post", url, true);
          q.setRequestHeader(
            "Content-Type",
            "application/x-www-form-urlencoded; charset=UTF-8"
          );
          q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
          q.send();

          q.onload = function () {
            var l = q.responseText;
            load(l);

            var ccHeader =
              q.getResponseHeader("Cache-Control") != null
                ? (q.getResponseHeader("Cache-Control").match(/\d+/) || [
                    null,
                  ])[0]
                : null;

            if (ccHeader) {
              cache.set(url, l, ccHeader);
            }
          };
        }

        function load(data) {
          data = JSON.parse(data);
          if (data.length === 0) {
            rs.innerHTML =
              '<div class="dd-rslt disabled">No matches found</div>';
          } else {
            for (i = 0; i < data.length; i++) {
              el = data[i];
              var hiddenClass = "";
              if (active.indexOf(el.ObjectId) !== -1) {
                hiddenClass = "hidden";
              }
              var id = el.ObjectId || el.Description;
              result +=
                '<div class="dd-rslt ' +
                hiddenClass +
                '" value="' +
                id +
                '">' +
                el.Name +
                "</div>";

              if (i == data.length - 1) {
                rs.innerHTML = result;
              }
            }

            if (rs.querySelectorAll(".dd-rslt:not(.hidden)").length < 1) {
              rs.innerHTML +=
                '<div class="dd-rslt disabled">No matches found</div>';
            }
          }
          if (data.length > 10) {
            rs.style.height = Math.min(data.length * 23 + 8, 238) + "px";
            try {
              SimpleScrollbar.initAll();
            } catch (e) {}
          }
        }
      });
    },
    unfcs: function (b) {
      var i = this.ddVsbl,
        c = this.ddCntr,
        rs = this.ddRslts,
        p = this.ddPhdr,
        dd = this.ddWrp,
        hi = this.ddHdn;
      k(function () {
        if (
          b.relatedTarget != null &&
          b.relatedTarget.closest(".dd-cntr") == c
        ) {
          return;
        } else {
          dd.classList.remove("dd-wrp-focus");
          dd.classList.remove("dd-wrp-show");
          rs.style.removeProperty("display");
          if (i.value.length < 1) {
            p.style.removeProperty("display");
          }
          if (!hi.hasAttribute("value")) {
            dd.classList.add("dd-wrp-invalid");
          } else {
            dd.classList.remove("dd-wrp-invalid");
          }
        }
      });
    },
    click: function (b) {
      var i = this.ddVsbl,
        type = this.type,
        rs = this.ddRslts,
        method = this.method,
        hdn = this.ddHdn,
        cntr = this.ddCntr,
        stw = this.stW,
        ipt = this.ddInpt,
        dd = this.ddWrp,
        t = this;
      k(function () {
        dd.classList.remove("dd-wrp-invalid");
        dd.classList.add("dd-wrp-focus");
        if (b.target.closest(".dd-rslt")) {
          var actv = b.target.closest(".dd-rslt");
          var actvName = actv.innerHTML;

          if (type == "SELECT") {
            i.value = "";
            if (
              typeof method != "undefined" &&
              method !== false &&
              method !== null
            ) {
              actv.classList.add("hidden");
              if (!rs.querySelector(".dd-rslt:not(.hidden)")) {
                rs.innerHTML =
                  '<div class="dd-rslt disabled">No matches found</div>';
              }
            } else {
              rs.innerHTML = "";
            }

            // special stuff for user search
            var group = "";
            var userDrop = "";
            if (
              i.hasAttribute("search-area") &&
              i.getAttribute("search-area") == "UserSearchMail"
            ) {
              group = actv.classList.contains("group") ? "group" : "";
              var split = actv.classList.contains("group")
                ? '<div class="mlbx-usrDrpItm">Expand Group</div>'
                : "";
              userDrop =
                '<div class="mlbx-usrDrp"><i class="fas fa-angle-down fa-sm"></i><div class="mlbx-usrDrpCtr"><div class="mlbx-usrDrpItm mail-new-message" data-from="Analytics" data-fromid="45781">New Mail</div>' +
                split +
                '<div class="separator"></div><div class="mlbx-usrDrpItm"><a class="ajax" href="/users?id=45781">Open Profile</a></div></div></div>';
            }

            ipt.insertAdjacentHTML(
              "beforebegin",
              '<div class="dd-itm ' +
                group +
                '">' +
                actvName +
                userDrop +
                "</div>"
            );
            hdn.innerHTML +=
              '<option class="' +
              group +
              '" selected="selected" value="' +
              actv.getAttribute("value") +
              '">' +
              actvName +
              "</option>";
            hdn.dispatchEvent(
              new CustomEvent("change", {
                bubbles: true,
              })
            );

            setTimeout(function () {
              i.focus();
            }, 0);
          } else {
            hdn.setAttribute("value", actv.getAttribute("value"));
            if (actv.classList.contains("group")) {
              i.setAttribute("group", "true");
              hdn.setAttribute("group", "true");
              // only change url in some cases
              if (window.location.pathname.toLowerCase() == "/users") {
                i.closest("form").setAttribute("action", "/groups");
              }
            } else {
              i.removeAttribute("group");
              hdn.removeAttribute("group");
              if (window.location.pathname.toLowerCase() == "/groups") {
                i.closest("form").setAttribute("action", "/users");
              }
            }
            //hdn.trigger('change');
            i.value = actvName;
            i.setAttribute("value", actvName);

            i.dispatchEvent(new CustomEvent("setWidth"));

            cntr.blur();
            //blurThis(i.closest('.dynamic-dropdown'));
            rs.style.removeProperty("height");

            rs.style.removeProperty("display");
            if (method !== "fullList") {
              rs.innerHTML = "";
            }
            hdn.dispatchEvent(
              new CustomEvent("change", {
                bubbles: true,
              })
            );
          }
        } else {
          // need to move input to under cursor
          if (
            dd.querySelector(".dd-itm") &&
            b.target.closest(".mlbx-usrDrp") == null
          ) {
            var itms = dd.querySelectorAll(".dd-itm"),
              myItms = [],
              mouseX = b.pageX,
              mouseY = b.pageY;
            // get items closest to mouse Y
            for (var g = 0; g < itms.length; g++) {
              if (
                mouseY > getOffset(itms[g]).top &&
                mouseY < getOffset(itms[g]).top + itms[g].clientHeight
              ) {
                myItms.push(itms[g]);
              }
            }

            for (g = 0; g < myItms.length; g++) {
              var itmL = getOffset(myItms[g]).left,
                itmC = itmL + myItms[g].clientWidth / 2,
                iptL = getOffset(ipt).left;
              if (mouseX < itmC) {
                myItms[g].parentElement.insertBefore(ipt, myItms[g]);
                break;
              } else if (mouseX > itmC) {
                insertAfter(ipt, myItms[g]);
              }
            }
          }
          setTimeout(function () {
            i.focus();
          }, 0);
          i.dispatchEvent(new CustomEvent("setWidth"));
        }
      });
    },
    stW: function (b) {
      var v = this.ddVsbl,
        c = this.ddCntr,
        d = document,
        q;
      k(function () {
        q = d.createElement("div");
        q.style.visibility = "hidden";
        q.style.width = "auto";
        q.style.zIndex = "-99";
        q.style.whiteSpace = "nowrap";
        q.style.position = "absolute";
        q.style.fontSize = v.style.fontSize;
        q.innerHTML = v.value;
        c.appendChild(q);
        if (
          v.value < 1 &&
          !!document.activeElement &&
          !document.activeElement.closest(".dd-vsbl")
        ) {
          v.style.width = 17 + "px";
        } else {
          v.style.width = Math.max(q.clientWidth + 17, 17) + "px";
        }
        q.parentElement.removeChild(q);
      });
    },
    search: function (b) {
      var el = this.ddVsbl,
        rs = this.ddRslts,
        hi = this.ddHdn,
        c = this.ddWrp,
        ctn = this.ddCntr,
        o,
        r,
        x,
        m = this.method,
        sa = this.searchArea,
        p = this.ddPhdr,
        data,
        url;

      k(function () {
        hi.removeAttribute("value");
        p.style.display = "none";
        c.classList.remove("dd-wrp-invalid");
        if (m == "fullList") {
          c.classList.add("dd-wrp-show");
          rs.style.display = "block";

          /*r = rs.getElementsByClassName('dd-rslt');
                    for(x=0;x<r.length;x++){
                       if(el.value.toUpperCase() == r[x].innerHTML.toUpperCase()){
                       		r[x].style.removeProperty('display');
                        } else {
                       		r[x].style.display = 'none';
                    	}
                    }*/
        } else {
          o = hi.getElementsByTagName("option");
          var values = [].forEach.call(o, function (el) {
            return el.value;
          });
          if (el.value.trim().length > 0) {
            data = {
              s: el.value,
              e: typeof values !== "undefined" ? values.toString() : "",
            };

            url = Object.keys(data)
              .map(function (k) {
                return (
                  encodeURIComponent(k) + "=" + encodeURIComponent(data[k])
                );
              })
              .join("&");

            if (typeof q !== "undefined" && q !== null) q.abort();

            q = new XMLHttpRequest();
            q.open("post", "/Search?handler=" + sa + "&" + url, true);
            q.setRequestHeader(
              "Content-Type",
              "application/x-www-form-urlencoded; charset=UTF-8"
            );
            q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            q.send();

            q.onload = function () {
              try {
                data = JSON.parse(q.responseText);
              } catch (e) {
                return !1;
              }
              rs.style.removeProperty("height");
              if (data.length === 0) {
                rs.innerHTML =
                  '<div class="dd-rslt disabled">No matches found</div>';
              } else {
                rs.innerHTML = "";
                var result = "";
                for (x = 0; x < data.length; x++) {
                  var id = data[x].ObjectId || data[x].Description;

                  var div = document.createElement("div");
                  div.classList.add("dd-rslt");
                  if (data[x].Type == "g") div.classList.add("group");
                  div.setAttribute("value", id);
                  div.innerHTML = data[x].Name;
                  if (x == 0) {
                    div.classList.add("active");
                  }
                  rs.appendChild(div);
                }
              }
              c.classList.add("dd-wrp-show");
              rs.style.display = "block";
              rs.style.removeProperty("left");
              // make sure it is not overflowing parent. Ajust left position if it is.
              var rsL = getOffset(rs).left + rs.clientWidth,
                ctnL =
                  getOffset(ctn.parentElement).left +
                  ctn.parentElement.clientWidth;
              if (rsL > ctnL) {
                rs.style.left = ctnL - rsL - 40 + "px";
              }

              rs.classList.remove("ss-container");

              if (data.length > 10) {
                rs.style.height = Math.min(data.length * 23 + 8, 238) + "px";
                try {
                  SimpleScrollbar.initAll();
                } catch (e) {}
              }
            };
          } else {
            rs.style.removeProperty("display");
            c.classList.remove("dd-wrp-show");
            hi.removeAttribute("value");
            p.style.removeProperty("display");
          }
        }
        setTimeout(function () {
          el.focus();
        }, 0);
      });
    },
  };

  load();
  d.addEventListener("ajax", function () {
    load();
  });
  d.addEventListener(
    "dropdown",
    function (e) {
      if (typeof e.detail !== "undefined" && !!e.detail && !!e.detail.el) {
        load(e.detail.el);
      } else {
        load();
      }
    },
    false
  );

  /*
    // 
    	function addItem(element, type){
    		var visibleInput,hiddenInput,hiddenInputType,activeItem,results,inputContainer;
    		if (type == 'exit'){
    			// element is dropdown-input (dynamic-dropdown-input)
    			visibleInput = element;
    			inputContainer = element.closest('.dynamic-dropdown-input-container');
    			hiddenInput = visibleInput.closest('.dynamic-dropdown').find('*[dname="'+ visibleInput.attr('dname')+ '"]');
    			results = visibleInput.siblings('.dynamic-dropdown-results');
    			hiddenInputType = hiddenInput.get(0).tagName;
    			activeItem = results.find('.dynamic-dropdown-result-item.active').first().length ? results.find('.dynamic-dropdown-result-item.active').first() : results.find('.dynamic-dropdown-result-item:visible:not(:contains("No matches found"))').first();
    			activeItem = '' ? activeItem.length < 1 : activeItem;

    			if(hiddenInputType == 'SELECT'){
    				if (activeItem.length > 0 && activeItem.attr('value').length > 0){
    					visibleInput.val('')
    					if(typeof visibleInput.attr('method') !== typeof undefined && visibleInput.attr('method') !== false){
    						activeItem.addClass('hidden');

    						//if nothing is dd-wrp-showing then add item "no items"
    				 		if(results.find('.dynamic-dropdown-result-item:not(.hidden)').length === 0){
    				 			results.append('<div class="dynamic-dropdown-result-item no-results" value="">No matches found</div>');
    				 	}
    					} else {
    						results.html('');

    					}
    					var group = activeItem.hasClass('group') ? 'group':'';
    					inputContainer.before('<div class="addedItem '+group+'">' + activeItem.text() + '</div>').focus();
    					hiddenInput.append('<option class="'+group+'" selected="selected" value="'+activeItem.attr('value')+'">' + activeItem.text() + '</option>');
    					hiddenInput.trigger("change");

    				} else {
    					visibleInput.val('');
    				}

    			} else {
    				element.val(activeItem.text());
    				element.trigger("change");
    				hiddenInput.attr('value',activeItem.attr('value'));
    				hiddenInput.trigger('change');
    				// if method not fullList then clear list	
    				if(visibleInput.attr('method') !== 'fullList'){
    					results.html('');
    					results.hide();
    					results.html('<div class="dynamic-dropdown-result-item no-results" value="">No matches found</div>');	
    				}
    			}

    			blurThis(visibleInput.closest('.dynamic-dropdown'));

    		} else if (type == 'click'){
    			// element is the "active" result-item
    			inputContainer = element.closest('.dynamic-dropdown-input-container');
    			visibleInput = element.closest('.dynamic-dropdown-results').siblings('.dynamic-dropdown-input');
    			hiddenInput = element.closest('.dynamic-dropdown').find('*[dname="'+ visibleInput.attr('dname')+ '"]');
    			hiddenInputType = hiddenInput.get(0).tagName;
    			results = visibleInput.siblings('.dynamic-dropdown-results');
    			activeItem = element.clone();

    			if(hiddenInputType == 'SELECT'){

    				visibleInput.val('');
    				if(typeof visibleInput.attr('method') !== typeof undefined && visibleInput.attr('method') !== false){
    						activeItem.addClass('hidden');

    						//if nothing is dd-wrp-showing then add item "no items"
    				 		if(results.find('.dynamic-dropdown-result-item:not(.hidden)').length === 0){
    				 			results.append('<div class="dynamic-dropdown-result-item no-results" value="">No matches found</div>');
    				 	}
    					} else {
    						results.html('');

    					}
    				var group = activeItem.hasClass('group') ? 'group':'';
    				inputContainer.before('<div class="addedItem '+group+'">' + activeItem.text() + '</div>').focus();
    				hiddenInput.append('<option class="'+group+'" selected="selected" value="'+activeItem.attr('value')+'">' + activeItem.text() + '</option>');
    				hiddenInput.trigger("change");

    		 		visibleInput.focus();

    			} else {
    				hiddenInput.attr('value',activeItem.attr('value'))
    				hiddenInput.trigger('change');
    				visibleInput.val(activeItem.text());
    				visibleInput.trigger("change");
    				blurThis(visibleInput.closest('.dynamic-dropdown'));
    				if(visibleInput.attr('method') !== 'fullList'){
    					results.html('<div class="dynamic-dropdown-result-item no-results" value="">No matches found</div>');
    				}
    			}
    		}
    		setWidth(visibleInput);
    	}


    	$('body').on('keydown', '.dynamic-dropdown-input', function(e) {
    		var inputContainer = $(this).closest('.dynamic-dropdown-input-container');
    		var input = $(this);
    		var thisVal;
    		
    	});


    	*/
})();
