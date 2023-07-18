var backgroundColor = [
    'rgba(255, 99, 132, 0.7)',
    'rgba(54, 162, 235, 0.7)',
    'rgba(255, 206, 86, 0.7)',
    'rgba(75, 192, 192, 0.7)',
    'rgba(153, 102, 255, 0.7)',
    'rgba(255, 159, 64, 0.7)'
];
var borderColor = [
    'rgba(255, 99, 132, 1)',
    'rgba(54, 162, 235, 1)',
    'rgba(255, 206, 86, 1)',
    'rgba(75, 192, 192, 1)',
    'rgba(153, 102, 255, 1)',
    'rgba(255, 159, 64, 1)'
];

var altColor = [
    'rgba(204, 5, 47, 0.7)',
    'rgba(1, 84, 140, 0.7)',
    'rgba(255, 191, 36, 0.7)',
    'rgba(24, 168, 168, 0.7)',
    'rgba(109, 54, 219, 0.7)',
    'rgba(238, 137, 38, 0.7)',
];

var borderColor = '#000';

function CreateBarGraph(element, array, label, text, onClick) {
    var data = array.map(x => x.TotalCount);
    new Chart(element, {
        type: 'bar',
        data: {
            labels: array.map(x => x.Label),
            datasets: [{
                data,
                label,
                backgroundColor: altColor,
                borderColor,
                fill: false
            }]
        },
        options: {
            title: {
                display: true,
                text,
                fontSize: 14,
            },
            scales: {
                yAxes: [{
                    ticks: {
                        max: Math.max(...data) + Math.ceil(Math.max(...data) / 10),
                        min: 0,
                        stepSize: Math.ceil(Math.max(...data) / 10),
                    }
                }]
            },
            legend: {
                display: true,
                position: 'bottom'
            },
            events: ['click', 'mousemove'],
            onClick: ((x, y, z) => {
                if (array[y[0]?._index])
                    onClick(array[y[0]?._index])
            }),
        }
    });

}

function CreateStackedBarGraph(element, array, label, text, onClick) {
    if (array.length > 0) {
        let stacks = [...new Set(array.map(x => x.LegendId))];

        let datasets = stacks.map((x, i) => {
            let lst = array.filter(y => y.LegendId == x);
            let label = lst[0].Legend;
            let data = lst.map(x => x.TotalCount);
            return {
                data,
                label,
                backgroundColor: altColor[i],
                borderColor: '#000',
                borderWidth: 1,
                fill: false
            };
        });

        let maxVal = Math.max(...datasets[0].data.map((x, i) => datasets.reduce((a, b) => a + b.data[i], 0)));
        let stepSize = Math.ceil(maxVal / 10);
        let yMax = maxVal + stepSize;
        let labels = [...new Set(array.map(x => x.Label))];

        new Chart(element, {
            type: 'bar',
            data: {
                labels,
                datasets
            },
            options: {
                title: {
                    display: true,
                    text,
                    fontSize: 14,
                },
                scales: {
                    xAxes: [{ stacked: true }],
                    yAxes: [
                        {
                            stacked: true,
                            ticks: {
                                max: yMax,
                                min: 0,
                                stepSize
                            }
                        }]
                },
                legend: {
                    display: true,
                    position: 'bottom'
                },
                events: ['click', 'mousemove'],
                onClick: ((evt, y, z) => {
                    if (y[0]) {
                        let chart = y[0]._chart;
                        let activePoint = chart.getElementAtEvent(evt)[0];
                        let data = activePoint._chart.data;
                        let datasetIndex = activePoint._datasetIndex;
                        let value = data.datasets[datasetIndex].data[activePoint._index];
                        let xIndex = y[0]?._index;
                        let label = data.labels[xIndex];
                        let legend = data.datasets[datasetIndex].label;
                        let selectedData = array.find(x => x.Label == label && x.Legend == legend);
                        onClick(selectedData);
                    }
                }),
            }
        });

    }
}

function CreatePieChart(ct, array, text, onClick) {
    var data = array.map(x => x.TotalCount);
    var myPieChart = new Chart(ct, {
        type: 'pie',
        data: {
            datasets: [{
                data,
                backgroundColor: altColor,
                borderColor,
                borderWidth: 1,
            }],
            // These labels appear in the legend and in the tooltips when hovering different arcs
            labels: array.map(x => x.Label)
        },
        options: {
            title: {
                display: true,
                text,
                fontSize: 14,
            },
            events: ['click', 'mousemove'],
            onClick: ((x, y, z) => {
                if (array[y[0]?._index])
                    onClick(array[y[0]?._index])
            }),
            legend: {
                display: true,
                position: 'bottom'
            },
        }
    });
}
function CreateDoughnutChart(ct, array, text, onClick) {
    var data = array.map(x => x.TotalCount);
    var altColor = array.map(x => x.altColor);
    var myDoughnutChart = new Chart(ct, {
        type: 'doughnut',
        cutoutPercentage: 30,
        data: {
            datasets: [{
                data,
                backgroundColor: altColor,
                borderColor: '#fff',
                borderWidth: 1,
            }],
            // These labels appear in the legend and in the tooltips when hovering different arcs
            labels: array.map(x => x.Label)
        },
        options: {
            cutoutPercentage: 90,
            title: {
                display: true,
                text,
                fontSize: 14,
            },
            events: ['click', 'mousemove'],
            onClick: ((x, y, z) => {
                if (array[y[0]?._index])
                    onClick(array[y[0]?._index])
            }),
            legend: {
                display: true,
                position: 'bottom'
            },
        }
    });
}