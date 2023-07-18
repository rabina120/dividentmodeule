function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
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
function Certificate(data) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.certNo);
        self.ShKitta = ko.observable(data.shKitta);
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.TotalKitta = ko.observable(data.totalKitta);
    }
}

 
function ShHolder(data) {
    var self = this;
    if (data != undefined) {

        self.ShholderNo = ko.observable(data.shholderNo);
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.ShKitta = ko.observable(data.shKitta);
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.ShareType = ko.observable(data.shareType);
        self.ShowerType = ko.observable(data.showerType);
        self.CertNo = ko.observable(data.certNo);
        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.TotalKitta = ko.observable(data.totShKitta);
        self.DistCode = ko.observable(data.distCode);
        self.PboxNo = ko.observable(data.pboxNo);
        self.FaName = ko.observable(data.faName);
        self.GrFaName = ko.observable(data.grFaName);
        self.telno = ko.observable(data.telno);
        self.FullName = self.FName() + " " + self.LName();
        self.Kitta = ko.observable(data.Kitta);
    }
}
var CertificateConsolidate = function () {
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.ShOwnerType = ko.observable();
    self.CertNo = ko.observable();
    self.telno = ko.observable();
    self.SelectedAction = ko.observable()
    self.ShholderNo = ko.observable()
    self.TKitta = ko.observable()
    self.EnglishName = ko.observable()
    self.gfdM = ko.observable()
    self.FatherName = ko.observable()
    self.GFatherName = ko.observable()
    self.lufgfM = ko.observable()
    self.ShareType = ko.observable()
    self.Districts = ko.observableArray([]);
    self.DistCode = ko.observable();
    self.NpDistName = ko.observable();
    self.EnDistName = ko.observable();
    self.ShholderNo == ko.observable();
    self.TelephoneNo = ko.observable()
    self.PoBox = ko.observable()
    self.SplitNo = ko.observable()
    self.SplitDate = ko.observable()
    self.remarks = ko.observable()
    self.Sno = ko.observable()
    self.CertificateNo = ko.observable()
    self.ShKitta = ko.observable()
    self.SrNoFrom = ko.observable()
    self.SrNoTo = ko.observable()
    self.CertificateNo = ko.observable()
    self.ShowerType = ko.observable()
    self.ShareCategory = ko.observable()
    self.ShholderNo = ko.observable();
    self.Certificate = ko.observable()
    self.Kitta = ko.observable();
    self.FName = ko.observable();
    self.LName = ko.observable();
    self.Address = ko.observable();
    self.DistCode = ko.observable();
    self.PboxNo = ko.observable();
    self.FaName = ko.observable();
    self.GrFaName = ko.observable();
    self.telno = ko.observable();
    self.FullName = ko.observable();
    self.TotalKitta = ko.observable();
    self.KittaToBeConsolidate = ko.observable();
    self.CertNo = ko.observable();
    self.CertificateConsolidateList = ko.observableArray([])
    //Loading company select options
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
    self.SplitDate(formatDate(new Date))

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

    

    //loading district on form load
    self.LoadDistrict = function () {

        $.ajax({
            type: "post",
            url: '/HolderManagement/ShareHolder/GetAllDistrict',
            datatype: "json",
           
            beforeSend: function (xhr) {
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
        }

    };
    self.GetCertificateDetails = () => {
        if (!Validate.empty(self.CertificateNo())) {
            Openloader()
            if (self.SelectedAction() == 'A') {
                $.ajax({
                    type: 'POST',
                    url: '/Certificate/CertificateConsolidate/GetCertificateDetails',
                    data: { 'CompCode': self.SelectedCompany(), 'ShholderNo': self.ShholderNo, 'CertificateNo': self.CertificateNo(), 'SelectedAction': self.SelectedAction() },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    dataType: 'json',
                    success: (result) => {
                        if (result.isSuccess) {
                            if (result.responseData.length > 0) {

                                self.ShKitta(result.responseData[0].shKitta);

                                self.SrNoFrom(result.responseData[0].srNoFrom)
                                self.SrNoTo(result.responseData[0].srNoTo)
                                self.ShareType(result.responseData[0].shareType)
                                self.ShareCategory(self.checkCategory(result.responseData.shareType))
                                self.ShOwnerType(result.responseData[0].shOwnerType)
                                //self.ShOwerType(self.checkOwnerType(result.responseData.shOwnerType))
                                


                            }
                            else {
                                alert('warning', 'No Record Found ');
                            }
                        }
                        else {
                            alert('error', result.message + 'CertNo:' + self.CertificateNo())
                            self.ShKitta(null);
                            self.SrNoFrom(null);
                            self.SrNoTo(null);
                            self.CertificateNo(null);
                            $('#CertificateNo').attr('disabled', false)
                        }
                    },
                    error: (error) => {
                        alert('error', error.message)
                        console.log('error =>', error.message)
                        self.ShKitta(null);
                        self.SrNoFrom(null);
                        self.SrNoTo(null);
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            } else if (self.SelectedAction() == 'D') {
                $.ajax({
                    type: 'POST',
                    url: '/Certificate/CertificateConsolidate/GetCertificateDetails',
                    data: { 'CompCode': self.SelectedCompany(), 'ShholderNo': self.ShholderNo, 'CertificateNo': self.CertNo, 'SelectedAction': self.SelectedAction() },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    dataType: 'json',
                    success: (result) => {
                        if (result.isSuccess) {
                            if (result.responseData != null) {
                                self.ShKitta(result.responseData[0].shKitta);

                                self.SrNoFrom(result.responseData[0].srNoFrom)
                                self.SrNoTo(result.responseData[0].srNoTo)
                                self.ShareType(result.responseData[0].shareType)
                                self.ShareCategory(self.checkCategory(result.responseData.shareType))
                                self.ShOwnerType(result.responseData[0].shOwnerType)
                                self.ShOwerType(self.checkOwnerType(result.responseData.shOwnerType))
                                

                            } 
                        } else {
                            alert('error', result.message)
                        }
                    }, error: (error) => {
                        alert('error', error.message)
                        console.log('error=>', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            }
        }
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

    self.GetCertificateInformation = () => {
        if (!Validate.empty(self.ShholderNo())) {
            Openloader()
            if (self.SelectedAction() == 'A') {
                $.ajax({
                    type: 'POST',
                    url: '/Certificate/CertificateConsolidate/GetShholderDetailsByShHolderNo',
                    data: { 'CompCode': self.SelectedCompany(), 'ShholderNo': self.ShholderNo, 'SelectedAction': self.SelectedAction() },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    dataType: 'json',
                    success: (result) => {
                        if (result.isSuccess) {
                            if (result.responseData.length > 0) {

                                self.FullName(result.responseData[0].fName + " " + result.responseData[0].lName);
                                self.FaName(result.responseData[0].faName);
                                self.GrFaName(result.responseData[0].grFaName);
                                self.TotalKitta(result.responseData[0].totalKitta);
                                self.DistCode(result.responseData[0].distCode);
                                self.DistCode(self.Districts().find(x => x.DistCode() == ko.toJS(result.responseData[0].distCode)).EnDistName());
                                self.PboxNo(result.responseData[0].pboxNo);
                                self.remarks(result.responseData[0].remarks);
                                self.Kitta = (result.responseData[0].Kitta);
                                self.ShKitta(result.responseData[0].shKitta);

                                self.SrNoFrom(result.responseData[0].srNoFrom)
                                self.SrNoTo(result.responseData[0].srNoTo)
                                self.ShareType(result.responseData[0].shareType)

                                self.ShOwnerType(result.responseData[0].shOwnerType)

                                $('#CertificateNo').attr('disabled', false);
                                $('#CertificateNo').focus();
                            } else {
                                alert('warning', 'No Record Found ');
                            }
                        } else {
                            alert('error', result.message)
                            $('#ShholderNo').attr('disabled', true)
                        }
                    }, error: (error) => {
                        alert('error', error.message)
                        console.log('error =>', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            } else if (self.SelectedAction() == 'D') {
                $.ajax({
                    type: 'POST',
                    url: '/Certificate/CertificateConsolidate/GetShholderDetailsByShHolderNo',
                    data: { 'CompCode': self.SelectedCompany(), 'ShholderNo': self.ShholderNo, 'SelectedAction': self.SelectedAction() },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    dataType: 'json',
                    success: (result) => {
                        if (result.isSuccess) {
                            if (result.responseData != null) {
                                self.FullName(result.responseData[0].fName + " " + result.responseData[0].lName);
                                self.FaName(result.responseData[0].faName);
                                self.GrFaName(result.responseData[0].grFaName);
                                self.TotalKitta(result.responseData[0].totalKitta);
                                self.DistCode(result.responseData[0].distCode);
                                self.DistCode(self.Districts().find(x => x.DistCode() == ko.toJS(result.responseData[0].distCode)).EnDistName());
                                self.PboxNo(result.responseData[0].pboxNo);
                                self.remarks(result.responseData[0].remarks);

                                self.ShKitta(result.responseData[0].shKitta);

                                self.SrNoFrom(result.responseData[0].srNoFrom)
                                self.SrNoTo(result.responseData[0].srNoTo)
                                self.ShareType(result.responseData[0].shareType)

                                self.ShOwnerType(result.responseData[0].shOwnerType)



                            } else {
                                alert('warning', 'No Record Found ');
                            }
                        } else {
                            alert('error', result.message)
                        }
                    }, error: (error) => {
                        alert('error', error.message)
                        console.log('error=>', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }

    self.Add = function () {

        var count = self.CertificateConsolidateList().length;
        if (ValidateTable()) {
            var ShKitta = self.ShKitta()
            var CertificateNo = self.CertificateNo()
            var srnofrom = self.SrNoFrom()
            var srnoto = self.SrNoTo()
            var shareType = self.ShareType()
            var shOwnerType = self.ShOwnerType()

            swal({
                title: "More Consolidate",
                text: "Do You Want Consolidate Certificate",
                icon: "success",
                buttons: true,
                dangerMode: true
            }).then((willSave) => {
                if (willSave) {
                    self.CertificateConsolidateList.push({
                        No: count + 1,
                        ShKitta: ShKitta,
                        CertNo: CertificateNo,
                        SrNoFrom: srnofrom,
                        SrNoTo: srnoto,
                        ShareType: shareType,
                        ShOwnerType: shOwnerType

                    });
                }
                else {

                    self.CertificateConsolidateList.push({
                        No: count + 1,
                        ShKitta: shKitta,
                        CertNo: CertificateNo,
                        SrNoFrom: srnofrom,
                        SrNoTo: srnoto,
                        ShareType: shareType,
                        ShOwnerType: shOwnerType
                    });


                    var srnotoLast = parseInt(CertificateLast.SrNoTo) + 1;


                    $('#SSrNoTo, #SSrNoFrom, #CertificateNo,#ShKitta').attr('disabled', true)
                }
            });

            self.ShKitta('')
            self.CertificateNo('')
            self.SrNoFrom('')
            self.SrNoTo('')





        }
    }
    self.SaveCertificateConsolidate = function () {
        if (self.Validation()) {
            Openloader()

            $.ajax({
                type: 'POST',
                url: '/Certificate/CertificateConsolidate/SaveCertificateConsolidate',
                data: {
                   
                    'CompCode': self.SelectedCompany(), 'aTTCertificateConsolidate': self.CertificateConsolidateList(),
                    'shholderno': self.ShholderNo(), 'Splitdate': self.SplitDate(), 'remarks': self.remarks(), 'SelectedAction': self.SelectedAction() 
                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
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
                failure: (error) => {
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
    self.ValidateTable = function () {
        return true;
    }
    self.Validation = function () {
        var errMsg = ""

        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company <br>"
        }
        if (Validate.empty(self.SelectedAction())) {
            errMsg += "Please Choose Action <br>"
        }
        
        
        if (Validate.empty(self.SplitDate())) {
            errMsg += "Please Enter Consolidate Date<br>"
        }
        if (self.SelectedAction() == "A") {
            if (Validate.empty(self.remarks())) {
                errMsg += "Please Enter Remarks<br>"
            }
        }

        if (Validate.empty(self.CertificateConsolidateList())) {
            errMsg += "No List in Table To Consolidate <br>"
        }
        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
        }
    }
    self.ClearControl = function () {
        $('#Add,#Edit,#Delete').attr('disabled', false);

        $('#Save,#Cancel').attr('disabled', true);
        
        
        self.CertificateNo('')
        self.ShKitta('')
        self.ShholderNo('')
         
        self.TotalKitta('')
        self.FullName('')
        self.FaName('')
        self.GrFaName('')
        self.DistCode('')
        self.PboxNo('')
        self.SrNoFrom('')
        self.SrNoTo('')
        self.Sno('')
        self.SplitDate('')
        self.remarks('')
       
        self.CertificateConsolidateList('')
      
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
    ko.applyBindings(new CertificateConsolidate())
})
