function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function ShHolderHistory(data) {
    var self = this;
    if (data != undefined) {
        self.FiscalYr = ko.observable(data.fiscalYr)
        self.Status = ko.observable(data.status)
        self.WarrantNo = ko.observable(data.warrantNo)
        self.HolderNo = ko.observable(data.holderNo)
        self.BOID = ko.observable(data.boid)
        self.Kitta = ko.observable(data.kitta)
        self.DividendAmt = ko.observable(data.dividendAmt)
        self.DivTax = ko.observable(data.divTax)
        self.BonusTax = ko.observable(data.bonusTax)
        self.BonusAdj = ko.observable(data.bonusAdj)
        self.PrevAdj = ko.observable(data.prevAdj)
        self.PayableAmt = ko.observable(data.payableAmt)

        self.IssueDate = ko.observable(data.issueDate == null ? "" : data.issueDate.substring(0, 10))
        self.PaidDate = ko.observable(data.paidDate == null ? "" : data.paidDate.substring(0, 10))
        self.CreditedDt = ko.observable(data.creditedDt == null ? "" : data.creditedDt.substring(0, 10))
        self.ApprovedDate = ko.observable(data.approvedDate == null ? "" : data.approvedDate.substring(0, 10))


        self.Bankname = ko.observable(data.bankname)
        self.Bankaccno = ko.observable(data.bankaccno)

        self.CertNo = ko.observable(data.certNo)
        self.SrnoFrom = ko.observable(data.srnoFrom)
        self.SrnoTo = ko.observable(data.srnoTo)

        self.PrevFraction = ko.observable(data.prevFraction)
        self.ActualBonus = ko.observable(data.actualBonus)
        self.FractionWithAB = ko.observable(data.fractionWithAB)
        self.IssueBonus = ko.observable(data.issueBonus)
        self.RemFraction = ko.observable(data.remFraction)
        self.Remarks = ko.observable(data.remarks)
    }
}

