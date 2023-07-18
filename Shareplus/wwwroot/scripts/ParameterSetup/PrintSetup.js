function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode);
        self.CompEnName = ko.observable(data.compCode + " " + data.compEnName);
        self.MaxKitta = ko.observable(data.maxKitta);
    }
}

//function Fields(data) {
//    var self = this;
//    if (data != undefined) {
//        self.ColumnName = ko.observable(data.ColumnName);
//        self.X = ko.observable(data.X);
//        self.Y = ko.observable(data.Y);
//    }
//}

self.AddPrintCoordinatesViewModel = function () {
    var self = this;
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.ColumnName = ko.observable();
    self.XCoordinate = ko.observable(0);
    self.YCoordinate = ko.observable(0);
    self.Action = ko.observable('A');
    self.Side = ko.observable('L');
    

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
                    if (!Validate.empty(localStorage.getItem('company-code'))) { self.SelectedCompany(self.CompanyDetails().find(x => x.CompCode() == companyCode).CompCode()); }
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

    self.FieldList = ko.observableArray([]);
    self.SelectedFieldList = ko.observableArray([]);
    self.FieldListWithCoordinates = ko.observableArray([])

    self.GetAllFields = () => {
        postReq(
            '/ParameterSetup/PrintSetup/GetAllPrintFields',
            null,
            null,
            self.FieldList,
            null
        );
        
    }
    self.GetAllFields();
    self.SelectedColumnName = ko.observable();

    self.CompantChange = () => {
        postReq(
            '/ParameterSetup/PrintSetup/GetAllPrintFieldsWithCoordinates',
            { CompCode: self.SelectedCompany() },
            null,
            resp => {
                if (resp == null || resp.length <= 0) {
                    self.SelectedFieldList([]);
                    self.SelectedFieldList.push({ ColumnName: 'Page', ColumnValue: 'Page', XCoordinate: 14.5, YCoordinate: 8,Side:'L' })
                }
                else {
                    //console.log(resp)
                    resp.map(x => {
                        if (x.ColumnName != 'Page') {
                            let value = ko.toJS(self.FieldList)?.find(y => y.Item2 == x.ColumnValue)?.Item1
                            x.ColumnName = value == undefined ? "Page" : value;
                        }
                    })
                    self.SelectedFieldList(resp);
                }

            },
            null
        );
    }

    self.SubmitCoordinates = () => {
        //console.log(self.SelectedFieldList());
        let data = {
            cordList: self.SelectedFieldList(),
            CompCode: self.SelectedCompany()
        }
        let errMsg = "";
        if (self.SelectedFieldList().length <= 0)
            errMsg += "Add atleast one field!! <br>";
        if (self.SelectedCompany() == undefined) {
            errMsg += "Company Cannot be empty";
            toastr.error(errMsg);
        }
        //console.log(data, "================")
        else {
            postReq(
                '/ParameterSetup/PrintSetup/SaveCompanyCoordinates',
                data,
                null,
                resp => {
                    self.SelectedFieldList([]);
                    self.SelectedCompany(null);
                    //console.log(resp, "--------------------")
                },
                null
            );
        }
    }

    self.AddFieldCoordinate = (data) => {
        if (self.SelectedColumnName() == undefined) toastr.error("Field Name Cannot be Empty!!");
        else {
            var colName = ko.toJS(self.FieldList)?.find(x => x.Item2 == self.SelectedColumnName())?.Item1;
            var filedObj = {
                ColumnName: colName == undefined ? "Page" : colName,
                ColumnValue: self.SelectedColumnName(),
                XCoordinate: self.XCoordinate(),
                YCoordinate: self.YCoordinate(),
                Side:self.Side()
            }
            self.SelectedFieldList.push((ko.toJS(filedObj)));
            self.SelectedColumnName(null);
            self.XCoordinate(0);
            self.YCoordinate(0);
            self.Action("A");
        }

        //console.log(self.SelectedFieldList());
    }

    self.EditFieldCoordinate = (data) => {
        //console.log(data)
        self.Action("E");
        self.ColumnName(data.ColumnName);
        self.SelectedColumnName(ko.toJS(self.FieldList)?.find(x => x.Item1 == data.ColumnName)?.Item2);
        self.XCoordinate(data.XCoordinate);
        self.YCoordinate(data.YCoordinate);
        self.SelectedFieldList.remove(data);
    }
    self.RemoveFieldCoordinate = (data) => {
        self.SelectedFieldList.remove(data);
    }

    


}

$(document).ready(function () {
    ko.applyBindings(new self.AddPrintCoordinatesViewModel());
})