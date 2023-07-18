function ParaComp(data) {

    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compCode + " " + data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function ShholderPSL(data) {
    var self = this;
    if (data != undefined) {
        self.PSLNo = ko.observable(data.pslNo);
        self.ShholderNo = ko.observable(data.shholderNo);
        self.Name = ko.observable(data.fName + ' ' + data.lName);
        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.FaName = ko.observable(data.faName);
        self.GrFaName = ko.observable(data.grFaName);
        self.TranDt = ko.observable(data.tranDt == null ? "" : data.tranDt.substring(0, 10));
        self.remark = ko.observable(data.remark);
        self.Selected = ko.observable();
        self.CertNo = ko.observable(data.certNo);
        self.SeqNo = ko.observable(data.seqNo);
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.TranType = ko.observable(data.tranType);
        self.PledgeCenter = ko.observable(data.pledgeCenter);
        /*self.PSL_clear_No = ko.observable(data.pSL_clear_No);*/
        self.PSL_clear_No = ko.observable(data.psL_clear_No);
        self.ChalaniNo = ko.observable(data.chalaniNo);

        self.CertNo = ko.observable(data.certNo);
        self.PledgeKitta = ko.observable(data.pledgeKitta);
        self.PledgeAmount = ko.observable(data.pledgeAmount);
        self.Charge = ko.observable(data.charge);
    }
}
function CertificateInsertData(data) {
    var self = this;
    if (data != undefined) {

        self.Selected = ko.observable();
        self.CertNo = ko.observable(data.certNo);
        self.PSLNo = ko.observable(data.pslNo);
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.SeqNo = ko.observable(data.seqNo);
        self.CertNo = ko.observable(data.certNo);


        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.PledgeKitta = ko.observable(data.pledgeKitta);

        self.ShholderNo = ko.observable(data.shholderNo);
    }
}



function CertificateInsert(data) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.certNo);
        self.PSLNo = ko.observable(data.pslNo);
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.SeqNo = ko.observable(data.seqNo);


        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.PledgeKitta = ko.observable(data.pledgeKitta);

        self.ShholderNo = ko.observable(data.shholderNo);

    }
}

function PSLInfo(data) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.certNo)
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.ShKitta = ko.observable(data.shKitta);
        self.PledgeKitta = ko.observable(data.pledgeKitta);
        self.PSLNo = ko.observable(data.pslNo);

        self.ShOwnerType = ko.observable(data.shOwnerType);

        self.ShholderNo = ko.observable(data.shholderNo);
        self.Selected = ko.observable(data.Kitta);
    }
}

