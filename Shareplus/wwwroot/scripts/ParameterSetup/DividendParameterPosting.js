function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}


var DividendParameterPosting = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()


    self.ActionType = ko.observable()

    // dataTable for the Holder List
    var dataTable;
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
    //load Holder List waiting for Posting

    loadDataTable = function () {
        if (!Validate.empty(self.SelectedCompany())) {
            dataTable = $('#DT_DividendParameter').dataTable({
                "ajax": {
                    "url": "/ParameterSetup/DividendPosting/GetDividendForApproval?CompCode=" + self.SelectedCompany(),
                    "type": "POST", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    "datatype": "json",
                    "dataSrc": function (json) {
                        if (json.isSuccess) {
                            if (json.responseData == null) {
                                return [];
                            }
                            else {
                                return json.responseData;
                            }
                        } else {
                            alert('error', json.message)
                            return [];
                        }

                    },
                    "complete": () => {
                        Closeloader()
                    }
                },
                "columns": [
                    {
                        "width": "5%",
                        "data": "divcode",
                        "render": function (data) {
                            return `<input  type='checkbox' class='form-control-lg' >`

                        },
                    },
                    { "data": "divcode", "width": "10%" },
                    { "data": "description", "visible": false, "width": "10%" },
                    { "data": "agmNo", "width": "10%" },
                    { "data": "fiscalYr", "width": "20%" },
                    { "data": "bonusShPer", "width": "10%" },
                    { "data": "taxper", "width": "10%" },
                    { "data": "username", "width": "10%" },
                    {
                        "data": "dDeclareDt", "render": (data) => {
                            return convertDate(data)
                        },
                        "width": "10%"
                    },
                    { "data": "divType", "visible": false, "width": "10%" },
                    { "data": "tablename1", "visible": false,  "width": "0%" },
                    { "data": "tablename2", "visible": false, "width": "0%" }
                ],

                "columnDefs": [{
                    "targets": 'no-sort',
                    "orderable": false,

                }],
                "language": {
                    "emptyTable": "no data found"
                }
            });
        }
    }

    self.SelectedCompany.subscribe(() => {

    loadDataTable();
    })

    self.Validation = function () {
        var errMsg = "";

        if (self.SelectedCompany() == undefined) {
            errMsg += "Please Select Company !!!</br>";
        }

        if ($('#DT_DividendParameter').find('input[type=checkbox]:checked').length <= 0) {
            errMsg += "Please Tick the Dividend Info !!!</br>";
        }

        if (Validate.empty(self.PostingDate())) {
            errMsg += "Please Enter Posting Date !!!</br>";
        }
        if (errMsg !== "") {
            alert('error', errMsg);
            return false;
        }
        else {
            return true;
        }

    }

    self.PostDividendSetUp = function (data) {

        if (self.Validation()) {
            Openloader()
            record = []
            self.ActionType(data)
            for (var i = 0; i < $('#DT_DividendParameter').DataTable().data().count(); i++) {
                var x = i + 1;
                var Check = $($('#DT_DividendParameter').DataTable().row(i).nodes()).find('input').prop('checked');
                if (Check != undefined && Check != "" && Check != false) {


                    var DIS = {
                        Divcode: $('#DT_DividendParameter').DataTable().row(i).data().divcode,
                        tablename1: $('#DT_DividendParameter').DataTable().row(i).data().tablename1,
                        tablename2: $('#DT_DividendParameter').DataTable().row(i).data().tablename2,
                        DivType: $('#DT_DividendParameter').DataTable().row(i).data().divType
                    }
                    record.push(DIS)
                }
            }


            $.ajax({
                type: "POST", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/ParameterSetup/DividendPosting/DividendRequestPosting',
                data: { 'aTTDividend': record, 'CompCode': self.SelectedCompany(), 'ActionType': data },

                success: function (result) {
                    if (result.isSuccess) {
                        alert('success', result.message)
                        $('#DT_DividendParameter').DataTable().destroy()
                        loadDataTable();
                        self.ClearControl()
                    }
                    else {
                        alert('error', result.message)
                        $('#DT_DividendParameter').DataTable().destroy()
                        loadDataTable();
                    }
                },
                error: function (eror) {
                    $('#DT_DividendParameter').DataTable().destroy()
                    loadDataTable();
                    alert('error', error)

                },
                complete: () => {
                    Closeloader()
                }
            })

        }
    }


}
$(document).ready(function () {
    ko.applyBindings(new DividendParameterPosting());

});

