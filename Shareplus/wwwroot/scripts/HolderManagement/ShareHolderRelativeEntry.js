function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function ShareHolder(data) {
    var self = this;
    if (data != undefined) {

        self.ShholderNo = ko.observable(data.shholderNo)
        self.TotalKitta = ko.observable(data.totalkitta)
        self.FName = ko.observable(data.fName)
        self.LName = ko.observable(data.lName)
        self.NpName = ko.observable(data.npName == null ? "" : data.npName)
        self.NpAddress = ko.observable(data.npAdd == null ? "" : data.npAdd)
        self.FullName = ko.observable(self.FName + ' ' + self.LName())
        self.Address = ko.observable(self.address)
        self.TelNo = ko.observable(self.telNo)

    }
}

function RelativeShareHolder(data) {
    var self = this;
    if (data != undefined) {
        self.rShHolderNo = ko.observable(data.shholderNo)
        self.rFName = ko.observable(data.fName)
        self.rLName = ko.observable(data.lName)
        self.rFullName = ko.observable(self.rFName() + ' ' + self.rLName())
        self.rFaName = ko.observable(data.faName)
        self.rGrFaName = ko.observable(data.grFaName)
        self.rAddress = ko.observable(data.address)
        self.rTelNo = ko.observable(data.telNo)
        self.SN =ko.observable(data.sn)
    }
}

