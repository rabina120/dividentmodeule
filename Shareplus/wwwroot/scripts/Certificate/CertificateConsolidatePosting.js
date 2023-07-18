function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        
    }
}

function CertificateConsolidate(data) {
    var self = this;
    if (data != undefined) {
        self.SplitNo = ko.observable(data.split_no)
        self.ShholderNo = ko.observable(data.shholderNo)
        self.FName = ko.observable(data.fName)
        self.LName = ko.observable(data.lName)
        self.CertNo = ko.observable(data.certNo)
        self.kitta = ko.observable(data.kitta)
        self.SrNoFrom = ko.observable(data.srNoFrom)
        self.SrNoTo = ko.observable(data.srNoTo)
        self.SDate = ko.observable(convertDate(data.split_dt))
        self.Selected = ko.observable()
    }

} 
var CertificateConsolidatePosting = function () {
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    //self.PostingDate = ko.observable();
    self.PostingRemarks = ko.observable()
    self.CertificateConsolidateList = ko.observableArray([])
    self.SelectedAction = ko.observable();
    self.PaymentDetails = ko.observableArray([])

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


   

    self.RefreshData = function () {
        if (self.ValidateCompany()) {
            $('#tbl_CertificateConsolidate').DataTable().clear();
            $('#tbl_CertificateConsolidate').DataTable().destroy();

            Openloader()
            $.ajax({
                type: "post",
                url: '/Certificate/CertificateConsolidatePosting/GetCertificateConsolidateCompanyData',
                data: { 'CompCode': self.SelectedCompany() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: (data) => {

                    $('#chk').prop('checked', false);
                    if (data.isSuccess) {
                        if (data.responseData.length > 0) {
                            var mappedTasks = $.map(data.responseData, function (item) {
                                return new CertificateConsolidate(item)
                            });
                            self.CertificateConsolidateList(mappedTasks);
                            console.log(ko.toJS(self.CertificateConsolidateList()))

                            $('#tbl_CertificateConsolidate').DataTable({
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

    self.SelectAll = ko.computed({
        read: () => !self.CertificateConsolidateList().find(x => !x.Selected()),
        write: t => self.CertificateConsolidateList().forEach(x => x.Selected(t)),
    })

    self.ViewDetail = function (data) {
        //$('#viewdetail').modal('show');
        Openloader()
        $.ajax({
            type: "post",
            url: '/Certificate/CertificateConsolidatePosting/GetCertificate',
            data: { 'CompCode': self.SelectedCompany(), 'SplitNo': ko.toJS(data.SplitNo), 'ShholderNo': ko.toJS(data.ShholderNo) },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            datatype: "json",
            success: (data) => {
                if (data.isSuccess) {
                    if (data.responseData.length > 0) {
                        var mappedTasks = $.map(data.responseData, function (item) {
                            return new CertificateConsolidate(item)
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
    self.PostCertificateConsolidateEntry = function (data) {
        if (self.Validation()) {
            var record = self.CertificateConsolidateList().filter(x => x.Selected()).map(x => ({
                split_no: x.SplitNo(),
                shholderNo: x.ShholderNo(),              
            }));
            self.SelectedAction(data);
            console.log(record);
            var RecordDetails = {
                compcode: self.SelectedCompany(),
                App_remarks: self.PostingRemarks(),
                App_date: self.PostingDate()
            }
            Openloader()
            $.ajax({
                type: 'POST',
                datatype: 'json',
                url: '/Certificate/CertificateConsolidatePosting/PostCertificateConsolidateEntry',
                data: { 'aTTCertificateConsolidatePostings': record, 'recorddetails': RecordDetails, 'SelectedAction': self.SelectedAction() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
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
    self.ClearControl = function () {
        
            $('#tbl_CertificateConsolidate tbody').empty()
            $('#tbl_CertificateConsolidate').DataTable().clear();
            $('#tbl_CertificateConsolidate').DataTable().destroy();

            self.PostingRemarks('')
            self.SelectedAction('')

        
    }
    self.Validation= function () {
        return true;
    }
}
$(function () {
    ko.applyBindings(new CertificateConsolidatePosting())
    
})