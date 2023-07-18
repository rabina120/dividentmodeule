function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}
function CertificateDetail(data) {
    var self = this;
    if (data != undefined) {
        self.CDCompcode = ko.observable(data.compcode);
        self.CDCert_Id = ko.observable(data.cert_Id);
        self.CDShare_type = ko.observable(data.share_type);
        self.CDDescription = ko.observable(data.description);
        self.CDIssuse_Dt = ko.observable(data.issuse_Dt);
        self.CDStart_SrNo = ko.observable(data.start_SrNo);
        self.CDEnd_SrNo = ko.observable(data.end_SrNo);
    }
}

function BulkCAEntry(data) {
    var self = this;
    if (data != undefined) {
        self.ShHolderNo = ko.observable(data.shHolderNo)
        self.CertNo = ko.observable(data.certNo)
        self.SrNoFrom = ko.observable(data.srNoFrom)
        self.SrNoTo = ko.observable(data.srNoTo)
        self.Kitta = ko.observable(data.kitta)
        self.BOID = ko.observable(data.boid)
        self.DPID = ko.observable(data.dpid)
        self.DPCode = ko.observable(data.dpCode == null ? "000" : data.dpCode)
        self.ISIN_NO = ko.observable(data.isiN_NO)
        self.HolderName = ko.observable(data.holderName)
    }
}

function ExcelSheetName(data, index) {
    var self = this;
    if (data != undefined) {
        self.SheetName = ko.observable(data);
        self.SheetId = ko.observable(index);
    }
}
function ShOwnerType(data) {
    var self = this;
    if (data != undefined) {
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.ShOwnerTypeName = ko.observable(data.shOwnerTypeName);
    }
}

