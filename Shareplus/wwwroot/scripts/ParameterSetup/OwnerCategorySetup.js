
function ShownerType(data) {
    var self = this;
    if (data != undefined) {
        self.ShOwnerType = ko.observable(data.shOwnerType);
        self.ShOwnerTypeName = ko.observable(data.shOwnerTypeName);
        self.ShOwnerTypeAndName = self.ShOwnerType() + " " + self.ShOwnerTypeName();
    }
}
function OwnerCategory(data) {
    var self = this;

}



self.OwnerCategorySetup = function () {

    var self = this;
    self.ShownerSubType = ko.observable();
    self.OwnerList = ko.observableArray([]);
    self.ShOwnerTypes = ko.observableArray([]);
    self.ShOwnerType = ko.observable();
    self.ShOwnerTypeName = ko.observable();
    self.SelectedShOwnerType = ko.observable();
    self.ShownerSubTypeId = ko.observable();




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
                    self.SelectedShOwnerType('02');
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


    self.EditedPR = function (data) {
        console.log(data);
        self.SelectedShOwnerType(data.ShownerTypeId);
        self.ShownerSubType(data.ShownerSubType);

        self.OwnerList.remove(data);

    }
    self.DeletePR = function (data) {
        self.OwnerList.remove(data);
    }

    self.Save = function () {
        var errMsg = "";
        if (self.OwnerList().length == 0) errMsg += "OwnerType Cannot be empty!!<br>";
        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        }
        else {
            postReqMsg(
                '/ParameterSetup/ownercategorysetup/SaveOwnerCategory',
                {
                    'ShownerType': self.OwnerList()
                },
                null,
                resp => {
                    if (resp.IsSuccess) {

                        self.OwnerList([]);
                    }
                }
            )
        }
    }

    self.GetShownerCategory = function () {
        postReqMsg(
            '/ParameterSetup/ownercategorysetup/GetOwnerCategory',
            null,
            null,
            resp => {
                if (resp.IsSuccess) {
                    resp.ResponseData.forEach((item) => {
                        self.OwnerList.push({
                            ShownerSubTypeId: item.ShownerSubTypeId,
                            ShownerType: self.ShOwnerTypes().find(x => x.ShOwnerType() == item.ShownerTypeId).ShOwnerTypeName(),
                            ShownerSubType: item.ShownerSubType,
                            ShownerTypeId: item.ShownerTypeId
                        })
                    })
                }
            }
        )
    }

    self.AddShownerType = function (data) {
        //console.log(self.Boid());
        var errMsg = "";
        if (Validate.empty(self.SelectedShOwnerType())) errMsg += "Ownertype Cannot be empty!!<br>";
        if (Validate.empty(self.ShownerSubType())) errMsg += "OwnerCategory Cannot be empty!!<br>";
        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        }
        else {
            var x=self.ShownerSubTypeId()
            if (Validate.empty(self.ShownerSubTypeId())) {

             x = self.OwnerList().count
            if (x == undefined) {
                x=0
                }
                x++;
            }
            
            
            self.OwnerList.push({
                ShownerSubTypeId:x,
                ShownerType: self.ShOwnerTypes().find(x => x.ShOwnerType() == self.SelectedShOwnerType()).ShOwnerTypeName(),
                ShownerSubType: self.ShownerSubType(),
                ShownerTypeId: self.SelectedShOwnerType()
            })
            self.SelectedShOwnerType(null);
            self.ShownerSubType(null);

        }
    }
    
}





$(document).ready(function () {
    ko.applyBindings(new self.OwnerCategorySetup());
})