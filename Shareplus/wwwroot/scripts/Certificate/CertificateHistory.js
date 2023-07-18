function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compCode + " " + data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}
function ShOwnerType(data) {
    var self = this;
    if (data != undefined) {
        self.shOwnerType = ko.observable(data.shOwnerType);
        self.shOwnerTypeName = ko.observable(data.shOwnerTypeName);
    }
}

function DAKHILTRANSFER(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.CompCode);
        self.CertNo = ko.observable(data.CertNo);
        self.BuyerName = ko.observable(data.buyerName);
        self.SHolderNo = ko.observable(data.sHolderNo);
        self.BHolderNo = ko.observable(data.bHolderNo);
        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.DakhilDt = ko.observable(data.dakhilDt);
        self.TranNo = ko.observable(data.tranNo);
        self.RegNo = ko.observable(data.regNo);
        self.Remarks = ko.observable(data.remarks);
        self.TranKitta = ko.observable(data.tranKitta);
        self.batchno = ko.observable(data.batchno);
        self.SelectedShownerType = ko.observable(data.shOwnerType);
    }
}
var CertificateHistoryviewModel = function () {

    //Compcode
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    //ShareType
    self.ShownerTypes = ko.observableArray([]);
    self.SelectedShownerType = ko.observable();
    self.shOwnerType = ko.observable();
    self.shOwnerTypeName = ko.observable();
   
    self.DAKHILTRANSFERLIST = ko.observableArray([]);
    self.CertNo = ko.observable();
    self.ShareHolderNo = ko.observable();
    self.StartSerialNo = ko.observable();
    self.EndSeriaLNo = ko.observable();
    self.ShareCategory = ko.observable();
    self.ShareOwnerType = ko.observable();
    self.PKitta = ko.observable();
    self.HolderName = ko.observable();
    self.PStatus = ko.observable();
    self.Address = ko.observable();
    self.compcode = ko.observable();
    self.SelectedShownerType = ko.observable();
    var companyCode = self.SelectedCompany()
    self.compcode(companyCode);

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


    //ShareOwnerType
    self.LoadShareOwnerType = function () {
        $.ajax({
            type: "post",
            url: '/Common/Common/GetShareOwnerType',
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            datatype: "json",
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new ShOwnerType(item)
                    });
                    self.ShownerTypes(mappedTasks);

                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
       
    }
    self.LoadShareOwnerType();

    self.GetCertInformation = function () {
        var companyCode = self.SelectedCompany()
        if (self.CertNo() != ' ' || self.SelectedCompany() != null) {
            Openloader();
            $.ajax({
                type: "post",
                url: '/Certificate/CertificateHistory/GetCertInformation',
                data: { 'CertNo': self.CertNo(), 'compcode': self.SelectedCompany() },
                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {

                    if (result.isSuccess) {
                        var data = result.responseData;
                        self.ShareHolderNo(result.responseData.shholderNo);
                        self.StartSerialNo(result.responseData.srNoFrom);
                        self.EndSeriaLNo(result.responseData.srNoTo);
                        self.ShareCategory(result.responseData.shareType);
                        self.ShareOwnerType(result.responseData.shOwnerType);
                        self.PKitta(result.responseData.shKitta);
                        self.HolderName(result.responseData.name);
                        self.PStatus(result.responseData.certStatus);
                        self.Address(result.responseData.address);
                        $('#btnClear').attr("disabled", false)
                        self.SelectedShownerType(result.responseData.shOwnerType);
                       
                        self.LoadData();


                    }
                    else {
                        alert('warning', result.message)
                       self.Clear();
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: function (result) {
                    Closeloader();
                }
            })
        }
        else {

        }
    }
    self.LoadData = function () {

        if (self.CertNo() != '' || self.SelectedCompany() != null) {
            $('#Dakhiltransfer').DataTable().clear()
            $('#Dakhiltransfer').DataTable().destroy();
            $.ajax({
                type: "post",
                url: '/Certificate/CertificateHistory/LoadCertificateTable',
                data: { 'CertNo': self.CertNo(), 'compcode': self.SelectedCompany() },
                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    var data = result.responseData;
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new DAKHILTRANSFER(item)
                    });
                    self.DAKHILTRANSFERLIST(ko.toJS(mappedTasks));
                    alert('success', result.message);
                   
                    $('#Dakhiltransfer').DataTable({
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
                        "scrollCollapse": true,
                        dom: 'Bfrtip',
                        buttons: [
                            {
                                extend: 'excelHtml5',
                                title: 'CertificateHistory_CertNo_' + self.CertNo()
                            }
                        ]

                    });
                   
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })
        }
    }
  
    self.Clear = function () {
        self.ShareHolderNo(null);
        self.StartSerialNo(null);
        self.EndSeriaLNo(null);
        self.ShareCategory();
        self.ShareOwnerType();
        self.PKitta(null);
        self.HolderName(null);
        self.PStatus(null);
        self.Address(null);
        self.ShareCategory(null);
        self.DAKHILTRANSFERLIST(null);
        self.SelectedShownerType(null);
        $('#Dakhiltransfer').DataTable().clear()
        $('#Dakhiltransfer').DataTable().destroy();
        self.DakhilTableNormal()
        self.CertNo(null);
    }
    self.DakhilTableNormal = function () {
        $('#Dakhiltransfer').DataTable({
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
            "scrollCollapse": true,
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'excelHtml5',
                    title: 'CertificateHistory-' + self.CertNo()
                }
            ]

        });
    }
    self.DakhilTableNormal()
}

$(document).ready(function () {


    ko.applyBindings(new CertificateHistoryviewModel());
  

});