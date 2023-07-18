function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}



function District(data) {
    var self = this;
    if (data != undefined) {
        self.DistCode = ko.observable(data.distCode);
        self.NpDistName = ko.observable(data.npDistName);
        self.EnDistName = ko.observable(data.enDistName);
        self.DistCodeName = self.DistCode() + " " + self.EnDistName();
    }
}

function ShHolderLockUnlock(data) {
    var self = this;
    if (data != undefined) {
        self.slu_compcode = ko.observable(data.compcode)
        self.slu_lock_id = ko.observable(data.lock_id)
        self.slu_ShholderNo = ko.observable(data.shholderNo)
        self.slu_lock_dt = ko.observable(data.lock_dt == null ? "" : data.lock_dt.substring(0, 10))
        self.slu_lock_remarks = ko.observable(data.lock_remarks)
        self.slu_status = ko.observable(data.status)
        self.slu_lock_by = ko.observable(data.lock_by)
        self.slu_approved = ko.observable(data.approved)
        self.slu_approved_by = ko.observable(data.approved_by)
        self.slu_app_status = ko.observable(data.app_status)
        self.slu_approved_date = ko.observable(data.approved_date)
        self.slu_unlock_by = ko.observable(data.unlock_by)
        self.slu_unlock_dt = ko.observable(data.unlock_dt == null ? "" : data.unlock_dt.substring(0, 10))
        self.slu_unlock_remarks = ko.observable(data.unlock_remarks)
        self.slu_approved_unlock = ko.observable(data.approved_unlock)
        self.slu_approved_by_unlock = ko.observable(data.approved_by_unlock)
        self.slu_app_status_unlock = ko.observable(data.app_status_unlock)
        self.slu_approved_unlock_dt = ko.observable(data.approved_unlock_dt == null ? "" : data.approved_unlock_dt.substring(0, 10))
        self.slu_approved_remarks = ko.observable(data.approved_remarks)
        self.slu_unlock_approved_remarks = ko.observable(data.unlock_approved_remarks)
        self.slu_fname = ko.observable(data.fname)
        self.slu_lname = ko.observable(data.lname)
    }
}

