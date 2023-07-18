function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function ShownerType(data) {
    var self = this;
    if (data != undefined) {
        self.ShOwnerTypeValue = ko.observable(data.shOwnerTypeCert);
        self.ShOwnerTypeName = ko.observable(data.shOwnerTypeName);
        self.ShOwnerTypeAndName = self.ShOwnerTypeValue() + " " + self.ShOwnerTypeName();
    }
}

function CertificateType(data) {
    var self = this;
    if (data != undefined) {
        self.CertificateTypeValue = ko.observable(data.certStatusId);
        self.CertificateTypeName = ko.observable(data.certStatus);
        self.CertificateTypeNameAndValue = self.CertificateTypeValue() + " " + self.CertificateTypeName();

    }
}
function ShareType(data) {
    var self = this;
    if (data != undefined) {
        self.ShareTypeValue = ko.observable(data.shareTypeID);
        self.ShareTypeName = ko.observable(data.shareType);
        self.ShareTypeNameAndValue = self.ShareTypeValue() + " " + self.ShareTypeName();

    }
}


function OccupationL(data) {
    var self = this;
    self.shownertype = ko.observable(data.shownertype);
    self.OccupationId = ko.observable(data.occupationId);
    self.OccupationN = ko.observable(data.occupationN);
}

function Certificate(data) {
    var self = this;
    if (data != undefined) {

        self.Compcode = ko.observable(data.compcode)
        self.ShHolderNo = ko.observable(data.shholderno)
        self.CertNo = ko.observable(data.certno)
        self.SrNoFrom = ko.observable(data.srnofrom)
        self.SrNoTo = ko.observable(data.srnoto)
        self.CertStatus = ko.observable(data.certstatus)
        self.CertStatusName = ko.observable(data.certstatusname)
        self.TimesOfSplit = ko.observable(data.timesofsplit)
        self.ShKitta = ko.observable(data.shkitta)
        self.ShOwnerType = ko.observable(data.shownertype)
        self.ShOwnerTypeName = ko.observable(data.shownertypename)
        self.ShareType = ko.observable(data.sharetype)
        self.ShareTypeName = ko.observable(data.sharetypename)
        self.IssueDate = ko.observable(data.issuedate == null ? '---' : data.issuedate.substring(0, 10))
        self.DistCert = ko.observable(data.distcert)
        self.TranDt = ko.observable(data.trandt == null ? '---' : data.trandt.substring(0, 10))
        self.DupliNo = ko.observable(data.duplino)
        self.PaidAmount = ko.observable(data.paidamount)
        self.TotalAmount = ko.observable(data.totalamount)
        self.UserName = ko.observable(data.username)
        self.EntryDate = ko.observable(data.entrydate == null ? '---' : data.entrydate.substring(0, 10))

    }
}


