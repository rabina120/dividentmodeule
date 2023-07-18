self.ParaComp = function(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

self.Dividend = function(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.compcode);
        self.Divcode = ko.observable(data.divcode);
        self.Description = ko.observable(data.description);
        self.FiscalYr = ko.observable(data.fiscalYr);
        self.ShowDividendText = self.Divcode() + " " + self.Description() + " " + self.FiscalYr();
    }
}


self.PoolAccountSplitPostingViewModel = function () {
    var self = this;
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.DividendLists = ko.observableArray();
    self.SelectedDividend = ko.observable();
    self.SplitPostingList = ko.observableArray([]);
    var companyCode = localStorage.getItem('company-code');
    self.PostingDate = ko.observable();
    self.Remarks = ko.observable();
    self.Action = ko.observable();


    $.ajax({
        type: "post",
        url: '/Common/Company/GetCompanyDetailsByCompanyCode',
        datatype: "json", beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (result) {
            if (result.isSuccess) {
                var mappedTasks = $.map(result.responseData, function (item) {
                    return new ParaComp(item)
                });
                self.CompanyDetails(mappedTasks);
                if (companyCode != null || companyCode != '' || companyCode != undefined) {
                    self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode)?.CompCode());
                    //$("#CompanyList").attr("disabled", true);
                }
            } else {
                alert('warning', result.message)
            }
        },
        error: function (error) {
            alert('error', error.message)
        }
    })

    self.SelectedCompany.subscribe(function () {
        //console.log("selected")
        self.DividendLists([]);
        $.ajax({
            type: "post",
            url: '/DividendProcessing/DemateDividendIssueEntry/GetAllDemateDividends',
            data: { 'CompCode': self.SelectedCompany() },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            datatype: "json",
            async: true,
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new Dividend(item)
                    });
                    self.DividendLists(mappedTasks);

                } else {
                    alert('warning', "No Dividend List Found ")
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
    })

    self.SelectedDividend.subscribe(function () {
        self.SplitPostingList([]);
        if (Validate.empty(self.SelectedCompany())) {
            toastr.error("Company cannot be empty!!!");
        }
        else {
            self.SplitPostingList([]);
            postReq(
                '/DividendManagement/PoolAccountSplitPosting/GetSplitPostingList',
                { CompCode: self.SelectedCompany(), DivCode: self.SelectedDividend() },
                null,
                resp => {
                    self.SplitPostingList(resp.map(x => ({
                        ...x,
                        Selected: ko.observable(false)
                    })))
                },
                null
            )
        }
    })

    self.SplitDetailsPosting = ko.observableArray([]);

    self.ViewDetails = function (data) {
        console.log(data);
        postReq(
            '/DividendManagement/PoolAccountSplitPosting/GetSplitPostingDetailList',
            { CompCode: self.SelectedCompany(), DivCode: self.SelectedDividend(), ParentId: data.Split_id },
            null,
            self.SplitDetailsPosting,
            null
        )
        $("#PostingDetailsModal").modal("show");
    }

    self.AuthorizePosting = function () {
        self.Action("A");
        self.SubmitForPosting();
    }
    self.RejectPosting = function () {
        self.Action("R");
        self.SubmitForPosting();
    }

    self.Validate = function () {
        let err = "";
        var list = ko.toJS(self.SplitPostingList).filter(x => x.Selected == true);
        if (list.length == 0) {
            err += "Select atleast one boid!!<br>";
        }
        if (Validate.empty(self.PostingDate())) {
            err += "Posting date cannot be empty!!<br>";
        }
        if (Validate.empty(self.Remarks())) {
            err += "Date Cannot be empty!!<br>";
        }
        if (err != "") {
            toastr.error(err);
            return false;
        }
        else {
            return true;
        }

    }

    self.SubmitForPosting = function () {
        var data = {
            PostingList: ko.toJS(self.SplitPostingList).filter(x => x.Selected == true),
            PostingDate: self.PostingDate(),
            Remarks: self.Remarks(),
            Action: self.Action()
        }
        if (self.Validate()) {
            postReqMsg(
                '/DividendManagement/PoolAccountSplitPosting/SubmitForPosting',
                data,
                null,
                resp => {
                    console.log(resp, "------------");
                },
                null
            )
        }
    }
}

$(document).ready(function () {
    ko.applyBindings(new self.PoolAccountSplitPostingViewModel());
})
