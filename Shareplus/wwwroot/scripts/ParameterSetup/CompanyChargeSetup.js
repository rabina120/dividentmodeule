//const { observable } = require("knockout");

function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function Charges(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.ChargeCode = ko.observable(data.chargeCode)
        self.ChargeType = ko.observable(data.charge_Desc)
        self.Charge = ko.observable(data.charge)
    }
}

var CompanyChargeSetup = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();


    self.ChargeType = ko.observable()
    self.Charge = ko.observable()
    self.ChargeCode = ko.observable()
    self.ActionType = ko.observable()
    self.ChargeList = ko.observable([])

    self.LoadCompany = function () {
        var companyCode = localStorage.getItem('company-code')
        $.ajax({
            url: '/Common/Company/GetCompanyDetails',
            type: "post",
            data: { 'CompCode': companyCode },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (result) => {
                if (result.isSuccess) {
                    var mappedTask = $.map(result.responseData, (item) => {
                        return new ParaComp(item)
                    })
                    self.CompanyDetails(mappedTask)
                    if (!Validate.empty(localStorage.getItem('company-code'))) { self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode()); }
                }
                console.log(result)
            },
            error: (error) => {
                alert('error', error.messsage)
            }
        })
    }
    self.LoadCompany()

    self.ChooseOptions = function (data) {
        self.ActionType(data)
        if (data == 'A') {
            $('#ChargeCode').attr('disabled', 'disabled')
        } else {
            $('#SaveButton').attr('value', 'Update')
        }
        $('#AddButton,#EditButton').attr('disabled', 'disabled')
    }

    self.Validation = function () {
        var errMsg = ''
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += 'Please Choose Company <br/>'
        }
        if (self.ActionType() != "A") {
            if (Validate.empty(self.ChargeCode())) {
                errMsg += 'Please Enter Charge Code <br/>'
            }
        }
        if (Validate.empty(self.ChargeType())){
            errMsg += 'Please Enter Charge Type <br/>'
        }
        if (errMsg != '') {
            alert('error', errMsg)
            return false
        } else {
            return true
        }
    }

    self.SaveCompanyCharge = function () {
        if (self.Validation()) {
            Openloader()

            var CompanyCharge = {
                CompCode : self.SelectedCompany(),
                ChargeCode : self.ChargeCode(),
                Charge_Desc : self.ChargeType(),
                charge :self.Charge()
            }
            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/CompanyChargeSetup/SaveCompanyCharge',
                data: { 'aTTCompanyCharge': CompanyCharge, 'ActionType': self.ActionType() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.hasError) {
                        alert('error', result.message)

                    } else {
                        if (result.isSuccess) {
                            alert('success', result.message)
                        } else {
                            alert('warning', result.message)
                        }
                    }
                    
                },
                error: (error) => {
                    alert('error', error.message)
                },
                complete: () => {
                    Closeloader()
                }
            })
            self.ClearControl()
        }

    }

    self.ClearControl = function () {
        $(' #ChargeCode, #AddButton, #EditButton').attr('disabled', false)
        //self.SelectedCompany('')
        self.ChargeCode('')
        self.ChargeType('')
        self.Charge('')
        self.ChargeList('')
        self.ActionType('')
    }  
}
 
self.GetCompanyCharge = function (data) {
    if (!Validate.empty(self.ActionType()) && self.ActionType()=='U') {
        /*$('#ModalOccupationInformation').modal('hide')*/
        Openloader()
        $.ajax({
            type: 'POST',
            url: '/ParameterSetup/CompanyChargeSetup/GetCompanyCharge',
            data: { 'Compcode': ko.toJS(self.SelectedCompany()), 'ChargeCode': ko.toJS(data)},
            datatype: 'json', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (result) => {
                if (result.hasError) {
                    alert('error', result.message)

                } else {
                    if (result.isSuccess) {
                        $('#ChargeCode, #Company').attr('disabled', 'disabled')
                        self.ChargeCode(result.responseData.chargeCode)
                        self.ChargeType(result.responseData.charge_Desc)
                        self.Charge(result.responseData.charge)

                    } else {
                        alert('warning', result.message)
                    }
                }
                
            },
            error: (error) => {
                alert('error', error.message)
            },
            complete: () => {
                Closeloader()
            }
        })
    } else {
        alert('error', 'Please Choose Action Type')
    }
}

self.GetCompanyChargeDetails = function () {
    if (!Validate.empty(self.ActionType()) && self.ActionType() == 'U') {

        Openloader()

        $.ajax({
            type: 'POST',
            url: '/ParameterSetup/CompanyChargeSetup/GetCompanyChargeDetail',
            data: { 'Compcode': ko.toJS(self.SelectedCompany()) },
            dataType: 'json', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (result) => {
                if (result.hasError) {
                    alert('error', result.message)

                } else {
                    if (result.isSuccess) {
                        if (result.responseData.length > 0) {
                            $('#Company').attr('disabled', true)
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new Charges(item);
                            });

                            self.ChargeList(mappedTasks);
                        }
                        else {
                            alert('warning', result.message)
                        }
                    }
                }
            },
            error: (error) => {
                alert('error', error.message)
            },
            complete: () => {
                Closeloader()
            }
        })
    }
    else if (Validate.empty(self.ActionType())) {
        alert('error', 'Please Choose Action Type')
    }
}
$(document).ready(function () {
    ko.applyBindings(new CompanyChargeSetup())
});
