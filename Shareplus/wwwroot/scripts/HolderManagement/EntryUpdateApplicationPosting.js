function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}


function ShHolderApplicationPosting(data) {
    var self = this;
    if (data != undefined) {
       self.ApplicationNo = ko.observable(data.applicationNo)
        self.ShHolderNo = ko.observable(data.shholderNo)
        self.Name = ko.observable(data.fName + ' ' + data.lName)
        self.UserName = ko.observable(data.userName)
        self.Action = ko.observable(data.action)
        self.Selected = ko.observable(false)
    }
}

var ShHolderApplicationPostingVM = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()


    self.PostingRemarks = ko.observable()
    self.PostingDate = ko.observable()
    self.ApplicationList  = ko.observableArray([])

    //gloabal variables
    var record = []
    //posting
    self.dateFrom = ko.observable();
    self.dateTo = ko.observable();

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
        read: () => !self.ApplicationList().find(x => !x.Selected()),
        write: t => self.ApplicationList().forEach(x => x.Selected(t)),
    })

    self.LoadData = function () {
        if (self.ValidateCompany()) {
            Openloader()
            var companyCode = self.SelectedCompany()

            $.ajax({
                type: "post",
                url: '/HolderManagement/UpdateApplicationPosting/GetAllApplicationList',
                data: { 'CompCode': companyCode, 'FromDate': self.dateFrom(), 'ToDate': self.dateTo()},
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    $('#tbl_Post_Data').DataTable().clear();
                    $('#tbl_Post_Data').DataTable().destroy();
                    if (result.isSuccess) {

                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new ShHolderApplicationPosting(item)
                        });
                        self.ApplicationList(mappedTasks);
                        Closeloader()
                    } else {
                        alert('warning', result.message)
                        Closeloader()
                    }
                    self.PostingRemarks('')

                },
                error: function (error) {
                    alert('error', error.message)
                    self.PostingRemarks('')
                    Closeloader()
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
                            { width: 100, targets: [1, 2, 3,4] },
                            { width: 300, targets: [5] },
                        ],
                        fixedColumns: true
                    })
                    self.PostingRemarks('')
                   
                }
            })
        }

    }
   /* self.LoadData()*/

    self.PostShholderInfo = function (data) {
        if (self.Validation()) {
            Openloader()
            for (var i = 0; i < $('#tbl_Post_Data').DataTable().data().count(); i++) {
                var x = i + 1;
                var Check = $($('#tbl_Post_Data').DataTable().row(i).nodes()).find('input').prop('checked');
                if (Check != undefined && Check != "" && Check != false) {


                    var DIS = {
                        ApplicationNo: $('#tbl_Post_Data').DataTable().row(i).data()[1],
                        ShholderNo: $('#tbl_Post_Data').DataTable().row(i).data()[2],
                    }

                    record.push(DIS)
                }

            }
            $.ajax({
                type: "POST",
                url: '/HolderManagement/UpdateApplicationPosting/SaveApplication',
                data: {
                    'aTTShHolders': record,
                    'CompCode': self.SelectedCompany(),
                    'SelectedAction': data,
                    'PostingRemarks': self.PostingRemarks(),
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
                    
                    LoadData()
                    self.PostingRemarks('')
                    record = []
                    Closeloader()
                }
            })
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

    ko.applyBindings(new ShHolderApplicationPostingVM());

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

    $('#tbl_Post_Data').DataTable({
        responsive: true,
        searching: true,
        scrollX: true,
        scrollY: true,
        paging: true,
        ordering: false,
        columnDefs: [
            { width: 100, targets: [1, 2, 3,4] },
            { width: 300, targets: [5] },
        ],
        fixedColumns: true
    })

});