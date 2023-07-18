
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
function HolderTitle(data) {
    var self = this;
    self.HolderTitleName = ko.observable(data.HolderTitleName);
}

function HolderNepaliTitle(data) {
    var self = this;
    self.HolderNepaliTitleName = ko.observable(convert_to_unicode_with_value(data.HolderNepaliTitleName));
    self.HolderNepaliName = ko.observable(data.HolderNepaliTitleName);
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
        self.Selected = ko.observable();
    }
}



var ShareHolderPosting = function () {

  
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

    //Holder Title

    self.HolderTitle = ko.observableArray([
        new HolderTitle({ "HolderTitleName": "Mr." }),
        new HolderTitle({ "HolderTitleName": "Ms." }),
        new HolderTitle({ "HolderTitleName": "Mrs." }),
    ])
    self.SelectedHolderTitle = ko.observable()

    self.HolderNepaliTitle = ko.observableArray([
        new HolderNepaliTitle({ "HolderNepaliTitleName": ">L" }), // sri
        new HolderNepaliTitle({ "HolderNepaliTitleName": ">LdtL" }), // srimati
        new HolderNepaliTitle({ "HolderNepaliTitleName": ';">L' }),// susri
        new HolderNepaliTitle({ "HolderNepaliTitleName": 'gfafns' }),//  nabalak
    ])
    self.SelectedHolderNepaliTitle = ko.observable()

    self.PostingRemarks = ko.observable()


    self.ShHolderList = ko.observableArray([])

  
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

    self.GEnName = ko.observable();
    self.GRelation = ko.observable();
    self.DOB = ko.observable();
    self.dateFrom = ko.observable();
    self.dateTo = ko.observable();


    //Add or Update Posting
    self.RecordType = ko.observable()

    // dataTable for the Holder List
    var dataTable;
    var record = [];

    //load Holder List waiting for Posting

    loadDataTable = function () {
        if (self.ValidateCompany()) {
            if (Validate.empty(self.RecordType())) {
                alert('warning', 'Please Select a Selection Type !!!')
            }
            else {
                Openloader()
                var companyCode = self.SelectedCompany()

                $.ajax({
                    type: "post",
                    url: '/HolderManagement/ShareHolderPosting/GetHolderForApproval',
                    data: { 'CompCode': companyCode, 'SelectedAction': self.RecordType(), 'FromDate': self.dateFrom(),'ToDate':self.dateTo() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        $('#tbl_Add_Records_Sholder').DataTable().clear();
                        $('#tbl_Add_Records_Sholder').DataTable().destroy();
                        if (result.isSuccess) {

                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new ShHolder(item)
                            });
                            self.ShHolderList(mappedTasks);
                        } else {
                            alert('warning', "No Data Found ")
                        }
                        self.PostingRemarks('')

                    },
                    error: function (error) {
                        $('#tbl_Add_Records_Sholder').DataTable().clear();
                        $('#tbl_Add_Records_Sholder').DataTable().destroy();
                        alert('error', error.message)
                        self.PostingRemarks('')
                    },
                    complete: () => {

                        $('#tbl_Add_Records_Sholder').DataTable({
                            responsive: true,
                            searching: true,
                            scrollX: true,
                            scrollY: true,
                            paging: true,
                            ordering: true,
                            columnDefs: [
                                { width: 100, targets: [1, 2, 3, 4, 5, 6] }
                            ],
                            fixedColumns: false
                        })
                        self.PostingRemarks('')
                        Closeloader()
                    }
                })
            }
           
        }
    }


    self.SelectAll = ko.computed({
        read: () => !self.ShHolderList().find(x => !x.Selected()),
        write: t => self.ShHolderList().forEach(x => x.Selected(t)),
    })


    //load owner type on form load
    self.LoadShOwnerType = function () {
        if (self.ValidateCompany()) {
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
                    } else {
                        alert('warning', result.Message)
                    }
                },
                error: function (error) {
                    alert('error', result.Message)
                }
            })
        }
    }
    self.LoadShOwnerType();
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


    //load sub owner type funciton
    self.LoadShOwnerSubType = function (data) {
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
                    self.SelectedShOwnerSubType(data)
                } else {
                    alert('warning', result.Message)
                }
            },
            error: function (error) {
                alert('error', result.Message)
            }
        })

    }

    GetSholderInformation = function (data) {
        if (self.ValidateCompany()) {
            $.ajax({
                type: "post",
                url: '/HolderManagement/ShareHolder/GetHolderInformation',
                data: { 'ShHolderNo': data, 'CompCode': self.SelectedCompany() },
                datatype: "json",
                success: function (result) {

                    if (result.isSuccess) {
                        $('#ModalShholderInformation').modal('show')
                        self.ShholderNo(result.responseData.shholderNo)
                        self.FName(result.responseData.fName + ' ' + result.responseData.lName);
                        self.LName(result.responseData.lName);
                        self.Address(result.responseData.address);
                        self.Address1(result.responseData.address1);
                        self.Address2(result.responseData.address2);

                        self.PboxNo(result.responseData.pboxNo);
                        self.NpTitle(result.responseData.NpTitle);
                        self.NpName(convert_to_unicode_with_value(result.responseData.npName));
                        self.NpAdd(convert_to_unicode_with_value(result.responseData.npAdd));
                        self.FaName(result.responseData.faName);
                        self.GrFaName(result.responseData.grFaName);
                        self.HusbandName(result.responseData.husbandName);
                        self.TelNo(result.responseData.telNo);
                        self.MobileNo(result.responseData.mobileNo);
                        self.EmailAdd(result.responseData.emailAdd);
                        self.CitizenshipNo(result.responseData.citizenshipNo);
                        self.Occupation(result.responseData.occupation);
                        self.NomineeName(result.responseData.nomineeName);
                        self.Relation(result.responseData.relation);

                        self.TotalKitta(result.responseData.totalKitta);
                        self.Tfrackitta(result.responseData.tfrackitta);


                        self.SelectedShOwnerType(result.responseData.shownertype)
                        self.LoadShOwnerSubType(result.responseData.shownerSubtype)
                        self.SelectedHolderTitle(result.responseData.title)

                        self.SelectedHolderNepaliTitle(result.responseData.npTitle)
                        //self.SelectedShOwnerSubType(result.responseData.shownerSubtype)
                        self.BankName(result.responseData.bankName);
                        self.AccountNo(result.responseData.accountNo);


                        if (result.responseData.minor) {
                            self.Minor(result.responseData.minor);
                            self.GRelation(result.responseData.aTTMinor.relation)
                        } else {
                            self.DOB('')
                            self.Minor('');
                            self.GRelation('')
                        }

                    } else {
                        alert('warning', result.Message)
                    }
                },
                error: function (error) {
                    alert('error', result.Message)
                }
            })
        }
    }


    self.RecordType.subscribe(function (data) {
        if (data == 'A')
            document.getElementById("btnDelete").style.display = "block";
        else
            document.getElementById("btnDelete").style.display = "none";
        
    })

    

    self.Validation = function () {
        var errMsg = "";
        if (self.SelectedCompany() == undefined) {
            errMsg += "Please Select Company !!!</br>";
        }
        
        if ($('#tbl_Add_Records_Sholder').find('input[type=checkbox]:checked').length == 0) {
            errMsg += "Please Select A Record !!!</br>";
        }
        if (Validate.empty(self.PostingRemarks())) {
            errMsg += "Please Enter Posting Remarks !!!</br>";
        }
        if (Validate.empty(self.PostingDate())) {
            errMsg += "Please Enter Posting Date !!!</br>";
        }
        if (Validate.empty(self.RecordType())) {
            errMsg += "Please Select a Selection Type !!!<br/>";
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }

    }


    self.PostShholderInfo = function (data) {
        if (self.Validation()) {
            Openloader()
            for (var i = 0; i < $('#tbl_Add_Records_Sholder').DataTable().data().count(); i++) {
                var x = i + 1;
                var Check = $($('#tbl_Add_Records_Sholder').DataTable().row(i).nodes()).find('input').prop('checked');
                if (Check != undefined && Check != "" && Check != false) {

                   
                    var DIS = {
                        ShholderNo: $('#tbl_Add_Records_Sholder').DataTable().row(i).data()[2],
                        Remarks: self.PostingRemarks(),
                        ApprovedDate: self.PostingDate(),
                        ActionType: data
                    }

                    record.push(DIS)      
                }
                
            }
            $.ajax({
                type: "POST",
                url: '/HolderManagement/ShareHolderPosting/PostShholderInfo',
                data: { 'aTTShHolder': record, 'CompCode': self.SelectedCompany(),'SelectedRecordType': self.RecordType() },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        alert('success', result.message)
                    }
                    else {
                        alert('error', result.message)                        
                    }
                    self.PostingRemarks('')
                },
                error: function (eror) {
                    alert('error', error.message)
                    self.PostingRemarks('')
                },
                complete: () => {
                    Closeloader()
                    loadDataTable()
                    self.PostingRemarks('')
                    record = []
                }
            })
        }
    }
}
$(document).ready(function () {
    ko.applyBindings(new ShareHolderPosting());
        $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
            autoclose: true,
    });
    
    $("#simple-date1 .input-group.date").datepicker("setDate", new Date());

    $('#simple-date2 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date2 .input-group.date").datepicker("setDate", new Date());
});

