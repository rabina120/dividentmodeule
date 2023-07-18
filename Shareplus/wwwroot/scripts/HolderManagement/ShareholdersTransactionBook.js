function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}


self.ShareHoldersTransactionBook = function () {
    var self = this;
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.ShareHolderNo = ko.observable()
    self.ShareHolderName = ko.observable()
    self.ShareHolderAddress = ko.observable()
    self.RightShare = ko.observable(0)
    self.BonusShare = ko.observable(0)
    self.OrdinaryShare = ko.observable(0)
    self.TotalShare = ko.observable(0)
    self.ShareHolderInfo = ko.observable();
    self.TransactionList = ko.observableArray([]);
    self.TransactionWithShareType = ko.observableArray([]);



    var companyCode = localStorage.getItem('company-code');

    $.ajax({
        type: "post",
        url: '/Common/Company/GetCompanyDetails',

        datatype: "json", beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (result) {
            if (result.isSuccess) {
                var mappedTasks = $.map(result.responseData, function (item) {
                    return new ParaComp(item)
                });
                self.CompanyDetails(mappedTasks);

                if (companyCode != undefined) {
                    self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode());
                    //self.LoadDividendList();
                }
                //$("#CompanyList").attr("disabled", true);
            } else {
                alert('warning', result.message)
            }
        },
        error: function (error) {
            alert('error', error.message)
        }
    })

    self.SelectedCompany.subscribe(function () {
        if (self.ShareHolderNo() != undefined || self.ShareHolderNo() != null) {
            postReqMsg(
                '/HolderManagement/ShareHolderTransactionBook/GetShareHolderTransactionBook',
                { CompCode: self.SelectedCompany(), SHNumber: self.ShareHolderNo() },
                null,
                resp => {
                    //self.ShareHolderInfo(resp.ResponseData2);
                    self.ShareHolderName(resp.ResponseData2.FName);
                    self.ShareHolderAddress(resp.ResponseData2.Address);
                    self.RightShare(resp.ResponseData2.rightkitta)
                    self.BonusShare(resp.ResponseData2.bonuskitta)
                    self.OrdinaryShare(resp.ResponseData2.ordinarykitta)
                    self.TotalShare(resp.ResponseData2.TotalKitta)

                    self.TransactionList(resp.ResponseData);
                },
                null
            );
        }
    })


    self.ShareHolderNo.subscribe(function (SHNo) {
        var errMsg = ""
        if (self.SelectedCompany() == null || self.SelectedCompany() == undefined) errMsg += 'Select a Company!!'
        if (errMsg != '') {
            toastr.error(errMsg);
            //self.ShareHolderNo(undefined);
            return false;
        }
        postReqMsg(
            '/HolderManagement/ShareHolderTransactionBook/GetShareHolderTransactionBook',
            { CompCode: self.SelectedCompany(), SHNumber: SHNo },
            null,
            resp => {
                //self.ShareHolderInfo(resp.ResponseData2);
                self.ShareHolderName(resp.ResponseData2.FName);
                self.ShareHolderAddress(resp.ResponseData2.Address);
                self.RightShare(resp.ResponseData2.rightkitta)
                self.BonusShare(resp.ResponseData2.bonuskitta)
                self.OrdinaryShare(resp.ResponseData2.ordinarykitta)
                self.TotalShare(resp.ResponseData2.TotalKitta)

                self.TransactionList(resp.ResponseData);
            },
            null
        );
    })

    var shareType;
    self.TypeName = ko.observable();
    self.ShowRightShares = function () {
        shareType = 4;
        self.TypeName("Right")
        self.RequestShareTypes(shareType);
        $("#viewShareDetails").modal("show");
    }
    self.ShowOrdinaryShares = function () {
        shareType = 2;
        self.TypeName("Ordinary")
        self.RequestShareTypes(shareType);
        $("#viewShareDetails").modal("show");
    }
    self.ShowBonusShares = function () {
        shareType = 3;
        self.TypeName("Bonus")
        self.RequestShareTypes(shareType);
        $("#viewShareDetails").modal("show");
    }


    self.RequestShareTypes = function (shareType) {
        self.TransactionWithShareType([]);
        postReq(
            '/HolderManagement/ShareHolderTransactionBook/GetShareTypes',
            { CompCode: self.SelectedCompany(), SHNumber: self.ShareHolderNo, ShareType: shareType },
            null,
            self.TransactionWithShareType,
            null
        )
        if (self.TransactionWithShareType().length == 0) toastr.warning("No data found!!");
    }

    self.PSList = [
        { DiplayName: "Purchase", Value: "P" },
        { DiplayName: "Sales", Value: "S" }
    ]
    self.PSReportType = ko.observable();

    self.GetPurchaseSalesReport = function () {
        $("#GetPurchaseSalesReport").modal("show");
    }
    self.EnableReport = ko.computed(function () {
        if ((self.SelectedCompany() == undefined || self.SelectedCompany() == null) && (self.ShareHolderNo() == undefined || self.ShareHolderNo() == null)) return false;
        else return true;
    })
    self.CompnyName = ko.observable();

    self.ExcelReport = function () {
        self.GeneratePSReport('E');
    }
    self.PDFReport = function () {
        self.GeneratePSReport('P');
    }
    self.GeneratePSReport = function (fileType) {
        self.CompnyName(self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName())
        //console.log(self.CompnyName())
        postReqMsg(
            '/HolderManagement/ShareHolderTransactionBook/GetPurchaseSalesReport',
            { CompCode: self.SelectedCompany(), SHNumber: self.ShareHolderNo, ShareType: self.PSReportType(), CompName: self.CompnyName, HolderName: self.ShareHolderName(), FileType: fileType },
            null,
            resp => {
                console.log(resp);
                var fileName = resp.Message;
                var a = document.createElement("a");
                a.href = "data:application/octet-stream;base64," + resp.ResponseData;
                a.download = fileName;
                a.click();
            },
            null
        )
        $("#GetPurchaseSalesReport").modal("hide");
    }

}

$(document).ready(function () {
    ko.applyBindings(new self.ShareHoldersTransactionBook());
})