var ShareHolderRelativeEntry = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()

    self.ShholderNo = ko.observable()
    self.TotalKitta = ko.observable()
    self.FName = ko.observable()
    self.LName = ko.observable()
    self.NpName = ko.observable()
    self.NpAddress = ko.observable()
    self.FullName = ko.observable()
    self.Address = ko.observable()
    self.TelNo = ko.observable()


    self.rShholderNo = ko.observable()
    self.FName = ko.observable()
    self.LName = ko.observable()
    self.rFullName = ko.observable()
    self.rNpName = ko.observable()
    self.rFaName = ko.observable()
    self.rGrFaName = ko.observable()
    self.rAddress = ko.observable()
    self.rNpAddress = ko.observable()
    self.rTelNo = ko.observable()
    self.SN = ko.observable()


    self.SelectedAction = ko.observable()
    self.RelativeShareHolderList =ko.observableArray([])


    //gloabal variables
    var optionAUD = "";

    function formatDate(date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2)
            month = '0' + month;
        if (day.length < 2)
            day = '0' + day;

        return [year, month, day].join('-');
    }

    //Validation 
    self.Validation = function (data = null) {
        var errMsg = '';

        if (Validate.empty(self.SelectedAction()))
            errMsg += "Please Select Add, Update Or Delete !!!"
        if (Validate.empty(self.ShholderNo()))
            errMsg += "Please Enter A ShHolder No !!!"
        if (data == 'R') {
            if (Validate.empty(self.SN()))
                errMsg += "Please Enter a SN !!!"


            if (Validate.empty(self.rShholderNo()))
                errMsg += "Please Enter A Relative ShHolder No !!!"
            else {
                if (self.rShholderNo() == self.ShholderNo())
                    errMsg += "ShHolder and Relative Holder No Cannot be the same !!!"
            }

        }
        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        } else {
            return true;
        }

    }

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


    self.GetSholderInformation = function (data) {
        var shholderNo = data.ShholderNo ? data.ShholderNo : self.ShholderNo()
        if (shholderNo != undefined && shholderNo != "") {
            if (self.Validation('S')) {

                Openloader()
                $.ajax({
                    type: "post",
                    url: '/HolderManagement/ShareHolderRelativeEntry/GetShHolder',
                    data: {
                        'CompCode': ko.toJS(self.SelectedCompany()),
                        'ShHolderNo': shholderNo,
                        'SelectedAction': optionAUD
                    }, beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    datatype: "json",
                    success: function (result) {
                        if (result.isSuccess) {

                            self.FullName(result.responseData.fName + ' ' + result.responseData.lName);
                            self.NpName(result.responseData.npName == null ? "" : result.responseData.npName)
                            self.NpAddress(result.responseData.npAdd == null ? "" : result.responseData.npAdd)
                            self.Address(result.responseData.address);
                            self.TelNo(result.responseData.telNo);
                            self.TotalKitta(result.responseData.totalKitta);

                            if (optionAUD == "A") {
                                Openloader()
                                $.ajax({
                                    type: "post",
                                    url: '/HolderManagement/ShareHolderRelativeEntry/GetMaxSN',
                                    data: {
                                        'CompCode': ko.toJS(self.SelectedCompany()),
                                        'ShHolderNo': shholderNo,
                                        'SelectedAction': optionAUD
                                    },
                                    datatype: "json",
                                    success: function (result) {
                                        self.SN(result.responseData);
                                        $('#rShHolderNo').attr('disabled', false)
                                        $('#rShHolderNo').focus()
                                    },
                                    error: function (error) {
                                        alert('error', error.message)
                                        self.refresh()
                                    },
                                    complete: () => {

                                    }
                                })
                            }
                            else {
                                Openloader()
                                $.ajax({
                                    type: "post",
                                    url: '/HolderManagement/ShareHolderRelativeEntry/GetRelativeShHolderForUpdateDelete',
                                    data: {
                                        'CompCode': ko.toJS(self.SelectedCompany()),
                                        'ShHolderNo': shholderNo,
                                        'SelectedAction': optionAUD
                                    },
                                    datatype: "json", beforeSend: function (xhr) {
                                        xhr.setRequestHeader("XSRF-TOKEN",
                                            $('input:hidden[name="__RequestVerificationToken"]').val());
                                    },
                                    success: function (result) {
                                        var mappedTasks = $.map(result.responseData, function (item) {
                                            return new RelativeShareHolder(item)
                                        });
                                        self.RelativeShareHolderList(mappedTasks);
                                        $('#HoldersList').modal('show');
                                    },
                                    error: function (error) {
                                        alert('error', error.message)
                                        self.refresh()
                                    },
                                    complete: () => {
                                        Closeloader()
                                    }
                                })
                            }
                           

                        } else {
                            alert('warning', result.message)
                            self.refresh()
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                        self.refresh()
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            }
        }


    }

    self.FillSNForDetail = function (data) {
        if (data != undefined) {

            self.rShholderNo(data.rShHolderNo())
            self.rFullName(data.rFName() + ' '+data.rLName())
            self.rNpName('')
            self.rFaName(data.rFaName())
            self.rGrFaName(data.rGrFaName())
            self.rAddress(data.rAddress())
            self.rNpAddress('')
            self.rTelNo(data.rTelNo())
            self.SN(data.SN())
        }
        if (optionAUD == "U") {
            $('#rShHolderNo').attr('disabled', false);
        }
        else {
            $('#rShHolderNo').attr('disabled', true)
        }
        $('#HoldersList').modal('hide');
        $("#saveShholder,#reportButton").attr("disabled", false);
    }

    self.GetRelativeSholderInformation = function (data) {
        if (optionAUD == "A" || optionAUD == "U") {

            var rshholderNo = data.rShholderNo ? data.rShholderNo : self.rShholderNo()
            if (rshholderNo != undefined && rshholderNo != "") {
                if (self.Validation('R')) {

                    Openloader()

                    $.ajax({
                        type: "post",
                        url: '/HolderManagement/ShareHolderRelativeEntry/GetRelativeShHolder',
                        data: {
                            'CompCode': ko.toJS(self.SelectedCompany()),
                            'ShHolderNo': rshholderNo,
                            'SelectedAction': optionAUD
                        },
                        datatype: "json", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (result) {
                            if (result.isSuccess) {
                                $("#saveShholder,#reportButton").attr("disabled", false);
                                self.rFullName(result.responseData.fName + ' ' + result.responseData.lName);
                                self.rNpName(result.responseData.npName ?? result.responseData.npName);
                                self.rFaName(result.responseData.faName)
                                self.rGrFaName(result.responseData.grFaName)
                                self.rAddress(result.responseData.address)
                                self.rNpAddress(result.responseData.npAdd ?? result.responseData.npAdd);
                                self.rTelNo(result.responseData.telNo)
                            } else {
                                alert('warning', result.message)
                                $('#rShHolderNo').focus()
                            }
                        },
                        error: function (error) {
                            alert('error', error.message)
                            $('#rShHolderNo').focus()

                        },
                        complete: () => {
                            Closeloader()

                        }
                    })
                }
            }
        }
    }


    //for choosing add or update or delete
    self.chooseOptions = function (data) {
        if (self.ValidateCompany()) {
            optionAUD = data;
            self.SelectedAction(data);
            enableDisableButtons();
            $('#ShHolderNo').attr("disabled", false);


            if (data == "A") {
               
                $('#saveShholder').val("Save");
                $('#saveShholder').removeClass(["btn-danger", "btn-warning"]);
                $('#saveShholder').addClass("btn-success");

            }
            else {
                if (data == "U") {
                    $('#saveShholder').val("Update");
                    $('#saveShholder').removeClass(["btn-danger", "btn-success"]);
                    $('#saveShholder').addClass("btn-warning");
                }
                else {
                    $('#saveShholder').val("Delete");
                    $('#saveShholder').removeClass(["btn-success", "btn-warning"]);
                    $('#saveShholder').addClass("btn-danger");
                }

            }
        }
    }

    self.Save = function () {
        if (self.Validation('R')) {
            if (self.ValidateCompany()) {
                Openloader()
                var shholder = {
                    shholderno : ko.toJS(self.rShholderNo()),
                    fname : ko.toJS(self.rFullName),
                    faname : ko.toJS(self.rFaName),
                    grfaname : ko.toJS(self.rGrFaName),
                    telno : ko.toJS(self.rTelNo),
                    address : ko.toJS(self.rAddress)
                }
                $.ajax({
                    type: "POST",
                    url: '/HolderManagement/ShareHolderRelativeEntry/SaveShHolderRelative',
                    data: {
                        'CompCode': ko.toJS(self.SelectedCompany()),
                        'ShHolderNo': ko.toJS(self.ShholderNo()),
                        'SelectedAction': optionAUD,
                        'relativeShholder': shholder,
                        'SN':ko.toJS(self.SN())
                    },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            alert('success', result.message)
                        } else {
                            alert('warning', result.message)
                            
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                        
                    },
                    complete: () => {
                        Closeloader()
                        self.refresh()
                    }
                })
            }
        }

    }

    self.ClearData = function () {
        self.ShholderNo('')
        self.TotalKitta('')
        self.FullName('')
        self.NpName('')
        self.Address('')
        self.NpAddress('')
        self.TelNo('')
        self.rShholderNo('')
        self.FName('')
        self.LName('')
        self.rFullName('')
        self.rNpName('')
        self.rFaName('')
        self.rGrFaName('')
        self.rAddress('')
        self.rNpAddress('')
        self.rTelNo('')
        self.SN('')
    }
    self.refresh = function (data) {
        self.ButtonOnLoad();
        self.ClearData()
        $("#ShHolderNo").attr("disabled", "disabled");
        $("#rShHolderNo").attr("disabled", "disabled");
    }
    self.report = function (data) {

    }
    self.enableDisableButtons = function () {
        $("#AddButton,#UpdateButton,#DeleteButton").attr("disabled", "disabled");
    }
    self.ButtonOnLoad = function () {
        $("#saveShholder,#reportButton").attr("disabled", true);
        $("#AddButton,#UpdateButton,#DeleteButton").attr("disabled", false);
    }
}

$(document).ready(function () {

    ko.applyBindings(new ShareHolderRelativeEntry());
    self.ButtonOnLoad()
});