function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function Sign(data) {
    var self = this;
    if (data != undefined) {
        self.signature = ko.observable(data.signature);
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
        self.telNo = ko.observable(data.telNo);
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

function District(data) {
    var self = this;
    if (data != undefined) {
        self.DistCode = ko.observable(data.distCode);
        self.NpDistName = ko.observable(data.npDistName);
        self.EnDistName = ko.observable(data.enDistName);
        self.DistCodeName = self.DistCode() + " " + self.EnDistName();
    }
}


function CashDividend(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.compcode);
        self.WarrantNo = ko.observable(data.warrantNo);
        self.ShHolderNo = ko.observable(data.shHolderNo);
        self.AgmNo = ko.observable(data.agmNo);
        self.divType = ko.observable(data.divType);
        self.seqno = ko.observable(data.seqno);
        self.DPayable = ko.observable(data.dPayable);
        self.WarrantAmt = ko.observable(data.warrantAmt);
        self.TaxDamt = ko.observable(data.taxDamt);
        self.bonustax = ko.observable(data.bonustax);
        self.bonusadj = ko.observable(data.bonusadj);
        self.prevadj = ko.observable(data.prevadj);
        self.TotShKitta = ko.observable(data.totShKitta);
        self.WIssued = ko.observable(data.wIssued);

        self.WIssuedDt = ko.observable(data.wAmtPaidDt);
        self.creditedDt = ko.observable(data.crediteDt);

        self.dwiby = ko.observable(data.dwiby);
        self.wissue_Approved = ko.observable(data.wissue_Approved);
        self.wissue_app_date = ko.observable(data.wissue_app_date );
        self.wissue_status = ko.observable(data.wissue_status);
        self.wissue_auth_remarks = ko.observable(data.wissue_auth_remarks);
        self.wissue_approvedby = ko.observable(data.wissue_approvedby);
        self.WPaid = ko.observable(data.wPaid);
        self.WAmtPaidDt = ko.observable(data.wAmtPaidDt);
        self.WPaidBy = ko.observable(data.wPaidBy);
        self.wpaid_approved = ko.observable(data.wpaid_approved);
        self.wpaid_app_date = ko.observable(data.wpaid_app_date);
        self.wpaid_status = ko.observable(data.wpaid_status);
        self.wpaid_auth_remarks = ko.observable(data.wpaid_auth_remarks);
        self.wpaid_approvedby = ko.observable(data.wpaid_approvedby);
        self.CashOrTran = ko.observable(data.cashOrTran);
        self.centerid = ko.observable(data.centerid);
        self.telno = ko.observable(data.telno);
        self.remarks = ko.observable(data.remarks);

        self.IssueRemarks = ko.observable(data.remarks);
        self.batchno = ko.observable(data.batchno);
        self.TotalPayable = self.WarrantAmt() - self.TaxDamt() - self.bonustax() - self.bonusadj() - self.prevadj();

        self.bankName = ko.observable(data.bankName);
        self.accountNo = ko.observable(data.bankAccNo);
       

        if (data.aTTShHolder != null) {
            self.ShholderNo = ko.observable(data.aTTShHolder.shholderNo);

            self.FName = ko.observable(data.aTTShHolder.fName);
            self.LName = ko.observable(data.aTTShHolder.lName);
            self.Address = ko.observable(data.aTTShHolder.address);
            self.DistCode = ko.observable(data.aTTShHolder.distCode);
            self.PboxNo = ko.observable(data.aTTShHolder.pboxNo);
            self.FaName = ko.observable(data.aTTShHolder.faName);
            self.GrFaName = ko.observable(data.aTTShHolder.grFaName);
            self.MobileNo = ko.observable(data.aTTShHolder.mobileNo);
            self.FullName = self.FName() + " " + self.LName();
        }
    }
}
function ShHolder(data) {
    var self = this;
    if (data != undefined) {

        self.ShholderNo = ko.observable(data.shholderNo);

        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.Address = ko.observable(data.address);
        self.DistCode = ko.observable(data.distCode);
        self.PboxNo = ko.observable(data.pboxNo);
        self.FaName = ko.observable(data.faName);
        self.GrFaName = ko.observable(data.grFaName);
        self.MobileNo = ko.observable(data.mobileNo);
        self.FullName = self.FName() + " " + self.LName();
    }
}




