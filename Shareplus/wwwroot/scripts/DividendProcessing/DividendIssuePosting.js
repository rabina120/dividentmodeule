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

function CashDividend(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.compcode);
        self.Divcode = ko.observable(data.divcode);
        self.Wissue_Approved = ko.observable(data.wissue_Approved)
        self.Shholderno = ko.observable(data.shHolderNo)
        self.AgmNo = ko.observable(data.agmNo)
        self.WarrantNo = ko.observable(data.warrantNo)
        self.WarrantAmt = ko.observable(data.warrantAmt)
        self.Taxdamt = ko.observable(data.taxDamt)
        self.Bonustax = ko.observable(data.bonustax)
        self.Bonusadj = ko.observable(data.bonusadj)
        self.Prevadj = ko.observable(data.prevadj)
        self.NetAmount = ko.observable(data.netAmount)
        self.Totshkitta = ko.observable(data.totShKitta)
        self.Dwiby = ko.observable(data.dwiby)
        self.Wissueddt = ko.observable(convertDate(data.wIssuedDt))
        self.Bonusadj = ko.observable(data.bonusadj)
        
        self.FName = ko.observable(data.attShholder.fName)
        self.LName = ko.observable(data.attShholder.lName)
        self.Selected = ko.observable();
    }
}

var CashDividendIssuePosting = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()

    self.PostingRemarks = ko.observable()
    self.ActionType = ko.observable()


    //Dividend ko table list lai
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.CashDividendTableName = ko.observable();

    //CashDividend Request
    self.compcode = ko.observable();
    self.Divcode = ko.observable();
    self.Wissue_Approved = ko.observable();
    self.Shholderno = ko.observable();
    self.AgmNo = ko.observable();
    self.WarrantNo = ko.observable();
    self.WarrantAmt = ko.observable();
    self.Taxdamt = ko.observable();
    self.Bonustax = ko.observable();
    self.Bonusadj = ko.observable();
    self.Prevadj = ko.observable();
    self.NetAmount = ko.observable();
    self.Totshkitta = ko.observable();
    self.Dwiby = ko.observable();
    self.Wissueddt = ko.observable();
    self.Bonusadj = ko.observable();
    self.Totshkitta = ko.observable();
    self.FName = ko.observable();
    self.LName = ko.observable();

    self.CashDividendList = ko.observable([]);

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

    /*self.PostingDate(formatDate(new Date))*/
    //Loading dividend lists select options
    self.LoadDividendLists = function () {
        if (self.ValidateCompany()) {
            var companyCode = SelectedCompany()
            $.ajax({
                type: "post", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/DividendProcessing/DividendIssueEntry/GetAllDividends',
                data: { 'CompCode': companyCode },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Dividend(item)
                        });
                        self.DividendLists(mappedTasks);
                    } else {
                        alert('warning', "No Dividend List Found ")
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })

        }
    }
    /*self.LoadDividendLists();*/

    //load Holder List waiting for Posting


    self.Validation = function (type) {
        var errMsg = "";
        if (type == "search") {
            if (self.SelectedCompany() == undefined) {
                errMsg += "Please Choose Company !!!</br>";
            }
            if (Validate.empty(self.SelectedDividend())) {
                errMsg += "Please Select Dividend !!!</br>";
            }
        }
        else {
            if (self.CashDividendList().filter(x => x.Selected()).length <= 0) {
                errMsg += "Please Tick the Cash Dividend Info !!!</br>";
            }
            if (Validate.empty(self.PostingRemarks())) {
                errMsg += "Please Enter Posting Remarks !!!</br>";
            }
            if (Validate.empty(self.PostingDate())) {
                errMsg += "Please Enter Posting Date !!!</br>";
            }
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }

    }
    self.SelectedCompany.subscribe(() => {
        if (!Validate.empty(self.SelectedCompany())) {
            self.LoadDividendLists();
        }
    })


    self.loadDataTable = function () {
        if (self.Validation('search')) {
            $('#tbl_CashDividend').DataTable().clear()
            $('#tbl_CashDividend').DataTable().destroy();
            Openloader()
            $.ajax({
                url: '/DividendProcessing/DividendIssuePosting/GetCashDividendForApproval',
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
                                return new CashDividend(item)
                            });
                            self.CashDividendList(mappedTasks);
                            
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
                        responsive: false,
                        searching: true,
                        scrollX: true,
                        scrollY: true,
                        paging: true,
                        ordering: false,
                        fixedHeader: true,
                        fixedColumns: true
                    });
                }
            })
        }

    }


    self.SelectAll = ko.computed({
        read: () => !self.CashDividendList().find(x => !x.Selected()),
        write: t => self.CashDividendList().forEach(x => x.Selected(t)),
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

    self.PostDividendSetUp = function (data) {
        if (self.Validation()) {
            var record = self.CashDividendList().filter(x => x.Selected()).map(x => ({
                ShHolderNo: x.Shholderno(),
                WarrantNo: x.WarrantNo()

            }));


            self.ActionType(data);
  
            var RecordDetails = {
                compcode: self.SelectedCompany(),
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
                url: '/DividendProcessing/DividendIssuePosting/PostCashDividendRequest',
                data: { 'aTTCashDividends': JSON.stringify(record) , 'RecordDetails': RecordDetails, 'ActionType': self.ActionType() },
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
                }
            })
            self.ClearControl()

        }
    }

    self.ClearControl = function () {
        $('#tbl_CashDividend').DataTable().clear()
        $('#tbl_CashDividend').DataTable().destroy();
        $('#tbl_CashDividend tbody').empty();

        $('#chk').prop('checked', false);
        self.PostingRemarks('')
        self.LoadDividendLists()
        //self.SelectedDividend(null)
        //self.SelectedDividend(undefined)
        $('#DividendList').val(null).trigger('change');
        self.CashDividendTableName('')
        $('#tbl_CashDividend').DataTable({
            responsive: false,
            searching: false,
            scrollX: true,
            scrollY: true,
            paging: false,
            ordering: false,
            fixedHeader:true
        })
    }


}
$(document).ready(function () {
    ko.applyBindings(new CashDividendIssuePosting());
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
