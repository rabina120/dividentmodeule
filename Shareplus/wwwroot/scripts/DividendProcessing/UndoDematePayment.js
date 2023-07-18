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
        self.WIssuedDt = ko.observable(data.wIssuedDt);
        self.dwiby = ko.observable(data.dwiby);
        self.wissue_Approved = ko.observable(data.wissue_Approved);
        self.wissue_app_date = ko.observable(data.wissue_app_date);
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
        self.batchno = ko.observable(data.batchno);

       
        self.TotalPayable = self.WarrantAmt() - self.TaxDamt() - self.bonustax() - self.bonusadj();
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


var CashCashDividendIssueEntryEntry = function () {
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

    //UNDO TYPE
    self.UndoType = ko.observable();
    self.UndoType = ko.observable("Payment");

    //Payment Center
    self.CenterId = ko.observable();
    self.CenterName = ko.observable();
    self.Address = ko.observable();
    self.NepaliName = ko.observable
    self.NepaliAddress = ko.observable();
    self.TelNo = ko.observable();

    //signature
    self.Signature = ko.observable();
    self.signature = ko.observable();


    //District ko lagi
    self.Districts = ko.observableArray([]);
    self.DistCode = ko.observable();
    self.NpDistName = ko.observable();
    self.EnDistName = ko.observable();


    self.DividentPaidBy = ko.observable();
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
                type: 'post',
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
            $("#textBasedOn2").html("BOID No:");
        }
        else {
            $("#textBasedOn").html("BOID No:");
            $("#textBasedOn2").html("Warrant No:");


        }
    })

    //enabling holderno input on change
    self.SelectedDividend.subscribe(function () {
        if (self.SelectedDividend() != "" && self.SelectedDividend() != undefined) {
            document.getElementById('textBasedOnPlace').disabled = false;
        } else {
            $('#textBasedOnPlace').attr('disabled', 'disabled');

        }
    });

    self.SearchValidation = () => {
        var errMsg = "";
        if (Validate.empty(self.SelectedDividend())) {
            errMsg = "Please Choose Dividend<br/>"
        }
        if (Validate.empty(self.ShholderNo())) {
            errMsg = self.BasedOn() == "W" ? "Enter Warrant No <br/>" : "Enter Shholder No <br/>"
        } else {
            if (self.BasedOn() != "W") {
                if (self.ShholderNo().length != 16) {
                    errMsg = "BOID Cant be less than 16 digits <br/>"
                }
            }
            //if (self.BasedOn() == "W") {
            //    if (self.ShholderNo().length != 10) {
            //        var warn = self.ShholderNo();
            //        warn = warn.padStart(10, '0');
            //        self.ShholderNo(warn)
            //    }
            //}
            //else {
            //    if (self.ShholderNo().length != 16) {
            //        errMsg = "BOID Cant be less than 16 digits <br/>"
            //    }
            //}
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
            var action = self.BasedOn();
            Openloader()
            $.ajax({
                type: "post",
                url: '/DividendProcessing/UndoDematePayment/GetDividendInformation',
                data: {
                    'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend().Divcode,
                    'shholderno': self.ShholderNo(), 'based': action, 'undoType': UndoType()
                },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        self.FullName(result.responseData.fullname);
                        self.Address(result.responseData.address);
                        self.FaName(result.responseData.faname);
                        self.GrFaName(result.responseData.grfaname);
                        self.TotalKitta(result.responseData.totshkitta);
                        self.AgmNo(result.responseData.agmNo);
                        self.divType(result.responseData.divType);
                        self.WarrantNo(result.responseData.warrantno);
                        self.ShholderNoNew(result.responseData.bO_idno);
                        self.WarrantAmt(result.responseData.warrantamt);
                        self.TaxDamt(result.responseData.taxdamt);
                        self.bonustax(result.responseData.bonustax);
                        self.bonusadj(result.responseData.bonusadj);
                        self.TotalPayable(result.responseData.warrantamt - result.responseData.taxdamt - result.responseData.bonustax - result.responseData.bonusadj);
                       
                        $('#DividendTable,#customRadio1,#customRadio2,#textBasedOnPlace').attr('disabled',true)
                    } else {
                        alert('warning', result.message)
                        self.clearControls()
                        $('#textBasedOnPlace').attr('disabled', true)
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
    self.SaveCashDividend = function () {

        if (self.ValidateForm()) {
            if (self.SelectedAction() == "A") {
                var text = "The Dividend will be issued.";
            }
            else {
                text = "The Dividend will be unissued.";
            }
            swal({
                title: "Are you sure?",
                text: "DIVIDEND WILL BE REVERTED.",
                icon: "warning",
                buttons: true,
                dangerMode: true
            }).then((willSave) => {
                if (willSave) {
                    var maxSeqNo;

                    Openloader()
                    $.ajax({
                        type: "POST",
                        url: '/DividendProcessing/UndoDematePayment/SaveCashDividend',
                        data: {
                            'DivCode': self.SelectedDividend().Divcode,
                            'CompCode': self.SelectedCompany(),
                            'undoType': self.UndoType(),
                            'warrantno': self.WarrantNo(),
                            'shholderno': self.ShholderNoNew()

                        },
                        datatype: "json", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (result) {
                            if (result.isSuccess) {
                                swal({
                                    title: "Cash Dividend",
                                    text: result.message,
                                    type: "success"
                                }).then(function () {
                                    self.clearControls();
                                    self.ButtonsOnLoad();
                                   
                                });

                            } else {
                                alert('warning', result.message);
                            }
                        },
                        error: function (error) {
                            alert('error', error.message)
                        },
                        complete: () => {

                            self.clearControls();
                            self.ButtonsOnLoad();

                            Closeloader()
                        }
                    });
                }
            });
        }

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
        self.TotalPayable("");
        self.WIssuedDt("");
        self.telno("");
        self.remarks("");
        self.SelectedAction('')
        self.SelectedDividend('')
        $('#DividendTable').val(null).trigger('change');
        $('#DividendTable,#customRadio1,#customRadio2,#textBasedOnPlace').attr('disabled', false)

    }
    self.EnableAllButtons = function () {
        $("#AddButton,#DeleteButton").removeAttr('disabled');
        $('SaveButton').attr('disabled', 'disabled');
    }
    self.ButtonsOnLoad = function () {
        $('SaveButton').attr('disabled', 'disabled');
    }
    self.ButtonsOnLoad();

    self.Refresh = function () {
        self.SelectedDividend("");
        $('#DividendTable').val(null).trigger('change');

        self.EnableAllButtons();
        self.clearControls();
        $('#textBasedOnPlace').attr('disabled', 'disabled');

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
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
    }
}
//CashCashDividendIssueEntryEntry.IsPaidBy(false); // The checkbox becomes unchecked
$(document).ready(function () {
    $('textBasedOnPlace').attr('disabled', 'disabled');

    // Bootstrap Date Picker

    $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date1 .input-group.date").datepicker("setDate", new Date());



    ko.applyBindings(new CashCashDividendIssueEntryEntry());
    self.Refresh()

});
