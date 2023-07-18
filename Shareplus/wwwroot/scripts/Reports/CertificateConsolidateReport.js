function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

var ConsolidateReport = function () {
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.HolderNoFrom = ko.observable();
    self.HolderNoTo = ko.observable();
    self.ConsolidateDate = ko.observable();

    self.SelectedAction = ko.observable();
    self.ConsolidateTo = ko.observable();
    self.BasedOn = ko.observable();
    self.BasedOn = ko.observable("P");
    self.BasedOn = ko.observable("U");
    self.BasedOn = ko.observable("R");
    self.SelectedDataType = ko.observable();
    self.DataTypeList = ko.observableArray([
        { DataType: "P", DataTypeName: "Posted" },
        { DataType: "U", DataTypeName: "Unposted" },
        { DataType: "R", DataTypeName: "Rejected" }
    ]);
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
    self.GenerateReport = function (data)
    {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.SelectedDataType())) {
                if (self.Validation(self.SelectedDataType())) {
                    Openloader()
                    var ReportData = {
                        "CompCode": self.SelectedCompany(),
                        "CompEnName": localStorage.getItem('company-name'),                         
                        "DataType": self.SelectedDataType(),
                       
                        "ConsolidateDate": self.ConsolidateDate(),
                        "ConsolidateTo": self.ConsolidateTo(),
                        
                        "HolderNoFrom": self.HolderNoFrom(),
                        "HolderNoTo": self.HolderNoTo(),
                       
                    }
                    $.ajax({
                        type: "post",
                        url: "/Reports/ConsolidateReport/GenerateReport",
                        data: {
                            "ReportData1": JSON.stringify(ReportData),
                            "ExportReportType": data
                            
                        }, beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        dataType: "json",
                        success: (result) => {
                            if (result.isSuccess) {
                                alert('success', "Report Generated Successfully");
                                var fileName = result.message;
                                var a = document.createElement("a");
                                a.href = "data:application/octet-stream;base64," + result.responseData;
                                a.download = fileName;
                                a.click();
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
            } else {
                alert('warning', 'Select A Report Type <br/>');
                //$('#ReportType').focus()
            }
        }
    }
    //changing html on select
    self.BasedOn.subscribe(function () {
        if (self.BasedOn() == 'R') {
            $("#textBasedOn").html("Posted:");
            $("#textBasedOn2").html("Unposted:");
            $("#textBasedOn3").html("Rejected:");

        }
        else {
            $("#textBasedOn").html("Posted:");
            $("#textBasedOn2").html("Unposted:");
            $("#textBasedOn3").html("Rejected:");

        }
    })

    self.Exit = function () {
        
        self.SelectedDataType('')   
        self.ConsolidateDate('')
        self.ConsolidateTo('')
        self.HolderNoFrom('')
        self.HolderNoTo('')
        
         
        //$("#CertDetails,#DPList").val('').trigger('change')

    }
   
    self.Validation = function (data) {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company <br/>";
        }

        if (Validate.empty(self.HolderNoFrom())) {
            errMsg += "Please Select Holder No From <br/>"
        }
        if (Validate.empty(self.HolderNoTo())) {
            errMsg += "Please Select Holder No To <br/>"
        }
        if (Validate.empty(self.ConsolidateDate())) {
            errMsg += "Please Select Cert No From <br/>"
        }
        if (Validate.empty(self.ConsolidateTo())) {
            errMsg += "Please Select Cert No To <br/>"
        }
        if (errMsg == "") {
            return true;
        } else {
            alert('warning', errMsg)
            return false
        }
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
    ko.applyBindings(new ConsolidateReport())
})