var ClearPSLEntry = function () {

    //for company
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.PSLTRANSFERLIST = ko.observable();
    self.ShholderPSLList = ko.observableArray([]);
    self.PSLInsertCertificate = ko.observableArray([]);
    self.PSLInsertCertificateList = ko.observableArray([]);
    self.MaxKitta = ko.observable();
    self.fName = ko.observable();
    self.lName = ko.observable();
    self.Name = ko.observable();
    self.name = ko.observable();
    self.Address = ko.observable();
    self.ShholderNo = ko.observable();
    self.PledgeKitta = ko.observable();
    self.PledgeAmount = ko.observable();
    self.TotalKitta = ko.observable();
    self.Totalpledgekitta = ko.observable();
    self.PSLNo = ko.observable();
    self.ChalaniNo = ko.observable();
    self.EffDate = ko.observable();
    self.Charge = ko.observable();
    self.SType = ko.observable();
    self.CPSLNo = ko.observable();
    self.ClearDate = ko.observable();
    self.remark = ko.observable();
    self.ClearRemark = ko.observable();
    self.FaName = ko.observable();
    self.GrFaName = ko.observable();
    self.CertNo = ko.observable();
    self.SavePSLEntry = ko.observable();
    self.ClearControl = ko.observable();
    self.SelectedAction = ko.observable();
    self.Selected = ko.observable();
    self.SearchPSL = ko.observable();
    self.PledgeKitta = ko.observable();
    self.TranType = ko.observable();
    self.TranDate = ko.observable();
    self.PledgeCenter = ko.observable();
    self.ShholderTransferList = ko.observableArray([]);
    self.checkValidation = ko.observable();
    self.total = ko.observable();
    self.PSL_clear_No = ko.observable();
    self.ClearedDt = ko.observable();
    self.Issuedup = ko.observable(true);
    self.ChalaniNo = ko.observable();
    self.Charge = ko.observable();
    self.Clear_Charge = ko.observable();
    self.SeqNo = ko.observable();
    self.CertNo = ko.observable();



    var Sum = 0;
    var Sum2 = 0;



    self.Validation = function (data) {
        var errMsg = "";
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "please select Company !!!<br/>";

        }

        if (Validate.empty(self.ShholderNo())) {
            errMsg += "Please select a ShareHolder No !!! <br/>";
        }


        if (errMsg == "") {
            return true;
        }
        else {
            alert('warning', errMsg);
            return false;
        }
    }
    
    self.SaveValidation = function (data) {
        var errMsg = "";
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "please select Company !!!<br>"
        }
        if (Validate.empty(self.ShholderNo())) {
            errMsg += "Please select a ShareHolder No !!! <br/>";
        }
        if ($('#PSL_Certificate_wrapper').find('input[type=checkbox]:checked').length <= 0) {
            errMsg += "Please Tick A Certificate </br>";
        }
        if (Validate.empty(self.Charge())) {
            errMsg += "Please Enter a Charge !!!<br>"
        }
        if (Validate.empty(self.ClearedDt())) {
            errMsg += "Please Enter a Cleared Date !!!<br>"
        }
        if (Validate.empty(self.ClearRemark())) {
            errMsg += "Please Enter A Remarks !!! <br/>";
        }
        if (errMsg == "") {
            return true;
        }
        else {
            alert('error', errMsg);
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

    self.SearchPSL = function () {
        $('#PSL_Certificate').DataTable().clear();
        $('#PSL_Certificate').DataTable().destroy();
        if (!Validate.empty(self.ShholderNo())) {
            $.ajax({
                url: '/Certificate/ClearPSLEntry/SearchHolderPSL',
                data: { 'CompCode': self.SelectedCompany(), 'ShholderNo': self.ShholderNo(), 'SelectedAction': self.SelectedAction() },
                type: 'POST', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: 'json',
                success: function (result) {

                    if (result.isSuccess) {
                        if (result.responseData.length > 0) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new ShholderPSL(item)
                            });
                            self.ShholderPSLList(mappedTasks);
                            $("#shholderSearch").modal('show');

                        } else {
                            alert('error', "No Record Found");
                            $("#shholderSearch").modal('hide');
                        }
                    } else {
                        alert('error', result.message);
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: function () {

                }
            })
        }
    }
    self.GetPSLInformation = function (data) {
        var shholderNo = data.ShholderNo ? data.ShholderNo : self.ShholderNo()
        $("#shholderSearch").modal('hide');
        if (shholderNo != undefined && shholderNo != "") {
            $('#PSL_Certificate').DataTable().clear();
            $('#PSL_Certificate').DataTable().destroy();
            $.ajax({
                type: "post",
                url: '/Certificate/ClearPSLEntry/GetPSLInformation',
                data: { 'ShHolderNo': ko.toJS(shholderNo), 'CompCode': ko.toJS(self.SelectedCompany()), 'SelectedAction': self.SelectedAction(), 'PSLNo': data.PSLNo() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        self.PSLNo(result.responseData[0].pslNo);
                        self.ShholderNo(result.responseData[0].shholderNo);
                        self.Name(result.responseData[0].fName + " " + result.responseData[0].lName);
                        self.Address(result.responseData[0].address);
                        self.FaName(result.responseData[0].faName);
                        self.TotalKitta(result.responseData[0].totalKitta);
                        self.PledgeKitta(result.responseData[0].pledgeKitta);
                        self.GrFaName(result.responseData[0].grFaName);
                        self.TranType(result.responseData[0].tranType);

                        self.PSLNo(result.responseData[0].pslNo);
                        if (result.responseData[0].charge == null) {
                            self.Charge('0');
                        }
                        else {
                            self.Charge(result.responseData[0].charge);
                        }
                        self.TranDate(result.responseData[0].tranDt.substring(0, 10));
                        self.PledgeCenter(result.responseData[0].pledgeCenter);
                        self.ChalaniNo(result.responseData[0].chalaniNo);
                        self.PledgeAmount(result.responseData[0].pledgeAmount);

                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new ShholderPSL(item)
                        });
                        self.ShholderTransferList(mappedTasks);
                        alert('success', result.message);
                    } else {
                        alert('warning', result.message)

                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }, complete: () => {
                    normalDataTable()
                }
            })
        }
    }

    self.SavePSLClearEntry = function () {
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

            var recorddata = self.ShholderTransferList().filter(x => x.Selected()).map(x => ({
                certNo: x.CertNo(),
                pslNo: x.PSLNo(),
                seqNo: x.SeqNo(),
                srNoFrom: x.SrNoFrom(),
                srNoTo: x.SrNoTo(),
                shholderNo: x.ShholderNo(),
                shOwnerType: x.ShOwnerType(),
                pledgeKitta: x.PledgeKitta()
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
                        url: '/Certificate/ClearPSLEntry/SavePSLClearEntry',
                        data: {
                            'CompCode': self.SelectedCompany(), "ShholderNo": self.ShholderNo(), "PSL_clear_No": self.PSL_clear_No(), "Charge": self.Charge(), "Remark": self.ClearRemark(), "ClearedDt": self.ClearedDt(),
                            "SelectedAction": self.SelectedAction(), "Issuedup": self.Issuedup(), "pslno": self.PSLNo(), 'ReportData': recorddata
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
    self.SelectAll = ko.computed({
        read: () => !self.ShholderTransferList().find(x => !x.Selected()),
        write: t => self.ShholderTransferList().forEach(x => x.Selected(t)),
    })

    self.SelectAll1 = ko.computed({
        read: () => !self.PSLInsertCertificate().find(y => !y.Selected()),
        write: t => self.PSLInsertCertificate().forEach(y => y.Selected(t)),
    })

    normalDataTable = () => {
        $('#PSL_Certificate').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            scrollCollapse: true,
            paging: false,
            ordering: false,
            fixedHeader: true,
            "scrollY": "650px",
            "sScrollX": "100%",
            "scrollCollapse": true
        });
    }

    $('#chk').click(() => {
        Sum2 = 0;
        if ($('#PSL_Certificate').DataTable().data().length > 0) {
            if (self.SelectAll()) {
                self.total(Sum2);
            } else {

                for (var i = 0; i < $('#PSL_Certificate').DataTable().data().length; i++) {
                    var x = i + 1;
                    Sum2 += parseInt($('#PSL_Certificate').DataTable().data()[i][8]);
                }
                self.total(Sum2);
            }
        }

    })
    checkValidation = function () {
        Sum = 0;
        for (var i = 0; i < $('#PSL_Certificate').DataTable().data().length; i++) {
            var x = i + 1;
            var Check = $($('#PSL_Certificate').DataTable().row(i).nodes()).find('input').prop('checked');
            if (Check != undefined && Check != "" && Check != false) {
                Sum += parseInt($('#PSL_Certificate').DataTable().data()[i][8]);
            }
        }
        self.total(Sum);
    }
    self.ClearControl = function () {
        $('#Add,#Edit,#Delete').attr('disabled', false);
        $('#Save,#Cancel').attr('disabled', true);
        $('#ShholderNo,#Sno,#CertificateNo,#SSrNoTo,#SSrNoFrom,#SplitDate,#remarks').attr('disabled', true)
        self.PSLNo('')
        self.Name('')
        self.Address('')
        self.ShholderNo('')
        self.FaName('')
        self.GrFaName('')
        self.TranType('')
        self.TranDate('')
        self.PledgeCenter('')
        self.TotalKitta('')
        self.Totalpledgekitta('')
        self.total('')
        self.remark('')
        self.Charge('')
        self.ClearedDt('')
        self.ClearRemark('')
        self.Issuedup(false)
        self.ChalaniNo('')
        self.PledgeAmount('');
        $('#PSL_Certificate').DataTable().clear();
        $('#PSL_Certificate').DataTable().destroy();
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
    ko.applyBindings(new ClearPSLEntry())
    normalDataTable()
})