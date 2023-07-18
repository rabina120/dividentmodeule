function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)

    }
}

function PSLPosting(data) {
    var self = this;
    if (data != undefined) {
        self.PSLNo = ko.observable(data.pslNo)
        self.ShholderNo = ko.observable(data.shholderNo)
        self.CertNo = ko.observable(data.certNo)
        self.FName = ko.observable(data.fName)
        self.LName = ko.observable(data.lName)
        self.Status = ko.observable(data.trantype)
        self.EntryUser = ko.observable(data.entryUser)
        self.EntryDate = ko.observable(data.entryDate)
        self.Totalkitta = ko.observable(data.totalKitta);
        self.TransDate = ko.observable(data.tranDt)
        self.Remark = ko.observable(data.remark)
        self.Selected = ko.observable()

        self.SrNoFrom = ko.observable(data.srNoFrom);
        self.SrNoTo = ko.observable(data.srNoTo);
        self.Seqno = ko.observable(data.seqno);
        self.PledgeKitta = ko.observable(data.pledgeKitta);
    }

}

var PSLEntryPosting = function () {
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.PSLNo = ko.observable();
    self.ShholderNo = ko.observable();
    self.CertNo = ko.observable();
    self.FName = ko.observable();
    self.LName = ko.observable();
    self.Status = ko.observable();
    self.EntryUser = ko.observable();
    self.EntryDate = ko.observable();
    self.TransDate = ko.observable();
    self.Totalkitta = ko.observable();
    self.Remark = ko.observable();
    self.Remarks = ko.observable();
    self.PostingRemarks = ko.observable();
    //self.PostingDate = ko.observable();
    self.PSLEntryPostingList = ko.observable([]);
    self.PaymentDetails = ko.observableArray([])
    self.Selected = ko.observable();
    self.SelectedAction = ko.observable();
    self.SrNoFrom = ko.observable();
    self.SrNoTo = ko.observable();
    self.Seqno = ko.observable();
    self.PledgeKitta = ko.observable();
    self.Remark = ko.observable();

    //Report
    self.CertNo = ko.observable();
    self.TranDt = ko.observable();
    self.PreparedBy = ko.observable();
    self.AuthorizedBy = ko.observable();
    self.Kitta = ko.observable();
    self.Name = ko.observable();
    self.TranType = ko.observable();
    self.ReportType = ko.observable();


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
    self.SelectAll = ko.computed({
        read: () => !self.PSLEntryPostingList().find(x => !x.Selected()),
        write: t => self.PSLEntryPostingList().forEach(x => x.Selected(t)),
    })

    self.RefreshData = function () {
        if (self.ValidateCompany()) {
            $('#tbl_PSLEntryPosting').DataTable().clear();
            $('#tbl_PSLEntryPosting').DataTable().destroy();

            Openloader()
            $.ajax({
                type: "post",
                url: '/Certificate/PSLEntryPosting/GetPSLEntryCompanyData',
                data: { 'CompCode': self.SelectedCompany() },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (data) => {

                    $('#chk').prop('checked', false);
                    if (data.isSuccess) {
                        if (data.responseData.length > 0) {
                            var mappedTasks = $.map(data.responseData, function (item) {
                                return new PSLPosting(item)
                            });
                            self.PSLEntryPostingList(mappedTasks);
                            console.log(ko.toJS(self.PSLEntryPostingList()))

                            $('#tbl_PSLEntryPosting').DataTable({
                                responsive: false,
                                searching: false,
                                scrollX: true,
                                scrollY: true,
                                scrollCollapse: true,
                                paging: true,
                                ordering: false,
                                fixedHeader: true,
                                "scrollY": "650px",
                                "sScrollX": "100%",
                                "scrollCollapse": true,

                            });
                        }
                        else {
                            alert('error', 'No Record Found')

                        }
                    } else {
                        alert('error', data.message)

                    }
                },
                error: (error) => {

                    alert('error', error.message)

                },
                complete: () => {
                    Closeloader()
                }

            })
        }

    };

    self.ViewDetail = function (data) {
        //$('#viewdetail').modal('show');
        Openloader()
        $.ajax({
            type: "post",
            url: '/Certificate/PSLEntryPosting/GetCertificate',
          /*  url: '/Certificate/PSLEntryPosting/PostPSLEntryPosting',*/
            data: { 'CompCode': self.SelectedCompany(), 'PSLNO': ko.toJS(data.PSLNo), 'ShholderNo': ko.toJS(data.ShholderNo) },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (data) => {
                if (data.isSuccess) {
                    if (data.responseData.length > 0) {
                        var mappedTasks = $.map(data.responseData, function (item) {
                            return new PSLPosting(item)
                        });
                        self.PaymentDetails(mappedTasks);
                        console.log(ko.toJS(self.PaymentDetails()))

                    }
                    else {
                        alert('error', 'No Record Found')

                    }
                } else {
                    alert('error', data.message)

                }
            },
            error: (error) => {
                alert('error', error.message)
            },
            complete: () => {
                Closeloader()
            }

        })

    }
    self.PostPSLEntryPosting = function (data) {
        if (self.PostValidation()) {

            var recorddata = self.PSLEntryPostingList().filter(x => x.Selected()).map(x => ({                
                PSLNo: x.PSLNo(),
                ShholderNo: x.ShholderNo(),
                CertNo: x.CertNo(),
                FName: x.FName(),
                LName: x.LName(),
                Status: x.Status(),
                EntryUser: x.EntryUser(),
                EntryDate: x.EntryDate(),
                Totalkitta: x.Totalkitta(),
                TransDate: x.TransDate(),
                Remark: x.Remark(),     



            }));

            self.SelectedAction(data);

            var RecordDetails = {
                compcode: self.SelectedCompany(),
                psl_approved_remarks: self.PostingRemarks(),
                AppDate_PSL: self.PostingDate()
            }
            Openloader()
            $.ajax({
                type: 'POST', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: 'json',
                url: '/Certificate/PSLEntryPosting/PostPSLEntryPosting',
                data: { 'aTTpSLEntryPostings': recorddata, 'recorddetails': RecordDetails, 'SelectedAction': self.SelectedAction() },
                success: (result) => {
                    if (result.isSuccess) {
                        alert('success', result.message)

                    } else {
                        alert('warning', result.message)

                    }
                },
                failure: (result) => {
                    alert('error', result.message)

                },
                error: (error) => {
                    alert('error', error.message)

                },
                complete: () => {
                    Closeloader()
                }
            })
            self.ClearControl()

        }
    }
    
    self.ViewReport = function (data) {
        if (self.ValidateCompany())
        {
            
                Openloader()

                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/Certificate/PSLEntryPosting/ViewReport',
                    data: {
                        'CompCode': self.SelectedCompany(),'PSLNo': self.PSLNo(), 'ShholderNo': self.ShholderNo(), 'CertNo': self.CertNo(), 'SrNoFrom': self.SrNoFrom(), 'SrNoTo': self.SrNoTo(), 'TranDt': self.TranDt(), 'TranType': self.TranType(),
                        'PreparedBy': self.PreparedBy(), 'AuthorizedBy': self.AuthorizedBy(), 'Kitta': self.Kitta(), 'FName': self.FName(), 'LName': self.LName(), 'Name': self.Name(), "ReportType": data
                    },
                    dataType: 'json',
                    success:  (result) => {
                        if (result.isSuccess) {
                            /* var arr = [result.message];*/
                            var fileName = result.message;
                            var a = document.createElement("a");
                            a.href = "data:application/octet-stream;base64," + result.responseData;
                            a.download = fileName;
                            a.click();
                        }
                        else {
                            /*console.log('error=>', result.message)*/
                            alert('error', result.message);
                        }

                    }, error: (error) => {
                        /*console.log('error=>', error.message)*/
                        alert('error', error.message)
                    },
                    complete: () => {

                        Closeloader()
                    }
                })

            
        }
    }
    self.ClearControl = function () {

        $('#tbl_PSLEntryPosting tbody').empty()
        $('#tbl_PSLEntryPosting').DataTable().clear();
        $('#tbl_PSLEntryPosting').DataTable().destroy();

        self.PostingRemarks('')
        self.SelectedAction('')


    }
    self.Validation = function () {
        return true;
    }
    self.PostValidation = function () {
        var errMsg = ""

        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company <br>"
        }
        if (Validate.empty(self.PostingRemarks())) {
            errMsg += "Please Enter Posting Remarks <br>"
        }
        if (Validate.empty(self.PostingDate())) {
            errMsg += "Please Select a Posting Date <br>"
        }
        if ($('#tbl_PSLEntryPosting').find('input[type=checkbox]:checked').length <= 0) {
            errMsg += "Please Tick A Certificate </br>";
        }

        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
        }
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
    ko.applyBindings(new PSLEntryPosting())
})