var ShareHolderLockUnlock = function () {

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
    self.FullName = ko.observable()
    self.ShOwnerType = ko.observable()
    self.FullNameNepali = ko.observable()
    self.ShOwnerSubType = ko.observable()
    self.FaName = ko.observable()
    self.GrFaName = ko.observable()
    self.Address = ko.observable()
    self.DistCode = ko.observable()
    self.TelNo = ko.observable()
    self.PoBoxNo = ko.observable()
    self.LockId = ko.observable()
    self.LockDate = ko.observable()
    self.Remarks = ko.observable()
    self.NpName = ko.observable()

    self.UnlockRemarks = ko.observable()
    self.UnlockId = ko.observable()
    self.UnlockDate = ko.observable()

    self.RecordType = ko.observable()

    self.ShHolderList = ko.observableArray([])

    self.SelectedAction = ko.observable()


    //shholder lock unlock data
    self.slu_compcode = ko.observable()
    self.slu_lock_id = ko.observable()
    self.slu_ShholderNo = ko.observable()
    self.slu_lock_dt = ko.observable()
    self.slu_lock_remarks = ko.observable()
    self.slu_status = ko.observable()
    self.slu_lock_by = ko.observable()
    self.slu_approved = ko.observable()
    self.slu_approved_by = ko.observable()
    self.slu_app_status = ko.observable()
    self.slu_approved_date = ko.observable()
    self.slu_unlock_by = ko.observable()
    self.slu_unlock_dt = ko.observable()
    self.slu_unlock_remarks = ko.observable()
    self.slu_approved_unlock = ko.observable()
    self.slu_approved_by_unlock = ko.observable()
    self.slu_app_status_unlock = ko.observable()
    self.slu_approved_unlock_dt = ko.observable()
    self.slu_approved_remarks = ko.observable()
    self.slu_unlock_approved_remarks = ko.observable()
    self.slu_fname = ko.observable()
    self.slu_lname = ko.observable()

    self.ShHolderLockUnlockList = ko.observableArray([])

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


    self.LockDate(formatDate(new Date))
    self.UnlockDate(formatDate(new Date))
    //Validation 
    self.Validation = function () {
        var errMsg = '';
        if (Validate.empty(self.RecordType())) 
            errMsg += "Please Select Lock/Unlock !!!";
        if (Validate.empty(self.SelectedAction()))
            errMsg += "Please Select Add Update Or Delete !!!"
        if (Validate.empty(self.ShholderNo()))
            errMsg += "Please Enter A ShHolder No !!!"
        if (optionAUD != "A") {
            if (Validate.empty(self.LockId()))
                errMsg += "Lock Id Didn't Generate, Please Insert Shholder No Again !!!";
        }
        if (Validate.empty(self.LockDate()))
            errMsg += "Please Add a Lock Date !!!";
        if (Validate.empty(self.Remarks()))
            errMsg += "Please Add a Remarks !!!";

        if (self.RecordType() == 'U') {
            if (Validate.empty(self.UnlockRemarks()))
                errMsg += "Please Add a Unlock Remarks !!!"
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
        $("#HoldersList").modal('hide');
        if (!Validate.empty(self.RecordType()) && !Validate.empty(optionAUD)) {
            if (shholderNo != undefined && shholderNo != "") {
                Openloader()
                $.ajax({
                    type: "post",
                    url: '/HolderManagement/ShareHolderLockUnlockEntry/GetHolderForLockUnlock',
                    data: {
                        'CompCode': ko.toJS(self.SelectedCompany()),
                        'ShHolderNo': ko.toJS(shholderNo),
                        'RecordType': self.RecordType(),
                        'SelectedAction': optionAUD
                    },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            $("#saveShholder").attr("disabled", false);
                            self.FName(result.responseData.fName);
                            self.LName(result.responseData.lName);
                            self.FullName(self.FName() + ' '+ self.LName())
                            self.Address(result.responseData.address);
                            self.DistCode(result.responseData.fName);
                            self.PoBoxNo(result.responseData.pboxNo);
                            self.FaName(result.responseData.faName);
                            self.GrFaName(result.responseData.grFaName);
                            self.NpName(convert_to_unicode_with_value(result.responseData.npName));
                            self.TelNo(result.responseData.telNo);
                            self.TotalKitta(result.responseData.totalKitta);
                            self.ShOwnerType(result.responseData.ownerType == "01" ? "01  Promoter" : "02  Public" );
                            self.ShOwnerSubType(result.responseData.shownerSubtype);
                            if (self.RecordType() == 'L') {
                                if (self.SelectedAction() == "D") {

                                    $('#remarks').attr("disabled", true)
                                }
                                else {
                                    $('#remarks').attr("disabled", false)

                                }
                            }
                            else {

                                $('#remarks').attr("disabled", true)
                                if (self.SelectedAction() == 'A'  || self.SelectedAction() == "U") {
                                    $('#unlockremarks').attr("disabled", false)
                                }
                                else {
                                    $('#unlockremarks').attr("disabled", true)

                                }
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
        else {
            alert('warning','Please Select Lock or Unlock First');
        }
       
    }


    //for choosing add or update or delete
    self.chooseOptions = function (data) {
        if (self.ValidateCompany()) {
            optionAUD = data;
            self.SelectedAction(data);
            enableDisableButtons();
            $("#Company").attr("disabled", false);
            if (self.RecordType() == 'L') {
                if (data == "A") {
                    $('#ShHolderNo').attr("disabled", false);
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
                    $('#ShHolderNo').attr("disabled", true);
                    Openloader()
                    $.ajax({
                        type: "post",
                        url: '/HolderManagement/ShareHolderLockUnlockEntry/GetRecordShHolderLuckDetail',
                        data: {
                            'CompCode': ko.toJS(self.SelectedCompany())
                        },
                        datatype: "json", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (result) {
                            if (result.isSuccess) {
                                var mappedTasks = $.map(result.responseData, function (item) {
                                    return new ShHolderLockUnlock(item)
                                });
                                self.ShHolderLockUnlockList(mappedTasks);
                                $("#HoldersList").modal('show');
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
                    });
                }
            }
            else {
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
                $('#ShHolderNo').attr("disabled", true);
                Openloader()
                $.ajax({
                    type: "post",
                    url: '/HolderManagement/ShareHolderLockUnlockEntry/GetRecordShHolderLuckDetail',
                    data: {
                        'CompCode': ko.toJS(self.SelectedCompany()),
                        'SelectedAction': self.SelectedAction(),
                        'RecordType': self.RecordType()
                    },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new ShHolderLockUnlock(item)
                            });
                            self.ShHolderLockUnlockList(mappedTasks);
                            $("#HoldersList").modal('show');
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
                });
            }

            
    }}

    self.RecordType.subscribe(function () {
        $("#AddButton,#UpdateButton,#DeleteButton").attr("disabled", false);
        if (self.RecordType() == "U") {
            $("#ifUnlock").show();
        }
        else $('#ifUnlock').hide()
    })
    self.ClearData = function () {

        self.ShholderNo('')
        self.TotalKitta('')
        self.FullName('')
        self.ShOwnerType('')
        self.FullNameNepali('')
        self.ShOwnerSubType('')
        self.FaName('')
        self.GrFaName('')
        self.Address('')
        self.DistCode('')
        self.TelNo('')
        self.PoBoxNo('')
        self.LockId('')
        self.Remarks('')
        self.NpName('')
        self.RecordType('')

        self.ShHolderList([])
    }
    self.Save = function (data) {
        if (self.ValidateCompany()) {
            if (self.Validation()) {
                var title, text, icon
                var companyCode = self.SelectedCompany()

                if (self.RecordType() == 'L') {
                    if (optionAUD == "A") {
                        text = "You want to Lock this Holder ?"
                        icon = "success"
                    }
                    else if (optionAUD == "U") {
                        text = "You want to Update this Holder ?"
                        icon = "warning"
                    }
                    else {
                        text = "You want to Remove this Holder Lock ?"
                        icon = "error"
                    }
                }
                else {
                    if (optionAUD == "A") {
                        text = "You want to Unlock this Holder ?"
                        icon = "success"
                    }
                    else if (optionAUD == "U") {
                        text = "You want to Update this Holder ?"
                        icon = "warning"
                    }
                    else {
                        text = "You want to Remove this Holder Unlock ?"
                        icon = "error"
                    }
                }
                
                swal({
                    title: "Are you sure?",
                    text: text,
                    icon: icon,
                    buttons: true,
                    dangerMode: true
                }).then((willSave) => {
                    if (willSave) {
                        Openloader()
                        $.ajax({
                            type: "POST",
                            url: "/HolderManagement/ShareHolderLockUnlockEntry/SaveHolderLockUnlock",
                            data: {
                                'CompCode': companyCode, 'ShHolderNo': ko.toJS(self.ShholderNo()),
                                'RecordType': self.RecordType(), 'SelectedAction': optionAUD, 'LockId': self.LockId(),
                                'LockRemarks': self.Remarks(), 'LockDate': self.LockDate(), 'UnlockDate': self.UnlockDate(),
                                'UnlockRemarks':self.UnlockRemarks()
                            },
                            dataType: "json", beforeSend: function (xhr) {
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
                                self.refresh()
                                Closeloader()
                            }
                        });
                    }
                });
            }
        }
        
    }

    self.GetSholderLockUnlockInformation = function (data) {
        $('#HoldersList').modal('hide');
        self.LockId(data.slu_lock_id())
        self.GetDataFromLockId(data.slu_lock_id());
    }

    self.GetDataFromLockId = function (data) {
        if (data != undefined) {
            if (self.ValidateCompany()) {
                Openloader()
                $.ajax({
                    type: "post",
                    url: '/HolderManagement/ShareHolderLockUnlockEntry/GetHolderByLockId',
                    data: { 'CompCode': ko.toJS(self.SelectedCompany()), 'LockId': data, 'RecordType': self.RecordType(), 'SelectedAction': optionAUD },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            if (self.RecordType == 'L') {
                                self.ShholderNo(result.responseData.shholderNo)
                                self.Remarks(result.responseData.lock_remarks)
                                self.GetSholderInformation(result.responseData)
                            }
                            else {
                                self.ShholderNo(result.responseData.shholderNo)
                                self.Remarks(result.responseData.lock_remarks)
                                self.LockDate(result.responseData.lock_dt == null ? "" : result.responseData.lock_dt.substring(0, 10))
                                self.UnlockRemarks(result.responseData.unlock_remarks)
                                $('#unlockremarks').attr("disabled", self.SelectedAction() == "U" ? false : true)
                                self.GetSholderInformation(result.responseData)

                            }
                            
                        }
                        else {
                            alert('error', result.message)
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                    
                })
            }
        }
    }
    self.refresh = function (data) {
        self.ButtonOnLoad();
        self.ClearData()
        $("#AddButton,#UpdateButton,#DeleteButton,#searchButton,#remarks,#unlockremarks,#ShHolderNo").attr("disabled", "disabled");
    }
  

    self.enableDisableButtons= function () {
        $("#AddButton,#UpdateButton,#DeleteButton,#searchButton").attr("disabled", "disabled");
        $("#searchButton").attr("disabled", false);
    }
    self.ButtonOnLoad = function () {
        $("#AddButton,#UpdateButton,#DeleteButton,#searchButton,#saveShholder").attr("disabled", true);
    }


}

$(document).ready(function () {

    ko.applyBindings(new ShareHolderLockUnlock());
    self.ButtonOnLoad()
    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
    $('#simple-date2 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
    self.refresh()

});