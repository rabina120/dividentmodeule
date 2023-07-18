function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function CertificateSplitData(data,index) {
    var self = this;
    if (data != undefined) {
        self.No = ko.observable(index+1);
        self.ShKitta = ko.observable(data.kitta);
        self.CertNo = ko.observable(data.certno);
        self.SrNoFrom = ko.observable(data.srnofrom)
        self.SrNoTo = ko.observable(data.srnoto)
    }
}


var CertificateSplit = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    self.SelectedAction = ko.observable()

    self.Certificate = ko.observable()
    self.SKitta = ko.observable()
    self.ShHolderNo = ko.observable()
    self.StartSNo = ko.observable()
    self.EndSNo = ko.observable()
    self.shareType = ko.observable();
    self.ShareCategory = ko.observable()
    self.TShareAmt = ko.observable()
    self.PAmount = ko.observable()
    self.ShOwerType = ko.observable()
    self.NpName = ko.observable()
    self.SplitDate = ko.observable()
    self.Remarks = ko.observable()
    self.Sno = ko.observable()
    self.KittaToBeSplit = ko.observable()
    self.CertificateNo = ko.observable()
    self.SSrNoFrom = ko.observable()
    self.SSrNoTo = ko.observable()
    self.TotalSplitted = ko.observable(0)
    self.DistCert = ko.observable()
    self.DistCertDate = ko.observable()
    self.DupliNo = ko.observable(false)
    self.ShowerType = ko.observable()
    self.PageNo = ko.observable()
    self.SplitNo = ko.observable()

    var splitCertCount = 0;
    var TotalSplittedKitta = 0;
    self.CertificateSplitList = ko.observableArray([])

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


    self.ChooseOption = function (data) {
        $('#Add,#Edit,#Delete').attr('disabled', true);
        $('#Save,#Cancel').attr('disabled', false);

        self.SelectedAction(data);
        switch (data) {
            case "A":
                $('#Certificate').attr('disabled', false);
                break;
            case "D":
                $('#Certificate').attr('disabled', false);
                break;
        }

    };

    self.GetCertificateInformation = () => {
        if (!Validate.empty(self.Certificate())) {
            Openloader()
            if (self.SelectedAction() == 'A') {
                
                $.ajax({
                    type: 'POST',
                    url: '/Certificate/CertificateSplit/GetCertificateDetailsByCertificateNo',
                    data: { 'CompCode': self.SelectedCompany(), 'CertificateNo': self.Certificate,'ActionType':self.SelectedAction()},
                    dataType: 'json', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: (result) => {
                        if (result.isSuccess) {
                            if (result.responseData.length > 0) {
                                self.Certificate(result.responseData[0].certNo)
                                self.SKitta(result.responseData[0].shKitta);
                                self.ShHolderNo(result.responseData[0].shHolderNo)
                                self.StartSNo(result.responseData[0].srNoFrom)
                                self.EndSNo(result.responseData[0].srNoTo)
                                self.shareType(result.responseData[0].shareType)
                                self.ShareCategory(self.checkCategory(result.responseData[0].shareType))
                                self.TShareAmt(result.responseData[0].totalAmount)
                                self.PAmount(result.responseData[0].paidAmount)
                                self.ShowerType(result.responseData[0].shOwnerType)
                                self.ShOwerType(self.checkOwnerType(result.responseData[0].shOwnerType))
                                if (result.responseData[0].npname == null) {
                                    alert('error', "Holder Not Found !!!")
                                }
                                self.NpName(result.responseData[0].npname ?? convert_to_unicode_with_value(result.responseData[0].npname))
                                self.DistCert(result.responseData[0].distCert)
                                self.DistCertDate(result.responseData[0].certDistDt)
                                if (result.responseData[0].dupliNo > 0) {
                                    self.DupliNo(true)
                                    swal({
                                        icon: 'info',
                                        title: 'Duplicate Certificate',
                                        text: 'Duplicate Certificate Issued : ' + result.responseData[0].dupliNo,

                                    })
                                }
                                $('#Sno,#KittaToBeSplit,#SplitDate,#Remarks').attr('disabled', false)



                            } else {
                                alert('warning', 'No Record Found ');
                            }
                        } else {
                            alert('error', result.message)
                            console.log('error =>', result.message)
                        }
                    }, error: (error) => {
                        alert('error', error.message)
                        console.log('error =>', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            }
            else if (self.SelectedAction() == 'D') {
                $.ajax({
                    type: 'POST',
                    url: '/Certificate/CertificateSplit/GetCertificateDetailsByCertificateNo',
                    data: { 'CompCode': self.SelectedCompany(), 'CertificateNo': self.Certificate(),'ActionType':self.SelectedAction() },
                    dataType: 'json', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: (result) => {
                        if (result.isSuccess) {
                            if (result.responseData != null) {
                                self.Certificate(result.responseData.certNo)
                                self.SKitta(result.responseData.shKitta);
                                self.ShHolderNo(result.responseData.shHolderNo)
                                self.StartSNo(result.responseData.srNoFrom)
                                self.EndSNo(result.responseData.srNoTo)
                                self.shareType(result.responseData.shareType)
                                self.ShareCategory(self.checkCategory(result.responseData.shareType))
                                self.TShareAmt(result.responseData.totalAmount)
                                self.PAmount(result.responseData.paidAmount)
                                self.ShowerType(result.responseData.shOwnerType)
                                self.ShOwerType(self.checkOwnerType(result.responseData.shOwnerType))
                                if (result.responseData.npname == null) {
                                    alert('error', "Holder Not Found !!!")
                                }
                                self.NpName(result.responseData.npname ?? result.responseData.npname)
                                self.DistCert(result.responseData.distCert)
                                
                               
                                self.Remarks(result.responseData.aTTCertificateSplitDetails[0].remarks)
                                if (result.responseData.dupliNo > 0) {
                                    self.DupliNo(true)
                                    swal({
                                        icon: 'info',
                                        title: 'Duplicate Certificate',
                                        text: 'Duplicate Certificate Issued : ' + result.responseData.dupliNo,

                                    })
                                }
                                var mappedTasks = $.map(result.responseData.aTTCertificateSplitDetails, function (item, index) {
                                    return new CertificateSplitData(item,index)
                                });
                                self.CertificateSplitList(mappedTasks);
                                self.PageNo(result.responseData.aTTCertificateSplitDetails[0].pageno)
                                self.SplitNo(result.responseData.aTTCertificateSplitDetails[0].split_no)
                                var splitDate = "";
                                if (result.responseData.aTTCertificateSplitDetails[0].split_dt != null) {
                                    splitDate = result.responseData.aTTCertificateSplitDetails[0].split_dt.slice(0, 10);
                                }
                                self.SplitDate(splitDate)

                            } else {
                                alert('warning', 'No Record Found ');
                            }
                        } else {
                            alert('error', result.message)
                        }
                    }, error: (error) => {
                        alert('error',error.message)
                        console.log('error=>',error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }

    self.ClearControl = function () {
        self.SelectedAction('')
    }

    self.checkCategory = function (data) {
        switch (data) {
            case 1:
                return "1 Preferential Share"
                break;
            case 2:
                return "2 Ordinary Share"
                break;
            case 3:
                return "3 Bonus Share"
                break;
            default:
                return "4 Right Share"
        }
    }

    self.CheckCertificateNo = function () {
        if (Validate.empty(self.CertificateSplitList())) {
            if (!Validate.empty(self.CertificateNo())) {
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/Certificate/CertificateSplit/CheckCertificateNo',
                    data: { 'CompCode': self.SelectedCompany(), 'CertificateNo': self.CertificateNo() },
                    dataType: 'JSON',
                    success: (result) => {
                        if (result.isSuccess) {
                            $('#CertificateNo').attr('disabled', true)
                            $('#Split').attr('disabled', false)

                            
                        } else {
                            alert('error', result.message)
                            self.CertificateNo('')
                        }
                    },
                    error: (error) => {
                        alert('error', error.message)
                        self.CertificateNo('')
                    }
                })
            } else {
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/Certificate/CertificateSplit/CheckCertificateNo',
                    data: { 'CompCode': self.SelectedCompany(), 'CertificateNo': self.CertificateNo() },
                    dataType: 'JSON',
                    success: (result) => {
                        if (result.isSuccess) {
                            $('#CertificateNo').attr('disabled', true)
                            $('#Split').attr('disabled', false)
                            self.CertificateNo(result.message)
                           
                        } else {
                            alert('error', result.message)
                            self.CertificateNo('')
                        }
                    },
                    error: (error) => {
                        alert('error', error.message)
                        self.CertificateNo('')
                    }
                })
            }
        } else {
         
            var CertificateLast = self.CertificateSplitList()[self.CertificateSplitList().filter(x => x.CertNo != self.Certificate()).length - 1]
            $('#CertificateNo').attr('disabled', true)
            self.CertificateNo(parseInt(CertificateLast.CertNo) + 1)
        }
    }

    self.Validation = () => {
        var errMsg = ""

        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company <br>"
        }
        if (Validate.empty(self.SelectedAction())) {
            errMsg += "Please Choose Action <br>"
        }
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Enter Certificate No <br>"
        }
        if (Validate.empty(self.SKitta())) {
            errMsg += "Please Enter SH Kitta <br>"
        }
        if (Validate.empty(self.StartSNo())) {
            errMsg += "No Start SNO <br>"
        }
        if (Validate.empty(self.EndSNo())) {
            errMsg += "No End SNO <br>"
        }
        if (Validate.empty(self.ShareCategory())) {
            errMsg += "Please Enter Share Category <br>"
        }
        if (Validate.empty(self.TShareAmt())) {
            errMsg += "Please Enter Total Share Amount<br>"
        }
        if (Validate.empty(self.PAmount())) {
            errMsg += "Please Enter Paid Amount<br>"
        }
        if (Validate.empty(self.ShOwerType())) {
            errMsg += "Please Enter Share Owner Type<br>"
        }
        if (Validate.empty(self.SplitDate())) {
            errMsg += "Please Enter Split Date<br>"
        }
        if (self.SelectedAction() == "A") {
            if (Validate.empty(self.Remarks())) {
                errMsg += "Please Enter Remarks<br>"
            }
        }
        
        if (Validate.empty(self.CertificateSplitList())) {
            errMsg += "No List in Table To Split <br>"
        }
        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
        }
    }

    self.SaveCertificateSplit = () => {
        if (self.Validation()) {
            Openloader()

            $.ajax({
                type: 'POST', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/Certificate/CertificateSplit/SaveCertificateSplit',
                data: {
                    'CompCode': self.SelectedCompany(), 'CertificateNo': self.Certificate(),
                    'srnofrom': self.StartSNo(), 'srnoto': self.EndSNo(), 'aTTCertificates': self.CertificateSplitList(),
                    'shholderno': self.ShHolderNo(), 'Splitdate': self.SplitDate(), 'shownertype': self.ShowerType(),
                    'sharetype': self.shareType(), 'remarks': self.Remarks(),'SelectedAction': self.SelectedAction(),'PageNo':self.PageNo(),'SplitNo':self.SplitNo()
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
                    jAlert(error, error.message);
                },
                failure: (error)  => {
                    jAlert(error, error.message);
                },
                complete: () => {
                    Closeloader()
                    self.ClearControl()
                }
            }).fail((xhr, status, message) => {
                jAlert(status, message);

            });
        }
    }

    self.CheckKittaSplitted = () => {
        if (self.SKitta() > self.KittaToBeSplit()) {
          
            var count = self.CertificateSplitList().filter(x => x.CertNo != self.Certificate()).length;

            if (count != 0) {
                self.SSrNoFrom(parseInt(self.CertificateSplitList().filter(x => x.CertNo != self.Certificate())[count - 1].SrNoTo) + 1);
                self.SSrNoTo(parseInt(self.SSrNoFrom()) + parseInt(self.KittaToBeSplit()) - parseInt(1));
            } else {
                self.SSrNoFrom(parseInt(self.StartSNo()));
                self.SSrNoTo(parseInt(self.SSrNoFrom()) + parseInt(self.KittaToBeSplit()) - 1);

            }

            self.CheckCertificateNo()
            
        }
        else {
            alert('warning', 'Splitted Kitta Cannot Be Greater Than Available Kitta.')
            
            self.KittaToBeSplit('');
            document.getElementById('KittaToBeSplit').focus();

        }
    }

    self.AddSplitCertificateToSplit = function () {
        var count = self.CertificateSplitList().length;
        if (ValidateTable()) {
            var kitta = self.KittaToBeSplit()
            var certNo = self.CertificateNo()
            var srnofrom = self.SSrNoFrom()
            var srnoto = self.SSrNoTo()

            var HolderKitta = parseInt(self.SKitta());
            var TotalKitta = parseInt(self.TotalSplitted())

            TotalSplittedKitta += parseInt(self.KittaToBeSplit())
            if (kitta > 0) {

            self.CertificateSplitList.push({
                No: count + 1,
                ShKitta: kitta,
                CertNo: certNo,
                SrNoFrom: srnofrom,
                SrNoTo: srnoto
            });

            }
            var CertificateLast = self.CertificateSplitList()[self.CertificateSplitList().length - 1]
            var srnotoLast = srnoto+ 1;
            TotalKitta = parseInt(self.TotalSplitted()) + parseInt( kitta)

            //var CertificateSplitList=self.CertificateSplitList().filter(x => x.CertNo != self.Certificate())[0];
            //self.CertificateSplitList(CertificateSplitList);
            self.CertificateSplitList.remove(function (item) {
                return item.CertNo == self.Certificate();
            });
            if ((HolderKitta - TotalKitta) > 0) {
                self.CertificateSplitList.push({
                    No: count + 2,
                    ShKitta: (HolderKitta - TotalKitta),
                    CertNo: self.Certificate(),
                    SrNoFrom: srnotoLast,
                    SrNoTo: self.EndSNo()
                });
            }
            

            self.KittaToBeSplit('')
            self.CertificateNo('')
            self.SSrNoFrom('')
            self.SSrNoTo('')

            self.CertificateNo(parseInt(certNo)+1)
            self.TotalSplitted(TotalSplittedKitta)


        }
    }


    self.RemoveCertificateRow = (data) => {
        console.log(data.No)
        if (self.CertificateSplitList().length != data.No) {
            var TS = self.TotalSplitted()
            self.CertificateSplitList.remove(function (item) {
                self.TotalSplitted(parseInt(TS) - parseInt(item.ShKitta))
                return item.No == data.No;
            });
        }
    }
    self.ValidateTable = () => {
        var errMsg = "";

        var total = 0;
        $.each(ko.toJS(self.CertificateSplitList), (index, value) => {
            if (value.CertNo != self.Certificate()) { total += parseInt(value.ShKitta); }
        });

        if (Validate.empty(self.KittaToBeSplit())) {
            errMsg += "Please Enter Kitta To be Split <br>"

        } else {
            total += parseInt(self.KittaToBeSplit())
        } if (parseInt(total) > parseInt(self.SKitta())) {
            errMsg += "Enter Kitta To be Split is Greater Than Sh Kitta <br>"
        } if (Validate.empty(self.CertificateNo())) {
            errMsg += "Please Enter Certificate No<br>"
        } if (Validate.empty(self.SSrNoFrom())) {
            errMsg += "Please Enter SSR No From <br>"
        } if (Validate.empty(self.SSrNoTo())) {
            errMsg += "Please Enter SSR No To <br>"
        }
        if (errMsg == "") {
            return true;
        } else {
            alert('error', errMsg)
            return false;
        }
    }
    self.checkOwnerType = function (data) {
        switch (data) {
            case 1:
                return "1 Promoter"
                break;
            case 2:
                return "2 Staff"
                break;
            default:
                return "3 Public"

        }
    }

    self.CreateReport = function () {
        swal({
            title: "Print Report",
            text: "Do You Want To Print Report",
            icon: "success",
            buttons: true,
            dangerMode: true
        }).then((willSave) => {
            if (willSave) {
                Openloader()
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/Certificate/CertificateSplit/CreateReport',
                    data: { 'CompCode': self.SelectedCompany(), 'CertificateNo': self.Certificate()},
                    dataType: 'JSON',
                    success: (result) => {
                        if (result.isSuccess) {
                           
                        } else {
                            alert('error', result.message)
                        }
                    },
                    error: (error) => {
                        alert('error', error.message)
                    },
                    complete: () => {
                        Closeloader()
                        self.ClearControl()
                    }
                })
            }
            else {

            }
        });
    }

    self.ClearControl = () => {
        $('#Add,#Edit,#Delete').attr('disabled', false);

        $('#Save,#Cancel').attr('disabled', true);
        $('#Certificate,#Sno,#KittaToBeSplit,#CertificateNo,#SSrNoTo,#SSrNoFrom,#SplitDate,#Remarks,#Split').attr('disabled', true)
        TotalSplittedKitta = 0;
        self.Certificate('')
        self.SKitta('')
        self.ShHolderNo('')
        self.StartSNo('')
        self.EndSNo('')
        self.ShareCategory('')
        self.TShareAmt('')
        self.PAmount('')
        self.ShOwerType('')
        self.NpName('')
        self.SplitDate('')
        self.Remarks('')
        self.Sno('')
        self.KittaToBeSplit('')
        self.CertificateNo('')
        self.SSrNoFrom('')
        self.SSrNoTo('')
        self.TotalSplitted('')
        self.DistCert('')
        self.DistCertDate('')
        self.DupliNo = ko.observable(false)
        self.CertificateSplitList('')
        self.ShowerType('')
        self.PageNo('')
        self.SplitNo('')
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
    ko.applyBindings(new CertificateSplit())
})