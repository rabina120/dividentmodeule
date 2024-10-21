function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function Bank(data) {
    var self = this;
    if (data != undefined) {
        self.BankText=ko.observable(data.bankName+'('+data.accountNo+')')
        self.BankID=ko.observable(data.bankID)
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
function Batch(data) {
    var self = this;
    if (data != undefined) {
        self.BatchId = ko.observable(data)
        self.BatchText = "BatchNo: " + data
    }
}


var AccountValidationViewModal = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()

    //Dividend ko table list lai
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.compcode = ko.observable();
    self.Divcode = ko.observable();
    self.Description = ko.observable();
    self.FiscalYr = ko.observable();
    self.BatchList = ko.observableArray([]);
    self.CurrentBatch = ko.observable();
    self.TotalRecords = ko.observable(0)
    
    self.BankUserNameTP = ko.observable()
    self.BankPasswordTP = ko.observable()
    self.BankAccountNumberTP = ko.observable()
    self.BankAccountNameTP = ko.observable()
    self.SourceBankList = ko.observableArray([]);
    self.SelectedBank = ko.observable();

    // validation
    self.Validation = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Select A Company <br/>"
        }
        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select A Dividend <br/>"
        }
        if (Validate.empty(self.CurrentBatch())) {
            errMsg += "Please Select Batch <br/>"
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
    }

  
    self.TransactionProcessingValidation = function () {
        var errMsg = ""
        
        if (Validate.empty(self.SelectedBank())) {
            errMsg += "Please Choose Source Bank Account <br/>"
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
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
                type: "post",
                url: '/FundTransfer/FundTransferProcessing/GetDividendList',
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
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })
        }

    }
    self.SelectedCompany.subscribe(() => {

    self.LoadDividendLists();
    })

    //get batch list
    self.LoadBatchList = function () {
        self.BatchList([])
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            if (self.SelectedDividend() != null) {
                Openloader()
                $.ajax({
                    type: "post",
                    url: '/FundTransfer/FundTransferProcessing/GetAllActiveBatch',
                    data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    async: true,
                    success: function (result) {
                        if (result.isSuccess) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new Batch(item)
                            });
                            self.BatchList(mappedTasks);

                        } else {
                            alert('warning', result.message)
                            
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)

                    },
                    complete: function () {
                        Closeloader()
                    }
                })

            }
        }
    }
    //check batch status
    self.CheckStatus = function () {
        if (self.Validation()) {
            self.TotalRecords(0);
            $('#TransactionProcessingStatusTable').DataTable().clear();
            $('#TransactionProcessingStatusTable').DataTable().destroy();
            Openloader()
            $.ajax({
                type: 'POST',
                url: '/FundTransfer/FundTransferProcessing/CheckBatchStatus',
                data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend(),'BatchID':self.CurrentBatch() },
                dataType: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        enableDiv(result.responseData,result.partial,result.isValid)
                        self.LoadBankList();
                    }
                    else {
                        alert('error', result.message);

                    }
                },
                error: (error) => {
                    alert('error', error.message);
                },
                complete: function () {
                    Closeloader()
                }

            })
        }
    }
    self.SelectedDividend.subscribe(() => {
        LoadBatchList();
    })
    self.CurrentBatch.subscribe(() => {
        CheckStatus();
    })
    //transaction processing
    self.TransactionProcessing  = function () {
        if (self.Validation() && self.TransactionProcessingValidation()) {
            if (!Validate.empty(self.CurrentBatch())) {
                $('#loginModalTransactionProcessing').modal('hide');
                alert('warning', "Transaction Processing Started <br/> This May Take Some Time!!")
                ShowMessage();
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/FundTransfer/FundTransferProcessing/TransactionProcessing',
                    data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend(), 'BatchID': self.CurrentBatch(), 'BankID':self.SelectedBank() },
                    dataType: 'json',
                    success: (result) => {
                        if (result.isSuccess) {
                            alert('success', result.message);
                        }
                        else {
                            alert('error', result.message);
                            self.ClearControl()
                        }
                    },
                    error: (error) => {
                        alert('error', error.message);
                        self.ClearControl()
                    },
                    complete: function () {
                        self.CheckStatus()
                        
                        
                    }

                })
            }
            else {
                alert('warning','Please Check The Status.')
            }
        }
    }
    self.LoadBankList = function () {

        $.ajax({
            type: "post",
            url: '/FundTransfer/FundTransferProcessing/GetSourceBankList',
            datatype: "json",
            data: {'Compcode':self.SelectedCompany()},
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new Bank(item)
                    });
                    self.SourceBankList(mappedTasks);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })


    }
    //route to transaction page
    self.RouteToStatus = function () {
        window.location.href ="/FundTransfer/TransactionStatus"
    }

    self.initElements = function () {
        $("#TP").removeClass();
        $("#TP").addClass('btn btn-primary col-11')

        $('#divTP,#divCO').hide()
    }
    initElements()

    // clear control
    self.ClearControl = function () {
        self.SelectedDividend('')
        self.CurrentBatch('')
        self.initElements()
        $('#DividendList').val(null).trigger('change')
        $('#StatusTableDiv').hide()
        $('#TransactionProcessingStatusTable').hide()


    }
    var ShowMessage = function () {
        $('#btnTP').hide();
        $('#msgTP').show();
        $('#msgTP').html("Transaction is Being Processed Please Wait!!");
    }

    self.enableDiv = function (data,isPartial,isValid) {
        var divName = '#div' + data
        initElements()
        $('#' + data).removeClass();
        $('#' + data).addClass('btn btn-success col-11');
        $(divName).show()

        if (data == "CO") {
            if (isPartial)
                $('#CompleteDiv').html("Only Partial Account Holders Have Been Processed for Transaction,Left Out Holders for this Dividend will be added in the Next Batch. Check Transaction Query For Completed Account Holders to Check Their Status.")
            else
                $('#CompleteDiv').html("All Account Holders Have been Credited. Check Transaction Query For Completed Account Holders to Check Their Status.")
        }

        if (data == "TP" || data == "CO") {
            if (isValid) {
                ShowMessage();
            }
            $('#StatusTableDiv').show()
            $('#TransactionProcessingStatusTable').show()
            $('#TransactionProcessingStatusTable').dataTable({
                processing: true,
                serverSide: true,
                searching: true,
                ordering: true,
                paging: true,
                destroy: true,
                "ajax": {
                    "url": "/FundTransfer/BatchProcessing/GetData",
                    "type": "POST", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    "data": { 'CompCode': self.SelectedCompany(), 'BatchID': self.CurrentBatch(), 'DivCode': self.SelectedDividend(), 'BatchStatus': data },
                    "datatype": "json",
                    "dataSrc": function (result) {
                        if (result.data.length == 0) {
                            alert('error', 'No Record Found !!!')
                        }
                        else {
                            $('#CheckStatus').attr('disabled', false);
                            self.TotalRecords(result.recordsTotal)
                        }
                        return result.data
                    }, "error": function (xhr, error, code) {
                        alert('error', code)
                    }
                },
                "columns": [
                    { "data": "fullName", "name": "FullName", "autoWidth": true },
                    { "data": "warrantNo", "name": "WarrantNo", "autoWidth": true },
                    { "data": "bankNo", "name": "BankNo", "autoWidth": true },
                    { "data": "bankName", "name": "BankName", "autoWidth": true },
                    { "data": "warrantAmt", "name": "WarrantAmt", "autoWidth": true },
                    //{ "data": "sub_token", "name": "sub_token", "autoWidth": true },
                    { "data": "transactionDetail", "name": "Status", "autoWidth": true },
                ],
            });
        } else {
            $('#StatusTableDiv').hide()

        }
        
    }

}

$(document).ready(function () {
    ko.applyBindings(new AccountValidationViewModal());
    $('#DividendList').select2({})
    $('#TP').attr('disabled', true)
    $('#StatusTableDiv').hide()
    $('#TransactionProcessingStatusTable').hide()

});
