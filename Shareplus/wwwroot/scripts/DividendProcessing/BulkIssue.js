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


function ExcelSheetName(data, index) {
    var self = this;
    if (data != undefined) {
        self.SheetName = ko.observable(data);
        self.SheetId = ko.observable(index);
    }
}


var BulkIssue = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()

    self.PostingRemarks = ko.observable()
    self.ActionType = ko.observable()


    //Dividend ko table list lai
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.CashDividendTableName = ko.observable();


    self.DivBasedOn = ko.observable();
    self.IsPaidBy = ko.observable(false);
    self.isIssue = ko.observable(false);
    self.isPay = ko.observable(false);
    self.SheetLists = ko.observableArray([]);
    self.SelectedSheet = ko.observable();


    self.TotalUploadedRecords = ko.observable();
   
    self.CashDividendList = ko.observable([])

    self.PostingDate(formatDate(new Date))


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
        if (self.ValidateCompany()) {
            Openloader()
            var companyCode = self.SelectedCompany()
            $.ajax({
                type: "post",
                url: '/DividendProcessing/BulkIssue/GetAllDividends',
                data: { 'CompCode': companyCode }, beforeSend: function (xhr) {
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
                }, complete: function () {
                    Closeloader()
                }
            })

        }
    }
    self.SelectedCompany.subscribe(() => {
        if (!Validate.empty(self.SelectedCompany())) {
            self.LoadDividendLists();
        }
    })

    //self.isIssue.subscribe(() => {
    //    if (!Validate.empty(self.SelectedCompany())) {
    //        self.LoadDividendLists();
    //    }
    //    else {
    //        alert('error', 'Please Select Company');
    //    }

    //})
    //self.isPay.subscribe(() => {
    //    if (!Validate.empty(self.SelectedCompany())) {
    //        self.LoadDividendLists();
    //    }
    //    else {
    //        alert('error', 'Please Select Company');
    //    }
    //})


    self.DummyDataDownload = function () {
        if (Validate.empty(self.DivBasedOn())) {
            alert('warning', 'Please Select a Div Type First !!!')
        } else {
            window.location.href = "/DividendProcessing/BulkIssue/DownloadExcelDocument?DivBasedOn=" + self.DivBasedOn()
        }
    }


    self.Validation = function () {
        var errMsg = "";

        if (self.SelectedCompany() == undefined) {
            errMsg += "Please Choose Company !!!</br>";
        }
        if (self.isIssue() == false && self.isPay() == false) {
            errMsg += "Please Bulk Upload Type!!!</br>";
        }

        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Select Dividend !!!</br>";
        }
        if (Validate.empty(self.PostingRemarks())) {
            errMsg += "Please Enter Posting Remarks !!!</br>";
        }
        if (Validate.empty(self.PostingDate())) {
            errMsg += "Please Enter Posting Date !!!</br>";
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }

    }

    var fileExtension = ""
    var filename = ""
    var extension = ""
    var fdata = ""
    var fdata = ""
    var fileUpload = ""
    var files = ""

    $('#fileupload').on('change', function () {
        fileExtension = ['xls', 'xlsx'];
        filename = $('#fileupload').val();
        if (filename.length == 0) {
            alert("Please select a file.");
            return false;
        }
        else {
            extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                alert("Please select only excel files.");
                return false;
            }
        }
        fdata = new FormData();
        fileUpload = $("#fileupload").get(0);
        files = fileUpload.files;
        fdata.append(files[0].name, files[0]);
        Openloader()

        $.ajax({
            type: "POST",
            url: "/DividendProcessing/BulkIssue/GetSheetNames",

            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: fdata,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.isSuccess) {
                    var mappedTask = $.map(response.responseData, (item, i) => {
                        return new ExcelSheetName(item, i)
                    });

                    self.SheetLists(mappedTask)
                } else {
                    alert('error', response.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            },
            complete: () => {
                Closeloader()

            }
        });

    })

    self.SelectAll = ko.computed({
        read: () => !self.CashDividendList().find(x => !x.Selected()),
        write: t => self.CashDividendList().forEach(x => x.Selected(t)),
    })


    self.UploadExcel = function () {

            fileExtension = ['xls', 'xlsx'];
            filename = $('#fileupload').val();
            if (filename.length == 0) {
                alert("Please select a file.");
                return false;
            }
            else {
                extension = filename.replace(/^.*\./, '');
                if ($.inArray(extension, fileExtension) == -1) {
                    alert("Please select only excel files.");
                    return false;
                }
            }
        if (extension == 'xlsx') {
            alert('info','XLSX Format may not be Supported by the Server!!')
        }
            fdata = new FormData();
            fileUpload = $("#fileupload").get(0);
            files = fileUpload.files;
            fdata.append('postedFile', files[0]);;

                swal({
                    title: "Are you sure?",
                    text: "You want to Upload the Excel File",
                    icon: "warning",
                    buttons: true,
                    dangerMode: true
                }).then((willSave) => {
                    if (willSave) {
                        Openloader()
                        $.ajax({
                            type: "POST",
                            url: "/DividendProcessing/BulkIssue/UploadSheet?SheetId=" + ko.toJS(self.SelectedSheet().SheetId)+"&DivType="+ko.toJS(self.DivBasedOn()),
                            async: true,
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            data: fdata,
                            contentType: false,
                            processData: false,
                            async: false,
                            success: function (response) {
                                if (response.isSuccess) {
                                    $('#uploadExcelDiv').hide()
                                    $('#uploadExcelRecordsDiv').show()
                                    $('#BulkIssuePayDiv').show()
                                    self.TotalUploadedRecords(response.totalRecords)

                                } else {
                                    alert('error', response.message)
                                    self.ClearControl()

                                }
                            },
                            error: function (error) {
                                alert('error', error.message)
                                self.ClearControl()
                            },
                            complete: () => {
                                Closeloader()

                            }
                        });
                    }
                });
            
    }

    self.PostIssue = function (data) {
        if (self.Validation()) {
            Openloader()
            $.ajax({
                type: "POST", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: "/DividendProcessing/BulkIssue/BulkDividendIssue",
                data: {
                    'CompCode': self.SelectedCompany(),
                    'DivCode': self.SelectedDividend(),
                    'DivType': self.DivBasedOn(),
                    'IssueDate': self.PostingDate(),
                    'isIssue': self.isIssue(),
                    'isPay': self.isPay(),
                    'IssueRemarks': self.PostingRemarks(),
                    'FileName': $('#fileupload').val().split('\\').pop(),
                    'SheetId': self.SelectedSheet().SheetId
                },
                dataType: "json",
                success: (result) => {
                    if (result.isSuccess) {
                        if (self.isIssue()){
                            if (result.issuedRecords > 0) {
                                alert('success', result.issuedRecords + ' RECORDS ISSUED')
                            }
                            else {
                                alert('warning', 'ISSUE FAILED: NO RECORDS MATCHED ')
                            }
                        }
                        if (self.isPay()) {
                            if (result.payedRecords > 0) {
                                alert('success', result.payedRecords + ' RECORDS PAYED')
                            }
                            else {
                                alert('warning', 'PAYMENT FAILED: NO RECORDS MATCHED ')
                            }
                        } 
                    }
                    else {
                        alert('warning', result.Message)
                    }
                },
                error: (error) => {
                    alert('error', error.message)
                },
                complete: () => {
                   
                }
            })
            self.ClearControl()
            Closeloader()
        }
    }

    self.ClearControl = function () {
        $('#tbl_Wissue').DataTable().clear()
        $('#tbl_Wissue').DataTable().destroy();
        $('#tbl_Wissue tbody').empty();

        $('#chk').prop('checked', false);
        self.PostingRemarks('')
        self.LoadDividendLists()
        //self.SelectedDividend(null)
        //self.SelectedDividend(undefined)
        $('#DividendList').val(null).trigger('change');
        $('#tbl_Wissue').DataTable({
            responsive: false,
            searching: false,
            scrollX: true,
            scrollY: true,
            paging: false,
            ordering: false,
            fixedHeader:true
        })
        self.TotalUploadedRecords('');
        $('#uploadExcelDiv').show();
        $('#uploadExcelRecordsDiv').hide();

        self.SelectedSheet('')
        $('#fileupload').val('')
    }
    self.ClearControl()

}
$(document).ready(function () {
    ko.applyBindings(new BulkIssue());
    $('#BulkIssuePayDiv').hide()
    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
   
});
