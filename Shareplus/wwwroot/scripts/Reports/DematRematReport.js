function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}
function DP(data) {
    var self = this;
    if (data != undefined) {
        self.DP_CODE = ko.observable(data.dP_CODE);
        self.DP_NAME = ko.observable(data.dP_NAME);
        self.Dp_Id_cds = ko.observable(data.dp_Id_cds);
        self.DisplayDP = ko.observable(data.dP_NAME + '  ' + data.dP_CODE);
    }
}
function ParaCompChild(data) {
    var self = this;
    if (data != undefined) {
        self.PCCompCode = ko.observable(data.compCode);
        self.PCISIN_NO = ko.observable(data.isiN_NO);
        self.PCShholderNo = ko.observable(data.shholderNo);
        self.PCDesc_share = ko.observable(data.desc_share);
        self.PCShownerType = ko.observable(data.shownerType);
    }
}
function CertificateDetail(data) {
    var self = this;
    if (data != undefined) {
        self.CDcompcode = ko.observable(data.compcode);
        self.CDCert_Id = ko.observable(data.cert_Id);
        self.CDShare_type = ko.observable(data.share_type);
        self.CDDescription = ko.observable(data.description);
        self.CDIssuse_Dt = ko.observable(data.issuse_Dt);
        self.CDStart_SrNo = ko.observable(data.start_SrNo);
        self.CDEnd_SrNo = ko.observable(data.end_SrNo);
    }
}


