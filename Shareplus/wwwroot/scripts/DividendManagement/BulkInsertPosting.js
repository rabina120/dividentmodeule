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
        self.TableName1 = ko.observable(data.tablename1)
        self.TableName2 = ko.observable(data.tablename2)
        self.ShowDividendText = self.Divcode() + " " + self.Description() + " " + self.FiscalYr();

    }
}

var CahsDividendIssuePosting = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    self.ShareTypeBasedOn = ko.observable('P');
    self.BonusTypeBasedOn = ko.observable('1');

    //Dividend ko table list lai
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.compcode = ko.observable();
    self.Divcode = ko.observable();
    self.Description = ko.observable();
    self.FiscalYr = ko.observable();

    //
    self.PrevTotKitta = ko.observable()
    self.PrevFrac = ko.observable()
    self.TotShKitta = ko.observable()
    self.ActualBonus = ko.observable()
    self.ABWithPFrac = ko.observable()
    self.IssueBonus = ko.observable()
    self.RemFrac = ko.observable()
    //
    self.WarrantAmount = ko.observable()
    self.DividendTax = ko.observable()
    self.BonusAdj = ko.observable()
    self.BonusTax = ko.observable()
    self.PreviousAdjust = ko.observable()
    self.TotalShKitta = ko.observable()
    //
    self.PostingRemarks = ko.observable()
    self.PostingDate = ko.observable(formatDate(new Date))

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
        self.DividendLists([])
        if (self.ValidateCompany()) {
            if (self.DividendValidation()) {
                var companyCode = self.SelectedCompany()
                $.ajax({
                    type: "post",
                    url: '/DividendManagement/BulkInsertPosting/GetDividendList',
                    data: { 'CompCode': companyCode, 'ShareType': self.ShareTypeBasedOn(), 'BonusType': self.BonusTypeBasedOn() },
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
                    }, complete: function (error) {
                        Closeloader()
                    }
                })
            }
        }
    }

    self.ValidateCompany = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Select Company !!!<br/>";
        }
        if (errMsg == "") {
            return true
        } else {
            alert('warning', errMsg)
            return false
        }
    }

    self.DividendValidation = function () {
        var errMsg = ""
        if (Validate.empty(self.ShareTypeBasedOn())) {
            errMsg += "Please Select Share Type !!!<br/>";
        }
        if (Validate.empty(self.BonusTypeBasedOn())) {
            errMsg += "Please Select Bonus Type !!!<br/>";
        }
        if (errMsg == "") {
            return true
        } else {
            alert('warning', errMsg)
            return false
        }
    }

    self.SaveValidation = function () {
        var errMsg = ""
        if (Validate.empty(self.ShareTypeBasedOn())) {
            errMsg += "Please Select Share Type !!!<br/>";
        }
        if (Validate.empty(self.BonusTypeBasedOn())) {
            errMsg += "Please Select Bonus Type !!!<br/>";
        }
        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select A Dividend !!!<br/>";
        }
        if (Validate.empty(self.PostingDate())) {
            errMsg += "Please Select Posting Date !!!<br/>";
        }
        //if (Validate.empty(self.PostingRemarks())) {
        //    errMsg += "Please Enter Posting Remarks !!!<br/>";
        //}
        if (errMsg == "") {
            return true
        } else {
            alert('warning', errMsg)
            return false
        }
    }

    self.ClearDividends = function () {
        $('#BonusDiv,#DividendDiv').hide()
        $('#SendToExcel').attr('disabled',true)
        self.PrevTotKitta('')
        self.PrevFrac('')
        self.TotShKitta('')
        self.ActualBonus('')
        self.ABWithPFrac('')
        self.IssueBonus('')
        self.RemFrac('')
        self.WarrantAmount('')
        self.DividendTax('')
        self.BonusAdj('')
        self.BonusTax('')
        self.PreviousAdjust('')
        self.TotalShKitta('')
        self.PostingRemarks('')
        self.PostingDate(formatDate(new Date))
    }
    
    self.SelectedDividend.subscribe(function () {
        if (self.ValidateCompany()) {
            if (self.DividendValidation()) {
                if (Validate.empty(self.SelectedDividend())) {
                    
                } else {
                    ClearDividends()
                    var companyCode = self.SelectedCompany()
                    Openloader()
                    $.ajax({
                        type: "post",
                        url: '/DividendManagement/BulkInsertPosting/GetSelectedDividendDetails',
                        data: { 'CompCode': companyCode, 'ShareType': self.ShareTypeBasedOn(), 'BonusType': self.BonusTypeBasedOn(), 'DivCode': self.SelectedDividend().Divcode() },
                        datatype: "json", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (result) {
                            if (result.isSuccess) {
                                if (self.BonusTypeBasedOn() == '1') {
                                    $('#DividendDiv').show()
                                    self.WarrantAmount(result.responseData.warrantAmt)
                                    self.DividendTax(result.responseData.taxDamt)
                                    self.BonusAdj(result.responseData.bonusAdj)
                                    self.BonusTax(result.responseData.bonusTax)
                                    self.PreviousAdjust(result.responseData.prevAdj)
                                    self.TotalShKitta(result.responseData.totShKitta)
                                }
                                else {
                                    $('#BonusDiv').show()
                                    self.PrevTotKitta(result.responseData.prevTotKitta)
                                    self.PrevFrac(result.responseData.prevFrac)
                                    self.TotShKitta(result.responseData.totShKitta)
                                    self.ActualBonus(result.responseData.actualBonus)
                                    self.ABWithPFrac(result.responseData.abWithPrevFrac)
                                    self.IssueBonus(result.responseData.issueBonus)
                                    self.RemFrac(result.responseData.remFrac)
                                }
                                $('#SendToExcel').attr('disabled',false)
                            } else {
                                alert('warning', result.message)
                                $('#DividendList').val(null).trigger('change')
                                ClearDividends()
                            }
                        },
                        error: function (error) {
                            alert('error', error.message)
                            $('#DividendList').val(null).trigger('change')
                            ClearDividends()
                        }, complete: function (error) {
                            Closeloader()
                        }
                    })
                }
            }
        }
    })

    self.ShareTypeBasedOn.subscribe(function () {
        self.LoadDividendLists()
        
    })
    self.BonusTypeBasedOn.subscribe(function () {
        self.LoadDividendLists()
    })

    self.SelectedCompany.subscribe(function () {
        self.LoadDividendLists()
    })

    self.SendToExcel = function () {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.SelectedDividend())) {
                var companyCode = self.SelectedCompany();
                var companyName = self.CompanyDetails().find(x => x.CompCode() == companyCode).CompEnName();
                Openloader()
                $.ajax({
                    type: "post", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/DividendManagement/BulkInsertPosting/GetSelectedDividendDetailsExcel',
                    data: { 'CompCode': companyCode, 'ShareType': self.ShareTypeBasedOn(), 'BonusType': self.BonusTypeBasedOn(), 'DivCode': self.SelectedDividend().Divcode(),'CompEnName':companyName},
                    datatype: "json",
                    success: function (result) {
                        if (result.isSuccess) {
                            var fileName = result.message;
                            var a = document.createElement("a");
                            a.href = "data:application/octet-stream;base64," + result.responseData;
                            a.download = fileName;
                            a.click();
                        } else {
                            alert('warning', result.message)
                            $('#DividendList').val(null).trigger('change')
                            self.DividendLists('')
                            ClearDividends()
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                        $('#DividendList').val(null).trigger('change')
                        self.DividendLists('')
                        ClearDividends()
                    }, complete: function (error) {
                        Closeloader()
                    }
                })
            }
        }
    }


    self.Save = function (data) {
        if (self.ValidateCompany()) {
            if (self.SaveValidation()) {
                var companyCode = self.SelectedCompany()
                Openloader()
                $.ajax({
                    type: "post", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/DividendManagement/BulkInsertPosting/Save',
                    data: {
                        'CompCode': companyCode,
                        'ShareType': self.ShareTypeBasedOn(),
                        'BonusType': self.BonusTypeBasedOn(),
                        'DivCode': self.SelectedDividend().Divcode(),
                        'AcceptOrReject': data
                    },
                    datatype: "json",
                    success: function (result) {
                        if (result.isSuccess) {
                           alert('success',result.message)
                        } else {
                           alert('warning', result.message)
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                    }, complete: function (error) {
                        Closeloader()
                        self.DividendLists('')
                        $('#DividendList').val(null).trigger('change')
                        ClearDividends()
                    }
                })
            }
        }
    }
    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });

}



$(document).ready(function () {
    ko.applyBindings(new CahsDividendIssuePosting());
    $('#BonusDiv,#DividendDiv').hide()

});
