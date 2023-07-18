//const { isNullOrUndefined } = require("../../vendor/datatables/pdfmake/pdfmake");

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
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.ShOwnerTypeName = ko.observable(data.shOwnerTypeName);
        self.ShOwnerTypeAndName = self.ShOwnerType() + " " + self.ShOwnerTypeName();
    }
}

function ShownerSubType(data) {
    var self = this;
    if (data != undefined) {
        self.ShOwnerType = ko.observable(data.shownertype);
        self.ShOwnerSubType = ko.observable(data.shownerSubtype);
        self.ShOwnerSubTypeName = ko.observable(data.name1);
    }
}

function District(data) {
    var self = this;
    if (data != undefined) {
        self.DistCode = ko.observable(data.distCode);
        self.NpDistName = ko.observable(data.npDistName);
        self.EnDistName = ko.observable(data.enDistName);
        self.DistCodeName = self.DistCode() + " " + self.EnDistName();
    }
}

function OccupationL(data) {
    var self = this;
    self.shownertype = ko.observable(data.shownertype);
    self.OccupationId = ko.observable(data.occupationId);
    self.OccupationN = ko.observable(data.occupationN);
}

function Sign(data) {
    var self = this;
    if (data != undefined) {
        self.signature = ko.observable(data.signature);
        self.FileLength = ko.observable(data.FileLength);
        self.captureby = ko.observable(data.captureby);
        self.capturedate = ko.observable(data.capturedate);
        self.Is_Approved = ko.observable(data.Is_Approved);
        self.approvedby = ko.observable(data.approvedby);
        self.ApprovedDate = ko.observable(data.ApprovedDate);
        self.action1 = ko.observable(data.action1);
        self.entrydate = ko.observable(data.entrydate);
        self.entryuser = ko.observable(data.entryuser);
        self.ScanedBy = ko.observable(data.ScanedBy);
        self.PassProcted = ko.observable(data.PassProcted);
    }
}

function CertDet(data, vm) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.compcode);
        self.ShHolderNo = ko.observable(data.shHolderNo);
        self.CertNo = ko.observable(data.certNo);
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.CertStatus = ko.observable(data.certStatus);
        self.TimesofSplit = ko.observable(data.timesOfSplit);
        self.ShKitta = ko.observable(data.shKitta);
        self.ShOwnerType = ko.observable(data.shOwnerTypeText);
        self.ShareType = ko.observable(data.shareType);
        self.DistCert = ko.observable(data.distCert);
        self.DupliNo = ko.observable(data.dupliNo);
        self.PaidAmount = ko.observable(data.paidAmount);
        self.TotalAmount = ko.observable(data.TotalAmount);
        self.UserName = ko.observable(data.userName);
        self.EntryDate = ko.observable(data.entryDate);
        self.Remarks = ko.observable(data.remarks);
        self.cert_id = ko.observable(data.cert_id);
        self.share_type = ko.observable(data.shareTypeText);
        self.description = ko.observable(data.description);
        self.start_srno = ko.observable(data.start_srno);
        self.end_srno = ko.observable(data.end_srno);
        self.pslno = ko.observable(data.pslno);
        self.entryuser = ko.observable(data.entryuser);
        

        //date format fix
        if (data.issuedate != null) {
            self.Issuedate = ko.observable(data.issuedate.substr(0, data.issuedate.lastIndexOf("T")))
        } else {
            self.Issuedate = data.issuedate
        };
        if (data.certDistDt != null) {
            self.CertDistDt = ko.observable(data.certDistDt.substr(0, data.certDistDt.lastIndexOf("T")))
        } else {
            self.CertDistDt = data.certDistDt
        };
        if (data.tranDt != null) {
            self.TranDt = ko.observable(data.tranDt.substr(0, data.tranDt.lastIndexOf("T")))
        } else {
            self.TranDt = data.tranDt
        };
        if (data.issuse_dt != null) {
            self.issuse_dt = ko.observable(data.issuse_dt.substr(0, data.issuse_dt.lastIndexOf("T")))
        } else {
            self.issuse_dt = data.issuse_dt
        };
        self.CertStatusText = ko.observable(data.certStatusText);
        self.Cid = ko.observable(data.certStatus)



    }

}
function CertStatusT(data) {
    if (data != undefined) {
        var self = this;
        self.CertStatusId = ko.observable(data.certStatusId);
        self.CertStatus = ko.observable(data.certStatus);
    }
}

function HolderTitle(data) {
    var self = this;
    self.HolderTitleName = ko.observable(data.HolderTitleName);
}

