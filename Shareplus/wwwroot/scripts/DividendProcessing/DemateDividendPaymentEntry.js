function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
    }
}
function PaymentCenter(data) {
    var self = this;
    if (data != undefined) {
        self.CenterId = ko.observable(data.centerId);
        self.CenterName = ko.observable(data.centerName);

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

function CashDividend(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.Compcode)
        self.WarrantNo = ko.observable(data.warrantno)
        self.ShHolderNo = ko.observable(data.BO_idno)
        self.FullName = ko.observable(data.fullname)
        self.FaName = ko.observable(data.faname)
        self.GrFaName = ko.observable(data.grfaname)
        self.AgmNo = ko.observable(data.agmNo)
        self.seqno = ko.observable(data.seqno)
        self.DPayable = ko.observable(data.dPayable)
        self.WarrantAmt = ko.observable(data.warrantamt)
        self.TaxDamt = ko.observable(data.taxdamt)
        self.bonustax = ko.observable(data.bonustax)
        self.bonusadj = ko.observable(data.bonusadj)
        self.prevadj = ko.observable(data.prevadj)
        self.TotShKitta = ko.observable(data.totshkitta)
        self.WIssued = ko.observable(data.wIssued)
        self.WIssuedDt = ko.observable(data.wAmtPaidDt)
        self.creditedDt = ko.observable(data.crediteddt)
        self.dwiby = ko.observable(data.dwiby)
        self.CenterId = ko.observable(data.centerId)
        self.remarks = ko.observable(data.remarks)
        self.Payment = ko.observable(data.Payment)
        self.PayUser = ko.observable(data.PayUser)
        self.isin_no = ko.observable(data.isin_no)
        self.Batchno = ko.observable(data.batchno)
        self.wissue_Approved = ko.observable(data.wissue_Approved)
        self.wissue_approvedby = ko.observable(data.wissue_Approvedby)
        self.wissue_app_date = ko.observable(data.wissue_app_date)
        self.wissue_status = ko.observable(data.wissue_status)
        self.wissue_auth_remarks = ko.observable(data.wissue_auth_remarks)
        self.WPaid = ko.observable(data.WPaid)

        self.creditedDt = ko.observable(data.crediteddt)

        self.WPaidBy = ko.observable(data.wPaidBy)
        self.wpaid_approved = ko.observable(data.wpaid_approved)
        self.wpaid_app_date = ko.observable(data.wpaid_app_date)
        self.wpaid_status = ko.observable(data.wpaid_status)
        self.wpaid_auth_remarks = ko.observable(data.wpaid_auth_remarks)
        self.wpaid_approvedby = ko.observable(data.wpaid_approvedby)
        self.bankname = ko.observable(data.bankname)
        self.bankaccno = ko.observable(data.bankaccno)
        self.bankadd = ko.observable(data.bankadd)
        self.crediteddt = ko.observable(data.crediteddt)


        self.bankName = ko.observable(data.bankName);
        self.accountNo = ko.observable(data.bankaccno);
    }
}


