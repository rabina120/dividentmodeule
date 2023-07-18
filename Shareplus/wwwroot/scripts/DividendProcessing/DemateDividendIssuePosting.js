function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function Dividend(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.compcode);
        self.Divcode = ko.observable(data.divcode);
        self.Description = ko.observable(data.description);
        self.FiscalYr = ko.observable(data.fiscalYr);
        self.ShowDividendText = self.Divcode() + " " + self.Description() + " " + self.FiscalYr();
    }
      
}

function CashDividendCDS(data) {
    var self = this;
    if (data != undefined) {
        self.BO_idno = ko.observable(data.bO_idno);
        self.compcode = ko.observable(data.compcode);
        self.Divcode = ko.observable(data.divcode);
        self.Wissue_Approved = ko.observable(data.wissue_Approved)
        self.Shholderno = ko.observable(data.shHolderNo)
        self.AgmNo = ko.observable(data.agmNo)
        self.WarrantNo = ko.observable(data.warrantno)
        self.WarrantAmt = ko.observable(data.warrantamt)
        self.Taxdamt = ko.observable(convertToFixTwo(data.taxdamt))
        self.Bonustax = ko.observable(data.bonustax)
        self.Bonusadj = ko.observable(data.bonusadj)
        self.Prevadj = ko.observable(data.prevadj)
        self.Totshkitta = ko.observable(data.totshkitta)
        self.Dwiby = ko.observable(data.dwiby)
        self.Wissueddt = ko.observable(convertDate(data.wIssuedDt))
        self.wissue_status = ko.observable(data.wissue_status)
        self.FullName = ko.observable(data.fullname)
        self.FName = ko.observable(data.fName)
        self.LName = ko.observable(data.lName)
        self.Selected = ko.observable();
    }
}

