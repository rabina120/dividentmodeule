function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function Dividend(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.compcode);
        self.Divcode = ko.observable(data.divcode);
        self.Description = ko.observable(data.description);
        self.FiscalYr = ko.observable(data.fiscalYr);
        self.ShowDividendText = self.Divcode() + " " + self.Description() + " " + self.FiscalYr();
        self.Status = ko.observable(data.bdStatus)
    }
}

function DividendBatch(data) {
    var self = this;
    if (data != undefined) {
        self.BatchId = ko.observable(data.batchID);
        self.BatchText = "BatchNo: " + data.batchID
        self.Status=ko.observable(data.status)
    }
}


var EsewaStatusReportViewModal = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()

    //Dividend ko table list lai
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.compcode = ko.observable();
    self.Divcode = ko.observable();
    self.Description = ko.observable();
    self.FiscalYr = ko.observable();

    self.CurrentBatch = ko.observable();
    self.BatchLists = ko.observableArray([])

    self.SelectedSubReportId = ko.observable()
    self.SelectedReportId = ko.observable()
    self.ReportLists = ko.observableArray([])
   

    self.SubReportLists  = ko.observableArray([])

    var FiscalYr = '';
    self.ReportValidation = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Select A Company <br/>"
        }
        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select A Dividend <br/>"
        }
        if (Validate.empty(self.CurrentBatch())) {
            errMsg += "Please Select A Batch <br/>"
        }
        if (Validate.empty(self.SelectedReportId())) {
            errMsg += "Please Select A Report <br/>"
        }
        if (Validate.empty(self.SelectedSubReportId())) {
            errMsg += "Please Select A SubReport <br/>"
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
    }


    // validation
    self.Validation = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Select A Company <br/>"
        }
        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select A Dividend <br/>"
        }
        if (Validate.empty(self.CurrentBatch())) {
            errMsg += "Please Select A Batch <br/>"
        }
        if (Validate.empty(self.SelectedReportId())) {
            errMsg += "Please Select A Report <br/>"
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
    }
    //Loading company select options
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
                    if(!Validate.empty(localStorage.getItem('company-code'))){self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode());}
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

    //Loading dividend lists select options

    self.LoadDividendLists = function () {
        
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            //self.DividendLists = ko.observableArray([]);
            $.ajax({
                type: "post",
                url: '/Reports/ESewaReports/GetAllDividends',
                data: { 'CompCode': self.SelectedCompany() },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Dividend(item)
                        });
                        self.DividendLists(mappedTasks);
                    } else {
                        alert('warning', "No Dividend List Found ")
                        window.location.reload();
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })
        }

     }
    self.SelectedCompany.subscribe(() => {
        self.LoadDividendLists();
    })

    self.SelectedDividend.subscribe(() => {
        if (self.SelectedDividend != undefined) {
            GetBatch();
            FiscalYr = self.DividendLists().find(x => x.DivCode = self.SelectedDividend()).FiscalYr();
        }
    })

    self.CurrentBatch.subscribe(function () {
        self.ReportLists([])
        self.SubReportLists([])
        var status = self.BatchLists().find(x => x.BatchId() == self.CurrentBatch()).Status();
        switch (status.trim()) {
            case "CI":
                alert('warning', 'THE SELECTED DIVIDEND HAS NO CDS DATA IMPORTED.')
                break;
            case "CC":
                alert('warning', "THE SELECTED DIVIDEND'S DATA IS YET TO BE VALIDATED")
                break;
            case "AV":
                alert('warning', "THE SELECTED DIVIDEND'S DATA IS YET TO BE VALIDATED THROUGH ESEWA")
            case "AV.1":
                alert('warning', "THE SELECTED DIVIDEND'S DATA IS YET TO COMPLETE VALIDATION THROUGH ESEWA")
            case "TP.1":
                alert('warning', "THE SELECTED DIVIDEND'S DATA IS YET TO COMPLETE TRANSACTION THROUGH ESEWA")
                ReportLists.push({ ReportId: "AV", ReportText: "Account Validation" })
                self.SelectedReportId('AV')
                break;
            case "TP":
                ReportLists.push({ ReportId: "AV", ReportText: "Account Validation" })
                self.SelectedReportId('AV')
                break;
            case "CO":
                ReportLists.push({ ReportId: "AV", ReportText: "Account Validation" })
                ReportLists.push({ ReportId: "TP", ReportText: "Transaction Processing" })
                ReportLists.push({ ReportId: "TS", ReportText: "Transaction Status" })
                self.SelectedReportId('AV')
                break;
        }
    })

    self.GetBatch = function () {
        $.ajax({
            type: "post",
            url: '/reports/ESewaReports/GetALLBatch',

            datatype: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: {'Compcode':self.SelectedCompany(),'DivCode':self.SelectedDividend()},
            success: function (result) {
                $("#modalCompany").modal('hide');
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new DividendBatch(item)
                    });
                    self.BatchLists(mappedTasks);
                    
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })

    }
    self.SelectedReportId.subscribe(function () {
        self.SubReportLists([])
        if (self.SelectedReportId() == "AV") {
            self.SubReportLists.push({
                "SubReportText": "Validated",
                "SubReportId": "V"
            }, {
                "SubReportText": "Non-Validated",
                "SubReportId": "NV"
            })
        } else if (self.SelectedReportId() == "TP") {
            self.SubReportLists.push({
                "SubReportText": "Processing",
                "SubReportId": "P"
            }, {
                "SubReportText": "Failed",
                "SubReportId": "F"
            })
        } else if (self.SelectedReportId() == "TS") {
            
            self.SubReportLists.push({
                "SubReportText": "Processing",
                "SubReportId": "P"
            }, {
                "SubReportText": "Success",
                "SubReportId": "S"
            }, {
                "SubReportText": "Failed",
                "SubReportId": "F"
            }, {
                "SubReportText": "Pending",
                "SubReportId": "PE"
            })
        }
    })

    self.GeneratePDF = function () {
        GenerateReport('P') 
    }
    self.GenerateExcel = function () {
        GenerateReport('E') 
    }
    self.GenerateReport = function (exportType) {
        if (self.ReportValidation()) {
            var CompEnName = self.CompanyDetails().find(x => x.CompCode = self.SelectedCompany()).CompEnName();
            
            Openloader()
            $.ajax({
                type: "post",
                url: '/reports/ESewaReports/GenerateReport',

                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                data: {
                    'Compcode': self.SelectedCompany(), 'DivCode': self.SelectedDividend(), 'Batch': self.CurrentBatch(),
                    'ReportType': self.SelectedReportId(), 'ReportSubType': self.SelectedSubReportId(),
                    'CompEnName': CompEnName,
                    'FiscalYear': FiscalYr, 'exportTo': exportType
                },
                success: function (result) {
                    
                    if (result.isSuccess) {
                        var fileName = result.message;
                        var a = document.createElement("a");
                        a.href = "data:application/octet-stream;base64," + result.responseData;
                        a.download = fileName;
                        a.click();

                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })
            Closeloader()
        }
    }
    self.initElements = function () {

    }
    initElements()

    // clear control
    self.ClearControl = function () {
        self.SelectedDividend('')
        self.CurrentBatch('')
        self.initElements()
        $('#DividendList').val(null).trigger('change')
    }


}

$(document).ready(function () {
    ko.applyBindings(new EsewaStatusReportViewModal());

});
