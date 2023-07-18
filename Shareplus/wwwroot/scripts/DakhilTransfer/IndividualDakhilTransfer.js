function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function Broker(data) {
    var self = this;
    if (data != undefined) {
        
        self.BCode = ko.observable(data.bcode)
        self.BrokerName = ko.observable(data.name)
        self.BrokerAddress = ko.observable(data.address)
        self.BrokerNpName = ko.observable(data.npname)
        self.BrokerNpAddress = ko.observable(data.npadd)
        self.BrokerTelNo = ko.observable(data.telno)
    }
}



var DakhilIndividualTransfer = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()

    self.BrokerName = ko.observable()

    self.CertificateNo = ko.observable()
    self.StartSrNo = ko.observable()
    self.ShKitta = ko.observable()
    self.ShHolderNo = ko.observable()
    self.TotalShareAmount = ko.observable()
    self.EndSrNo = ko.observable()
    self.ShareCategory = ko.observable()
    self.PaidAmount = ko.observable()
    self.ShOwnerType = ko.observable()
    self.ShHolderName = ko.observable()
    self.isDuplicate = ko.observable()
    self.confirmDakhilEntry = ko.observable()
    self.RegistrationNo = ko.observable()
    self.Letter = ko.observable()
    self.TranNo = ko.observable()
    self.DakhilDate = ko.observable()
    self.DakhilCharge = ko.observable()
    self.Remarks = ko.observable()
    self.ExistingShareHolder = ko.observable()
    self.BuyerHolderNo = ko.observable()
    self.BuyerName = ko.observable()
    self.BuyerAddress = ko.observable()
    self.BrokerList = ko.observableArray([])
    self.TradeTypeList = ko.observableArray([])
    self.SelectedBroker = ko.observable()
    self.SelectedTradeType = ko.observable()

    self.TradeTypeList([
        { TradeTypeName: "1 Trading", TradeTypeId: "1" },
        { TradeTypeName: "2 Inheritance", TradeTypeId: "2" },
        { TradeTypeName: "3 Will", TradeTypeId: "3" },
        { TradeTypeName: "4 Claim", TradeTypeId: "4" }
    ])
    //gloabal variables
    var optionAUD = "";
    var maxKitta = 0;

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

    self.DakhilDate(formatDate(new Date))

    //Validation 
    self.Validation = function (data = null) {
        var errMsg = '';

        if (Validate.empty(self.CertificateNo())) {
            errMsg += "Please Enter Certificate No !!!";
        }
        if (Validate.empty(self.Letter())) {
            errMsg += "Please Enter Letter No !!!";
        }
        if (Validate.empty(self.DakhilDate())) {
            errMsg += "Please Enter Dakhil Date !!!";
        }
        if (Validate.empty(self.SelectedBroker())) {
            errMsg += "Please Select A Broker !!!";
        }
        if (Validate.empty(self.SelectedTradeType())) {
            errMsg += "Please Select A Trade Type !!!";
        }
        if (Validate.empty(self.DakhilCharge())) {
            errMsg += "Please Enter Dakhil Charge !!!";
        }
        if (Validate.empty(self.Remarks())) {
            errMsg += "Please Add Remarks!!!";
        }
        if (Validate.empty(self.BuyerHolderNo())) {
            errMsg += "Please Enter A Buyer Holder No!!!";
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

    self.LoadBrokerList = function () {
        Openloader()
        var companyCode =self.SelectedCompany()
        $.ajax({
            type: "post",
            url: '/DakhilTransfer/DakhilIndividualTransferEntry/GetBrokerList',
            data: { 'CompCode': companyCode },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new Broker(item)
                    });
                    self.BrokerList(mappedTasks);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }, complete: () => {
                Closeloader()
            }
        })
    }
    if (self.ValidateCompany) {
        self.LoadBrokerList();
    }

    //clear
    self.ClearControls = (data) => {
        self.CertificateNo('');
        self.StartSrNo('');
        self.EndSrNo('');
        self.ShKitta('');
        self.ShHolderNo('');
        self.TotalShareAmount('');
        self.ShareCategory('');
        self.PaidAmount('');
        self.ShOwnerType('');
        self.ShHolderName('');
        self.isDuplicate(false);
        self.confirmDakhilEntry(false);
        $('#RegistrationDiv,#BuyerDiv,#registrationNoDiv,#tranNoDiv').hide();
        if (data == "All") {
            $('#Save,#confirmDakhilEntry,#CertificateNo').attr('disabled', true);
            $('#AddButton,#UpdateButton,#DeleteButton').attr('disabled', false);
            $('#Save').val("Save");
            $('#Save').removeClass(["btn-danger", "btn-warning"]);
            $('#Save').addClass("btn-success");
        }
    }

    self.enableRegistrationDiv = () => {
        $('#RegistrationDiv').show()
        $('#Letter,#BrokerList,#TradeTypeList,#Remarks,#DakhilDate').attr('disabled',false)
        self.DakhilCharge(5)
        self.ExistingShareHolder(true)

    }
    self.DisableRegistrationDiv = () => {
        $('#RegistrationDiv').hide()
        $('#Letter,#BrokerList,#TradeTypeList,#Remarks,#DakhilDate').attr('disabled', true)
        self.DakhilCharge('')
        self.ExistingShareHolder(false)
        
    }
    self.enableBuyerDiv = () => {
        $('#BuyerDiv').show()
        $('#BuyerHolderNo').attr('disabled', false)
        $('#BuyerHolderNo').focus()
    }
    self.disableBuyerDiv = () => {
        $('#BuyerDiv').hide()
        $('#BuyerHolderNo').attr('disabled', true)
        self.BuyerHolderNo('')
        self.BuyerName('')
        self.BuyerAddress('')
    }
    //for choosing add or update or delete
    self.chooseOptions = function (data) {
        if (self.ValidateCompany()) {
            optionAUD = data;


            $('#CertificateNo').attr('disabled',false)
            $('#AddButton,#UpdateButton,#DeleteButton').attr('disabled', true)
           
          

            if (data == "A") {
                $('#RegistrationDiv,#BuyerDiv').hide();
                $('#Save').val("Save");
                $('#Save').removeClass(["btn-danger", "btn-warning"]);
                $('#Save').addClass("btn-success");

            }
            else {
                $('#registrationNoDiv,#tranNoDiv').show();
                $('#RegistrationDiv,#BuyerDiv').show();
                if (data == "U") {
                    $('#Save').val("Update");
                    $('#Save').removeClass(["btn-danger", "btn-success"]);
                    $('#Save').addClass("btn-warning");
                }
                else {
                    $('#Save').val("Delete");
                    $('#Save').removeClass(["btn-success", "btn-warning"]);
                    $('#Save').addClass("btn-danger");
                }

            }

            $('#CertificateNo').focus();
        }
    }

    //getting data based on certificate no
    self.GetCertificateInformation = (data) => {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.CertificateNo())) {
                var companyCode =self.SelectedCompany()
                Openloader()
                $.ajax({
                    type: "post",
                    url: '/DakhilTransfer/DakhilIndividualTransferEntry/GetCertificateInformation',
                    data: { 'CompCode': companyCode, 'CertificateNo': self.CertificateNo(),'SelectedAction':optionAUD },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            if (optionAUD == 'A') {
                                $('#Letter,#DakhilDate,#BrokerList,#TradeTypeList,#Remarks,#BuyerHolderNo').attr('disabled', false)

                                self.StartSrNo(result.responseData.srNoFrom)
                                self.ShKitta(result.responseData.shKitta)
                                self.ShHolderNo(result.responseData.shHolderNo)
                                self.TotalShareAmount(result.responseData.totalAmount)
                                self.EndSrNo(result.responseData.srNoTo)
                                switch (result.responseData.shareType) {
                                    case 1:
                                        self.ShareCategory('1 Preferential Share')
                                        break;
                                    case 2:
                                        self.ShareCategory('2 Ordinary Share')
                                        break;
                                    default:
                                        self.ShareCategory('3 Bonus Share')
                                        break;
                                }
                                switch (result.responseData.shOwnerType) {
                                    case 1:
                                        self.ShOwnerType('1 Promotor')
                                        break;
                                    case 2:
                                        self.ShOwnerType('2 Staff')
                                        break;
                                    default:
                                        self.ShOwnerType('3 Public')
                                        break;
                                }
                                self.PaidAmount(result.responseData.paidAmount)
                                self.ShHolderName(result.responseData.name)
                                if (result.responseData.dupliNo > 0) {
                                    self.isDuplicate(true)
                                    swal('Duplicate Certificate Issued : ' + result.responseData.dupliNo)
                                }
                                $('#confirmDakhilEntry').attr('disabled', false)
                            }
                            else {
                                self.StartSrNo(result.responseData.srNoFrom);
                                self.ShKitta(result.responseData.shKitta);
                                self.ShHolderNo(result.responseData.shHolderNo);
                                self.TotalShareAmount(result.responseData.totalAmount);
                                self.EndSrNo(result.responseData.srNoTo);
                                self.PaidAmount(result.responseData.paidAmount);
                                self.ShHolderName(result.responseData.name);
                                if (result.responseData.dupliNo > 0) {
                                    self.isDuplicate(true)
                                    swal('Duplicate Certificate Issued : ' + result.responseData.dupliNo)
                                }
                                switch (result.responseData.shareType) {
                                    case 1:
                                        self.ShareCategory('1 Preferential Share')
                                        break;
                                    case 2:
                                        self.ShareCategory('2 Ordinary Share')
                                        break;
                                    default:
                                        self.ShareCategory('3 Bonus Share')
                                        break;
                                }
                                switch (result.responseData.shOwnerType) {
                                    case 1:
                                        self.ShOwnerType('1 Promotor')
                                        break;
                                    case 2:
                                        self.ShOwnerType('2 Staff')
                                        break;
                                    default:
                                        self.ShOwnerType('3 Public')
                                        break;
                                }
                                self.confirmDakhilEntry(true);
                                self.Letter(result.responseData.letterNo);
                                self.RegistrationNo(result.responseData.regNo);
                                self.TranNo(result.responseData.tranNo);
                                self.DakhilDate(result.responseData.dakhilDt.substring(0, 10));
                                $('#BrokerList').val(result.responseData.brokerCode).trigger('change');
                                $('#TradeTypeList').val(result.responseData.tradeType).trigger('change');
                                //self.SelectedBroker(result.responseData.brokerCode);
                                //self.SelectedTradeType(result.responseData.tradeType);
                                self.DakhilCharge(result.responseData.charge);
                                self.Remarks(result.responseData.remarks);
                                self.ExistingShareHolder(result.responseData.bHoldExist);
                                self.BuyerHolderNo(result.responseData.bHolderNo);
                                self.BuyerName(result.responseData.buyerName);
                                self.BuyerAddress(result.responseData.buyerAddress);
                                $('#Save').attr('disabled', false)
                                $('#DakhilCharge').attr('disabled', false)

                                if (optionAUD == "D") {
                                    $('#Letter,#DakhilDate,#BrokerList,#TradeTypeList,#Remarks,#BuyerHolderNo').attr('disabled', true)
                                }
                                else {
                                    $('#Letter,#DakhilDate,#BrokerList,#TradeTypeList,#Remarks,#BuyerHolderNo').attr('disabled',false)
                                }
                            
                            }
                        } else {
                            alert('warning', result.message)
                            self.ClearControls()
                            $('#confirmDakhilEntry').attr('disabled', true)
                            $('#CertificateNo').focus();
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                        self.ClearControls()
                        $('#confirmDakhilEntry').attr('disabled', true)
                        $('#CertificateNo').focus();

                    }, complete: () => {
                        Closeloader()
                    }
                })

            } else {
                alert('warning', 'Please Enter Certificate No.')
                $('#CertificateNo').focus()
            }
           
        }
    }
    //registering new dakhil transfer
    self.confirmDakhilEntry.subscribe((data) => {
        if (data != false) {
            if (self.ValidateCompany()) {
                if (Validate.empty(self.CertificateNo()) == false) {
                    
                    self.enableRegistrationDiv()
                    $('#Letter').focus();
                }
                else {
                    alert('warning', 'Please Enter a Certificate No First !!!');
                    $('#CertificateNo').focus()
                    self.DisableRegistrationDiv()
                }
            }
        } else {
            self.DisableRegistrationDiv()
            $('#RegistrationDiv').hide()
        }
    })
    //
    self.ExistingShareHolder.subscribe((data) => {
        if (data != false) {
            self.enableBuyerDiv()
        } else {
            self.disableBuyerDiv()
        }
    })


    //buyerholder data
    self.GetBuyerInformation = (data) => {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.BuyerHolderNo())) {
                if (self.BuyerHolderNo() != self.ShHolderNo()) {
                    Openloader()
                    var companyCode =self.SelectedCompany()
                    $.ajax({
                        type: "post",
                        url: '/DakhilTransfer/DakhilIndividualTransferEntry/GetBuyerInformation',
                        data: { 'CompCode': companyCode, 'BHolderNo': self.BuyerHolderNo() },
                        datatype: "json", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (result) {
                            if (result.isSuccess) {
                                maxKitta = result.responseData.totalKitta + result.responseData.tKitta;
                                if (maxKitta < localStorage.getItem('company-max-kitta')) {
                                    self.BuyerName(result.responseData.fName + ' ' + result.responseData.lName);
                                    self.BuyerAddress(result.responseData.address);
                                    if (result.responseData.relativeName != null) {
                                        swal("The Holder is Relative of : " + result.responseData.relativeName)
                                    } else {
                                        alert("warning","No Relative Found !!!")
                                    }
                                } else {
                                    alert(`Buyer couldn't buy sharekitta greater than ` + localStorage.getItem('company-max-kitta'));
                                }

                                $('#Save').attr('disabled',false)
                            } else {
                                alert('warning', result.message)
                                self.BuyerHolderNo('')
                                self.BuyerName('')
                                self.BuyerAddress('')
                                $('#BuyerHolderNo').focus()
                                $('#Save').attr('disabled', false)

                            }
                        },
                        error: function (error) {
                            alert('error', error.message)
                            self.BuyerHolderNo('')
                            self.BuyerName('')
                            self.BuyerAddress('')
                            $('#BuyerHolderNo').focus()
                            $('#Save').attr('disabled', false)

                        }, complete: () => {
                            Closeloader()
                        }
                    })
                } else {
                    alert('warning', 'Share Holder and Buyer Holder Cannot Be the Same.');
                    self.BuyerHolderNo('')
                    self.BuyerName('')
                    self.BuyerAddress('')
                    $('#BuyerHolderNo').focus()
                }
            }
        }
    }
    //Save function
    self.Save = () => {
        if (self.ValidateCompany()) {
            if (self.Validation()) {
                if (optionAUD == 'U' || optionAUD == 'D') {
                    swal({
                        title: optionAUD == 'U'? "Update Dakhil Transfer":"Delete Dakhil Transfer",
                        text: optionAUD == 'U' ? "Update Dakhiled Certificate " + self.CertificateNo() : "Delete Dakhiled Certificate " + self.CertificateNo(),
                        icon: optionAUD == 'U'? "info":"warning",
                        buttons: true,
                        dangerMode: true
                    }).then((willSave) => {
                        if (willSave) {
                            self.SaveFunction()
                        }
                    });
                } else {
                    self.SaveFunction();
                }
                
                    
            }
        }
    }

    self.SaveFunction = () => {
        Openloader()
        var companyCode =self.SelectedCompany()
        var DTS = {
            'CompCode': companyCode, 'CertificateNo': self.CertificateNo(), 'ShHolderNo': self.ShHolderNo(),
            'BHolderNo': self.BuyerHolderNo(), 'SrNoFrom': self.StartSrNo(), 'SrNoTo': self.EndSrNo(),
            'BrokerCode': self.SelectedBroker(), 'LetterNo': self.Letter(), 'Charge': self.DakhilCharge(),
            'TradeType': self.SelectedTradeType(), 'DakhilDate': self.DakhilDate(), 'BHolderExist': true,
            'Remarks': self.Remarks(), 'TranKitta': self.ShKitta()
        }
        $.ajax({
            type: "POST",
            url: '/DakhilTransfer/DakhilIndividualTransferEntry/SaveDakhilTransfer',
            data: {
                'DakhilTransferData': DTS,
                'SelectedAction': optionAUD
            }, beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            datatype: "json",
            success: function (result) {
                if (result.isSuccess) {
                    swal(result.message)
                    self.ClearControls('All')
                } else {
                    alert('warning', result.message)
                    self.BuyerHolderNo('')
                    self.BuyerName('')
                    self.BuyerAddress('')
                    $('#BuyerHolderNo').focus()
                    $('#Save').attr('disabled', false)

                }
            },
            error: function (error) {
                alert('error', error.message)
                self.BuyerHolderNo('')
                self.BuyerName('')
                self.BuyerAddress('')
                $('#BuyerHolderNo').focus()
                $('#Save').attr('disabled', false)

            }, complete: () => {
                Closeloader()
            }
        })
    }

    //refresh
    self.refresh = function (data) {
       self.ClearControls("All")
    }
    
}

$(document).ready(function () {
    ko.applyBindings(new DakhilIndividualTransfer());

    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
    self.ClearControls("All")
    $('#Save,#confirmDakhilEntry,#CertificateNo').attr('disabled', true);
                                $('#Company').attr('disabled', false)

});