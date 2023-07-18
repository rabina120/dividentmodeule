function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function CertifiacteDetail(data) {
    var self = this;
    if (data != undefined) {
        self.rev_tran_no = ko.observable(data.rev_tran_no)
        self.shholderno = ko.observable(data.shholderno)
        self.bo_acct_no = ko.observable(data.bo_acct_no)
        self.regno = ko.observable(data.regno)
        self.drn_no = ko.observable(data.drn_no)
        self.rev_by = ko.observable(data.rev_by)
        self.rev_date = ko.observable(data.rev_date == null ? "": data.rev_date.substring(0,10))
        self.tr_date = ko.observable(data.tr_date == null ? "" : data.tr_date.substring(0, 10))
        self.rev_remarks = ko.observable(data.rev_remarks)
        self.name = ko.observable(data.name)
        self.shkitta  = ko.observable(data.shkitta )
        self.pcs_demate_holderno  = ko.observable(data.pcs_demate_holderno)
        self.srnofrom  = ko.observable(data.srnofrom)
        self.srnoto  = ko.observable(data.srnoto)
        self.certno  = ko.observable(data.srnoto)
        self.Selected = ko.observable()
    }
}

var CertificateReversalViewModal = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()




    self.PostingRemarks = ko.observable()
    self.PostingDate = ko.observable()


    self.CertificateReversalList = ko.observableArray([])
    self.CertificateSingleReversalList = ko.observableArray([])

    //gloabal variables
    var record= []

    //posting
    self.dateFrom = ko.observable();
    self.dateTo = ko.observable();


    self.PostingDate(formatDate(new Date))

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

  
   

    self.SelectAll = ko.computed({
        read: () => !self.CertificateReversalList().find(x => !x.Selected()),
        write: t => self.CertificateReversalList().forEach(x => x.Selected(t)),
    })


    self.ClearData = function () {

        self.ShholderNo('')
        self.TotalKitta('')
        self.FullName('')
        self.ShOwnerType('')
        self.FullNameNepali('')
        self.ShOwnerSubType('')
        self.FaName('')
        self.GrFaName('')
        self.Address('')
        self.DistCode('')
        self.TelNo('')
        self.PoBoxNo('')
        self.LockId('')
        self.Remarks('')


        self.ShHolderList([])
    }

   
    self.refresh = function (data) {
        self.ButtonOnLoad();
        self.ClearData()
       
    }
    self.report = function (data) {

    }

   

    self.PostReversal = function (data) {
        if (self.Validation()) {
            Openloader()
            for (var i = 0; i < $('#tbl_Post_Data').DataTable().data().count(); i++) {
                var x = i + 1;
                var Check = $($('#tbl_Post_Data').DataTable().row(i).nodes()).find('input').prop('checked');
                if (Check != undefined && Check != "" && Check != false) {


                    var DIS = {
                        rev_tran_no: $('#tbl_Post_Data').DataTable().row(i).data()[1],
                        drn_no: $('#tbl_Post_Data').DataTable().row(i).data()[2],
                        regno: $('#tbl_Post_Data').DataTable().row(i).data()[3],
                        shholderno: $('#tbl_Post_Data').DataTable().row(i).data()[4]
                    }

                    record.push(DIS)
                }

            }
            $.ajax({
                type: "POST",
                url: '/CDS/CertificateReversalPosting/PostData',
                data: {
                  
                    'ReversalCertificates': record,
                    'CompCode': self.SelectedCompany(),
                    'SelectedAction': data,
                    'Remarks': self.PostingRemarks(),
                    'PostingDate': self.PostingDate()

                },
                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        alert('success', result.message)
                    }
                    else {
                        alert('error', result.message)
                    }
                    self.PostingRemarks('')
                },
                error: function (eror) {
                    alert('error', error.message)
                    self.PostingRemarks('')
                },
                complete: () => {
                    loadDataTable()
                    self.PostingRemarks('')
                    record = []
                    Closeloader()
                   
                }
            })
        }
    }


    loadDataTable = function () {
        if (self.ValidateCompany()) {
            Openloader()
            var companyCode = self.SelectedCompany()

            $.ajax({
                type: "post",
                url: '/CDS/CertificateReversalPosting/GetDataForPosting',
                data: {
                    'CompCode': companyCode, 'FromDate': self.dateFrom(), 'ToDate': self.dateTo()},
                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    $('#tbl_Post_Data').DataTable().clear();
                    $('#tbl_Post_Data').DataTable().destroy();
                    if (result.isSuccess) {

                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new CertifiacteDetail(item)
                        });
                        self.CertificateReversalList(mappedTasks);
                    } else {
                        alert('warning', "No Data Found ")
                    }
                    self.PostingRemarks('')

                },
                error: function (error) {
                    $('#tbl_Post_Data').DataTable().clear();
                    $('#tbl_Post_Data').DataTable().destroy();
                    alert('error', error.message)
                    self.PostingRemarks('')
                },
                complete: () => {
                            
                    self.NormalDataTable()
                    self.PostingRemarks('')
                    Closeloader()
                }
            })
        }
    }
/*    loadDataTable()*/
    self.ViewSingleRematerializeDetail = function (data) {
        if (self.ValidateCompany()) {
            if (data != null) {
                Openloader()
                var companyCode = self.SelectedCompany()

                $.ajax({
                    type: "post",
                    url: '/CDS/CertificateReversalPosting/ViewSingleRematerializeDetail',
                    data: { 'CompCode': companyCode, 'RevTranNo': data.rev_tran_no(),'RegNo':data.regno(),'DrnNo':data.drn_no() },
                    datatype: "json",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            $('#certificateDetailModal').modal('show');
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new CertifiacteDetail(item)
                            });
                            self.CertificateSingleReversalList([])
                            self.CertificateSingleReversalList(mappedTasks);
                        } else {
                            alert('warning',result.message)
                            $('#certificateDetailModal').modal('hide');
                            self.CertificateSingleReversalList([])
                        }

                    },
                    error: function (error) {
                        alert('error', error.message)
                        self.CertificateSingleReversalList([])
                    },
                    complete: () => {
                        Closeloader()
                    }
                })
            }
        }
    }

    self.Validation = function () {
        var errMsg = "";
        if (self.SelectedCompany() == undefined) {
            errMsg += "Please Select Company !!!</br>";
        }

        if ($('#tbl_Post_Data').find('input[type=checkbox]:checked').length == 0) {
            errMsg += "Please Select A Record !!!</br>";
        }
        if (Validate.empty(self.PostingRemarks())) {
            errMsg += "Please Enter Posting Remarks !!!</br>";
        }
        if (Validate.empty(self.PostingDate())) {
            errMsg += "Please Enter Posting Date !!!</br>";
        }
        
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }

    }
    self.NormalDataTable = () => {
        $('#tbl_Post_Data').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: false,
            columnDefs: [
                { width: 100, targets: [2,3,4,5,6,7,8,9,10] },
              
            ],
            fixedColumns: true
        })
    }
}

$(document).ready(function () {

    ko.applyBindings(new CertificateReversalViewModal());

    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });

    $('#simple-date2 .input-group.date').datepicker({
        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true,
    });

    self.NormalDataTable()

});