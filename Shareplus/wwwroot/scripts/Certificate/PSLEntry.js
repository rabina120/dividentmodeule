function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)

    }
}

function Certificate(data) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.certNo)
        self.Selected = ko.observable();

    }
}

function CertificateData(data) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.CertNo)
        self.Selected = ko.observable();
        self.SrNoFrom = ko.observable(data.SrNoFrom);
        self.SrNoTo = ko.observable(data.SrNoTo);
        self.ShKitta = ko.observable(data.ShKitta);
        self.CertStatus = ko.observable(data.CertStatus);
        self.ShOwnerType = ko.observable(data.ShOwnerType);
        self.ShholderNo = ko.observable(data.ShholderNo);
        self.Selected = ko.observable();

    }
}
function CertificateInfo(data) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.certNo)
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.ShKitta = ko.observable(data.shKitta);
        self.CertStatus = ko.observable(data.certStatus);
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.FullName = self.FName() + " " + self.LName();
        self.TranDt = ko.observable(data.tranDt)
        self.Remark = ko.observable(data.remark)
        self.total = ko.observable(data.total)
        self.Selected = ko.observable();
        self.ShholderNo = ko.observable(data.shholderNo);
        self.Selected = ko.observable();
    }
}
function CertificateInfo1(data) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.certNo)
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);

        self.ShKitta = ko.observable(data.pledgeKitta);
        self.CertStatus = ko.observable(data.certStatus);
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.FullName = self.FName() + " " + self.LName();
        self.TranDt = ko.observable(data.tranDt)
        self.Remark = ko.observable(data.remark)
        self.Selected = ko.observable();
        self.ShholderNo = ko.observable(data.shholderNo);
        self.Selected = ko.observable();
        self.Pcode = ko.observable(data.pcode)
        self.Name = ko.observable(data.name);
        self.SelectedShareType = ko.observable(data.selectedShareType);

        //self.PledgeAtName = self.Pcode() + " " + self.Name();
        //self.SelectedPledgeAt = ko.observable();
        //self.PledgeAtList = ko.observableArray([])
    }
}

function Certificatelist(data) {
    var self = this;
    if (data != undefined) {
        self.PSLNo = ko.observable(data.pslNo);
        self.ShholderNo = ko.observable(data.shholderNo);
        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.TranDt = ko.observable(data.tranDt)
        self.Remark = ko.observable(data.remark);
        self.Selected = ko.observable();

    }
}
function ShHolder(data) {
    var self = this;
    if (data != undefined) {
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.ShholderNo = ko.observable(data.shholderNo);
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.ShKitta = ko.observable(data.shKitta);
        self.Address = ko.observable(data.shareType);
        self.ShareType = ko.observable(data.shareType);
        self.ShowerType = ko.observable(data.showerType);
        self.CertNo = ko.observable(data.certNo);
        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.Totalkitta = ko.observable(data.totalKitta);
        self.DistCode = ko.observable(data.distCode);
        self.PboxNo = ko.observable(data.pboxNo);
        self.FaName = ko.observable(data.faName);
        self.GrFaName = ko.observable(data.grFaName);
        self.PSLNo = ko.observable(data.pslNo)
        self.telno = ko.observable(data.telno);
        self.FullName = self.FName() + " " + self.LName();
        self.TranDt = ko.observable(data.tranDt)
        self.Remark = ko.observable(data.remark)
        /*  self.Remark = ko.observable(data.PSLNo() + " " + data.PledgeAtList().find(x => x.Pcode() == self.SelectedPledgeAt()).Name())*/
        self.Selected = ko.observable();
        self.Totalpledgekitta = ko.observable(data.pledgekitta)
        self.EffectiveDate = ko.observable(data.tranDt)
        self.Selected = ko.observable();

    }
}