var CashDemateIssuePosting = function () {
    //Companykolagi

    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    self.PostingRemarks = ko.observable()
    self.ActionType = ko.observable()


    //Dividend ko table list lai
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.CashDemateTableName = ko.observable();

    //CashDividend Request
    self.compcode = ko.observable();
    self.Divcode = ko.observable();
    self.Wissue_Approved = ko.observable()
    self.Shholderno = ko.observable()
    self.AgmNo = ko.observable()
    self.WarrantNo = ko.observable()
    self.WarrantAmt = ko.observable()
    self.Taxdamt = ko.observable()
    self.Bonustax = ko.observable()
    self.Bonusadj = ko.observable()
    self.Totshkitta = ko.observable()
    self.Dwiby = ko.observable()
    self.Wissueddt = ko.observable()
    self.Bonusadj = ko.observable()
    self.Totshkitta = ko.observable()
    self.Wissue_approvedby = ko.observable()
    self.FName = ko.observable()
    self.LName = ko.observable()
    self.BO_idno = ko.observable()
    self.FullName = ko.observable()


    self.CashDemateList = ko.observable([])

    //posting
    self.dateFrom = ko.observable();
    self.dateTo = ko.observable();

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

    

    //Loading dividend lists select options
    self.LoadDividendLists = function () {
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            $.ajax({
                type: "post",
                url: '/DividendProcessing/DemateDividendIssueEntry/GetAllDemateDividends',
                data: { 'CompCode': companyCode },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Dividend(item)
                        });
                        self.DividendLists(mappedTasks);
                    }
                    else {
                        
                        self.DividendLists(mappedTasks);
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })
        }
    }
    self.SelectedCompany.subscribe(() => {
        if (!Validate.empty(self.SelectedCompany())) {
            self.LoadDividendLists();
        }
    })

    //load Holder List waiting for Posting




    self.Validation = function () {
        var errMsg = "";


        if (self.SelectedCompany() == undefined) {
            errMsg += "Please Choose Company !!!</br>";
        }
        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select Dividend !!!</br>";
        }
        
        //if (self.CashDemateList().filter(x => x.Selected()).length <= 0) {
        //    errMsg += "Please Tick the Cash Demate Info !!!</br>";
        //}
        //if (Validate.empty(self.PostingRemarks())) {
        //    errMsg += "Please Enter Posting Remarks !!!</br>";
        //}
        //if (Validate.empty(self.PostingDate())) {
        //    errMsg += "Please Enter Posting Date !!!</br>";
        //}
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }

    }

    self.LoadDataTable = function () {
        if (self.Validation()) {
            $('#tbl_CashDividend').DataTable().clear()
            $('#tbl_CashDividend').DataTable().destroy();
            Openloader()
            $.ajax({
                url: '/DividendProcessing/DemateDividendIssuePosting/GetCashDemateForApproval',
                data: { 'CompCode': self.SelectedCompany(), 'FromDate': self.dateFrom(), 'ToDate': self.dateTo(), 'Divcode': SelectedDividend() },
                type: 'POST', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                dataType: 'json',
                success: function (result) {

                    $('#chk').prop('checked', false);
                    if (result.isSuccess) {
                        if (result.responseData.length > 0) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new CashDividendCDS(item)
                            });
                            self.CashDemateList(mappedTasks);
                        }
                        else {
                            alert('error', 'No Record Found')
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
                    $('#tbl_CashDividend').DataTable({
                        responsive: true,
                        searching: true,
                        scrollX: true,
                        scrollY: true,
                        paging: true,
                        ordering: false,
                        columnDefs: [
                            { width: 100, targets: [2, 3, 9, 10] },
                            { width: 50, targets: [1] },
                            { width: 80, targets: [6, 7, 8] },
                        ],
                        fixedColumns: true
                    })
                }
            })
        }
        
    }



    self.SelectAll = ko.computed({
        read: () => !self.CashDemateList().find(x => !x.Selected()),
        write: t => self.CashDemateList().forEach(x => x.Selected(t)),
    })


    self.PostDividendSetUp = function (data) {
        if (self.Validation()) {

            if (Validate.empty(self.PostingRemarks())) {
                alert('error', "Please Enter Remarks !!!");
            }
            else {

                var record = self.CashDemateList().filter(x => x.Selected()).map(x => ({
                    warrantno: x.WarrantNo(),
                    BO_idno: x.BO_idno(),

                }));
                self.ActionType(data)

                console.log("", record)
                var RecordDetails = {
                    Compcode: self.SelectedCompany(),
                    DivCode: self.SelectedDividend(),
                    wissue_auth_remarks: self.PostingRemarks(),
                    wissue_app_date: self.PostingDate(),
                }


                Openloader()
                $.ajax({
                    type: 'POST',
                    datatype: 'json', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/DividendProcessing/DemateDividendIssuePosting/PostCashDemateRequest',
                    data: { 'attDivMasterCDS': JSON.stringify(record), 'RecordDetails': RecordDetails, 'ActionType': self.ActionType() },
                    success: (result) => {
                        if (result.isSuccess) {
                            alert('success', result.message)

                        } else {
                            alert('warning', result.message)
                        }
                    },
                    failure: (result) => {
                        alert('error', result.message)

                    },
                    error: (error) => {
                        alert('error', error.message)

                    },
                    complete: () => {
                        Closeloader()

                        self.ClearControl()

                    }
                })
            }



        }
    }

    self.ClearControl = function () {
        $('#tbl_CashDividend').DataTable().clear()
        $('#tbl_CashDividend').DataTable().destroy();
        $('#tbl_CashDividend tbody').empty();

        $('#chk').prop('checked', false);
        self.PostingRemarks('')
        self.LoadDividendLists()
        self.SelectedDividend(null)
        self.SelectedDividend(undefined)
        $('#tbl_CashDividend').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: false,
           
            fixedColumns: true
        })
    }


}


$(document).ready(function () {
    ko.applyBindings(new CashDemateIssuePosting());
    $('#tbl_CashDividend').DataTable({
        responsive: true,
        searching: true,
        scrollX: true,
        scrollY: true,
        paging: true,
        ordering: false,
        fixedColumns: true
    })
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