var DividentPaymentEntry = function () {
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

    self.PaidBy = ko.observable('Cash');

    self.SelectedAction = ko.observable();

    //data
    self.bankName = ko.observable();
    self.accountNo = ko.observable();

    self.WarrantNo = ko.observable();
    self.ShHolderNo = ko.observable();
    self.AgmNo = ko.observable();
    self.divType = ko.observable();
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
    self.IssueRemarks = ko.observable();
    self.ShholderNo = ko.observable();
    self.ShholderNoNew = ko.observable();

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
    self.telNo = ko.observable();
    self.SelectedPaymentCenter = ko.observable();
    self.PaymentCenters = ko.observableArray([]);

    //signature
    self.Signature = ko.observable();
    self.signature = ko.observable();

    //while saving
    self.whileSaveHolderNo = ko.observable();
    self.whileSaveWarrantNo = ko.observable();

    //District ko lagi
    self.Districts = ko.observableArray([]);
    self.DistCode = ko.observable();
    self.NpDistName = ko.observable();
    self.EnDistName = ko.observable();

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
   
    //Loading company select options
   
    self.LoadCompany();

    //Loading dividend lists select options
    self.LoadDividendLists = function () {
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            $.ajax({
                type: "post",
                url: '/DividendProcessing/DividendIssueEntry/GetAllDividends',
                data: { 'CompCode': self.SelectedCompany() },
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


    //loading district on form load
    self.LoadDistrict = function () {

        $.ajax({
            type: "post",
            url: '/HolderManagement/ShareHolder/GetAllDistrict',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new District(item);
                    });
                    self.Districts(mappedTasks);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })

    }
    self.LoadDistrict();


    //changing html on select
    self.BasedOn.subscribe(function () {
        if (self.BasedOn() == 'W') {
            $("#textBasedOn").html("Warrant No:");
            $("#textBasedOn2").html("Shholder No:");
            $('#warrantNum').attr('placeholder', 'Shholder No');

        }
        else {
            $("#textBasedOn").html("Shholder No:");
            $("#textBasedOn2").html("Warrant No:");
            $('#warrantNum').attr('placeholder', 'Warrent No');


        }
    })

    //enabling holderno input on change
    self.SelectedDividend.subscribe(function () {
        if (self.SelectedDividend() != "" && self.SelectedDividend() != undefined) {
            document.getElementById('textBasedOnPlace').disabled = false;
            // document.getElementById('textBasedOnPlace').focus();
        } else {
            $('#textBasedOnPlace').attr('disabled', 'disabled');

        }
    });


    self.chooseOptions = function (data) {
        $('#DividendTable,#SaveButton').attr('disabled', false);
        $('#DeleteButton,#AddButton').attr('disabled', true);

        self.SelectedAction(data);
        switch (data) {
            case "A":
                $('#PadIssueDate,#CreditedDate,#bankName,#accountNo,#payCenterSelect,#telno,#remarks').attr('disabled', false);
                break;
            case "D":
                $('#PaidIssueDate,#CreditedDate,#bankName,#accountNo,#payCenterSelect,#telno,#remarks').attr('disabled', true);
                break;
        }

    };


    self.SearchValidation = () => {
        var errMsg = "";
        if (Validate.empty(self.SelectedDividend())) {
            errMsg = "Please Choose Dividend<br/>"
        }
        if (Validate.empty(self.SelectedAction())) {
            errMsg = "Please Choose Action Issue or Unissue<br/>"
        }
        if (Validate.empty(self.ShholderNo())) {
            errMsg = self.BasedOn() == "W" ? "Enter Warrant No <br/>" : "Enter Shholder No <br/>"
        } 

        if (errMsg == "") {
            return true;
        } else {
            alert('error', errMsg)
            return false;
        }
    }

    //finding record from the database
    self.searchData = function () {


        if (self.SearchValidation()) {


            document.getElementById("SignatureButton").style.visibility = "visible";

            var action = self.BasedOn();
            Openloader()
            $.ajax({
                type: "post", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/DividendProcessing/DividendPaymentEntry/GetDividendInformation',
                data: {
                    'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend().Divcode,
                    'shholderno': self.ShholderNo(), 'a': action, 'ac': self.SelectedAction()
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        self.FullName(result.responseData.attShholder.fName + " " + result.responseData.attShholder.lName);
                        self.Address(result.responseData.attShholder.address);
                        self.FaName(result.responseData.attShholder.faName);
                        self.GrFaName(result.responseData.attShholder.grFaName);
                        self.TotalKitta(result.responseData.totShKitta);
                        //self.DistCode(result.responseData.attShholder.distCode);
                        self.DistCode(self.Districts().find(x => x.DistCode() == ko.toJS(result.responseData.attShholder.distCode)).EnDistName());
                        //if(!Validate.empty(localStorage.getItem('company-code'))){self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode());}
                        self.PboxNo(result.responseData.attShholder.pboxNo);
                        self.MobileNo(result.responseData.attShholder.telNo != null ? result.responseData.attShholder.telNo : result.responseData.attShholder.mobileNo);
                        self.AgmNo(result.responseData.agmNo);
                        self.divType(result.responseData.divType);
                        if (self.BasedOn() == "S") {
                            self.WarrantNo(result.responseData.warrantNo);
                            self.whileSaveWarrantNo(result.responseData.warrantNo);
                            self.whileSaveHolderNo(self.ShholderNo());
                        }
                        else {
                            self.WarrantNo(result.responseData.shHolderNo);
                            self.whileSaveWarrantNo(self.ShholderNo());
                            self.whileSaveHolderNo(result.responseData.shHolderNo);
                        }

                        self.bankName(result.responseData.bankName);
                        self.accountNo(result.responseData.bankAccNo);

                        self.ShholderNoNew(result.responseData.shHolderNo);
                        self.WarrantAmt(result.responseData.warrantAmt);
                        self.TaxDamt(result.responseData.taxDamt);
                        self.bonustax(result.responseData.bonustax);
                        self.bonusadj(result.responseData.bonusadj);
                        self.prevadj(result.responseData.prevadj)
                        self.TotalPayable(result.responseData.warrantAmt - result.responseData.taxDamt - result.responseData.bonustax - result.responseData.bonusadj - result.responseData.prevadj);

                        var wissueddate = result.responseData.wAmtPaidDt;
                        if (wissueddate != null) {
                            wissueddate = wissueddate.substr(0, 10);
                        }
                        self.WIssuedDt(wissueddate);
                        var crediteddate = result.responseData.creditedDt;
                        if (crediteddate != null) {
                            crediteddate = crediteddate.substr(0, 10);
                        }
                        self.creditedDt(crediteddate);



                        $('#WIssued').attr('disabled', 'disabled');
                        self.telno(result.responseData.telNo);
                      
                        self.IssueRemarks(result.responseData.remarks);
                        var something = parseInt(result.responseData.centerid);

                        self.SelectedPaymentCenter(self.PaymentCenters().find(x => x.CenterId() == result.responseData.centerid));
                        if (self.SelectedPaymentCenter() != null) {
                            document.getElementById("select2-payCenterSelect-container").innerHTML = ko.toJS(self.SelectedPaymentCenter().PaymentDisplayName)
                        }
                        //var paidBy = result.responseData.cashOrTran == '1' ? "Cash" : "Cheque";
                        self.PaidBy(result.responseData.cashOrTran);

                    } else {
                        alert('warning', result.message)
                        self.clearControls()
                        $('#DividendTable, #textBasedOnPlace').attr('disabled', true)


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



    }

    //saving data
    self.SaveDividentPaymentEntry = function () {

        if (self.ValidateForm()) {
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
                        url: '/DividendProcessing/DividendPaymentEntry/SaveDividentPaymentEntry',
                        data: {
                            'DivCode': self.SelectedDividend().Divcode,
                            'CompCode': self.SelectedCompany(),
                            'bankName': self.bankName(),
                            'accountNo': self.accountNo(),
                            'centerid': self.SelectedPaymentCenter().CenterId,
                            'telno': self.telno(),
                            'cashOrCheque': self.PaidBy(),
                            'warrantNo': self.whileSaveWarrantNo(),
                            'shholderno': self.whileSaveHolderNo(),
                            'selectedAction': self.SelectedAction(),
                            'creditedDt': self.creditedDt(),
                            'wissueddate': self.WIssuedDt(),
                            'IssueRemarks': self.IssueRemarks()

                        },
                        datatype: "json",
                        success: function (result) {
                            if (result.isSuccess) {
                                swal({
                                    title: "Cash Dividend",
                                    text: result.message,
                                    type: "success"
                                }).then(function () {
                                    self.clearControls();
                                    self.ButtonsOnLoad();
                                    $('#DividendTable').attr('disabled', true);
                                });

                            } else {
                                alert('warning', result.message);
                            }
                        },
                        error: function (error) {
                            alert('error', error.message)
                        },
                        complete: () => {

                            //self.clearControls();
                            //self.ButtonsOnLoad();
                            //$('#DividendTable').attr('disabled', true);

                            Closeloader()
                        }
                    });
                }
            });
        }

    }

    //for showing signature in signature modal
    self.GetSignature = function () {
        document.getElementById("SignatureButton").disabled = true;
        Openloader()
        $.ajax({
            type: "post",
            url: '/HolderManagement/ShareHolder/GetSignature',
            data: { 'holderno': ko.toJS(self.ShholderNoNew()), 'compcode': ko.toJS(self.SelectedCompany()) },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    self.Signature("data:image/jpeg;base64," + result.responseData.signature);

                    $("#signatureModalTheme").modal('show');
                    document.getElementById("SignatureButton").disabled = false;


                } else if (!result.isSuccess) {

                    alert('error', "Sorry, the signature could not be found.");
                    document.getElementById("SignatureButton").disabled = false;

                }
            },
            error: function (error) {
                alert('error', error.message);
                document.getElementById("SignatureButton").disabled = false;


            },
            complete: () => {
                Closeloader()
            }

        });


    }

    self.clearControls = function () {
        self.ShholderNo("");

        self.FullName("");
        self.Address("");
        self.FaName("");
        self.GrFaName("");
        self.TotalKitta("");
        self.DistCode("");
        self.PboxNo("");
        self.MobileNo("");
        self.AgmNo("");
        self.divType("");
        self.WarrantNo("");
        self.WarrantAmt("");
        self.TaxDamt("");
        self.bonustax("");
        self.bonusadj("");
        self.prevadj("");
        self.TotalPayable("");
        self.WIssuedDt("");
        self.creditedDt("");
        self.SelectedPaymentCenter("");
        self.telno("");
        self.remarks("");


        self.bankName("");
        self.accountNo("");
        
        self.SelectedAction('')
        self.SelectedDividend('')
        $('#DividendTable').val(null).trigger('change');
        $('#payCenterSelect').val(null).trigger('change');
        document.getElementById("SignatureButton").style.visibility = "hidden";
        $('#AddButton, #DeleteButton').attr('disabled', false)
        self.PaidBy(null)
        self.SelectedAction('')
        self.IssueRemarks('')
    }
    self.EnableAllButtons = function () {
        $("#AddButton,#DeleteButton").removeAttr('disabled');
        $('SaveButton').attr('disabled', 'disabled');
    }
    self.ButtonsOnLoad = function () {
        $('SaveButton').attr('disabled', 'disabled');
        document.getElementById("SignatureButton").style.visibility = "hidden";
    }
    self.ButtonsOnLoad();

    self.Refresh = function () {
        self.SelectedDividend("");
        $('#DividendTable').val(null).trigger('change');

        self.EnableAllButtons();
        self.clearControls();
        $('#textBasedOnPlace,#DividendTable').attr('disabled', 'disabled');
        document.getElementById("SignatureButton").style.visibility = "hidden";

    }
    self.ValidateForm = function () {
        var errMsg = "";

        if (self.SelectedCompany() == undefined) {
            errMsg += "Please Choose Company !!!</br>";

        }
        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select Dividend !!!</br>";

        }
        if (Validate.empty(self.ShholderNo())) {
            errMsg += "Please Select ShholderNo or Warrant No !!!</br>";
            alert('error', errMsg)
            return false;

        }
        if (self.SelectedAction() == "A") {
            if (Validate.empty(self.PaidBy())) {
                errMsg += "Please Select Payment Type !!!</br>";
            }
            if (Validate.empty(self.WIssuedDt())) {
                errMsg += "Please Select Paid Issue Date !!!</br>";
            }
            if (Validate.empty(self.creditedDt())) {
                errMsg += "Please Select Credited Date !!!</br>";
            }
            if (Validate.empty(self.bankName())) {
                errMsg += "Please Add Bank Name !!!</br>";
            } if (Validate.empty(self.accountNo())) {
                errMsg += "Please Add Account No !!!</br>";
            }
            if (Validate.empty(self.SelectedPaymentCenter())) {
                errMsg += "Please Select Payment Center !!!</br>";
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
    $('textBasedOnPlace', 'DividendTable').attr('disabled', 'disabled');
    // Select2 Single  with Placeholder for Company

    // Bootstrap Date Picker

    $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date1 .input-group.date").datepicker("setDate", new Date());



    ko.applyBindings(new DividentPaymentEntry());

});
