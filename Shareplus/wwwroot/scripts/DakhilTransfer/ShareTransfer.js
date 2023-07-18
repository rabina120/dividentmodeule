function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function DakhilTransfer(data) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.certNo)
        self.SHolderNo = ko.observable(data.sHolderNo)
        self.BHolderNo = ko.observable(data.bHolderNo)
        self.SrNoFrom = ko.observable(data.srNoFrom)
        self.SrNoTo = ko.observable(data.srNoTo)
        self.RegNo = ko.observable(data.regNo)
        self.Foliono = ko.observable(data.foliono)
        self.TradeType = ko.observable(data.tradeType)
        self.PageNo = ko.observable(data.pageNo)
        self.DakhilDt = ko.observable(data.dakhilDt)
        self.CertTagNo = ko.observable(data.certTagNo)
        self.StockExDt = ko.observable(data.stockExDt)
        self.LetterNo = ko.observable(data.letterNo)
        self.Iono = ko.observable(data.iono)
        self.TranNo = ko.observable(data.tranNo)
        self.Brokercode = ko.observable(data.brokercode)
        self.Charge = ko.observable(data.charge)
        self.BHoldExist = ko.observable(data.bHoldExist)
        self.Transfered = ko.observable(data.transfered)
        self.TransferDt = ko.observable(data.transferDt == null ? "Not Transfered" : data.transferDt.substring(0,10))
        self.Batchno = ko.observable(data.batchno)
        self.TransactNo = ko.observable(data.transactNo)
        self.TranKitta = ko.observable(data.tranKitta)
        self.SplittedFr = ko.observable(data.splittedFr)
        self.Remarks = ko.observable(data.remarks)
        self.UserName = ko.observable(data.userName)
        self.EntryDate = ko.observable(data.entryDate)
        self.Approved = ko.observable(data.approved)
        self.Approvedby = ko.observable(data.approvedby)
        self.App_status = ko.observable(data.app_status)
        self.App_date = ko.observable(data.app_date)
        self.App_remarks = ko.observable(data.app_remarks)
        self.Trans_code = ko.observable(data.trans_code)
        self.SellerHolderName = ko.observable(data.sellerHolderName)
        self.BuyerHolderName = ko.observable(data.buyerHolderName)
        self.TotalRecords = ko.observable(data.totalRecords)
        self.Selected = ko.observable(false)
    }
}


var ShareTransfer = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()

    self.RegNoFrom = ko.observable()
    self.RegNoTo = ko.observable()
    self.DakhilDateFrom = ko.observable(formatDate(new Date))
    self.DakhilDateTo = ko.observable(formatDate(new Date))
    self.FolioNo = ko.observable('0')
    self.BatchNo = ko.observable()
    self.TransferDate = ko.observable(formatDate(new Date))

    self.DakhilTransferList = ko.observableArray([])
    self.IndividualDakhilTransferList = ko.observableArray([])

    self.Validation = function (data = null) {
        var errMsg = '';



        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        } else {
            return true;
        }

    }

    var record = []

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

    self.Search = () => {
        if (self.ValidateCompany()) {
            $('#tbl_Share_Transfer').DataTable().clear();
            $('#tbl_Share_Transfer').DataTable().destroy();
            Openloader()
            $.ajax({
                type: "POST", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/DakhilTransfer/ShareTransfer/GetShareTransferList',
                data: {
                    'CompCode': self.SelectedCompany(),
                    'RegNoFrom': self.RegNoFrom(),
                    'RegNoTo': self.RegNoTo(),
                    'DateFrom': self.DakhilDateFrom(),
                    'DateTo': self.DakhilDateTo()
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        if (result.message != "") {
                            swal({
                                title: "Warning",
                                text: "Following \n"
                                    + result.message +
                                    "\n Have Been Already Transfered Today. Continue With The Process ?",
                                icon: "warning",
                                buttons: [
                                    'Cancel',
                                    'Continue'
                                ],
                                dangerMode: true,
                            }).then(function (isConfirm) {
                                if (isConfirm) {
                                    $('#tbl_Share_Transfer').DataTable().destroy();
                                    var mappedTasks = $.map(result.responseData, function (item) {
                                        return new DakhilTransfer(item)
                                    });
                                    self.DakhilTransferList(mappedTasks);
                                    dataTableNormal()
                                    
                                } else {
                                    self.Refresh()
                                    
                                }
                            })
                        }
                        else {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new DakhilTransfer(item)
                            });
                            self.DakhilTransferList(mappedTasks);
                            dataTableNormal()
                        }
                    } else {
                        alert('error', result.message)
                        dataTableNormal()
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                    dataTableNormal()
                },
                complete: () => {
                    Closeloader()
                }
            })
        }
    }
    self.Save = (data) => {
        if (self.ValidateCompany()) {
            for (var i = 0; i < self.DakhilTransferList().length; i++) {
                if(self.DakhilTransferList()[i].Selected() == true)
                    record.push(self.DakhilTransferList()[i]);
            }
            if (record.length == 0) {
                alert('error', 'Please Select a Record to Save .')
            } else {
                Openloader()
                $.ajax({
                    type: "POST", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/DakhilTransfer/ShareTransfer/SaveShareTransfer',
                    data: {
                        'CompCode': self.SelectedCompany(),
                        'aTTShareDakhilTransfers': record,
                        'FolioNo': self.FolioNo(),
                        'BatchNo': self.BatchNo(),
                        'TransferedDate': self.TransferDate(),
                        'SelectedAction':data
                    },
                    datatype: "json",
                    success: function (result) {
                        if (result.isSuccess) {
                            alert('success', result.message)
                        } else {
                            alert('error', result.message)
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                    },
                    complete: () => {
                        self.Refresh()
                        Closeloader()
                    }
                })
            }
        }
    }
    self.SearchIndividualData = (data) => {
        if (data != null) {
            if (self.ValidateCompany()) {
                
                Openloader()
                $.ajax({
                    type: "POST", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/DakhilTransfer/ShareTransfer/GetIndividualShareTransferList',
                    data: {
                        'CompCode': self.SelectedCompany(),
                        'RegNo': data.RegNo(),
                        'BHolderNo': data.BHolderNo()
                    },
                    datatype: "json",
                    success: function (result) {
                        if (result.isSuccess) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new DakhilTransfer(item)
                            });
                            self.IndividualDakhilTransferList(mappedTasks);
                            $('#IndividualShareTransferModal').modal('show')
                        } else {
                            alert('error', result.message)
                            $('#IndividualShareTransferModal').modal('hide')

                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                        $('#IndividualShareTransferModal').modal('hide')

                    },
                    complete: () => {
                        Closeloader()
                       
                    }
                })
            }
        }
    }
    self.Refresh = () => {
        $('#tbl_Share_Transfer').DataTable().clear();
        $('#tbl_Share_Transfer').DataTable().destroy();
        dataTableNormal()
        self.RegNoFrom('')
        self.RegNoTo('')
        self.FolioNo('0')
        self.DakhilDateFrom(formatDate(new Date))
        self.DakhilDateTo(formatDate(new Date))
        self.TransferDate(formatDate(new Date))
    }
    self.SelectAll = ko.computed({
        read: () => !self.DakhilTransferList().find(x => !x.Selected()),
        write: t => self.DakhilTransferList().forEach(x => x.Selected(t)),
    })


    dataTableNormal = () => {
        $('#tbl_Share_Transfer').DataTable({
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
    ko.applyBindings(new ShareTransfer());
    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
    $('#simple-date2 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
    dataTableNormal()
   
});