var DematRematReport = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();


    //DPLISt
    self.DP_CODE = ko.observable();
    self.DP_NAME = ko.observable();
    self.Dp_Id_cds = ko.observable();
    self.SelectedDP = ko.observable();
    self.DisplayDP = ko.observable();
    self.DPList = ko.observableArray([]);

    self.DemateReqFrom = ko.observable();
    self.DemateReqTo = ko.observable();
    self.EntryDateFrom = ko.observable();
    self.EntryDateTo = ko.observable();
    self.RegNoFrom = ko.observable();
    self.RegNoTo = ko.observable();
    self.HolderNoFrom = ko.observable();
    self.HolderNoTo = ko.observable();
    self.CertNoFrom = ko.observable();
    self.CertNoTo = ko.observable();

    //ParaCOmpChild ko lagi
    self.PCCompCode = ko.observable();
    self.PCISIN_NO = ko.observable();
    self.PCShholderNo = ko.observable();
    self.PCDesc_share = ko.observable();
    self.PCShownerType = ko.observable();
    self.SelectedISINNO = ko.observable();
    self.ParaCompChildList = ko.observableArray([]);

    //certificalte dwetail table
    self.CDcompcode = ko.observable();
    self.CDCert_Id = ko.observable();
    self.CDShare_type = ko.observable();
    self.CDDescription = ko.observable();
    self.CDIssuse_Dt = ko.observable();
    self.CDStart_SrNo = ko.observable();
    self.CDEnd_SrNo = ko.observable();
    self.SelectedCertDetails = ko.observable();
    self.CertificateDetailList = ko.observableArray([]);

    self.SelectedReportType = ko.observable();
    self.SelectedDataType = ko.observable();
    self.SelectedOrderBy = ko.observable();
    self.SelectedSecondaryReportType = ko.observable();
    self.SelectedDemateType = ko.observable();

    self.ReportTypeList = ko.observableArray([
        { ReportType: "D", ReportName: "Demate" },
        { ReportType: "R", ReportName: "Reversal" }
    ]);
    self.DataTypeList = ko.observableArray([
        { DataType: "P", DataTypeName: "Posted" },
        { DataType: "U", DataTypeName: "Unposted" },
        { DataType: "R", DataTypeName: "Rejected" }
    ]);
    self.OrderByList = ko.observableArray([
        { OrderByValue: "C", OrderByName: "Cert No" },
        { OrderByValue: "H", OrderByName: "Holder No" },
        { OrderByValue: "T", OrderByName: "Tran Date" },
        { OrderByValue: "R", OrderByName: "RegNo" }
    ]);
    self.SecondaryReportTypeList  = ko.observableArray([
        { SecondaryReportTypeValue: "D", SecondaryReportTypeName: "Detail List" },
        { SecondaryReportTypeValue: "S", SecondaryReportTypeName: "Summary List" },
        { SecondaryReportTypeValue: "DN", SecondaryReportTypeName: "DRN No" }
    ]);
    self.DemateTypeList = ko.observableArray([
        { DemateTypeValue: "DR", DemateTypeName: "DR" },
        { DemateTypeValue: "CA", DemateTypeName: "CA" }
    ]);

    //GLOBAL
    var record = []
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

    self.Validation = function (data) {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company <br/>";
        }

        if (Validate.empty(self.SelectedReportType())) {
            errMsg += "Please Select A Report Type <br/>"
        }
        if (Validate.empty(self.SelectedDataType())) {
            errMsg += "Please Select PSL Status <br/>"
        }

        if (Validate.empty(self.SelectedSecondaryReportType())) {
            errMsg += "Please Select Report Option <br/>"
        }
        
        if (errMsg == "") {
            return true;
        } else {
            alert('warning', errMsg)
            return false
        }
    }



    self.Refresh = function () {
        self.SelectedReportType('D')
        self.SelectedDataType('')
        self.SelectedOrderBy('C')
        self.SelectedSecondaryReportType('D')
        self.SelectedISINNO('')
        self.SelectedDemateType('DR')
        self.SelectedCertDetails('')
        self.SelectedDP('')
        self.DemateReqFrom('')
        self.DemateReqTo('')
        self.EntryDateFrom('')
        self.EntryDateTo('')
        self.RegNoFrom('')
        self.RegNoTo('')
        self.HolderNoFrom('')
        self.HolderNoTo('')
        self.CertNoFrom('')
        self.CertNoTo('')
        $("#CertDetails,#DPList").val('').trigger('change')
        
    }

    self.SelectedReportType.subscribe((data) => {


        if (data == "R") {
            self.SelectedDemateType('')
            self.SelectedCertDetails('')
            $('#DemateType,#CertDetails,#DPList,#ISINNO').attr('disabled', true)

            $('#RevFromLabel,#RevToLabel,#PostingFromLabel,#PostingToLabel,#RevNoFromLabel,#RevNoToLabel').show()
            $('#DemateReqFromLabel,#DemateReqToLabel,#EntryDateFromLabel,#EntryDateToLabel,#RegNoFromLabel,#RegNoToLabel').hide()
        }
        else {
            $('#DemateType,#CertDetails,#DPList,#ISINNO').attr('disabled', false)

            $('#RevFromLabel,#RevToLabel,#PostingFromLabel,#PostingToLabel,#RevNoFromLabel,#RevNoToLabel').hide()
            $('#DemateReqFromLabel,#DemateReqToLabel,#EntryDateFromLabel,#EntryDateToLabel,#RegNoFromLabel,#RegNoToLabel').show()
        }

    });

  

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

    //DP
    self.GetDP = function () {
        Openloader()
        $.ajax({
            type: 'POST',
            url: '/Reports/DematRematReport/GETDP',
            
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            dataType: 'json',
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new DP(item)
                    });
                    self.DPList(mappedTasks);

                    $('#DPList').attr('disabled', false);
                }
                else {
                    alert('error', result.message);
                    $('#DPList').attr('disabled', true);

                }
            }, error: function (error) {
                alert('error', error.message);
                $('#DPList').attr('disabled', true);
            },
            complete: () => {
                Closeloader()
            }


        })
    }
    self.GetDP();
    
    self.SelectedCompany.subscribe(function () {
        self.GetBonusIssueList();

        var companyCode = self.SelectedCompany()

        $.ajax({
            type: "post",
            url: '/Reports/DematRematReport/GetAllParaCompChild',
            data: { 'CompCode': companyCode },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            datatype: "json",
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new ParaCompChild(item)
                    });
                    self.ParaCompChildList(mappedTasks);
                    $('#ISINNO').attr('disabled', false);

                } else {
                    alert('warning', result.message);
                    $('#ISINNO').attr('disabled', true);
                }
            },
            error: function (error) {
                alert('error', error.message)
                $('#ISINNO').attr('disabled', true);
            }
        })

    })


    self.GetBonusIssueList = function () {
        if (self.ValidateCompany()) {
            var companyCode = self.SelectedCompany();
            $.ajax({
                type: 'POST',
                url: '/Reports/DematRematReport/GetDataFromCertificateDetail',
                data: { 'CompCode': companyCode },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                dataType: 'json',
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new CertificateDetail(item);
                        });
                        self.CertificateDetailList(mappedTasks);
                        $('#CertDetails').attr('disabled', false)
                    }
                    else {
                        alert('error', result.message);
                        $('#CertDetails').attr('disabled', true)
                    }

                }, error: function (error) {
                    alert('error', error.message);
                    $('#CertDetails').attr('disabled', true)
                }
            })
        }

    }


    self.GenerateReport = (data) => {

        if (self.Validation()) {
            Openloader()
            var ReportData = {
                "CompCode": self.SelectedCompany(),
                "CompEnName": self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName(),
                "ReportType": self.SelectedReportType(),
                "DataType": self.SelectedDataType(),
                "OrderBy": self.SelectedOrderBy(),
                "SecondaryReportType": self.SelectedSecondaryReportType(),
                "ISINNo": self.SelectedISINNO(),
                "DP": self.SelectedDP(),
                "DemateType": self.SelectedDemateType(),
                "CertDetails": self.SelectedCertDetails() == undefined || "" ? null : self.CertificateDetailList().find(x => x.CDCert_Id() == self.SelectedCertDetails()).CDCert_Id(),
                "DemateReqFrom": self.DemateReqFrom(),
                "DemateReqTo": self.DemateReqTo(),
                "EntryDateFrom": self.EntryDateFrom(),
                "EntryDateTo": self.EntryDateTo(),
                "RegNoFrom": self.RegNoFrom(),
                "RegNoTo": self.RegNoTo(),
                "HolderNoFrom": self.HolderNoFrom(),
                "HolderNoTo": self.HolderNoTo(),
                "CertNoFrom": self.CertNoFrom(),
                "CertNoTo": self.CertNoTo()
            }
            $.ajax({
                type: "POST",
                url: "/Reports/DematRematReport/GenerateReport",
                data: {

                    "ReportData": ReportData,
                    "ExportReportType": data
                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                dataType: "json",
                success: (result) => {
                    if (result.isSuccess) {
                        if (data == 'E') {
                            var fileName = result.message;
                            var a = document.createElement("a");
                            a.href = "data:application/octet-stream;base64," + result.responseData;
                            a.download = fileName;
                            a.click();
                        }
                        else {
                            var x = window.open();
                            if (x) {
                                x.document.open();
                                x.document.write('<iframe width="100%" height="100%" src="' + result.responseData.message + '"></iframe>');
                                x.document.title = result.message;
                                x.document.close();
                            } else {
                                alert('warning', 'Failed to Open Pdf File.');
                                alert('success', 'Downloading Pdf File.');
                                var fileName = result.message;
                                var a = document.createElement("a");
                                a.href = result.responseData.message;
                                a.download = fileName;
                                a.click();
                            }
                        }
                    } else {
                        alert('error', result.message)
                    }
                }, error: (error) => {
                    alert('error', error.message)
                }, complete: () => {
                    //self.Refresh()
                    Closeloader()
                }
            })
        }
    }

    self.SelectedOrderBy('R')
    self.SelectedSecondaryReportType('D')
}
$(function () {
    ko.applyBindings(new DematRematReport())

    $('#simple-date1 .input-group.date').datepicker({
        
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    }); $('#simple-date2 .input-group.date').datepicker({
        "setDate": new Date(),
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    }); $('#simple-date3 .input-group.date').datepicker({
        
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    }); $('#simple-date4 .input-group.date').datepicker({
        
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
    self.Refresh()
})