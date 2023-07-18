function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}
function BOPostingViewModel() {
    var self = this;
    self.CompanyDetails = ko.observableArray();
    self.SelectedCompany = ko.observable();
    self.TransferTypes = [
        { Type: "BO", TypeName: "BO To BO Transfer" },
        { Type: "DT", TypeName: "Death Transfer" },
        { Type: "BE", TypeName: "BOID Edit" }
    ]
    self.TransferType = ko.observable();
    var companyCode = localStorage.getItem('company-code');
    if (companyCode != null || companyCode != '') {
        $.ajax({
            type: "post",
            url: '/Common/Company/GetCompanyDetails',
            data: { 'CompCode': companyCode },
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

                    if (!Validate.empty(companyCode)) { self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode()); }
                    //$("#CompanyList").attr("disabled", true);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
    }

    self.HolderChangePostingList = ko.observableArray([]);
    self.PostingRemarks = ko.observable();
    self.PostingDate = ko.observable();
    self.BoidTransferDetails = ko.observableArray([]);

    self.ChangeTransferType = () => {
        var data = {
            CompCode: self.SelectedCompany(),
            TranType: self.TransferType(),
        }
        if (ko.toJS(self.TransferType()) != undefined) {
            postReq(
                '/DividendManagement/boidtransferposting/GetHolderChangelistForPosting',
                data,
                null,
                resp => {
                    self.HolderChangePostingList(resp.map(x => ({
                        ...x,
                        Selected: ko.observable(false)
                    })))
                },
                null
            )
        } else {
            self.HolderChangePostingList([]);
        }
    }

    self.SelectAll = ko.computed({
        read: () => !self.HolderChangePostingList().find(x => !x.Selected()),
        write: t => self.HolderChangePostingList().forEach(x => x.Selected(t)),
    })

    self.VerifyHolder = () => {
        self.VerifyRejectHolder('A');
    }

    self.RejectHolder = () => {
        self.VerifyRejectHolder('R');
    }
    
    self.GetDetails = (data) => {
        console.log(data);
        var detailData = {
            CompCode: self.SelectedCompany(),
            TranType: self.TransferType(),
            ParentId: data.Id
        }
        postReq(
            '/DividendManagement/BoidTransferPosting/GetHolderChangelistForPosting',
            detailData,
            null,
            self.BoidTransferDetails,
            null
        )
        $("#BoidDetails").modal('show');
    }

    

    var record = [];

    self.EnableVerifyButtons = () => {
        var data = ko.toJS(self.HolderChangePostingList());
        var selectedList = data.filter(x => x.Selected == true);
        var errMsg = ""
        if (Validate.empty(self.PostingRemarks())) {
            errMsg += "Please Enter posting remarks!!!<br/>"
        }
        if (selectedList.length < 1) {
            errMsg += "Please select a entry <br/>"
        }
        if (Validate.empty(self.PostingDate())) {
            errMsg += "Posting date cannot be empty <br/>"
        }
        
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }
    }

    self.VerifyRejectHolder = (status) => {
        if (self.EnableVerifyButtons()) {
            //console.log(status, "status")
            //console.log(self.HolderChangePostingList())
            Openloader()
            var SelectedList = ko.toJS(self.HolderChangePostingList()).filter(x => x.Selected == true);
            console.log(SelectedList);
            //for (var i = 0; i < $('#tbl_Add_Holder_List').DataTable().data().count(); i++) {
            //    var x = i + 1;
            //    var Check = $($('#tbl_Add_Holder_List').DataTable().row(i).nodes()).find('input').prop('checked');
            //    if (Check != undefined && Check != "" && Check != false) {


            //        var DIS = {
            //            ShholderNo: $('#tbl_Add_Holder_List').DataTable().row(i).data()[2],
            //            Remarks: self.PostingRemarks(),
            //            ApprovedDate: self.PostingDate(),
            //            ActionType: status
            //        }

            //        record.push(DIS)
            //    }
            //}
            var data = {
                SelectedList,
                ActionType: status,
                Remarks: self.PostingRemarks(),
                ApprovedDate: self.PostingDate(),
                CompCode: self.SelectedCompany(),
                TranType: self.TransferType()
            }

            postReq(
                '/DividendManagement/boidtransferposting/VerifyRejectHolderList',
                data,
                null,
                resp => {

                },
                null
            )
            Closeloader()
        }
    }

}

$(document).ready(function () {
    ko.applyBindings(new BOPostingViewModel());
})
