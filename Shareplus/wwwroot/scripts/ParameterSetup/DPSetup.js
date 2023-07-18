function DPDetail(data) {
    var self = this;
    if (data != undefined) {
        self.DPCode = ko.observable(data.dP_CODE);
        self.DPName = ko.observable(data.dP_NAME);
        self.CDSDPID = ko.observable(data.dp_Id_cds);
    }
}
var DPSetup = function () {
    self.DPCode = ko.observable();
    self.DPName = ko.observable();
    self.CDSDPID = ko.observable();
    self.ActionType = ko.observable();
    self.DP_Detail_List = ko.observableArray([]);

    self.ChooseOptions = function (data) {
        self.ActionType(data)
        if (data == 'A') {
            $('#DPCode').attr('disabled', true)
        } else {
            $('#SaveButton').attr('value', 'Update')
        }
        $('#AddButton,#EditButton').attr('disabled', 'disabled')

        $('#SearchButton,#SaveButton').attr('disabled', false)
    }
    self.Validation = function () {
        var errMsg = ''
        if (self.ActionType() != "A") {
            if (Validate.empty(self.DPCode())) {
                errMsg += 'Please Enter DPCOde <br/>'
            }
        }
        if (Validate.empty(self.DPName())) {
            errMsg += 'Please Enter DP Name<br/>'
        }
        if (Validate.empty(self.CDSDPID())) {
            errMsg += 'Please Enter DP ID CDS <br/>'
        } else if (self.CDSDPID().length != 8) {
            errMsg += 'DP ID CDS  Length Must Be 8<br/>'
        }
        if (errMsg != '') {
            alert('error', errMsg)
            return false
        } else {
            return true
        }
    }


    self.GetDPDetail = function (data) {
        if (!Validate.empty(self.ActionType()) && self.ActionType() == 'U') {
            $('#DPDetailList').modal('hide')
            Openloader()

            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/DPSetup/GetDPDetails',
                data: { 'DPCode': data },
                dataType: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        //$('#tbl_Charge').modal('show')
                        $('#DPCode').attr('disabled', true)
                        self.DPCode(result.responseData.dP_CODE)
                        self.DPName(result.responseData.dP_NAME)
                        self.CDSDPID(result.responseData.dp_Id_cds)
                    }
                    else {
                        alert('warning', 'NO such DP Code Registered');
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
        else {
            alert('error', 'Please Choose Action Type')
        }
    }
    self.SaveDPDetails = function () {
        if (self.Validation()) {
            Openloader()

            var DpSetup = {
                DP_Code: self.DPCode(),
                DP_Name: self.DPName(),
                Dp_Id_cds: self.CDSDPID()
            }
            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/DPSetup/SaveDPDetails',
                data: { 'aTTDPSetup': DpSetup, 'ActionType': self.ActionType() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        alert('success', result.message)
                    } else {
                        alert('info', 'Failed to Make Changes')
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
        $(' #AddButton, #EditButton,#DPCode').attr('disabled', false)
        //self.SelectedCompany('')
        self.DPCode('')
        self.DPName('')
        self.CDSDPID('')
        self.ActionType('')
        $(' #SaveButton, #SearchButton').attr('disabled', 'disabled')

        $('#SaveButton').attr('value', 'Save')
    }

    self.LoadDPDetailList = function () {
        if (self.ActionType() == 'U') {
            Openloader()

            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/DPSetup/LoadDPDetailList',
                datatype: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        if (result.responseData.length > 0) {
                            $('#DPDetailList').modal('show')
                            var mappedTask = $.map(result.responseData, function (item) {
                                return new DPDetail(item)
                            });
                            self.DP_Detail_List(mappedTask);
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
        else {
            alert('error', 'Please choose edit option.')
        }
    }

}

$(document).ready(function () {
    ko.applyBindings(new DPSetup())
});