function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)

    }
}
function pledgeAt(data) {
    var self = this;
    if (data != undefined) {
        self.Pcode = ko.observable(data.pcode)
        self.Name = ko.observable(data.name);
        self.PledgeAtName = ko.observable(data.pcode + ' ' + data.name)
    }
}

var PSLReport = function () {
    self.CompanyDetails = ko.observableArray([]);
    self.CompEnName = ko.observable();
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.SplitDateFrom = ko.observable();
    self.SplitDateTo = ko.observable();
    self.HolderNoFrom = ko.observable();
    self.HolderNoTo = ko.observable();
    self.CertNoFrom = ko.observable();
    self.CertNoTo = ko.observable();
    self.PSLDateFrom = ko.observable();
    self.PSLDateTo = ko.observable();
    self.CertificateNoFrom = ko.observable();
    self.CertificateNoTo = ko.observable();
    self.PledgeAt = ko.observable();

    self.TranType = ko.observable();
    self.ShareType = ko.observable();
    self.DataType = ko.observable();
    self.PCode = ko.observable(),

        self.BasedOn = ko.observable();
    self.BasedOn = ko.observable("UP");
    self.BasedOn = ko.observable("CP");
    self.BasedOn = ko.observable("P");
    self.BasedOn = ko.observable("U");
    self.BasedOn = ko.observable("R");
    self.BasedOn = ko.observable("C");
    self.BasedOn = ko.observable("H");
    self.BasedOn = ko.observable("T");
    self.BasedOn = ko.observable("Pr");
    self.BasedOn = ko.observable("Pu");
    self.BasedOn = ko.observable("A");
    self.BasedOn = ko.observable("N");
    self.BasedOn = ko.observable("Pl");
    self.BasedOn = ko.observable("S");
    self.BasedOn = ko.observable("L");
    self.SelectedAction = ko.observable();
    self.SelectedReportType = ko.observable();
    self.SelectedAppStatus = ko.observable();
    self.SelectedDataType3 = ko.observable();
    self.SelectedShOwnerType = ko.observable();


    self.ReportType = ko.observableArray([
        { DataType: "U", DataTypeName: "UnClearPSL" },
        { DataType: "C", DataTypeName: "ClearPSL" },

    ]);
    self.AppStatus = ko.observableArray([
        { DataType: "P", DataTypeName: "Posted" },
        { DataType: "U", DataTypeName: "UnPosted" },
        { DataType: "R", DataTypeName: "Rejected" }
    ]);
    self.OrderBy = ko.observableArray([
        { DataType: "C", DataTypeName: "By CertNO" },
        { DataType: "H", DataTypeName: "By HOlderNO" },
        { DataType: "T", DataTypeName: "By TranDt" }
    ]);
    self.ShOwnerType = ko.observableArray([
        { DataType: "1", DataTypeName: "Promoter" },
        { DataType: "2", DataTypeName: "Public" },
        { DataType: " ", DataTypeName: "All" }
    ]);
    self.SelectedTranType = ko.observable();
    self.TranTypeList = ko.observableArray([
        { TranType: "2", TranTypeName: "Pledge" },
        { TranType: "3", TranTypeName: "Suspend" },
        { TranType: "4", TranTypeName: "Lost" }

    ]);
    //Setting default value
    self.SelectedOrderBy = ko.observable("T")
    self.SelectedShOwnerType = ko.observable(" ")


    self.Pcode = ko.observable()
    self.Name = ko.observable()
    self.PledgeAtName = ko.observable()
    self.SelectedPledgeAt = ko.observable();
    self.PledgeAtList = ko.observableArray([])
    self.CertificateNoFrom = ko.observable();
    self.CertificateNoTo = ko.observable();

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

    self.LoadPledgeAt = function () {

        $.ajax({
            type: "post",
            url: '/Reports/PSLReport/GetAllPledgeAt',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new pledgeAt(item)
                    });
                    self.PledgeAtList(mappedTasks);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })


    }
    self.LoadPledgeAt();



    self.GenerateData = function (exportType) {
        //$('#tbl_CashDividendReport').DataTable().clear();
        //$('#tbl_CashDividendReport').DataTable().destroy();
        if (self.ValidateCompany()) {
            if (self.ValidateForm()) {

                Openloader()
                CompEnName = self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName();
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/Reports/PSLReport/GenerateReport',
                    data: {
                        'CompCode': self.SelectedCompany(),
                        "CompEnName": CompEnName,
                        'PCode': self.SelectedPledgeAt(),
                        'TranType': SelectedTranType(),
                        'PSLDateFrom': self.PSLDateFrom(),
                        'PSLDateTo': self.PSLDateTo(),
                        'HolderNoFrom': self.HolderNoFrom(),
                        'HolderNoTo': self.HolderNoTo(),
                        'CertNoFrom': self.CertNoFrom(),
                        'CertNoTo': self.CertNoTo(),
                        'AppStatus': self.SelectedAppStatus(),
                        'OrderBy': self.SelectedOrderBy(),
                        'ShareType': self.SelectedShOwnerType(),
                        'ReportType': self.SelectedReportType()
                    },
                    dataType: "json",
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

                        Closeloader()
                    }
                })

            }
        }
    }

    self.GenerateDataPdf = function (exportType) {
        //$('#tbl_CashDividendReport').DataTable().clear();
        //$('#tbl_CashDividendReport').DataTable().destroy();
        if (self.ValidateCompany()) {
            if (self.ValidateForm()) {

                Openloader()
                CompEnName = self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName();
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/Reports/PSLReport/GenerateReoprtPdf',
                    data: {
                        'CompCode': self.SelectedCompany(),
                        "CompEnName": CompEnName,
                        'PCode': self.SelectedPledgeAt(),
                        'TranType': SelectedTranType(),
                        'PSLDateFrom': self.PSLDateFrom(),
                        'PSLDateTo': self.PSLDateTo(),
                        'HolderNoFrom': self.HolderNoFrom(),
                        'HolderNoTo': self.HolderNoTo(),
                        'CertNoFrom': self.CertNoFrom(),
                        'CertNoTo': self.CertNoTo(),
                        'AppStatus': self.SelectedAppStatus(),
                        'OrderBy': self.SelectedOrderBy(),
                        'ShareType': self.SelectedShOwnerType(),
                        'ReportType': self.SelectedReportType()
                    },
                    dataType: "json",
                    success: (result) => {
                        if (result.isSuccess) {
                            var embed = "<embed width='100%' height='100%'  type='application/pdf' src='data:application/pdf;base64," + result.responseData + "'/>"
                            var x = window.open();
                            x.document.open();
                            x.document.write(embed);
                            x.document.title = result.message;

                            //var fileName = result.message;
                            //var a = document.createElement("a");
                            //a.href = "data:application/octet-stream;base64," + result.responseData;
                            //a.download = fileName;
                            //a.click();
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
        }
    }

    self.selectedValueChanged = function () {
        var dropdown = document.getElementById("pslType");

        if (self.SelectedTranType() == '2') {
            document.getElementById("pslType").hidden = false;
        }
        else {
            document.getElementById("pslType").hidden = true;
        }
    }

    self.ValidateForm = function () {

        var errMsg = "";
        if (Validate.empty(self.SelectedReportType())) {
            errMsg += "Select Report Type .<br/>"
        }
        if (Validate.empty(self.SelectedAppStatus())) {
            errMsg += "Select App Status .<br/>"
        }
        if (Validate.empty(self.SelectedOrderBy())) {
            errMsg += "Select Order By .<br/>"
        }
        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        }
        else {
            return true;
        }

    }

}




$(document).ready(function () {


   
    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
    $('#Add,#Edit,#Delete').attr('disabled', false);

    $('#Save,#Cancel').attr('disabled', true);

    ko.applyBindings(new PSLReport());

})