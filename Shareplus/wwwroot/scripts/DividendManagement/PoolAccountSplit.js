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

function PoolAccountSplitViewModel() {
    var self = this;
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.DividendLists = ko.observableArray();
    self.SelectedDividend = ko.observable();
    self.CompCode = ko.observable(localStorage.getItem('company-code'));
    self.HolderInfo = ko.observable();
    self.BOID = ko.observable();
    self.BoidInfoArray = ko.observable();
    self.Action = ko.observable();

    self.AddSplitEntry = function () {
        self.Action('A');
        $("#DeleteSplitEntry").attr("disabled", true);
    }
    self.DeleteSplitEntry = function () {
        self.Action('D');
        $("#AddSplitEntry").attr("disabled", true);

    }


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

    self.GetInfoValidation = function () {
        var msg = "";
        if (Validate.empty(self.SelectedCompany())) {
            msg += "Company cannot be empty!! <br/>"
        }
        if (Validate.empty(self.BOID())) {
            msg += "BOID number cannot be empty!! <br/>"
        }
        if (Validate.empty(self.SelectedDividend())) {
            msg += "Please select a the divident year!! <br/>"
        }
        if (msg !== "") {
            alert('error', msg);
            return false;
        }
        else {
            return true;
        }
    }
    $("#holderInfo :input").attr("disabled", true);
    self.BOIDChange = (boid) => {
        self.BoidInfoArray(null);
        if (self.GetInfoValidation()) {
            //console.log(boid, 'boid');
            var data = {
                BOID: self.BOID(),
                DivCode: self.SelectedDividend(),
                CompCode: self.SelectedCompany(),
                Action: self.Action(),
            }
            postReq(
                '/DividendManagement/PoolAccountSplit/GetHolderInfoForSplit',
                data,
                null,
                resp => {
                    //console.log(resp);
                    self.BoidInfoArray(resp);
                },
                null
            )
            //console.log(ko.toJS(self.BoidInfoArray));
        }

    }

    self.ShowHolderInfo = ko.computed(function () {
        //console.log(ko.toJS(self.BoidInfoArray)?.fullname, "----------");
        if (ko.toJS(self.BoidInfoArray)?.fullname != undefined) return true;
        else return false;
    })

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

    var companyCode = localStorage.getItem('company-code');
    //console.log(companyCode);
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
                    if (companyCode != null || companyCode != '' || companyCode != undefined) {
                        self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode)?.CompCode());
                        //$("#CompanyList").attr("disabled", true);
                    }
            } else {
                alert('warning', result.message)
            }
        },
        error: function (error) {
            alert('error', error.message)
        }
    })

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
            url: "/DividendManagement/PoolAccountSplit/GetSheetNames?CompCode=" + self.SelectedCompany() + "&DivCode=" + self.SelectedDividend() + "&BOID=" + self.BOID(),
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
                Closeloader()
            },
            complete: () => {
                Closeloader()

            }
        });

    })

    self.SelectedCompany.subscribe(() => {
        //self.LoadDividendList();
        self.DividendLists([]);
        if (self.SelectedCompany != undefined) {
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

    });
    
    
    
    self.StartRow = ko.observable();
    self.EndRow = ko.observable();

    self.UploadExcel = () => {
        if (self.Action() == 'A') {

            var errText = "";
            fileExtension = ['xls', 'xlsx'];
            filename = $('#fileupload').val();
            //console.log(self.BOIDChange())
            if (Validate.empty(self.SelectedCompany())) {
                errText += "Select a Company.<br>"
            }
            if (Validate.empty(self.SelectedDividend())) {
                errText += "Select a dividend.<br>"
            }
            if (Validate.empty(self.BOID())) {
                errText += "BOID cannot be empty.<br>"
            }
            if (filename.length == 0) {
                errText += "Please select a file.";
            }
            if (errText != "") {
                alert('error', errText);
                return false;
            }
            else {
                let errlist = "";
                extension = filename.replace(/^.*\./, '');
                if ($.inArray(extension, fileExtension) == -1) {
                    errlist += "Please select only excel files.<br>";
                }
                if (self.SelectedSheet() == undefined) {
                    errlist += "Please select a sheet name.<br>"
                }
                if (errlist != "") {
                    toastr.error(errlist);
                    return false;
                }
            }
            fdata = new FormData();
            fileUpload = $("#fileupload").get(0);
            files = fileUpload.files;
            fdata.append('postedFile', files[0]);;

            swal({
                title: "Are you sure?",
                text: "You want to Submit this List!!",
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
                        url: "/DividendManagement/PoolAccountSplit/UploadSheet?SheetId=" + ko.toJS(self.SelectedSheet()).SheetID + "&CompCode=" + self.SelectedCompany() + "&DivCode=" + self.SelectedDividend() + "&BOID=" + self.BOID() + "&ActionType=" + self.Action(),
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        data: fdata,
                        contentType: false,
                        processData: false,
                        async: false,
                        success: (data) => {
                            //Closeloader();
                            //console.log(data);
                            if (data.isSuccess) {
                                toastr.success(data.message);
                                self.ClearAll()
                            } else {
                                toastr.error(data.message);
                            }
                            //    //if (data.responseData.length > 0) {
                            //    //    var mappedTasks = $.map(data.responseData, function (item) {
                            //    //        return new BulkCAEntry(item)
                            //    //    });
                            //    //    self.ExcelDataList(mappedTasks)
                            //    //    self.TotalKitta(self.ExcelDataList().reduce((n, { Kitta }) => n + parseInt(Kitta()), 0))
                            //    //    self.DisableAfterUpload();
                            //    //    Closeloader();
                            //    //}
                            //    //else {
                            //    //    alert('error', data.message);
                            //    //    Closeloader();
                            //} else {
                            //    alert('error', data.message);
                            //}
                        },
                        error: (error) => {
                            alert('error', error.message)
                        },
                        complete: () => {
                            Closeloader()
                            //dataTableNormal()
                        }

                    });
                }

            });
        } else {
            if (self.BoidInfoArray() == undefined) {
                toastr.error("Search your BOID First!!!");
                return false
            } else {
                swal({
                    title: "Are you sure?",
                    text: "You want to delete this BOID!!!",
                    icon: "warning",
                    buttons: true,
                    dangerMode: true
                }).then((willSave) => {
                    if (willSave) {
                        let sheetid = 0;
                        $.ajax({
                            type: "POST",
                            url: "/DividendManagement/PoolAccountSplit/UploadSheet?SheetId=" + sheetid + "&CompCode=" + self.SelectedCompany() + "&DivCode=" + self.SelectedDividend() + "&BOID=" + self.BOID() + "&ActionType=" + self.Action(),
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            contentType: false,
                            processData: false,
                            async: false,
                            success: (data) => {
                                //Closeloader();
                                //console.log(data);
                                if (data.isSuccess) {
                                    toastr.success(data.message);
                                    self.ClearAll()
                                } else {
                                    toastr.error(data.message);
                                }
                            },
                            error: (error) => {
                                alert('error', error.message)
                            },
                            complete: () => {
                                Closeloader()
                                //dataTableNormal()
                            }

                        });
                    }
                })
            }
            
        }

    }

    self.DownloadDummyExcel = () => {
        window.location.href = "/DividendManagement/PoolAccountSplit/DownloadExcelDocument";
        return true;
    }


    self.SavePoolSplit = () => {
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

                data = {
                    file: fdata,
                    CompCode: self.SelectedCompany(),
                    Dividend: self.SelectedDividend(),

                }

                postReq(
                    '/DividendManagement/PoolAccountSplit/SavePoolAccount',
                    data,
                    null,
                    null,
                    null
                )
            }


            //var HolderInfo = {
            //    CompCode: self.CompCode(),
            //    TranType: self.TransferType(),
            //    PreviousBOID: self.BoidKeyword(),
            //    NewBoid: self.NewBOID()
            //}
            //postReq(
            //    '/DemateDividendManagement/Botobotransfer/SaveHolderForBoidChange',
            //    { HolderInfo },
            //    null,
            //    null,
            //    null
            //)
        }
    }

    self.ClearAll = () => {
        self.BOID(null);
        self.BoidInfoArray(null);
        fdata = new FormData();
        self.SelectedSheet(null);
    }
}
$(document).ready(() => {
    ko.applyBindings(new PoolAccountSplitViewModel());
})