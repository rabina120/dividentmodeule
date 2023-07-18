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

function SellerShareHolder(data,maxRegNo,tranNo) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.certNo)
        self.ShHolderNo = ko.observable(data.shHolderNo)
        self.FName = ko.observable(data.fName);
        self.LName = ko.observable(data.lName);
        self.SHolderName = ko.observable(data.fName + ' ' + data.lName)
        self.ShKitta = ko.observable(data.shKitta)
        self.SrNoFrom = ko.observable(data.srNoFrom)
        self.SrNoTo = ko.observable(data.srNoTo)
        self.RegNo = ko.observable(maxRegNo)
        self.TranNo = ko.observable(tranNo)
    }
}



var DakhilManyToOneTransfer = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()


    self.BuyerHolderNo = ko.observable()
    self.BuyerName = ko.observable()
    self.BuyerAddress = ko.observable()
    self.BuyerTotalKitta = ko.observable()

    self.Letter = ko.observable()
    self.DakhilDate = ko.observable()
    self.TranNo = ko.observable()
    self.SellerHolderNo = ko.observable()
    self.SellerCertificateNo = ko.observable()
    self.SellerHolderName = ko.observable()
    self.SellerStartSrNo = ko.observable()
    self.SellerEndSrNo = ko.observable()
    self.RegNo = ko.observable()
    self.TranNo = ko.observable()



    self.BrokerList = ko.observableArray([])
    self.TradeTypeList = ko.observableArray([])
    self.SelectedBroker = ko.observable()
    self.SelectedTradeType = ko.observable()
    self.SellerHolderList = ko.observableArray([])


    self.TradeTypeList([
        { TradeTypeName: "1 Trading", TradeTypeId: "1" },
        { TradeTypeName: "2 Inheritance", TradeTypeId: "2" },
        { TradeTypeName: "3 Will", TradeTypeId: "3" },
        { TradeTypeName: "4 Claim", TradeTypeId: "4" }
    ])
    //gloabal variables
    var maxRegNo = 0;
    var maxKitta = 0;
    var sellerList = [];

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
        if (Validate.empty(self.BuyerHolderNo())) {
            errMsg += "Please Enter A Buyer Holder No !!!";
        }
        if (Validate.empty(self.Letter())) {
            errMsg += "Please Enter A Letter No !!!";
        }
        if (Validate.empty(self.SelectedBroker())) {
            errMsg += "Please Select A Broker !!!";
        }
        if (Validate.empty(self.SelectedTradeType())) {
            errMsg += "Please Select A TradeType !!!";
        }
        if (Validate.empty(self.DakhilDate())){
            errMsg += "Please Enter A DakhilDate !!!";
        }
        if (self.SellerHolderList().length <= 0) {
            errMsg += "Please Enter Atleast One Seller Certificate No !!!";
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
        var companyCode = self.SelectedCompany()
        $.ajax({
            type: "post",
            url: '/DakhilTransfer/DakhilManyToOneTransferEntry/GetBrokerList',
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
        $('#Letter,#BrokerList,#TradeTypeList,#DakhilDate,#SellerCertificateNo').attr('disabled', true);
        $('#BuyerHolderNo').focus();
        self.BuyerHolderNo('');
        self.BuyerName('');
        self.BuyerAddress('');
        self.BuyerTotalKitta('');
        self.Letter('');
        self.SelectedBroker('');
        self.SelectedTradeType('');
        $('#BrokerList').val(null).trigger('change');
        $('#TradeTypeList').val(null).trigger('change');
        self.DakhilDate(formatDate(new Date));
        self.SellerCertificateNo('');
        self.TranNo('');
        self.RegNo('');
        self.SellerHolderNo('');
        self.SellerHolderName('');
        self.SellerStartSrNo('');
        self.SellerEndSrNo('');
        self.SellerHolderList([]);
        sellerList = []
    }


    //buyerholder data
    self.GetBuyerInformation = (data) => {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.BuyerHolderNo())) {
                Openloader()
                var companyCode = self.SelectedCompany()
                $.ajax({
                    type: "post",
                    url: '/DakhilTransfer/DakhilManyToOneTransferEntry/GetBuyerInformation',
                    data: { 'CompCode': companyCode, 'BHolderNo': self.BuyerHolderNo() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            self.BuyerName(result.responseData.fName + " " + result.responseData.lName);
                            self.BuyerAddress(result.responseData.address);
                            self.BuyerTotalKitta(result.responseData.totalKitta);
                            if (result.responseData.relativeName != null) {
                                swal('The Holder is a Relative of : ' + result.responseData.relativeName)
                            }
                            else {
                                alert('info', 'No Holder Relative Found !!!');
                            }
                            $('#Letter,#BrokerList,#TradeTypeList,#DakhilDate,#SellerCertificateNo').attr('disabled', false)
                            $('#Letter').focus()
                        } else {
                            alert('warning', result.message)
                            $('#Letter,#BrokerList,#TradeTypeList,#DakhilDate,#SellerCertificateNo').attr('disabled', true)
                            

                        }
                    },
                    error: function (error) {
                        alert('error', error.message);
                        $('#Letter,#BrokerList,#TradeTypeList,#DakhilDate,#SellerCertificateNo').attr('disabled', true)
                       
                    }, complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }

    //selller information
    self.GetSellerCertificateInformation = (data) => {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.SellerCertificateNo())) {
                Openloader()
                $('#SellerCertificateNo').focusout()
                var companyCode = self.SelectedCompany()
                $.ajax({
                    type: "post",
                    url: '/DakhilTransfer/DakhilManyToOneTransferEntry/GetSellerCertificateInformation',
                    data: { 'CompCode': companyCode, 'CertificateNo': self.SellerCertificateNo() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            if (self.SellerHolderList().find(x => x.CertNo() == result.responseData.certNo) == undefined) {
                                if (result.responseData.shOwnerType == 2) {
                                    swal({
                                        title: "Certificate Under Staff Share",
                                        text: "This certificate is issued under staff's share,Proceed Dakhil?",
                                        icon: "warning",
                                        buttons: true,
                                        dangerMode: true
                                    }).then((willSave) => {
                                        if (willSave) {
                                            self.FillSellerInformation(result)
                                        } else {
                                            $('#SellerCertificateNo').focus()
                                        }
                                    });
                                }
                                else {
                                    self.FillSellerInformation(result)
                                }
                            } else {
                                alert('info', 'Certificate Already Added !!!');
                                $('#SellerCertificateNo').focus()

                            }
                        } else {
                            alert('error', result.message);
                            $('#SellerCertificateNo').focus()

                        }


                    },
                    error: function (error) {
                        alert('error', error.message);
                    }, complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }

    //filling datatable
    self.FillSellerInformation = function (result) {
        if (result != undefined) {
            maxKitta += result.responseData.shKitta;
            if (maxKitta < self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).MaxKitta()) {
                self.SellerHolderNo(result.responseData.shHolderNo);
                self.SellerHolderName(result.responseData.fName + ' ' + result.responseData.lName);
                self.SellerStartSrNo(result.responseData.srNoFrom);
                self.SellerEndSrNo(result.responseData.srNoTo);
                //self.TranNo(result.responseData.tranNo)
                //max reg no
                if (self.SellerHolderList().length == 0) {
                    var companyCode = self.SelectedCompany()
                    $.ajax({
                        type: "post", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        url: '/DakhilTransfer/DakhilManyToOneTransferEntry/GetMaxRegNo',
                        data: { 'CompCode': companyCode },
                        datatype: "json",
                        success: function (resultMax) {
                            if (resultMax.isSuccess) {
                                maxRegNo = resultMax.responseData;
                                self.RegNo(maxRegNo)
                               // self.TranNo(1)
                                var mappedTasks = new SellerShareHolder(result.responseData, maxRegNo, self.TranNo());
                                self.SellerHolderList.push(mappedTasks);
                            }
                            else
                                alert('error', result.message)
                        }
                        , error: (error) => {
                            alert('error', error.message)
                        }
                    })
                }
                //from table if table already filled
                else {
                    maxRegNo++;
                    self.RegNo(maxRegNo);
                   
                    var mappedTasks = new SellerShareHolder(result.responseData, maxRegNo, self.TranNo());
                    self.SellerHolderList.push(mappedTasks);

                }
                
            }
            else {
                swal('Maximum kitta limit exceed!!!');
                $('#SellerCetificateNo').focus()
            }
        }
    }
    //removing from data table
    self.RemoveFromSellerHolderList = function (data) {
        if (self.SellerHolderList().find(x => x.CertNo == data) != undefined) {
            self.SellerHolderList.remove(x => x.CertNo == data);
        }
    }
    //enabklind disableing save button
    self.SellerHolderList.subscribe(function () {
        if (self.SellerHolderList().length > 0)
            $('#Save').attr('disabled', false)
        else
            $('#Save').attr('disabled', true)

    })
    //refresh
    self.refresh = function (data) {
        self.ClearControls("All")
    }
    //save
    self.Save = () => {
        if (self.ValidateCompany()) {
            if (self.Validation()) {
                swal({
                    title: "Are You Sure ?",
                    text: "Are All the Information Correct ?",
                    icon: "warning",
                    buttons: true,
                    dangerMode: true
                }).then((willSave) => {
                    if (willSave) {
                        var companyCode = self.SelectedCompany()
                        Openloader()
                        
                        $.ajax({
                            type: "POST", beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            url: '/DakhilTransfer/DakhilManyToOneTransferEntry/SaveBatchDakhilTransfer',
                            data: {
                                'CompCode': companyCode, 'BuyerHolderNo': self.BuyerHolderNo(), 'sellers': ko.toJS(self.SellerHolderList()), 'Letter': self.Letter(),
                                'TradeType': self.SelectedTradeType(), 'Broker': self.SelectedBroker, 'DakhilDate': self.DakhilDate()
                            },
                            datatype: "json",
                            success: function (result) {
                                if (result.isSuccess) {
                                    alert('success', result.message)
                                }
                                else
                                    alert('error', result.message)
                            }
                            , error: (error) => {
                                alert('error', error.message)
                            }, complete: () => {
                                self.ClearControls()
                                Closeloader()
                            }
                        });
                    }
                });
            }
        }
    }

}

$(document).ready(function () {
    ko.applyBindings(new DakhilManyToOneTransfer());

    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
    //$('#tbl_Transfer_List').DataTable({
    //    searching: false,
    //    ordering: false,
    //    paging: false,
    //    scrollY: true
    //})
    $('#Company').removeAttr('disabled');
});