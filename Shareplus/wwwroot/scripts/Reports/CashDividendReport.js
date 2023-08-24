function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function PaymentCenter(data) {
    var self = this;
    if (data != undefined) {
        self.CenterId = ko.observable(data.centerId);
        self.CenterName = ko.observable(data.centerName);
        self.Address = ko.observable(data.address);
        self.NepaliName = ko.observable(data.nepaliName);
        self.NepaliAddress = ko.observable(data.nepaliAddress);
        self.TelNo = ko.observable(data.telNo);
        self.PaymentDisplayName = self.CenterId() + " " + self.CenterName()
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

function RejectedDividend(data) {
    var self = this;
    if (data != undefined) {
        self.rcompcode = ko.observable(data.compcode);
        self.rBoid = ko.observable(data.boid);
        self.rshholderno = ko.observable(data.shholderno);
        self.rFullname = ko.observable(data.fullname);
        self.rFiscalYr = ko.observable(data.fiscalYr);
        self.rbatchno = ko.observable(data.batchno);
        self.rbankaccno = ko.observable(data.bankaccno);
        self.rbankname = ko.observable(data.bankname);
        if (data.rejecteddate != null) {
            self.rrejecteddate = ko.observable(data.rejecteddate.slice(0, 10));
        }
        else {
            self.rrejecteddate = ko.observable(data.rejecteddate);
        }
        self.rrejectedby = ko.observable(data.rejectedby);
        self.rremarks = ko.observable(data.remarks);
    }
}
function CDividend(data) {
    var self = this;
    if (data != undefined) {
        self.dshholderno = ko.observable(data.shholderno);
        self.dBO_idno = ko.observable(data.bO_idno);
        self.dfullname = ko.observable(data.fullname);
        self.dwarrantno = ko.observable(data.warrantno);
        self.dwarrantamt = ko.observable(data.warrantamt);
        self.dtaxdamt = ko.observable(data.taxdamt);
        self.dbonustax = ko.observable(data.bonustax);
        self.dbonusadj = ko.observable(data.bonusadj);
        if (data.wIssuedDt != null) {
            self.dWIssuedDt = ko.observable(data.wIssuedDt.slice(0, 10));
        } else {
            self.dWIssuedDt = ko.observable(data.wIssuedDt);

        }
        self.ddwiby = ko.observable(data.dwiby);
        self.dremarks = ko.observable(data.remarks);
        self.dtotshkitta = ko.observable(data.totshkitta);
        self.dbankname = ko.observable(data.bankname);
        self.dbankaccno = ko.observable(data.bankaccno);
        if (data.creditedDt != null) {
            self.dcreditedDt = ko.observable(data.creditedDt.slice(0, 10));
        }
        else {
            self.dcreditedDt = ko.observable(data.creditedDt);

        }
        self.dnetamt = ko.observable(data.warrantamt - data.taxdamt - data.bonusadj - data.bonustax);
    }
}
function ShownerType(data) {
    var self = this;
    if (data != undefined) {
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.ShOwnerTypeName = ko.observable(data.shOwnerTypeName);
        self.ShOwnerTypeAndName = self.ShOwnerType() + " " + self.ShOwnerTypeName();
    }
}

function ShareType(data) {
    var self = this;
    if (data != undefined) {
        self.ShareTypeValue = ko.observable(data.shareTypeID);
        self.ShareTypeName = ko.observable(data.shareType);
        self.ShareTypeNameAndValue = self.ShareTypeValue() + " " + self.ShareTypeName();

    }
}



var CashDividendReport = function () {
    //Companykolagi 
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    //Dividend ko table list lai
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.compcode = ko.observable();
    self.Divcode = ko.observable();
    self.Description = ko.observable();
    self.FiscalYr = ko.observable();
    self.ShowDividendText = ko.observable();

    self.batchNo = ko.observable();


    //Occupation type kolagi
    self.OccupationTypeList = ko.observableArray([])
    self.OccupationTypeName = ko.observable();
    self.OccupationTypeValue = ko.observable();
    self.OccupationTypeList.push(
        { "OccupationTypeName": "Public", "OccupationTypeValue": "1" },
        { "OccupationTypeName": "Promoter", "OccupationTypeValue": "2" }
        );
    self.SelectedOccupationType = ko.observable()

    //date

    self.dateFrom = ko.observable();
    self.dateTo = ko.observable();

    self.payType = ko.observable("A");
    self.BasedOn = ko.observable("Posted");
    self.ShareTypeBasedOn = ko.observable('P');

    self.kittaTo = ko.observable();
    self.kittaFrom = ko.observable();

    self.seqNoFrom = ko.observable();
    self.seqNoTo = ko.observable();

    self.withBankDetails = ko.observable(false);


    //rejected 
    self.rcompcode = ko.observable();
    self.rBoid = ko.observable();
    self.rshholderno = ko.observable();
    self.rFullname = ko.observable();
    self.rFiscalYr = ko.observable();
    self.rbatchno = ko.observable();
    self.rbankaccno = ko.observable();
    self.rbankname = ko.observable();
    self.rrejecteddate = ko.observable();
    self.rrejectedby = ko.observable();
    self.rremarks = ko.observable();
    self.RejectedDividendLists = ko.observableArray([]);

    //unissued and issued

    self.dshholderno = ko.observable();
    self.dBO_idno = ko.observable();
    self.dfullname = ko.observable();
    self.dwarrantno = ko.observable();
    self.dwarrantamt = ko.observable();
    self.dtaxdamt = ko.observable();
    self.dbonustax = ko.observable();
    self.dbonusadj = ko.observable();
    self.dWIssuedDt = ko.observable();
    self.ddwiby = ko.observable();
    self.dremarks = ko.observable();
    self.dtotshkitta = ko.observable();
    self.dbankname = ko.observable();
    self.dbankaccno = ko.observable();
    self.dcreditedDt = ko.observable();
    self.dnetamt = ko.observable();
    self.CDividendLists = ko.observableArray([]);


    //Payment Center
    self.CenterId = ko.observable();
    self.CenterName = ko.observable();
    self.Address = ko.observable();
    self.NepaliName = ko.observable
    self.NepaliAddress = ko.observable();
    self.TelNo = ko.observable();
    self.SelectedPaymentCenter = ko.observable();
    self.PaymentCenters = ko.observableArray([]);

    var FiscalYr, CompEnName;

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

    //Loading dividend lists select options
    self.LoadDividendLists = function () {
        if (self.ValidateCompany()) {
            var companyCode = self.SelectedCompany()
            $.ajax({
                type: "post",
                url: '/DividendProcessing/DividendIssueEntry/GetAllDividends',
                data: { 'CompCode': companyCode },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Dividend(item)
                        });
                        self.DividendLists(mappedTasks);
                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })

        }
    }

    self.SelectedDividend.subscribe(function () {
        FiscalYr = self.DividendLists().find(x => x.Divcode() == self.SelectedDividend()).FiscalYr();
    })
    self.SelectedCompany.subscribe(function () {
        self.LoadDividendLists();
        self.LoadPaymentCenter();
        //CompEnName = self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName();
    })
    //load payment centers
    self.LoadPaymentCenter = function () {
        if (self.ValidateCompany()) {
            $.ajax({
                type: "post",
                url: '/DividendProcessing/DividendIssueEntry/GetAllPaymentCenter',
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new PaymentCenter(item)
                        });
                        self.PaymentCenters(mappedTasks);
                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.statusText)
                }
            })

        }
    }
   
    self.LoadCompany();
    self.LoadDividendLists();
    self.LoadPaymentCenter();
 
  
   
    self.GenerateData = function (exportType) {
        $('#tbl_CashDividendReport').DataTable().clear();
        $('#tbl_CashDividendReport').DataTable().destroy();
        if (self.ValidateCompany()) {
            if (self.ValidateForm()) {
                var e = document.getElementById("reportSelect2");
                var value = e.options[e.selectedIndex].value;// get selected option value
                var text = e.options[e.selectedIndex].text;
                var shareType = self.ShareTypeBasedOn() == "P" ? "Physical" : "Demate";// gets text

                var u = document.getElementById("undoType");
                var undoType = u.options[u.selectedIndex].value;// get selected option value


                text = text + shareType;
                CompEnName = self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName();
                Openloader()
                
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/Reports/CashDividendReport/GenerateDataForReport',
                    data: {
                        'CompCode': self.SelectedCompany(), 'CompEnName': CompEnName, 'FiscalYr': FiscalYr, 'DivCode': self.SelectedDividend(), 'SelectedReportType': value, 'UndoType': undoType, 'SelectedReportName': text, 'seqNoFrom': self.seqNoFrom(), 'seqNoTo': self.seqNoTo(),
                        'KittaFrom': self.kittaFrom(), 'KittaTo': self.kittaTo(), 'DateFrom': self.dateFrom(), 'DateTo': self.dateTo, 'Posted': self.BasedOn(), 'PaymentType': self.payType(), 'PaymentCenter': self.SelectedPaymentCenter(), 'Occupation': self.SelectedOccupationType(),
                        'BatchNo': self.batchNo(), 'WithBankDetails': self.withBankDetails(), 'ShareType': self.ShareTypeBasedOn(), 'ExportFileType': exportType, 'FiscalYear': self.SelectedDividend()!=undefined? self.DividendLists().find(x => x.Divcode() == self.SelectedDividend()).FiscalYr():null
                    },
                    dataType: 'json',
                    success: function (result) {
                        if (result.hasError) {
                            alert('error', result.message);
                        } else {
                            if (result.isSuccess) {
                                if (exportType == 'E') {
                                    var fileName = result.message;
                                    var a = document.createElement("a");
                                    a.href = "data:application/octet-stream;base64," + result.responseData;
                                    a.download = fileName;
                                    a.click();
                                }
                                else {
                                    var byteArray = Uint8Array.from(atob(result.responseData), c => c.charCodeAt(0)); // Decode base64 data
                                    var pdfBlob = new Blob([byteArray], { type: 'application/pdf' });
                                    var x = window.open();
                                    if (x) {
                                        x.document.open();
                                        x.document.write('<iframe width="100%" height="100%" src="' + URL.createObjectURL(pdfBlob) + '"></iframe>');
                                        x.document.title = result.message;
                                        x.document.close();
                                    } else {
                                        alert('warning', 'Failed to open file.');
                                    }
                                }
                            }
                            else {
                                alert('error', result.message);
                            }
                        }
                    }, error: function (error) {
                        alert('error', error.statusText)
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
                
            }
        }
    }


    self.ValidateForm = function () {
        var errMsg = "";
        if (!$('#reportSelect2').val()) {
            errMsg += "Please Select Report Type !!!</br>";
            alert('error', errMsg);
            return false;
        }
        if ($('#reportSelect2').val() != "RL") {
            if (Validate.empty(self.SelectedDividend())) {
                errMsg += "Please Select Dividend !!!</br>";
                alert('error', errMsg);
                return false;
            }
        }

        if (Validate.empty(self.BasedOn())) {
            errMsg += "Select Posted or Unposted !!!<br/>";
        }
        if (Validate.empty(self.payType())) {
            errMsg += "Select Payment Type !!!<br/>";

        }
        if (errMsg != "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
    }
    self.ShowHolderNo = () => {
        //$('.seq').show();

    }
    self.HideHolderNo = () => {
        //$('.seq').hide();


    }
}

$(document).ready(function () {

    $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date1 .input-group.date").datepicker("setDate", new Date());

    $('#simple-date2 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date2 .input-group.date").datepicker("setDate", new Date());

    $('#tbl_CashDividendPhysicalReport').DataTable({
        responsive: true,
        "searching": false,
        scrollX: true,
        scrollY: true,
        scrollCollapse: true,
        paging: false,
        ordering: false,
        fixedHeader: true
    })
    $('#tbl_CashDividendDemateReport').DataTable({
        responsive: true,
        "searching": false,
        scrollX: true,
        scrollY: true,
        scrollCollapse: true,
        paging: false,
        ordering: false,
    })
    $('#tbl_CashDividendPhysicalUnIssuedReport').DataTable({
        responsive: true,
        "searching": false,
        scrollX: true,
        scrollY: true,
        scrollCollapse: true,
        paging: false,
        ordering: false,
        fixedHeader: true
    })
    $('#tbl_CashDividendDemateUnIssuedReport').DataTable({
        responsive: true,
        "searching": false,
        scrollX: true,
        scrollY: true,
        scrollCollapse: true,
        paging: false,
        ordering: false,
    })
    $('#tbl_CashDividendPhysicalUnIssuedWithBankReport').DataTable({
        responsive: true,
        "searching": false,
        scrollX: true,
        scrollY: true,
        scrollCollapse: true,
        paging: false,
        ordering: false,
        fixedHeader: true
    })
    $('#tbl_CashDividendDemateUnIssuedWithBankReport').DataTable({
        responsive: true,
        "searching": false,
        scrollX: true,
        scrollY: true,
        scrollCollapse: true,
        paging: false,
        ordering: false,
    })
    $('#tbl_RejectedDividendReport').DataTable({
        responsive: true,
        "searching": false,
        scrollX: true,
        scrollY: true,
        scrollCollapse: true,
        paging: false,
        ordering: false,
    })

    $('#CashPhysicalDividendTableHide,#CashPhysicalDividendUnIssuedTableHide,#CashPhysicalDividendUnIssuedWithBankTableHide,#CashDemateDividendTableHide,#CashDemateDividendUnIssuedTableHide,#CashDemateDividendUnIssuedWithBankTableHide,#RejectedDividendTableHide').css('display', 'none');
    document.getElementById("reportSelect2").onchange = function () { ShowHideDiv($('#reportSelect2').find(":selected").val()) };

    ShowHideDiv($('#reportSelect2').find(":selected").val());

    function ShowHideDiv(data) {
        if (data == "LUDW") {
            $('#withBankDetaisDiv').css('visibility', 'hidden');
        }
        else {
            $('#withBankDetaisDiv').css('visibility', 'visible');
        }

        if (data == "SRIP" || data == "RL") {
            $('#ShowHideSummary').css('display', 'none');
        }
        else {
            $('#ShowHideSummary').css('display', 'block');
        }
        if (data == "RL") {
            $('#dividendListDiv,#ShareTypeDiv').css('display', 'none');
        }
        if (data == "UNDO") {
            $('#SelectUndoType').show();
            $('#StatusType').hide();
            $('#undoType').val('ALL');
        }
        if (data != "UNDO") {
            $('#StatusType').show();
            $('#SelectUndoType').hide();
        }
        else {
            $('#dividendListDiv,#ShareTypeDiv').css('display', 'block');

        }
    }

    $('#withBankDetaisDiv').css('visibility', 'visible');
    $('#SelectUndoType').hide();

    ko.applyBindings(new CashDividendReport());
});