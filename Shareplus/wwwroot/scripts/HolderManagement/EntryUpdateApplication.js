function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}
var EntryUpdateApplication = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()

    //
    self.ShHolderNo = ko.observable()
    self.TotalKitta = ko.observable()
    self.ShHolderName = ko.observable()
    self.FatherName = ko.observable()
    self.GrandFatherName = ko.observable()
    self.HusbandName = ko.observable()
    self.Address = ko.observable()
    self.ApplicationNo = ko.observable()
    self.ApplicationDate = ko.observable()
    self.Action = ko.observable()






    var optionAUD = "";
    //Validation 
    self.Validation = function () {

        var errMsg = "";
        if (Validate.empty(self.ShHolderNo())) {
            errMsg += "Enter A ShHolderNo First .<br/>"
        }
        if (Validate.empty(self.Action())) {
            errMsg += "Enter An Action First .<br/>"
        }
        if (optionAUD != "A") {
            if (Validate.empty(self.ApplicationNo())) {
                errMsg += "Enter A Application No First .<br/>"
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


    //for choosing add or update or delete
    self.chooseOptions = function (data) {
        optionAUD = data;
        $('#AddButton,#UpdateButton,#DeleteButton').attr('disabled',true)
        if (optionAUD === "A") {
            $('#ShHolderNo,#btnShHolderList').attr('disabled', false)
            $('#ShHolderNo').focus()
        } else {
            $('#ShHolderNo,#btnShHolderList').attr('disabled', true)
            $('#ApplicationNo,#btnApplicationList').attr('disabled', false)
            $('#ApplicationNo').focus()
            if (optionAUD === "U") {
            }
            
        }
    }

    //shholder information
    self.GetSholderInformation = function (data) {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.ShHolderNo())) {
                Openloader()
                var companyCode = self.SelectedCompany()
                $.ajax({
                    type: "post",
                    url: '/HolderManagement/UpdateApplicationEntry/GetInformationForApplication',
                    data: {
                        'CompCode': companyCode,
                        'SelectedAction': optionAUD,
                        'ShHolderNo': self.ShHolderNo(),
                        'ApplicationNo': self.ApplicationNo()
                    },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            self.TotalKitta(result.responseData.totalKitta);
                            self.ShHolderName(result.responseData.fName + ' '+result.responseData.lName);
                            self.FatherName(result.responseData.faName);
                            self.GrandFatherName(result.responseData.grFaName);
                            self.HusbandName(result.responseData.husbandName);
                            self.Address(result.responseData.address);
                            $('#Action,#ApplicationDate,#SaveApplication').attr('disabled', false)
                            self.ApplicationDate(formatDate(new Date))
                            $('#Action').focus()

                        } else {
                            alert('warning', result.message)
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                    }, complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }
    //information using application no
    self.GetApplicationInformation = function (data) {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.ApplicationNo())) {
                Openloader()
                var companyCode = self.SelectedCompany()
                $.ajax({
                    type: "post", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/HolderManagement/UpdateApplicationEntry/GetInformationFromApplicationNo',
                    data: {
                        'CompCode': companyCode,
                        'SelectedAction': optionAUD,
                        'ApplicationNo': self.ApplicationNo()
                    },
                    datatype: "json",
                    success: function (result) {
                        if (result.isSuccess) {
                            self.TotalKitta(result.responseData.totalKitta);
                            self.ShHolderName(result.responseData.fName + ' ' + result.responseData.lName);
                            self.FatherName(result.responseData.faName);
                            self.GrandFatherName(result.responseData.grFaName);
                            self.HusbandName(result.responseData.husbandName);
                            self.Address(result.responseData.address);
                            $('#Action,#ApplicationDate,#SaveApplication').attr('disabled', false)
                            self.ApplicationDate(result.responseData.applicationDate == null ? "" : result.responseData.applicationDate.substring(0,10))
                            self.Action(result.responseData.action)
                            self.ShHolderNo(result.responseData.shholderNo)
                           

                        } else {
                            alert('warning', result.message)
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                    }, complete: () => {
                       
                        Closeloader()
                    }
                })
            }
        }
    }
    self.SaveApplication = function () {
        if (self.ValidateCompany()) {
            if (self.Validation()){
                Openloader()
                var companyCode = self.SelectedCompany()
                $.ajax({
                    type: "POST", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/HolderManagement/UpdateApplicationEntry/SaveApplicationForUpdate',
                    data: {
                        'CompCode': companyCode,
                        'SelectedAction': optionAUD,
                        'ShHolderNo': self.ShHolderNo(),
                        'ApplicationNo': self.ApplicationNo(),
                        'ApplicationDate': self.ApplicationDate(),
                        'Action': self.Action()

                    },
                    datatype: "json",
                    success: function (result) {
                        if (result.isSuccess) {
                            alert('success',result.message)

                        } else {
                            alert('error', result.message)
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                    }, complete: () => {
                        self.Refresh()
                        Closeloader()
                    }
                })
            }
        }
    }
    self.Refresh = function () {
        self.ShHolderNo('')
        self.TotalKitta('')
        self.ShHolderName('')
        self.FatherName('')
        self.GrandFatherName('')
        self.HusbandName('')
        self.Address('')
        self.ApplicationNo('')
        self.ApplicationDate('')
        self.Action('')
        $('#SaveApplication,#ShHolderNo,#btnShHolderList,#ApplicationDate,#Action,#btnApplicationList,#ApplicationNo').attr('disabled',true)
        $('#AddButton,#UpdateButton,#DeleteButton').attr('disabled',false)
    }
}

$(document).ready(function () {
    ko.applyBindings(new EntryUpdateApplication());
    self.Refresh();
    $('#btnShHolderList,#btnApplicationList').hide();
});