function HolderNepaliTitle(data) {
    var self = this;
    self.HolderNepaliTitleName = ko.observable(convert_to_unicode_with_value(data.HolderNepaliTitleName));
    self.HolderNepaliName = ko.observable(data.HolderNepaliTitleName);
}

function ShHolder(data) {
    var self = this;
    if (data != undefined) {

        self.ShholderNo = ko.observable(data.shholderNo);
        self.Title = ko.observable(data.title);
        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.Address = ko.observable(data.address);
        self.Address1 = ko.observable(data.address1);
        self.Address2 = ko.observable(data.address2);
        self.DistCode = ko.observable(data.distCode);
        self.PboxNo = ko.observable(data.pboxNo);
        self.NpTitle = ko.observable(data.npTitle);
        self.NpName = ko.observable(data.npName);
        self.NpAdd = ko.observable(data.npAdd);
        self.FaName = ko.observable(data.faName);
        self.GrFaName = ko.observable(data.grFaName);
        self.HusbandName = ko.observable(data.husbandName);
        self.TelNo = ko.observable(data.telNo);
        self.MobileNo = ko.observable(data.mobileNo);
        self.EmailAdd = ko.observable(data.emailAdd);
        self.CitizenshipNo = ko.observable(data.citizenshipNo);
        self.Occupation = ko.observable(data.occupation);
        self.NomineeName = ko.observable(data.nomineeName);
        self.Relation = ko.observable(data.relation);
        self.Minor = ko.observable(data.minor);
       
        self.UserName = ko.observable(data.userName);
        self.Entrydate = ko.observable(data.entrydate);
        self.ActionType = ko.observable(data.actionType);
        self.shownertype = ko.observable(data.shownertype);
        self.ShownerSubtype = ko.observable(data.shownerSubtype);
        self.BankName = ko.observable(data.bankName);
        self.AccountNo = ko.observable(data.accountNo);
        self.Approved = ko.observable(data.approved);
        self.AppStatus = ko.observable(data.appStatus);
        self.ApprovedBy = ko.observable(data.approvedBy);
        self.ApprovedDate = ko.observable(data.approvedDate);
        self.Remarks = ko.observable(data.remarks);
        self.AppRemarks = ko.observable(data.appRemarks);
        self.HolderLock = ko.observable(data.holderLock);
        self.RefAppNo = ko.observable(data.refAppNo);
        if (data.aTTMinor != null) {
            self.GEnName = ko.observable(data.aTTMinor.gEnName);
            self.GRelation = ko.observable(data.aTTMinor.relation);
            self.DOB = ko.observable(data.aTTMinor.dateOfBirth);
        }
    }
}


