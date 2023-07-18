function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}


function DividendBatch(data) {
    var self = this;
    if (data != undefined) {
        self.BatchId = ko.observable(data);
        self.BatchText = "BatchNo: " + data
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


var TransctionStatusViewModal = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()


    self.BatchLists = ko.observableArray([])
    self.SelectedBatchId = ko.observable()

    self.TotalRecords = ko.observable(0)

    self.BankUserName = ko.observable()
    self.BankPassword = ko.observable()

    //Dividend ko table list lai
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.compcode = ko.observable();
    self.Divcode = ko.observable();
    self.Description = ko.observable();
    self.FiscalYr = ko.observable();

    //batch validation
    self.BatchValidation = function () {
        var errMsg = ""

        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company !!!<br/>"
        }

        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Choose Dividend !!!<br>"
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
        var companyCode =self.SelectedCompany()
        if (companyCode != '') {
            $.ajax({
                type: "post",
                url: '/ESewaTransaction/TransactionStatus/GetDividendList',
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

    self.LoadDividendLists();
    })

    //Loading dividend lists select options
    self.LoadBatchLists = function () {
        if (self.BatchValidation()) {
            Openloader()
            $.ajax({
                type: "post",
                url: '/ESewaTransaction/TransactionStatus/GetBatchList',
                data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend() },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        if (result.responseData != null) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new DividendBatch(item)
                            });

                            self.BatchLists(mappedTasks);
                        } else {
                            alert('warning', "No Batch List Found ")
                        }
                    } else {
                        alert('error', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }, complete: function(){
                    Closeloader()
                }
            })
        }
    }
    //self.LoadBatchLists();


    self.SelectedDividend.subscribe((data) => {
        if (data != undefined) {
            self.LoadBatchLists();
        } else {
            self.BatchLists([])
        }
    });

    self.SelectedBatchId.subscribe(function () {
        $('#SaveQueryTransactionList').attr('disabled', true);
        self.TotalRecords(0);
        $('#TransactionProcessingStatusTable').DataTable().clear();
        $('#TransactionProcessingStatusTable').DataTable().destroy();

    })
    self.LoadTransactionsStatus = () => {
        if (self.ValidationForm()) {

            $('#TransactionProcessingStatusTable').dataTable({
                processing: true,
                serverSide: true,
                searching: true,
                ordering: true,
                paging: true,
                destroy: true,
                "ajax": {
                    "url": "/ESewaTransaction/TransactionStatus/GetAccountValidatedData",
                    "type": "POST", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    "data": { 'CompCode': self.SelectedCompany(), 'BatchID': self.SelectedBatchId(), 'DivCode': self.SelectedDividend(),'BatchStatus':'CO'},
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
        }
    }


    self.CheckUpdatedStatus  = () => {
            if (self.TotalRecords() > 0) {
                alert('warning','Fetching Status from Esewa!!')
                $.ajax({
                    type: 'POST',
                    url: '/ESewaTransaction/TransactionStatus/UpdateTransactionStatus',
                    async:true,
                    data: { 'CompCode': self.SelectedCompany(), 'BatchNo': self.SelectedBatchId(), 'DivCode': self.SelectedDividend(), 'BankUserName': self.BankUserName(), 'BankPassword': self.BankPassword() },
                    dataType: 'json', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: (result) => {
                        if (result.isSuccess) {
                            alert('success', result.message)
                            LoadTransactionsStatus()
                        } else {
                            alert('error', result.message)
                            
                        }
                    },
                    error: (error) => {
                        alert('error', error.message)
                    }, complete: () => {
                        
                        Coseloader();
                    }

                })
            } else {
                alert('error', 'No Record in Table For Transaction')
            }
        
    }

    self.ValidationForm = () => {
        var errMsg = ""

        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company !!!<br/>"
        }

        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Choose Dividend !!!<br>"
        }
        if (Validate.empty(self.SelectedBatchId())) {
            errMsg += "Please Choose A Batch !!!<br/>"
        }

        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
    }

    self.ValidationFormDetails = () => {
        var errMsg = ""

        if (Validate.empty(self.BankUserName())) {
            errMsg += "Esewa UserName Error !!!<br/>"
        }
        if (Validate.empty(self.BankPassword())) {
            errMsg += "Esewa Password Error !!!<br/>"
        }

        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            $('#loginModal').modal('hide');
            return true;
        }
    }


    self.ClearControl = () => {
        self.TotalRecords(0);
        $('#TransactionProcessingStatusTable').DataTable().clear();
        $('#TransactionProcessingStatusTable').DataTable().destroy();
        $('#CheckStatus').attr('disabled', true);
        self.BankUserName('')
        self.BankPassword('')
        $('#loginModal').modal('hide')
        self.SelectedBatchId('');
        self.DividendLists([]);
        self.SelectedDividend('')

    }

}

$(document).ready(function () {
    $('#SaveQueryTransactionList').attr('disabled', true);

    ko.applyBindings(new TransctionStatusViewModal());

    $('#CheckStatus').attr('disabled', true)
});