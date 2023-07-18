function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}


self.CertificateSplitData = function (data) {
    var self = this;
    if (data != undefined) {
        self.SplitNo = ko.observable(data.split_no)
        self.ShHolderNo = ko.observable(data.shholderNo)
        self.FName = ko.observable(data.fName)
        self.LName = ko.observable(data.lName)
        self.CertNo = ko.observable(data.certNo);
        self.ShKitta = ko.observable(data.kitta);
        self.SrNoFrom = ko.observable(data.srNoFrom)
        self.SrNoTo = ko.observable(data.srNoTo)
        self.SDate = ko.observable(convertDate(data.split_dt))
        self.Remarks = ko.observable(data.split_remarks)
        self.Selected = ko.observable()



    }
}

var CertificateSplitPosting = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();
    self.PostingRemarks = ko.observable()
    self.CertificateSplitList = ko.observableArray([])
    self.ActionType = ko.observable()

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
            $('#tbl_CertificateSplit').DataTable().clear();
            $('#tbl_CertificateSplit').DataTable().destroy();

            Openloader()
            $.ajax({
                type: "post",
                url: '/Certificate/CertificateSplitPosting/GetCertificateSplitCompanyData',
                data: { 'CompCode': self.SelectedCompany() },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        if (result.responseData.length > 0) {
                            var mappedTasks = $.map(result.responseData, function (item) {
                                return new CertificateSplitData(item)
                            });
                            self.CertificateSplitList(mappedTasks);



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
        read: () => !self.CertificateSplitList().find(x => !x.Selected()),
        write: t => self.CertificateSplitList().forEach(x => x.Selected(t)),
    })


    self.ValidationPosting = function () {
        var errMsg = ""
        if (Validate.empty(self.SelectedCompany())) {
            errMsg += "Please Choose Company !!<br/>";
        } if (self.CertificateSplitList().filter(x => x.Selected()).length <= 0) {
            errMsg += "Please Tick the Rematerialize Info !!</br>";
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


    self.PostCertificateSplitEntry = function (data) {
        if (self.ValidationPosting()) {
            var record = self.CertificateSplitList().filter(x => x.Selected()).map(x => ({
                split_no: x.SplitNo(),
                shholderNo: x.ShHolderNo(),
                certNo: x.CertNo(),
                kitta: x.ShKitta(),
                srNoFrom: x.SrNoFrom(),
                srNoTo: x.SrNoTo(),
                split_dt: x.SDate()


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
                datatype: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/Certificate/CertificateSplitPosting/PostCertificateSplitEntry',
                data: { 'certificateDemate': record, 'RecordDetails': RecordDetails, 'ActionType': self.ActionType() },
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
        $('#tbl_CertificateSplit tbody').empty()
        $('#tbl_CertificateSplit').DataTable().clear();
        $('#tbl_CertificateSplit').DataTable().destroy();

        self.PostingRemarks('')
        self.ActionType('')

    }
    self.NormalDataTable = () => {
        $('#tbl_CertificateSplit').DataTable({
            responsive: true,
            searching: true,
            scrollX: true,
            scrollY: true,
            paging: true,
            ordering: false,
            columnDefs: [
                { width: 100, targets: [1, 2,  4, 5, 6, 7] },
                {width: 150, targets:[3,8,9]}
            ],
            fixedColumns: true
        })

    }
}

$(function () {
    ko.applyBindings(new CertificateSplitPosting())
    self.NormalDataTable()
})