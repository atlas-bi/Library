function loadFlowcharts() {
    var d = document,
        m = d.getElementsByClassName('mermaid');

    [].forEach.call(m,function(e){
        if(e.getElementsByTagName('svg').length == 0){
            var chart, code = e.innerHTML;
            e.innerHTML = '';
            if(code.trim() != ''){
                try{
                    chart = flowchart.parse(code);
                    chart.drawSVG(e);
                } catch (l){}
            }
        }
    });
}

loadFlowcharts();

document.addEventListener('load-charts', function(){
    setTimeout(function(){
        loadFlowcharts();
    },0);
});