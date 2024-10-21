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
function Batch(data) {
    var self = this;
    if (data != undefined) {
        self.BatchId = ko.observable(data);
        self.BatchText = "BatchNo: " + data
    }
}

function ExcelSheetName(data, index) {
    var self = this;
    if (data != undefined) {
        self.SheetName = ko.observable(data);
        self.SheetID = ko.observable(index);
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
    self.CurrentBatchStatus = ko.observable();
    self.TotalRecords = ko.observable(0)

    self.BankUserNameAV = ko.observable()
    self.BankPasswordAV = ko.observable()
    self.BankUserNameTP = ko.observable()
    self.BankPasswordTP = ko.observable()
    self.BankAccountNumberTP = ko.observable()
    self.BankAccountNameTP = ko.observable()

    //excel
    var fileExtension = ""
    var filename = ""
    var extension = ""
    var fdata = ""
    var fdata = ""
    var fileUpload = ""
    var files = ""


    //Excel sheets list lai
    self.SheetLists = ko.observableArray([]);
    self.SelectedSheet = ko.observable();

    // validation
    self.Validation = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Select A Company <br/>"
        }
        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select A Dividend <br/>"
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
        if (Validate.empty(self.BankUserNameTP())) {
            errMsg += "Please Enter A UserName <br/>"
        }
        if (Validate.empty(self.BankPasswordTP())) {
            errMsg += "Please Select A Password <br/>"
        }
        if (Validate.empty(self.BankAccountNameTP())) {
            errMsg += "Please Enter Bank Account Name <br/>"
        }
        if (Validate.empty(self.BankPasswordTP())) {
            errMsg += "Please Enter Bank Account Number <br/>"
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
    }

    // importing cds validation
    self.ImportValidation = function () {
        var errMsg = ""
        if ($('#fileUpload').val() == "") {
            errMsg += "Please Select An Excel File <br/>"
        }
        if (Validate.empty(self.SelectedSheet().SheetID())) {
            errMsg += "Please Select A Sheet First <br/>"
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
    self.LoadCompany1 = function () {

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
    self.LoadCompany1();

    //Loading dividend lists select options
    self.LoadDividendLists = function () {
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            $.ajax({
                type: "post",
                url: '/DividendProcessing/DividendIssueEntry/GetAllDividends',
                data: { 'CompCode': self.SelectedCompany() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                async:true,
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

    //get batch list
    self.LoadBatchList = function () {
        self.BatchList([])
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            if (self.SelectedDividend() != null) {
                Openloader()
                $.ajax({
                    type: "post",
                    url: '/FundTransfer/BatchProcessing/GetAllActiveBatch',
                    data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend() },
                    datatype: "json",
                    async: true, beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new Batch(item)
                            });
                            self.BatchList(mappedTasks);

                        } else {
                            alert('warning', result.message)
                            if (result.responseData == null && result.isValid) {
                                enableDiv('BC', result.isPartial)

                            }
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
        
            self.TotalRecords(0);
            $('#TransactionProcessingStatusTable').DataTable().clear();
            $('#TransactionProcessingStatusTable').DataTable().destroy();
        if (self.CurrentBatch() != null) {
            Openloader()
            $.ajax({
                type: 'POST',
                url: '/FundTransfer/BatchProcessing/CheckBatchStatus',
                data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend(), 'BatchId': self.CurrentBatch() },
                dataType: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                async: true,
                success: (result) => {
                    if (result.isSuccess) {

                        enableDiv(result.responseData, result.partial, result.isValid)
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
    self.CurrentBatch.subscribe(function (){
        if (!Validate.empty(self.CurrentBatch()))
            CheckStatus();
        else self.ClearAllViews();
    })
    self.SelectedDividend.subscribe(() => {

        self.ClearAllViews();
            LoadBatchList();  
    })

    self.ClearAllViews = function() {
        $("#BC,#CI,#CC,#AV").removeClass();
        $("#BC,#CI,#CC,#AV").addClass('btn btn-primary col-11');
        $('#divBC,#divCI,#divCC,#divAV,#divTP,#divCO').hide();
        $('#TransactionProcessingStatusTable').DataTable().clear();
        $('#TransactionProcessingStatusTable').DataTable().destroy();
        enableDiv('BC', false)
}
    //create batch
    self.CreateBatch = function () {
        if (self.Validation()) {
            Openloader()
            $.ajax({
                type: 'POST',
                url: '/FundTransfer/BatchProcessing/CreateBatch',
                data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend()},
                dataType: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        alert('success', result.message);
                        LoadBatchList();
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
                    Closeloader()
                }

            })
        }
    }

    //import data from cds sheet
    $('#fileupload').on('change', function () {
        var goodFile = true;
        fileExtension = ['xls', 'xlsx'];
        filename = $('#fileupload').val();
        if (filename.length == 0) {
            alert('info', "Please select a file.");
            goodFile = false;
        }
        else {
            extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                alert('info', "Please select only excel files.");
                goodFile = false;
            }
        }

        if (goodFile) {
            fdata = new FormData();
            fileUpload = $("#fileupload").get(0);
            files = fileUpload.files;
            fdata.append(files[0].name, files[0]);
            alert('warning', "Uploading Excel File <br/> Donot Leave this Page!!")

            $.ajax({
                type: "POST",
                url: "/FundTransfer/BatchProcessing/GetSheetNames?CompCode="+self.SelectedCompany()+"&DivCode="+self.SelectedDividend()+"&BatchID="+self.CurrentBatch(),
                async: true, beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                data: fdata,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.isSuccess) {
                        var mappedTask = $.map(response.responseData, (item, index) => {
                            return new ExcelSheetName(item, index)
                        });

                        self.SheetLists(mappedTask)
                        alert('success', "Excel File Selected!!")

                    } else {
                        alert('error', response.message)

                    }
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: () => {

                }
            });
        }


    })

    //generate dummy data
    self.GenerateDummyExcel = function () {
        window.location.href = "/FundTransfer/BatchProcessing/DownloadExcelDocument"
}

    //send cds data to server
    self.ImportCDSData = function () {
        if (self.Validation()) {
            if (self.ImportValidation()) {
                var check = true;
                alert('warning', "Uploading Data <br/> Donot Leave this Page!!");
                fileExtension = ['xls', 'xlsx'];
                filename = $('#fileupload').val();
                if (filename.length == 0) {
                    alert("error", "Please select a file.");
                    check = false;
                }
                else {
                    extension = filename.replace(/^.*\./, '');
                    if ($.inArray(extension, fileExtension) == -1) {
                        alert("error", "Please select only excel files.");
                        check = false;
                    }
                }
                if (check == true) {
                    
                    fdata = new FormData();
                    fileUpload = $("#fileupload").get(0);
                    files = fileUpload.files;
                    fdata.append(files[0].name, files[0]);
                    

                    $.ajax({
                        type: "POST", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        url: "/FundTransfer/BatchProcessing/UploadSheet?SheetId=" + self.SelectedSheet().SheetID() + "&CompCode="+self.SelectedCompany()+"&DivCode="+self.SelectedDividend()+"&BatchID="+self.CurrentBatch(),
                        async: true,
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        data: fdata,
                        contentType: false,
                        processData: false,
                       
                        success: function (response) {
                            if (response.isSuccess) {
                                alert('success', response.message)
                                self.CheckStatus()
                            } else {
                                alert('error', response.message)
                            }
                        },
                        error: function (error) {
                            alert('error', error.message)
                        },
                        complete: function () {
                            self.ClearControlCDSImport()
                           
                        }
                    });
                }
            }
        }
       
    }

    //cds data validate
    self.ValidateCDSData = function () {
        if (self.Validation()) {
            alert('warning', "Validating Data <br/> Donot Leave this Page!!")
            $.ajax({
                type: 'POST', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/FundTransfer/BatchProcessing/ValidateCDSData',
                data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend(),'BatchID':self.CurrentBatch() },
                dataType: 'json',
                success: (result) => {
                    if (result.isSuccess) {
                        alert('success', result.message);
                        self.CheckStatus()
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
                   
                }

            })
        }
    }

    //update all bank lists
    self.UpdateBankDetails = function () {
        if (self.Validation()) {
            if (!Validate.empty(self.CurrentBatch())) {
                alert('warning', "Getting Bank Details <br/> Donot Leave this Page!!")
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/FundTransfer/BatchProcessing/UpdateBankDetails',
                    data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend(), 'BatchID': self.CurrentBatch() },
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
                alert('warning', 'Please Check The Status.')
            }
        }
    }

    //account validation
    self.ValidateAccountDetails = function () {
        if (self.Validation() ) {
            if (!Validate.empty(self.CurrentBatch())) {
                $('#loginModalAccountValidation').modal('hide');
                alert('warning', "Validating Accounts <br/> This May Take Some Time!!")
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/FundTransfer/BatchProcessing/ValidateAccountDetails',
                    data: { 'CompCode': self.SelectedCompany(), 'DivCode': self.SelectedDividend(), 'BatchID': self.CurrentBatch(),'BankUserName':self.BankUserNameAV(),'BankPassword':self.BankPasswordAV() },
                    dataType: 'json',
                    success: (result) => {
                        if (result.isSuccess) {
                            alert('success', result.message);
                            CheckStatus();
                            enableDiv('AV', result.isPartial,true)
                        }
                        else {
                            alert('error', result.message);
                            //self.ClearControl()
                        }
                    },
                    error: (error) => {
                        alert('error', error.message);
                        self.ClearControl()
                    },
                    complete: function () {
                       
                        
                        
                    }

                })
            }
            else {
                alert('warning', 'Please Check The Status.')
            }
        }
    }
    
 

    //createing batch after transcation is complete 
    self.CreateBatchAfterComplete = function () {
        swal({
            title: "Are you sure?",
            text: `A New Batch Will Be Created And Old Batch will be set as Completed.`,
            icon: "warning",
            buttons: true,
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
            }
        });
    }
    //route to transaction page
    self.RouteToTransaction = function () {
        window.location.href ="/FundTransfer/FundTransferProcessing"
    }
    self.RouteToStatus = function () {
        window.location.href ="/FundTransfer/TransactionStatus"
    }

    //clear contol bank update
    self.ClearControlBankUpdate = function () {
        self.ClearControl();
        self.CurrentBatch('')
    }

    //clear control cds import
    self.ClearControlCDSImport = function () {
        $('#fileupload').val(null)
        self.SelectedSheet("")
        self.SheetLists([])
    }

    self.initElements = function () {
        $("#BC,#CI,#CC,#AV").removeClass();
        $("#BC,#CI,#CC,#AV").addClass('btn btn-primary col-11')

        $('#divBC,#divCI,#divCC,#divAV,#divTP,#divCO').hide()
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

    self.enableDiv = function (data,isPartial,hasStarted) {
        var divName = '#div' + data
        initElements()
        $('#' + data).removeClass();
        $('#' + data).addClass('btn btn-success col-11');
        $(divName).show()
        if (data == "BC") self.CurrentBatch('')

        if (data == "TP") {
            if (isPartial)
                $('#CompleteDiv').html("Only Partial Account Holders Have Been Validated,Left Out Holders for this Dividend should be added in the Next Batch. Call Higher Authority To Process Transaction.")
            else
                $('#CompleteDiv').html("All Account Holders Have been Validated. Call Higher Authority To Process Transaction.")
        }
        if (data == "CO") {
            if (isPartial)
                $('#CompleteDiv2').html("Only Partial Account Holders Have Been Processed for Transaction,Left Out Holders for this Dividend should be added in the Next Batch. Call Higher Authority To Update Status.")
            else
                $('#CompleteDiv2').html("All Account Holders Have been Processed. Call Higher Authority To Update Status.")
        }

        if (data == "AV" || data == "TP") {
            

            $('#StatusTableDiv').show()
            $('#TransactionProcessingStatusTable').show()
            $('#TransactionProcessingStatusTable').dataTable({
                processing: true,
                serverSide: true,
                searching: true,
                ordering: true,
                paging: true,
                destroy: true,
                buttons: ['excel','pdf'],
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
                    
                    { "data": "validateMessage", "name": "Status", "autoWidth": true },
                ],
            });
            
        } else {
            $('#StatusTableDiv').hide()

        }
        if (data == 'AV') {

            if (hasStarted) {
                $('#AVDiv').hide();
                $('#AVDivH').show();
                $('#AVDiv2').html("Account Validation Started It may take some time!.")
            }
            else {
                $('#AVDivH').hide();
                $('#AVDiv').show();
            }
        }
        
    }

}

$(document).ready(function () {
    ko.applyBindings(new AccountValidationViewModal());

    $('#BC,#CI,#CC,#AV,#TP').attr('disabled', true)
    $('#StatusTableDiv').hide()
    $('#TransactionProcessingStatusTable').hide()
    $('AVDivH').hide();

});
