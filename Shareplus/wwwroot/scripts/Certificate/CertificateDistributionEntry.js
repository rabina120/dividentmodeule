

function ParaComp(data) {

    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compCode + " " + data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function ShHolder(data) {
    var self = this;
    if (data != undefined) {

        self.ShholderNo = ko.observable(data.ShholderNo);
        self.Title = ko.observable(data.title);
        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.Name = ko.observable(data.name);
        self.name = ko.observable(data.fName + " " + data.lName);
        self.Address = ko.observable(data.Address);  
        self.TotalKitta = ko.observable(data.totalKitta);
        self.DistDate = ko.observable(data.DistDate);

    }
}

///tbl certificates data
function Certificate(data) {


    var self = this;
    if (data != undefined) {


        self.TBLCertCompCode = ko.observable(data.compcode);
        self.TBLCertCShholderNo = ko.observable(data.shHolderNo);
        self.TBLDistributed = ko.observable(data.distributed);
        self.TBLCertCertNo1 = ko.observable(data.certNo);
        self.TBLCertSrNoFrom = ko.observable(data.srNoFrom);
        self.TBLCertSrNoTo = ko.observable(data.srNoTo);
        self.TBLCertShKitta = ko.observable(data.shKitta);
        self.SelectedDistributed();

    }
}

///used for Distributiontransfer
function DISTRIBUTIONTRANSFER(data) {
    var self = this;
    if (data != undefined) {

        self.CompCode = ko.observable(data.compCode);
        self.CertNo = ko.observable(data.certNo);
        self.SrNo = ko.observable(data.SrNo);
        self.DistDate = ko.observable(data.DistDate);
        self.ShholderNo = ko.observable(data.ShholderNo);  
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.CertDistDt = ko.observable(data.certDistDt);
        self.TranKitta = ko.observable(data.tranKitta);
        self.DistCert = ko.observable(data.distCert);
        self.ShKitta = ko.observable(data.shKitta);
        self.Selected = ko.observable(false);
 
    }
}


///DistributionEntryViewModel
var CertificateDistributionEntry = function () {

    self.Name = ko.observable();
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();
    self.fName = ko.observable();
    self.lName = ko.observable();
    self.SaveDistributionEntry = ko.observable();
    self.DISTRIBUTIONTRANSFERLIST = ko.observableArray([]);
    self.ClearControl = ko.observable();
    self.StartSerialNo = ko.observable();
    self.EndSeriaLNo = ko.observable();
    self.Address = ko.observable();
    self.address = ko.observable();
    self.ShKitta = ko.observable();
    self.TotalKitta = ko.observable();
    self.totalKitta = ko.observable();
    self.DistDate = ko.observable();
    self.name = ko.observable();
    self.Name = ko.observable();
    self.ShHolderList = ko.observableArray([]);
    self.Entrydate = ko.observable();
    self.shholderNo = ko.observable();
    self.ShholderNo = ko.observable();
    self.DistCert = ko.observable();
    self.CertDistDt = ko.observable();
    self.TBLCertCompCode = ko.observable();
    self.TBLCertShHolderNo = ko.observable();
    self.TBLCertCertNo1 = ko.observable();
    self.TBLCertSrNoFrom = ko.observable();
    self.TBLCertSrNoTo = ko.observable();
    self.TBLCertShKitta = ko.observable();
    self.SelectedDistributed = ko.observable();
    self.CertificateList1 = ko.observableArray([]);
    self.CertificateList = ko.observableArray([]);
    self.Selected = ko.observable()
    self.ChooseOptions = ko.observable();
    self.actionType = ko.observable();
    self.SelectedAction = ko.observable();

    var record = [];

    self.Validation = function (data) {
        var errMsg = "";
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "please select Company !!!<br/>";

        }

        if (Validate.empty(self.ShholderNo())) {
            errMsg += "Please select a ShareHolder No !!! <br/>";
        }

        if (data == 'S') {

            if (Validate.empty(self.SelectedAction())) {
                errMsg += "Please Choose Action Type !!! <br/>";
            }

            if (Validate.empty(self.DistDate())) {
                errMsg += "Please Enter Distributed Date !!! <br/>";
            }

            if ($('#Distribution_Certificate').find('input[type=checkbox]:checked').length <= 0) {
                errMsg += "Please Tick A Certificate To Distribute !!!</br>";
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

    


    // Loading company select options
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


    self.GetSholderInformation = function (data) {
        var ShholderNo = self.ShholderNo();      
        if (ShholderNo != undefined && ShholderNo != "") {
            $.ajax({
                type: "post",
                url: '/Certificate/CertificateDistributionEntry/GetSholderInformation',
                data: { 'ShholderNo': ShholderNo, 'CompCode': ko.toJS(self.SelectedCompany()) },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {

                    if (result.isSuccess) {
                      
                        self.ShholderNo(result.responseData.shholderNo);
                        var name = result.responseData.fName + ' ' + result.responseData.lName;
                        self.Name(name);               
                        self.Address(result.responseData.address);                    
                        self.TotalKitta(result.responseData.totalKitta);
                        self.LoadData(); ///load Certificate

                    } else {
                        alert('warning', result.message);
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })
        }
    }


    self.ChooseOptions = function (data) {
        $('#Add,#Delete').attr('disabled', true);
        $('#Save,#Cancel,#ShholderNo,#DistDate').attr('disabled', false);

        self.SelectedAction(data);
        switch (data) {

            case "A":
                $('#ShholderNo').attr('disabled', false);
                self.ShholderNo()
                break;

            case "D":

                $('#ShholderNo').attr('disabled', false);
                break;
        }
    }

    self.LoadData = function () {
        var companycode = self.SelectedCompany()
        $('#Distribution_Certificate').DataTable().clear();
        $('#Distribution_Certificate').DataTable().destroy();

            $.ajax({
                type: "post",
                url: '/Certificate/CertificateDistributionEntry/GET_SHHOLDER_CERTDISTRIBUTE',
                data: { 'compcode': companycode, 'ShholderNo': self.ShholderNo(), 'SelectedAction': self.SelectedAction() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    var data = result.responseData;
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new DISTRIBUTIONTRANSFER(item)
                    });
                    self.DISTRIBUTIONTRANSFERLIST(mappedTasks);
                    alert('success', result.message);

                    
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: () => {
                    $('#Distribution_Certificate').DataTable({
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
            })



      
    }

    ///saving 
    self.SaveDistributionEntry = function (data) {

        if (self.Validation(data)) {
            $('#Save,#Cancel').attr('disabled', true);
            var companyCode = self.SelectedCompany()
            for (var i = 0; i < $('#Distribution_Certificate').DataTable().data().length; i++) {
                var x = i + 1;
                var Check = $($('#Distribution_Certificate').DataTable().row(i).node()).find('input').prop('checked');
                if (Check != undefined && Check != "" && Check != false) {
                    var Data = {
                        SrNo: $('#Distribution_Certificate').DataTable().data()[i][1],
                        CertNo: $('#Distribution_Certificate').DataTable().data()[i][2],
                        SrNoFrom: $('#Distribution_Certificate').DataTable().data()[i][3],
                        SrNoTo: $('#Distribution_Certificate').DataTable().data()[i][4],
                        ShKitta: $('#Distribution_Certificate').DataTable().data()[i][5],
                        DistDate: $('#Distribution_Certificate').DataTable().data()[i][6],
                        Distributed: $('#Distribution_Certificate').DataTable().data()[i][7]                                                                  
                    }
                    record.push(Data)
                }
            }
            var text, title;
            if (SelectedAction() == "A") {
                text = "Distributed Certificates will Be saved with ShholderNo" + self.ShholderNo();
                title = "Distributed Certificates ?"
            }

            else {
                text = "Distributed Certificates with ShholderNo" + self.ShholderNo() + "will be deleted";
                title = "Delete Distributed Certificate ?"
            }
        }
        if (record.length > 0) {
            swal({
                title: title,
                text: text,
                icon: "success",
                buttons: true,
                dangerMode: true
            }).then((willSave) => {
                $.ajax({
                    type: "POST",
                    url: '/Certificate/CertificateDistributionEntry/SaveDistributionCertificate',
                    data: {
                                                                                                             
                        'certificatelist': record, 'compcode': companyCode,
                        'ShholderNo': self.ShholderNo(), 'DistDate': self.DistDate(),
                        'selectedaction': self.SelectedAction()
                    },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    datatype:"json",
                    success: function (response) {
                        if (response.isSuccess) {
                            alert('success', response.message);
                            self.ClearControl()
                        }
                        //} else {
                        //    alert('error', response.message);

                        //}
                    },
                    error: function (error) {
                        alert('error', response.message);
                    },
                    complete: () => {
                        record = []
                        $('#save,#cancel').attr('disabled', false);
                    }
                });
            })
        }
    }

    self.SelectAll = ko.computed({
        read: () => !self.DISTRIBUTIONTRANSFERLIST().find(x => !x.Selected()),
        write: t => self.DISTRIBUTIONTRANSFERLIST().forEach(x => x.Selected(t)),
    })
    self.ClearControl = () => {
        $('#ShHolderNo,#DistDate').attr('disabled', true);      
        self.ShholderNo('');
        self.totalKitta('');
        self.Name('');
        self.Address('');
        $("#certificatelistData").empty();
        self.DISTRIBUTIONTRANSFERLIST([]);
        $('#DISTRIBUTIONTRANSFER').DataTable().clear()
        $('#DISTRIBUTIONTRANSFER').DataTable().destroy();
        
        record = [];

        $('#Add,#Delete').attr('disabled', false);
        $('#Save,#Cancel').attr('disabled', true);
        $('#Distribution_Certificate').DataTable().destroy();
        $('#Distribution_Certificate tbody').empty();
    }
}






















$(document).ready(function () {

    $(function () {
        $('#DistDate').datepicker({

            format: 'yyyy-mm-dd',
            todayHighlight: true,
            autoclose: true,
            endDate: '+0d'
       
        });
        $('#DistDate').datepicker('setDate', 'today');

        $('#Add,#Delete').attr('disabled', false);
        $('#Save,#Cancel').attr('disabled', true);

        ko.applyBindings(new CertificateDistributionEntry());
    });
})
