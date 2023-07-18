function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}


function HolderMergeData(data) {
    var self = this;
    if (data != undefined) {

        self.app_date = ko.observable(data.app_date)
        self.app_status = ko.observable(data.app_status)
        self.approved = ko.observable(data.approved)
        self.approved_by = ko.observable(data.approved_by)
        self.approved_remarks = ko.observable(data.approved_remarks)
        self.entry_dt = ko.observable(data.entry_dt == null ? "" : data.entry_dt.substring(0, 10))
        self.holdernofrom = ko.observable(data.holdernofrom)
        self.holdernoto = ko.observable(data.holdernoto)
        self.kitta_from = ko.observable(data.kitta_from)
        self.kitta_to = ko.observable(data.kitta_to)
        self.merge_by = ko.observable(data.merge_by)
        self.merge_id = ko.observable(data.merge_id)
        self.merge_remarks = ko.observable(data.merge_remarks)
        self.mergedate = ko.observable(data.mergedate == null ? "" : data.mergedate.substring(0, 10))
        self.holdernofromname = ko.observable(data.holdernofromname)
        self.holdernotoname = ko.observable(data.holdernotoname)
        self.Selected = ko.observable()
    }
}

var HolderMergePosting = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()


    self.PostingDate = ko.observable()
    self.PostingRemarks = ko.observable()
    self.HolderMergeDataList = ko.observableArray([])

    //gloabal variables
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
        read: () => !self.HolderMergeDataList().find(x => !x.Selected()),
        write: t => self.HolderMergeDataList().forEach(x => x.Selected(t)),
    })


    self.ClearData = function () {

        self.PostingRemarks('')
        self.HolderMergeDataList([])
        $('#tbl_Add_Records_Sholder').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: false,
            columnDefs: [
                { width: 100, targets: [1, 2, 3, 4, 5, 6, 7, 8] }
            ],
            fixedColumns: true
        })
    }

    self.refresh = function (data) {
        self.ClearData()
    }
    self.report = function (data) {

    }

    self.PostShholderInfo = function (data) {
        if (self.Validation()) {
            Openloader()
            for (var i = 0; i < $('#tbl_Add_Records_Sholder').DataTable().data().count(); i++) {
                var x = i + 1;
                var Check = $($('#tbl_Add_Records_Sholder').DataTable().row(i).nodes()).find('input').prop('checked');
                if (Check != undefined && Check != "" && Check != false) {


                    var DIS = {
                        merge_id: $('#tbl_Add_Records_Sholder').DataTable().row(i).data()[1],
                        holdernofrom: $('#tbl_Add_Records_Sholder').DataTable().row(i).data()[2],
                        holdernoto: $('#tbl_Add_Records_Sholder').DataTable().row(i).data()[3],
                    }

                    record.push(DIS)
                }

            }
            $.ajax({
                type: "POST",
                url: '/HolderManagement/HolderMergePosting/SaveHolderPosting',
                data: {
                    'aTTMergeDetails': record,
                    'CompCode': self.SelectedCompany(),
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

            Openloader()
            var companyCode = self.SelectedCompany()

            $.ajax({
                type: "post",
                url: '/HolderManagement/HolderMergePosting/GetAllMergeHoldersList',
                data: { 'CompCode': companyCode, 'FromDate': self.dateFrom(), 'ToDate': self.dateTo()},
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    $('#tbl_Add_Records_Sholder').DataTable().clear();
                    $('#tbl_Add_Records_Sholder').DataTable().destroy();
                    if (result.isSuccess) {

                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new HolderMergeData(item)
                        });
                        self.HolderMergeDataList(mappedTasks);
                    } else {
                        alert('warning', "No Data Found ")
                    }
                    self.PostingRemarks('')

                },
                error: function (error) {
                    $('#tbl_Add_Records_Sholder').DataTable().clear();
                    $('#tbl_Add_Records_Sholder').DataTable().destroy();
                    alert('error', error.message)
                    self.PostingRemarks('')
                },
                complete: () => {

                    $('#tbl_Add_Records_Sholder').DataTable({
                        responsive: true,
                        searching: true,
                        scrollX: true,
                        scrollY: true,
                        paging: true,
                        ordering: false,
                        columnDefs: [
                            { width: 100, targets: [1, 2, 3, 4, 5, 6, 7, 8] }
                        ],
                        fixedColumns: true
                    })
                    self.PostingRemarks('')
                    Closeloader()
                }
            })
        }
    }
    self.loadDataTable()
    self.Validation = function () {
        var errMsg = "";
        if (self.SelectedCompany() == undefined) {
            errMsg += "Please Select Company !!!</br>";
        }

        if ($('#tbl_Add_Records_Sholder').find('input[type=checkbox]:checked').length == 0) {
            errMsg += "Please Select A Record !!!</br>";
        }
        if (Validate.empty(self.PostingRemarks())) {
            errMsg += "Please Enter Posting Remarks !!!</br>";
        }
        if (Validate.empty(self.PostingDate())) {
            errMsg += "Please Enter Posting Date !!!</br>";
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

    ko.applyBindings(new HolderMergePosting());

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

    $('#tbl_Add_Records_Sholder').DataTable({
        responsive: true,
        searching: true,
        scrollX: true,
        scrollY: true,
        paging: true,
        ordering: false,
        columnDefs: [
            { width: 100, targets: [1, 2, 3, 4, 5, 6, 7, 8] }
        ],
        fixedColumns: true
    })

});