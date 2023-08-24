function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

var ShholderLockUnlockReport = function ()
{
    self.CompanyDetails = ko.observableArray([]);
    self.CompEnName = ko.observable();
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.DateFrom = ko.observable();
    self.DateTo = ko.observable();
    self.HolderNoFrom = ko.observable();
    self.HolderNoTo = ko.observable();
    self.SelectedDataType = ko.observable();
    self.SelectedStatusType = ko.observable();

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

    //Report generation
    self.GenerateReport = function (data) {
        if (self.ValidateCompany()) {
            if (self.Validation()) {
                Openloader()
                CompEnName = self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName();
                $.ajax({
                    type: "post", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: "/Reports/ShholderLockUnlockReport/ExportExcel",
                    data: {
                        "CompCode": self.SelectedCompany(),
                        "CompEnName": CompEnName,
                        "DataType": self.SelectedDataType(),
                        "StatusType": self.SelectedStatusType(),
                        "DateFrom": self.DateFrom(),
                        "DateTo": self.DateTo(),
                        "HolderNoFrom": self.HolderNoFrom(),
                        "HolderNoTo": self.HolderNoTo(),
                        "ReportType":data

                    },
                    dataType: "json",
                    success: (result) => {
                        if (result.isSuccess) {
                            if (data == 'E') {
                                var fileName = result.message;
                                var a = document.createElement("a");
                                a.href = "data:application/octet-stream;base64," + result.responseData;
                                a.download = fileName;
                                a.click();
                                alert('success', "Report Generated Successfully");
                            }
                            else {
                                var embed = "<embed width='100%' height='100%'  type='application/pdf' src='data:application/pdf;base64," + result.responseData + "'/>"
                                var x = window.open();
                                if (x) {
                                    x.document.open();
                                    x.document.write(embed);
                                    x.document.title = result.message;
                                    x.document.close();
                                } else {
                                    alert('warning', 'Failed to View Report.');
                                    alert('success', 'Downloading pdf repot');
                                    var fileName = result.message;
                                    var a = document.createElement("a");
                                    a.href = "data:application/octet-stream;base64," + result.responseData;
                                    a.download = fileName;
                                    a.click();
                                }
                            }
                            
                        } else {
                            alert('error', result.message)
                        }
                    }, error: (error) => {
                        alert('error', error.message)
                    }, complete: () => {

                        Closeloader()
                    }
                })
            }
        }
    }


}
self.Clear = function () {

    self.SelectedDataType('')
    self.SelectedStatusType('')
    self.DateFrom('')
    self.DateTo('')
    self.HolderNoFrom('')
    self.HolderNoTo('')
}


self.Validation = function (data) {
    var errMsg = ""
    if (Validate.empty(self.SelectedDataType())) {
        errMsg += "Please Select Type. <br/>";
    }
    if (Validate.empty(self.SelectedStatusType())) {
        errMsg += "Please Select Status. <br/>";
    }
    if (errMsg == "") {
        return true;
    } else {
        alert('warning', errMsg)
        return false
    }
}


$(function () {


    $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true,
        endDate: '+0d',


    });
    $('#simple-date2 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true,
        endDate: '+0d',


    });
    $('#Add,#Edit,#Delete').attr('disabled', false);

    $('#Save,#Cancel').attr('disabled', true);

    ko.applyBindings(new ShholderLockUnlockReport())

})