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
var CertificateListviewModel = function () {
    self.SelectedAction = ko.observable();
    //Compcode
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();
    self.ListCertificateRadio = ko.observable();
    self.FromDate = ko.observable();
    self.ToDate = ko.observable();
    self.HNoFrom = ko.observable();
    self.HNoTo = ko.observable();
    self.CNoFrom = ko.observable();
    self.CNoTo = ko.observable();
    self.ShareKittaFrom = ko.observable();
    self.ShareKittaTo = ko.observable();
    self.SerialNoFrom = ko.observable();
    self.SerialNoTo = ko.observable();
    self.ShareType = ko.observable();

    self.PostedUnposted = ko.observable();
    self.ShownerTypes = ko.observableArray([]);
    self.SelectedShownerType = ko.observable();
    self.SelectedShareType = ko.observable();
    self.ShareTypes = ko.observableArray([]);
    self.OrderBy = ko.observable();
    self.SelectedDataType = ko.observable();
    self.SelectedOrderBy = ko.observable();
    self.DataTypeList = ko.observableArray([
        { DataType: "P", DataTypeName: "Posted" },
        { DataType: "U", DataTypeName: "UnPosted" }
    ]);
    SelectedListType = ko.observable();

    self.ShownerTypesList = ko.observableArray([
        { shOwnerType: "1", shOwnerTypeName: "PROMOTER" },
        { shOwnerType: "2", shOwnerTypeName: "STAFF" },
        { shOwnerType: "3", shOwnerTypeName: "PUBLIC" }
    ]);
    self.ListTypeList = ko.observableArray([
        { ListType: "F", ListTypeName: "Full Paid" },
        { ListType: "D", ListTypeName: "Duplicate" },
        { ListType: "U", ListTypeName: "Unpaid" }
    ]);

    self.OrderByList = ko.observableArray([
        { OrderBy: "C", OrderByName: "Cert No" },
        { OrderBy: "H", OrderByName: "Holder No" },
        { OrderBy: "S", OrderByName: "Serial No" }
    ]);
    //ShareOwnerType



    self.LoadShareTypes = function () {
        $.ajax({
            type: "POST", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            url: '/Common/Common/GetShareTypes',
            data: { 'CompCode': self.SelectedCompany() },
            datatype: "json",
            success: function (result) {
                if (result.isSuccess) {
                    self.ShareTypes(result.responseData);

                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })

    }
    self.SelectedCompany.subscribe(function () {
        self.LoadShareTypes()
    })

    

    //DistributedCertificatesListValidation 
    self.DistributedCertificatesListValidation = function () {
        var errMsg = ""
        if (Validate.empty(self.FromDate())) {
            errMsg += "Please Select From Date <br/>"
        }
        if (Validate.empty(self.ToDate())) {
            errMsg += "Please Select To Date <br/>"
        }
        if (errMsg == "") {
            return true;
        } else {
            alert('warning', errMsg)
            return false
        }
    }
    //DuplicateCertificatesListValidation
    self.DuplicateCertificatesListValidation = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedListType())) {
            errMsg += "Please Select A List Type <br/>"
        }
        if (errMsg == "") {
            return true;
        } else {
            alert('warning', errMsg)
            return false
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


    self.ToggleCerificateMode = function () {
        var x = document.getElementById("hideDUCL");
        var y = document.getElementById("hideDC");
        var z = document.getElementById("hideAC");
        y.style.display = "none";
        x.style.display = "none";
        z.style.display = "none";
        if (self.ListCertificateRadio() == 'DUCL') {
            x.style.display = "block";
            return;
        }
        if (self.ListCertificateRadio() == 'DC') {
            y.style.display = "block";
            return;
        }
        if (self.ListCertificateRadio() == 'AC') {
            z.style.display = "block";
            
            return;
        }
    };

    self.DistributedCertificatesList = function (data) {
        if (self.ValidateCompany()) {
            if (self.DistributedCertificatesListValidation()) {

                Openloader()
                var ReportData = {
                    "CompCode": self.SelectedCompany(),
                    "CompEnName": self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName(),
                    /*"DataType": self.SelectedDataType(),*/

                    "HolderNoFrom": self.HNoFrom(),
                    "HolderNoTo": self.HNoTo(),

                    "CertNoFrom": self.CNoFrom(),
                    "CertNoTo": self.CNoTo(),

                    "FromDate": self.FromDate(),

                    "ToDate": self.ToDate()

                }
                self.SelectedAction(data);
                $.ajax({
                    type: "post",
                    url: '/Certificate/CertificateList/DistributedUnDistributedList',
                    data: {
                        
                        "ReportDataDistributedCertificatesList": ReportData,
                        'OrderBy': self.SelectedOrderBy, 'Listtype': self.SelectedDataType(), 'sharetype': self.SelectedShareType(), 'SelectedAction': self.SelectedAction()

                    },
                    dataType: 'json',
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {

                            var fileName = result.message;
                            var a = document.createElement("a");
                            a.href = "data:application/octet-stream;base64," + result.responseData;
                            a.download = fileName;
                            a.click();
                        }
                        else {
                            console.log('error=>', result.message)
                            alert('error', result.message);
                        }

                    }, error: function (error) {
                        console.log('error=>', error.message)
                        alert('error', error.message)
                    },
                    complete: () => {

                        Closeloader()

                    }
                })


            }
           
        }
      

    }

    self.DistributedCertificatesListPdf = function (data) {
        if (self.ValidateCompany()) {
            if (self.DistributedCertificatesListValidation()) {

                Openloader()
                var ReportData = {
                    "CompCode": self.SelectedCompany(),
                    "CompEnName": self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName(),
                    /*"DataType": self.SelectedDataType(),*/

                    "HolderNoFrom": self.HNoFrom(),
                    "HolderNoTo": self.HNoTo(),

                    "CertNoFrom": self.CNoFrom(),
                    "CertNoTo": self.CNoTo(),

                    "FromDate": self.FromDate(),

                    "ToDate": self.ToDate()

                }
                self.SelectedAction(data);
                $.ajax({
                    type: "post",
                    url: '/Certificate/CertificateList/DistributedUnDistributedListReport',
                    data: {
                       
                        "ReportDataDistributedCertificatesList": ReportData,
                        'OrderBy': self.SelectedOrderBy, 'Listtype': self.SelectedDataType(), 'sharetype': self.SelectedShareType(), 'SelectedAction': self.SelectedAction()

                    },
                    dataType: 'json',
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            var fileName = result.message;
                            var a = document.createElement("a");
                            a.href = "data:application/octet-stream;base64," + result.responseData;
                            a.download = fileName;
                            a.click();
                           /* var x = window.open();*/
                            //if (x) {
                            //    x.document.open();
                            //    x.document.write('<iframe width="100%" height="100%" src="' + result.responseData.message + '"></iframe>');
                            //    x.document.title = result.message;
                            //    x.document.close();
                            //} else {
                            //    alert('warning', 'Failed to Open Pdf File.');
                            //    alert('success', 'Downloading Pdf File.');
                            //    var fileName = result.message;
                            //    var a = document.createElement("a");
                            //    a.href = result.responseData.message;
                            //    a.download = fileName;
                            //    a.click();
                            //}
                        }
                        else {
                            console.log('error=>', result.message)
                            alert('error', result.message);
                        }

                    }, error: function (error) {
                        console.log('error=>', error.message)
                        alert('error', error.message)
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }

    self.DisplayAllCertificateLists = function () {

    }

    self.GetALLCerticateLists = function () {
        if (self.ValidateCompany()) {

            Openloader()
            var ReportData = {
                "CompCode": self.SelectedCompany(),
                "CompEnName": self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName(),
                "HolderNoFrom": self.HNoFrom(),
                "HolderNoTo": self.HNoTo(),
                "CertNoFrom": self.CNoFrom(),
                "CertNoTo": self.CNoTo(),
                "ShareKittaFrom": self.ShareKittaFrom(),
                "ShareKittaTo": self.ShareKittaTo(),
                "SerialNoFrom": self.SerialNoTo(),
                "SerialNoTo": self.SerialNoTo()

            }

            $.ajax({
                type: "POST",
                url: '/Certificate/CertificateList/AllCertiicateList',
                data: {
                  
                    "ReportDataForAllCertificate": ReportData,
                    'OrderBy': self.SelectedOrderBy, 'ShareOwnerType': self.SelectedShownerType()

                },
                dataType: 'json',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {

                        var fileName = result.message;
                        var a = document.createElement("a");
                        a.href = "data:application/octet-stream;base64," + result.responseData;
                        a.download = fileName;
                        a.click();
                    }
                    else {
                        console.log('error=>', result.message)
                        alert('error', result.message);
                    }

                }, error: function (error) {
                    console.log('error=>', error.message)
                    alert('error', error.message)
                },
                complete: () => {
                    Closeloader()
                }
            })
        }
    }

    self.GetALLCerticateListsPdf = function () {
        if (self.ValidateCompany()) {

            Openloader()
            var ReportData = {
                "CompCode": self.SelectedCompany(),
                "CompEnName": self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName(),
                "HolderNoFrom": self.HNoFrom(),
                "HolderNoTo": self.HNoTo(),
                "CertNoFrom": self.CNoFrom(),
                "CertNoTo": self.CNoTo(),
                "ShareKittaFrom": self.ShareKittaFrom(),
                "ShareKittaTo": self.ShareKittaTo(),
                "SerialNoFrom": self.SerialNoTo(),
                "SerialNoTo": self.SerialNoTo()

            }

            $.ajax({
                type: "POST",
                url: '/Certificate/CertificateList/AllCertiicateListReport',
                data: {
                 
                    "ReportDataForAllCertificate": ReportData,
                    'OrderBy': self.SelectedOrderBy, 'ShareOwnerType': self.SelectedShownerType()

                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                dataType: 'json',
                success: function (result) {
                    if (result.isSuccess) {
                        var fileName = result.message;
                        var a = document.createElement("a");
                        a.href = "data:application/octet-stream;base64," + result.responseData;
                        a.download = fileName;
                        a.click();
                        //var x = window.open();
                        //if (x) {
                        //    x.document.open();
                        //    x.document.write('<iframe width="100%" height="100%" src="' + result.responseData.message + '"></iframe>');
                        //    x.document.title = result.message;
                        //    x.document.close();
                        //} else {
                        //    alert('warning', 'Failed to Open Pdf File.');
                        //    alert('success', 'Downloading Pdf File.');
                        //    var fileName = result.message;
                        //    var a = document.createElement("a");
                        //    a.href = result.responseData.message;
                        //    a.download = fileName;
                        //    a.click();
                        //}
                    }
                    else {
                        console.log('error=>', result.message)
                        alert('error', result.message);
                    }

                }, error: function (error) {
                    console.log('error=>', error.message)
                    alert('error', error.message)
                },
                complete: () => {

                    Closeloader()

                }
            })


        }
    }

    self.DisplayDuplicateCerticateLists = function () {
        if (self.ValidateCompany()) {
            if (self.DuplicateCertificatesListValidation()) {
                Openloader()

                $.ajax({
                    type: 'POST',
                    url: '/Certificate/CertificateList/DisplayDuplicateLists',
                    data: {
                     
                        'CompCode': self.SelectedCompany(), 'OrderBy': self.SelectedOrderBy(), 'Listtype': self.SelectedListType()
                    },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    dataType: 'json',
                    success: function (result) {
                        if (result.isSuccess) {
                            /* var arr = [result.message];*/
                            var fileName = result.message;
                            var a = document.createElement("a");
                            a.href = "data:application/octet-stream;base64," + result.responseData;
                            a.download = fileName;
                            a.click();
                        }
                        else {
                            console.log('error=>', result.message)
                            alert('error', result.message);
                        }

                    }, error: function (error) { 
                        console.log('error=>', error.message)
                        alert('error', error.message)
                    },
                    complete: () => {

                        Closeloader()
                    }
                })
            }
        }
    }

    self.Validation = function (data) {
        var errMsg = ""
        if (errMsg == "") {
            return true;
        } else {
            alert('warning', errMsg)
            return false
        }
    }
    self.SelectedOrderBy('C')
    self.SelectedShownerType('3')
}
$(document).ready(function () {


    $('#simple-date1 .input-group.date').datepicker({
        "setDate": new Date(),
        format: 'yyyy-mm-dd',
        todayHighlight: true,
        autoclose: true,
        endDate: '+0d'
    });
    $('#simple-date2 .input-group.date').datepicker({
        "setDate": new Date(),
        format: 'yyyy-mm-dd',
        todayHighlight: true,
        autoclose: true,
        endDate: '+0d'
    });
    ko.applyBindings(new CertificateListviewModel());

});