var DemateDividendPaymentEntry = function () {
    self.BOIDNO = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.SelectedCompany = ko.observable();
    self.CompanyDetails = ko.observableArray([]);

    self.BasedOn = ko.observable();
    self.BasedOn = ko.observable("S");

    self.DividendLists = ko.observableArray([]);
    self.ShowDividendText = ko.observable();
    self.Divcode = ko.observable();
    self.SelectedDividend = ko.observable();
    self.FullName = ko.observable();
    self.Address = ko.observable();
    self.FaName = ko.observable();
    self.GrFaName = ko.observable();
    self.TotalKitta = ko.observable();
    self.AgmNo = ko.observable();
    self.ShholderNoNew = ko.observable();
    self.SelectedAction = ko.observable();

    self.WarrantNo = ko.observable();
    self.WarrantAmt = ko.observable();
    self.TaxDamt = ko.observable();
    self.bonustax = ko.observable();
    self.prevadj = ko.observable();
    self.bonusadj = ko.observable();
    self.TotalPayable = ko.observable();
    self.WIssuedDt = ko.observable();
    self.creditedDt = ko.observable();
    self.PaymentCenters = ko.observableArray([]);
    self.PaymentDisplayName = ko.observable();
    self.SelectedPaymentCenter = ko.observable();
    self.remarks = ko.observable();
    self.Payment = ko.observable();
    self.PayUser = ko.observable();

    //while saving
    self.whileSaveBoidNo = ko.observable();
    self.whileSaveWarrantNo = ko.observable();

    self.bankName = ko.observable();
    self.accountNo = ko.observable();

    self.CenterId = ko.observable();
    self.CenterName = ko.observable();

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

    
    self.ValidateLength = function () {

        var errMsg = "";
        if (self.BasedOn() == "S") {
            if (self.BOIDNO().length != 16) {
                errMsg = "BOID Should Be 16 Digits.";
                alert('error', errMsg);
                document.getElementById("textBasedOnPlace").focus();
                return false;
            }
        }
        else {
            if (self.BOIDNO().length != 10) {
                var warn = self.BOIDNO();
                warn = warn.padStart(10, '0');
                self.BOIDNO(warn)

            }
        }
        return true;
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

    //Loading dividend lists select options
    self.LoadDividendLists = function () {
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            $.ajax({
                type: "post", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/DividendProcessing/DemateDividendIssueEntry/GetAllDemateDividends',
                data: { 'CompCode': companyCode },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Dividend(item)
                        });
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
    // load payment centers
    self.LoadPaymentCenter = function () {
        $.ajax({
            type: "post", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            url: '/DividendProcessing/DividendIssueEntry/GetAllPaymentCenter',
            datatype: "json",
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
                alert('error', error.message)
            }
        })
    }
    self.LoadPaymentCenter();

    //changing html on select
    self.BasedOn.subscribe(function () {
        if (self.BasedOn() == 'W') {
            $("#textBasedOn").html("Warrant No:");
            $("#textBasedOn2").html("BOID No:");
            self.clearControls();

        }
        else {
            $("#textBasedOn").html("BOID No:");
            $("#textBasedOn2").html("Warrant No:");
            self.clearControls();

        }
    })
    //enabling holderno input on change
    self.SelectedDividend.subscribe(function () {
        if (self.SelectedDividend() != "") {
            document.getElementById('textBasedOnPlace').disabled = false;
            document.getElementById('textBasedOnPlace').focus();
        }
    });

    self.SaveDemateDividendPayment = function () {
        if (self.ValidateForm() && self.ValidateLength() && validateBeforeSave()) {

            if (self.SelectedAction() == "A") {
                var text = "The Dividend will be Paid.";
            }
            else {
                text = "The Dividend will be Unpaid.";
            }
            swal({
                title: "Are you sure?",
                text: text,
                icon: "warning",
                buttons: true,
                dangerMode: true
            }).then((willSave) => {
                if (willSave) {
                    var maxSeqNo;
                    Openloader()
                    $.ajax({
                        type: "Post", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        url: '/DividendProcessing/DemateDividendPaymentEntry/SaveDemateDividendPaymentEntry',
                        data: {
                            'DivCode': self.SelectedDividend(),
                            'CompCode': self.SelectedCompany(),
                            'bankName': self.bankName(),
                            'accountNo': self.accountNo(),
                            'centerid': self.SelectedPaymentCenter(),
                            'remarks': self.remarks(),
                            'Payment': self.Payment(),
                            'PayUser': self.PayUser(),
                            'warrantNo': self.whileSaveWarrantNo(),
                            'boidno': self.whileSaveBoidNo(),
                            'selectedAction': self.SelectedAction(),
                            'creditedDt': self.creditedDt(),
                            'wissueddate': self.WIssuedDt()

                        },
                        datatype: "json",
                        success: function (result) {
                            if (result.isSuccess) {
                                swal({
                                    title: "Demate Dividend",
                                    text: result.message,
                                    type: "success"
                                }).then(function () {
                                    location.reload();
                                });

                            } else {
                                alert('warning', result.message);
                            }
                        },
                        error: function (error) {
                            alert('error', error.message)
                        },
                        complete: () => {
                            Closeloader()
                        }
                    });
                }
            });
        }

    }

    self.Refresh = function () {
        self.SelectedDividend("");
        self.EnableAllButtons();
        self.clearControls();
        document.getElementById('textBasedOnPlace').disabled = true;
        document.getElementById('DividendTable').disabled = true;

    }

    self.searchData = function () {

        if (Validate.empty(self.BOIDNO())) {
            self.clearControls();
        }


        else {
            if (self.ValidateLength()) {
                var action = self.BasedOn();
                $.ajax({
                    type: "post", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/DividendProcessing/DemateDividendPaymentEntry/GetDemateDividendInformation',
                    data: {
                        'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend(),
                        'shholderno': self.BOIDNO(), 'a': action, 'ActionType': self.SelectedAction()
                    },
                    datatype: "json",
                    success: function (result) {
                        if (result.isSuccess) {
                            self.FullName(result.responseData.fullname);
                            self.Address(result.responseData.address);
                            self.FaName(result.responseData.faname);
                            self.GrFaName(result.responseData.grfaname);
                            self.TotalKitta(result.responseData.totshkitta);
                            self.AgmNo(result.responseData.agmNo);
                            if (self.BasedOn() == "S") {
                                self.WarrantNo(result.responseData.warrantno);
                                self.whileSaveWarrantNo(result.responseData.warrantno);
                                self.whileSaveBoidNo(self.BOIDNO());
                            }
                            else {
                                self.WarrantNo(result.responseData.bO_idno);
                                self.whileSaveWarrantNo(self.BOIDNO());
                                self.whileSaveBoidNo(result.responseData.bO_idno);
                            }
                            self.bankName(result.responseData.bankname);
                            self.accountNo(result.responseData.bankaccno);

                            self.ShholderNoNew(result.responseData.bO_idno);
                            self.WarrantAmt(result.responseData.warrantamt);
                            self.TaxDamt(result.responseData.taxdamt);
                            self.bonustax(result.responseData.bonustax);
                            self.bonusadj(result.responseData.bonusadj);
                            self.prevadj(result.responseData.prevadj);
                            self.TotalPayable(result.responseData.warrantamt - result.responseData.taxdamt - result.responseData.bonustax - result.responseData.bonusadj - result.responseData.prevadj);
                            var wissueddate = result.responseData.wAmtPaidDt;
                            if (wissueddate != null) {
                                wissueddate = wissueddate.substr(0, 10);
                                
                            }
                            self.WIssuedDt(wissueddate);
                            var creditedDate = result.responseData.crediteddt;
                            if (creditedDate != null) {
                                creditedDate = creditedDate.substr(0, 10);

                            }
                            self.creditedDt(creditedDate);


                            self.remarks(result.responseData.remarks);
                            self.Payment(result.responseData.Payment);
                            self.PayUser(result.responseData.PayUser);
                            self.SelectedPaymentCenter(self.PaymentCenters().find(x => x.CenterId() == result.responseData.centerId));
                            if (self.SelectedPaymentCenter() != null) {
                                document.getElementById("select2-payCenterSelect-container").innerHTML = ko.toJS(self.SelectedPaymentCenter().PaymentDisplayName)
                            }

                        } else {
                            alert('warning', result.message)
                            self.clearControls()

                        }
                    },
                    error: function (error) {
                        alert('error', error.message)

                    }
                });
            }
        }

    }
    self.clearControls = function () {
        self.BOIDNO("");
        self.FullName("");
        self.Address("");
        self.FaName("");
        self.GrFaName("");
        self.TotalKitta("");

        self.AgmNo("");
        self.WarrantNo("");
        self.WarrantAmt("");
        self.TaxDamt("");
        self.bonustax("");
        self.bonusadj("");
        self.TotalPayable("");
        self.WIssuedDt("");
        self.creditedDt("");

        self.bankName("");
        self.accountNo("");

        self.remarks("");
        self.Payment("");
        self.PayUser("");
        $('#payCenterSelect').val(null).trigger('change');
    }
    self.chooseOptions = function (data) {
        document.getElementById('DividendTable').disabled = false;

        $('#AddButton,#DeleteButton').attr('disabled', true);
        $('#SaveButton').attr('disabled', false);

        self.SelectedAction(data);
        switch (data) {
            case "A":
                $('#wIssued,#WPaid,#creditedDt,#bankName,#accountNo,#payCenterSelect,#remarks').attr('disabled', false);
                break;
            case "D":
                $('#wIssued,#WPaid,#creditedDt,#bankName,#accountNo,#payCenterSelect,#remarks').attr('disabled', true);


                break;
        }

    };
    self.validateBeforeSave = function () {
        var errMsg = "";

        if (self.SelectedAction() == "A") {
            if (Validate.empty(self.SelectedPaymentCenter())) {
                errMsg += "Please Select Payment Center !!!</br>";

            }
            if (Validate.empty(self.remarks())) {
                errMsg += "Please Enter Remarks !!!</br>";

            }
            if (Validate.empty(self.bankName())) {
                errMsg += "Please Add Bank Name !!!</br>";
            } if (Validate.empty(self.accountNo())) {
                errMsg += "Please Add Account No !!!</br>";
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

    self.ValidateForm = function () {
        var errMsg = "";

        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select Dividend !!!</br>";

        }
        if (Validate.empty(self.BOIDNO())) {
            errMsg += "Please Enter BOIDNO or Warrant No !!!</br>";

        }
        if (Validate.empty(self.BasedOn())) {
            errMsg += "Please Choose BOIDNO or Warrant No !!!</br>";
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
    document.getElementById('textBasedOnPlace').disabled = true;
    document.getElementById('DividendTable').disabled = true;
    // Select2 Single  with Placeholder for Company

    // Bootstrap Date Picker


    $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    }).datepicker('update', new Date());




    ko.applyBindings(new DemateDividendPaymentEntry());

});