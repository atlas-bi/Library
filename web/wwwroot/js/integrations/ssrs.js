alert('hi');
var form = document.createElement("form");
var element1 = document.createElement("input");


form.method = "POST";
form.action = window.location.href;

element1.value="ANDREWS, LIZY";
element1.name="ReportViewerControl$ctl04$ctl07$txtValue";
form.appendChild(element1);


document.body.appendChild(form);

form.submit();
