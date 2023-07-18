

function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}


self.CDSCompanyParameterViewModel = function () {
    var self = this;
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.OwnerTypeList = ko.observableArray([]);
    self.IsinNo = ko.observable();
    self.ShholderNo = ko.observable();
    self.ParamList = ko.observableArray([]);
    self.OwnerTypeList.push({ 'ShOwnerTypeName': 'Promoter', 'ShOwnerType': '1' })
    self.OwnerTypeList.push({ 'ShOwnerTypeName': 'Public', 'ShOwnerType': '2' })
    self.SelectedOwnerType = ko.observable();
    self.Description = ko.observable();

    
   



    var companyCode = localStorage.getItem('company-code');

    $.ajax({
        type: "post",
        url: '/Common/Company/GetCompanyDetails',

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

                if (companyCode != undefined) {
                    self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode());
                    //self.LoadDividendList();
                }
                //$("#CompanyList").attr("disabled", true);
            } else {
                alert('warning', result.message)
            }
        },
        error: function (error) {
            alert('error', error.message)
        }
    })

    self.SelectedCompany.subscribe(function () {
        self.ParamList.removeAll();
        
        if (!Validate.empty(self.SelectedCompany())) {
         
            $.ajax({
                type: "post",
                url: '/ParameterSetup/CDSCompanyParameter/GetCDSCompanyParameter',
                data: {'compcode':self.SelectedCompany()},
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        result.responseData.forEach((item) => {
                            self.ParamList.push({
                                'IsinNo': item.isinNo,
                                'ShholderNo': item.shholderNo,
                                'Description': item.description,
                                'ownertype': item.ownerType,
                                'Type': self.OwnerTypeList().find(x => x.ShOwnerType == item.ownerType).ShOwnerTypeName

                            })
                        })
                        
                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })

        }
    })

   

  
    self.AddPLRL = function (data) {
        //console.log(self.Boid());
        var errMsg = "";
        if (Validate.empty(self.IsinNo())) errMsg += "IsinNo Cannot be empty!!<br>";
        if (Validate.empty(self.ShholderNo())) errMsg += "ShholderNo Cannot be empty!!<br>";
        if (self.ParamList().find(x => x.ownertype == self.SelectedOwnerType())!=undefined) errMsg += "Multiple owner type detected";
        if (errMsg != "") {
            toastr.error(errMsg);
            return false;
        }
        else {
            self.ParamList.push({
                'IsinNo': self.IsinNo(),
                'ShholderNo': self.ShholderNo(),
                'Description': self.Description(),
                'ownertype': self.SelectedOwnerType(),
                'Type':self.OwnerTypeList().find(x=>x.ShOwnerType==self.SelectedOwnerType()).ShOwnerTypeName

            })
            self.IsinNo(null);
            self.ShholderNo(null);
            self.Description(null);
            self.SelectedOwnerType(null);
        }
    }


    self.EditCDSCompanyParameter = function (data) {
        console.log(data);
        self.IsinNo(data.IsinNo);
        self.ShholderNo(data.ShholderNo);
        self.Description(data.Description);
        self.SelectedOwnerType(data.ownertype);
        self.ParamList.remove(data);

    }
    self.DeleteCDSCompanyParameter = function (data) {
        self.ParamList.remove(data);
/*        alert('success','Record deleted succesfully.')*/
    }

    self.UploadCDSCompanyParameterFinal = function () {
        postReqMsg(
            '/ParameterSetup/CDSCompanyParameter/SaveCDSCompanyParameterList',
            {
                'CompCode': self.SelectedCompany(),


                'ParamList': self.ParamList()
            },
            null,
            resp => {
                if (resp.IsSuccess) {
                    self.SelectedCompany(null);
                    self.ParamList([]);
                }
            }
        )
    }

}

$(document).ready(function () {
    ko.applyBindings(new self.CDSCompanyParameterViewModel());
})