
var PaymentSetup = function () {

    self.PCode = ko.observable()
    self.PName = ko.observable()
    self.PAddress = ko.observable()
    self.PNepName = ko.observable()
    self.PNepAdd = ko.observable()
    self.TelNo = ko.observable()
    self.ActionType = ko.observable()

    self.ChooseOptions = function (data) {
        self.ActionType(data)
        if (data == 'A') {
            $('#PaymentCode').attr('disabled', 'disabled')
        } else {
            $('#SaveButton').attr('value', 'Update')
        }
        $('#AddButton,#UpdateButton').attr('disabled', 'disabled')
    }


    self.Validation = function () {
        var errMsg = "";
        if (Validate.empty(self.ActionType())) {
            errMsg += "Please Choose Action Type <br/>"
        }
        if (self.ActionType() != "A") {
            if (Validate.empty(self.PCode())) {
                errMsg += "Please Enter Payment Code <br/>"
            }
        }
        if (Validate.empty(self.PName())) {
            errMsg += "Please Enter Payment  Name <br/>"
        }
        if (Validate.empty(self.PAddress())) {
            errMsg += "Please Enter Payment Address <br/>"
        }

        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
        }

    }

    self.GetPaymentCode = function () {
        $.ajax({
            type: 'POST',
            url: '/ParameterSetup/PaymentSetup/GetPaymentCode',
            dataType: 'JSON', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (result) => {
                if (result.isSuccess) {
                    $('#PaymentCode').attr('disabled', 'disabled')
                    self.PCode(result.message)

                } else {
                    alert('warning', 'Failed to Get New Payment Code')
                }
            },
            error: (error) => {
                alert('error', result.message)
            }
        })
    }

    self.GetPaymentDetails = function (data) {
        if (!Validate.empty(self.ActionType())) {
            Openloader()

            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/PaymentSetup/GetPaymentDetails',
                data: { 'CenterId': ko.toJS(data) },
                dataType: 'JSON', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        $('#PaymentCode, #PName').attr('disabled', 'disabled')
                        self.PName(result.responseData.centerName)
                        self.PAddress(result.responseData.address)
                        self.PNepName(convert_to_unicode_with_value(result.responseData.nepaliName))
                        self.PNepAdd(convert_to_unicode_with_value(result.responseData.nepaliAddress))
                        self.TelNo(result.responseData.telNo)

                    } else {
                        alert('warning', 'Failed to Get Payment Details')
                        self.ClearControl()
                    }
                },
                error: (error) => {
                    alert('error', result.message)
                    self.ClearControl()
                },
                complete: () => {
                    Closeloader()
                }
            })
        } else {
            alert('warning', 'Please Choose Action Type')

        }
    }



    self.SavePaymentDetails = function () {
        if (self.Validation()) {
            Openloader()

            var details = {
                centerId: self.PCode(),
                centerName: self.PName(),
                address: self.PAddress(),
                nepaliName: self.PNepName(),
                nepaliAddress: self.PNepAdd(),
                telNo: self.TelNo()
            }
            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/PaymentSetup/SavePaymentDetails',
                data: { 'aTTPamentType': ko.toJS(details), 'ActionType': self.ActionType() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        alert('success', result.message)
                        self.ClearControl()
                    } else {
                        alert('warning', result.message)
                        self.ClearControl()
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
    }

    self.ClearControl = function () {
        $('#AddButton,#UpdateButton, #PaymentCode, #PName').attr('disabled', false)
        $('#SaveButton').attr('value', 'Save')
        self.PCode('')
        self.PName('')
        self.PAddress('')
        self.PNepName('')
        self.PNepAdd('')
        self.TelNo('')
        self.ActionType('')

    }
}


$(document).ready(function () {
    ko.applyBindings(new PaymentSetup());

});
