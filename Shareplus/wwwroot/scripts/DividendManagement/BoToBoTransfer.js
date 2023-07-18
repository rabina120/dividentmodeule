function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function BoToBoTransferViewModel() {
    var self = this;
    self.TransferTypes = [
        { Type: "BO", TypeName: "BO To BO Transfer" },
        { Type: "DT", TypeName: "Death Transfer" },
        { Type: "BE", TypeName: "BOID Edit" }
    ]
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.TransferType = ko.observable();
    self.BoidKeyword = ko.observable();
    self.BOIDList = ko.observableArray();
    self.DiveidendTransferHistory = ko.observableArray();
    self.NewBOID = ko.observable();
    self.Remarks = ko.observable();
    self.CompCode = ko.observable(localStorage.getItem('company-code'));
    self.HolderInfoObj = ko.observableArray([]);
    self.Action = ko.observable();
    self.NewName = ko.observable();
    self.NewFName = ko.observable();
    self.NewGFName = ko.observable();
    self.NewAddress = ko.observable();


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

    self.AddBOIDTransfer = () => {
        self.Action('A');
        $('#updateBtn').attr('disabled', true);
        $('#deleteBtn').attr('disabled', true);
    }
    self.UpdateBOIDTransfer = () => {
        self.Action('U');
        $('#addBtn').attr('disabled', true);
        $('#deleteBtn').attr('disabled', true);
    }
    self.DeleteBOIDTransfer = () => {
        self.Action('D');
        $('#updateBtn').attr('disabled', true);
        $('#addBtn').attr('disabled', true);
    }

    self.SearchBoid = () => {
        let errMsg = "";
        if (self.SelectedCompany() == undefined) errMsg += "Please select a company!!<br>"
        if (self.BoidKeyword() == undefined) {
            errMsg += "BOID cannot be empty!!<br>";
            toastr.error(errMsg);
        }
        else {
            postReq(
                '/DividendManagement/Botobotransfer/GetHolderInformation',
                { CompCode: self.SelectedCompany(), BOID: self.BoidKeyword(), Action: self.Action() },
                null,
                resp => {
                    self.HolderInfoObj(resp);
                        self.NewName("");
                        self.NewFName("");
                        self.NewGFName("");
                    self.NewAddress("");
                    if (resp.length >= 1) {
                        //&& (self.TransferType() == "BE" || (self.TransferType() == "DT" && self.Action == 'U'))) {
                        let info = resp[0];
                        self.NewName(info.fullname);
                        self.NewFName(info.faname);
                        self.NewGFName(info.grfaname);
                        self.NewAddress(info.address);
                    }
                },
                null
            )
        }
    }

    self.ChangeNewBoID = () => {

    }

    self.Validate = () => {
        console.log(self.Action())

            var data = ko.toJS(self.HolderInfoObj());
            var errMsg = ""
            if (self.Action() == 'A') {
                if (Validate.empty(self.TransferType())) {
                    errMsg += "Please select a transfer type <br/>"
                }
            }
            if (Validate.empty(self.SelectedCompany())) {
                errMsg += "Please select a company <br/>"
            }
            if (Validate.empty(self.BoidKeyword())) {
                errMsg += "BOID Cannot Be Empty <br/>"
            }
            if (Validate.empty(self.NewBOID()) && self.TransferType() != "BE") {
                errMsg += "Enter New BOID <br/>"
            }
            if (data != undefined && data.isloked) {
                errMsg += "This old BOID is Locked!!! <br/>"
            }
            //if (data == undefined) {
            //    errMsg += "BOID Cannot Be Empty <br/>"
            //}
            if (errMsg !== "") {
                alert('error', errMsg);
                return false;
            }
            else {
                return true;
            }
        }


    self.SaveBOIDChange = () => {
        if (self.Validate()) {
            var HolderInfo = {
                CompCode: self.SelectedCompany(),
                TranType: self.TransferType(),
                PreviousBOID: self.BoidKeyword(),
                NewBoid: self.NewBOID(),
                Action: self.Action()
            }
            if (self.TransferType() == 'DT' || self.TransferType() == 'BE') {
                HolderInfo = {
                    ...HolderInfo,
                    NewName: self.NewName(),
                    NewFName: self.NewFName(),
                    NewGFName: self.NewGFName(),
                    NewAddress: self.NewAddress()
                }
            }
            else {
                HolderInfo = {
                    ...HolderInfo,
                    NewName: self.HolderInfoObj()[0].fullname,
                    NewFName: self.HolderInfoObj()[0].fname,
                        NewGFName: self.HolderInfoObj()[0].grfaname,
                            NewAddress: self.HolderInfoObj()[0].address
                }
            }
            postReqMessage(
                '/DividendManagement/Botobotransfer/SaveHolderForBoidChange',
                { HolderInfo },
                null,
                resp => {
                    if (resp.isSuccess) self.ClearAll();
                },
                null
            )
        }
    }

    self.ClearAll = () => {
        self.SearchBoid('');
        self.NewBOID('');
        self.NewFName('');
        self.NewGFName('');
        self.HolderInfoObj([]);
        self.Action('');
    }
}

$(document).ready(() => {
    ko.applyBindings(new BoToBoTransferViewModel());
})