function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

var SignatureIndividualCapture = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()

    self.ShHolderNo = ko.observable();
    self.TotalKitta = ko.observable();
    self.ShHolderAddress = ko.observable();
    self.ShHolderName = ko.observable();
    self.ScannedBy = ko.observable();
    self.Signature = ko.observable();
    self.signature = ko.observable();
    self.FileLength = ko.observable();
    self.captureby = ko.observable();
    self.capturedate = ko.observable();
    self.Is_Approved = ko.observable();
    self.approvedby = ko.observable();
    self.ApprovedDate = ko.observable();
    self.action1 = ko.observable();
    self.entrydate = ko.observable();
    self.entryuser = ko.observable();
    self.PassProcted = ko.observable();
    self.FileNameSign = ko.observable();


    var optionAUD;


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
    self.ClearControls = function () {
        $('#AddButton,#UpdateButton,#DeleteButton').attr('disabled',false)
        $('#ShHolderNo,#signatureFile').attr('disabled', true)
        self.ShHolderNo('');
        self.TotalKitta('');
        self.ShHolderAddress('');
        self.ShHolderName('');
        self.ScannedBy('');
        self.Signature('');
        self.signature('');
        self.FileLength('');
        self.captureby('');
        self.capturedate('');
        self.Is_Approved('');
        self.approvedby('');
        self.ApprovedDate('');
        self.action1('');
        self.entrydate('');
        self.entryuser('');
        self.PassProcted('');
        self.FileNameSign('');
    }
    
    //for choosing add or update or delete
    self.chooseOptions = function (data) {
        if (self.ValidateCompany()) {

            optionAUD = data;
            $('#ShHolderNo').attr('disabled',false)
            $('#AddButton,#UpdateButton,#DeleteButton').attr('disabled',true)
            
            if (data == "A") {
                $('#Save').val("Save");
                $('#Save').removeClass(["btn-danger", "btn-warning"]);
                $('#Save').addClass("btn-success");

            }
            else {
                if (data == "U") {
                    $('#Save').val("Replace");
                    $('#Save').removeClass(["btn-danger", "btn-success"]);
                    $('#Save').addClass("btn-warning");
                }
                else {
                    $('#Save').val("Delete");
                    $('#Save').removeClass(["btn-success", "btn-warning"]);
                    $('#Save').addClass("btn-danger");
                }

            }
            $('#ShHolderNo').focus();
        }
    }

    self.GetShHolderInformation = (data) => {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.ShHolderNo())) {
                self.TotalKitta('');
                self.ShHolderName('');
                self.ShHolderAddress('');
                self.ScannedBy('');
                self.Signature('');
                self.FileLength('');

                var companyCode = self.SelectedCompany();
                Openloader()
                $.ajax({
                    type: "post",
                    url: "/SignatureHandling/SignatureIndividualCapture/GetShHolderInformation",
                    data: {
                        "CompCode": companyCode,
                        "ShHolderNo": self.ShHolderNo(),
                        "SelectedAction": optionAUD
                    },
                    dataType: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: (result) => {
                        if (result.isSuccess) {

                            if (optionAUD == "A") {
                                if (result.responseData.signature != null) {
                                    alert('warning', 'Signature Already Added !!!');
                                    $('#signatureFile').attr("disabled", true)
                                    self.Signature("data:Image/jpeg;base64," + result.responseData.signature);
                                    self.ScannedBy(result.responseData.scanedBy)
                                } else {
                                    $('#signatureFile').attr("disabled", false)
                                    self.Signature('')
                                    self.ScannedBy(result.responseData.userName)
                                }
                            }
                            else if (optionAUD == "U") {
                                if (result.responseData.signature != null) {
                                    self.Signature("data:Image/"+result.responseData.imageType+";base64," + result.responseData.signature);     
                                }
                                self.ScannedBy(result.responseData.userName)
                            }
                            else {
                                if (result.responseData.signature != null) {
                                    self.Signature("data:Image/" + result.responseData.imageType +";base64," + result.responseData.signature);
                                    self.ScannedBy(result.responseData.scanedBy)
                                }
                            }
                            if (result.responseData.signature != null ) {
                                if (optionAUD == "A") {
                                    $('#signatureFile').attr("disabled", true)
                                } else if (optionAUD == "U") {
                                    $('#signatureFile').attr("disabled", false)
                                }
                                else {
                                    $('#signatureFile').attr("disabled", true)
                                }
                                self.Signature("data:Image/" + result.responseData.imageType +";base64," + result.responseData.signature);
                            }
                            else {
                                self.ScannedBy()
                                $('#signatureFile').attr("disabled", false)
                                self.Signature()
                            }
                            if (optionAUD == "D")
                                self.ScannedBy(result.responseData.scanedBy);
                            else
                                self.ScannedBy(result.responseData.userName);

                            self.TotalKitta(result.responseData.totalKitta);
                            self.ShHolderName(result.responseData.name);
                            self.ShHolderAddress(result.responseData.address);
                        } else {
                            alert('error', result.message)
                            $('#signatureFile').attr("disabled", true)
                            self.TotalKitta('');
                            self.ShHolderName('');
                            self.ShHolderAddress('');
                            self.ScannedBy('');
                            self.Signature('');
                            self.FileLength('');
                            $('#ShHolderNo').focus()

                        }
                    },
                    error: (error) => {
                        alert('error', error.message)
                        $('#signatureFile').attr("disabled", true)
                        self.TotalKitta('');
                        self.ShHolderName('');
                        self.ShHolderAddress('');
                        self.ScannedBy('');
                        self.Signature('');
                        self.FileLength('');
                        $('#ShHolderNo').focus()

                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }
    self.Save = (data) => {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.ShHolderNo())) {
                if (!Validate.empty(self.signature()) || optionAUD == "D") {
                    if (!Validate.empty(self.ScannedBy()) || optionAUD == "D") {
                        var text, title, icon;
                        if (optionAUD == "A") {
                            text = "Do You Want To Add New Signature ?";
                            title = "Add New Signature ?";
                            icon = "success"
                        } else if (optionAUD == "U") {
                            text = "Do You Want To Update The Signature ?";
                            title = "Update Signature ?";
                            icon = "warning"
                        }
                        else {
                            text = "Do You Want To Delete Signature ?";
                            title = "Delete Signature ?";
                            icon = "error"
                        }
                        swal({
                            title: title,
                            text: text,
                            icon: icon,
                            buttons: true,
                            dangerMode: true
                        }).then((willDelete) => {
                            if (willDelete) {

                                var companyCode = self.SelectedCompany();
                                Openloader()
                                $.ajax({
                                    type: "POST",
                                    url: "/SignatureHandling/SignatureIndividualCapture/SaveSignatureInformation",
                                    data: {
                                        "CompCode": companyCode,
                                        "ShHolderNo": self.ShHolderNo(),
                                        "ScannedBy": self.ScannedBy(),
                                        "SelectedAction": optionAUD,
                                        "Signature": self.signature(),
                                        "FileLength": self.FileLength()
                                    },
                                    dataType: "json", beforeSend: function (xhr) {
                                        xhr.setRequestHeader("XSRF-TOKEN",
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
                                })

                            }
                        });
                    }
                    else {
                        alert('warning', 'Enter Scanned By !!!');

                    }
                }
                else {
                    alert('warning', 'Select an Image First !!!');
                }
            } else {
                alert('warning', 'Enter a ShHolderNo First !!!');
            }
        }

    }
    


    //getting signature
    //for loading file in the image box upon upload
    var loadFile = function (obj) {
        self.Signature(URL.createObjectURL(obj));
    };

    //FOr uploading images

    $("#signatureFile").on("change", function () {
        if (this.files[0] != undefined) {
            edocerror = "";
            docsize = "";
            filename = "";
            docsize = this.files[0].size;

            var fileInput = document.getElementById('signatureFile');
            docname = fileInput.files[0].name;

            var files = !!this.files ? this.files : [];
            if (!files.length || !window.FileReader) return;

            var reader = new FileReader();
            reader.readAsDataURL(files[0]);


            if ((files[0].type == "image/jpeg") || (files[0].type == "image/png") || (files[0].type == "image/bmp")) {

            }
            else {

                edocerror = 1;
                toastr.error("Document format must only be jpeg or png or bmpmj'h!!!<br>");
                return;
            }
            if (files[0].size / 1024 / 1024 > 1000000) {
                edocerror = 2;
                toastr.error("Document size must be less than 1 mb !!!<br>");
                return;
            }
            filename = fileInput.files[0].name;

            loadFile(this.files[0]);

            reader.onloadend = function (event) {

                var dataUri = event.target.result
                docname = dataUri;
                self.signature(docname);
                self.FileLength(docsize);
                self.FileNameSign(filename);
            }
        } else {
            self.signature('');
        }
    });

    self.Refresh = (data) => {
    self.ClearControls()}
}

$(document).ready(function () {
    ko.applyBindings(new SignatureIndividualCapture());
});