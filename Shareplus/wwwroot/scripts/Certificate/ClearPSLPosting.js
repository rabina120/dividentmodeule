function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)

    }
}
function ClearPSLPostingItem(data) {
    var self = this;
    if (data != undefined) {
        self.PSLNo = ko.observable(data.pslNo)
        self.ShholderNo = ko.observable(data.shholderNo)
        self.FName = ko.observable(data.fName)
        self.LName = ko.observable(data.lName)
        self.Name = ko.observable(data.name)
        self.Status = ko.observable(data.status)
        self.EntryUser = ko.observable(data.entryUser)
        self.EntryDate = ko.observable(data.entryDate)
        self.Totalkitta = ko.observable(data.totalKitta);
        self.TransDate = ko.observable(data.tranDt)
        self.ClearDate = ko.observable(data.clearDate)
        self.Kitta = ko.observable(data.kitta)
        self.Remark = ko.observable(data.remark)
        self.Selected = ko.observable()
        self.PSL_CLEAR_NO = ko.observable(data.psL_CLEAR_NO)
        self.CertNo = ko.observable(data.certNo)
        self.SrNoFrom = ko.observable(data.srNoFrom)
        self.SrNoTo = ko.observable(data.srNoTo)
        self.SeqNo = ko.observable(data.seqNo)
        self.ClearRemark = ko.observable(data.clearRemark)
        self.PledgeKitta = ko.observable(data.pledgeKitta)
    }

}

var ClearPSLPosting = function () {
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.PSLNo = ko.observable();
    self.Name = ko.observable();
    self.ShholderNo = ko.observable();
    self.FName = ko.observable();
    self.LName = ko.observable();
    self.Status = ko.observable();
    self.EntryUser = ko.observable();
    self.EntryDate = ko.observable();
    self.TransDate = ko.observable();
    self.ClearDate = ko.observable();
    self.Kitta = ko.observable();
    self.Totalkitta = ko.observable();
    self.SeqNo = ko.observable();
    self.SrNoFrom = ko.observable();
    self.SrNoTo = ko.observable();
    self.PledgeKitta = ko.observable();
    self.ClearRemark = ko.observable();
    self.Remark = ko.observable();
    self.Remarks = ko.observable();
    self.PostingRemarks = ko.observable();
    self.CertNo = ko.observable();
    self.CertDetList = ko.observableArray([])
    self.PSL_CLEAR_NO = ko.observable();
    self.PreparedBy = ko.observable();
    self.TranType = ko.observable();
    //self.PostingDate = ko.observable();
    self.ClearPSLPostingList = ko.observable([]);
    self.GetSingleClearPSLData = ko.observable();
    
    self.Selected = ko.observable();
    self.SelectedAction = ko.observable();

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
        read: () => !self.ClearPSLPostingList().find(x => !x.Selected()),
        write: t => self.ClearPSLPostingList().forEach(x => x.Selected(t)),
    })

    self.RefreshData = function () {
        if (self.ValidateCompany()) {
            $('#tbl_ClearPSLPosting').DataTable().clear();
            $('#tbl_ClearPSLPosting').DataTable().destroy();

            Openloader()
            $.ajax({
                type: "post",
                url: '/Certificate/ClearPSLPosting/GetClearPSLPostingCompanyData',
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
                                return new ClearPSLPostingItem(item)
                            });
                            self.ClearPSLPostingList(mappedTasks);
                            console.log(ko.toJS(self.ClearPSLPostingList()))

                            $('#tbl_ClearPSLPosting').DataTable({
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

    
    //after selecting a record for posting
    self.GetSingleClearPSLData = (data) => {
        Openloader()
        var companyCode = self.SelectedCompany()
        $.ajax({
            type: 'POST',
            datatype: 'json', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            url: '/Certificate/ClearPSLPosting/GetSingleClearPSLData',
            data: {
                'CompCode': companyCode,
                'PSL_CLEAR_NO': data.PSL_CLEAR_NO,
                'ShholderNo': data.ShholderNo()
            },
            success: (result) => {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new ClearPSLPostingItem(item)
                    });
                    self.CertDetList(mappedTasks);
                    $('#certificateDetailModal').modal('show')
                } else {
                    alert('error', result.message)
                    $('#certificateDetailModal').modal('hide')
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
    }

    self.PostPSLClearPosting = function (data) {
        if (self.PostValidation()) {
            var recorddata = self.ClearPSLPostingList().filter(x => x.Selected()).map(x => ({
                PSL_CLEAR_NO: x.PSL_CLEAR_NO(),
                ShholderNo: x.ShholderNo(),
                FName: x.FName(),
                LName: x.LName(),
                Status: x.Status(),
                EntryUser: x.EntryUser(),
                EntryDate: x.EntryDate(),
                Kitta: x.Kitta(),
                ClearDate: x.ClearDate(),
                Remark: x.Remark(),
                CertNo: x.CertNo()
            }));
            self.SelectedAction(data);

            var RecordDetails = {
                compcode: self.SelectedCompany(),
                clear_approved_remarks: self.PostingRemarks(),
                AppDate_Clear: self.PostingDate()
            }
            Openloader()
            $.ajax({
                type: 'POST',
                datatype: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/Certificate/ClearPSLPosting/PostPSLClearPosting',
                data: { 'aTTpSLClearPostings': recorddata, 'recorddetails': RecordDetails, 'SelectedAction': self.SelectedAction() },
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
        if (self.ValidateCompany()) {

            Openloader()

            $.ajax({
                type: 'GET', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/Certificate/ClearPSLPosting/ViewReport',
                data: {
                    'CompCode': self.SelectedCompany(), 'PSL_CLEAR_NO': self.PSL_CLEAR_NO(), 'ShholderNo': self.ShholderNo(), 'CertNo': self.CertNo(), 'SrNoFrom': self.SrNoFrom(), 'SrNoTo': self.SrNoTo(), 'ClearDate': self.ClearDate(), 'TranType': self.TranType(),
                    'PreparedBy': self.PreparedBy() , 'Kitta': self.Kitta(), 'FName': self.FName(), 'LName': self.LName(), 'Name': self.Name(), "ReportType": data
                },
                dataType: 'json',
                success: (result) => {
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

        $('#tbl_ClearPSLPosting tbody').empty()
        $('#tbl_ClearPSLPosting').DataTable().clear();
        $('#tbl_ClearPSLPosting').DataTable().destroy();

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
        if ($('#tbl_ClearPSLPosting').find('input[type=checkbox]:checked').length <= 0) {
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
    ko.applyBindings(new ClearPSLPosting())
})