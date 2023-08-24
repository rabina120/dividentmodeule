function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compCode + " " + data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}
function ShownerType(data) {
    var self = this;
    if (data != undefined) {
        self.ShOwnerType = ko.observable(data.shOwnerType );
        self.ShOwnerTypeName = ko.observable(data.shOwnerTypeName);
        self.ShOwnerTypeAndName = self.ShOwnerType() + " " + self.ShOwnerTypeName();
    }
}
function ShownerSubType(data) {
    var self = this;
    if (data != undefined) {
        self.ShOwnerType = ko.observable(data.shownertype ?? data.ShOwnerType());
        self.ShOwnerSubType = ko.observable(data.shownerSubtype ?? data.ShOwnerSubType());
        self.ShOwnerSubTypeName = ko.observable(data.name1 ?? data.ShOwnerSubTypeName());
        self.ShOwnerSubTypeAndName = self.ShOwnerSubType() + " " + self.ShOwnerSubTypeName();
    }
}
function District(data) {
    var self = this;
    if (data != undefined) {
        self.DistCode = ko.observable(data.distCode);
        self.NpDistName = ko.observable(data.npDistName);
        self.EnDistName = ko.observable(data.enDistName);
        self.DistCodeName = self.DistCode() + " " + self.EnDistName();
    }
}
function OccupationL(data) {
    var self = this;
    self.shownertype = ko.observable(data.shownertype);
    self.OccupationId = ko.observable(data.occupationId);
    self.OccupationN = ko.observable(data.occupationN);
    self.OccupationName = self.OccupationId() + ' '+self.OccupationN()
}

