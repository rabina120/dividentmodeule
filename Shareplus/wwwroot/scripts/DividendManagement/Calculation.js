function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function Dividend(data) {
    var self = this;
    if (data != undefined) {
        self.compcode = ko.observable(data.compcode);
        self.Divcode = ko.observable(data.divcode);
        self.Description = ko.observable(data.description);
        self.FiscalYr = ko.observable(data.fiscalYr);
        self.ShowDividendText = self.Divcode() + " " + self.Description() + " " + self.FiscalYr();

    }
}

function ExcelSheetName(data, index) {
    var self = this;
    if (data != undefined) {
        self.SheetName = ko.observable(data);
        self.SheetID = ko.observable(index);
    }
}

var Calculation = function () {
    var self = this;
    self.CompanyDetails = ko.observableArray([]);
    self.DivUploadList = ko.observableArray([]);
    self.SheetLists = ko.observableArray([]);
    self.DividendLists = ko.observableArray([]);
    self.BonusLists = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.SelectedDividend = ko.observable();
    self.SelectedBonus = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();
    self.SelectedSheet = ko.observable();
    self.Action = ko.observable('A');

    self.Pagination = ko.observable();
    self.HolderNo = ko.observable();
    self.HolderName = ko.observable();
    self.Address = ko.observable();
    self.TotalKitta = ko.observable();
    self.FractionKitta = ko.observable();
    self.Total = ko.observable();
    self.radioSelectedOptionValue = ko.observable();
    self.Boid = ko.observable();
    self.FilterParams = ko.observable({});





    self.ActualDividentAmount = ko.observable();
    self.DividendTax = ko.observable();
    self.NetPay = ko.observable();
    self.ActualBonus = ko.observable();
    self.BonusWithPrevFraction = ko.observable();
    self.IssueBonus = ko.observable();
    self.RemainingFraction = ko.observable();
    self.bonustax = ko.observable();




    self.VisibleBOID = ko.observable(false);
    self.VisibleHolderNo = ko.observable(true);

    self.radioSelectedOptionValue.subscribe(data => {
        self.DivUploadList([]);
        if (data == 'P') {
            self.VisibleHolderNo(true);
            self.VisibleBOID(false);
        }
        else {
            self.VisibleBOID(true);
            self.VisibleHolderNo(false);
        }
    })

    self.RefreshPage = function (resp) {
        self.DivUploadList(resp.map(x => ({
            holderNo: x.HolderNo,
            boid: x.Boid,
            holderName: x.HolderName,
            address: x.Address,
            totalKitta: x.TotalKitta,
            fractionKitta: x.FractionKitta,
            total: x.Total,
            CompanyId: x.CompanyId,
            ActualDividentAmount: x.ActualDividentAmount,
            DividendTax: x.DividendTax,
            NetPay: x.NetPay,
            ActualBonus: x.ActualBonus,
            BonusWithPrevFraction: x.BonusWithPrevFraction,
            IssueBonus: x.IssueBonus,
            RemainingFraction: x.RemainingFraction,
            bonustax: x.bonustax, 
        })));
    }



    self.AllListData = ko.observableArray([]);


    self.Getdata = function () { 

        $.ajax({
            type: "post",
            url: '/DividendManagement/Calculation/GetAllCalculationData',
            data: { 'CompanyId': self.SelectedCompany(), 'selectedOption': self.radioSelectedOptionValue(), 'pageNo': 0 ,'pageSize':0 },
            datatype: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                self.AllListData([]);
                var resp = JSON.parse(result);
                    self.AllListData(resp.ResponseData.map(x => ({
                        holderNo: x.HolderNo,
                        boid: x.Boid,
                        holderName: x.HolderName,
                        address: x.Address,
                        totalKitta: x.TotalKitta,
                        fractionKitta: x.FractionKitta,
                        total: x.Total,
                        CompanyId: x.CompanyId,
                        ActualDividentAmount: x.ActualDividentAmount,
                        DividendTax: x.DividendTax,
                        NetPay: x.NetPay,
                        ActualBonus: x.ActualBonus,
                        BonusWithPrevFraction: x.BonusWithPrevFraction,
                        IssueBonus: x.IssueBonus,
                        RemainingFraction: x.RemainingFraction,
                        bonustax: x.bonustax,
                    })));
            },
            error: function (error) {
                alert('error', error.message)
            }
        })


    }

    self.Search = function () {
        self.FilterParams({
            selectedOption: self.radioSelectedOptionValue(),
            CompanyId: self.SelectedCompany(),
        });

        if (!Validate.empty(self.radioSelectedOptionValue()) && !Validate.empty(self.SelectedCompany())) {
            $("#Companysel").prop('disabled', true);
            self.Pagination.FetchData();
        }
        else {
            alert('warning', 'Please Select Company and Div type');  
        }
    }
    var defaultData = {
        PostUrl: '/DividendManagement/Calculation/GetAllCalculationData',
        CallBack: self.RefreshPage,
        FilterParams: self.FilterParams,
    }
    self.Pagination = new koPaginator(defaultData);

    self.Validation = function () {
        var errorMessage = '';

        if (self.radioSelectedOptionValue() == 'P') {
            if (Validate.empty(self.HolderNo())) {
                errorMessage += 'Please Enter Holder No <br>';
            }
        }
        else {
            if (Validate.empty(self.Boid())) {
                errorMessage += 'Please Enter Boid <br>';
            }
        }


        if (Validate.empty(self.HolderName())) {
            errorMessage += 'Please Enter Holder Name<br>';
        }

        if (Validate.empty(self.TotalKitta())) {
            errorMessage += 'Please Enter Total Kitta <br>';
        }

        if (Validate.empty(self.FractionKitta())) {
            errorMessage += 'Please Enter Fraction Kitta <br>';
        }

        if (Validate.empty(self.SelectedCompany())) {
            errorMessage += 'Please Select Company <br>';
        }

        if (Validate.empty(self.radioSelectedOptionValue())) {
            errorMessage += 'Please Select Div type <br>';
        }

        if (errorMessage != "") {
            alert('warning', errorMessage)
            return false;
        }
        else {
            return true;
        }
    }

    self.SelectedCompany.subscribe(() => {
        self.radioSelectedOptionValue('');
       
    })

    self.radioSelectedOptionValue.subscribe(val => {
        self.DivUploadList([]);
        self.ClearControls();
        if (!Validate.empty(self.SelectedCompany())) {
            if (self.radioSelectedOptionValue() == 'P') {
                self.LoadDividendLists('01');
                self.LoadBonusLists('03');
            }
            else if (self.radioSelectedOptionValue() == 'D') {
                self.LoadDividendLists('02');
                self.LoadBonusLists('04');
            }
        }
    });




    //Loading dividend lists select options
    self.LoadDividendLists = function (divdata) {
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            $.ajax({
                type: "post",
                url: '/DividendManagement/BulkInsert/GetAllDividends',
                data: { 'CompCode': companyCode, 'DivType': divdata },
                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    self.DividendLists([]);
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Dividend(item)
                        });
                        self.DividendLists(mappedTasks);
                        $('#DividendList').attr("disabled", false);
                    } else {
                        alert('warning', "No Dividend List Found ")
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })
        }

    }


    //Loading bonus lists select options
    self.LoadBonusLists = function (divdata) {
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            $.ajax({
                type: "post",
                url: '/DividendManagement/BulkInsert/GetAllDividends',
                data: { 'CompCode': companyCode, 'DivType': divdata },
                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    self.BonusLists([]);
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Dividend(item)
                        });
                        self.BonusLists(mappedTasks);
                        $('#DividendList').attr("disabled", false);
                    } else {
                        alert('warning', "No Bonus List Found ")
                    }
                },
                error: function (error) {
                    alert('error', error.message)
                }
            })
        }

    }


    //save individual records
    self.Save = function () {
        var obj = {
            Action: self.Action(),
            Boid: self.Boid(),
            HolderNo: self.HolderNo(),
            HolderName: self.HolderName(),
            Address: self.Address(),
            TotalKitta: self.TotalKitta(),
            FractionKitta: self.FractionKitta(),
            Total: self.Total(),
            CompanyId: self.SelectedCompany(),
        };

        postReq('/DividendManagement/Calculation/SaveCalculation', { 'data': obj, 'selectedOption': self.radioSelectedOptionValue() }, null, resp => {
            console.log(resp);
        }, null);
    }


    self.DownloadDummyExcel = () => {
        window.location.href = "/DividendManagement/Calculation/DownloadExcelDocument?selectedoption=" + self.radioSelectedOptionValue();
        return true;
    }

    self.ResetForm = function () {
        $("#Companysel").prop('disabled', false);
        self.SelectedCompany(null);
        self.radioSelectedOptionValue(null);
        self.SelectedDividend(null);
        self.SelectedBonus(null);
        self.DivUploadList([]);
    }



    self.CalculateValidation = function () {
        var errmsg = "";
        if (Validate.empty(self.SelectedCompany())) {
            errmsg +="Please select Company for calculation !!!!<br>"   
        }

        if (Validate.empty(self.radioSelectedOptionValue())) {
            errmsg += "Please select Div type for calculation !!!!<br>"
        }

        if (Validate.empty(self.SelectedBonus()) && Validate.empty(self.SelectedDividend())) {
            errmsg += "Please select Dividend or Bonus for calculations !!!! <br>"
        }


        if (self.DivUploadList().length == 0) {
            errmsg += "Please add data for calculations(kitta,holderno,Holder Name) !!!! <br>"
        }

        if (errmsg != "") {
            alert('warning', errmsg);
            return false;
        }
        else {
            return true;
        }
            
    }


    //calculate bonus or divident
    self.Calculate = function () {
        //self.Getdata();
        Openloader();
        if (self.CalculateValidation()) {
            $.ajax({
                type: "post",
                url: '/DividendManagement/Calculation/Calculate',
                data: { /*'args': JSON.stringify(self.AllListData()),*/'compcode': self.SelectedCompany(), 'selectedOption': self.radioSelectedOptionValue(), 'Bonus': self.SelectedBonus(), 'Divident': self.SelectedDividend() },
                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    var res = JSON.parse(result);
                    alert('success', res.Message);
                    self.Pagination.FetchData();
                },
                error: function (error) {
                    alert('error', error.message)
                },
                complete: function () {
                    Closeloader();
                }
                //complete: () => {
                //    Closeloader()
                //}
            });
        }
    }



    self.AddDiv = function (data) {
        if (self.Validation()) {
            self.Save();
            self.Pagination.FetchData();
            self.ClearControls();
        }
    }

    self.EditDiv = function (data) {
        document.getElementById('forTopView').scrollIntoView();
        document.getElementById("forFocusInput").focus();
        self.Action('E');
        self.Boid(data.boid);
        self.HolderNo(data.holderNo);
        self.HolderName(data.holderName);
        self.Address(data.address);
        self.TotalKitta(data.totalKitta);
        self.FractionKitta(data.fractionKitta);
        self.Total(data.total);

        self.ActualDividentAmount(data.ActualDividentAmount);
        self.DividendTax(data.DividendTax);
        self.NetPay(data.NetPay);
        self.ActualBonus(data.ActualBonus);
        self.BonusWithPrevFraction(data.BonusWithPrevFraction);
        self.IssueBonus(data.IssueBonus);
        self.RemainingFraction(data.RemainingFraction);
        self.bonustax(data.bonustax);

        $("#holderno").prop("disabled", true);
        $("#boid").prop("disabled", true);
    }


    $('#fileupload').on('change', function () {
        fileExtension = ['xls', 'xlsx'];
        filename = $('#fileupload').val();
        if (filename.length == 0) {
            alert("Please select a file.");
            return false;
        }
        else {
            extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                alert("Please select only excel files.");
                return false;
            }
        }
        fdata = new FormData();
        fileUpload = $("#fileupload").get(0);
        files = fileUpload.files;
        fdata.append(files[0].name, files[0]);
        console.log(fdata, 'fdata');
        Openloader()

        //postreqfile is from common.js

        //postReqFile(
        //    "/DividendManagement/Calculation/GetSheetNames?CompCode=" + self.SelectedCompany(),
        //    fdata,
        //    null,
        //    resp => {
        //        if (resp.isSuccess) {
        //            var mappedTask = $.map(resp.responseData, (item, i) => {
        //                    return new ExcelSheetName(item, i)
        //                });
        //                self.SheetLists(mappedTask)
        //        }
        //        else {
        //            alert('error', response.Message)
        //        }
        //        Closeloader();
        //    },
        //    null
        //)



        $.ajax({
            type: "post",
            url: "/DividendManagement/Calculation/GetSheetNames?CompCode=" + self.SelectedCompany(),
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: fdata,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.isSuccess) {
                    var mappedTask = $.map(response.responseData, (item, i) => {
                        return new ExcelSheetName(item, i)
                    });

                    self.SheetLists(mappedTask)
                } else {
                    alert('error', response.message)
                }
            },
            error: function (error) {
                alert('error', error.message)
            },
            complete: () => {
                Closeloader()

            }
        });
    })


    self.UploadExcel = () => {
        if (!Validate.empty(self.SelectedCompany()) && !Validate.empty(self.radioSelectedOptionValue())) {
            fileExtension = ['xls', 'xlsx'];
            filename = $('#fileupload').val();

            if (filename.length == 0) {
                toastr.error("Please select a file.");
                return false;
            }
            else {
                extension = filename.replace(/^.*\./, '');
                if ($.inArray(extension, fileExtension) == -1) {
                    toastr.error("Please select only excel files.");
                    return false;
                }
            }
            fdata = new FormData();
            fileUpload = $("#fileupload").get(0);
            files = fileUpload.files;
            fdata.append('postedFile', files[0]);

            //console.log(files)

            swal({
                title: "Are you sure?",
                text: "You want to Upload the Excel File",
                icon: "warning",
                buttons: true,
                dangerMode: true
            }).then((willSave) => {
                if (willSave) {
                    $('#tbl_Excel_Data').DataTable().clear();
                    $('#tbl_Excel_Data').DataTable().destroy();
                    Openloader();
                    $.ajax({
                        type: "POST",
                        url: "/DividendManagement/Calculation/UploadSheet?SheetId=" + self.SelectedSheet()?.SheetID() + "&CompCode=" + self.SelectedCompany() + "&selectedOption=" + self.radioSelectedOptionValue(),
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        data: fdata,
                        contentType: false,
                        processData: false,
                        async: false,
                        success: (data) => {
                            console.log('-----', data);
                            if (data.isSuccess) {
                                alert('success', data.message);
                                self.FilterParams({
                                    selectedOption: self.radioSelectedOptionValue(),
                                    CompanyId: self.SelectedCompany(),
                                });
                                self.Pagination.FetchData();
                            } else {
                                alert('error', data.message)

                            }
                        },
                        error: (error) => {
                            alert('error', error.message)

                        },
                        complete: () => {
                            Closeloader()
                        }

                    });

                }

            });
        }
        else {
            alert('warning', 'Please Select Company/Div type Before Uploading');
        }
    }


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

    self.ClearControls = function () {
        self.Boid('');
        self.HolderName('');
        self.HolderNo('');
        self.Address('');
        self.TotalKitta('');
        self.FractionKitta('');
        self.Total('');

    }
}
$(function () {
    ko.applyBindings(new Calculation())

})
