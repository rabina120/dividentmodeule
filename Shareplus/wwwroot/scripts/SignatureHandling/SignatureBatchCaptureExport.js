function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}


var SignatureBatchImportExport = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()


    self.SignatureImportExport = ko.observable()
    self.StartShHolderNo = ko.observable()
    self.EndShHolderNo = ko.observable()
    //self.CaptureDate = ko.observable()
    self.SignatureArray = ko.observableArray([])
    self.ExportLocation = ko.observable()


    self.SignatureImportExport('I')

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

    //ClearControls
    self.ClearControls = function (data) {
        self.StartShHolderNo('')
        self.EndShHolderNo('')
        //self.CaptureDate('')
        self.SignatureArray([])
        self.ExportLocation('')
        if (data == 'A')
            self.SignatureImportExport('I')
    }
    //validation
    self.Validation = function (data) {
        var errMsg = "";
        if (self.SignatureImportExport() == "I") {
            if (self.SignatureArray().length == 0) {
                errMsg += "Please Select Images To Insert !!!";
            }
        } else {
            if (Validate.empty(self.StartShHolderNo())) {
                errMsg += "Please Enter Start ShHolderNo !!! <br/>";
            }
            if (Validate.empty(self.EndShHolderNo())) {
                errMsg += "Please Enter End ShHolderNo!!!";
            }
        }

        if (errMsg == "") {
            return true;
        }
        else {
            alert('warning', errMsg);
            return false;
        }
    }

    self.SignatureImportExport.subscribe(() => {
        if (self.SignatureImportExport() == "I") {
            $('#StartShHolderNo,#EndShHolderNo').attr('disabled', true)
            $('#signatureFile').attr('disabled', false)
            $('#UploadMessage').show()
            $('#Save').val("Save");
        }
        else {
            $('#StartShHolderNo,#EndShHolderNo').attr('disabled', false)
            $('#signatureFile').attr('disabled', true)
            $('#UploadMessage').hide()
            $('#Save').val("Export");

        }
    })


    //FOr uploading images
    $("#signatureFile").on("change", function () {
        if (this.files != undefined) {
            var Signatures = []
            for (i = 0; i < this.files.length; i++) {
                edocerror = "";
                docsize = "";
                filename = "";
                doctype = "";
                signatureString64 = "";
                docsize = this.files[i].size;
                doctype = this.files[i].type;
                filename = this.files[i].name;


                if ((doctype == "image/jpeg") || (doctype == "image/png") || (doctype == "image/bmp")) {

                }
                else {
                    edocerror = 1;
                    toastr.error("Some Files Are Not In JPEG/PNG/BMP Format !!!<br>");
                    return;
                }
                if (docsize / 1024 / 1024 > 1000000) {
                    edocerror = 2;
                    toastr.error("Some Document Size Exceeds 1 MB !!!<br>");
                    return;
                }
                var reader = new FileReader();
                reader.readAsDataURL(this.files[i]);

                reader.onloadend = function (event) {

                    var signatureData = {
                        "FileName": filename.substring(0, filename.lastIndexOf('.')) || filename,
                        "base64SignatureString": event.target.result,
                        "FileLength": docsize
                    }
                    Signatures.push(signatureData)
                }


            }
            self.SignatureArray(Signatures)
        }
    });



    self.Save = (data) => {
        if (self.ValidateCompany()) {
            if (self.Validation()) {
                if (self.SignatureImportExport() == "I") {
                    swal({
                        title: "You Are About To Upload Signatures ",
                        text: "Are You Sure All The Signatures Are Valid ?",
                        icon: "warning",
                        buttons: true,
                        dangerMode: true
                    }).then((willDelete) => {
                        if (willDelete) {

                            var companyCode = self.SelectedCompany();
                            Openloader()
                            $.ajax({
                                type: "POST",
                                url: "/SignatureHandling/SignatureBatchCapture/SaveBatchSignature",
                                data: {
                                    "CompCode": companyCode,
                                    "Signatures": self.SignatureArray()
                                },
                                dataType: "json", beforeSend: function (xhr) {
                                    xhr.setRequestHeader('XSRF-TOKEN',
                                        $('input:hidden[name="__RequestVerificationToken"]').val());
                                },
                                success: (result) => {
                                    if (result.isSuccess) {
                                        alert('success', result.message)
                                    } else {
                                        alert('error', result.message)


                                    }
                                },
                                error: (error) => {
                                    alert('error', error.message)
                                },
                                complete: () => {
                                    Closeloader()
                                    ClearControls()
                                }
                            });
                        }
                    });
                }
                else {
                    var companyCode = self.SelectedCompany();
                    Openloader()
                    $.ajax({
                        type: "POST",
                        url: "/SignatureHandling/SignatureBatchCapture/ExportBatchSignature",
                        data: {
                            "CompCode": companyCode,
                            "StartShHolderNo": self.StartShHolderNo(),
                            "EndShHolderNo": self.EndShHolderNo()
                        },
                        dataType: "json", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: (result) => {
                            if (result.isSuccess) {
                                var fileName = result.message;
                                var a = document.createElement("a");
                                a.href = "data:application/octet-stream;base64," + result.responseData;
                                a.download = fileName;
                                a.click();
                            } else {
                                alert('error', result.message)


                            }
                        },
                        error: (error) => {
                            alert('error', error.message)
                        },
                        complete: () => {
                            Closeloader()
                            ClearControls()
                        }
                    });

                }
            }
        }

    }

    self.Refresh = (data) => {
        self.ClearControls()
    }

}

$(document).ready(function () {
    ko.applyBindings(new SignatureBatchImportExport());
});