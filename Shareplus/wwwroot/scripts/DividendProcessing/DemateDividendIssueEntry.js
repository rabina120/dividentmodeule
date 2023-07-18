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




function CashDividend(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.Compcode)
        self.WarrantNo = ko.observable(data.warrantno)
        self.ShHolderNo = ko.observable(data.BO_idno)
        self.FullName = ko.observable(data.fullname)
        self.FaName = ko.observable(data.faname)
        self.GrFaName = ko.observable(data.grfaname)
        self.Address = ko.observable(data.address)
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
        self.WIssuedDt = ko.observable(data.wIssuedDt)
        self.creditedDt = ko.observable(data.crediteddt)
        self.CenterId = ko.observable(data.centerId)
        self.remarks = ko.observable(data.remarks)
        self.isin_no = ko.observable(data.isin_no)
        self.Batchno = ko.observable(data.batchno)
        self.wissue_Approved = ko.observable(data.wissue_Approved)
        self.wissue_approvedby = ko.observable(data.wissue_Approvedby)
        self.wissue_app_date = ko.observable(data.wissue_app_date)
        self.wissue_status = ko.observable(data.wissue_status)
        self.wissue_auth_remarks = ko.observable(data.wissue_auth_remarks)
        self.WPaid = ko.observable(data.WPaid)
        self.WAmtPaidDt = ko.observable(data.wAmtPaidDt)
        self.WPaidBy = ko.observable(data.wPaidBy)
        self.wpaid_approved = ko.observable(data.wpaid_approved)
        self.wpaid_app_date = ko.observable(data.wpaid_app_date)
        self.wpaid_status = ko.observable(data.wpaid_status)
        self.wpaid_auth_remarks = ko.observable(data.wpaid_auth_remarks)
        self.wpaid_approvedby = ko.observable(data.wpaid_approvedby)
        self.bankname = ko.observable(data.bankname)
        self.bankaccno = ko.observable(data.bankaccno)
        self.bankName = ko.observable(data.bankName);
        self.accountNo = ko.observable(data.accountNo);

    }
}