var HolderReports = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    self.ReportList = ko.observableArray([])
    self.SelectedReport = ko.observable()

    self.Details = ko.observableArray([])
    self.SelectedDetails = ko.observable()

    self.Districts = ko.observableArray([])
    self.SelectedDistrict = ko.observable()
    self.Occupations = ko.observableArray([])
    self.SelectedOccupation = ko.observable()
    self.HolderTypes = ko.observableArray([])
    self.SelectedHolderType = ko.observable()
    self.ShOwnerTypes = ko.observableArray([])
    self.SelectedShOwnerType = ko.observable()
    self.ShOwnerSubTypes = ko.observableArray([])
    self.FilteredShOwnerSubTypes = ko.observableArray([])
    self.SelectedShOwnerSubType = ko.observable()
    self.OrderByList = ko.observableArray([])
    self.SelectedOrderBy = ko.observable()

    self.ShareHolderNoFrom = ko.observable()
    self.ShareHolderNoTo = ko.observable()
    self.ShareHolderName = ko.observable()
    self.ShareHolderAddress = ko.observable()
    self.ShareKittaFrom = ko.observable()
    self.ShareKittaTo = ko.observable()
    self.ShareHolderNoFrom = ko.observable()
    self.ShareHolderNoFrom = ko.observable()
    self.ShareHolderNoFrom = ko.observable()

    self.ReportList.push({ "ReportID": "ATL", "ReportName":"Address Table List"})
    self.ReportList.push({ "ReportID": "SHL", "ReportName":"Share Holder List"})
    self.ReportList.push({ "ReportID": "SHLN", "ReportName":"Share Holder List Nepali"})
    self.ReportList.push({ "ReportID": "HAL", "ReportName":"Holder Attendance List"})
    self.ReportList.push({ "ReportID": "SHDL", "ReportName":"Share Holders Details List"})
    self.ReportList.push({ "ReportID": "SHLZK", "ReportName":"Share Holder List Zero Kitta"})
    self.ReportList.push({ "ReportID": "FL", "ReportName": "Fraction List" })
    self.ReportList.push({ "ReportID": "AFL", "ReportName": "All Fraction List" })

    self.Details.push({ "DetailsName": "All","DetailsID":"ALL"})
    self.Details.push({ "DetailsName": "Telephone Only","DetailsID":"TO"})
    self.Details.push({ "DetailsName": "PO Box Only","DetailsID":"PBO"})
    self.Details.push({ "DetailsName": "PO Box And Telephone No","DetailsID":"PBATN"})
    self.Details.push({ "DetailsName": "No PO Box And Telephone No", "DetailsID": "NPBATN" })


    self.HolderTypes.push({ "HolderTypeName": "All", "HolderTypeId": "A" })
    self.HolderTypes.push({ "HolderTypeName": "Minor", "HolderTypeId": "M" })
    self.HolderTypes.push({ "HolderTypeName": "No Minor", "HolderTypeId": "N" })

    self.OrderByList.push({ "OrderByName":"Name","OrderByID":"N"})
    self.OrderByList.push({ "OrderByName": "Holder No", "OrderByID": "H" })

    self.SelectedOrderBy('N')

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

    self.LoadDistrict = function () {
       
            $.ajax({
                type: "post",
                url: '/Reports/ShareHolderReport/GetAllDistrict',
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new District(item);
                        });
                        self.Districts(mappedTasks);
                    } else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: () => {
                   
                }
            })
       
    }
    self.LoadDistrict();
    
    self.LoadOccupation = function () {
        $.ajax({
            type: "post",
            url: '/Reports/ShareHolderReport/GetAllOccupation',

            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new OccupationL(item);
                    });
                    self.Occupations(mappedTasks);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            },
            complete: () => {
               
            }
        })
    }
    self.LoadOccupation();

    self.LoadShOwnerType = function () {
        $.ajax({
            type: "post",
            url: '/Reports/ShareHolderReport/GetAllShOwnerType',

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
            },
            complete: () => {
              
            }
        })
    }
    self.LoadShOwnerType();
    
    self.LoadShOwnerType = function () {
        $.ajax({
            type: "post",
            url: '/Reports/ShareHolderReport/GetAllShOwnerSubType',

            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    var mappedTasks = $.map(result.responseData, function (item) {
                        return new ShownerSubType(item);
                    });
                    self.ShOwnerSubTypes(mappedTasks);
                } else {
                    alert('warning', result.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            },
            complete: () => {
              
            }
        })
    }
    self.LoadShOwnerType();


    self.SelectedShOwnerType.subscribe(function () {
        self.FilteredShOwnerSubTypes([])
        var mappedTasks = $.map(self.ShOwnerSubTypes(), function (item) {
            if (item.ShOwnerType() == self.SelectedShOwnerType()) 
                return new ShownerSubType(item);
            
        });
        self.FilteredShOwnerSubTypes(mappedTasks)
    })

    self.ReportVaidation = function () {
        var errMsg = "";

        if (Validate.empty(self.SelectedReport())) {
            errMsg += "Please Select A Report .<br/>";
        }
        if (Validate.empty(self.SelectedDetails())) {
            errMsg += "Please Select A Detail .<br/>";
        }
        if (Validate.empty(self.SelectedOrderBy())) {
            errMsg += "Please Select Order By .<br/>";
        }
        
        if (errMsg == "") {
            return true;
        }
        else {
            alert('warning', errMsg);
            return false;
        }
    }
    self.GenerateReport = function (data) {
        if (self.ValidateCompany()) {
            if (self.ReportVaidation()) {
                Openloader()
                var companyCode = self.SelectedCompany()
                var company = {
                    "CompCode": self.SelectedCompany(),
                    "CompEnName": self.CompanyDetails().find(x => x.CompCode = self.SelectedCompany()).CompEnName()
                }
                var ReportData = {
                    
                    "ReportType": self.SelectedReport(),
                    "ReportDetails": self.SelectedDetails(),
                    "ShHolderNoFrom": self.ShareHolderNoFrom(),
                    "ShHolderNoTo": self.ShareHolderNoTo(),
                    "ShHolderName": self.ShareHolderName(),
                    "ShKittaFrom": self.ShareKittaFrom(),
                    "ShKittaTo": self.ShareKittaTo(),
                    "Address": self.ShareHolderAddress(),
                    "District": self.SelectedDistrict(),
                    "Occupation": self.SelectedOccupation(),
                    "HolderType": self.SelectedHolderType(),
                    "ShOwnerType": self.SelectedShOwnerType(),
                    "ShOwnerSubType": self.SelectedShOwnerSubType(),
                    "OrderBy": self.SelectedOrderBy()
                }
                $.ajax({
                    type: "post",
                    url: '/Reports/ShareHolderReport/GenerateReport',
                    data: { 'company': company, 'ReportData': ReportData, 'ExcelReportType': data },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (result) {
                        if (result.isSuccess) {
                            if (data == 'E') {
                                var fileName = result.message;
                                var a = document.createElement("a");
                                a.href = "data:application/octet-stream;base64," + result.responseData;
                                a.download = fileName;
                                a.click();
                            }
                            else {
                                var embed = "<embed width='100%' height='100%'  type='application/pdf' src='data:application/pdf;base64," + result.responseData + "'/>"
                                var x = window.open();
                                if (x) {
                                    x.document.open();
                                    x.document.write(embed);
                                    x.document.title = result.message;
                                    x.document.close();
                                } else {
                                    alert('warning', 'Failed to View Report.');
                                    alert('success', 'Downloading pdf repot');
                                    var fileName = result.message;
                                    var a = document.createElement("a");
                                    a.href = "data:application/octet-stream;base64," + result.responseData;
                                    a.download = fileName;
                                    a.click();
                                }
                            }
                        }
                        else {
                            alert('error', result.message);
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                        self.ClearControl()
                    },
                    complete: () => {
                        Closeloader()
                        
                    }
                })
            }
        }
    }
    self.SelectedDetails('ALL')
    self.SelectedOrderBy('H')
    self.SelectedReport('SHL')
    self.ClearControl = () => {

        self.SelectedReport('')
        self.SelectedDetails('')
        self.SelectedDistrict('')
        self.SelectedOccupation('')
        self.SelectedHolderType('')
        self.SelectedShOwnerType('')
        self.SelectedShOwnerSubType('')
        self.ShareHolderNoFrom('')
        self.ShareHolderNoTo('')
        self.ShareHolderName('')
        self.ShareHolderAddress('')
        self.ShareKittaFrom('')
        self.ShareKittaTo('')
        self.ShareHolderNoFrom('')
        self.ShareHolderNoFrom('')
        self.ShareHolderNoFrom('')
        self.SelectedOrderBy('N')
        $('#DistrictSelect2').val('').trigger('change')
    }

   // self.ClearControl()
}


$(document).ready(function () {

    ko.applyBindings(new HolderReports())
});