var HoldersHistory = function () {
    //Companykolagi
    var self = this;
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    self.OccupationList = ko.observableArray([])
    self.SelectedOccupation = ko.observable()

    self.ShholderNo = ko.observable();
    self.Name = ko.observable();
    self.Address = ko.observable();
    self.District = ko.observable();
    self.FaName = ko.observable();
    self.GrFaName = ko.observable();
    self.TelNo = ko.observable();
    self.AccountNo = ko.observable();
    self.BankName = ko.observable();
    self.Occupation = ko.observable();
    self.Kitta = ko.observable();

    self.ShareTypeBasedOn = ko.observable('P');
    self.BonusTypeBasedOn = ko.observable('1');

    self.OccupationList.push({ 'OccupationName': 'Promoter', 'OccupationId': '2' })
    self.OccupationList.push({ 'OccupationName': 'Public', 'OccupationId': '1' })

    self.ShHolderHistoryList = ko.observableArray([]);

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
                    if (!Validate.empty(localStorage.getItem('company-code'))) { self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode()); }
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

    //changing html on select
    self.ShareTypeBasedOn.subscribe(function () {
        if (self.ShareTypeBasedOn() == 'P') {
            $("#ShHolderNoLabel").html("ShHolder No :");
            $("#ShHolderNo").attr("placeholder", "Enter ShHolder No");
        }
        else {
            $("#ShHolderNoLabel").html("BOID :");
        }
    });

    self.BonusTypeBasedOn.subscribe(function () {
        if (self.BonusTypeBasedOn() == "2") {
            $('#table_Bonus').show();
            $('#table_Dividend').hide();

            $('#tbl_Holder_History_Bonus').DataTable().clear();
            $('#tbl_Holder_History_Bonus').DataTable().destroy();
            $('#tbl_Holder_History_Dividend').DataTable().clear();
            $('#tbl_Holder_History_Dividend').DataTable().destroy();
            self.LoadBonusTable()

        } else {
            $('#table_Bonus').hide();
            $('#table_Dividend').show();

            $('#tbl_Holder_History_Bonus').DataTable().clear();
            $('#tbl_Holder_History_Bonus').DataTable().destroy();
            $('#tbl_Holder_History_Dividend').DataTable().clear();
            $('#tbl_Holder_History_Dividend').DataTable().destroy();

            self.LoadDividendTable()
        }
    });

    self.Validation = () => {
        var errMsg = "";
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company<br/> "
        }
        //if (Validate.empty(self.ShholderNo())) {
        //    errMsg += "Enter Holder No <br/>"
        //}
        if (Validate.empty(self.SelectedOccupation())) {
            errMsg += "Please Choose Occupation <br/>"
        }
        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
        }

    }

    self.GetSholderInformation = () => {
        if (self.Validation()) {
            var companyCode = self.SelectedCompany();

            $('#tbl_Holder_History_Bonus').DataTable().clear();
            $('#tbl_Holder_History_Bonus').DataTable().destroy();
            $('#tbl_Holder_History_Dividend').DataTable().clear();
            $('#tbl_Holder_History_Dividend').DataTable().destroy();
            //if (self.ShareTypeBasedOn() == 'P') {
            //    $('#DholderNoD').attr("data-bind", "text: HolderNo")
            //}
            //else {
            //    $('#DholderNoD').attr("data-bind", "text: BOID")
            //}

            Openloader()
            $.ajax({
                type: "post",
                url: '/DividendManagement/HoldersHistory/GetHolderHistoryData',
                data: {
                    'CompCode': self.SelectedCompany(),
                    'DivType': self.BonusTypeBasedOn(),
                    'ShHolderNo': self.ShholderNo(),
                    'ShareType': self.ShareTypeBasedOn(),
                    'Occupation': self.SelectedOccupation()
                },
                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        self.Name(result.responseData[0].fName + ' ' + (result.responseData[0].lName != null ? result.responseData[0].lName : ''));
                        self.Kitta(result.responseData[0].totalKitta);
                        self.FaName(result.responseData[0].faName);
                        self.GrFaName(result.responseData[0].grFaName);
                        self.Address(result.responseData[0].address);
                        self.District(result.responseData[0].distCode);
                        self.TelNo(result.responseData[0].telNo);
                        if (result.responseData[0].occupation == 1) {
                            self.Occupation('Public')
                        }
                        else {
                            self.Occupation('Promotor')
                        }

                        self.AccountNo(result.responseData[0].accountNo ?? "---");
                        self.BankName(result.responseData[0].bankName ?? "---");
                        var mappedTasks = $.map(result.responseData[1], function (item) {
                            return new ShHolderHistory(item)
                        });
                        self.ShHolderHistoryList(mappedTasks);
                    } else {
                        self.clearInput()
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }, complete: () => {
                    Closeloader();
                    self.LoadBonusTable()
                    self.LoadDividendTable()
                }
            })

        }

    }

    self.clearInput = function () {
        self.Name(null);
        self.Kitta(null);
        self.FaName(null);
        self.GrFaName(null);
        self.Address(null);
        self.District(null);
        self.TelNo(null);
        self.AccountNo(null);
        self.Occupation(null);
    }

    self.LoadBonusTable = () => {
        $('#tbl_Holder_History_Bonus').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: true,

            dom: 'Bfrtip',
            buttons: [
                'excel', {
                    extend: 'pdfHtml5',
                    text: 'PDF', orientation: 'landscape',
                    customize: function (doc) {
                        doc.content.splice(0, 0, {
                            margin: [0, 0, 0, 0],
                            alignment: 'left',
                            image: LogoForPDF,
                            fit: [60, 60]
                        });
                        doc.content.splice(1, 0, {
                            margin: [0, 0, 0, 0],
                            alignment: 'center',
                            text: "Bonus History"

                        });

                    }, exportOptions: {
                        columns: self.ShareTypeBasedOn() === 'D' ?
                            [0, 1, 2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13] :
                            [0, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13]

                    }, messageTop: function () {
                        return "Name: " + self.Name() + " || Address: " + self.Address();
                    }
                }
            ], lengthMenu: [
                [10, 25, 50, -1],
                [10, 25, 50, 'All'],
            ]
        })
    }
    self.LoadDividendTable = () => {
        $('#tbl_Holder_History_Dividend').DataTable({
            
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: true,
            dom: 'Bfrtip',
            
            buttons: [
                {
                    extend: 'excel'
                    , exportOptions: {
                    }, messageTop: function () {
                        var holderBoid;
                        if (self.ShareTypeBasedOn() == 'D') { holderBoid = " || BOID : " }
                        else { holderBoid = " || HolderNo: " }
                        return "Name: " + self.Name() + " || Address: " + self.Address() + holderBoid + self.ShholderNo();
                    }
                }
                , {
                    extend: 'pdfHtml5', pageSize: 'letter',
                    text: 'PDF', orientation: 'landscape',
                    customize: function (doc) {
                        doc.content.splice(0, 0, {
                            margin: [0, 0, 0, 0],
                            alignment: 'left',
                            image: LogoForPDF,
                            fit: [80, 80]
                        });
                        doc.content.splice(1, 0, {
                            margin: [0, 0, 0, 0],
                            alignment: 'center',
                            text: "Dividend History"

                        });
                        
                    }, exportOptions: {
                        columns: self.ShareTypeBasedOn() === 'D' ?
                            [0, 1, 2, 3, 5, 6, 12, 15, 16, 17] :
                            [0, 1, 2, 3, 4, 6, 12, 15, 16, 17]
                 
                    }, messageTop: function () {
                        var holderBoid;
                        if (self.ShareTypeBasedOn() == 'D') { holderBoid = " || BOID : " }
                        else { holderBoid = " || HolderNo: " }
                        return "Name: " + self.Name() + " || Address: " + self.Address() + holderBoid + self.ShholderNo();
                    }
                }
            ]
        })
    }
    self.LoadBonusTable()
    self.LoadDividendTable()

}



$(document).ready(function () {
    ko.applyBindings(new HoldersHistory());
    $('#table_Bonus').hide();
    $('#table_Dividend').show();

});