var DemateDividendIssueEntry = function () {
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

    //shholder or warrant checkbox
    self.BasedOn = ko.observable();
    self.BasedOn = ko.observable("S");
    self.IsPaidBy = ko.observable(true);


    self.SelectedAction = ko.observable();

    //data
    self.WarrantNo = ko.observable();
    self.BOIDNO = ko.observable();
    self.AgmNo = ko.observable();
    self.seqno = ko.observable();
    self.DPayable = ko.observable();
    self.WarrantAmt = ko.observable();
    self.TaxDamt = ko.observable();
    self.bonustax = ko.observable();
    self.bonusadj = ko.observable();
    self.prevadj = ko.observable();
    self.TotShKitta = ko.observable();
    self.WIssued = ko.observable();
    self.WIssuedDt = ko.observable();
    self.creditedDt = ko.observable();
    self.dwiby = ko.observable();
    self.wissue_Approved = ko.observable();
    self.wissue_app_date = ko.observable();
    self.wissue_status = ko.observable();
    self.wissue_auth_remarks = ko.observable();
    self.wissue_approvedby = ko.observable();
    self.WPaid = ko.observable();
    self.WAmtPaidDt = ko.observable();
    self.WPaidBy = ko.observable();
    self.wpaid_approved = ko.observable();
    self.wpaid_app_date = ko.observable();
    self.wpaid_status = ko.observable();
    self.wpaid_auth_remarks = ko.observable();
    self.wpaid_approvedby = ko.observable();
    self.CashOrTran = ko.observable();
    self.centerid = ko.observable();
    self.telno = ko.observable();
    self.remarks = ko.observable();
    self.batchno = ko.observable();
    self.TotalPayable = ko.observable();

    self.ShholderNoNew = ko.observable();

    //while saving
    self.whileSaveBoidno = ko.observable();
    self.whileSaveWarrantoNo = ko.observable();

    self.FName = ko.observable();
    self.LName = ko.observable();
    self.Address = ko.observable();
    self.DistCode = ko.observable();
    self.PboxNo = ko.observable();
    self.FaName = ko.observable();
    self.GrFaName = ko.observable();
    self.MobileNo = ko.observable();
    self.FullName = ko.observable();
    self.TotalKitta = ko.observable();

    //Payment Center
    self.CenterId = ko.observable();
    self.CenterName = ko.observable();
    self.Address = ko.observable();
    self.NepaliName = ko.observable
    self.NepaliAddress = ko.observable();
    self.TelNo = ko.observable();
    self.SelectedPaymentCenter = ko.observable();
    self.PaymentCenters = ko.observableArray([]);

    self.bankName = ko.observable();
    self.accountNo = ko.observable();



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

    //load payment centers
    self.LoadPaymentCenter = function () {
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
            //document.getElementById('textBasedOnPlace').focus();
        }
    });


    self.chooseOptions = function (data) {
        document.getElementById('DividendTable').disabled = false;
        document.getElementById('textBasedOnPlace').disabled = false;
        $('#AddButton,#DeleteButton').attr('disabled', true);
        $('#SaveButton').attr('disabled', false);

        self.SelectedAction(data);
        switch (data) {
            case "A":
                $('#wIssued,#creditedDt,#payCenterSelect,#remarks,#bankName,#accountNo,#DividentPayment').attr('disabled', false);
                break;
            case "D":
                $('#wIssued,#creditedDt,#payCenterSelect,#remarks,#bankName,#accountNo,#DividentPayment').attr('disabled', true);
                break;
        }

    };

    //finding record from the database
    self.searchData = function () {
        var errormsg = "";
        if (Validate.empty(self.BOIDNO()) || Validate.empty(self.SelectedDividend())) {
            return;
            
        }
       
        else {
            if (self.ValidateLength()) {
                Openloader()
                var action = self.BasedOn();
                $.ajax({
                    type: "post", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/DividendProcessing/DemateDividendIssueEntry/GetDemateDividendInformation',
                    data: {
                        'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend(),
                        'shholderno': self.BOIDNO(), 'actionType': action, 'SelectedAction': self.SelectedAction()
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
                                self.whileSaveWarrantoNo(result.responseData.warrantno);
                                self.whileSaveBoidno(self.BOIDNO());
                            }
                            else {
                                self.WarrantNo(result.responseData.bO_idno);
                                self.whileSaveWarrantoNo(self.BOIDNO());
                                self.whileSaveBoidno(result.responseData.bO_idno);
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

                            var wissueddate = result.responseData.wIssuedDt;
                            if (wissueddate != null) {
                                wissueddate = wissueddate.substr(0, 10);
                            }
                            self.WIssuedDt(wissueddate);

                            var creditedDate = result.responseData.crediteddt;
                            if (creditedDate != null) {
                                creditedDate = creditedDate.substr(0, 10);
                            }
                            self.creditedDt(creditedDate);


                            self.telno(result.responseData.telno);
                            if (self.SelectedAction() == "A")
                                $('#wIssued,#payCenterSelect,#remarks').attr('disabled', false);
                            else {
                                $('#wIssued,#payCenterSelect,#remarks').attr('disabled', true);
                                self.remarks(result.responseData.remarks);
                                self.SelectedPaymentCenter(self.PaymentCenters().find(x => x.CenterId() == result.responseData.centerId));
                                if (self.SelectedPaymentCenter() != null) {
                                    document.getElementById("select2-payCenterSelect-container").innerHTML = ko.toJS(self.SelectedPaymentCenter().PaymentDisplayName)
                                }
                            }

                        } else {
                            $('#textBasedOnPlace').focus()
                            alert('warning', result.message)
                            self.clearControls()
                            
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                        //$('#textBasedOnPlace').focus()
                        //self.clearControls()
                       
                    }, complete: () =>{
                        Closeloader()
                        
                    }
                });
            }
        }

    }

    //saving data
    self.SaveDemateDividend = function () {
        if (self.ValidateForm() && self.ValidateLength()) {
                if (self.SelectedAction() == "A") {
                    var text = "The Dividend will be issued.";
                }
                else {
                    text = "The Dividend will be unissued.";
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
                            type: "POST",
                            url: '/DividendProcessing/DemateDividendIssueEntry/SaveCashDemateDividend',
                            data: {
                                'DivCode': self.SelectedDividend(),
                                'CompCode': self.SelectedCompany(),
                                'centerid': self.SelectedPaymentCenter().CenterId,
                                'remarks': self.remarks(),
                                'bankName': self.bankName(),
                                'accountNo': self.accountNo(),
                                'compcode': self.SelectedCompany(),
                                'warrantNo': self.whileSaveWarrantoNo(),
                                'boidno': self.whileSaveBoidno(),
                                'selectedAction': self.SelectedAction(),
                                'wissueddate': self.WIssuedDt(),
                                'creditedDt': self.creditedDt(),
                                  'IsPaidBy': self.IsPaidBy()

                            },
                            datatype: "json", beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
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


    self.clearControls = function () {
        self.BOIDNO("");
        self.FullName("");
        self.Address("");
        self.FaName("");
        self.GrFaName("");
        self.TotalKitta("");
        self.DistCode("");
        self.PboxNo("");
        self.MobileNo("");
        self.bankName("");
        self.accountNo("");
        self.AgmNo("");
        self.WarrantNo("");
        self.WarrantAmt("");
        self.TaxDamt("");
        self.bonustax("");
        self.bonusadj("");
        self.prevadj("");
        self.TotalPayable("");
        self.WIssuedDt("");
        self.creditedDt("");
        self.telno("");
        self.remarks("");
        self.SelectedPaymentCenter('')
        self.SelectedDividend('')
        $('#DividendTable').val('').trigger('change');
        $('#payCenterSelect').val('').trigger('change');
    }
    self.EnableAllButtons = function () {
        document.getElementById("AddButton").disabled = false;
        document.getElementById("DeleteButton").disabled = false;
        document.getElementById("SaveButton").disabled = true;
    }
    self.ButtonsOnLoad = function () {
        document.getElementById("SaveButton").disabled = true;
        $('#wIssued,#payCenterSelect,#remarks').attr('disabled', true);

    }
    self.ButtonsOnLoad();

    self.Refresh = function () {
        self.SelectedDividend("");
        self.EnableAllButtons();
        self.clearControls();
        document.getElementById('textBasedOnPlace').disabled = true;
        document.getElementById('DividendTable').disabled = true;

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
        //else {
        //    if (self.BOIDNO().length != 10) {
        //        var warn = self.BOIDNO();
        //        warn = warn.padStart(10, '0');
        //        self.BOIDNO(warn)

        //    }
        //}
        return true;
    }
    self.ValidateForm = function () {
        var errMsg = "";

        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select Dividend !!!</br>";
            
        }
        if (Validate.empty(self.BOIDNO())) {
            errMsg += "Please Enter ShholderNo or Warrant No !!!</br>";
            
        }
        if (Validate.empty(self.BasedOn())){
            errMsg += "Please Choose ShholderNo or Warrant No !!!</br>";
        }
        
        if (self.SelectedAction() == "A") {
            if (Validate.empty(self.WIssuedDt())) {
                errMsg += "Please Add The Issued Date !!!</br>";
            }
            if (self.IsPaidBy()) {
                if (Validate.empty(self.creditedDt())) {
                    errMsg += "Please Add The credited Date !!!</br>";
                }
            }
            if (Validate.empty(self.SelectedPaymentCenter())) {
                errMsg += "Please Select Payment Center !!!</br>";
            }
            if (Validate.empty(self.bankName())) {
                errMsg += "Please Add Bank Name !!!</br>";
            } if (Validate.empty(self.accountNo())) {
                errMsg += "Please Add Account No !!!</br>";
            }
            if (Validate.empty(self.remarks())) {
                errMsg += "Please Add A Remark !!!</br>";
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




    ko.applyBindings(new DemateDividendIssueEntry());

});
