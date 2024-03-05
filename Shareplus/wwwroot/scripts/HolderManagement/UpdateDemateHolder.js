var UpdateDemateHolder = function () {
    //status
    self.TotalRecords = ko.observable();
    self.InvalidRecords = ko.observable();
    self.NewRecords = ko.observable();
    self.ExistingRecords = ko.observable();
    self.SelectedFile = ko.observable();

    self.UploadFile = function () {
        fdata = new FormData();
        fileUpload = $("#fileupload").get(0);
        files = fileUpload.files;
        fdata.append('postedFile', files[0]);

        if (self.Validation()) {
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
        }
    }

    self.saveRecord = function () {
        if (self.Validation()) {
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
        }
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
        if (Validate.empty(self.SelectedFile())) {
            errMsg += "Please Select a File <br/> "
        }
        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
        }
    }
}

$(document).ready(function () {
    ko.applyBindings(new UpdateDemateHolder());

});

