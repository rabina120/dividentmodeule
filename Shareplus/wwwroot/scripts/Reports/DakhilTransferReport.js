function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function Broker(data) {
    var self = this;
    if (data != undefined) {

        self.BCode = ko.observable(data.bcode)
        self.BrokerName = ko.observable(data.name)
        self.BrokerAddress = ko.observable(data.address)
        self.BrokerNpName = ko.observable(data.npname)
        self.BrokerNpAddress = ko.observable(data.npadd)
        self.BrokerTelNo = ko.observable(data.telno)
    }
}
function DakhilTransferReportViewModel() {
    var self = this;

    self.CompanyDetails = ko.observableArray([])
    
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.CompanyList = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.ReportType = ko.observable();
    self.FromDate = ko.observable();
    self.ToDate = ko.observable();
    self.BrokerList = ko.observableArray([]);
    self.BrokerName = ko.observable()
    self.SelectedBroker = ko.observable();
    self.RegNoFrom = ko.observable();
    self.RegNoTo = ko.observable();
    self.ShareKittaFrom = ko.observable();
    self.ShareKittaTo = ko.observable();
    self.BHolderNoFrom = ko.observable();
    self.BHolderNoTo = ko.observable();
    self.SHolderNoFrom = ko.observable();
    self.SHolderNoTo = ko.observable();
    self.SelectedReport = ko.observable();
   


    self.LoadCompany = function () {

        var companyCode = localStorage.getItem('company-code')
        $.ajax({
            type: "post",
            url: '/Common/Company/GetCompanyDetails',

            datatype: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                $("#modalCompany").modal('hide');
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new ParaComp(item)
                    });
                    self.CompanyDetails(mappedTasks);
                    if (!Validate.empty(localStorage.getItem('company-code'))) { self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode()); }
                    // $("#Company").attr("disabled", true);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })


    }
    self.LoadCompany();

    self.LoadBrokerList = function () {
        Openloader()
        var companyCode = self.SelectedCompany()
        $.ajax({
            type: "post",
            url: '/DakhilTransfer/DakhilIndividualTransferEntry/GetBrokerList',
            data: { 'CompCode': companyCode },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new Broker(item)
                    });
                    self.BrokerList(mappedTasks);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }, complete: () => {
                Closeloader()
            }
        })
    }
      self.LoadBrokerList();
    

    self.ReportData = ko.observable();

    self.GeneratePdfReport = function () {
        CompEnName = self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName();
        var ReportData = {
            CompCode: self.SelectedCompany(),
            ReportType: self.ReportType(),
            SelectedAction: self.SelectedReport(),
            FromDate: self.FromDate(),
            ToDate: self.ToDate(),
            RegnoFrom: self.RegNoFrom(),
            RegnoTo: self.RegNoTo(),
            TranKittaFrom: self.ShareKittaFrom(),
            TranKittaTo: self.ShareKittaTo(),
            BHolderNoFrom: self.BHolderNoFrom(),
            BHolderNoTo: self.BHolderNoTo(),
            SHolderNoFrom: self.SHolderNoFrom(),
            SHolderNoTo: self.SHolderNoTo(),
            Broker: self.SelectedBroker(),
            CompEnName: CompEnName
        }

        if (!Validate.empty(self.SelectedCompany())) {
            Openloader()

            $.ajax({
                url: '/Reports/DakhilTransfer/GenerateReportForPDF',
                type: "post",
                data: ReportData,
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        var x = window.open();
                        if (x) {
                            x.document.open();
                            x.document.write('<iframe width="100%" height="100%" src="' + result.responseData.message + '"></iframe>');
                            x.document.title = result.message;
                            x.document.close();
                        } else {
                            var fileName = result.message;
                            var a = document.createElement("a");
                            a.href = "data:application/octet-stream;base64," + result.responseData;
                            a.download = fileName;
                            a.click();
                        //    alert('warning', 'Failed to Open Pdf File.');
                        //    alert('success', 'Downloading Pdf File.');
                        //    var fileName = result.message;
                        //    var a = document.createElement("a");
                        //    a.href = result.responseData.message;
                        //    a.download = fileName;
                        //    a.click();
                        }
                    }
                    else {
                        alert('error', result.message);
                    }

                }, error: function (error) {
                    alert('error', error.message)
                },
                complete: () => {
                    Closeloader()
                }
            }) 
            
        }
        else {
            alert('error', 'Please Select A Company !!!');
        }
  
    }
    self.GenerateExcelReport = function () {
        CompEnName = self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName();
        var ReportData = {
            CompCode: self.SelectedCompany(),
            ReportType: self.ReportType(),
            SelectedAction: self.SelectedReport(),
            FromDate: self.FromDate(),
            ToDate: self.ToDate(),
            RegnoFrom: self.RegNoFrom(),
            RegnoTo: self.RegNoTo(),
            TranKittaFrom: self.ShareKittaFrom(),
            TranKittaTo: self.ShareKittaTo(),
            BHolderNoFrom: self.BHolderNoFrom(),
            BHolderNoTo: self.BHolderNoTo(),
            SHolderNoFrom: self.SHolderNoFrom(),
            SHolderNoTo: self.SHolderNoTo(),
            Broker: self.SelectedBroker(),
            CompEnName: CompEnName
        }
        if (!Validate.empty(self.SelectedCompany())) {
            Openloader()

            $.ajax({
                url: '/Reports/DakhilTransfer/GenerateReportForExcel',
                type: "post",
                data: ReportData,
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {

                    if (result.isSuccess) {
                        var fileName = result.message;
                        var a = document.createElement("a");
                        a.href = "data:application/octet-stream;base64," + result.responseData;
                        a.download = fileName;
                        a.click();
                    }
                    else {
                        alert('error', result.message);
                    }

                }, error: function (error) {
                    alert('error', error.message)
                },
                complete: () => {

                    
                }
            })
            Closeloader()

        }
        else {
            alert('error', 'Please Select A Company !!!');
        }
    }

}

$(document).ready(function () {
    ko.applyBindings(new DakhilTransferReportViewModel());
    $('#date1 .date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });

    $('#date2 .date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#date2 .input-group.date").datepicker("setDate", new Date());
})