var CertificateEntry = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()

    self.ShholderNo = ko.observable()
    self.Name = ko.observable()
    self.FaName = ko.observable()
    self.GrFaName = ko.observable()
    self.Address = ko.observable()
    self.TelNo = ko.observable()
    self.Occupation = ko.observable()
    self.Minor = ko.observable()
    self.TotalKitta = ko.observable()

    self.CertificateNo = ko.observable()
    self.SelectedShareType = ko.observable()
    self.ShareKitta = ko.observable()
    self.CertificateIssuedDate = ko.observable(formatDate(new Date()))
    self.SelectedCertificateType = ko.observable()
    self.StartSerialNo = ko.observable()
    self.EndSerialNo = ko.observable()
    self.SelectedShOwnerType = ko.observable()

    self.ShHolderNoFrom = ko.observable()
    self.ShHolderNoTo = ko.observable()
    self.CertificateNoFrom = ko.observable()
    self.CertificateNoTo = ko.observable()
    self.SerialNoFrom = ko.observable()
    self.SerialNoTo = ko.observable()
    self.ShareKittaFrom = ko.observable()
    self.ShareKittaTo = ko.observable()

    self.CertificateTypeList = ko.observableArray([])
    self.ShOwnerTypeList = ko.observableArray([])
    self.CertificateSearchList = ko.observableArray([])
    self.ShareTypeList = ko.observableArray([])


    //gloabal variables
    var optionAUD = "";
    var MaxSrNoFrom, MaxCertNo = 0;

    //Validation 
    self.SaveValidation = function () {

        var errMsg = "";

        if (Validate.empty(self.ShholderNo())) {
            errMsg += "Please Select A Holder No. <br/>";
        }
        if (Validate.empty(self.CertificateNo())) {
            errMsg += "Please Enter A Certificate No. <br/>";
        }
        if (Validate.empty(self.SelectedShareType())) {
            errMsg += "Please Select A Share Type. <br/>";
        }
        if (Validate.empty(self.ShareKitta())) {
            errMsg += "Please Enter A ShKitta. <br/>";
        }
        if (self.ShareKitta() == "0") {
            errMsg += "ShKitta Cannot Be 0. <br/>";
        }
        if (Validate.empty(self.CertificateIssuedDate())) {
            errMsg += "Please Enter A Issued Date. <br/>";
        }
        if (Validate.empty(self.SelectedCertificateType())) {
            errMsg += "Please Select A Certificate Type. <br/>";
        }
        if (Validate.empty(self.StartSerialNo())) {
            errMsg += "Please Enter A Start Serial No. <br/>";
        }
        if (Validate.empty(self.EndSerialNo())) {
            errMsg += "Please Select A End Serial No. <br/>";
        }
        if (Validate.empty(self.SelectedShOwnerType())) {
            errMsg += "Please Select A Share Owner Type. <br/>";
        }
        if (!Validate.empty(self.ShareKitta()) && !Validate.empty(self.StartSerialNo()) && !Validate.empty(self.EndSerialNo())) {
            if (parseInt(self.ShareKitta()) != parseInt(self.EndSerialNo()) - parseInt(self.StartSerialNo()) + 1) {
                errMsg += "Either Shkitta or Start Serial No or End Serial No Not Corrent. <br/>";
            }
        }

        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        } else {
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

    //LOAD Sharetypes STATUS
    self.LoadShareTypes = function () {
        if (self.ValidateCompany()) {
            $.ajax({
                type: "post",
                url: '/Certificate/CertificateEntry/LoadShareTypes',
           
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new ShareType(item)
                        });
                        self.ShareTypeList(mappedTasks);

                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })

        }
    }
    self.LoadShareTypes();
    //LOAD LoadShOwnerTypes STATUS
    self.LoadShOwnerTypes = function () {
        if (self.ValidateCompany()) {
            $.ajax({
                type: "post",
                url: '/Certificate/CertificateEntry/LoadShOwnerTypes',
         
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new ShownerType(item)
                        });
                        self.ShOwnerTypeList(mappedTasks);

                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })

        }
    }
    self.LoadShOwnerTypes();

    //LOAD CERT STATUS
    self.LoadCertStatus = function () {
        if (self.ValidateCompany()) {
            $.ajax({
                type: "post",
                url: '/Certificate/CertificateEntry/LoadCertStatuses',
               
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new CertificateType(item)
                        });
                        self.CertificateTypeList(mappedTasks);

                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })

        }
    }
    self.LoadCertStatus();
    //for choosing add or update or delete
    self.chooseOptions = function (data) {
        optionAUD = data;
        self.EnableDisableAEDButton(true)
        if (optionAUD === "A") {
            $("#ShHolderNo,#ShareKitta").attr('disabled', false)
            $("#SearchButton").attr('disabled', true)
            $("#SaveButton").text('Save')
        } else {
            $("#ShHolderNo,#ShareKitta").attr('disabled', true)
            $("#SearchButton").attr('disabled', false)
            $("#ShHolderNo").focus();
            if (optionAUD == "U") {
                $("#SaveButton").text('Update')
            }
            else
                $("#SaveButton").text('Delete')
        }
    }

    //SEARCH ON FIELD ON LOST FOCUS
    self.GetSholderInformation = function (data) {
        if (optionAUD == "A") {
            var shholderNo = self.ShholderNo()
            if (shholderNo != undefined && shholderNo != "") {
                Openloader()
                $.ajax({
                    type: "POST",
                    url: '/Certificate/CertificateEntry/GetShHolderInformation',
                    data: { 'ShHolderNo': ko.toJS(shholderNo), 'CompCode': ko.toJS(self.SelectedCompany()), 'SelectedAction': optionAUD },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    datatype: "json",
                    success: function (result) {
                        if (result.isSuccess) {
                            self.Name(result.responseData.fName + ' ' + result.responseData.lName)
                            self.FaName(result.responseData.faName)
                            self.GrFaName(result.responseData.grFaName)
                            self.Address(result.responseData.address)
                            self.TelNo(result.responseData.telNo)
                            self.Occupation(result.responseData.occupationId + ": " + result.responseData.occupationN)
                            self.Minor(result.responseData.minor == true ? "Yes" : "No")
                            self.TotalKitta(result.responseData.totalKitta)
                            self.EnableCertificateInputs();
                            if (optionAUD == "A") {
                                MaxSrNoFrom = result.responseData.maxSrNoFrom;
                                MaxCertNo = result.responseData.maxCertNo;
                                self.StartSerialNo(MaxSrNoFrom)
                                self.EndSerialNo(MaxSrNoFrom + self.ShareKitta())
                                self.CertificateNo(MaxCertNo)
                            } else {
                                self.CertificateNo(data.CertNo())
                                self.StartSerialNo(data.SrNoFrom())
                                self.EndSerialNo(data.SrNoTo())
                                self.ShareKitta(data.ShKitta())
                                self.CertificateIssuedDate(data.IssueDate() == "---" ? '' : data.IssueDate())
                                self.SelectedShareType(data.ShareType())
                                self.SelectedShOwnerType(data.ShOwnerType())
                                self.SelectedCertificateType(data.CertStatus())
                            }
                        } else {
                            alert('warning', result.message)
                            //self.refresh()
                        }
                    },
                    error: function (error) {
                    }, complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }

    self.EnableCertificateInputs = () => {
        $('#shareType,#ShareKitta,#CertificateIssuedDate,#CertificateStatusSelect,#ShOwnerType,#StartSerialNo').attr('disabled', false)
        if (optionAUD == "A") {
            $('#CertificateNo').attr('disabled',false)
        } else{
            $('#CertificateNo,#ShareKitta').attr('disabled', true)
            if (optionAUD == "D")
                $('#shareType,#ShareKitta,#CertificateIssuedDate,#CertificateStatusSelect,#ShOwnerType,#StartSerialNo').attr('disabled', true)
        }
    }
    //search certificate validation
    self.CertificateValidation = function () {
        var errMsg = "";
        if (Validate.empty(self.ShHolderNoFrom()) && Validate.empty(self.ShHolderNoTo()) && Validate.empty(self.CertificateNoFrom())
            && Validate.empty(self.CertificateNoTo()) && Validate.empty(self.SerialNoFrom()) && Validate.empty(self.SerialNoTo()) &&
            Validate.empty(self.ShareKittaFrom()) && Validate.empty(self.ShareKittaTo())) {
            errMsg = 'Please Insert At Least One Field.<br/>';
        } else {
            if (!Validate.empty(self.ShHolderNoTo())) {
                if (Validate.empty(self.ShHolderNoFrom())) {
                    errMsg += 'ShHolderNoFrom Is Required If ShHolderNoTo Is Inserted.<br/>';
                }
            }
            if (!Validate.empty(self.ShHolderNoFrom())) {
                if (Validate.empty(self.ShHolderNoTo())) {
                    errMsg += 'ShHolderNoTo Is Required If ShHolderNoFrom Is Inserted.<br/>';
                }
            }
            if (!Validate.empty(self.CertificateNoTo())) {
                if (Validate.empty(self.CertificateNoFrom())) {
                    errMsg += 'CertificateNoFrom Is Required If CertificateNoTo Is Inserted.<br/>';
                }
            }
            if (!Validate.empty(self.CertificateNoFrom())) {
                if (Validate.empty(self.CertificateNoTo())) {
                    errMsg += 'CertificateNoTo Is Required If CertificateNoFrom Is Inserted.<br/>';
                }
            }
            if (!Validate.empty(self.SerialNoTo())) {
                if (Validate.empty(self.SerialNoFrom())) {
                    errMsg += 'SerialNoFrom Is Required If SerialNoTo Is Inserted.<br/>';
                }
            }
            if (!Validate.empty(self.SerialNoFrom())) {
                if (Validate.empty(self.SerialNoTo())) {
                    errMsg += 'SerialNoTo Is Required If SerialNoFrom Is Inserted.<br/>';
                }
            }
            if (!Validate.empty(self.SerialNoTo())) {
                if (Validate.empty(self.ShareKittaFrom())) {
                    errMsg += 'SerialNoTo Is Required If ShHolderNoTo Is Inserted.<br/>';
                }
            }
            if (!Validate.empty(self.ShareKittaFrom())) {
                if (Validate.empty(self.ShareKittaTo())) {
                    errMsg += 'ShareKittaTo Is Required If ShareKittaFrom Is Inserted.<br/>';
                }
            }

        }
        if (errMsg != "") {
            alert('warning', errMsg);
            return false;
        } else {
            return true;
        }

    }
    //search SearchCertificate from modal
    self.SearchCertificate = function () {
        if (self.ValidateCompany()) {
            if (self.CertificateValidation()) {
                $('#CertDetTable').DataTable().clear()
                $('#CertDetTable').DataTable().destroy();
                Openloader()
                $.ajax({
                    type: "POST",
                    url: '/Certificate/CertificateEntry/SearchCertificate',
                    data: {
                       
                        'ShHolderNoFrom': self.ShHolderNoFrom(), 'ShHolderNoTo': self.ShHolderNoTo(), 'CertificateNoFrom': self.CertificateNoFrom(),
                        'CertificateNoTo': self.CertificateNoTo(), 'SerialNoFrom': self.SerialNoFrom(), 'SerialNoTo': self.SerialNoTo(),
                        'ShareKittaFrom': self.ShareKittaFrom(), 'ShareKittaTo': self.ShareKittaTo(), 'CompCode': self.SelectedCompany()
                    },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    datatype: "json",
                    success: function (result) {

                        if (result.isSuccess) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new Certificate(item)
                            });
                            self.CertificateSearchList(mappedTasks);
                            self.ClearControlSearch()
                            $('#searchQueryModalTheme').modal('hide')
                            $('#CertificateSearchModal').modal('show')
                            $('#CertDetTable').DataTable()
                        } else {
                            alert('warning', result.message)
                        }
                    },
                    error: function (error) {
                    }, complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }

    //fill certificate information based on data given
    self.FillCertificateInformation = function (data) {
        if (data != null) {
            $('#CertDetTable').DataTable().clear()
            $('#CertDetTable').DataTable().destroy();
            $('#CertificateSearchModal').modal('hide')
            Openloader()
            $.ajax({
                type: "POST",
                url: '/Certificate/CertificateEntry/GetShHolderInformation',
                data: { 'ShHolderNo': data.ShHolderNo(), 'CompCode': ko.toJS(self.SelectedCompany()), 'SelectedAction': optionAUD },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        self.Name(result.responseData.fName + ' ' + result.responseData.lName)
                        self.FaName(result.responseData.faName)
                        self.GrFaName(result.responseData.grFaName)
                        self.Address(result.responseData.address)
                        self.TelNo(result.responseData.telNo)
                        self.Occupation(result.responseData.occupationId + ": " + result.responseData.occupationN)
                        self.Minor(result.responseData.minor == true ? "Yes" : "No")
                        self.TotalKitta(result.responseData.totalKitta)
                        self.EnableCertificateInputs();
                        self.ShholderNo(data.ShHolderNo())
                        self.CertificateNo(data.CertNo())
                        self.StartSerialNo(data.SrNoFrom())
                        self.EndSerialNo(data.SrNoTo())
                        self.ShareKitta(data.ShKitta())
                        self.CertificateIssuedDate(data.IssueDate() == "---" ? '' : data.IssueDate())
                        self.SelectedShareType(data.ShareType())
                        self.SelectedShOwnerType(data.ShOwnerType())
                        self.SelectedCertificateType(data.CertStatus())

                    } else {
                        alert('warning', result.message)
                        //self.refresh()
                    }
                },
                error: function (error) {
                }, complete: () => {
                    Closeloader()
                }
            })
        }
    }

    //save
    self.Save = function () {
        if (self.ValidateCompany()) {
            if (self.SaveValidation()) {
               
                if (optionAUD == "A") {
                    Openloader()
                    $.ajax({
                        type: "POST",
                        url: '/Certificate/CertificateEntry/SaveCertificate',
                        data: {
                           
                            'ShHolderNo': self.ShholderNo(), 'CompCode': self.SelectedCompany(), 'SelectedAction': optionAUD,
                            'CertificateNo': self.CertificateNo(), 'ShareType': self.SelectedShareType(), 'ShareKitta': self.ShareKitta(),
                            'CertificateIssuedDate': self.CertificateIssuedDate(), 'CertificateType': self.SelectedCertificateType(),
                            'StartSerialNo': self.StartSerialNo(), 'EndSerialNo': self.EndSerialNo, 'ShOwnerType': self.SelectedShOwnerType()
                        },
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        datatype: "json",
                        success: function (result) {

                            if (result.isSuccess) {
                                alert('success',result.message)

                            } else {
                                alert('warning', result.message)
                            }
                        },
                        error: function (error) {
                        }, complete: () => {
                            ClearControl()
                            Closeloader()
                        }
                    })
                } else {
                    swal({
                        title: "Are you sure?",
                        text: optionAUD == "U" ? "The Certificate will be Updated" : "The Cerficate will be Deleted",
                        icon: "warning",
                        buttons: true,
                        dangerMode: true
                    }).then((willSave) => {
                        if (willSave) {
                            Openloader()
                            $.ajax({
                                type: "POST",
                                url: '/Certificate/CertificateEntry/SaveCertificate',
                                data: {
                                 
                                    'ShHolderNo': self.ShholderNo(), 'CompCode': self.SelectedCompany(), 'SelectedAction': optionAUD,
                                    'CertificateNo': self.CertificateNo(), 'ShareType': self.SelectedShareType(), 'ShareKitta': self.ShareKitta(),
                                    'CertificateIssuedDate': self.CertificateIssuedDate(), 'CertificateType': self.SelectedCertificateType(),
                                    'StartSerialNo': self.StartSerialNo(), 'EndSerialNo': self.EndSerialNo, 'ShOwnerType': self.SelectedShOwnerType()
                                },
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader("XSRF-TOKEN",
                                        $('input:hidden[name="__RequestVerificationToken"]').val());
                                },
                                datatype: "json",
                                success: function (result) {

                                    if (result.isSuccess) {

                                        alert('success', result.message)
                                    } else {
                                        alert('warning', result.message)
                                    }
                                },
                                error: function (error) {
                                }, complete: () => {
                                    ClearControl()
                                    Closeloader()
                                }
                            })
                        }
                    });
                }
                
            }
        }
      
    }
    //calculate end serial no
    self.ShareKitta.subscribe(function () {
        if (optionAUD == "A") {
            if (!Validate.empty(self.StartSerialNo())) {
                self.EndSerialNo(parseInt(self.StartSerialNo()) + parseInt(self.ShareKitta()) - 1)
            }

        }
        
    })

    self.StartSerialNo.subscribe(function () {
        if (!Validate.empty(self.StartSerialNo()) && !Validate.empty(self.ShareKitta())) {
            self.EndSerialNo(parseInt(self.StartSerialNo()) + parseInt(self.ShareKitta()) - 1)
        }
    })


    self.EnableDisableAEDButton = (data) => {
        $('#AddButton,#UpdateButton,#DeleteButton').attr('disabled', data)
    }
    //clear control
    self.ClearControl = function () {
        self.ShholderNo('')
        self.Name('')
        self.FaName('')
        self.GrFaName('')
        self.Address('')
        self.TelNo('')
        self.Occupation('')
        self.Minor('')
        self.TotalKitta('')
        self.CertificateNo('')
        self.SelectedShareType('')
        self.ShareKitta('')
        self.CertificateIssuedDate(formatDate(new Date()))
        self.SelectedCertificateType('')
        self.StartSerialNo('')
        self.EndSerialNo('')
        self.SelectedShOwnerType('')
        $('#SearchButton,#ShHolderNo,#CertificateNo,#shareType,#ShareKitta,#CertificateIssuedDate,#CertificateStatusSelect,#StartSerialNo,#ShOwnerType').attr('disabled', true)
        self.EnableDisableAEDButton(false)
        $("#SaveButton").text('Save')

    }
    self.ClearControl()
    //clear control search modal
    self.ClearControlSearch = function () {
        self.ShHolderNoFrom('')
        self.ShHolderNoTo('')
        self.CertificateNoFrom('')
        self.CertificateNoTo('')
        self.SerialNoFrom('')
        self.SerialNoTo('')
        self.ShareKittaFrom('')
        self.ShareKittaTo('')
    }
}

$(document).ready(function () {
    $('#simple-date1 .input-group.date').datepicker({
        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    ko.applyBindings(new CertificateEntry());


});