var ManageShareHolder = function () {

    self.scheme_code = ko.observable();
    self.SchemeEnName = ko.observable();


    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()

    //ShOwnertype ko lagi
    self.ShOwnerTypes = ko.observableArray([]);
    self.ShOwnerType = ko.observable();
    self.ShOwnerTypeName = ko.observable();
    self.SelectedShOwnerType = ko.observable();

    //ShownerSUbType ko lagi
    self.ShOwnerSubTypes = ko.observableArray([]);
    self.SelectedShOwnerSubType = ko.observable();
    self.ShOwnerType = ko.observable();
    self.ShOwnerSubType = ko.observable();
    self.ShOwnerSubTypeName = ko.observable();

    //District ko lagi
    self.Districts = ko.observableArray([]);
    self.SelectedDistrict = ko.observable();
    self.DistCode = ko.observable();
    self.NpDistName = ko.observable();
    self.EnDistName = ko.observable();

    //Occupation ko lagi
    self.Occupations = ko.observableArray([]);
    self.SelectedOccupation = ko.observable();
    self.shownertype = ko.observable();
    self.OccupationId = ko.observable();
    self.OccupationN = ko.observable();

    //certdet ko lagi
    self.compcode = ko.observable();
    self.ShHolderNo = ko.observable();
    self.CertNo = ko.observable();
    self.SrNoFrom = ko.observable();
    self.SrNoTo = ko.observable();
    self.CertStatus = ko.observable();
    self.TimesofSplit = ko.observable();
    self.ShKitta = ko.observable();
    self.ShOwnerType = ko.observable();
    self.ShareType = ko.observable();
    self.Issuedate = ko.observable();
    self.DistCert = ko.observable();
    self.CertDistDt = ko.observable();
    self.TranDt = ko.observable();
    self.DupliNo = ko.observable();
    self.PaidAmount = ko.observable();
    self.TotalAmount = ko.observable();
    self.UserName = ko.observable();
    self.EntryDate = ko.observable();
    self.Remarks = ko.observable();
    self.cert_id = ko.observable();
    self.share_type = ko.observable();
    self.description = ko.observable();
    self.issuse_dt = ko.observable();
    self.start_srno = ko.observable();
    self.end_srno = ko.observable();
    self.pslno = ko.observable();
    self.entryuser = ko.observable();
    self.CertDetList = ko.observableArray([]);

    //cert status lai
    self.CertStatuses = ko.observableArray([]);
    self.CertStatusId = ko.observable();
    self.CertStatus = ko.observable();
    self.SelectedCertStatus = ko.observable();

    //update remarks

    self.UpdateRemarks = ko.observable();

    //loading data for datatable

    //loading data table for certificate list


    self.HolderTitle = ko.observableArray([
        new HolderTitle({ "HolderTitleName": "Mr." }),
        new HolderTitle({ "HolderTitleName": "Ms." }),
        new HolderTitle({ "HolderTitleName": "Mrs." }),
    ])
    self.SelectedHolderTitle = ko.observable();


    self.HolderNepaliTitle = ko.observableArray([
        new HolderNepaliTitle({ "HolderNepaliTitleName": ">L" }), // sri
        new HolderNepaliTitle({ "HolderNepaliTitleName": ">LdtL" }), // srimati
        new HolderNepaliTitle({ "HolderNepaliTitleName": ';">L' }), // susri
        new HolderNepaliTitle({ "HolderNepaliTitleName": 'gfafns' }), //  nabalak
    ])
    self.SelectedHolderNepaliTitle = ko.observable()


    //for modal
    self.firstName = ko.observable()
    self.lastName = ko.observable()
    self.grandFatherName = ko.observable()
    self.fatherName = ko.observable()

    //Shholderkolagi
    self.ShholderNo = ko.observable();
    self.Title = ko.observable();
    self.FName = ko.observable();
    self.LName = ko.observable();
    self.Address = ko.observable();
    self.Address1 = ko.observable();
    self.Address2 = ko.observable();
    self.DistCode = ko.observable();
    self.PboxNo = ko.observable();
    self.NpTitle = ko.observable();
    self.NpName = ko.observable();
    self.NpAdd = ko.observable();
    self.FaName = ko.observable();
    self.GrFaName = ko.observable();
    self.HusbandName = ko.observable();
    self.TelNo = ko.observable();
    self.MobileNo = ko.observable();
    self.EmailAdd = ko.observable();
    self.CitizenshipNo = ko.observable();
    self.Occupation = ko.observable();
    self.NomineeName = ko.observable();
    self.Relation = ko.observable();
    self.Minor = ko.observable();
    self.TotalKitta = ko.observable();
    self.Tfrackitta = ko.observable();
    self.UserName = ko.observable();
    self.Entrydate = ko.observable();
    self.ActionType = ko.observable();
    self.shownertype = ko.observable();
    self.ShownerSubtype = ko.observable();
    self.BankName = ko.observable();
    self.AccountNo = ko.observable();
    self.Approved = ko.observable();
    self.AppStatus = ko.observable();
    self.ApprovedBy = ko.observable();
    self.ApprovedDate = ko.observable();
    self.Remarks = ko.observable();
    self.AppRemarks = ko.observable();
    self.HolderLock = ko.observable();
    self.RefAppNo = ko.observable();
    self.Nomiee = ko.observable(false);
    self.LockMessage = ko.observable();

    self.ShHolderList = ko.observableArray([])
    //minor

    self.GEnName = ko.observable();
    self.GRelation = ko.observable();
    self.Minor = ko.observable(false);
    self.DOB = ko.observable();

    //Signature ko lagi 

    self.Signature = ko.observable();


    //Signature ko lagi
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
    self.ScanedBy = ko.observable();
    self.PassProcted = ko.observable();
    self.FileNameSign = ko.observable();


    //for update remarks

    self.UpdateRemarks = ko.observable();




    //gloabal variables
    var docsize = "";
    var docname = "";
    var filename = "";

    var optionAUD = "";




    //disable buttons
    var disableButtons = function () {
        document.getElementById("AddButton").disabled = "disabled";
        //document.getElementById("DeleteButton").disabled = "disabled";
        document.getElementById("SearchButton").disabled = "disabled";
    };
    var enableButtons = function () {
        document.getElementById("AddButton").disabled = false;
        //document.getElementById("DeleteButton").disabled = false;
        document.getElementById("SearchButton").disabled = false;
    };

    var clearAllData = function () {
        enableButtons();
        self.ShholderNo('');
        self.Title('');
        self.FName('');
        self.LName('');
        self.Address('');
        self.Address1('');
        self.Address2('');
        self.DistCode('');
        self.PboxNo('');
        self.NpTitle('');
        self.NpName('');
        self.NpAdd('');
        self.FaName('');
        self.GrFaName('');
        self.HusbandName('');
        self.TelNo('');
        self.MobileNo('');
        self.EmailAdd('');
        self.CitizenshipNo('');
        self.Occupation('');
        self.NomineeName('');
        self.Relation('');
        self.Minor('');
        self.TotalKitta('');
        self.Tfrackitta('');
        self.UserName('');
        self.Entrydate('');
        self.ActionType('');
        self.shownertype('');
        self.ShownerSubtype('');
        self.BankName('');
        self.AccountNo('');

        self.HolderLock('');
        self.GEnName('');
        self.GRelation('');
        self.Minor('');
        self.DOB('');
        self.signature('');
        self.FileLength('');
        self.FileNameSign('');
        self.captureby('');
        self.capturedate('');
        self.Is_Approved('');
        self.approvedby('');
        self.ApprovedDate('');
        self.action1('');
        self.entrydate('');
        self.entryuser('');
        self.ScanedBy('');
        self.PassProcted('');
        self.SelectedDistrict('');
        self.SelectedHolderTitle('');
        self.SelectedOccupation('');
        self.SelectedShOwnerSubType('');
        self.SelectedShOwnerType('02');
        self.Nomiee(false);
        
        document.getElementById("showCertificate").style.display = "none";
        document.getElementById("showSignature").style.display = "none";
        document.getElementById("saveShholder").disabled = "disabled";
        document.getElementById("report").style.visibility = "hidden";
    }


    //Validation 
    self.Validation = function () {

        var errMsg = "";
        if (optionAUD != "A") {
            if (Validate.empty(self.ShholderNo())) {
                errMsg += "Please Enter Sh Holder No<br/>"
            }

        }
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company<br/>";
        }

        if (Validate.empty(self.SelectedShOwnerType())) {
            errMsg += "Please Choose shownertype<br/>";
        }
        if (Validate.empty(self.SelectedShOwnerSubType())) {
            errMsg += "Please Choose Showner Sub type<br/>";
        }
        if (Validate.empty(self.SelectedHolderTitle())) {
            errMsg += "Please Choose Title<br/>"
        }
        if (Validate.empty(self.FName())) {
            errMsg += "Please Enter First Name<br/>"
        }
        if (Validate.empty(self.LName())) {
            errMsg += "Please Enter Last Name<br/>"
        }
        if (Validate.empty(self.Address())) {
            errMsg += "Please Enter Address<br/>"
        }
        if (Validate.empty(self.LName())) {
            errMsg += "Please Enter Last Name<br/>"
        }
        if (Validate.empty(self.NpAdd())) {
            errMsg += "Please Enter Nepali Address <br/>"
        }
        
       
        if (Validate.empty(self.SelectedDistrict())) {
            errMsg += "Please Choose District Code<br/>"
        }
        if (Validate.empty(self.SelectedHolderNepaliTitle())) {
            errMsg += "Please Choose Nepali Title<br/>"
        }
        if (Validate.empty(self.NpName())) {
            errMsg += "Please Enter Nepali Name<br/>"
        }
        if (Validate.empty(self.FaName())) {
            errMsg += "Please Enter Father Name<br/>"
        }
        if (Validate.empty(self.GrFaName())) {
            errMsg += "Please Enter Grand Father Name<br/>"
        }
        
        
        if (Validate.empty(self.CitizenshipNo())) {
            errMsg += "Please Enter Citizenship No<br/>"
        }
        if (Validate.empty(self.SelectedOccupation())) {
            errMsg += "Please Enter Occupation<br/>"
        }
        if (self.Nomiee()) {
            if (Validate.empty(self.NomineeName())) {
                errMsg += "Please Enter Nominee Name<br/>";
            }
            if (Validate.empty(self.Relation())) {
                errMsg += "Please Enter Relation<br/>";
            }
        }
        if (self.Minor()) {
            if (Validate.empty(self.GEnName())) {
                errMsg += "Please Enter Guardian's Name<br/>";
            }
            if (Validate.empty(self.GRelation())) {
                errMsg += "Please Enter Guardian Relation<br/>";
            }
            if (Validate.empty(self.DOB())) {
                errMsg += "Please Enter Date of Birth<br/>";
            }
        }

        
        if (optionAUD == "U") {
            if (Validate.empty(self.UpdateRemarks())) {
                errMsg += "Please Add Update Remarks !!!";
            }
        }

        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        } else {
            return true;
        }

    }

    var GetNewShHolderNo = function (compcode) {
        if (self.ValidateCompany()) {
            $.ajax({
                type: "post",
                url: '/HolderManagement/ShareHolder/GetNewShHolderNo',
                data: { 'compcode': self.SelectedCompany() },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        self.ShholderNo(result.responseData);
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

    //datatable lai data load gareko certificate ko lagi
    self.LoadCertDet = function () {
        document.getElementById("showCertificate").disabled = true;
        $.ajax({
            type: "post",
            url: '/HolderManagement/Certificate/GetCertDet',
            data: {
                'ShHolderNo': self.ShholderNo(),
                'CompCode': self.SelectedCompany()
            },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {

                $('#CertDetTable').DataTable().destroy();
                $('#CertDetTable tbody').empty();


                if (result.isSuccess) {
                    $('#certificateModalTheme').modal('show');


                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new CertDet(item)
                    });
                    self.CertDetList(mappedTasks);

                    $('#CertDetTable').DataTable();
                    document.getElementById("showCertificate").disabled = false;

                } else {
                    alert('warning', result.message)
                    document.getElementById("showCertificate").disabled = false;

                }
            },
            error: function (error) {
                alert('error', error.message)
                document.getElementById("showCertificate").disabled = false;
            }
        });


    }
    //load owner type on form load
    self.LoadShOwnerType = function () {
        $.ajax({
            type: "post",
            url: '/HolderManagement/ShareHolder/GetAllShOwnerType',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new ShownerType(item);
                    });
                    self.ShOwnerTypes(mappedTasks);
                    self.SelectedShOwnerType('02');
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
    }
    self.LoadShOwnerType();

    //load sub owner type funciton
    self.LoadShOwnerSubType = function () {
        $.ajax({
            type: "post",
            url: '/HolderManagement/ShareHolder/GetAllShOwnerSubType',
            data: { 'shownertype': self.SelectedShOwnerType() },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new ShownerSubType(item);
                    });
                    self.ShOwnerSubTypes(mappedTasks);
                    self.SelectedShOwnerSubType('01')
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
    }

    //load occupation function
    self.LoadOccupation = function () {
        $.ajax({
            type: "post",
            url: '/HolderManagement/ShareHolder/GetAllOccupation',
            data: { 'shownertype': self.SelectedShOwnerType() },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new OccupationL(item);
                    });
                    self.Occupations(mappedTasks);

                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
    }

    //loading showuner sub type and occupation
    self.SelectedShOwnerType.subscribe(function () {
        self.LoadShOwnerSubType();
        self.LoadOccupation();
    });


    //loading district on form load
    self.LoadDistrict = function () {
        $.ajax({
            type: "post",
            url: '/HolderManagement/ShareHolder/GetAllDistrict',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new District(item);
                    });
                    self.Districts(mappedTasks);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
    }
    self.LoadDistrict();


    self.SaveCertificate = function () {
        swal({
            title: "Are you sure?",
            text: `Certificate of Holder No ${self.ShholderNo()} Will Be Updated.`,
            icon: "warning",
            buttons: true,
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    type: "PUT",
                    url: `/HolderManagement/Certificate/UpdateCertificates`,
                    data: { 'shholderno': self.ShholderNo(), 'lisOfCertificates': self.CertDetList() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (data) {
                        if (data.isSuccess) {
                            self.LoadCertDet()

                            toastr.success(data.message);
                        }
                        else {
                            toastr.error(data.message);

                        }
                    }
                });
            }
        });
        //console.log(ko.toJS(self.CertDetList()))
    }

    self.ShholderNo.subscribe(() => {
        if (!Validate.empty(self.ShholderNo())) self.GetSholderInformation();
    })
    self.GetSholderInformation = function (data) {
        var shholderNo = self.ShholderNo();
        if (data != undefined) {
            shholderNo = data.ShholderNo();
            self.ShholderNo(shholderNo);
        }
        $("#HoldersList").modal('hide');
        if (shholderNo != undefined && shholderNo != "") {
            Openloader()
            $.ajax({
                type: "post",
                url: '/HolderManagement/ShareHolder/GetHolderInformation',
                data: { 'ShHolderNo': ko.toJS(shholderNo), 'CompCode': ko.toJS(self.SelectedCompany()), 'SelectedAction': optionAUD },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {

                    if (result.isSuccess) {
                        
                        $('#holderlock').modal('hide');
  
                        //self.Title(result.responseData.title);
                        
                        self.FName(result.responseData.fName);
                        self.LName(result.responseData.lName);
                        self.Address(result.responseData.address);
                        self.Address1(result.responseData.address1);
                        self.Address2(result.responseData.address2);
                        //self.DistCode(result.responseData.fName);
                        self.PboxNo(result.responseData.pboxNo);
                        if (self.NpName != null && self.NpName != undefined) {
                            self.NpName(convert_to_unicode_with_value(result.responseData.npName));
                        }
                        if (self.NpAdd != null && self.NpAdd != undefined) {
                            self.NpAdd(convert_to_unicode_with_value(result.responseData.npAdd));
                        }
                        self.FaName(result.responseData.faName);
                        self.GrFaName(result.responseData.grFaName);
                        self.HusbandName(result.responseData.husbandName);
                        self.TelNo(result.responseData.telNo);
                        self.MobileNo(result.responseData.mobileNo);
                        self.EmailAdd(result.responseData.emailAdd);
                        self.CitizenshipNo(result.responseData.citizenshipNo);
                        self.SelectedOccupation(result.responseData.occupation);
                        self.NomineeName(result.responseData.nomineeName);
                        if (result.responseData.nomineeName != null) {
                            self.Nomiee(true)
                        }
                        self.Relation(result.responseData.relation);
                        
                        if (result.responseData.totalKitta != undefined) {
                            self.TotalKitta('Total Kitta: ' + result.responseData.totalKitta)
                            self.Tfrackitta('Fraction: ' +result.responseData.tfrackitta);
                        }
                        

                        self.BankName(result.responseData.bankName);
                        self.AccountNo(result.responseData.accountNo);

                        self.SelectedDistrict(result.responseData.distCode);
                        self.SelectedOccupation(result.responseData.occupation);
                        self.SelectedShOwnerType(result.responseData.shownertype);
                        self.SelectedShOwnerSubType(result.responseData.shownerSubtype);
                        self.SelectedHolderTitle(result.responseData.title);
                        self.SelectedHolderNepaliTitle(result.responseData.npTitle)
                        $('#showCertificate').get(0).style.display = '';
                        $('#showSignature').get(0).style.display = '';
                       

                        if (result.responseData.minor) {
                            self.DOB((result.responseData.aTTMinor.dateOfBirth))
                            self.Minor(result.responseData.minor);
                            self.GRelation(result.responseData.aTTMinor.relation);
                            self.GEnName(result.responseData.aTTMinor.gEnName);
                        } else {
                            self.DOB('')
                            self.Minor('');
                            self.GRelation('')
                        }
                        if (result.isValid) {
                            alert('warning', result.message)
                            $('#holderlock').attr('hidden', false);

                            self.LockMessage(result.message);
                        }
                    } else {
                        alert('warning', result.message)
                        self.refresh()
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                    self.refresh()
                }, complete: () => {
                    Closeloader()
                }
            })
        }
    }

    //for loading file in the image box upon upload
    var loadFile = function (obj) {
        var image = document.getElementById('signatureOutput');
        image.src = URL.createObjectURL(obj);
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


            if ((files[0].type != "image/jpeg")) {

                edocerror = 1;
                toastr.error("Document format must only be jpeg!!!<br>");
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


    //for showing signature in signature modal
    self.GetSignature = function () {
        var showSignValidate = true;
        if (ko.toJS(self.ShholderNo()) == undefined) {
            toastr.error("Please Type a Sholder No.");
            showSignValidate = false;
        }
        if (ko.toJS(self.SelectedCompany()) == undefined) {
            toastr.error("Please Select a Company.");
            showSignValidate = false;
        }
        if (showSignValidate) {
            document.getElementById("showSignature").disabled = true;
            Openloader()
            $.ajax({
                type: "post",
                url: '/HolderManagement/ShareHolder/GetSignature',
                data: { 'holderno': ko.toJS(self.ShholderNo()), 'compcode': ko.toJS(self.SelectedCompany()) },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        self.Signature("data:image/jpeg;base64," + result.responseData.signature);
                        $("#signatureModalTheme").modal('show');
                        document.getElementById("showSignature").disabled = false;

                    } else if (!result.isSuccess) {

                        alert('error', "Sorry, the signature could not be found.");
                        document.getElementById("showSignature").disabled = false;

                    }
                },
                error: function (error) {
                    alert('error', error.message);
                    document.getElementById("showSignature").disabled = false;

                },
                complete: () => {
                    Closeloader()
                }
            });
        }
    }


    //for saving ShHolder
    self.SaveShHolder = function () {
        if (self.Validation()) {
            if (optionAUD != "") {
                var shHolderModal = {
                    'compcode': self.SelectedCompany(),
                    'ShholderNo': self.ShholderNo(),
                    'Title': self.SelectedHolderTitle(),
                    'FName': self.FName(),
                    'LName': self.LName(),
                    'Address': self.Address(),
                    'Address1': self.Address1(),
                    'Address2': self.Address2(),
                    'DistCode': self.SelectedDistrict(),
                    'PboxNo': self.PboxNo(),
                    'NpTitle': self.SelectedHolderNepaliTitle(),
                    'NpName': self.NpName(),
                    'NpAdd': self.NpAdd(),
                    'FaName': self.FaName(),
                    'GrFaName': self.GrFaName(),
                    'HusbandName': self.HusbandName(),
                    'TelNo': self.TelNo(),
                    'MobileNo': self.MobileNo(),
                    'EmailAdd': self.EmailAdd(),
                    'CitizenshipNo': self.CitizenshipNo(),
                    'Occupation': self.SelectedOccupation(),
                    'NomineeName': self.NomineeName(),
                    'Relation': self.Relation(),
                    'Minor': self.Minor(),
                    'ActionType': optionAUD,
                    'shownertype': self.SelectedShOwnerType(),
                    'ShownerSubtype': self.SelectedShOwnerSubType(),
                    'BankName': self.BankName(),
                    'AccountNo': self.AccountNo(),
                    'AppStatus': "UNPOSTED",
                    'Approved': "N",
                    'HolderLock': "Y",
                    'aTTMinor': {
                        'GEnName': self.GEnName(),
                        'Relation': self.GRelation(),
                        'DOB': self.DOB()
                    }
                };
                //var signature = self.signature();
                //var signatureFileLength = self.FileLength();
                //var signname = self.FileNameSign();

                var text = "";
                if (optionAUD == "A") {
                    text = `${self.ShholderNo()} Holder will be created.`;
                }
                else if (optionAUD == "U") {
                    text = `${self.ShholderNo()} Holder's Details will be Updated.`;
                }
                else if (optionAUD == "D") {
                    text = `${self.ShholderNo()} Holder will be Deleted.`;
                }

                enableButtons();
                swal({
                    title: "Are you sure?",
                    text: text,
                    icon: "warning",
                    buttons: true,
                    dangerMode: true
                }).then((willSave) => {
                    if (willSave) {
                        Openloader()
                        $.ajax
                            ({
                                type: "post",
                                url: "/HolderManagement/ShareHolder/SaveShholder",
                                data: {
                                    aTTShHolder: shHolderModal,
                                    updateRemarks: self.UpdateRemarks() ?? self.UpdateRemarks(),
                                    selectedAction: optionAUD
                                },
                                dataType: "json", beforeSend: function (xhr) {
                                    xhr.setRequestHeader("XSRF-TOKEN",
                                        $('input:hidden[name="__RequestVerificationToken"]').val());
                                },
                                success: (result) => {
                                    if (result.IsSuccess) {
                                        swal({
                                            title: "",
                                            text: result.Message,
                                            icon: "info",
                                            button: "Ok",
                                        }).then(function (result) {
                                            self.refresh();
                                        });
                                    } else {
                                        alert('error', result.Message)
                                    }
                                }, error: (error) => {
                                    alert('error', error.message == undefined ? error.Message : error.message)
                                }, complete: () => {
                                    Closeloader()
                                }
                            })
                    }
                });


            } else {
                alert("error", "Please Choose Add or Update.");
            }
        }
    }

    // to serach with Modal
    self.SearchShHolder = function () {
        if (!Validate.empty(self.firstName()) || !Validate.empty(self.lastName()) || !Validate.empty(self.fatherName()) || !Validate.empty(self.grandFatherName())) {
            Openloader()
            $.ajax({
                url: '/HolderManagement/ShareHolder/GetHolderByQuery',
                data: { 'CompCode': self.SelectedCompany(), 'FirstName': self.firstName(), 'LastName': self.lastName(), 'FatherName': self.fatherName(), 'GrandFatherName': self.grandFatherName() },
                type: 'POST',
                datatype: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    $("#searchQueryModalTheme").modal('hide');
                    if (result.isSuccess) {

                        if (result.responseData.length > 0) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new ShHolder(item)
                            });
                            self.ShHolderList(mappedTasks);
                            $("#HoldersList").modal('show');

                        } else {
                            alert('error', "No Record Found");
                            $("#HoldersList").modal('hide');
                        }
                    } else {
                        alert('error', result.message);
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }, complete: () => {
                    Closeloader()
                }

            })

        }

    }

    //popping div elements for minor and nominee
    self.Nomiee.subscribe(function (data) {
        if (data == true) {
            $('#ifNominee').get(0).style.display = '';
        } else if (data == false || data == undefined) {
            document.getElementById("ifNominee").style.display = "none";
        }
    });

    self.Minor.subscribe(function (data) {
        if (data == true) {
            $('#ifMinor').get(0).style.display = '';
        } else if (data == false || data == undefined) {
            document.getElementById("ifMinor").style.display = "none";
        }
    });

    //for choosing add or update or delete
    self.chooseOptions = function (data) {
        clearAllData();
        optionAUD = data;
        //$('#ifSignature').get(0).style.display = '';
        disableButtons();
        //document.getElementById("saveShholder").disabled = false;
        if (optionAUD === "A") {

            document.getElementById("ifUpdateRemarks").style.display = "none";
            document.getElementById("ShHolderNo").disabled = true;
            document.getElementById("saveShholder").disabled = false;

            self.EnableElements();
        } else {
            if (optionAUD === "D") {
                self.EnableElements();
                document.getElementById("ShHolderNo").disabled = false;
                //document.getElementById("ifUpdateRemarks").style.display = "block";

            }
            document.getElementById("ShHolderNo").focus();
        }
        
    }

    self.DisableElements = function () {
        document.getElementById("shOwnerType").disabled = true;
        document.getElementById("subShownerType").disabled = true;
        document.getElementById("title").disabled = true;
        document.getElementById("firstName").disabled = true;
        document.getElementById("lastName").disabled = true;
        document.getElementById("nepTitle").disabled = true;
        document.getElementById("nepName").disabled = true;
        document.getElementById("fullAddress").disabled = true;
        document.getElementById("nepAddress").disabled = true;
        document.getElementById("address1").disabled = true;
        document.getElementById("address2").disabled = true;
        document.getElementById("pBoxNo").disabled = true;
        document.getElementById("districtCodeSelect2").disabled = true;
        document.getElementById("citizenshipNo").disabled = true;
        document.getElementById("email").disabled = true;
        document.getElementById("telNo").disabled = true;
        document.getElementById("mobileNo").disabled = true;
        document.getElementById("bankName").disabled = true;
        document.getElementById("accNo").disabled = true;
        document.getElementById("faName").disabled = true;
        document.getElementById("grFaName").disabled = true;
        document.getElementById("husbandName").disabled = true;
        document.getElementById("occupation").disabled = true;
        document.getElementById("accNo").disabled = true;
        document.getElementById("nomineeName").disabled = true;
        document.getElementById("nomineeRelation").disabled = true;
        document.getElementById("gEnName").disabled = true;
        document.getElementById("gRelation").disabled = true;
        document.getElementById("dateOfBirth").disabled = true;
    }

    self.DisableElements();
    $('#holderlock').attr('hidden', true);

    self.EnableElements = function () {
        document.getElementById("shOwnerType").disabled = false;
        document.getElementById("subShownerType").disabled = false;
        document.getElementById("title").disabled = false;
        document.getElementById("firstName").disabled = false;
        document.getElementById("lastName").disabled = false;
        document.getElementById("nepTitle").disabled = false;
        document.getElementById("nepName").disabled = false;
        document.getElementById("fullAddress").disabled = false;
        document.getElementById("nepAddress").disabled = false;
        document.getElementById("address1").disabled = false;
        document.getElementById("address2").disabled = false;
        document.getElementById("pBoxNo").disabled = false;
        document.getElementById("districtCodeSelect2").disabled = false;
        document.getElementById("citizenshipNo").disabled = false;
        document.getElementById("email").disabled = false;
        document.getElementById("telNo").disabled = false;
        document.getElementById("mobileNo").disabled = false;
        document.getElementById("bankName").disabled = false;
        document.getElementById("accNo").disabled = false;
        document.getElementById("faName").disabled = false;
        document.getElementById("grFaName").disabled = false;
        document.getElementById("husbandName").disabled = false;
        document.getElementById("occupation").disabled = false;
        document.getElementById("accNo").disabled = false;
        document.getElementById("nomineeName").disabled = false;
        document.getElementById("nomineeRelation").disabled = false;
        document.getElementById("gEnName").disabled = false;
        document.getElementById("gRelation").disabled = false;
        document.getElementById("dateOfBirth").disabled = false;
    }

    self.refresh = function () {
        clearAllData();
        enableButtons();
        self.DisableElements();
        //document.getElementById("ifSignature").style.display = "none";
        document.getElementById("ifUpdateRemarks").style.display = "none";
        document.getElementById("ShHolderNo").disabled = true;

    }
}

$(document).ready(function () {
    document.getElementById("showCertificate").style.display = "none";
    document.getElementById("showSignature").style.display = "none";
    document.getElementById("saveShholder").disabled = "disabled";
    document.getElementById("report").style.display = "none";
    //document.getElementById("ifSignature").style.display = "none";
    document.getElementById("ifMinor").style.display = "none";
    document.getElementById("ifNominee").style.display = "none";
    document.getElementById("ifUpdateRemarks").style.display = "none";
    ko.applyBindings(new ManageShareHolder());

});