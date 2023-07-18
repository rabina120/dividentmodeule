function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}
function ShHolderSignature(data) {
    var self = this;
    if (data != undefined) {
        self.Selected = ko.observable(false)
        self.ShHolderNo = ko.observable(data.shHolderNo)
        self.Name = ko.observable(data.name)
        self.ScannedBy = ko.observable(data.scanedBy)
    }
}


var SignatureApproveUnapprove = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()


    self.SignatureApprovedUnapproved = ko.observable()
    self.ApprovedBy = ko.observable()
    self.ApprovedDate = ko.observable()
    self.PasswordProtected = ko.observable()
    self.UnapprovePasswordProtected = ko.observable()
    self.Password = ko.observable()
    self.ConfirmPassword = ko.observable()


    self.ShHolderNo = ko.observable()
    self.TotalKitta = ko.observable()
    self.ShHolderName = ko.observable()
    self.ShHolderAddress = ko.observable()
    self.ScannedBy = ko.observable()
    self.UApprovedBy = ko.observable()
    self.UApprovedDate = ko.observable()


    self.Signature = ko.observable()

    self.ShHolderSignatureList = ko.observableArray([])

    self.SignatureApprovedUnapproved('A')


    var record = []

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

    self.ApprovedDate(formatDate(new Date))
    self.UApprovedDate(formatDate(new Date))

    self.SelectAll = ko.computed({
        read: () => !self.ShHolderSignatureList().find(x => !x.Selected()),
        write: t => self.ShHolderSignatureList().forEach(x => x.Selected(t)),
    })

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

    //ClearControls
    self.ClearControls = function (data) {
        if (data == "A")
            self.SignatureApprovedUnapproved('A')
        self.ApprovedBy('')
        self.ApprovedDate(formatDate(new Date))
        self.PasswordProtected(false)
        self.UnapprovePasswordProtected(false)
        self.Password('')
        self.ConfirmPassword('')
        self.ShHolderNo('')
        self.TotalKitta('')
        self.ShHolderName('')
        self.ShHolderAddress('')
        self.ScannedBy('')
        self.UApprovedBy('')
        self.UApprovedDate(formatDate(new Date))
        $('#tbl_ShHolderList').DataTable().clear();
        $('#tbl_ShHolderList').DataTable().destroy();
        self.ShHolderSignatureList([])
        record = []
    }
    self.ClearControlsUnApprove = () => {
        self.ShHolderNo('');
        self.ShHolderName('');
        self.TotalKitta('');
        self.ShHolderAddress('');
        self.ScannedBy('');
        self.UApprovedBy('');
        self.UApprovedDate('');
        self.PasswordProtected('');
        self.Signature('')
        self.UnapprovePasswordProtected(false)
        $('#UnapproveSignatureDiv').hide()
        self.SignatureApprovedUnapproved('U')
    }

    //validation
    self.Validation = function (data) {
        var errMsg = "";
        if (data == "A") {
            if (Validate.empty(self.SignatureApprovedUnapproved())) {
                errMsg += "Please Select an Option First !!! <br/>";
            }
            if (Validate.empty(self.ApprovedBy())) {
                errMsg += "Approved By Didn't Load Properly, Please Refresh !!! <br/>";
            }
            if (Validate.empty(self.ApprovedDate())) {
                errMsg += "Approved Date Didn't Load Properly, Please Refresh !!! <br/>";
            }
            if (Validate.empty(self.SignatureApprovedUnapproved())) {
                errMsg += "Please Select an Option First !!! <br/>";
            }
            if (self.PasswordProtected()) {
                if (Validate.empty(self.Password())) {
                    errMsg += "Please Enter A Password !!!</br>";
                } else {
                    if (Validate.empty(self.Password())) {
                        errMsg += "Please Enter Confirm Password !!!</br>";
                    }
                }
            }
            if ($('#tbl_ShHolderList').find('input[type=checkbox]:checked').length == 0) {
                errMsg += "Please Select A Record !!!</br>";
            }
        }
        if (data == "U") {
            if (Validate.empty(self.SignatureApprovedUnapproved())) {
                errMsg += "Please Select an Option First !!! <br/>";
            }
            if (Validate.empty(self.ShHolderNo())) {
                errMsg += "Please Enter ShHolderNo !!! <br/>";
            }


        }
        if (errMsg == "") {
            return true;
        }
        else {
            alert('warning', errMsg);
            return false;
        }
    }

    self.SignatureApprovedUnapproved.subscribe(() => {
        if (self.SignatureApprovedUnapproved() == "A") {
            $('#ApproveDiv').show()
            $('#UnApproveDiv').hide()
            $('#Save').val("Approve");
            self.GetSignaturesForApproval()
        }
        else {
            $('#ApproveDiv,#UnapproveSignatureDiv').hide()
            $('#UnApproveDiv').show()
            $('#Save').val("Unapprove");

        }
    })
    //password div
    self.PasswordProtected.subscribe(() => {
        if (self.PasswordProtected()) {
            $('#passwordProtectedDiv').show()
        }
        else {
            $('#passwordProtectedDiv').hide()
        }
    })

    //unappriove
    self.GetShHolderInformation = (data) => {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.ShHolderNo())) {
                var companyCode = self.SelectedCompany();
                Openloader()
                $.ajax({
                    type: "post",
                    url: 'SignatureApprovedUnapproved/GetUnApproveHolderDetail',
                    data: { 'CompCode': companyCode, 'ShHolderNo': self.ShHolderNo() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            self.ShHolderName(result.responseData.name);
                            self.TotalKitta(result.responseData.totalKitta);
                            self.ShHolderAddress(result.responseData.address);
                            self.ScannedBy(result.responseData.scanedBy);
                            self.UApprovedBy(result.responseData.approvedBy);
                            self.UApprovedDate(result.responseData.approvedDate == null ?"": result.responseData.approvedDate.substring(0,10));
                            self.UnapprovePasswordProtected(result.responseData.passProcted);
                            $('#UnapproveSignatureDiv').show()
                            self.Signature("data:image/" + result.responseData.imageType + ";base64," + result.responseData.signature)

                        } else {
                            alert('warning', result.message)
                            self.ClearControlsUnApprove()
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                        self.ClearControlsUnApprove()
                    }, complete: () => {
                        Closeloader()
                    }
                })

            }
        }

    }
    //unapoproved signatures list
    self.GetSignaturesForApproval = (data) => {
        if (self.ValidateCompany()) {
            var companyCode = self.SelectedCompany();
            Openloader()
            $.ajax({
                type: "post",
                url: '/SignatureHandling/SignatureApprovedUnapproved/GetAllSignatureList',
                data: { 'CompCode': companyCode },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {

                        $('#tbl_ShHolderList').DataTable().clear();
                        $('#tbl_ShHolderList').DataTable().destroy();
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new ShHolderSignature(item)
                        });
                        self.ApprovedBy(result.responseData[0].userName)
                        self.ShHolderSignatureList(mappedTasks);

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
    self.GetSignaturesForApproval()

    //password and confgirm password
    self.CheckPassword = (data) => {
        if (data == "P") {
            if (!Validate.empty(self.ConfirmPassword())) {
                if (self.Password() != self.ConfirmPassword()) {
                    alert('error', 'Passwords Dont Match !!!');
                } else {
                    $('#ConfirmPassword').focus()
                }
            } else {
                $('#ConfirmPassword').focus()
            }
        } else {
            if (!Validate.empty(self.Password())) {
                if (self.Password() != self.ConfirmPassword()) {
                    alert('error', 'Passwords Dont Match !!!');
                }
            }
        }
    }


    //get single signatures
    self.GetShHolderSignature = (data) => {
        if (self.ValidateCompany()) {
            Openloader()
            var companyCode = self.SelectedCompany()
            $.ajax({
                type: "post",
                url: '/SignatureHandling/SignatureApprovedUnapproved/GetSingleSignature',
                data: { 'CompCode': companyCode, 'ShHolderNo': data },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        self.Signature("data:image/" + result.responseData.imageType + ";base64," + result.responseData.signature)
                        $('#signatureModalTheme').modal('show')
                    } else {
                        alert('warning', result.message)
                        self.Signature('')
                        $('#signatureModalTheme').modal('hide')

                    }
                },
                error: function (error) {
                    alert('error', error.message)
                    self.Signature('')
                    $('#signatureModalTheme').modal('hide')
                }, complete: () => {
                    Closeloader()
                }
            })
        }
    }


    self.Save = (data) => {
        if (self.ValidateCompany()) {
            if (self.Validation(self.SignatureApprovedUnapproved())) {
                Openloader()
                if (self.SignatureApprovedUnapproved() == 'A') {
                    for (var i = 0; i < $('#tbl_ShHolderList').DataTable().data().count(); i++) {
                        var Check = $($('#tbl_ShHolderList').DataTable().row(i).nodes()).find('input').prop('checked');
                        if (Check != undefined && Check != "" && Check != false) {
                            record.push($('#tbl_ShHolderList').DataTable().row(i).data()[1])
                        }


                    }
                    $.ajax({
                        type: "POST",
                        url: '/SignatureHandling/SignatureApprovedUnapproved/SaveApprove',
                        data: {
                            'ShHolderNos': record,
                            'CompCode': self.SelectedCompany(),
                            'SelectedAction': self.SignatureApprovedUnapproved(),
                            'ApprovedDate': self.PostingDate(),
                            'hasPassword': self.PasswordProtected(),
                            'Password': self.Password(),
                            'ScannedBy': self.ApprovedBy()

                        },
                        datatype: "json", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (result) {
                            if (result.isSuccess) {
                                alert('success', result.message)
                            }
                            else {
                                alert('error', result.message)
                            }
                        },
                        error: function (eror) {
                            alert('error', error.message)
                        },
                        complete: () => {
                            self.GetSignaturesForApproval()
                            Closeloader()

                        }
                    })
                } else {
                    $.ajax({
                        type: "POST",
                        url: '/SignatureHandling/SignatureApprovedUnapproved/SaveUnapprove',
                        data: {
                            'ShHolderNo': self.ShHolderNo(),
                            'CompCode': self.SelectedCompany(),
                            'SelectedAction': self.SignatureApprovedUnapproved()
                        },
                        datatype: "json", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (result) {
                            if (result.isSuccess) {
                                alert('success', result.message)
                            }
                            else {
                                alert('error', result.message)
                            }
                        },
                        error: function (eror) {
                            alert('error', error.message)
                        },
                        complete: () => {
                            self.ClearControlsUnApprove()
                            Closeloader()

                        }
                    })
                }
                
            }
        }
    }
    self.Report = (data) => {

    }

    self.Refresh = (data) => {
        self.ClearControls()
    }

}

$(document).ready(function () {
    function toggle(source) {
        checkboxes = document.getElementsByName('case[]'); for (var i = 0, n = checkboxes.length;
            i < n; i++) { checkboxes[i].checked = source.checked; }
    }
    ko.applyBindings(new SignatureApproveUnapprove());
    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });

    $('#ApproveDiv').show()
    $('#passwordProtectedDiv,#UnapproveSignatureDiv').hide()
    $('#UnApproveDiv').hide()
});