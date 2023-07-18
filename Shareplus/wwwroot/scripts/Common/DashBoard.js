var DematRematReport = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();
}














$(document).ready(function () {
    let bool = true

    $('#sidebarToggleTop').click(function () {

        if (bool == true) {
            bool = false
            $('#LogoImage').attr('src', '/img/logoSmall.jpg');
            $('#LogoImage').height(50);
            $('#LogoImage').delay(200).fadeIn();
        }
        else {
            bool = true
            $('#LogoImage').attr('src', '/img/logo.jpg');
            $('#LogoImage').height(50);
            $('#LogoImage').delay(200).fadeIn();
        }
    });


    var companyCode = localStorage.getItem('company-code')
    Openloader()
    //$.ajax({
    //    type: "POST",
    //    url: '/Common/Dashboard/GetDashBoardItems',
    //    data: { 'CompCode': companyCode },
    //    datatype: "json",
    //    success: function (result) {
    //        if (result != null) {
    //            google.charts.load("current", { 'packages': ['corechart', 'bar', 'table'] });
    //            google.charts.setOnLoadCallback(drawChart);

    //            function drawChart() {
    //                var data = google.visualization.arrayToDataTable(result);

    //                var options = {
    //                    title: 'Company Summary',
    //                    is3D: true,
    //                };

    //                var chart = new google.visualization.PieChart(document.getElementById('piechart_3d'));
    //                chart.draw(data, options);
    //                var options1 = {
    //                    title: 'Company Summary'

    //                };
    //                var chart1 = new google.visualization.PieChart(document.getElementById('donutchart'));
    //                chart1.draw(data, options1);
    //                var options2 = {

    //                    title: 'Company Performance',
    //                    subtitle: 'Sales, Expenses, and Profit: 2014-2017',

    //                };
    //                var data3 = new google.visualization.DataTable();
    //                data3.addColumn('string', 'Name');
    //                data3.addColumn('number', 'Salary');
    //                data3.addColumn('boolean', 'Full Time Employee');
    //                data3.addRows([
    //                    ['Mike', { v: 10000, f: '$10,000' }, true],
    //                    ['Jim', { v: 8000, f: '$8,000' }, false],
    //                    ['Alice', { v: 12500, f: '$12,500' }, true],
    //                    ['Bob', { v: 7000, f: '$7,000' }, true]
    //                ]);

    //                var table3 = new google.visualization.Table(document.getElementById('table_div'));

    //                table3.draw(data3, { showRowNumber: true, width: '100%', height: '100%' });
    //        }
            
                
    //           // var chart2 = new google.charts.Bar(document.getElementById('columnchart_material'));

    //           //chart2.draw(data, google.charts.Bar.convertOptions(options2));

    //        }

    //    },
    //    error: function (error) {
    //        alert('error', error.message)
            
    //    }
    //})
    var LoadDashboard = function () {
        $.ajax({
            type: "POST", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            url: '/Common/Dashboard/GetDashBoardItems',
            data: { 'CompCode': companyCode },
            datatype: "json",
            success: function (result) {
               
                if (result.isSuccess) {
                    var xValues = ["Normal : " + result.responseData.normalCertificate, "Demated : " + result.responseData.demateCertificate];
                    var yValues = [result.responseData.normalCertificate, result.responseData.demateCertificate];
                    var barColors = [
                        "#0967a1",
                        "#38d200"
                    ];

                    new Chart("myChart", {
                        type: "doughnut",
                        data: {
                            labels: xValues,
                            datasets: [{
                                backgroundColor: barColors,
                                data: yValues
                            }]
                        },
                        options: {
                            title: {
                                display: true,
                                text: "Certificate Chart"
                            }
                        }
                    });


                    var xValues = ["Normal : " + result.responseData.normalKitta, "Demated : " + result.responseData.demateKitta];
                    var yValues = [result.responseData.normalKitta, result.responseData.demateKitta];
                    var barColors = [
                        "#0967a1",
                        "#38d200"
                    ];

                    new Chart("myChart2", {
                        type: "doughnut",
                        data: {
                            labels: xValues,
                            datasets: [{
                                backgroundColor: barColors,
                                data: yValues
                            }],
                            hoverOffset: 4
                        },
                        options: {
                            title: {
                                display: true,
                                text: "Kitta Chart"
                            }
                        }
                    });
                } else {
                   
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
    }

    LoadDashboard()
})