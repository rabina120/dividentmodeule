function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function DP(data) {
    var self = this;
    if (data != undefined) {
        self.DP_CODE = ko.observable(data.dP_CODE);
        self.DP_NAME = ko.observable(data.dP_NAME);
        self.Dp_Id_cds = ko.observable(data.dp_Id_cds);
        self.DisplayDP = ko.observable(data.dP_NAME + '  ' + data.dP_CODE);
    }
}


function ParaCompChild(data) {
    var self = this;
    if (data != undefined) {
        self.PCCompCode = ko.observable(data.compCode);
        self.PCISIN_NO = ko.observable(data.isiN_NO);
        self.PCShholderNo = ko.observable(data.shholderNo);
        self.PCDesc_share = ko.observable(data.desc_share);
        self.PCShownerType = ko.observable(data.shownerType);
    }
}

function CertificateDetail(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.compcode);
        self.ShHolderNo = ko.observable(data.shHolderNo);
        self.CertNo = ko.observable(data.certNo);
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.CertStatus = ko.observable(data.certStatus);
        self.ShKitta = ko.observable(data.shKitta);
        self.ShOwnerType = ko.observable(data.shOwnerTypeText);
        self.ShareType = ko.observable(data.shareTypeText);
        self.CertStatusText = ko.observable(data.certStatusText);
        self.ShOwnerTypeText = ko.observable(data.shOwnerTypeText);
        self.ShareTypeText = ko.observable(data.shareTypeText);
        self.Selected = ko.observable();
    }

}

