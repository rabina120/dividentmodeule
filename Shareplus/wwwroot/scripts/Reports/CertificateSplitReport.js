function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)

    }
}

var CertificateSplitReport = function () {
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
    self.CertificateNoFrom = ko.observable();
    self.CertificateNoTo = ko.observable();

    self.BasedOn = ko.observable();
    self.BasedOn = ko.observable("P");
    self.BasedOn = ko.observable("U");
    self.SelectedAction = ko.observable();
    self.SelectedDataType = ko.observable('P')
    self.FromSystem = ko.observable('N')
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

    self.GenerateReport = function (data) {
        if (self.ValidateCompany()) {
            if (self.Validation()) {
                Openloader()
                var ReportData = {
                    "CompCode": self.SelectedCompany(),
                    "CompEnName": localStorage.getItem('company-name'),
                    "DataType": self.SelectedDataType(),
                    "SplitDateFrom": self.SplitDateFrom(),
                    "SplitDateTo": self.SplitDateTo(),
                    "CertNoFrom": parseInt(self.CertNoFrom(), 10),
                    "CertNoTo": parseInt(self.CertNoTo(), 10),
                    "HolderNoFrom": parseInt(self.HolderNoFrom(), 10),
                    "HolderNoTo": parseInt(self.HolderNoTo(), 10),

                }
                $.ajax({
                    type: "post", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: "/Reports/CertificateSplitReport/GenerateReport",
                    data: {
                        "ReportData1": JSON.stringify(ReportData),
                        "ExportReportType": data,
                        "FromSystem": self.FromSystem()

                    },
                    dataType: "json",
                    success: (result) => {
                        if (result.isSuccess) {
                            var byteArray = Uint8Array.from(atob(result.responseData), c => c.charCodeAt(0)); // Decode base64 data
                            var pdfBlob = new Blob([byteArray], { type: 'application/pdf' });
                            var x = window.open();
                            if (x) {
                                x.document.open();
                                x.document.write('<iframe width="100%" height="100%" src="' + URL.createObjectURL(pdfBlob) + '"></iframe>');
                                x.document.title = result.message;
                                x.document.close();
                            } else {
                                alert('warning', 'Failed to open file.');
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
    self.SplitDateFrom('')
    self.SplitDateTo('')
    self.CertNoFrom('')
    self.CertNoTo('')
    self.HolderNoFrom('')
    self.HolderNoTo('')
}


self.Validation = function (data) {
    var errMsg = ""
    if (Validate.empty(self.SelectedDataType())) {
        errMsg += "Please Select A Data Type. <br/>";
    }
    if (Validate.empty(self.FromSystem())) {
        errMsg += "Please Select A System. <br/>";
    }
    if (Validate.empty(self.SplitDateFrom())) {
        errMsg += "Please Enter Split Date From. <br/>";
    }
    if (Validate.empty(self.SplitDateTo())) {
        errMsg += "Please Enter Split Date To. <br/>";
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

        dateFormat: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true,
        endDate: '+0d',


    });
    $('#Add,#Edit,#Delete').attr('disabled', false);

    $('#Save,#Cancel').attr('disabled', true);

    ko.applyBindings(new CertificateSplitReport())




})