function pledgeAt(data) {
    var self = this;
    if (data != undefined) {
        self.Pcode = ko.observable(data.pcode)
        self.Name = ko.observable(data.name);
        self.PledgeAtName = ko.observable(data.pcode + ' ' + data.name)
    }
}
var PSLEntry = function () {
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.total = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.ShholderNo = ko.observable();
    self.SelectedAction = ko.observable();
    self.FName = ko.observable();
    self.LName = ko.observable();
    self.Address = ko.observable();
    self.DistCode = ko.observable();
    self.PboxNo = ko.observable();
    self.FaName = ko.observable();
    self.GrFaName = ko.observable();
    self.telno = ko.observable();
    self.FullName = ko.observable();
    self.Totalkitta = ko.observable();
    self.PSLEntryCertificate = ko.observableArray([]);
    self.PSLEntryCertificateList = ko.observableArray([]);
    self.PaymentDetails = ko.observableArray([]);
    self.Charge = ko.observable();
    self.EffectiveDate = ko.observable();
    self.PledgeAt = ko.observable();
    self.ChalaniNo = ko.observable();
    self.Totalpledgekitta = ko.observable();
    self.PledgeAmount = ko.observable();
    self.Remark = ko.observable();
    self.CertNo = ko.observable();
    self.SrNoFrom = ko.observable();
    self.SrNoTo = ko.observable();
    self.ShKitta = ko.observable();
    self.CertStatus = ko.observable();
    self.ShOwnerType = ko.observable();
    self.Code = ko.observable();
    self.PSLNo = ko.observable();
    self.optionAUD = ko.observable();
    self.ShholderNo = ko.observable();
    self.firstName = ko.observable()
    self.lastName = ko.observable()
    self.grandFatherName = ko.observable()
    self.fatherName = ko.observable()
    self.Selected = ko.observable();
    self.TranDt = ko.observable();
    self.GetSholderInformation = ko.observable()
    self.ChalaniNo = ko.observable();
    self.ShHolderList = ko.observableArray([])
    self.CertDetList = ko.observableArray([]);
    self.TranType = ko.observableArray();

    self.SelectedShareType = ko.observable();
    self.ShareTypeList = ko.observableArray([

        { ShareType: "2", ShareTypeName: "Pledge" },
        { ShareType: "3", ShareTypeName: "Suspend" },
        { ShareType: "4", ShareTypeName: "Lost" }

    ]);
    self.Pcode = ko.observable()
    self.Name = ko.observable()
    self.PledgeAtName = ko.observable()
    self.SelectedPledgeAt = ko.observable();
    self.PledgeAtList = ko.observableArray([])

    self.EffectiveDate(formatDate(new Date))

    /*self.Remark(PSLNo() + " " + PledgeAtList().find(x => x.Pcode() == SelectedPledgeAt()).Name());*/

    self.GetSholderInformation = function (data) {
        var shholderNo = data.ShholderNo;
        var pslno = data.PSLNo;
        $("#HoldersList").modal('hide');
        $('#tbl_PSLEntryCertificate1').DataTable().clear()
        $('#tbl_PSLEntryCertificate1').DataTable().destroy();
        if (shholderNo != undefined && shholderNo != "") {
            if (self.SelectedAction() == 'D' || self.SelectedAction() == 'E') {
                var SelectedAction = 'D';
                Openloader()
                $.ajax({
                    type: "post",
                    url: '/Certificate/PSLEntry/GetShholderDetailsByShHolderNo',
                    data: { 'CompCode': self.SelectedCompany(), 'ShholderNo': shholderNo, 'SelectedAction': SelectedAction, 'pslno': pslno },

                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        $('#CertDetTable').DataTable().destroy();
                        $('#CertDetTable tbody').empty();
                        $('#chk').prop('checked', false);
                        if (result.isSuccess) {
                            self.FullName(result.responseData[0].fName + " " + result.responseData[0].lName);
                            self.FaName(result.responseData[0].faName);
                            self.GrFaName(result.responseData[0].grFaName);
                            self.Address(result.responseData[0].address);
                            self.Totalkitta(result.responseData[0].totalKitta);
                            self.Pcode(result.responseData[0].pcode);
                            self.ShholderNo(result.responseData[0].shholderNo);
                            self.FName(result.responseData[0].fName);
                            self.LName(result.responseData[0].lName);
                            self.total(result.responseData[0].total);
                            $("#ShareType").attr("disabled", false);
                            self.SelectedShareType(self.ShareTypeList().find(x => x.ShareTypeName == result.responseData[0].certStatus).ShareType);
                            //  self.SelectedShareType(result.responseData[0].selectedShareType);                      
                            //     self.Remark(PSLNo() + " " + PledgeAtList().find(x => x.Pcode() == SelectedPledgeAt()).Name()); 



                            self.Remark(result.responseData[0].remark);
                            self.Charge(result.responseData[0].charge);
                            self.PledgeAmount(result.responseData[0].pledgeAmount);
                            self.TranDt(result.responseData[0].tranDt);
                            self.EffectiveDate(result.responseData[0].tranDt);
                            self.PSLNo(result.responseData[0].pslNo);
                            $("#PledgeAt").attr("disabled", false);
                            self.SelectedPledgeAt(result.responseData[0].pcode);
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new CertificateInfo1(item)
                            });
                            self.PSLEntryCertificate(mappedTasks);
                        } else {

                            alert('warning', result.message)

                        }
                    },
                    error: function (error) {
                        document.getElementById("showCertificate").disabled = false;
                        alert('error', error.message)

                    }, complete: function () {
                        Closeloader()
                    }
                })
            }
            else {
                self.GetCertificateInformation(ko.toJS(data));
            }
        }
    }
    self.GetCertificateInformation = (data) => {
        var shholderNo = data.ShholderNo ? data.ShholderNo : self.ShholderNo()
        $('#tbl_PSLEntryCertificate1').DataTable().clear()
        $('#tbl_PSLEntryCertificate1').DataTable().destroy();
        if (shholderNo != undefined && shholderNo != "") {
            Openloader()
            if (self.SelectedAction() == 'A') {
                var SelectedAction = 'A';
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/Certificate/PSLEntry/GetShholderDetailsByShHolderNo',
                    data: { 'CompCode': self.SelectedCompany(), 'ShholderNo': shholderNo, 'SelectedAction': SelectedAction, 'pslno': data.PSLNo },
                    dataType: 'json',
                    success: (result) => {



                        $('#chk').prop('checked', false);
                        if (result.isSuccess) {

                            if (result.responseData.length > 0) {
                                self.FullName(result.responseData[0].fName + " " + result.responseData[0].lName);
                                self.FaName(result.responseData[0].faName);
                                self.GrFaName(result.responseData[0].grFaName);
                                self.Address(result.responseData[0].address);
                                self.Totalkitta(result.responseData[0].totalKitta);
                                self.PSLNo(result.responseData[0].pslNo);
                                if (self.SelectedPledgeAt() != null) {
                                    document.getElementById("select2-PledgeAt-container").innerHTML = ko.toJS(self.SelectedPledgeAt().PledgeAtName)
                                }
                                /* self.Remark(PSLNo() + " " + PledgeAtList().find(x => x.Pcode() == SelectedPledgeAt()).Name());*/
                                var mappedTasks = $.map(result.responseData, function (item) {
                                    return new CertificateInfo(item)
                                });
                                self.PSLEntryCertificate(mappedTasks);



                            } else {
                                alert('warning', 'No Record Found ');
                            }
                        } else {
                            alert('error', result.message)

                            $('#ShholderNo').attr('disabled', true)
                        }
                    }, error: (error) => {
                        alert('error', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                })

            } else {
                var SelectedAction = 'D';
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/Certificate/PSLEntry/GetShholderDetailsByShHolderNo',
                    data: { 'CompCode': self.SelectedCompany(), 'ShholderNo': shholderNo, 'SelectedAction': SelectedAction, 'pslno': data.PSLNo },
                    dataType: 'json',
                    success: (result) => {
                        $('#chk').prop('checked', false);
                        if (result.isSuccess) {
                            if (result.responseData.length > 0) {
                                $('#HoldersList').modal('show');
                                var mappedTasks = $.map(result.responseData, function (item) {
                                    return new ShHolder(item)
                                });
                                self.ShHolderList(mappedTasks);
                            } else {
                                alert('warning', 'No Record Found ');
                            }
                        } else {
                            alert('error', result.message)

                            $('#ShholderNo').attr('disabled', true)
                        }
                    }, error: (error) => {
                        alert('error', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            }

        }

    }
    self.LoadPledgeAt = function () {

        $.ajax({
            type: "post",
            url: '/Certificate/PSLEntry/GetAllPledgeAt',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new pledgeAt(item)
                    });
                    self.PledgeAtList(mappedTasks);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })


    }
    self.LoadPledgeAt();
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
    self.ChooseOption = function (data) {
        $('#Add,#Edit,#Delete').attr('disabled', true);
        $('#Save,#Cancel').attr('disabled', false);

        self.SelectedAction(data);
        switch (data) {
            case "A":
                $('#ShholderNo').attr('disabled', false);

                break;
            case "D":
                $('#ShholderNo').attr('disabled', false);


                break;
            case "E":
                $('#ShholderNo').attr('disabled', false);


                break;
        }

    };
    self.Search = function () {
        $('#tblSearch').DataTable().clear();
        $('#tblSearch').DataTable().destroy();
        if (!Validate.empty(self.ShholderNo())) {
            if (self.ValidateCompany()) {

                Openloader()
                $.ajax({
                    type: "post",
                    url: '/Certificate/PSLEntry/GetholderinfoBysearch',
                    data: { 'CompCode': self.SelectedCompany(), 'ShholderNo': ShholderNo },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: (data) => {


                        if (data.isSuccess) {
                            if (data.responseData.length > 0) {
                                var mappedTasks = $.map(data.responseData, function (item) {
                                    return new Certificatelist(item)
                                });
                                self.PaymentDetails(mappedTasks);
                            }
                            else {
                                alert('error', 'No Record Found')
                            }
                        } else {
                            alert('error', data.message)
                        }
                    },
                    error: (error) => {
                        alert('error', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }

                })
            }
        }
    };

    self.SelectAll = ko.computed({
        read: () => !self.PSLEntryCertificate().find(x => !x.Selected()),
        write: t => self.PSLEntryCertificate().forEach(x => x.Selected(t)),
    })
    self.SelectedShareType.subscribe((data) => {

        if (data != null) {
            Openloader()
            $.ajax({
                url: '/Certificate/PSLEntry/Getstatus',
                data: { 'Trantype': self.SelectedShareType() },
                /* data: { 'Trantype': self.TranType },*/
                type: 'POST',
                dataType: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (data) => {


                    if (data.isSuccess) {
                        if (data.responseData.length > 0) {

                            self.Charge(data.responseData[0].charge);

                        }
                        else {
                            alert('error', 'No Record Found')

                        }
                    } else {
                        alert('error', data.message)

                    }
                },
                error: (error) => {

                    alert('error', error.message)

                },
                complete: () => {
                    Closeloader()
                }

            })
        }

    });
    self.Insert = function (data) {
        if (self.Validation()) {
            var record = self.PSLEntryCertificateList().filter(y => y.Selected()).map(y => ({
                CertNo: y.CertNo(),

            }));
            Openloader()
            $.ajax({
                type: 'POST',
                datatype: 'json',
                url: '/Certificate/PSLEntry/InsertCertnoInfo',
                data: { 'aTTPSLEntry': record }, beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (data) => {

                    if (data.isSuccess) {
                        if (data.responseData.length > 0) {
                            var mappedTasks = $.map(data.responseData, function (item) {
                                return new CertificateInfo(item)
                            });
                            self.PSLEntryCertificate(mappedTasks);
                            var ar = self.PSLEntryCertificateList();
                            for (var key in record) {
                                var a = "";
                                var i = 0;
                                if (i < record.length) {
                                    a += record[i++] + "<br>";
                                }
                                var value = ko.toJS(self.PSLEntryCertificateList()).find(x => x.CertNo == record[key].CertNo);
                                if (value != undefined) {
                                    var newArray = ko.toJS(self.PSLEntryCertificateList()).filter((item) => item.CertNo !== value.CertNo);
                                    var mappedTasks = $.map(newArray, function (item) {
                                        return new CertificateData(item)
                                    });
                                    self.PSLEntryCertificateList(mappedTasks);
                                }
                            }
                            $('#certificateModalTheme').modal('hide');
                        }
                        else {
                            alert('error', 'No Record Found')
                        }
                    } else {
                        alert('error', data.message)
                    }
                },
                error: (error) => {
                    alert('error', error.message)
                },
                complete: () => {
                    Closeloader()
                }

            })
        }
    };
    var Sum,Sum2 = 0;
    self.checkValidation = function () {

        Sum = 0;
        for (var i = 0; i < $('#tbl_PSLEntryCertificate1').DataTable().data().length; i++) {
            var x = i + 1;
            var Check = $($('#tbl_PSLEntryCertificate1').DataTable().row(i).nodes()).find('input').prop('checked');
            if (Check != undefined && Check != "" && Check != false) {


                Sum += parseInt($('#tbl_PSLEntryCertificate1').DataTable().data()[i][6]);


            }
        }
        self.total(Sum);
    }

    $('#chk').click(() => {
        Sum2 = 0;
        if ($('#tbl_PSLEntryCertificate1').DataTable().data().length > 0) {
            if (self.SelectAll()) {
                self.total(Sum2);
            } else {

                for (var i = 0; i < $('#tbl_PSLEntryCertificate1').DataTable().data().length; i++) {
                    var x = i + 1;
                    Sum2 += parseInt($('#tbl_PSLEntryCertificate1').DataTable().data()[i][6]);
                }
                self.total(Sum2);
            }
        }

    })
    self.LoadCertDet = function () {
        $('#certificateModalTheme').modal('show');
    }
    self.ValidateTable = function () {
        return true;
    }
    self.SavePSLEntry = function () {
        if (self.SaveValidation()) {


            var recordmsg;
            if (self.SelectedAction() == 'A') {
                recordmsg = "You want to save record ?";
            }
            else if (self.SelectedAction() == 'E') {
                recordmsg = "You want to edit record ?";
            }
            else if (self.SelectedAction() == 'D') {

                recordmsg = "You want to delete record ?";
            }
            var recorddata = self.PSLEntryCertificate().filter(x => x.Selected()).map(x => ({
                certNo: x.CertNo(),
                certStatus: x.CertStatus(),
                srNoFrom: x.SrNoFrom(),
                srNoTo: x.SrNoTo(),
                shOwnerType: x.ShOwnerType(),
                shKitta: x.ShKitta(),

            }));
                swal({
                    title: "Are you sure?",
                    text: recordmsg,
                    icon: "warning",
                    buttons: true,
                    dangerMode: true
                }).then((willSave) => {
                    if (willSave) {
                        Openloader()
                        $.ajax({
                            type: 'POST',
                            datatype: 'json', beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            url: '/Certificate/PSLEntry/SavePslBatchEntry',
                            data: {
                                'CompCode': self.SelectedCompany(), "ShholderNo": self.ShholderNo(), "Code": self.SelectedPledgeAt(), "Remark": self.Remark(), "Transdate": self.EffectiveDate(),
                                "SelectedAction": self.SelectedAction(), "Pleggeamount": self.PledgeAmount(), "pslno": self.PSLNo(), "Status": self.SelectedShareType(), 'PSLEntry': recorddata, "charge": self.Charge()
                            },

                            dataType: 'json',

                            success: (result) => {

                                if (result.isSuccess) {
                                    alert('success', result.message);
                                }
                                else {
                                    alert('error', result.message);
                                }

                            },
                            error: (error) => {
                                alert('error', error.message);
                            },
                            failure: (error) => {
                                alert('error', error.message);
                            },
                            complete: () => {
                                Closeloader()
                                self.ClearControl()
                            }
                        }).fail((xhr, status, message) => {
                            alert(status, message);

                        });
                    }
                });
        }


    }
    self.SearchShHolder = function () {
        if (!Validate.empty(self.FName()) || !Validate.empty(self.LName()) || !Validate.empty(self.FaName()) || !Validate.empty(self.GrFaName())) {

            $.ajax({
                type: "post",
                url: '/Certificate/PSLEntry/GetHolderByQuery', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                data: { 'CompCode': self.SelectedCompany(), 'FirstName': self.FName(), 'LastName': self.LName(), 'FatherName': self.FaName(), 'GrandFatherName': self.GrFaName() },

                datatype: 'json',
                success: function (result) {
                    $("#searchQueryModalTheme").modal('hide');
                    if (result.isSuccess) {

                        if (result.responseData.length > 0) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new Certificatelist(item)
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
                }

            })

        }

    }
    self.ClearControl = function () {
        $('#Add,#Edit,#Delete').attr('disabled', false);

        $('#Save,#Cancel').attr('disabled', true);
        $('#ShholderNo,#Sno,#CertificateNo,#SSrNoTo,#SSrNoFrom,#SplitDate,#remarks').attr('disabled', true)

        self.CertNo('')
        self.ShKitta('')
        self.ShholderNo('')
        self.PSLNo('')
        self.FullName('')
        self.FaName('')
        self.GrFaName('')
        self.SrNoFrom('')
        self.SrNoTo('')
        self.Totalkitta('')
        self.Totalpledgekitta('')
        self.Remark('')
        self.Charge('')
        self.EffectiveDate('')
        self.SelectedShareType('')
        self.SelectedPledgeAt('')
        self.ChalaniNo('')
        self.total('')
        self.Address('')

        self.PledgeAmount('')
        $('#tbl_PSLEntryCertificate').DataTable().clear();
        $('#tbl_PSLEntryCertificate').DataTable().destroy();
        $('#tbl_PSLEntryCertificate1').DataTable().clear();
        $('#tbl_PSLEntryCertificate1').DataTable().destroy();
    }
    self.Validation = function () {
        var errMsg = ""

        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company <br>"
        }
        if (Validate.empty(self.SelectedAction())) {
            errMsg += "Please Choose Action <br>"
        }


        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
        }
    } 
    self.SaveValidation = function () {
        var errMsg = ""

        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company <br>"
        }
        if (Validate.empty(self.SelectedAction())) {
            errMsg += "Please Choose Action <br>"
        }
        if (Validate.empty(self.SelectedShareType())) {
            errMsg += "Please Choose A Share Type <br>"
        }
        if (Validate.empty(self.Charge())) {
            errMsg += "Please Enter A Charge <br>"
        }
        if (Validate.empty(self.EffectiveDate())) {
            errMsg += "Please Enter an Effective Date <br>"
        }
        if (Validate.empty(self.SelectedPledgeAt())) {
            errMsg += "Please Choose Pledge Center<br>"
        }
        if (Validate.empty(self.PledgeAmount())) {
            errMsg += "Please Enter Pledge Amount <br>"
        }
        if (Validate.empty(self.Remark())) {
            errMsg += "Please Enter Remark <br>"
        }
        if ($('#tbl_PSLEntryCertificate1').find('input[type=checkbox]:checked').length <= 0) {
            errMsg += "Please Tick A Certificate </br>";
        }

        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
        }
    }



}

    $(function () {

        $('#simple-date1 .input-group.date').datepicker({

            dateFormat: 'yyyy-mm-dd',
            todayBtn: 'linked',
            todayHighlight: true,
            autoclose: true,
            endDate: '+0d',


        });

        $('#Add,#Edit,#Delete').attr('disabled', false);

        $('#Save,#Cancel').attr('disabled', true);
        ko.applyBindings(new PSLEntry())
    })