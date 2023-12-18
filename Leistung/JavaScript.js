let socket;
var timeout = setInterval(UpdateDaten, 5000);

function UpdateDaten() {

    socket.send("s");

}
window.onload = function () {

    socket = new WebSocket('ws://127.0.0.1:8081');

    function updateIntervall() {
        var intervallValue = document.getElementById("intervallInput").value;
        clearInterval(timeout);
        timeout = setInterval(UpdateDaten, intervallValue*1000);
        document.getElementById("intervallValue").innerText = intervallValue;
    }


    document.getElementById("intervallInput").oninput = updateIntervall;

    socket.onopen = function () {
        alert("Verbindung hergestellt")

    }
    socket.onmessage = NachrichtEmpfang;
    function NachrichtEmpfang(event) {
        let data = JSON.parse(event.data);

        console.log(event.data);
        let cpu = data.Cpu;
        let ram = data.Mem;
        let hdd = data.Disk;
        document.getElementById("cpu").innerHTML = "CPU: " + cpu + "%";
        document.getElementById("memory").innerHTML = "Memory: " + ram + "MB";
        document.getElementById("hdd").innerHTML = "HDD: " + hdd + "%";
        document.getElementById("pro1").value = parseFloat(cpu);
        document.getElementById("mem").value = parseFloat(ram);
        document.getElementById("hd").value = parseFloat(hdd);

        drawChart(parseFloat(cpu), parseFloat(ram), parseFloat(hdd))

    }
}
google.charts.load('current', { 'packages': ['gauge'] });
google.charts.setOnLoadCallback(drawChart);

function drawChart(cpulabel, ramlabel, disklabel) {

    var data = google.visualization.arrayToDataTable([
        ['Label', 'Value'],
        ['Memory', (ramlabel / 32768 * 100)],
        ['CPU', cpulabel],
        ['Disk', disklabel]
    ]);

    var options = {
        width: 400, height: 120,
        redFrom: 90, redTo: 100,
        yellowFrom: 75, yellowTo: 90,
        minorTicks: 5
    };

    var chart = new google.visualization.Gauge(document.getElementById('chart_div'));

    chart.draw(data, options);
}
