function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode);
        self.CompEnName = ko.observable(data.compCode + " " + data.compEnName);
        self.MaxKitta = ko.observable(data.maxKitta);
    }
}
function CertificateList(data) {
    var self = this;
    if (data != undefined) {
        self.CertNo = ko.observable(data.certNo);
        self.SrNoFrom = ko.observable(data.certNo);
        self.SrNoTo = ko.observable(data.certNo);
        self.ShKitta = ko.observable(data.certNo);
        self.Action = ko.observable(data.certNo);
        self.Address = ko.observable(data.Address);
    }
}

var CertificatePrintViewModal = function ()  {
    //Compcode
    var self = this;
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    self.Shholderno = ko.observable();
    self.Name = ko.observable();
    self.Address = ko.observable();
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
                    if (!Validate.empty(localStorage.getItem('company-code'))) { self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode()); }
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
    self.NpName = ko.observable();
    self.NpAdd = ko.observable();

    self.GetHolderInformation = () => {
        if (!Validate.empty(self.SelectedCompany())) {
            $.ajax({
                type: "post",
                url: '/Certificate/Print/GetHolderInformation',

                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                }, data: { 'compcode': self.SelectedCompany(), 'shholderno': self.Shholderno() },
                success: function (result) {
                    if (result.isSuccess) {
                        self.Name(result.responseData.fName + ' ' + result.responseData.lName);
                        self.Address(result.responseData.address)
                        self.NpName(result.responseData.npName)
                        self.NpAdd(result.responseData.npAdd)
                        self.loadDataTable();
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

    self.CerificateList = ko.observableArray([]);

    self.loadDataTable = function () {
        self.CerificateList([]);
        $.ajax({
            type: "post",
            url: '/Certificate/Print/GetAllCertificates',

            datatype: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            }, data: { 'compcode': self.SelectedCompany(), 'shholderno': self.Shholderno() },
            success: function (result) {
                if (result.isSuccess) {
                    
                    self.CerificateList(
                        result.responseData.map(x => ({
                        ...x,
                        Address: self.Address()
                    })))
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
        //postReq(
        //    '/Certificate/Print/GetAllCertificates',
        //    { 'compcode': self.SelectedCompany(), 'shholderno': self.Shholderno() },
        //    null,
        //    resp => {
        //        console.log(resp, "-------------")
        //    },
        //    null
        //)


        //dataTable = $('#DT_load').dataTable({
        //    "ajax": {
        //        "url": "/Certificate/Print/GetAllCertificates",
        //        "type": "POST", beforeSend: function (xhr) {
        //            xhr.setRequestHeader("XSRF-TOKEN",
        //                $('input:hidden[name="__RequestVerificationToken"]').val());
        //        }, data: { 'compcode': self.SelectedCompany(), 'shholderno': self.Shholderno() },
        //        "datatype": "json"
        //    },
        //    "columns": [
        //        { "data": "CertNo", "width": "5%" },
        //        { "data": "SrnoFrom", "width": "15%" },
        //        { "data": "SrnoTo", "width": "15%" },
        //        { "data": "ShKitta", "width": "10%" },
               
        //        {
        //            "data": "CertNo",
        //            "render": function (data, type, row, meta) {
        //                return `<div class="text-center">
        //                <a  onclick=ShowPrintModal(${row}) class='btn btn-primary text-white' style='cursor:pointer;'>
        //                    Rights
        //                </a>
        //                </div>`;
        //            },
        //            "width": "10%"
        //        }
        //    ],
        //    "dom": 'Bfrtip',
            
        //    "aoColumnDefs": [
        //        {
        //            "bSortable": false,
        //            "aTargets": [-1]
        //        }
        //    ],
        //    "language": {
        //        "emptyTable": "no data found"
        //    }
        ////});
    }

    self.SelectedCretificateList = ko.observableArray();
    self.SelectCertificates = function (data) {
        console.log(data);
        if (self.SelectedCretificateList()?.find(x => x.CERTNO == data.CERTNO)) toastr.error("This Certificate has been added already!!!");
        else self.SelectedCretificateList.push(data);
    }
    self.RemoveSelectedCertificate = function (data) {
        self.SelectedCretificateList.remove(data)
    }

    self.PrintSelectedCertificates = function () {
        if (self.SelectedCretificateList().length > 0) {
            $.ajax({
                type: "post",
                url: '/Certificate/Print/PrintCertificates',
                data: { list: self.SelectedCretificateList(), CompCode: self.SelectedCompany() },
                datatype: "json",
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
                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })
            //postReq(
            //    '/Certificate/Print/PrintCertificates',
            //    { list: self.SelectedCretificateList(), CompCode: self.SelectedCompany() },
            //    null,
            //    result => {
            //        console.log(result)
            //        var fileName = result.Message;
            //        var a = document.createElement("a");
            //        a.href = "data:application/octet-stream;base64," + result.ResponseData;
            //        a.download = fileName;
            //        a.click();
            //    },
            //    null
            //)
            //console.log(self.SelectedCretificateList())
        }
        else toastr.error("Please Select at least one certificate to print!!!");
    }

//    var ShowPrintModal = (row) => {

//    }
//    $.ajax({
//        type: "post",
//        url: '/certificate/print/print',

//        datatype: "json",
//        beforeSend: function (xhr) {
//            xhr.setRequestHeader("XSRF-TOKEN",
//                $('input:hidden[name="__RequestVerificationToken"]').val());
//        },
//        success: function (result) {
//            $("#modalCompany").modal('hide');
//            if (result.isSuccess) {
//                var mappedTasks = $.map(result.responseData, function (item) {
//                    return new ParaComp(item)
//                });
//                self.CompanyDetails(mappedTasks);
//                if (!Validate.empty(localStorage.getItem('company-code'))) { self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode()); }
//                // $("#Company").attr("disabled", true);
//            } else {
//                alert('warning', result.message)
//            }
//        },
//        error: function (error) {
//            alert('error', error.message)
//        }
//    })

}

$(document).ready(function () {
    ko.applyBindings(new CertificatePrintViewModal());
});