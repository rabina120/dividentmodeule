function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
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
        self.slu_approved_date = ko.observable(data.approved_date == null ? "" : data.approved_date.substring(0, 10))
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
        self.Selected = ko.observable()
    }
}

var ShareHolderLockUnlockPosting = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()


    self.RecordType = ko.observable()


    self.PostingRemarks = ko.observable()
    self.PostingDate = ko.observable()


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
    var record = []
    //posting
    self.dateFrom = ko.observable();
    self.dateTo = ko.observable();

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


    self.PostingDate(formatDate(new Date))

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

  
   

    self.SelectAll = ko.computed({
        read: () => !self.ShHolderLockUnlockList().find(x => !x.Selected()),
        write: t => self.ShHolderLockUnlockList().forEach(x => x.Selected(t)),
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

        self.RecordType('')

        self.ShHolderList([])
    }

    //self.RecordType.subscribe(function () {
    //    if (self.ValidateCompany()) {
    //        loadDataTable()
    //    }
    //})
   
    self.refresh = function (data) {
        self.ButtonOnLoad();
        self.ClearData()
        $("#AddButton,#UpdateButton,#DeleteButton,#searchButton").attr("disabled", "disabled");
    }
    self.report = function (data) {

    }

    self.enableDisableButtons= function () {
        $("#AddButton,#UpdateButton,#DeleteButton,#searchButton").attr("disabled", "disabled");
        $("#searchButton").attr("disabled", false);
    }
    self.ButtonOnLoad = function () {
        $("#AddButton,#UpdateButton,#DeleteButton,#searchButton,#saveShholder,#reportButton").attr("disabled", true);
    }

    self.PostShholderInfo = function (data) {
        if (self.Validation()) {
            Openloader()
            for (var i = 0; i < $('#tbl_Post_Data').DataTable().data().count(); i++) {
                var x = i + 1;
                var Check = $($('#tbl_Post_Data').DataTable().row(i).nodes()).find('input').prop('checked');
                if (Check != undefined && Check != "" && Check != false) {


                    var DIS = {
                        lock_id: $('#tbl_Post_Data').DataTable().row(i).data()[1],
                        ShholderNo: $('#tbl_Post_Data').DataTable().row(i).data()[2],
                    }

                    record.push(DIS)
                }

            }
            $.ajax({
                type: "POST",
                url: '/HolderManagement/ShareHolderLockUnlockPosting/PostLockUnlockData',
                data: {
                    'ShHolderLocks': record,
                    'CompCode': self.SelectedCompany(),
                    'RecordType': self.RecordType(),
                    'SelectedAction': data,
                    'Remarks': self.PostingRemarks(),
                    'PostingDate': self.PostingDate()

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
                    self.PostingRemarks('')
                },
                error: function (eror) {
                    alert('error', error.message)
                    self.PostingRemarks('')
                },
                complete: () => {
                    Closeloader()
                    loadDataTable()
                    self.PostingRemarks('')
                    record = []
                }
            })
        }
    }


    loadDataTable = function () {
        if (self.ValidateCompany()) {
            if (Validate.empty(self.RecordType())) {
                alert('warning', 'Please Select a Selection Type !!!')
            }
            else {
                Openloader()
                var companyCode = self.SelectedCompany()

                $.ajax({
                    type: "post",
                    url: '/HolderManagement/ShareHolderLockUnlockPosting/GetLockUnlockData',
                    data: { 'CompCode': companyCode, 'FromDate': self.dateFrom(), 'ToDate': self.dateTo(), 'RecordType': self.RecordType() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        $('#tbl_Post_Data').DataTable().clear();
                        $('#tbl_Post_Data').DataTable().destroy();
                        if (result.isSuccess) {

                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new ShHolderLockUnlock(item)
                            });
                            self.ShHolderLockUnlockList(mappedTasks);
                        } else {
                            alert('warning', "No Data Found ")
                        }
                        self.PostingRemarks('')

                    },
                    error: function (error) {
                        $('#tbl_Post_Data').DataTable().clear();
                        $('#tbl_Post_Data').DataTable().destroy();
                        alert('error', error.message)
                        self.PostingRemarks('')
                    },
                    complete: () => {

                        $('#tbl_Post_Data').DataTable({
                            responsive: true,
                            searching: true,
                            scrollX: true,
                            scrollY: true,
                            paging: true,
                            ordering: false,
                            columnDefs: [
                                { width: 100, targets: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10] }
                            ],
                            fixedColumns: true
                        })
                        self.PostingRemarks('')
                        Closeloader()
                    }
                })
            }

        }
    }
    self.Validation = function () {
        var errMsg = "";
        if (self.SelectedCompany() == undefined) {
            errMsg += "Please Select Company !!!</br>";
        }

        if ($('#tbl_Post_Data').find('input[type=checkbox]:checked').length == 0) {
            errMsg += "Please Select A Record !!!</br>";
        }
        if (Validate.empty(self.PostingRemarks())) {
            errMsg += "Please Enter Posting Remarks !!!</br>";
        }
        if (Validate.empty(self.PostingDate())) {
            errMsg += "Please Enter Posting Date !!!</br>";
        }
        if (Validate.empty(self.RecordType())) {
            errMsg += "Please Select a Selection Type !!!<br/>";
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }

    }


}

$(document).ready(function () {

    ko.applyBindings(new ShareHolderLockUnlockPosting());
    $("#Company").attr("disabled", false);
    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });

    $('#simple-date2 .input-group.date').datepicker({
        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true,
    });

});