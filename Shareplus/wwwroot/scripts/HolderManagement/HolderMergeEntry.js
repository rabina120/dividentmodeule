function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
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

function MergeDetail(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.compcode)
        self.merge_id = ko.observable(data.merge_id)
        self.holdernofrom = ko.observable(data.holdernofrom)
        self.holdernoto = ko.observable(data.holdernoto)
        self.kitta_from = ko.observable(data.kitta_from)
        self.kitta_to = ko.observable(data.kitta_to)
        self.mergedate = ko.observable(data.mergedate == null ? "" : data.mergedate.substring(0, 10))
        self.merge_remarks = ko.observable(data.merge_remarks)
        self.merge_by = ko.observable(data.merge_by)
        self.entry_dt = ko.observable(data.entry_dt)
        self.approved = ko.observable(data.approved)
        self.app_status = ko.observable(data.app_status)
        self.app_date = ko.observable(data.app_date)
        self.approved_by = ko.observable(data.approved_by)
        self.approved_remarks = ko.observable(data.approved_remarks)
    }
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
        self.TotalKitta = ko.observable(data.totalKitta);
        self.Tfrackitta = ko.observable(data.tfrackitta);
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
    }
}


var HolderMergeEntry = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()


    //merge data
    self.MergeNo = ko.observable()
    self.MergeRemarks = ko.observable()
    self.MergeDate = ko.observable()

    //Shholderkolagi
    self.ShholderNo = ko.observable();
    self.TotalKitta = ko.observable();
    self.FullName = ko.observable();
    self.OwnerType = ko.observable();
    self.NpName = ko.observable();
    self.OwnerSubType = ko.observable();
    self.FaName = ko.observable();
    self.GrFaName = ko.observable();
    self.Address = ko.observable();
    self.District = ko.observable();
    self.TelNo = ko.observable();
    self.PboxNo = ko.observable();


    self.ShholderNoTo = ko.observable();
    self.TotalKittaTo = ko.observable();
    self.FullNameTo = ko.observable();
    self.OwnerTypeTo = ko.observable();
    self.NpNameTo = ko.observable();
    self.OwnerSubTypeTo = ko.observable();
    self.FaNameTo = ko.observable();
    self.GrFaNameTo = ko.observable();
    self.AddressTo = ko.observable();
    self.DistrictTo = ko.observable();
    self.TelNoTo = ko.observable();
    self.PboxNoTo = ko.observable();

    //Signature ko lagi 

    self.Signature = ko.observable();
    self.SignatureTo = ko.observable();


    self.ShHolderMergeList = ko.observableArray([])


    //gloabal variables
    var docsize = "";
    var docname = "";
    var filename = "";

    var optionAUD = "";
    function formatDate(date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2)
            month = '0' + month;
        if (day.length < 2)
            day = '0' + day;

        return [year, month, day].join('-');
    }


    self.MergeDate(formatDate(new Date))

    var clearAllData = function () {
        //Shholderkolagi
        self.ShholderNo('');
        self.TotalKitta('');
        self.FullName('');
        self.OwnerType('');
        self.NpName('');
        self.OwnerSubType('');
        self.FaName('');
        self.GrFaName('');
        self.Address('');
        self.District('');
        self.TelNo('');
        self.PboxNo('');
        self.ShholderNoTo('');
        self.TotalKittaTo('');
        self.FullNameTo('');
        self.OwnerTypeTo('');
        self.NpNameTo('');
        self.OwnerSubTypeTo('');
        self.FaNameTo('');
        self.GrFaNameTo('');
        self.AddressTo('');
        self.DistrictTo('');
        self.TelNoTo('');
        self.PboxNoTo('');
        self.MergeNo('')
        self.MergeRemarks('')
        self.MergeDate(formatDate(new Date))
        self.Signature('');
    }

    var clearFromData = function () {
        self.ShholderNo('');
        self.TotalKitta('');
        self.FullName('');
        self.OwnerType('');
        self.NpName('');
        self.OwnerSubType('');
        self.FaName('');
        self.GrFaName('');
        self.Address('');
        self.District('');
        self.TelNo('');
        self.PboxNo('');
    }

    var clearToData = function () {
        self.ShholderNoTo('');
        self.TotalKittaTo('');
        self.FullNameTo('');
        self.OwnerTypeTo('');
        self.NpNameTo('');
        self.OwnerSubTypeTo('');
        self.FaNameTo('');
        self.GrFaNameTo('');
        self.AddressTo('');
        self.DistrictTo('');
        self.TelNoTo('');
        self.PboxNoTo('');
    }


    //Validation 
    self.Validation = function () {
        var errMsg = "";
        if (optionAUD != "A") {
            if (Validate.empty(self.MergeNo()))
                errMsg+= "Please Enter Merge No !!!<br/>"
        }
        if (Validate.empty(self.ShholderNo())) {
            errMsg += "Please Enter Sh Holder No !!!<br/>"
        }
        if (Validate.empty(self.ShholderNoTo())) {
            errMsg += "Please Enter Sh Holder No !!!<br/>"
        }
        if (Validate.empty(self.MergeRemarks())) {
            errMsg += "Please Enter Rtemarks !!! <br/>"
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

    //self.GetMaxMergeNo = function () {
    //    if (self.ValidateCompany()) {
    //        Openloader()
    //        $.ajax({
    //            type: "post",
    //            url: '/HolderManagement/HolderMergeEntry/GetMaxMergeNo',
    //            data: { 'CompCode': self.SelectedCompany() },
    //            datatype: "json",
    //            success: function (result) {
    //                if (result.isSuccess) {
    //                    self.MergeNo(result.responseData)
    //                } else{
    //                    alert('error', result.message)
    //                }
    //            },
    //            error: function (error) {
    //                alert('error', error.message);
    //                document.getElementById("showSignature").disabled = false;
    //            },
    //            complete: () => {
    //                Closeloader()
    //            }
    //        });
    //    }
    //}


    self.GetSholderInformation = function (data,recordType) {
        if (self.ValidateCompany()) {

            if (!Validate.empty(recordType == "From" ? self.ShholderNo() : self.ShholderNoTo())) {
            if (self.ShholderNo() != self.ShholderNoTo()) {
                var shholderNo = data();
                if (shholderNo == null || undefined) {
                    recordType == "From" ? self.ShholderNo() : self.ShholderNoTo()
                }
                Openloader()
                $.ajax({
                    type: "post",
                    url: '/HolderManagement/HolderMergeEntry/GetHolderForMerge',
                    data: {
                        'ShHolderNo': shholderNo,
                        'CompCode': self.SelectedCompany(),
                        'SelectedAction': optionAUD
                    },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            if (recordType == "From") {
                                self.TotalKitta(result.responseData.totalKitta);
                                self.FullName(result.responseData.fName + ' ' + result.responseData.lName);
                                self.OwnerType(result.responseData.shownertype);
                                self.NpName(result.responseData.npName);
                                self.OwnerSubType(result.responseData.shownerSubtype);
                                self.FaName(result.responseData.faName);
                                self.GrFaName(result.responseData.grFaName);
                                self.Address(result.responseData.address);
                                self.District(result.responseData.distCode);
                                self.TelNo(result.responseData.telNo);
                                self.PboxNo(result.responseData.pboxNo);
                                $('#shHolderFromSignature').attr('disabled',false)
                            }
                            else {
                                self.TotalKittaTo(result.responseData.totalKitta);
                                self.FullNameTo(result.responseData.fName + ' ' + result.responseData.lName);
                                self.OwnerTypeTo(result.responseData.shownertype);
                                self.NpNameTo(result.responseData.npName);
                                self.OwnerSubTypeTo(result.responseData.shownerSubtype);
                                self.FaNameTo(result.responseData.faName);
                                self.GrFaNameTo(result.responseData.grFaName);
                                self.AddressTo(result.responseData.address);
                                self.DistrictTo(result.responseData.distCode);
                                self.TelNoTo(result.responseData.telNo);
                                self.PboxNoTo(result.responseData.pboxNo);
                                $('#shHolderToSignature').attr('disabled', false)
                            }

                        } else {
                            alert('error', result.message)
                            recordType == "From" ? clearFromData() : clearToData()
                            recordType == "From" ? $('#ShholderNo').focus() : $('#ShholderNoTo').focus()
                            recordType == "From" ? $('#shHolderFromSignature').attr('disabled', true) : $('#shHolderToSignature').attr('disabled', true)
                        }
                    },
                    error: function (error) {
                        alert('error', error.message);
                        $('#shHolderToSignature,#shHolderFromSignature').attr('disabled', true)
                    },
                    complete: () => {
                        Closeloader()
                    }
                });
            } else {
                alert('warning','From Holder and To Holder Cannot Be Same !!!')
            }
            
            }
        }
    }
    //for showing signature in signature modal
    self.GetSignature = function (data) {
        if (self.ValidateCompany()) {
            var showSignValidate = true;
            if (data == 'From') {
                if (Validate.empty(self.ShholderNo())) {
                    showSignValidate = false;
                }
            } else {
                if (Validate.empty(self.ShholderNoTo())) {
                    showSignValidate = false;
                }
            }
            if (showSignValidate) {
                
                Openloader()
                $.ajax({
                    type: "post",
                    url: '/HolderManagement/HolderMergeEntry/GetSignature',
                    data: { 'ShHolderNo': data == "From" ? self.ShholderNo() : self.ShholderNoTo(), 'CompCode': self.SelectedCompany() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                           
                            if (data == 'From') {
                                self.Signature("data:image/jpeg;base64," + result.responseData.signature);
                                $('#signatureFromDiv').show()
                            }
                            else {
                                self.SignatureTo("data:image/jpeg;base64," + result.responseData.signature);
                                $('#signatureToDiv').show()
                            }
                        } else {
                            alert('error', "Sorry, the signature could not be found.");
                            if (data == 'From') {
                                self.Signature('')
                                $('#signatureFromDiv').hide()
                            }
                            else {
                                self.SignatureTo('')
                                $('#signatureToDiv').hide()
                            }
                        }
                    },
                    error: function (error) {
                        alert('error', error.message);
                        if (data == 'From') {
                            self.Signature('')
                            $('#signatureFromDiv').hide()
                        }
                        else {
                            self.SignatureTo('')
                            $('#signatureToDiv').hide()
                        }

                    },
                    complete: () => {
                        Closeloader()
                    }
                });
            }
            else {
                alert('error', 'Load Data First !!!')
            }
        } 
    }



    self.SaveShHolder = function () {
        if (self.ValidateCompany()) {
            if (self.Validation()) {
                Openloader()
                var shholder = {
                    ShHolderNo: self.ShholderNo(),
                    TotalKitta: self.TotalKitta()
                }

                var shholderTo = {
                    ShHolderNo: self.ShholderNoTo(),
                    TotalKitta: self.TotalKittaTo()
                }
                $.ajax({
                    type: 'POST',
                    url: '/HolderManagement/HolderMergeEntry/SaveHolderForMerge',
                    data: {
                        'CompCode': self.SelectedCompany(),
                        'shholder': shholder,
                        'shHolderForMerge': shholderTo,
                        'SelectedAction': optionAUD,
                        'Remarks': self.MergeRemarks(),
                        'MergeDate': self.MergeDate(),
                        'MergeNo': self.MergeNo()
                    },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            alert('success',result.message)
                        }
                        else {
                            alert('error',result.message)
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                    },
                    complete: () => {
                        self.refresh()
                        Closeloader()
                    }
                })
            }
        }
    }

    self.ShowSignatureModal = function (data) {
        if (data != undefined) {
            if (data == 'From') {
                $('#signatureModalFromTheme').modal('show')

            }
            else {
                $('#signatureModalToTheme').modal('show')

            }
        }
    }

    self.ShowMergeDataModal = function () {
        if (self.ValidateCompany()) {
            Openloader()
            $.ajax({
                type: 'POST',
                url: '/HolderManagement/HolderMergeEntry/GetMergeHolderList',
                data: {
                    'CompCode': self.SelectedCompany()
                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        $('#HoldersList').modal('show')
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new MergeDetail(item)
                        });
                        self.ShHolderMergeList(mappedTasks);
                        
                    } else {
                        alert('error',result.message)
                    }
                }, error: (error) => {
                    alert('error',error.meessage)
                },
                complete: () => {
                    Closeloader()
                }
                
            })
        }
    }

    self.ShowSelectedMergeDetail = function (data) {
        self.MergeNo(data.merge_id());
        $('#ShholderNo,#ShholderNoTo').attr('disabled', optionAUD == "U"? false: true)
        self.ShholderNo(data.holdernofrom());
        self.ShholderNoTo(data.holdernoto());
        self.MergeRemarks(data.merge_remarks());
        $('#HoldersList').modal('hide');
        self.GetSholderInformation(data.holdernofrom,'From')
        self.GetSholderInformation(data.holdernoto,'To')
    }

    self.MergeNo.subscribe(()=> {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.MergeNo())) {
                $.ajax({
                    type: 'POST',
                    url: '/HolderManagement/HolderMergeEntry/GetMergeHolderList',
                    data: {
                        'CompCode': self.SelectedCompany(),
                        'MergeNo':self.MergeNo()
                    }, beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: (result) => {
                        if (result.isSuccess) {
                            $('#ShholderNo,#ShholderNoTo').attr('disabled',optionAUD == "U" ? false:true)
                            self.ShholderNo(result.responseData[0].holdernofrom);
                            self.ShholderNoTo(result.responseData[0].holdernoto);
                            self.MergeRemarks(result.responseData[0].merge_remarks);
                            self.GetSholderInformation(self.ShholderNo, 'From')
                            self.GetSholderInformation(self.ShholderNoTo, 'To')
                        } else {
                            alert('error', result.message)
                        }
                    }, error: (error) => {
                        alert('error', error.meessage)
                    },
                    complete: () => {
                        Closeloader()
                    }

                })
            }
        }
    })
    //self.MergeNo.subscribe(function () {
    //    if (optionAUD != "A") {
    //        if (self.ValidateCompany()) {
    //            if (!Validate.empty(self.MergeNo())) {
    //                Openloader()
    //                $.ajax({
    //                    type: 'POST',
    //                    url: '/HolderManagement/HolderMergeEntry/GetDataFromMergeEntry',
    //                    data: {
    //                        'CompCode': self.SelectedCompany(),
    //                        'MergeId': self.MergeNo()
    //                    },
    //                    success: (result) => {
    //                        if (result.isSuccess) {
    //                            self.ShholderNo(result.responseData.holderfrom)
    //                            self.ShholderNoTo(result.responseData.holderto)
    //                        }
    //                        else {
    //                            alert('error',result.message)
    //                        }
    //                    },
    //                    error: (error) => {

    //                    },
    //                    complete: () => {
    //                        Closeloader()
    //                    }

    //                })
    //            } else {
    //                alert('warning', 'Please Select A Merge Id !!!')
    //            }
    //        }
    //    }
    //})


    //for choosing add or update or delete
    self.chooseOptions = function (data) {
        optionAUD = data;

        if (optionAUD === "A") {
            //self.GetMaxMergeNo();
            $('#saveShholder').val("Save");
            $('#saveShholder').removeClass(["btn-danger", "btn-warning"]);
            $('#saveShholder').addClass("btn-success");
            $('#ShholderNo,#ShholderNoTo,#remarks').attr('disabled', false)
            $('#remarks').focus()

        } else {
            $('#mergeNoDiv').show()
            $('#mergeNo,#mergeNoButton').attr('disabled', false)
            $('#ShholderNo,#ShholderNoTo,#remarks').attr('disabled', true)
            $('#mergeNo').focus()
            if (optionAUD === "U") {

                $('#saveShholder').val("Update");
                $('#remarks').attr("disabled", false);
                $('#saveShholder').removeClass(["btn-danger", "btn-success"]);
                $('#saveShholder').addClass("btn-warning");
            }
            else {
                $('#saveShholder').val("Delete");
                $('#saveShholder').removeClass(["btn-success", "btn-warning"]);
                $('#saveShholder').addClass("btn-danger");
            }

        }
        $('#saveShholder').attr('disabled', false)
        $('#AddButton,#UpdateButton,#DeleteButton').attr('disabled', true)



    }

    self.refresh = function () {
        $('#remarks,#ShholderNo,#ShholderNoTo,#saveShholder,#shHolderToSignature,#shHolderFromSignature').attr('disabled', true)
        $('#AddButton,#UpdateButton,#DeleteButton').attr('disabled', false)
        $('#signatureToDiv,#signatureFromDiv,#mergeNoDiv').hide()
        $('#saveShholder').val("Save");
        $('#saveShholder').removeClass(["btn-danger", "btn-warning"]);
        $('#saveShholder').addClass("btn-success");
        clearAllData()
    }
}

$(document).ready(function () {
    $('#signatureToDiv,#signatureFromDiv,#mergeNoDiv').hide()
    ko.applyBindings(new HolderMergeEntry());
});