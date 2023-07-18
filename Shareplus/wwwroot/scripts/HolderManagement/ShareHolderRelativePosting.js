
function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}



function ShHolder(data) {
    var self = this;
    if (data != undefined) {

        self.ShholderNo = ko.observable(data.shholderNo);
        self.Name = ko.observable(data.name);
        self.FaName = ko.observable(data.faName);
        self.RHolderNo = ko.observable(data.rHolderNo);
        self.RHolderName = ko.observable(data.rHolderName);
        self.SN = ko.observable(data.sn)
        self.Selected = ko.observable();
    }
}



var ShareHolderPosting = function () {


    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()


    self.ShHolderList = ko.observableArray([])


    //Shholderkolagi
    self.ShholderNo = ko.observable();
    self.Name = ko.observable();
    self.Status = ko.observable();
    self.Name = ko.observable();
    self.FaName = ko.observable();
    self.RHolderNo = ko.observable();


    // dataTable for the Holder List
    var dataTable;
    var record = [];
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
    //load Holder List waiting for Posting

    loadDataTable = function () {
        if (self.ValidateCompany()) {

            Openloader()
            var companyCode = self.SelectedCompany()

            $.ajax({
                type: "post",
                url: '/HolderManagement/ShareHolderRelativePosting/GetHolderForPosting',
                data: { 'CompCode': companyCode, 'FromDate': self.dateFrom(), 'ToDate': self.dateTo() },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    $('#tbl_Add_Records_Sholder').DataTable().clear();
                    $('#tbl_Add_Records_Sholder').DataTable().destroy();
                    if (result.isSuccess) {

                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new ShHolder(item)
                        });
                        self.ShHolderList(mappedTasks);
                    } else {
                        alert('warning', "No Data Found ")
                    }
                    

                },
                error: function (error) {
                    $('#tbl_Add_Records_Sholder').DataTable().clear();
                    $('#tbl_Add_Records_Sholder').DataTable().destroy();
                    alert('error', error.message)
                    
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
                            { width: 100, targets: [1, 2, 3, 4, 5, 6] }
                        ],
                        fixedColumns: true
                    })
                   
                    Closeloader()
                }
            })
        }
    }

    /*loadDataTable();*/

    self.SelectAll = ko.computed({
        read: () => !self.ShHolderList().find(x => !x.Selected()),
        write: t => self.ShHolderList().forEach(x => x.Selected(t)),
    })

    self.Validation = function () {
        var errMsg = "";
        if (self.SelectedCompany() == undefined) {
            errMsg += "Please Select Company !!!</br>";
        }

        if ($('#tbl_Add_Records_Sholder').find('input[type=checkbox]:checked').length == 0) {
            errMsg += "Please Select A Record !!!</br>";
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


    self.PostShholderInfo = function (data) {
        if (self.Validation()) {
            Openloader()
            for (var i = 0; i < $('#tbl_Add_Records_Sholder').DataTable().data().count(); i++) {
                var x = i + 1;
                var Check = $($('#tbl_Add_Records_Sholder').DataTable().row(i).nodes()).find('input').prop('checked');
                if (Check != undefined && Check != "" && Check != false) {
                    var DIS = {
                        ShholderNo: $('#tbl_Add_Records_Sholder').DataTable().row(i).data()[2],
                        RHolderNo: $('#tbl_Add_Records_Sholder').DataTable().row(i).data()[5],
                        SN: $('#tbl_Add_Records_Sholder').DataTable().row(i).data()[1]
                    }
                    record.push(DIS)
                }

            }
            $.ajax({
                type: "POST",
                url: '/HolderManagement/ShareHolderRelativePosting/SaveShHolderRelative',
                data: {
                    'attShHolderForRelatives': record,
                    'CompCode': self.SelectedCompany(),
                    
                    'ApprovedDate': self.PostingDate(),
                    'SelectedAction': data },
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
                    Closeloader()
                    loadDataTable()
                   
                }
            })
        }
    }
}
$(document).ready(function () {
    ko.applyBindings(new ShareHolderPosting());
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
            { width: 130, targets: [1, 2, 3, 4, 5, 6] }
        ],
        fixedColumns: true
    })
});

