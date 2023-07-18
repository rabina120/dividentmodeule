var loadCompanyTable;

$(document).ready(function () {
  

    $('#CompanyNameShow')[0].innerHTML = localStorage.getItem('company-name') == '' ? '' : localStorage.getItem('company-name');


    var now = new Date();
    var month = (now.getMonth() + 1);
    var day = now.getDate();
    if (month < 10)
        month = '0' + month;
    if (day < 10)
        day = '0' + day;
    var today = now.getFullYear() + '-' + month + '-' + day;
    self.PostingDate = ko.observable(today);



    loadCompanyTable = function () {

        $('#DT_load_COMPANY').DataTable().destroy();

        dataTable = $('#DT_load_COMPANY').dataTable({
            'ajax': {
                'url': '/Common/Company/GetCompanyDetails',
                'type': 'POST', beforeSend: function (xhr) {
                    xhr.setRequestHeader('XSRF-TOKEN',
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                'datatype': 'json',
                'dataSrc': function (json) {
                    
                    if (json.responseData.length == 1) {
                        loadCompanyByData(json.responseData[0].compCode);
                        $('.dropdown-item.company').hide();
                    }
                    else {
                        $('.dropdown-item.company').show();
                    }
                    

                    return json.responseData;
                }
            },
            'columns': [
                { 'data': 'compCode', 'visible': false },
                { 'data': 'compEnName' },
                { 'data': 'compEnAdd1' },
                { 'data': 'maxKitta', 'visible': false },
                {
                    'data': 'compCode',


                    'render': function (data, type, row) {

                        return `<div class="text-center">
                        <a onclick=ChooseCompany("${row.compCode}","${row.compEnName.replace(/\s/g, '-')}","${row.maxKitta}") class='btn btn-success text-white' style='cursor:pointer;'>
                            Choose
                        </a>
                      
                        </div>`;
                    },


                }
            ],
            'aoColumnDefs': [
                {
                    'bSortable': false,
                    'bTargets': [0, 1, 2, 3, 4],
                    'aTargets': [-1],
                    'bSearchable': false,
                    'aTargets': [0, 1, 2, 3, 4],
                }
            ],
            'bPaginate': false,
            'bLengthChange': false,
            'bFilter': false,
            'bInfo': false,
            'bAutoWidth': false,
            'searchable': false,
            'language': {
                'emptyTable': 'no data found'
            }
        });
    };

    self.ValidateCompany = function () {
        if (Validate.empty(self.SelectedCompany())) {
            return false;
        }
        else {
            return true;
        }
    }

    loadCompanyByData = (data) => {
        Openloader();
        return $.ajax({
            type: 'post',
            url: '/Common/Company/ConnectCompany',
            data: {
                CompCode: data,
            },
            datatype: 'json', beforeSend: function (xhr) {
                xhr.setRequestHeader('XSRF-TOKEN',
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    localStorage.setItem('company-code', result.responseData[0].compCode);
                    localStorage.setItem('company-name', result.responseData[0].compEnName);
                    localStorage.setItem('company-max-kitta', result.responseData[0].maxKitta);
                    window.location.reload();
                }
                else {
                    alert('error', result.message);
                }
            },
            error: function (error) {
                alert('error', error.message);
            },
            complete: () => {

            }
        });
    };

    loadCompanyByCompCode = () => {
        Openloader();
        $.ajax({
            type: 'post',
            url: '/Common/Company/GetCompanyDetailsByCompCode',
            data: {
                'CompCode': '001'
            },
            datatype: 'json', beforeSend: function (xhr) {
                xhr.setRequestHeader('XSRF-TOKEN',
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                    localStorage.setItem('company-code', result.responseData[0].compCode);
                    localStorage.setItem('company-name', result.responseData[0].compEnName);
                    localStorage.setItem('company-max-kitta', result.responseData[0].maxKitta);
                    window.location.reload();
                }
                else {
                    alert('error', result.message);
                }
            },
            error: function (error) {
                alert('error', error.message);
            },
            complete: () => {
                Closeloader();
            }
        });
    };



    if (localStorage.getItem('company-code') == null || localStorage.getItem('company-code') == undefined)
        loadCompanyTable();

    self.Logout = () => {
        localStorage.clear();
        $.ajax({
            type: 'post',
            url: '/Security/Login/Logout',
            
            datatype: 'json', beforeSend: function (xhr) {
                xhr.setRequestHeader('XSRF-TOKEN',
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                window.location.reload();
            },
            error: function (error) {
                alert('error', error.message);
            },
            complete: () => {
                window.location.reload();
            }
        });
    };

    self.ChooseCompany = function (data, compEnName, maxKitta) {
        loadCompanyByData(data)
            .then(() => {
                localStorage.setItem('company-code', data);
                localStorage.setItem('company-name', compEnName);
                localStorage.setItem('company-max-kitta', maxKitta);
                $('#modalCompany').modal('hide');
                window.location.reload();
            });
    };
    DisconnectCompany = function () {
        localStorage.removeItem('company-code');
        localStorage.removeItem('company-name');
        $('#CompanyNameShow').val('');

        $.ajax({
            type: 'post',
            url: '/Common/Company/DisconnectCompany',
          
             beforeSend: function (xhr) {
                xhr.setRequestHeader('XSRF-TOKEN',
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.isSuccess) {
                  
                    window.location.reload();
                }
                else {
                    alert('error', result.message);
                }
            },
            error: function (error) {
                alert('error', error.message);
            }
            
        });
        

    };
    
  
    $(".simple-date-picker").datepicker("setDate", new Date());
});


