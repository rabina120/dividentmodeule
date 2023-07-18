
var BrokerSetup = function () {
    self.BrokerId = ko.observable()
    self.BrokerName = ko.observable()
    self.BrokerAdd = ko.observable()
    self.BrokerNpName = ko.observable()
    self.BrokerNpAdd = ko.observable()
    self.BrokerCont = ko.observable()
    self.BrokerTelNo = ko.observable()
    self.ActionType = ko.observable()

    self.ChooseOptions = function (data) {
        self.ActionType(data)
        if (data == 'A') {
            $('#BrokerId').attr('disabled','disabled')
        } else {
            $('#SaveButton').attr('value','Update')
        }
        $('#AddButton,#EditButton, #DeleteButton').attr('disabled','disabled')
    }

    self.Validation = function () {
        var errMsg = ""

        if (Validate.empty(self.ActionType())) {
            errMsg += 'Please Choose Action Type <br/>'
        }
        if (Validate.empty(self.ActionType() != "A")){
            if (Validate.empty(self.BrokerId())) {
                errMsg += 'Please Enter Broker ID <br/>'
            }
        }
        if (Validate.empty(self.BrokerName())) {
            errMsg += 'Please Enter Broker Name <br/>'
        }

        if (Validate.empty(self.BrokerAdd())) {
            errMsg += 'Please Enter Broker Address <br/>'
        }
        if (Validate.empty(self.BrokerTelNo())) {
            errMsg += 'Please Enter Broker Tel No<br/>'
        }
        if (errMsg == "") {
            return true;
        } else {
            alert('error', errMsg)
            return false
        }

    }
    self.GetBrokerCode = function () {
        $.ajax({
            type: 'POST',
            url: '/ParameterSetup/BrokerSetup/GetBrokerCode',
            dataType: 'JSON', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (result) => {
                if (result.isSuccess) {
                    $('#BrokerId').attr('disabled', 'disabled')
                    self.BrokerId(result.message)

                } else {
                    alert('warning', 'Failed to Get New Broker Code')
                }
            },
            error: (error) => {
                alert('error', result.message)
            }
        })
    }


    self.GetBrokerDetails = function (data) {
        if (!Validate.empty(self.ActionType())) {
            Openloader()
            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/BrokerSetup/GetBrokerDetail',
                data: { 'Bcode': data },
                dataType: 'JSON', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        $('#BrokerId, #BrokerName').attr('disabled', 'disabled')
                        self.BrokerId(result.responseData.bcode)
                        self.BrokerName(result.responseData.name)
                        self.BrokerAdd(result.responseData.address)
                        self.BrokerNpName(result.responseData.npname)
                        self.BrokerNpAdd(result.responseData.npadd)
                        self.BrokerCont(result.responseData.contactperson1)
                        self.BrokerTelNo(result.responseData.telno)


                    } else {
                        alert('warning', result.message)
                    }
                },
                error: (error) => {
                    alert('error', result.message)
                },
                complete: () => {
                    Closeloader()
                }
            })
        } else {
            alert('error', 'Please Choose Action Type')
        }
    }

    self.SaveBrokerDetails = function () {
        if (self.Validation()) {
            Openloader()

            var Broker = {
                bcode: self.BrokerId(),
                name: self.BrokerName(),
                address: self.BrokerAdd(),
                npname: self.BrokerNpName(),
                npadd: self.BrokerNpAdd(),
                contactperson1: self.BrokerCont(),
                telno: self.BrokerTelNo(),
            }
            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/BrokerSetup/SaveBrokerDetails',
                data: { 'aTTBroker': Broker, 'ActionType': self.ActionType() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        alert('success', result.message)
                    } else {
                        alert('warning', result.message)
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
        $('#AddButton,#EditButton,#DeleteButton, #BrokerId, #BrokerName').attr('disabled', false)
        $('#SaveButton').attr('value', 'Save')
        self.BrokerId('')
        self.BrokerName('')
        self.BrokerAdd('')
        self.BrokerNpName('')
        self.BrokerNpAdd('')
        self.BrokerCont('')
        self.BrokerTelNo('')
        self.ActionType('')
    }
}




$(document).ready(function () {
    ko.applyBindings(new BrokerSetup());

});
