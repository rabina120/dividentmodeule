
function Paracomp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}



function CertificateDetail(data) {
    //var self = this;
    if (data != undefined) {
        this.tcertno = ko.observable(data.certno)
        this.tshkitta = ko.observable(data.shkitta)
        this.tsrnofrom = ko.observable(data.srnofrom)
        this.tsrnoto = ko.observable(data.srnoto)
        this.tbo_acct_no = ko.observable(data.bo_acct_no)
        this.tapp_date = ko.observable(data.app_date == null || undefined ? "" : data.app_date.substring(0, 10))
        this.tregno = ko.observable(data.regno)
        this.tisin_no = ko.observable(data.isin_no)
        this.tshholderNo = ko.observable(data.aTTShHolder.shholderNo);
        this.Selected = ko.observable()

    }

}

var ReversalEntry = function () {
    //ParaComp
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    self.ShholderNo = ko.observable();
    self.HOLDERNAME = ko.observable();
    self.TotalKitta = ko.observable();

    self.DateFrom = ko.observable();
    self.DateTo = ko.observable();
    self.PostingRemarks = ko.observable()

    self.compcode = ko.observable();
    self.ShHolderNoC = ko.observable();
    self.CertNo = ko.observable();
    self.SrNoFrom = ko.observable();
    self.SrNoTo = ko.observable();
    self.CertStatus = ko.observable();
    self.ShKitta = ko.observable();
    self.ShOwnerType = ko.observable();
    self.ShareType = ko.observable();
    self.CertificateList = ko.observableArray([]);


    self.DRNNO = ko.observable()
    self.DateFrom(formatDate(new Date))
    self.DateTo(formatDate(new Date))

    //global
    var record = [];
    //Load company to select option
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
                    var mappedTasks = $.map(result.responseData, function (item)
                    {
                        return new Paracomp(item);
                    });
                    self.CompanyDetails(mappedTasks);
                    if (!Validate.empty(localStorage.getItem('company-code'))) {
                        self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode())
                    }
                    // $("#Company").attr("disabled", true);
                }
                else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })


    }
    self.LoadCompany();


    self.SelectAll = ko.computed({
        read: () => !self.CertificateList().find(x => !x.Selected()),
        write: t => self.CertificateList().forEach(x => x.Selected(t)),
    })


    self.SaveReversal = function (data) {
        if (self.Validation()) {
            Openloader()
            for (var i = 0; i < $('#tbl_Certificate_List').DataTable().data().count(); i++) {
                var x = i + 1;
                var Check = $($('#tbl_Certificate_List').DataTable().row(i).nodes()).find('input').prop('checked');
                if (Check != undefined && Check != "" && Check != false) {


                    var DIS = {
                        certno: $('#tbl_Certificate_List').DataTable().row(i).data()[1],
                        shkitta: $('#tbl_Certificate_List').DataTable().row(i).data()[2],
                        srnofrom: $('#tbl_Certificate_List').DataTable().row(i).data()[3],
                        srnoto: $('#tbl_Certificate_List').DataTable().row(i).data()[4],
                        bo_acct_no: $('#tbl_Certificate_List').DataTable().row(i).data()[5],
                        app_date: $('#tbl_Certificate_List').DataTable().row(i).data()[6],
                        regno: $('#tbl_Certificate_List').DataTable().row(i).data()[7],
                        isin_no: $('#tbl_Certificate_List').DataTable().row(i).data()[8],
                        aTTShHolder: {
                            shholderNo: $('#tbl_Certificate_List').DataTable().row(i).data()[9]
                        }

                    }

                    record.push(DIS)
                }

            }
            $.ajax({
                type: "POST",
                url: "/CDS/CertificateReversalEntry/SaveReversalCertificate",
                data: {
                   
                    CompCode: self.SelectedCompany(),
                    SelectedAction: data,
                    Remarks: self.PostingRemarks(),
                    certificates: record,
                    DRNNO: self.DRNNO(),
                    ShHolderNo: self.ShholderNo()

                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.isSuccess) {
                        alert('success', result.message)

                    }
                    else {
                        alert('error', result.message)

                    }
                }, error: (err) => {
                    alert('error', err.message)

                },
                complete: () => {
                    record = []
                    Closeloader()
                    self.ClearControl()
                }
            })
        }

    }

    self.Validation = (data) => {
        var errMsg = "";
        if (data == "R") {
            if (Validate.empty(self.DateFrom())) {
                errMsg += "Please Enter Date From !!!</br>";
            }
            if (Validate.empty(self.DateTo())) {
                errMsg += "Please Enter Date To !!!</br>";
            }
        } else {
            if ($('#tbl_Certificate_List').find('input[type=checkbox]:checked').length == 0) {
                errMsg += "Please Select A Record !!!</br>";
            }
            if (Validate.empty(self.PostingRemarks())) {
                errMsg += "Please Enter Posting Remarks !!!</br>";
            }
            if (Validate.empty(self.ShholderNo())) {
                errMsg += "Please Enter Shholderno !!!</br>";
            }
            if (Validate.empty(self.DRNNO())) {
                errMsg += "Please Enter DRNno !!!</br>";
            }
        }

        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
    }

    self.ClearControl = () => {
        self.NormalDataTable();
        record = []
        self.ShholderNo('')
        self.HOLDERNAME('')
        self.TotalKitta('')
        self.DRNNO('')
        self.PostingRemarks('')
        $('#DIV_DRN').hide()
        $('#DRNNo,#DRNDataButton,#Save').attr('disabled', true)

    }
    self.GenerateReport = () => {
        if (self.ValidateCompany()) {
            if (self.Validation("R")) {
                Openloader()
                $.ajax({
                    type: "post",
                    url: "/CDS/CertificateReversalEntry/GenerateReport",
                    data: {
                       
                        "CompCode": self.SelectedCompany(),
                        "DateFrom": self.DateFrom(),
                        "DateTo": self.DateTo(),
                        "CompEnName": self.CompanyDetails().find(x => x.CompCode() == self.SelectedCompany()).CompEnName()
                    },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: (result) => {
                        if (result.isSuccess) {
                            $('#reportModal').modal('hide');
                            var fileName = result.message;
                            var a = document.createElement("a");
                            a.href = "data:application/octet-stream;base64," + result.responseData;
                            a.download = fileName;
                            a.click();


                        }
                        else {
                            alert('error', result.message)
                        }
                    }, error: (error) => {
                        alert('error', error.message)
                        $('#reportModal').modal('hide');

                       
                    },
                    complete: () => {
                        Closeloader()
                        self.ClearControl()
                    }
                })
            }
        }

    }
    self.GetDrnData = () => {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.ShholderNo())) {
                if (!Validate.empty(self.DRNNO())) {
                    Openloader()

                    $('#tbl_Certificate_List').DataTable().clear();
                    $('#tbl_Certificate_List').DataTable().destroy();
                    $.ajax({
                        type: "post",
                        url: "/CDS/CertificateReversalEntry/GetDataFromDRN",
                        data: {
                           
                            "CompCode": self.SelectedCompany(),
                            "DRNNO": self.DRNNO(), "ShholderNo": self.ShholderNo()
                        },
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: (result) => {
                            if (result.isSuccess) {
                                var mappedTasks = $.map(result.responseData, function (item) {
                                    return new CertificateDetail(item)
                                });
                                self.CertificateList(mappedTasks);
                                $('#Save').attr('disabled', false)
                            } else {
                                alert('error', result.message)
                                $('#Save').attr('disabled', true)

                            }
                        }, error: (err) => {
                            alert('error', result.message)
                            self.DRNNO('')
                            $('#Save').attr('disabled', true)

                        },
                        complete: () => {
                            Closeloader()
                            $('#tbl_Certificate_List').DataTable({
                                responsive: true,
                                searching: true,
                                scrollX: true,
                                scrollY: true,
                                paging: true,
                                ordering: false,
                                columnDefs: [
                                    { width: 100, targets: [1, 2, 3, 4, 6, 7, 9] },
                                    { width: 200, targets: [5, 8] }
                                ],
                                fixedColumns: true
                            })
                        }
                    })
                } else {
                    alert('warning', 'DRN No Cannot Be Empty !!!');
                }
            } else {
                alert('warning', 'ShHolderNo Cannot Be Empty !!!');
            }
        }

    }
    self.GetSholderInformation = (data) => {
        if (self.ValidateCompany()) {
            if (!Validate.empty(self.ShholderNo())) {
                Openloader();
                $.ajax({
                    type: "post",
                    url: "/CDS/CertificateReversalEntry/GetShHolderInformation",
                    data: {
                       
                        "CompCode": self.SelectedCompany(),
                        "ShHolderNo": self.ShholderNo()
                    },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: (result) => {
                        if (result.isSuccess) {
                            self.HOLDERNAME(result.responseData.fName + ' ' + result.responseData.lName)
                            self.TotalKitta(result.responseData.totalKitta)
                            $('#DIV_DRN').show();
                            $('#DRNNo,#DRNDataButton').attr('disabled', false)
                            $('#DRNNo').focus();

                        } else {
                            alert('warning',result.message)
                        }
                    }, error: (error) => {
                        alert('error', error.message)
                        $('#DRNNo,#DRNDataButton').attr('disabled', true)
                        self.ClearControl()
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            } else {
                self.ClearControl()
            }
        }
    }
    self.NormalDataTable = () => {
        $('#tbl_Certificate_List').DataTable().clear();
        $('#tbl_Certificate_List').DataTable().destroy();
        $('#tbl_Certificate_List').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: false,
            columnDefs: [
                { width: 100, targets: [1, 2, 3, 4, 6, 7, 9] },
                { width: 200, targets: [5, 8] }
            ],
            fixedColumns: true
        })
    }
}

$(function () {

    ko.applyBindings(new ReversalEntry())
    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });

    $('#simple-date2 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });

    self.NormalDataTable();
    $('#DIV_DRN').hide()
})