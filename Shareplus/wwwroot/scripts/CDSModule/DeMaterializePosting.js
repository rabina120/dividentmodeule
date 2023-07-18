function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}


function ParaCompChild(data) {
    var self = this;
    if (data != undefined) {
        self.ISIN_NO = ko.observable(data.isiN_NO)
        self.ShholderNo = ko.observable(data.shholderNo)
        self.Desc_share = ko.observable(data.desc_share)
        self.ShownerType = ko.observable(data.shownerType)
    }
}

self.DeMaterializeData = function (data) {
    var self = this;
    if (data != undefined) {
        self.DemateRegno = ko.observable(data.demate_regno)
        self.TransDate = ko.observable(convertDate(data.tr_date))
        self.Remarks = ko.observable(data.remarks)
        self.EntryDate = ko.observable(convertDate(data.entrydate))
        self.EntryUser = ko.observable(data.entryUser)
        self.RegNo = ko.observable(data.regno)
        if (data.aTTShHolder != null) {
            self.FName = ko.observable(data.aTTShHolder.fName)
            self.LName = ko.observable(data.aTTShHolder.lName)
        }
        self.ISINNO = ko.observable(data.isin_no)
        self.BoAccountNo = ko.observable(data.bo_acct_no)
        self.SeqNo = ko.observable(data.seq_no)
        self.DrnNo = ko.observable(data.drn_no)
        self.CertNo = ko.observable(data.certno)
        self.SrNoFrom = ko.observable(data.srnofrom)
        self.SrNoTo = ko.observable(data.srnoto)
        self.ShKitta = ko.observable(data.shkitta)
        self.ShholderNo = ko.observable(data.shholderno)
        self.Dp_Code = ko.observable(data.dp_code)


        self.Selected = ko.observable()
    }
}

