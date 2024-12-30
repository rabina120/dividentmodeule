function ExcelSheetName(data, index) {
    var self = this;
    if (data != undefined) {
        self.SheetName = ko.observable(data);
        self.SheetId = ko.observable(index);
    }
}

var UpdateDemateHolder = function () {
    //status
    self.TotalRecords = ko.observable();
    self.InvalidRecords = ko.observable();
    self.NewRecords = ko.observable();
    self.ExistingRecords = ko.observable();
    self.SelectedFile = ko.observable();
    self.SheetLists = ko.observableArray([]);
    self.SelectedSheet = ko.observable();
    self.CashDividendList = ko.observable([])
    var fileExtension = ""
    var filename = ""
    var extension = ""
    var fdata = ""
    var fdata = ""
    var fileUpload = ""
    var files = ""

    $('#fileupload1').on('change', function () {
        fileExtension = ['xls', 'xlsx'];
        filename = $('#fileupload1').val();
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
        fileUpload = $("#fileupload1").get(0);
        files = fileUpload.files;
        fdata.append(files[0].name, files[0]);
        Openloader()

        $.ajax({
            type: "POST",
            url: "/HolderManagement/UpdateDemateHolder/GetSheetNames",

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
        const fileExtension = ['xls', 'xlsx'];
        const fileInput = $('#fileupload1');
        const filename = fileInput.val();

        // Check if a file is selected
        if (!filename) {
            swal({
                title: "Error",
                text: "Please select a file.",
                icon: "error",
                buttons: true
            });
            return false;
        }

        // Validate file extension
        const extension = filename.split('.').pop().toLowerCase();
        if (!fileExtension.includes(extension)) {
            swal({
                title: "Error",
                text: "Please select only Excel files.",
                icon: "error",
                buttons: true
            });
            return false;
        }

        // Warn about potential XLSX compatibility issues
        if (extension === 'xlsx') {
            swal({
                title: "Info",
                text: "XLSX Format may not be Supported by the Server!",
                icon: "info",
                buttons: true
            });
        }

        // Prepare FormData for file upload
        const fdata = new FormData();
        const files = fileInput.get(0).files;
        if (!files || files.length === 0) {
            swal({
                title: "Error",
                text: "No file found in the input.",
                icon: "error",
                buttons: true
            });
            return false;
        }
        fdata.append('postedFile', files[0]);

        // Confirmation dialog before proceeding
        swal({
            title: "Are you sure?",
            text: "You want to upload the Excel file.",
            icon: "warning",
            buttons: true,
            dangerMode: true
        }).then((willSave) => {
            if (willSave) {
                Openloader(); // Show loader during processing
                $.ajax({
                    type: "POST",
                    url: "/HolderManagement/UpdateDemateHolder/UploadSheet?SheetId=" + ko.toJS(self.SelectedSheet().SheetId),
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    data: fdata,
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        if (response.isSuccess) {
                            self.TotalRecords(response.responseData[0].TotalRecords);
                            self.ExistingRecords(response.responseData[0].ExistingHolder);
                            self.NewRecords(response.responseData[0].TotalRecords - response.responseData[0].ExistingHolder);
                            self.InvalidRecords(response.responseData[0].InvalidRecords);
                            if (response.responseData[0].InvalidRecords > 0) {
                                $("#dwnldListDiv").show();
                            }
                        } else {
                            // Show error message
                            swal({
                                title: "Error",
                                text: response.message,
                                icon: "error",
                                buttons: true
                            });
                        }
                    },
                    error: function (xhr, status, error) {
                        // Show error message on failure
                        swal({
                            title: "Error",
                            text: error || "An error occurred during the upload process.",
                            icon: "error",
                            buttons: true
                        });
                    },
                    complete: function () {
                        Closeloader(); // Hide loader when processing completes
                    }
                });
            }
        });
    };

    //self.UploadExcel = function () {
    //    const fileExtension = ['xls', 'xlsx'];
    //    const filename = $('#fileupload1').val();

    //    // Check if a file is selected
    //    if (filename.length == 0) {
    //        swal({
    //            title: "Error",
    //            text: "Please select a file.",
    //            icon: "error",
    //            buttons: true
    //        });
    //        return false;
    //    }

    //    // Validate file extension
    //    const extension = filename.replace(/^.*\./, '');
    //    if ($.inArray(extension, fileExtension) == -1) {
    //        swal({
    //            title: "Error",
    //            text: "Please select only Excel files.",
    //            icon: "error",
    //            buttons: true
    //        });
    //        return false;
    //    }

    //    // Warn about potential XLSX compatibility issues
    //    if (extension === 'xlsx') {
    //        swal({
    //            title: "Info",
    //            text: "XLSX Format may not be Supported by the Server!!",
    //            icon: "info",
    //            buttons: true
    //        });
    //    }

    //    // Prepare FormData for file upload
    //    const fdata = new FormData();
    //    const fileUpload = $("#fileupload1").get(0);
    //    const files = fileUpload.files;
    //    fdata.append('postedFile', files[0]);

    //    // Confirmation dialog before proceeding
    //    swal({
    //        title: "Are you sure?",
    //        text: "You want to Upload the Excel File",
    //        icon: "warning",
    //        buttons: true,
    //        dangerMode: true
    //    }).then((willSave) => {
    //        if (willSave) {
    //            Openloader(); // Show loader during processing
    //            $.ajax({
    //                type: "POST",
    //                //url: "/HolderManagement/UpdateDemateHolder/UploadFile",
    //                url: "/HolderManagement/UpdateDemateHolder/UploadSheet?SheetId=" + ko.toJS(self.SelectedSheet().SheetId),
    //                beforeSend: function (xhr) {
    //                    xhr.setRequestHeader("XSRF-TOKEN",
    //                        $('input:hidden[name="__RequestVerificationToken"]').val());
    //                },
    //                data: fdata,

    //                success: function (response) {
    //                    if (response.isSuccess) {
    //                        self.TotalRecords(response.responseData[0].TotalRecords);
    //                        self.ExistingRecords(response.responseData[0].ExistingHolder);
    //                        self.NewRecords(response.responseData[0].TotalRecords - response.responseData[0].ExistingHolder)
    //                        self.InvalidRecords(response.responseData[0].InvalidRecords)
    //                        if (response.responseData[0].InvalidRecords > 0) {
    //                            $("#dwnldListDiv").show();
    //                        }
    //                    }
    //                    else {
    //                        // Show error message
    //                        swal({
    //                            title: "Error",
    //                            text: response.message,
    //                            icon: "error",
    //                            buttons: true
    //                        });

    //                    }
    //                },
    //                error: function (error) {
    //                    // Show error message on failure
    //                    swal({
    //                        title: "Error",
    //                        text: error.message,
    //                        icon: "error",
    //                        buttons: true
    //                    });

    //                },
    //                complete: function () {
    //                    Closeloader(); // Hide loader when processing completes
    //                }
    //            });
    //        }
    //    });
    //};


    //self.UploadExcel = function () {
    //    const fileExtension = ['xls', 'xlsx'];
    //    const filename = $('#fileupload1').val();

    //    // Check if a file is selected
    //    if (filename.length == 0) {
    //        swal({
    //            title: "Error",
    //            text: "Please select a file.",
    //            icon: "error",
    //            buttons: true
    //        });
    //        return false;
    //    }

    //    // Validate file extension
    //    const extension = filename.replace(/^.*\./, '');
    //    if ($.inArray(extension, fileExtension) == -1) {
    //        swal({
    //            title: "Error",
    //            text: "Please select only Excel files.",
    //            icon: "error",
    //            buttons: true
    //        });
    //        return false;
    //    }

    //    // Warn about potential XLSX compatibility issues
    //    if (extension === 'xlsx') {
    //        swal({
    //            title: "Info",
    //            text: "XLSX Format may not be Supported by the Server!!",
    //            icon: "info",
    //            buttons: true
    //        });
    //    }

    //    // Prepare FormData for file upload
    //    const fdata = new FormData();
    //    const fileUpload = $("#fileupload1").get(0);
    //    const files = fileUpload.files;
    //    fdata.append('postedFile', files[0]);

    //    // Confirmation dialog before proceeding
    //    swal({
    //        title: "Are you sure?",
    //        text: "You want to Upload the Excel File",
    //        icon: "warning",
    //        buttons: true,
    //        dangerMode: true
    //    }).then((willSave) => {
    //        if (willSave) {
    //            Openloader(); // Show loader during processing
    //            $.ajax({
    //                type: "POST",
    //                url: "/HolderManagement/UpdateDemateHolder/UploadSheet?SheetId=" + ko.toJS(self.SelectedSheet().SheetId),
    //                beforeSend: function (xhr) {
    //                    xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
    //                },
    //                data: fdata,
    //                contentType: false,
    //                processData: false,
    //                success: function (response) {
    //                    if (response.isSuccess) {
    //                        self.TotalRecords(response.responseData[0].TotalRecords);
    //                        self.ExistingRecords(response.responseData[0].ExistingHolder);
    //                        self.NewRecords(response.responseData[0].TotalRecords - response.responseData[0].ExistingHolder)
    //                        self.InvalidRecords(response.responseData[0].InvalidRecords)
    //                        if (response.responseData[0].InvalidRecords > 0) {
    //                            $("#dwnldListDiv").show();
    //                        }
    //                    }
    //                    else {
    //                        alert('error', 'Upload Failed. Please Check Your File..!')
    //                    }
    //                },
    //                error: function (error) {
    //                    alert('error', error.message)
    //                },
    //                complete: () => {
    //                    Closeloader()
    //                }
    //            });
                 
    //        }
    //    });
    //};
    self.UploadFile = function () {
        fdata = new FormData();
        fileUpload = $("#fileupload").get(0);
        files = fileUpload.files;
        fdata.append('postedFile', files[0]);

      /*  if (self.Validation()) {*/
            Openloader()
            $.ajax({
                type: "POST",
                url: "/HolderManagement/UpdateDemateHolder/UploadFile",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                data: fdata,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.isSuccess) {
                        self.TotalRecords(response.responseData[0].TotalRecords);
                        self.ExistingRecords(response.responseData[0].ExistingHolder);
                        self.NewRecords(response.responseData[0].TotalRecords - response.responseData[0].ExistingHolder)
                        self.InvalidRecords(response.responseData[0].InvalidRecords)
                        if (response.responseData[0].InvalidRecords > 0) {
                            $("#dwnldListDiv").show();
                        }
                    }
                    else {
                        alert('error', 'Upload Failed. Please Check Your File..!')
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: () => {
                    Closeloader()
                }
            });
  /*      }*/
    }

    self.saveRecord = function () {
        /*if (self.Validation()) {*/
            Openloader()
            $.ajax({
                type: "POST",
                url: "/HolderManagement/UpdateDemateHolder/SaveRecord",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.isSuccess) {
                        alert('success', 'File Uploded Succesfully')
                    }
                    else {
                        alert('error', 'Upload Failed. Please Check Your File..!')
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: () => {
                    Closeloader();
                    self.clearForm();
                }
            });
       /* }*/
    }

    self.DownldInvalidRecords = function () {
        alert("success", "Development Pending");
    }

    self.clearForm = function () {
        self.SelectedFile("");
        self.TotalRecords("");
        self.ExistingRecords("");
        self.NewRecords("");
        self.InvalidRecords("");
    }

    self.Validation = function () {
        var errMsg = "";
        //if (Validate.empty(self.SelectedFile())) {
        //    errMsg += "Please Select a File <br/> "
        //}
        //if (errMsg == "") {
        //    return true
        //} else {
        //    alert('error', errMsg)
        //    return false
        //}
        return true;
    }
}

$(document).ready(function () {
    ko.applyBindings(new UpdateDemateHolder());

});

