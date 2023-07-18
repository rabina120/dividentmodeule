var CompanySetup = function () {

    self.Compcode = ko.observable()
    self.CompEngName = ko.observable()
    self.ComEngAd1 = ko.observable()
    self.CompEngAd2 = ko.observable()
    self.CompNpName = ko.observable()
    self.CompNpAd1 = ko.observable()
    self.CompNpAd2 = ko.observable()
    self.TelNo = ko.observable()
    self.PostBox = ko.observable()
    self.Email = ko.observable()
    self.CompEngAd2 = ko.observable()
    self.MaxKitta = ko.observable()
    self.PerShare = ko.observable()
    self.FirstCallValue = ko.observable()
    self.FirstCallDate = ko.observable()
    self.SecondCalValue = ko.observable()
    self.SecondCallDate = ko.observable()
    self.ThirdCallValue = ko.observable()
    self.ThirdCallDate = ko.observable()
    self.SoftwareDate = ko.observable()
    self.SoftwareNo = ko.observable()
    self.AgentName = ko.observable()
    self.SignatureDirect = ko.observable()
    self.SoldCode = ko.observable()
    self.AGMNo = ko.observable()

    self.ActionType = ko.observable()

    self.ChooseOptions = function (data) {
        self.ActionType(data)
        if (data == 'A') {
            $('#compcode').attr('disabled', true)
        } else {
            $('#compcode').attr('disabled', false)
            $('#compcode').focus()
            $('#SaveButton').attr('value', 'Update')
        }
        $('#SaveButton').attr('disabled', false)
        $('#AddButton,#UpdateButton').attr('disabled', 'disabled')
    }



    self.Validation = function () {
        var errMsg = ""
        if (Validate.empty(self.ActionType())) {
            errMsg += "Please Choose Action Type <br/>"
        }
        if (Validate.empty(self.Compcode())) {
            errMsg += "Please Enter Company Code  <br/>"
        }
        if (Validate.empty(self.CompEngName())) {
            errMsg += "Please Enter Company English Name <br/>"
        }
        if (Validate.empty(self.ComEngAd1())) {
            errMsg += "Please Enter Company English Address 1  <br/>"
        }
        if (Validate.empty(self.TelNo())) {
            errMsg += "Please Enter Telephone No  <br/>"
        }
        if (Validate.empty(self.PostBox())) {
            errMsg += "Please Enter Post Box  <br/>"
        }
        if (Validate.empty(self.MaxKitta())) {
            errMsg += "Please Enter Maximum Kitta  <br/>"
        }
        if (Validate.empty(self.PerShare())) {
            errMsg += "Please Enter PerShare Value  <br/>"
        }
        if (Validate.empty(self.FirstCallValue())) {
            errMsg += "Please Enter First Call Value  <br/>"
        }
        if (Validate.empty(self.FirstCallDate())) {
            errMsg += "Please Enter First Call Date  <br/>"
        }
        if (Validate.empty(self.SecondCalValue())) {
            errMsg += "Please Enter Second Call Value  <br/>"
        }
        if (Validate.empty(self.SecondCallDate())) {
            errMsg += "Please Enter Second Call Date  <br/>"
        }
        if (Validate.empty(self.ThirdCallValue())) {
            errMsg += "Please Enter Third Call Value  <br/>"
        }
        if (Validate.empty(self.ThirdCallDate())) {
            errMsg += "Please Enter Third Call Date  <br/>"
        }
        if (Validate.empty(self.SoftwareDate())) {
            errMsg += "Please Enter Software Date  <br/>"
        }
        if (Validate.empty(self.SoftwareNo())) {
            errMsg += "Please Enter Software Numeber  <br/>"
        }
        if (Validate.empty(self.AgentName())) {
            errMsg += "Please Enter Agent Name  <br/>"
        }
        if (Validate.empty(self.SoldCode())) {
            errMsg += "Please Enter Sold Code  <br/>"
        }
        if (Validate.empty(self.AGMNo())) {
            errMsg += "Please Enter AGM Number  <br/>"
        }
        if (errMsg == "") {
            return true;
        } else {
            alert("error", errMsg)
            return false;
        }
    }
    self.GetCompanyDetails = function (data) {
        if (self.ActionType() == 'U') {
            Openloader()
            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/CompanySetup/GetCompanyDetails?CompCode=' + `${ko.toJS(data)}`,
                dataType: 'json',
                success: function (result) {
                    if (result.isSuccess) {
                        if (result.responseData != null) {
                            self.CompEngName(result.responseData.compEnName)
                            self.ComEngAd1(result.responseData.compEnAdd1)

                            self.CompEngAd2(result.responseData.compEnAdd2)
                            self.CompNpName(result.responseData.compNpName)
                            self.CompNpAd1(result.responseData.compNpAdd1)
                            self.CompNpAd2(result.responseData.compNpAdd2)
                            self.TelNo(result.responseData.telNo)
                            self.PostBox(result.responseData.pBoxNo)
                            self.Email(result.responseData.email)
                            self.MaxKitta(result.responseData.maxKitta)
                            self.PerShare(result.responseData.perShVal)
                            self.FirstCallValue(result.responseData.fstCallVal)
                            self.FirstCallDate(convertDate(result.responseData.fstCallDt))
                            self.SecondCalValue(result.responseData.sndCallVal)

                            self.SecondCallDate(convertDate(result.responseData.sndCallDt))
                            self.ThirdCallValue(result.responseData.trdCallVal)
                            self.ThirdCallDate(convertDate(result.responseData.trdCallDt))
                            self.SoftwareDate(convertDate(result.responseData.softInstDt))
                            self.SoftwareNo(result.responseData.softInstNo)
                            self.AgentName(result.responseData.agentName)
                            self.SignatureDirect(result.responseData.signDir)
                            self.SoldCode(result.responseData.soldSoftCode)
                            self.AGMNo(result.responseData.curAgmNo)
                        }
                    } else {
                        alert('error', 'Failed to Get Company Code')
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: () => {
                    Closeloader()
                }
            })
        } else {
            alert('warning', 'Please Choose Action Type')
        }
    }




    self.SaveCompanyDetails = function () {
        if (self.Validation()) {
            var company = {
                compcode: self.Compcode(),
                compEnName: self.CompEngName(),
                compEnAdd1: self.ComEngAd1(),
                compEnAdd2: self.CompEngAd2(),
                compNpName: self.CompNpName(),
                compNpAdd1: self.CompNpAd1(),
                compNpAdd2: self.CompNpAd2(),
                telNo: self.TelNo(),
                pBoxNo: self.PostBox(),
                email: self.Email(),
                maxKitta: self.MaxKitta(),
                perShVal: self.PerShare(),
                fstCallVal: self.FirstCallValue(),
                fstCallDt: self.FirstCallDate(),
                sndCallVal: self.SecondCalValue(),
                sndCallDt: self.SecondCallDate(),
                trdCallVal: self.ThirdCallValue(),
                trdCallDt: self.ThirdCallDate(),
                softInstDt: self.SoftwareDate(),
                softInstNo: self.SoftwareNo(),
                agentName: self.AgentName(),
                signDir: self.SignatureDirect(),
                soldSoftCode  : self.SoldCode(),
                curAgmNo: self.AGMNo(),
            }
            swal({
                title: "Are you sure?",
                text: self.ActionType() == "U" ? "Once Updated, you will not be able to recover" : "Once Saved, you will not be able to recover" ,
                icon: "warning",
                buttons: true,
                dangerMode: true
            }).then((willDelete) => {
                if (willDelete) {

                    Openloader()

                    $.ajax({
                        type: 'POST',
                        url: '/ParameterSetup/CompanySetup/SaveCompanyDetails',
                        data: { 'aTTCompanySetup': ko.toJS(company), 'ActionType': ko.toJS(self.ActionType()) },
                        dataType: 'JSON', beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: (result) => {
                            if (result.isSuccess) {
                                alert('success', result.message)
                            }
                            else {
                                alert('error', result.message)
                                console.log('error=>', result.message)
                            }
                            self.ClearControl()
                        },
                        error: (error) => {
                            alert('error', error.message)
                        },
                        complete: () => {
                            Closeloader()
                        }
                    })
                   
                }
            });
        }
    }

    self.ClearControl = function () {
        $('#compcode, #AddButton, #UpdateButton').attr('disabled', false)
        self.Compcode('')
        self.CompEngName('')
        self.ComEngAd1('')
        self.CompEngAd2('')
        self.CompNpName('')
        self.CompNpAd1('')
        self.CompNpAd2('')
        self.TelNo('')
        self.PostBox('')
        self.Email('')
        self.MaxKitta('')
        self.PerShare('')
        self.FirstCallValue('')
        self.FirstCallDate(convertDate(''))
        self.SecondCalValue('')
        self.SecondCallDate(convertDate(''))
        self.ThirdCallValue('')
        self.ThirdCallDate(convertDate(''))
        self.SoftwareDate(convertDate(''))
        self.SoftwareNo('')
        self.AgentName('')
        self.SignatureDirect('')
        self.SoldCode('')
        self.AGMNo('')
        $('#SaveButton').attr('value', 'Save')
        $('#SaveButton').attr('disabled', 'disabled')

    }
}
$(document).ready(function () {
    ko.applyBindings(new CompanySetup());

});