var DeMaterializeEntry = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();


    //certificalte dwetail table
    self.CDcompcode = ko.observable();
    self.CDCert_Id = ko.observable();
    self.CDShare_type = ko.observable();
    self.CDDescription = ko.observable();
    self.CDIssuse_Dt = ko.observable();
    self.CDStart_SrNo = ko.observable();
    self.CDEnd_SrNo = ko.observable();
    self.SelectedCertDetail = ko.observable();
    self.CertificateDetailList = ko.observableArray([]);

    self.UploadFrom = ko.observable('E');
    self.CertDetailList = ko.observableArray([])
    self.SelectedCertDetail = ko.observable()
    self.OwnerTypeList = ko.observableArray([])
    self.SelectedOwnerType = ko.observable()
    self.SheetLists = ko.observableArray([])
    self.SelectedSheet = ko.observable()
    self.StartRow = ko.observable()
    self.EndRow = ko.observable()
    self.TransactionDate = ko.observable(formatDate(new Date))
    self.ExcelDataList = ko.observableArray([])

    self.TotalKitta = ko.observable();

    // for showing details
    self.Details = ko.observableArray([]);
    self.TotalRecords = ko.observable(0);
    //variables
    record = [];
    var fileExtension = ""
    var filename = ""
    var extension = ""
    var fdata = ""
    var fdata = ""
    var fileUpload = ""
    var files = ""
    self.OwnerTypeList.push({ 'ShOwnerTypeName': 'Promoter','ShOwnerType':'1'})
    self.OwnerTypeList.push({ 'ShOwnerTypeName': 'Public', 'ShOwnerType': '3' })

    var totalKitta = 0;

    self.Validation = function (data) {
        var errMsg = "";

        if (Validate.empty(self.SelectedCertDetail())) {
            errMsg += "Please Select A Bonus Issue .<br/>";
        }

        if (Validate.empty(self.SelectedOwnerType())) {
            errMsg += "Please Select An Owner Type .<br/>";
        }
        if (filename.length == 0) {
            errMsg += "Please Select An Excel File .<br/>";
        }
        if (Validate.empty(self.SelectedSheet())) {
            errMsg += "Please Select A Sheet .<br/>";
        }


        if (errMsg == "") {
            return true;
        }
        else {
            alert('warning', errMsg);
            return false;
        }
    }
    self.UploadFrom.subscribe(function () {
        if (self.UploadFrom() == "S") {
            $('#fileupload,#SheetNameList').attr('disabled', true)
        } else {
            $('#fileupload,#SheetNameList').attr('disabled', false)
        }
    })

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
                        return new ParaComp(item);
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

    //certificate details
    self.GetBonusIssueList = function () {
        if (self.ValidateCompany()) {
            var companyCode = self.SelectedCompany();
            $.ajax({
                type: 'POST',
                url: '/CDS/DematerializeEntry/GetDataFromCertificateDetail',
                data: { 'CompCode': companyCode },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                dataType: 'json',
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new CertificateDetail(item);
                        });
                        self.CertDetailList(mappedTasks);
                    }
                    else {
                        alert('error', result.message);
                    }

                }, error: function (error) {
                    alert('error', error.message);
                }
            })
        }
        else self.CertDetailList([]);
        
    }
    self.SelectedCompany.subscribe(()=>{
        self.GetBonusIssueList();
    })

    //generate dummy data
    self.DummyDataDownload = function () {

        window.location.href = "/CDS/BulkCAEntry/DownloadExcelDocument"

    }
    //ownerType
    
    

    //file change
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
            type: "post",
            url: "/CDS/BulkCAEntry/GetSheetNames",

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

    self.UploadExcel = function () {
        if (self.ValidateCompany()) {
            if (self.Validation()) {
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
                fdata.append('postedFile', files[0]);;

                if (self.Validation()) {
                    swal({
                        title: "Are you sure?",
                        text: "You want to Upload the Excel File",
                        icon: "warning",
                        buttons: true,
                        dangerMode: true
                    }).then((willSave) => {
                        if (willSave) {
                            $('#tbl_Excel_Data').DataTable().clear();
                            $('#tbl_Excel_Data').DataTable().destroy();
                            Openloader()
                            $.ajax({
                                type: "POST",
                                url: "/CDS/BulkCAEntry/Upload?SheetId=" + ko.toJS(self.SelectedSheet().SheetId()) + "&StartRow=" + self.StartRow() + "&EndRow=" + self.EndRow() + "&CompCode=" +  self.SelectedCompany() + "&CertDetailId=" + self.SelectedCertDetail() + "&ShOwnerType=" + self.SelectedOwnerType(),
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader("XSRF-TOKEN",
                                        $('input:hidden[name="__RequestVerificationToken"]').val());
                                },
                                data: fdata,
                                contentType: false,
                                processData: false,
                                async: false,
                                success: (data) => {
                                    if (data.isSuccess) {
                                        if (data.responseData.length > 0) {
                                            var mappedTasks = $.map(data.responseData, function (item) {
                                                return new BulkCAEntry(item)
                                            });
                                            self.ExcelDataList(mappedTasks)
                                            self.TotalKitta(self.ExcelDataList().reduce((n, { Kitta }) => n + parseInt(Kitta()), 0))
                                            self.DisableAfterUpload()
                                        }
                                        else {
                                            alert('error', data.message)
                                            self.Refresh()
                                        }
                                    } else {
                                        alert('error', data.message)
                                        self.Refresh()

                                    }
                                },
                                error: (error) => {
                                    alert('error', error.message)
                                    self.Refresh()

                                },
                                complete: () => {
                                    Closeloader()
                                    dataTableNormal()
                                }

                            });
                        }

                    });

                }
            }
        }


    }
    self.Save = function () {
        if (self.ValidateCompany()) {
            if (self.Validation()) {

                swal({
                    title: "Are you sure?",
                    text: "The Above Records Will Be Demated ",
                    icon: "warning",
                    buttons: true,
                    dangerMode: true
                }).then((willSave) => {
                    if (willSave) {
                        Openloader()

                        $.ajax({
                            type: 'post',
                            url: '/CDS/BulkCAEntry/SaveBulkCAEntry',
                            data: {

                                'aTTBulkCAEntries': self.ExcelDataList(),
                                'CompCode': self.SelectedCompany(),
                                'TransactionDate': self.TransactionDate(),
                                'CertDetail': self.SelectedCertDetail()
                            },
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            dataType: 'json',
                            success: function (result) {
                                if (result.isSuccess) {
                                    swal({
                                        title: "Certificate Dematerialized ",
                                        text: result.message,
                                        icon: "info",
                                        button: "Ok",
                                    }).then(function (result) {
                                        self.Refresh();
                                        location.reload();
                                    });
                                }
                                else {
                                    alert('error', result.message);
                                }

                            }, error: function (error) {
                                alert('error', error.message);
                            },
                            complete: () => {
                                self.Refresh()
                                $('#tbl_Excel_Data').DataTable().clear();
                                $('#tbl_Excel_Data').DataTable().destroy();
                                dataTableNormal()
                                Closeloader()

                            }
                        })
                    }
                })
            }
        }
    }


    self.OpenPopup = function (data) {
        self.TotalRecords(0);
        if (self.SelectedCompany() != null && self.SelectedOwnerType() != null) {
            $('#DetailsPopup').modal('show');
            $('#tbl_Details').show();
            $('#tbl_Details').dataTable({
                processing: true,
                serverSide: true,
                searching: true,
                ordering: true,
                paging: true,
                destroy: true,
                "ajax": {
                    "url": "/CDS/BulkCAEntry/GetData",
                    "type": "POST", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    "data": { 'CompanyCode': self.SelectedCompany(), 'Cret_Id': self.SelectedCertDetail(), 'ShOwnerType': self.SelectedOwnerType() },
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
                    { "data": "certNo", "name": "certNo", "autoWidth": true },
                    { "data": "shHolderNo", "name": "shHolderNo", "autoWidth": true },
                    { "data": "holderName", "name": "holderName", "autoWidth": true },
                    { "data": "srNoFrom", "name": "srNoFrom", "autoWidth": true },
                    { "data": "srNoTo", "name": "srNoTo", "autoWidth": true },
                    { "data": "kitta", "name": "kitta", "autoWidth": true },
                    { "data": "boid", "name": "boid", "autoWidth": true },
                    { "data": "dpid", "name": "dpid", "autoWidth": true },
                    { "data": "isiN_NO", "name": "isIN_NO", "autoWidth": true },
                    { "data": "description", "name": "description", "autoWidth": true },
                ],
            });
        }
        else {
            alert('warning', 'Please choose Company and Owner Type');
        }
    }

    self.GenerateReport = function (data) {
        let companyCode = self.SelectedCompany();
        let CompEnName = self.CompanyDetails().find(x => x.CompCode() == companyCode).CompEnName();
        console.log(CompEnName);
        Openloader();
        $.ajax({
            type: "post",
            url: '/CDS/BulkCAEntry/GenerateReport',
            data: { 'CompanyCode': self.SelectedCompany(), 'Cret_Id': self.SelectedCertDetail(), 'ShOwnerType': self.SelectedOwnerType(), 'ReportType': data, 'CompEnName': CompEnName },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var fileName = result.message;
                    var a = document.createElement("a");
                    a.href = "data:application/octet-stream;base64," + result.responseData;
                    a.download = fileName;
                    a.click();
                }
                else {
                    alert('error', result.message);
                }
            },
            error: function (error) {
                alert('error', error.message)
                self.ClearControl()
            },
            complete: () => {
                Closeloader()

            }
        })
    }





    self.Refresh = function () {
        self.SelectedCertDetail('')
        self.SelectedOwnerType('')
        self.SelectedSheet('')
        self.SheetLists([])
        self.StartRow('')
        self.EndRow('')
        self.TotalKitta('')
        $("#fileupload").val('')
        $('#CertDetails,#OwnerType,#fileupload,#SheetNameList,#StartRow,#EndRow,#TransactionDate,#UploadExcel').attr('disabled', false)

    }

    self.DisableAfterUpload = function () {
        $('#CertDetails,#OwnerType,#fileupload,#SheetNameList,#StartRow,#EndRow,#TransactionDate,#UploadExcel').attr('disabled', true)
    }
    dataTableNormal = () => {
        $('#tbl_Excel_Data').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: false,
            columnDefs: [
                { width: 100, targets: [1, 3, 4, 5, 6, 8, 9] },
                { width: 120, targets: [2, 7, 10] }
            ],
            fixedColumns: true,
            dom: 'Bfrtip',
            buttons: [
                'excel'
            ]

        })
    }

}

$(function () {
    ko.applyBindings(new DeMaterializeEntry())
    dataTableNormal()
})