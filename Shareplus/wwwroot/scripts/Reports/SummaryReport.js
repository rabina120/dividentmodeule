function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compCode + " " + data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

var SummaryReport = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();
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
    self.GenerateReport = function (data) {
        if (self.ValidateCompany()) {
            debugger
            Openloader()
            var companyCode = self.SelectedCompany()
            var CompEnName = self.CompanyDetails().find(x => x.CompCode() == companyCode).CompEnName;
            $.ajax({
                url: '/Reports/SummaryReport/GenerateReport',
                type: "post",
                data: { 'CompCode': self.SelectedCompany(), 'CompEnName': CompEnName, 'ReportType':data },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: (result) => {
                    
                        if (result.isSuccess) {
                            /* var arr = [result.message];*/
                            var fileName = result.message;
                            var a = document.createElement("a");
                            a.href = "data:application/octet-stream;base64," + result.responseData;
                            a.download = fileName;
                            a.click();
                        }
                        else {
                            console.log('error => '+ result.message)
                            alert('error', result.message);
                        }

                    }, error: function (error) {
                        console.log('error=>',error.message)
                        alert('error', error.message)
                    },
                    complete: () => {

                        Closeloader()
                    }
            })
        }
        
    }

}

$(document).ready(function () {
    ko.applyBindings(new SummaryReport())
});
