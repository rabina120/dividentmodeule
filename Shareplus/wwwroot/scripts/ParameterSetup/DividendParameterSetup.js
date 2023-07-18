function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}
var DividendParameterSetUp = function () {

    self.scheme_code = ko.observable();
    self.SchemeEnName = ko.observable();


    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()

    // dividend
    self.Divcode = ko.observable();
    self.AgmNo = ko.observable();
    self.FiscalYr = ko.observable();
    self.taxper = ko.observable();
    self.BonusShPer = ko.observable();
    self.DDeclareDt = ko.observable();
    self.DivType = ko.observable();
    self.ActionType = ko.observable()

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


    self.ChooseOption = (data) => {
        self.ActionType(data)
        
        if (data == 'A') {
            $('#Add, #Delete').attr('disabled', 'disabled')
            $('#Agm, #FiscalYear, #DivPer, #Tax, #Date, #Save, .custom-control-input').attr('disabled', false)
        } else {
            $('#DivCode,#Delete').attr('disabled', false)
            $('#Add,#Agm, #FiscalYear, #DivPer, #Tax, #Date').attr('disabled', 'disabled')
        }
    }
    self.Validation = function () {
        var errMsg = "";
        if (self.ActionType() != "A") {
            if (Validate.empty(self.Divcode())) {
                errMsg += "Please Enter Dividend Code <br>"
            }
        }
        if (Validate.empty(self.AgmNo())) {
            errMsg += "Please Enter AGM No <br>"
        }
        if (Validate.empty(self.FiscalYr())) {
            errMsg += "Please Enter Fiscal Year <br>"
        }
        if (Validate.empty(self.taxper())) {
            errMsg += "Please Enter Tax Percent <br>"
        } else {
            if (self.taxper() > 100) {
                errMsg += "Please Enter Valid Tax Percent<br>"
            }
        }
        if (Validate.empty(self.BonusShPer())) {
            errMsg += "Please Enter Bonus Percent <br>"
        } else {
            if (self.BonusShPer() > 100) {
                errMsg += "Please Enter Valid Bonus Percent <br>"
            }
        }
        if (Validate.empty(self.DDeclareDt())) {
            errMsg += "Please Enter Declare Date <br>"
        }
        if (Validate.empty(self.DivType())){
            errMsg += "Please Choose Dividend Type <br>"
        }
        if (errMsg != "") {
            alert("error", errMsg)
            return false
        } else {
            return true
        }
    }

    self.GetDividivendDetails = function () {
        if (!Validate.empty(self.Divcode())) {
            Openloader()
            $.ajax({
                type: "post",
                url: "/ParameterSetup/DividendSetUp/GetDividivendDetails",
                data: { 'CompCode': localStorage.getItem("company-code"), 'DivCode': self.Divcode() },
                dataType: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (data) {
                    if (data.isSuccess) {
                        if (data.responseData != null) {
                            self.Divcode(data.responseData.divcode)
                            self.AgmNo(data.responseData.agmNo)
                            self.FiscalYr(data.responseData.fiscalYr)
                            self.taxper(data.responseData.taxper)
                            self.BonusShPer(data.responseData.bonusShPer)

                            self.DDeclareDt(convertDate(data.responseData.dDeclareDt))
                            self.DivType(String(data.responseData.divType))
                            $('.custom-control-input').attr('disabled', true)
                            self.DeleteParameterSetup()

                        } else {
                            alert("warning", "Record Not Found")
                            self.Divcode('')
                            self.ClearControl()
                            console.log("error => ", data.message)
                        }
                    }
                    else {
                        self.Divcode('')
                        alert("error", data.message)
                        self.ClearControl()
                    }
                },
                error: function (error) {
                    alert("error", error.message)
                },
                complete: () => {
                    Closeloader()
                }
            })
        } else {
            alert("error", "Please Enter Dividend Code")
        }
    }

    //self.SaveRefresh = () => {
    //    if (self.ActionType() == 'A') {
    //        self.SaveDividendParameter()
    //    } else if (self.ActionType() == 'D') {
    //        self.DeleteParameterSetup()
    //    }
    //}

    self.DeleteParameterSetup = function () {

        swal({
            title: "Are you sure?",
            text: `${self.Divcode()} will be Deleted, Once Deleted can not be able to recover`,
            icon: "warning",
            buttons: true,
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                Openloader()
                $.ajax({
                    type: "post",
                    url: "/ParameterSetup/DividendSetUp/DeleteParameterSetup",
                    data: { 'CompCode': localStorage.getItem("company-code"), 'DivCode': self.Divcode(), 'ActionType': self.DivType() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (data) {
                        if (data.isSuccess) {
                            alert('success', data.message)
                            self.ClearControl()
                        }
                        else {
                            alert('error', data.message)
                            self.ClearControl()
                            console.log("error => ", data.message)
                        }
                    },
                    error: (error) => {
                        alert('error', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                });
            }
        });


    }

    //self.GetDividendCode = function () {
    //    if (localStorage.getItem("company-code").length > 0) {
    //        Openloader()
    //        $.ajax({
    //            type: "post",
    //            url: "/DividendManagement/DividendSetUp/GetDividendCode",
    //            data: { 'CompCode': localStorage.getItem("company-code") },
    //            dataType: "json",
    //            success: function (data) {
    //                if (data.isSuccess) {
    //                    if (data.responseData > 0) {
    //                        $('#DivCode').attr('disabled', 'true')
    //                        self.Divcode(data.responseData)
    //                        $('#Add, #Delete').attr('disabled', 'true')
    //                        self.DDeclareDt(self.PostingDate())
    //                    } else {
    //                        alert("warning", "Failed to Generate Dividend Code " + data.message)
    //                        console.log("error => ", data.message)
    //                    }
    //                } else {
    //                    alert('error', data.message)
    //                }
    //            },
    //            error: function (error) {
    //                alert("error", error.message)
    //            },
    //            complete: () => {
    //                Closeloader()
    //            }
    //        })
    //    } else {
    //        alert("error", "Please Choose Company")
    //    }
    //}

    self.SaveDividendParameter = function () {

        if (self.Validation()) {
            var data = {
                'Divcode': self.Divcode(),
                'AgmNo': self.AgmNo(),
                'FiscalYr': self.FiscalYr(),
                'taxper': self.taxper(),
                'BonusShPer': self.BonusShPer(),
                'DDeclareDt': self.DDeclareDt(),
                'compcode': self.SelectedCompany()
            }

            $.ajax({
                type: "POST",

                url: "/ParameterSetup/DividendSetUp/SaveParameterSetup",
                data: { 'aTTDividend': data, 'ActionType': self.DivType() },
                dataType: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (data) {
                    if (data.isSuccess) {
                        alert("success", data.message)
                        self.ClearControl()
                    } else {
                        alert('error', data.message)
                        console.log("error => ", data.message)
                    }
                },
                error: function (error) {
                    alert("error", error.message)
                }
            })
        }
    }
    self.ClearControl = function () {
        self.Divcode('')
        self.AgmNo('')
        self.FiscalYr('')
        self.taxper('')
        self.BonusShPer('')
        self.DDeclareDt('')
        self.DivType('')
        self.ActionType('')
        $(' #Add, #Delete').attr('disabled', false)
        $('#DivCode, #Save, #Agm, #FiscalYear, #DivPer, #Tax, #Date').attr('disabled', 'disabled')
        $('.custom-control-input').attr('disabled', 'disabled')
        $('#Save').attr('disabled', 'disabled')

    }
}

$(document).ready(function () {

    ko.applyBindings(new DividendParameterSetUp());

});