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
        self.SheetID = ko.observable(index);
    }
}
self.DivUploadViewModel = function () {
    var self = this;
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.DivUploadList = ko.observableArray([]);
    self.Boid = ko.observable();
    self.Name = ko.observable();
    self.Tax = ko.observable(parseFloat(0).toFixed(2));
    self.Remarks = ko.observable();
    self.Date = ko.observable();
    self.SelectedAction = ko.observable();
    self.PledgeReleseStatus = ko.observable(true);

    self.UploadList = ko.observableArray([
        { Code: 'TL', DisplayName: 'Taxiation Upload' },
        { Code: 'PL', DisplayName: 'Pool Account Upload' }
    ]);

    self.SelectedDivType = ko.observable();


    self.AddAction = function () {
        self.SelectedAction('A')
        $("#Delete").attr("disabled", true);
    }
    self.DeleteAction = function () {
        self.SelectedAction('D')
        $("#Add").attr("disabled", true);
    }

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
        if (!Validate.empty(self.SelectedCompany())) {
            self.DividendLists([]);
            $.ajax({
                type: "post",
                url: '/DividendProcessing/DemateDividendIssueEntry/GetAllDemateDividends',
                data: { 'CompCode': self.SelectedCompany() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                async: true,
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
    })

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

    self.DownloadDummyExcel = () => {
        if (Validate.empty(self.SelectedDivType())) {
            toastr.error("Select a upload type first!!!");
        }
        else {
            window.location.href = "/DividendManagement/DivUpload/DownloadExcelDocument?DivType=" + self.SelectedDivType();
            return true;
        }
    }

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
            url: "/DividendManagement/DivUpload/GetSheetNames?CompCode=" + self.SelectedCompany() + "&DivCode=" + self.SelectedDividend() + "&DivType=" + self.SelectedDivType(),
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

    self.UploadExcel = () => {
        fileExtension = ['xls', 'xlsx'];
        filename = $('#fileupload').val();

        if (filename.length == 0) {
            toastr.error("Please select a file.");
            return false;
        }
        else {
            extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                toastr.error("Please select only excel files.");
                return false;
            }
        }
        fdata = new FormData();
        fileUpload = $("#fileupload").get(0);
        files = fileUpload.files;
        fdata.append('postedFile', files[0]);

        //console.log(files)

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
                    url: "/DividendManagement/DivUpload/UploadSheet?SheetId=" + ko.toJS(self.SelectedSheet()).SheetID + "&CompCode=" + self.SelectedCompany() + "&DivCode=" + self.SelectedDividend() + "&DivType=" + self.SelectedDivType(),
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
                            console.log(data.responseData)
                            data.responseData.map(x => self.DivUploadList.push(x));
                            //self.DivUploadList(data.responseData);
                        } else {
                            alert('error', data.message)

                        }
                    },
                    error: (error) => {
                        alert('error', error.message)

                    },
                    complete: () => {
                        Closeloader()
                    }

                });

                //let prdata = {
                //    fdata,
                //    SheetId: self.SelectedSheet(),
                //    CompCode: self.SelectedCompany(),
                //    DivCode: self.SelectedDividend()
                //}
                //postReq(
                //    "/DividendManagement/DivUpload/UploadSheet?SheetId=" + ko.toJS(self.SelectedSheet()).SheetID + "&CompCode=" + self.SelectedCompany() + "&DivCode=" + self.SelectedDividend(),
                //    fdata,
                //    resp => {
                //        console.log(resp)
                //    }
                //)
            }

        });



    }
    self.AddDiv = function (data) {
        //console.log(self.Boid());
        var errMsg = "";
        if (Validate.empty(self.Boid())) errMsg += "Boid Cannot be empty!!<br>";
        if (Validate.empty(self.Name())) errMsg += "Name Cannot be empty!!<br>";
        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        }
        else {
            self.DivUploadList.push({
                boid: self.Boid(),
                name: self.Name(),
                tax: self.Tax(),
            })
            self.Boid(null);
            self.Name(null);
            self.Tax(null);
        }
    }
    self.EditDiv = function (data) {
        console.log(data);
        self.Boid(data.boid);
        self.Name(data.name);
        self.Tax(data.tax);
        self.DivUploadList.remove(data);

    }
    self.DeleteDiv = function (data) {
        self.DivUploadList.remove(data);
    }

    self.UploadDivUploadListFinal = function () {
        var errMsg = "";
        if (Validate.empty(self.SelectedCompany())) errMsg += "Boid Cannot be empty!!<br>";
        if (Validate.empty(self.SelectedDividend())) errMsg += "Boid Cannot be empty!!<br>";
        if (Validate.empty(self.SelectedDivType())) errMsg += "Upload Type Cannot be empty!!<br>";
        if (self.DivUploadList().length == 0) errMsg += "Boid Cannot be empty!!<br>";
        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        }
        else {
            postReqMsg(
                '/DividendManagement/DivUpload/SaveDivUploadList',
                {
                    CompCode: self.SelectedCompany(),
                    DivCode: self.SelectedDividend(),
                    DivUploadList: self.DivUploadList(),
                    PRStatus: self.StatusName(),
                    SelectedAction: self.SelectedAction()
                },
                null,
                resp => {
                    if (resp.IsSuccess) {
                        self.SelectedCompany(null);
                        self.DivUploadList([]);
                    }
                }
            )
        }
    }

}

$(document).ready(function () {
    ko.applyBindings(new self.DivUploadViewModel());
})