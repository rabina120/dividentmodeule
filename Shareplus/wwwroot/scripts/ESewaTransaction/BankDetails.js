

function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

var BankDetails = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([])
    self.SelectedCompany = ko.observable()
    self.CompCode = ko.observable()
    self.CompEnName = ko.observable()
    self.CompEnAdd1 = ko.observable()
    self.MaxKitta = ko.observable()
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

    //for datatable
    self.BankName = ko.observable();
    self.BankCode = ko.observable();
    self.SwiftCode = ko.observable();
    self.Account_Validation = ko.observable();
    self.BankList = ko.observableArray([]);
    self.Regex = ko.observable();

    self.ModalBankName = ko.observable();
    self.ModalSwiftCode = ko.observable();
    self.ModalBankCode = ko.observable();

    function Bank(data,index) {
        var self = this;
        if (data != undefined) {
            
            self.BankName = ko.observable(data.bank_Name)
            self.BankCode = ko.observable(data.nchl_code)
            self.SwiftCode = ko.observable(data.bank_Code)
            self.Account_Validation = ko.observable(data.account_validation)
            self.Regex = ko.observable(data.regex_code)
            

        }
    }

    self.UpdateBanks = function () {
        for (var i = 0; i < $('#BankDetails').DataTable().data().length; i++) {
            Openloader();
            var x = i + 1;
                record = []
                var Data = {
                    bank_Code: $('#BankDetails').DataTable().data()[i][1],
                    bank_Name: $('#BankDetails').DataTable().data()[i][0],
                    Account_Validation: $('#BankDetails').DataTable().data()[i][3],
                    nchl_code: $('#BankDetails').DataTable().data()[i][2],
                    Regex: $('#BankDetails').DataTable().data()[i][4]


                }
                record.push(Data)

                $.ajax({
                    type: "post",
                    url: '/ESewaTransaction/BankDetails/UpdateBanks',
                    datatype: "json",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    data: { "banklist":record},
                    success: function (data) {
                        if (data.isSuccess) {

                            alert('success', result.message)


                        } else {
                            alert('warning', result.message)
                        }
                    },
                    error: function (error) {
                        alert('error', error.message)
                    },
                    complete: function () {
                        Closeloader()
                    }
                })
            
        }
        
    }
    //update all bank lists
    self.GetBanksFromEsewa = function () {
      
                alert('warning', "Getting Bank Details <br/> Donot Leave this Page!!")
                $.ajax({
                    type: 'POST', beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    url: '/ESewaTransaction/BankDetails/GetBanksFromEsewa',
                    
                    dataType: 'json',
                    success: (result) => {
                        if (result.isSuccess) {
                            alert('success', result.message);
                        }
                        else {
                            alert('error', result.message);
                            
                        }
                    },
                    error: (error) => {
                        alert('error', error.message);
                        
                    },
                    complete: function () {
                        self.GetBanks()

                    }

                })
            
    }

    self.SetBankCode = function () {
        Openloader('Updating Bank Code')
        $.ajax({
            type: "post", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            url: '/ESewaTransaction/BankDetails/UpdateBankDetails',
            datatype: "json",
            data: {'bankcode':self.ModalBankCode,'swiftcode':self.ModalSwiftCode},
            success: function (data) {
                if (data.isSuccess) {
                    alert('sucess', data.message)
                    }
                    else {
                        alert('warning', data.message)

                    }
                },
            error: function (error) {
                alert('error', error.message)
            },
            complete: function () {
                Closeloader()
                $('#UpdateBankModal').modal('hide')
                GetBanks()
            }

        })

    }
    self.OpenModal = function (data) {
        $('#UpdateBankModal').modal('show')
        self.ModalBankName(data.BankName());
        self.ModalSwiftCode(data.SwiftCode());
        self.ModalBankCode(data.BankCode());

    }
    self.GetBanks = function () {
        Openloader('Loading Bank List')
        self.BankList([])
        $('#BankDetails').DataTable().clear()
        $('#BankDetails').DataTable().destroy();
            $.ajax({
                type: "post",
                url: '/ESewaTransaction/BankDetails/GetBanks',
                datatype: "json", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (data) {
                    if (data.isSuccess) {
                        if (data.responseData.length > 0) {
                            var mappedTasks = $.map(data.responseData, function (item,index) {
                                return new Bank(item,index+1)
                            });
                            self.BankList(mappedTasks);


                            $('#BankDetails').DataTable({
                                responsive: true,
                                searching: true,
                                scrollX: false,
                                scrollY: false,
                                scrollCollapse: true,
                                paging: false,
                                ordering: false,
                                fixedHeader: false,
                                "scrollY": "650px",
                                "sScrollX": "100%",
                                "scrollCollapse": true,

                            });
                            $('#BankDetailsDiv').show()
                        }
                        else {
                            alert('error', 'No Record Found')

                        }
                       

                       
                    } else {
                        alert('warning', data.message)
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: function () {
                    Closeloader()
                }
                
            })
        
    }
    
}

$(document).ready(function () {
    ko.applyBindings(new BankDetails());
    $('#BankDetailsDiv').hide()
   
});