var DeMaterializePosting = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();


    self.RegNoFrom = ko.observable()
    self.RegNoTo = ko.observable()

    self.ISIN_NO = ko.observable()
    self.ShholderNo = ko.observable()
    self.Desc_share = ko.observable()
    self.ShownerType = ko.observable()
    self.ISINNOList = ko.observableArray([])
    self.SelectedISINNO = ko.observable();
    self.CheckCA = ko.observable();


    self.isCA = ko.observable('DR');

    self.PostingRemarks = ko.observable()
    //self.PostingDate = ko.observable()
    self.DeMaterializeList = ko.observableArray([])
    self.CertDetList = ko.observableArray([])
    self.ActionType = ko.observable()

    //posting
    self.dateFrom = ko.observable();
    self.dateTo = ko.observable();

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


    self.LoadISINNOList = function () {
        if (!Validate.empty(self.SelectedCompany())) {
            self.ISINNOList.removeAll();
            $.ajax({
                type: "post",
                url: '/CDS/DeMaterializeEntry/GetAllParaCompChild',
                data: { 'CompCode': self.SelectedCompany() },
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
                        self.ISINNOList(mappedTasks);



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

    self.SelectedCompany.subscribe(() => {
        
        self.LoadISINNOList();
    })

    
    self.CheckCA.subscribe(function (data) {
        if (data == true) {
            self.isCA('CA')
        } else if (data == false || data == undefined) {
            self.isCA('DR')
            self.DeMaterializeList([])

        }
        $('#tbl_DeMaterialize').DataTable().clear()
        $('#tbl_DeMaterialize').DataTable().destroy();
    });

    self.Validation = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company <br/>";
        } if (Validate.empty(self.SelectedISINNO())) {
            errMsg += "Please Choose ISINNO <br/>";
        }
        if (errMsg == "") {
            return true;
        } else {
            alert('error', errMsg)
            return false
        }
    }
    self.RefreshData = function () {
        if (self.Validation()) {
            $('#tbl_DeMaterialize').DataTable().clear()
            $('#tbl_DeMaterialize').DataTable().destroy();
            Openloader()
            $.ajax({
                type: "post",
                url: '/CDS/DeMaterializePosting/GetDeMaterializeData',
                data: { 'CompCode': self.SelectedCompany(), 'FromDate': self.dateFrom(), 'ToDate': self.dateTo(), 'RegNoFrom': self.RegNoFrom(), 'RegNoTo': self.RegNoTo(), 'ISINNO': self.SelectedISINNO().ISIN_NO(), 'CheckCA': self.isCA() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        if (result.responseData.length > 0) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new DeMaterializeData(item)
                            });
                            self.DeMaterializeList(mappedTasks);

                            
                        } else {
                            alert('warning', 'No Record Found !!')
                        }

                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: () => {
                    $('#tbl_DeMaterialize').DataTable({
                        responsive: true,
                        searching: true,
                        scrollX: true,
                        scrollY: true,
                        paging: true,
                        ordering: false,
                        columnDefs: [
                            { width: 100, targets: [1, 2, 3, 4, 5, 6] }
                        ],
                        fixedColumns: true
                    })

                    Closeloader()
                }
            })
        }
    }

    self.SelectAll = ko.computed({
        read: () => !self.DeMaterializeList().find(x => !x.Selected()),
        write: t => self.DeMaterializeList().forEach(x => x.Selected(t)),
    })

    self.ValidationPosting = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company !!<br/>";
        } if (self.DeMaterializeList().filter(x => x.Selected()).length <= 0) {
            errMsg += "Please Tick the Dematerialize Info !!</br>";
        }
        if (Validate.empty(self.PostingRemarks())) {
            errMsg += "Please Enter Posting Remarks !!</br>";
        }
        if (Validate.empty(self.PostingDate())) {
            errMsg += "Please Enter Posting Date !!</br>";
        }
        if (errMsg == "") {
            return true;
        } else {
            alert('error', errMsg)
            return false
        }
    }

    //after selecting a record for posting
    self.GetSingleDematerializeData = (data) => {
        Openloader()
        var companyCode = self.SelectedCompany()
        $.ajax({
            type: 'POST',
            datatype: 'json',
            url: '/CDS/DeMaterializePosting/GetSingleDematerializeData',
            data: {
               
                'CompCode': companyCode,
                'DemateRegno': data.DemateRegno(),
                'RegNo': data.RegNo(),
                'ISINNo': data.ISINNO(),
                'Remarks': data.Remarks(),
                'DRNNo': data.DrnNo()
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (result) => {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new DeMaterializeData(item)
                    });
                    self.CertDetList(mappedTasks);
                    $('#certificateDetailModal').modal('show')
                } else {
                    alert('error',result.message)
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

    self.PostDeMaterializeEntry = function (data) {
        if (self.ValidationPosting()) {
            var record = self.DeMaterializeList().filter(x => x.Selected()).map(x => ({
                demate_regno: x.DemateRegno(),
                regno: x.RegNo(),
                isin_no: x.ISINNO(),
                certNO: x.CertNo(),

            }));
            self.ActionType(data);

            var RecordDetails = {
                compcode: self.SelectedCompany(),
                App_remarks: self.PostingRemarks(),
                App_date: self.PostingDate(),
            }

            Openloader()
            $.ajax({
                type: 'POST',
                datatype: 'json',
                url: '/CDS/DeMaterializePosting/PostDeMaterializeEntry',
                data: { 'certificateDemate': record, 'RecordDetails': RecordDetails, 'ActionType': self.ActionType() },
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
            

        }
    }

    self.ClearControl = function () {
        self.NormalDataTable()
        self.RegNoFrom('')
        self.RegNoTo('')
        self.ISIN_NO('')
        self.SelectedISINNO('')
        self.isCA('DR')
        self.CheckCA(false)
        self.ActionType('')
        self.PostingRemarks('')
    }

    self.NormalDataTable = () => {
        $('#tbl_DeMaterialize').DataTable().clear();
        $('#tbl_DeMaterialize').DataTable().destroy();
        $('#tbl_DeMaterialize').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: false,
            columnDefs: [
                { width: 100, targets: [1, 2, 3, 4, 5, 6] }
            ],
            fixedColumns: true
        })
    }
   
}
$(function () {
    ko.applyBindings(new DeMaterializePosting())
    self.NormalDataTable()

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

   
})