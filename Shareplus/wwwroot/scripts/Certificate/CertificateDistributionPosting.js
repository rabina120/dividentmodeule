function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}




self.CertificateDistributionData = function (data) {
    var self = this;
    if (data != undefined) {
        self.ShHolderNo = ko.observable(data.shholderNo)
        self.FName = ko.observable(data.aTTShHolder.fName)
        self.Name = ko.observable(data.aTTShHolder.fName + ' ' + data.aTTShHolder.lName)
        self.LName = ko.observable(data.aTTShHolder.lName)
        self.Name = ko.observable(data.name)
        self.CertNo = ko.observable(data.certNo);
        self.ShKitta = ko.observable(data.shKitta);
        self.Address = ko.observable(data.aTTShHolder.address)
        self.SrNoFrom = ko.observable(data.srNoFrom)
        self.SrNoTo = ko.observable(data.srNoTo)
        self.CertDistDt = ko.observable(convertDate(data.certDistDt))
        self.UserName = ko.observable(data.userName)
        self.Remarks = ko.observable(data.distribution_remarks)
        self.Selected = ko.observable();
        self.PostingRemarks = ko.observable();
        self.PostingDate = ko.observable();
        self.PostCertificateDistributionEntry = ko.observable();
        self.ClearControl = ko.observable();
       


    }
}

var CertificateDistributionPosting = function () {

    //for company
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();
    self.PostingRemarks = ko.observable();
    self.CertificateDistributionList = ko.observableArray([]);
    self.ActionType = ko.observable();
    self.StartIssuedDate = ko.observable();
    self.EndIssuedDate = ko.observable();
    self.PostCertificateDistributionEntry = ko.observable();
    self.DistDate = ko.observable();
    self.ShKitta = ko.observable();
    self.Address = ko.observable();

    self.FName = ko.observable();
    self.LName = ko.observable();
    self.EntryBy = ko.observable();
    self.UserName = ko.observable();
 
    self.RefreshData = ko.observable();
    self.TotalKitta = ko.observable();
    self.ClearControl = ko.observable();
    self.startdate = ko.observable();
    self.enddate = ko.observable();



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

    

    self.RefreshData = function () {
        if (self.ValidateCompany()) {
            $('#tbl_CertificateDistribute').DataTable().clear();
            $('#tbl_CertificateDistribute').DataTable().destroy();

            Openloader()
            $.ajax({
                type: "post",
                url: '/Certificate/CertificateDistributionPosting/GetCertificateDistributionCompanyData',
                data: { 'CompCode': self.SelectedCompany(), 'startdate': self.startdate(), 'enddate': self.enddate() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                datatype: "json",
                success: function (result) {
                    if (result.isSuccess) {
                        if (result.responseData.length > 0) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new CertificateDistributionData(item)
                            });
                            self.CertificateDistributionList(mappedTasks);



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
                    self.NormalDataTable()

                    Closeloader()
                }
            })
        }
    }

    self.SelectAll = ko.computed({
        read: () => !self.CertificateDistributionList().find(x => !x.Selected()),
        write: t => self.CertificateDistributionList().forEach(x => x.Selected(t)),
    })

    self.ValidationPosting = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company !!<br/>";
        } if (self.CertificateDistributionList().filter(x => x.Selected()).length <= 0) {
            errMsg += "Please Tick the Distributed Info !!</br>";
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

    self.PostCertificateDistributionEntry = function (data) {
        if (self.ValidationPosting()) {
            var record = self.CertificateDistributionList().filter(x => x.Selected()).map(x => ({
                certNo: x.CertNo(),
                shholderNo: x.ShHolderNo(),
                name: x.Name(),
                address: x.Address(),
                certDistDtL: x.CertDistDt(),
                kitta: x.ShKitta(),
                srNoFrom: x.SrNoFrom(),
                srNoTo: x.SrNoTo(),
                userName: x.UserName()


            }));
            self.ActionType(data);

            var RecordDetails = {
                compcode: self.SelectedCompany(),
                App_remarks: self.PostingRemarks(),
                App_date: self.PostingDate()
            }

            Openloader()
            $.ajax({
                type: 'POST',
                datatype: 'json',
                url: '/Certificate/CertificateDistributionPosting/PostCertificateDistributionEntry',
                data: {  'certificateDemate': record, 'RecordDetails': RecordDetails, 'ActionType': self.ActionType()},
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



    self.NormalDataTable = () => {
        $('#tbl_CertificateDistribute').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: false,
            columnDefs: [
                { width: 100, targets: [1, 2, 4, 5, 6, 7] },
                { width: 150, targets: [3, 8, 9] }
            ],
            fixedColumns: true
        })

    }

    self.ClearControl = function () {
        $('#tbl_CertificateDistribute tbody').empty()
        $('#tbl_CertificateDistribute').DataTable().clear();
        $('#tbl_CertificateDistribute').DataTable().destroy();

        self.PostingRemarks('')
        self.ActionType('')

    }


}



$(function () {
    ko.applyBindings(new CertificateDistributionPosting())
    self.NormalDataTable()
    $('#simple-date1' ).datepicker({

        format: 'yyyy-mm-dd',
        todayHighlight: true,
        autoclose: true,
        endDate: '+0d',
      
    });
    $('#simple-date1').datepicker('setDate', 'today');

    $('#simple-date2').datepicker({

        format: 'yyyy-mm-dd',
        todayHighlight: true,
        autoclose: true,
        endDate: '+0d',
        
    });
    $('#simple-date2').datepicker('setDate', 'today');

})
