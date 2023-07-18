function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compCode + " "+ data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}

function Dividend(data) {
    var self = this;
    if (data != undefined) {
        self.AgmNo = ko.observable(data.agmNo);
        self.compcode = ko.observable(data.compcode);
        self.Divcode = ko.observable(data.divcode);
        self.Description = ko.observable(data.description);
        self.FiscalYr = ko.observable(data.fiscalYr);
        self.ShowDividendText = self.Divcode() + " " + self.Description() + " " + self.FiscalYr();

    }
}

function ExcelSheetName(data,index) {
    var self = this;
    if (data != undefined) {
        self.SheetName = ko.observable(data);
        self.SheetId = ko.observable(index);
    }
}

var CashDividendBulkInsert = function () {
    //Companykolagi
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();

    //Dividend ko table list lai
    self.DividendLists = ko.observableArray([]);
    self.SelectedDividend = ko.observable();
    self.compcode = ko.observable();
    self.Divcode = ko.observable();
    self.Description = ko.observable();
    self.FiscalYr = ko.observable();
    self.AgmNo = ko.observable();

    self.SheetLists = ko.observableArray([]);
    self.SelectedSheet = ko.observable();
    //DIVIDEND
    self.WARRANTAMT = ko.observable();
    self.TOTALCOUNT = ko.observable();
    self.DivTax = ko.observable();
    self.bonusadj = ko.observable();
    self.TOTSHKITTA = ko.observable();
    self.tFRACKITTA = ko.observable();
    self.Totaldividend = ko.observable();
    self.TotalBonus = ko.observable();
    self.BonusTax = ko.observable();
    self.PrevAdj = ko.observable();
    self.totalIssueBonus = ko.observable();
    self.TotalRefractionbonus = ko.observable();
    self.TotalNetPay = ko.observable();


    self.TOTALWARRANTAMT = ko.observable();
    self.TOTALTAXDAMT = ko.observable();

    //BONUS
    self.TOTALCOUNTBONUS = ko.observable();
    self.TOTALKITTABONUS = ko.observable();
    self.TFRACKITTABONUS = ko.observable();
    self.TOTALBONUS = ko.observable();
    self.ACTUALBONUSBONUS = ko.observable();
    self.ACUTALBONUSWITHPREVFRACBONUS = ko.observable();
    self.ISSUEBONUSBONUS = ko.observable();
    self.REMFRACBONUS = ko.observable();
    self.BONUSTAXBONUS = ko.observable();

    self.ConfirmSubbmission = ko.observable();


    self.DivBasedOn = ko.observable();
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

    self.SelectedDividend.subscribe(function (data) {
        

        self.AgmNo(self.DividendLists().find(x => x.Divcode = data).AgmNo())



    })

    self.DivBasedOn.subscribe(function () {
        if (!Validate.empty(self.SelectedCompany())) {
            self.LoadDividendLists();

        }
        
    });
    self.SelectedCompany.subscribe(() => {
        if (!Validate.empty(self.DivBasedOn())) {
            self.LoadDividendLists();

        }
    })
    //Loading dividend lists select options
    self.LoadDividendLists = function () {
        var companyCode = self.SelectedCompany()
        if (companyCode != '') {
            $.ajax({
                type: "post",
                url: '/DividendManagement/BulkInsert/GetAllDividends',
                data: { 'CompCode': companyCode, 'DivType': self.DivBasedOn()},
                datatype: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (result) {
                    self.DividendLists([]);
                    $('#DividendList').attr("disabled", true);

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


    self.Validation = () => {
        var errMsg = "";
        if (Validate.empty(self.SelectedDividend())) {
            errMsg += "Please Choose Dividend<br/> "
        }
        if (Validate.empty(self.SelectedSheet())) {
            errMsg += "Please Choose Sheet <br/>"
        }
        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
        }

    }


    var fileExtension = ""
    var filename = ""
    var extension = ""
    var fdata = ""
    var fdata = ""
    var fileUpload = ""
    var files = ""

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
        Openloader()

        $.ajax({
            type: "POST",
            url: "/DividendManagement/BulkInsert/GetExcelSheetNames",

            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: fdata ,
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

    self.CheckRecords = function () {
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
        fdata.append('postedFile', files[0]);;

        if (self.Validation()) {
            swal({
                title: "Are you sure?",
                text: "You want to Check the Excel File",
                icon: "warning",
                buttons: true,
                dangerMode: true
            }).then((willSave) => {
                if (willSave) {
                    Openloader()
                    $.ajax({
                        type: "POST",
                        url: "/DividendManagement/BulkInsert/CheckRecord?SheetId=" + ko.toJS(self.SelectedSheet().SheetId()) + "&DivCode=" + self.SelectedDividend() + "&CompCode=" + self.SelectedCompany() + "&ActionType=" + self.DivBasedOn(),
                        async: true,
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        data: fdata ,
                        contentType: false,
                        processData: false,
                        async: false,
                        success: function (response) {
                            if (response.isSuccess) {
                                $('#checktabledividend,checktablebonus').hide();
                                $('#confirmDiv').show();

                                if (self.DivBasedOn() == "01" || self.DivBasedOn() == "02") {
                                    $('#checktabledividend').show();
                                    self.TOTALCOUNT(response.responseData.totalcount);
                                    self.TOTALWARRANTAMT(response.responseData.warrantamt);
                                    self.TOTSHKITTA(response.responseData.totshkitta);
                                    self.TOTALTAXDAMT(response.responseData.divTax);
                                } else {
                                    $('#checktablebonus').show();
                                    self.TOTALCOUNTBONUS(response.responseData.totalcount)
                                    self.TOTALKITTABONUS(response.responseData.totalkitta)
                                    self.TFRACKITTABONUS(response.responseData.tfrackitta)
                                    self.TOTALBONUS(response.responseData.total)
                                    self.ACTUALBONUSBONUS(response.responseData.actualbonus)
                                    self.ACUTALBONUSWITHPREVFRACBONUS(response.responseData.actualbonuswithprevfrac)
                                    self.ISSUEBONUSBONUS(response.responseData.issuebonus)
                                    self.REMFRACBONUS(response.responseData.remfrac)
                                    self.BONUSTAXBONUS(response.responseData.bonustax)
                                    

                                }
                                
                                alert('success', response.message)
                            } else {
                                alert('error', response.message)
                                $('confirmDiv').hide();
                            }
                        },
                        error: function (error) {
                            alert('error', error.message)
                            $('confirmDiv').hide();
                        },
                        complete: () => {
                            Closeloader()
                            /*ClearControl()*/
                        }
                    });
                }
            });
        }

    }

    //generate dummy data
    self.DummyDataDownload = function () {
        if (Validate.empty(self.DivBasedOn())) {
            alert('warning','Please Select a Div Type First !!!')   
        } else {
            window.location.href = "/DividendManagement/BulkInsert/DownloadExcelDocument?DivBasedOn="+self.DivBasedOn()
        }
    }

    self.UploadRecords = function () {
        if (Validate.empty(self.ConfirmSubbmission())) {
            alert('warning', 'Please Confirm Before Upload!')
        } else {
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
            fdata.append('postedFile', files[0]);;

            if (self.Validation()) {
                swal({
                    title: "Are you sure?",
                    text: "You want to Upload the Excel File",
                    icon: "warning",
                    buttons: true,
                    dangerMode: true
                }).then((willSave) => {
                    if (willSave) {
                        Openloader()
                        $.ajax({
                            type: "POST",
                            url: "/DividendManagement/BulkInsert/UploadSheet?SheetId=" + ko.toJS(self.SelectedSheet().SheetId()) + "&DivCode=" + self.SelectedDividend() + "&Agmno=" + self.AgmNo() + "&CompCode=" + self.SelectedCompany() + "&ActionType=" + self.DivBasedOn(),
                            async: true,
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            data: fdata,
                            contentType: false,
                            processData: false,
                            async: false,
                            success: function (response) {
                                if (response.isSuccess) {
                                    alert('success', response.message)


                                } else {
                                    alert('error', response.message)
                                    /*location.reload() */
                                }
                            },
                            error: function (error) {
                                alert('error', error.message)
                            },
                            complete: () => {
                                Closeloader()


                            }
                        });
                    }
                });
            }    
        }



        

    }
    self.ClearControl = function () {
        fileExtension = ""
        filename = ""
        extension = ""
        fdata = ""
        fdata = ""
        fileUpload = ""
        files = ""
        self.SheetLists('')
        self.SelectedSheet('')
        self.SelectedDividend('')
        $('#fileupload').val('');
        $('#Save').attr('disabled', true);
        $('#checktabledividend').hide();
        $('#checktablebonus').hide();
        $('#confirmDiv').hide();
        $('#DividendList').val('').trigger('change')
        self.WARRANTAMT();
        self.TOTALCOUNT();
        self.DivTax();
        self.bonusadj();
        self.TOTSHKITTA();
        self.tFRACKITTA();
        self.TotalNetPay();
        self.totalIssueBonus();
        self.TotalRefractionbonus();
        self.BonusTax();
        self.PrevAdj();
        self.TOTALCOUNTBONUS()
        self.TOTALKITTABONUS()
        self.TFRACKITTABONUS()
        self.TOTALBONUS()
        self.ACTUALBONUSBONUS()
        self.ACUTALBONUSWITHPREVFRACBONUS()
        self.ISSUEBONUSBONUS()
        self.REMFRACBONUS()
        self.BONUSTAXBONUS()
        self.TOTALWARRANTAMT();
        self.TOTALTAXDAMT();
        self.LoadDividendLists();
    }
}

$(document).ready(function () {
    // Select2 Single  with Placeholder for Company

    
    ko.applyBindings(new CashDividendBulkInsert());
    $('#checktabledividend').hide();
    $('#checktablebonus').hide();
    $('#confirmDiv').hide();


});