function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}



var SignatureReport = function () {

    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()

    self.DateFrom = ko.observable();
    self.DateTo = ko.observable();
    self.HolderFrom = ko.observable();
    self.HolderTo = ko.observable();
    self.SignatureApprovedUnapproved = ko.observable();
    self.SignatureApprovedUnapproved('A')

    function formatDate(date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2)
            month = '0' + month;
        if (day.length < 2)
            day = '0' + day;

        return [year, month, day].join('-');
    }
    self.DateFrom(formatDate(new Date))
    self.DateTo(formatDate(new Date))





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

   
    //validation
    self.Validation = function (data) {
        var errMsg = "";
        
        if (errMsg == "") {
            return true;
        }
        else {
            alert('warning', errMsg);
            return false;
        }
    }

    self.SignatureApprovedUnapproved.subscribe(() => {
        if (self.SignatureApprovedUnapproved() == "C") {
            $('#DetailsDiv').hide()
        } else
            $('#DetailsDiv').show()
    })
  

    self.Report = (data) => {
        if (self.ValidateCompany()) {
            Openloader()
            var companyCode = self.SelectedCompany()
            $.ajax({
                type: "post",
                url: '/Reports/SignatureReport/GenerateReport',
                data: {
                    'CompCode': companyCode,
                    'DateFrom': self.DateFrom(),
                    'DateTo':self.DateTo(),
                    'HolderFrom':self.HolderFrom(),
                    'HolderTo': self.HolderTo(),
                    'SelectedAction': self.SignatureApprovedUnapproved()
                },
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    if (result.isSuccess) {
                        var fileName = result.message;
                        var a = document.createElement("a");
                        a.href = "data:application/octet-stream;base64," + result.responseData;
                        a.download = fileName;
                        a.click();
                    }
                     else {
                        alert('warning', result.message)
                    }
                },
                error: function (error) {
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
    ko.applyBindings(new SignatureReport());
    $('#simple-date1 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
    $('#simple-date2 .input-group.date').datepicker({
        todayHighlight: true,
        endDate: '+0d',
        format: 'yyyy-mm-dd',
    });
});