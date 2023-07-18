
function ShownerType(data) {
    var self = this;
    if (data != undefined) {
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.ShOwnerTypeName = ko.observable(data.shOwnerTypeName);
        self.ShOwnerTypeAndName = self.ShOwnerType() + " " + self.ShOwnerTypeName();
    }
}

function Occupation(data) {
    var self = this;
    if (data != undefined) {
        self.OccupationID = ko.observable(data.occupationId)
        self.OccupationName = ko.observable(data.occupationN)
        self.ShOwnerType = ko.observable(data.shownertype)
    }
}

var OccupationSetup = function () {
    self.OccupationID = ko.observable()
    self.OccupationName = ko.observable()
    self.ActionType = ko.observable()
    self.OccupationList = ko.observableArray([])

    //ShOwnertype ko lagi
    self.ShOwnerTypes = ko.observableArray([]);
    self.ShOwnerType = ko.observable();
    self.ShOwnerTypeName = ko.observable();
    self.SelectedShOwnerType = ko.observable();


    //load owner type on form load
    self.LoadShOwnerType = function () {
        $.ajax({
            type: "post",
            url: '/HolderManagement/ShareHolder/GetAllShOwnerType',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new ShownerType(item);
                    });
                    self.ShOwnerTypes(mappedTasks);

                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            }
        })
    }
    self.LoadShOwnerType();


    self.ChooseOptions = function (data) {
        self.ActionType(data)
        if (data == 'A') {
            $('#OccId').attr('disabled', 'disabled')
            $('#shOwnerType').attr('disabled', false)
        } else {
            $('#OccId, #shOwnerType').attr('disabled', 'disabled')

            Openloader()

            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/OccupationSetup/LoadOccupationList',
                dataType: 'json', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.hasError) {
                        alert('error', result.message)
                    } else {
                        if (result.isSuccess) {
                            if (result.responseData.length > 0) {
                                $('#ModalOccupationInformation').modal('show')
                                var mappedTasks = $.map(result.responseData, function (item) {
                                    return new Occupation(item);
                                });
                                self.OccupationList(mappedTasks);
                            }
                        } else {
                            alert('warning', result.message)
                        }
                    }

                },
                error: (error) => {
                    alert('error', error.message)
                },
                complete: () => {
                    Closeloader()
                }
            })
            $('#SaveButton').attr('value', 'Update')
        }
        $('#AddButton,#UpdateButton').attr('disabled', 'disabled')
    }

    self.Validation = function () {
        var errMsg = ""

        if (Validate.empty(self.ActionType())) {
            errMsg += 'Please Choose Action Type <br/>'
        }
        if (self.ActionType() != "A") {
            if (Validate.empty(self.OccupationID())) {
                errMsg += 'Please Enter Occupation ID <br/>'
            }
        }
        if (Validate.empty(self.OccupationName())) {
            errMsg += 'Please Enter Occupation Name <br/>'
        }
        if (Validate.empty(self.SelectedShOwnerType())) {
            errMsg += 'Please Choose Sh Owner Type <br/>'
        }
        if (errMsg == "") {
            return true;
        } else {
            alert('error', errMsg)
            return false
        }

    }
    self.GetOccupationDetails = function (data) {
        if (!Validate.empty(self.ActionType())) {
            $('#ModalOccupationInformation').modal('hide')
            Openloader()

            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/OccupationSetup/GetOccupationDetails',
                data: { 'OccupationId': data }, beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                dataType: 'JSON',
                success: (result) => {
                    if (result.hasError) {
                        alert('error', result.message)
                    }
                    else {
                        if (result.isSuccess) {
                            
                            self.OccupationID(result.responseData.occupationId)
                            self.OccupationName(result.responseData.occupationN)
                            self.SelectedShOwnerType(result.responseData.shownertype)

                        } else {
                            alert('warning', result.message)
                        }
                    }
                },
                error: (error) => {
                    alert('error', result.message)
                },
                complete: () => {
                    Closeloader()
                }
            })
        } else {
            alert('error', 'Please Choose Action Type')
        }
    }

    self.SaveOccupationDetails = function () {
        if (self.Validation()) {

            Openloader()

            var Occupation = {
                shownertype: self.SelectedShOwnerType(),
                OccupationId: self.OccupationID(),
                OccupationN: self.OccupationName()
            }
            $.ajax({
                type: 'POST',
                url: '/ParameterSetup/OccupationSetup/SaveOccupationDetails',
                data: { 'aTTOccupation': Occupation, 'ActionType': self.ActionType() },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    if (result.hasError) {
                        alert('error', result.message)
                    } else {
                        if (result.isSuccess) {
                            alert('success', result.message)
                        } else {
                            alert('warning', result.message)
                        }
                    }
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
        $('#AddButton,#UpdateButton, #OccId, #OccName').attr('disabled', false)
        $('#SaveButton').attr('value', 'Save')
        self.OccupationID('')
        self.OccupationName('')
        self.ActionType('')
        self.SelectedShOwnerType('')
    }
}




$(document).ready(function () {
    ko.applyBindings(new OccupationSetup());

});