function TBLCertificateDetail(data) {
    var self = this;
    if (data != undefined) {
        self.TBLCDcompcode = ko.observable(data.compcode);
        self.TBLCDCert_Id = ko.observable(data.cert_Id);
        self.TBLCDShare_type = ko.observable(data.share_type);
        self.TBLCDDescription = ko.observable(data.description);
        self.TBLCDIssuse_Dt = ko.observable(data.issuse_Dt);
        self.TBLCDStart_SrNo = ko.observable(data.start_SrNo);
        self.TBLCDEnd_SrNo = ko.observable(data.end_SrNo);
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
function ShHolder(data) {
    var self = this;
    if (data != undefined) {

        self.SShholderNo = ko.observable(data.shholderNo);
        self.STitle = ko.observable(data.title);
        self.SFName = ko.observable(data.fName);
        self.SLName = ko.observable(data.lName);
        self.SAddress = ko.observable(data.address);
        self.SDistCode = ko.observable(data.distCode);
        self.SPboxNo = ko.observable(data.pboxNo);
        self.SNpTitle = ko.observable(data.npTitle);
        self.SNpName = ko.observable(data.npName);
        self.SNpAdd = ko.observable(data.npAdd);
        self.SFaName = ko.observable(data.faName);
        self.SGrFaName = ko.observable(data.grFaName);
        self.SHusbandName = ko.observable(data.husbandName);
        self.SOccupation = ko.observable(data.occupation);
        self.STotalKitta = ko.observable(data.totalKitta);
        self.STfrackitta = ko.observable(data.tfrackitta);


    }
}

function DematedCertificate(data) {
    var self = this;
    if (data != undefined) {
        self.DCDemateRegNo = ko.observable(data.demate_regno);
        self.DCShHolderNo = ko.observable(data.shholderno);
        self.DCFName = ko.observable(data.aTTShHolder.fName);
        self.DCLName = ko.observable(data.aTTShHolder.lName);
        self.DCTRDate = ko.observable(data.tr_date != "" ? data.tr_date.slice(0, 10) : "");
        self.DCRemarks = ko.observable(data.remarks);
    }
}




var DeMaterializeEntry = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();


    self.CDSNO = ko.observable();
    self.Description = ko.observable();
    self.Address = ko.observable();
    self.FatherName = ko.observable();
    self.GFatherName = ko.observable();
    self.TotalKitta = ko.observable();
    self.RegNo = ko.observable();
    self.RecID = ko.observable();
    self.DemateType = ko.observable();
    self.DemateReqDate = ko.observable();
    self.DRNNo = ko.observable();
    self.EntryDate = ko.observable();
    self.BoAccountNo = ko.observable();
    self.BoAccountNoFirst = ko.observable();
    self.BoAccountNoLast = ko.observable();
    self.TotalDemateKitta = ko.observable();
    self.Remarks = ko.observable();
    self.ShareHolderNo = ko.observable();
    self.HolderName = ko.observable();
    //ParaCOmpChild ko lagi
    self.PCCompCode = ko.observable();
    self.PCISIN_NO = ko.observable();
    self.PCShholderNo = ko.observable();
    self.PCDesc_share = ko.observable();
    self.PCShownerType = ko.observable();
    self.SelectedISINNO = ko.observable();
    self.ParaCompChildList = ko.observableArray([]);


    //DPLISt

    self.DP_CODE = ko.observable();
    self.DP_NAME = ko.observable();
    self.Dp_Id_cds = ko.observable();
    self.SelectedDP = ko.observable();
    self.DisplayDP = ko.observable();
    self.DPList = ko.observableArray([]);


    //Certificate Details

    self.compcode = ko.observable();
    self.ShHolderNo = ko.observable();
    self.CertNo = ko.observable();
    self.SrNoFrom = ko.observable();
    self.SrNoTo = ko.observable();
    self.CertStatus = ko.observable();
    self.ShKitta = ko.observable();
    self.ShOwnerType = ko.observable();
    self.ShareType = ko.observable();
    self.CertificateList = ko.observableArray([]);


    //certificalte dwetail table
    self.TBLCDcompcode = ko.observable();
    self.TBLCDCert_Id = ko.observable();
    self.TBLCDShare_type = ko.observable();
    self.TBLCDDescription = ko.observable();
    self.TBLCDIssuse_Dt = ko.observable();
    self.TBLCDStart_SrNo = ko.observable();
    self.TBLCDEnd_SrNo = ko.observable();
    self.SelectedBonusIssue = ko.observable();
    self.TBLCertificateDetailList = ko.observableArray([]);

    //Signature ko lagi 

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
    self.ScanedBy = ko.observable();
    self.PassProcted = ko.observable();
    self.FileNameSign = ko.observable();

    //shholder list 
    self.SShholderNo = ko.observable();
    self.STitle = ko.observable();
    self.SFName = ko.observable();
    self.SLName = ko.observable();
    self.SAddress = ko.observable();
    self.SDistCode = ko.observable();
    self.SPboxNo = ko.observable();
    self.SNpTitle = ko.observable();
    self.SNpName = ko.observable();
    self.SNpAdd = ko.observable();
    self.SFaName = ko.observable();
    self.SGrFaName = ko.observable();
    self.SHusbandName = ko.observable();
    self.SOccupation = ko.observable();
    self.STotalKitta = ko.observable();
    self.STfrackitta = ko.observable();
    self.ShHolderList = ko.observableArray([]);
    self.GetSholderInformation = ko.observable();
    self.SearchShHolder = ko.observable();
    //for searching
    self.firstName = ko.observable();

    //tbl certificate demate for edit and delete
    self.DCDemateRegNo = ko.observable();
    self.DCShHolderNo = ko.observable();
    self.DCFName = ko.observable();
    self.DCLName = ko.observable();
    self.DCTRDate = ko.observable();
    self.DCRemarks = ko.observable();
    self.DCDematedCertificateList = ko.observable();
    self.GetDematedCertificateInformation = ko.observable();

    //variables

    record = [];
    var totalPledgeKitta = 0;
    var startSrNo;
    var endSrNo;

    self.SelectedAction = ko.observable()



    self.Validation = function (data) {
        var errMsg = "";
        if (Validate.empty(self.ShareHolderNo())) {
            errMsg += "Please Select a ShareHolder No !!! <br/>";
        }
        if (Validate.empty(self.SelectedISINNO())) {
            errMsg += "Please Select ISIN No !!!<br/>";
        }
        if (Validate.empty(self.DemateType())) {
            errMsg += "Please Select Demate Type !!! <br/>";
        }
        if (data == 'S') {
            if (Validate.empty(self.DRNNo())) {
                errMsg += "Please Enter a DRN No. !!! <br/>";
            }
            if (Validate.empty(self.SelectedDP())) {
                errMsg += "Please Select a DP !!!<br/>";
            }
            if (Validate.empty(self.DemateReqDate())) {
                errMsg += "Please Enter Demate Request Date !!! <br/>";
            }
            if (Validate.empty(self.EntryDate())) {
                errMsg += "Please Enter Entry Date !!! <br/>";
            }
            if (Validate.empty(self.BoAccountNoLast())) {
                errMsg += "Please Enter BOID !!! <br/>";
            }
            if (self.DemateType() == '2') {
                if (Validate.empty(self.SelectedBonusIssue())) {
                    errMsg += "Please Select Certificate Issue Type !!! <br/>";
                }
            }

            if ($('#tbl_certificate_list').find('input[type=checkbox]:checked').length <= 0) {
                errMsg += "Please Tick A Certificate To Dematerialize !!!</br>";
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

    self.GetBonusIssueList = function () {
        if (self.ValidateCompany()) {
            var companyCode = self.SelectedCompany();
            $.ajax({
                type: 'POST',
                url: '/CDS/DematerializeEntry/GetDataFromCertificateDetail',
                data: { 'CompCode': companyCode },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                dataType: 'json',
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new TBLCertificateDetail(item);
                        });
                        self.TBLCertificateDetailList(mappedTasks);
                    }
                    else {
                        alert('error', result.message);
                    }

                }, error: function (error) {
                    alert('error', error.message);
                }
            })
        }

    }
    self.GetBonusIssueList();


    var d = new Date(),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    var entryDate = [year, month, day].join('-');



    self.GetDP = function () {
        $.ajax({
            type: 'POST',
            url: '/CDS/DematerializeEntry/GETDP',
         
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            dataType: 'json',
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new DP(item)
                    });
                    self.DPList(mappedTasks);
                }
                else {
                    alert('error', result.message);
                }
            }, error: function (error) {
                alert('error', error.message);
            }


        })
    }
    self.GetDP();
    
    self.GetAllParacompChild = () => {
        if (!Validate.empty(self.SelectedCompany())) {
            var companyCode = self.SelectedCompany()
            $.ajax({
                type: "post",
                url: '/CDS/DeMaterializeEntry/GetAllParaCompChild',
                data: { 'CompCode': companyCode },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new ParaCompChild(item)
                        });
                        self.ParaCompChildList(mappedTasks);

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
    self.SelectedCompany.subscribe(function () {
        
        self.GetAllParacompChild();
        self.GetBonusIssueList();

    })
    self.SelectedISINNO.subscribe(function () {
        if (self.SelectedISINNO() != undefined) {
            if (self.SelectedISINNO() != '') {
                self.PCShownerType(self.ParaCompChildList().find(x => x.PCISIN_NO() == self.SelectedISINNO()).PCShownerType());
                self.PCDesc_share(self.ParaCompChildList().find(x => x.PCISIN_NO() == self.SelectedISINNO()).PCDesc_share());
                self.Description(self.PCShownerType() + ' ' + self.PCDesc_share());
                self.CDSNO(self.ParaCompChildList().find(x => x.PCISIN_NO() == self.SelectedISINNO()).PCShholderNo());

                $('#holderNo').attr('disabled', false);
                $('#SearchButton').attr('disabled', false);
                $('#holderNo').focus();

                self.ClearControl('ISIN');
            }
        }
    });

    self.SearchData = function () {
        if (!Validate.empty(self.ShareHolderNo()))
            if (ValidateCompany()) {
                if (!Validate.empty(self.SelectedISINNO())) {
                    Openloader()
                    var companyCode = self.SelectedCompany()

                    $.ajax({
                        type: "post",
                        url: '/CDS/DeMaterializeEntry/GetShHolderInformation',
                        data: {'CompCode': companyCode, 'Occupation': self.PCShownerType() == 3 ? 1 : 2, 'ShHolderNo': self.ShareHolderNo() },
                        datatype: "json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (result) {
                            if (result.isSuccess) {

                                if (self.SelectedAction() == "A") {
                                    self.HolderName(result.responseData.fName + ' ' + result.responseData.lName);
                                    self.Address(result.responseData.address);
                                    self.FatherName(result.responseData.faName);
                                    self.GFatherName(result.responseData.grFaName);
                                    self.TotalKitta(result.responseData.totalKitta);
                                    self.DemateType('1');
                                    $('#showSignature').css("visibility", "visible");
                                    $('#REQ,#CA,#GetCertificate').attr('disabled', false);


                                    //$.ajax({
                                    //    type: "post",
                                    //    url: "/CDS/DematerializeEntry/GetMaxRegNo",
                                    //    data: { 'CompCode': companyCode },
                                    //    datatype: "json",
                                    //    success: function (result) {
                                    //        if (result.isSuccess) {
                                    //            self.RegNo(result.responseData);
                                    //        }
                                    //        else {
                                    //            alert('error', 'Cannot Generate New Reg No.');
                                    //        }
                                    //    }, error: function (error) {
                                    //        alert('error', error.message);
                                    //    }
                                    //});
                                }
                                else if (self.SelectedAction() == "U" || self.SelectedAction() == "D") {
                                    if (self.SelectedAction() == "U") {
                                        $('#Save').val('Update');

                                        $('#Save').removeClass('btn-success btn-danger');
                                        $('#Save').addClass('btn-warning');
                                    }
                                    else {
                                        $('#Save').val('Delete');
                                        $('#Save').removeClass('btn-warning btn-success');
                                        $('#Save').addClass('btn-danger');
                                    }
                                    $.ajax({
                                        type: 'POST',
                                        url: '/CDS/DematerializeEntry/GetDematedCertificateList',
                                        data: { 'CompCode': companyCode, 'HolderNo': self.ShareHolderNo() },
                                        beforeSend: function (xhr) {
                                            xhr.setRequestHeader("XSRF-TOKEN",
                                                $('input:hidden[name="__RequestVerificationToken"]').val());
                                        },
                                        dataType: 'json',
                                        success: function (result2) {
                                            if (result2.isSuccess) {
                                                var mappedTasks = $.map(result2.responseData, function (item) {
                                                    return new DematedCertificate(item)
                                                });
                                                self.DCDematedCertificateList(mappedTasks);
                                                self.HolderName(result.responseData.fName + ' ' + result.responseData.lName);
                                                self.Address(result.responseData.address);
                                                self.FatherName(result.responseData.faName);
                                                self.GFatherName(result.responseData.grFaName);
                                                self.TotalKitta(result.responseData.totalKitta);
                                                self.EntryDate(entryDate);

                                                $("#Demated_Certitificate_Table").modal('show');
                                                $("#REQ,#CA").attr('disabled', true);
                                            } else {
                                                alert('error', result2.message);
                                            }
                                        }, error: function (error) {
                                            alert('error', error.message);

                                        }, complete: () => {
                                            Closeloader()
                                        }
                                    })
                                }

                            } else {
                                alert('warning', result.message)
                            }
                        },
                        error: function (error) {
                            alert('error', error.message)
                        }, complete: () => {
                            if (self.SelectedAction() == 'A') {
                                Closeloader()
                            }
                        }
                    })
                }

            }
    }

    self.GetCertificateList = function () {
        if (self.ValidateCompany()) {
            $('#tbl_certificate_list').DataTable().clear();
            $('#tbl_certificate_list').DataTable().destroy();

            if (self.DemateType() == '2' && Validate.empty(self.SelectedBonusIssue())) {
                alert('error', 'Please Select a Certificate Detail !!! <br/>');
            }
            else {
                if (self.Validation(self.SelectedAction())) {


                    Openloader()

                    var companyCode = self.SelectedCompany();
                    if (self.DemateType() == "1") {
                        startSrNo;
                        endSrNo;
                        $.ajax({
                            type: 'POST',
                            url: '/CDS/DematerializeEntry/GetCertificateDetails',
                            data: { 'CompCode': companyCode, 'DemateType': self.DemateType(), 'HolderNo': self.ShareHolderNo(), 'ShOwnerType': self.PCShownerType(), 'SrNoFrom': startSrNo, 'SrNoTo': endSrNo },
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            dataType: 'json',
                            success: function (result) {
                                if (result.isSuccess) {
                                    $('#DRNNo,#DemateReqFrom,#DemateReqDate,#DPList').attr('disabled', false);
                                    $('#REQ,#CA').attr('disabled', true);

                                    var mappedTasks = $.map(result.responseData, function (item) {
                                        return new CertificateDetail(item);
                                    })
                                    self.CertificateList(mappedTasks);
                                    self.EntryDate(entryDate);
                                    $('#DRNNo').attr('disabled', self.DemateType() == '2' ? true : false);
                                    if (self.DemateType() == '2') self.DRNNo(0)
                                    $('#tbl_certificate_list').DataTable({
                                        responsive: true,
                                        searching: true,
                                        scrollX: true,
                                        scrollY: true,
                                        scrollCollapse: true,
                                        paging: true,
                                        ordering: false,
                                        fixedHeader: true,
                                        "scrollY": "650px",
                                        "sScrollX": "100%",
                                        "scrollCollapse": true

                                    });


                                } else {
                                    console.log('error=>', result.message);
                                    alert('error', result.message);
                                }
                            },
                            error: function (error) {
                                alert('error', error.message);
                                console.log('error=>', error.message);
                            }, complete: () => {

                                Closeloader()


                            }
                        })
                    }
                    else {
                        var companyCode = self.SelectedCompany();

                        $.ajax({
                            type: 'POST',
                            url: '/CDS/DematerializeEntry/GetStartSrNoEndSrNo',
                            data: { 'CompCode': companyCode, 'BonusIssueCode': self.SelectedBonusIssue(), 'Description': '' },
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            dataType: 'json',
                            success: function (resultSerial) {
                                if (resultSerial.isSuccess) {

                                    startSrNo = resultSerial.responseData[0].srNoFrom;
                                    endSrNo = resultSerial.responseData[0].srNoTo;

                                    $.ajax({
                                        type: 'POST',
                                        url: '/CDS/DematerializeEntry/GetCertificateDetails',
                                        data: { 'CompCode': companyCode, 'DemateType': self.DemateType(), 'HolderNo': self.ShareHolderNo(), 'ShOwnerType': self.PCShownerType(), 'SrNoFrom': startSrNo, 'SrNoTo': endSrNo },
                                        beforeSend: function (xhr) {
                                            xhr.setRequestHeader("XSRF-TOKEN",
                                                $('input:hidden[name="__RequestVerificationToken"]').val());
                                        },
                                        dataType: 'json',
                                        success: function (result) {
                                            if (result.isSuccess) {

                                                $('#DRNNo,#DemateReqFrom,#DemateReqDate,#DPList').attr('disabled', false);
                                                $('#REQ,#CA').attr('disabled', true);

                                                var mappedTasks = $.map(result.responseData, function (item) {
                                                    return new CertificateDetail(item);
                                                })
                                                self.CertificateList(mappedTasks);
                                                self.EntryDate(entryDate);
                                                $('#DRNNo').attr('disabled', self.DemateType() == '2' ? true : false);
                                                if (self.DemateType() == '2') self.DRNNo(0)
                                                $('#tbl_certificate_list').DataTable({
                                                    responsive: true,
                                                    searching: true,
                                                    scrollX: true,
                                                    scrollY: true,
                                                    scrollCollapse: true,
                                                    paging: true,
                                                    ordering: false,
                                                    fixedHeader: true,
                                                    "scrollY": "650px",
                                                    "sScrollX": "100%",
                                                    "scrollCollapse": true

                                                });


                                            } else {
                                                console.log('error=>', result.message);
                                                alert('error', result.message);
                                            }
                                        },
                                        error: function (error) {
                                            alert('error', error.message);
                                            console.log('error=>', error.message);
                                        }, complete: () => {




                                        }
                                    })

                                }
                            }, error: function (errorSerial) {
                                alert('error', errorSerial.message)
                                console.log('error =>', errorSerial.message)
                            }, complete: () => {
                                Closeloader()

                            }
                        });
                    }






                }
            }
        }
    }

    self.DemateType.subscribe(function () {
        self.Remarks(self.DemateType() == '1' ? 'DR' : 'CR');
        $('#BonusIssueSelect').attr('disabled', self.DemateType() == '1' ? true : false);
        self.SelectedBonusIssue('')
        $('#BonusIssueSelect').val("").trigger('change');


    })

    self.SelectedDP.subscribe(function () {
        if (!Validate.empty(self.SelectedDP())) {
            self.BoAccountNoFirst(self.DPList().find(x => x.DP_CODE() == self.SelectedDP()).Dp_Id_cds())
            $('#BoAccountNoLast').attr('disabled', false);
        }
        else {
            self.BoAccountNoFirst("");

            $('#BoAccountNoLast').attr('disabled', true);

        }
    });

    self.MakeBOID = function () {
        if (!Validate.empty(self.BoAccountNoFirst()) && !Validate.empty(self.BoAccountNoLast())) {
            var boid = self.BoAccountNoLast();
            boid = boid.padStart(8, '0');
            self.BoAccountNoLast(boid);
        }
    }


    self.SelectAll = ko.computed({
        read: () => !self.CertificateList().find(x => !x.Selected()),
        write: t => self.CertificateList().forEach(x => x.Selected(t))
    })

    $('#chk').click(() => {
        if ($('#tbl_certificate_list').DataTable().data().length > 0) {
            totalPledgeKitta2 = 0;
            for (var i = 0; i < $('#tbl_certificate_list').DataTable().data().length; i++) {
                var x = i + 1;
                totalPledgeKitta2 += parseInt($('#tbl_certificate_list').DataTable().data()[i][4]);
            }
            self.TotalDemateKitta(totalPledgeKitta2);
        }
    })


    CalculatePledgeKitta = function () {
        totalPledgeKitta = 0;
        for (var i = 0; i < $('#tbl_certificate_list').DataTable().data().length; i++) {
            var x = i + 1;
            var Check = $($('#tbl_certificate_list').DataTable().row(i).nodes()).find('input').prop('checked');
            if (Check != undefined && Check != "" && Check != false) {


                totalPledgeKitta += parseInt($('#tbl_certificate_list').DataTable().data()[i][4]);


            }
        }
        self.TotalDemateKitta(totalPledgeKitta);
    }

    self.ChooseOption = function (data) {
        $('#Add,#Edit,#Delete').attr('disabled', true);
        $('#Save,#Cancel,#ISINNO').attr('disabled', false);

        self.SelectedAction(data);
        switch (data) {
            case "A":
                $('#wIssued,#payCenterSelect,#telno,#remarks,#RegNo').attr('disabled', false);
                $('#GetCertificate').css('visibility', 'visible');
                $('#Save').removeClass('btn-warning btn-danger');
                $('#Save').addClass('btn-success');

                //self.GetMaxDemateRegNo();
                break;
            case "U":
                $('#wIssued,#payCenterSelect,#telno,#remarks').attr('disabled', false);
                $('#RegNo').attr('disabled', true);
                $('#GetCertificate').css('visibility', 'hidden');

                $('#Save').val('Update');
                $('#Save').removeClass('btn-success btn-danger');
                $('#Save').addClass('btn-warning');


                break;
            case "D":
                $('#wIssued,#payCenterSelect,#telno,#remarks,#RegNo').attr('disabled', true);
                $('#GetCertificate').css('visibility', 'hidden');

                $('#Save').val('Delete');
                $('#Save').removeClass('btn-warning btn-success');
                $('#Save').addClass('btn-danger');


                break;
        }



    };

    //self.GetMaxDemateRegNo = function () {
    //    if (self.ValidateCompany()) {
    //        var companyCode = self.SelectedCompany();
    //        $.ajax({
    //            type: 'POST',
    //            url: '/CDS/DematerializeEntry/GetMaxDemateRegNo',
    //            data: { 'CompCode': companyCode },
    //            dataType: 'json',
    //            success: function (result) {
    //                if (result.isSuccess) {
    //                    self.RecID(result.responseData);
    //                }
    //                else {
    //                    alert('error', 'Cannot Get New Reg NO.');
    //                }
    //            }, error: function (error) {
    //                alert('error', error.message)
    //            }
    //        })
    //    }
    //}

    self.SaveDeMaterializeData = function (data) {
        if (self.ValidateCompany()) {
            if (self.Validation(data)) {
                $('#Save,#Cancel').attr('disabled', true);
                var companyCode = self.SelectedCompany();
                for (var i = 0; i < $('#tbl_certificate_list').DataTable().data().length; i++) {
                    var x = i + 1;
                    var Check = $($('#tbl_certificate_list').DataTable().row(i).nodes()).find('input').prop('checked');
                    if (Check != undefined && Check != "" && Check != false) {
                        var Data = {
                            CertNo: $('#tbl_certificate_list').DataTable().data()[i][1],
                            CertStatus: '1',
                            SrNoFrom: $('#tbl_certificate_list').DataTable().data()[i][2],
                            SrNoTo: $('#tbl_certificate_list').DataTable().data()[i][3],
                            ShKitta: $('#tbl_certificate_list').DataTable().data()[i][4],
                            ShareType: $('#tbl_certificate_list').DataTable().data()[i][5],
                            ShOwnerType: $('#tbl_certificate_list').DataTable().data()[i][6],
                        }
                        record.push(Data)
                    }
                }
                var text, title;

                if (SelectedAction() == "A") {
                    text = "Certificates Will Be Dematerialized .";
                    title = "Dematerialize Certificates ?"
                }
                else if (SelectedAction() == "U") {
                    text = "Dematerialized Certificates Will Be Updated with Rec ID: " + self.RecID();
                    title = "Updated Dematerialized Certificate?"
                }
                else {
                    text = "Dematerialized Certificates With Rec ID: " + self.RecID() + " Will Be Deleted";
                    title = "Deleted Dematerialized Certificates ?"
                }
                swal({
                    title: title,
                    text: text,
                    icon: "success",
                    buttons: true,
                    dangerMode: true
                }).then((willSave) => {
                    if (willSave) {
                        Openloader()

                        $.ajax({
                            type: 'POST',
                            url: '/CDS/DematerializeEntry/SaveDematerializeCertificate',
                            data: {
                                
                                'CertificateList': record, 'CompCode': companyCode, 'DemateRegNo': self.RecID(),
                                'ShHolderNo': self.ShareHolderNo(), 'EntryDate': self.EntryDate(),
                                'DemateReqDate': self.DemateReqDate(), 'BOID': self.BoAccountNoFirst() + self.BoAccountNoLast(),
                                'DrnNo': self.DRNNo(), 'DPCode': self.SelectedDP(), 'Remarks': self.Remarks(), 'RegNO': self.RegNo(),
                                'ISINNo': self.SelectedISINNO(), 'BonusCode': self.DemateType() == '1' ? "" : (self.TBLCertificateDetailList().find(x => x.TBLCDCert_Id() == self.SelectedBonusIssue()).TBLCDDescription())
                                , 'SelectedAction': self.SelectedAction()
                            },
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            dataType: 'json',
                            success: function (result) {
                                if (result.isSuccess) {
                                    //alert('Success', 'Certificates Have Been Dematerialized with Rec ID: ' + self.RecID());
                                    var text, title;

                                    if (SelectedAction() == "A") {
                                        text = result.message;
                                        title = "Do you want to Dematerialize more certificates ?"
                                    }
                                    else if (SelectedAction() == "U") {
                                        text = "Dematerialized Certificates Have Been Updated with Rec ID: " + self.RecID();
                                        title = "Updated Dematerialized Certificate !!!"

                                    }
                                    else {
                                        text = "Dematerialized Certificates With Rec ID: " + self.RecID() + " Have Been Deleted";
                                        title = "Deleted Dematerialized Certificates !!!"
                                    }


                                    swal({
                                        title: title,
                                        text: text,
                                        icon: "info",
                                        button: "Ok",
                                    }).then(function (result) {
                                        self.ClearControl('Clear');
                                        location.reload();

                                    });

                                }
                                else {
                                    alert('error', result.message);
                                }
                            }, error: function (error) {
                                alert('error', error.message);
                            },
                            complete: () => {

                                $('#Save,#Cancel').attr('disabled', false);
                                Closeloader()


                            }
                        })

                    }
                    else {
                        $('#Save,#Cancel').attr('disabled', false);

                    }

                });



            }
        }
    }

    // to serach with Modal
    self.SearchShHolder = function () {
        if (!Validate.empty(self.firstName())) {
            Openloader()

            $.ajax({
                url: '/CDS/DematerializeEntry/GetHolderByQuery',
                data: { 'CompCode': self.SelectedCompany(), 'FirstName': self.firstName(), 'Occupation': self.PCShownerType() == 3 ? 1 : 2 },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                type: 'POST',
                datatype: 'json',
                success: function (result) {
                    $("#searchQueryModalTheme").modal('hide');
                    if (result.isSuccess) {

                        if (result.responseData.length > 0) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new ShHolder(item)
                            });
                            self.ShHolderList(mappedTasks);
                            self.EntryDate(entryDate);
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
                },
                complete: () => {
                    Closeloader()

                }

            })

        }

    }

    self.GetSholderInformation = function (data) {
        var shholderNo = data.SShholderNo ? data.SShholderNo : self.ShareHolderNo()
        $("#HoldersList").modal('hide');
        self.ShareHolderNo(ko.toJS(shholderNo));
        self.SearchData();
    }


    //EDIT RA DELETE MODAL CLICK GARE PAXI
    self.GetDematedCertificateInformation = function (data) {
        var mpslno = data.DCDemateRegNo();
        var companyCode = self.SelectedCompany();
        Openloader()
        $('#tbl_certificate_list').DataTable().clear();
        $('#tbl_certificate_list').DataTable().destroy();
        $.ajax({
            type: 'POST',
            url: '/CDS/DematerializeEntry/GetDematedCertificateDetails',
            data: {
                
                'CompCode': companyCode,
                'DemateRegNo': mpslno
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },

            dataType: 'json',
            success: function (result) {
                if (result.isSuccess) {
                    $("#Demated_Certitificate_Table").modal('hide');

                    $('#DRNNo,#DemateReqFrom,#DemateReqDate,#DPList,#BonusIssueSelect').attr('disabled', false);

                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new CertificateDetail(item);
                    })
                    self.CertificateList(mappedTasks);
                    self.SelectAll(true);

                    self.RecID(result.responseData[0].demate_regno);
                    self.RegNo(result.responseData[0].regno);
                    self.DemateReqDate(result.responseData[0].tr_date.slice(0, 10));
                    self.Remarks(result.responseData[0].remarks || null);
                    self.SelectedDP(self.DPList().find(x => x.Dp_Id_cds() == result.responseData[0].bo_acct_no.slice(0, 8)).DP_CODE());
                    $('#DPList').val(self.DPList().find(x => x.Dp_Id_cds() == result.responseData[0].bo_acct_no.slice(0, 8)).DP_CODE()).trigger('change');
                    self.BoAccountNoFirst(result.responseData[0].bo_acct_no.slice(0, 8));
                    self.BoAccountNoLast(result.responseData[0].bo_acct_no.slice(8, 16))
                    self.DemateType(result.responseData[0].remarks == "CR" || null ? '2' : '1');
                    if (self.DemateType() == "2") {
                        $('#DRNNo').attr('disabled', true);
                        $('#BonusIssueSelect').val(self.TBLCertificateDetailList().find(x => x.TBLCDDescription() == result.responseData[0].bonuscode).TBLCDCert_Id()).trigger('change');
                    }

                    //if (self.SelectedAction() != "D") {
                    //    $('#DRNNo').attr('disabled', self.DemateType() == '2' ? true : false);
                    //} else {
                    //    $('#BonusIssueSelect').val(self.DPList().find(x => x.TBLCDDescription() == result.responseData[0].description).TBLCDCert_Id()).trigger('change');

                    //}
                    self.DRNNo(result.responseData[0].drn_no || null);
                    self.EntryDate(entryDate);
                    $('#BonusIssueSelect').attr('disabled', true)
                    if (self.SelectedAction() == "D") {

                        $("#tbl_certificate_list_tbl").find("*").attr("disabled", "disabled");
                        $('#DRNNo,#DPList,#DemateReqDate,#BoAccountNoLast').attr('disabled', true);

                    }
                }
                else {
                    alert('error', result.message)
                    console.log('error =>', result.message)
                }


            },
            error: function (error) {
                alert('error', error.message);


            }, complete: function () {
                Closeloader()
            }
        })
    }

    //for showing signature in signature modal
    self.GetSignature = function () {
        var showSignValidate = true;
        if (Validate.empty(self.ShareHolderNo())) {
            toastr.error("Please Type a Sholder No.");
            showSignValidate = false;
        }
        if (!self.ValidateCompany()) {
            toastr.error("Please Select a Company.");
            showSignValidate = false;
        }
        if (showSignValidate) {
            var companyCode = self.SelectedCompany();
            document.getElementById("showSignature").disabled = true;

            $.ajax({
                type: "post",
                url: '/CDS/DematerializeEntry/GetSignature',
                data: { 'HolderNo': self.ShareHolderNo(), 'CompCode': companyCode },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
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

                }
            });
        }
    }



    self.ClearControl = function (data) {
        $('#REQ,#CA,#DRNNo,#DemateReqFrom,#DemateReqDate,#DPList,#BonusIssueSelect,#GetCertificate').attr('disabled', true);
        $('#showSignature').css("visibility", "hidden");

        if (data == "Clear") {
            self.SelectedISINNO('');
            $('#Add,#Edit,#Delete').attr('disabled', false);
            $('#ISINNO,#holderNo,#Save,#Cancel').attr('disabled', true);
            self.CDSNO('');
            self.Description('');
            self.RecID('');
            self.RegNo('');

        }
        self.ShareHolderNo('');
        self.HolderName('');
        self.Address('');
        self.FatherName('');
        self.GFatherName('');
        self.TotalKitta('');
        self.DemateType('');
        self.DRNNo('');
        self.SelectedDP('');
        self.DemateReqDate('');
        self.EntryDate('');
        self.BoAccountNoFirst('');
        self.BoAccountNoLast('');
        self.TotalDemateKitta('');
        self.Remarks('');
        self.SelectedBonusIssue('');



        $('#tbl_certificate_list').DataTable().clear();
        $('#tbl_certificate_list').DataTable().destroy();
        $('#tbl_certificate_list').DataTable({
            responsive: false,
            searching: false,
            scrollX: true,
            scrollY: true,
            paging: false,
            ordering: false,
            fixedHeader: true
        });
        $('#DPList').val('').trigger('change');


        $('#Save').removeClass('btn-warning btn-danger');
        $('#Save').addClass('btn-success');
        $('#Save').val('Save');

        $('#BonusIssueSelect').attr('disabled', true);

    }

}
$(function () {


    $('#simple-date1 .input-group.date').datepicker({
        "setDate": new Date(),
        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date1 .input-group.date").datepicker("setDate", new Date());
    $('#simple-date2 .input-group.date').datepicker({
        "setDate": new Date(),
        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date2 .input-group.date").datepicker("setDate", new Date());

    $('#tbl_certificate_list').DataTable({
        responsive: true,
        searching: true,
        scrollX: true,
        scrollY: true,
        scrollCollapse: true,
        paging: true,
        ordering: false,
        fixedHeader: true,
        "sScrollX": "100%",
        "scrollCollapse": true

    });

    $('#Add,#Edit,#Delete').attr('disabled', false);
    $('#showSignature,#GetCertificate').css("visibility", "hidden");

    $('#Save,#Cancel').attr('disabled', true);
    ko.applyBindings(new DeMaterializeEntry())
})