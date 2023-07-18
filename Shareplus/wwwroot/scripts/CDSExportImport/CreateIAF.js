function ExcelSheetName(data, index) {
    var self = this;
    if (data != undefined) {
        self.SheetName = ko.observable(data);
        self.SheetId = ko.observable(index);
    }
}
function ParaComp(data) {
    var self = this;
    if (data != undefined) {
        self.CompCode = ko.observable(data.compCode)
        self.CompEnName = ko.observable(data.compCode + " " + data.compEnName)
        self.MaxKitta = ko.observable(data.maxKitta)
    }
}
function sheetdata(data) {
    var self = this;
    if (data != undefined) {

        self.BOID = ko.observable(data.boid);
        self.CurrentKitta = ko.observable(data.currentKitta);
        self.LockKitta = ko.observable(data.lockKitta);
        self.LockCode = ko.observable(data.lockCode);
        self.LockReason = ko.observable(data.lockReason);
        self.LockDate = ko.observable(data.lockDate);
        self.RTAReg = ko.observable(data.rtaReg);
    }
}

var CreateIAFFile = function () {
    self.CompanyDetails = ko.observableArray([]);
    self.SelectedCompany = ko.observable();
    self.CompCode = ko.observable();
    self.CompEnName = ko.observable();
    self.CompEnAdd1 = ko.observable();
    self.MaxKitta = ko.observable();
    self.datas = ko.observableArray([]);

    self.SheetLists = ko.observableArray([]);
    self.SelectedSheet = ko.observable();

    self.startRowNo = ko.observable();
    self.totRow = ko.observable();
    self.BOID = ko.observable();
    self.CurrentKitta = ko.observable();
    self.LockKitta = ko.observable();
    self.LockCode = ko.observable();
    self.LockReason = ko.observable();
    self.LockDate = ko.observable();
    self.RTAReg = ko.observable();

    self.TotalRecord = ko.observable();
    self.TotalCurrentQty = ko.observable();   
    self.TotalLockQty = ko.observable();


    self.selectedItem = ko.observable();


  

    var fileExtension = ""
    var filename = ""
    var extension = ""
    var fdata = ""
    var fdata = ""
    var fileUpload = ""
    var files = ""

    //generate dummy data
    self.GenerateDummyExcel = function () {
        window.location.href = "/CDSExportImport/CreateIAF/DownloadExcelDocument"
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
        Openloader()

        $.ajax({
            type: "POST",
            url: "/CDSExportImport/CreateIAF/GetSheetNames",

            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: fdata,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.isSuccess) {
                    console.log(response.responseData)
                    var mappedTask = $.map(response.responseData, (item, i) => {
                        return new ExcelSheetName(item, i)
                    });

                    self.SheetLists(mappedTask)
                } else {
                    console.log(response.responseData)
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
    self.CreateIAF = function () {
        Openloader()
        $.ajax({
            url: "/CDSExportImport/CreateIAF/CreateFile",
            type: "POST",
            data: { 'Excel': JSON.stringify(ko.toJS(self.datas())), 'TotRecord': self.TotalRecord(), 'TotCurrQty': ko.toJS(self.TotalCurrentQty()), 'TotLockQty': self.TotalLockQty() },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (result) => {
                if (result.isSuccess) {
                    var fileName = result.message;
                    alert("success", fileName);
                    self.ClearControl();
                    var a = document.createElement("a");
                    a.href = "data:application/octet-stream;base64," + result.responseData;
                    a.download = "IAF" + ".iaf"
                    a.click();
                }
                else {
                    console.log('error => ' + result.message)
                    alert('error', result.message);
                }

            }, error: function (error) {
                console.log('error=>', error.message)
                alert('error', error.message)
            },
            complete: () => {

                Closeloader()
            }
        })
    }
    String.prototype.pad = function pad(str, max) {

        str = str.toString();
        return str.length < max ? pad("0" + str, max) : str;
    };
    var s = "";

    self.GetTotal = function () {

        var str2 = ".000";
        var Tot_Record = 0;
        var Tot_CurrQty = 0;
     
        var Tot_LockQty = 0;


        if (self.datas().length > 0) {
            for (var i = 0; i < self.datas().length; i++) {

                var x = i + 1;

                var Record = $(tblUsers.rows.item(x).cells[0]).text().trim();
                var CurrQty = $(tblUsers.rows.item(x).cells[2]).text().trim();
              
                var LockQty = $(tblUsers.rows.item(x).cells[3]).text().trim();

                Tot_Record = self.datas().length;
                Tot_CurrQty = (Tot_CurrQty + +CurrQty);
                Tot_LockQty = (Tot_LockQty + +LockQty);


                self.TotalRecord(s.pad((Tot_Record.toString()), 10));
                self.TotalCurrentQty(s.pad((Tot_CurrQty.toString()).concat(str2), 16));

                self.TotalLockQty(s.pad((Tot_LockQty.toString()).concat(str2), 16));



            }
        }
        else {
            self.TotRecord('');
            self.TotCurrQty('');
             
            self.TotLockQty('');
        }

    }
    self.Upload = function () {

        fileExtension = ['xls', 'xlsx'];
        filename = $('#fileupload').val();

        if (filename.length == 0|| filename=="") {
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
                self.datas([]);
                if (willSave) {

                    Openloader()
                    $.ajax({
                        type: "POST",
                        url: "/CDSExportImport/CreateIAF/Upload?SheetId=" + ko.toJS(self.SelectedSheet().SheetId()) + "&StartRow=" + self.startRowNo() + "&totRow=" + self.totRow(),
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        data: fdata,
                        contentType: false,
                        processData: false,
                        async: false,
                        success: (data) => {
                            if (data.isSuccess) {
                                self.ClearControl();
                                if (data.responseData.length > 0) {
                                    var mappedTasks = $.map(data.responseData, function (item) {
                                        return new sheetdata(item)
                                    });
                                    self.datas(mappedTasks);
                                    self.GetTotal();
                                    $('#tblUsers').DataTable({
                                        responsive: true,
                                        scrollX: true,
                                        scrollY: true,
                                        paging: true,
                                        ordering: false,
                                        fixedColumns: true
                                    })

                                }
                                else {
                                    alert('error', 'No Record Found')

                                }
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

    }
    
   
    self.Validation = function () {
        var errMsg = "";

        if (Validate.empty(self.SelectedSheet())) {
            errMsg += "Please Choose Sheet <br/>"
        }
        if (Validate.empty(self.startRowNo())) {
            errMsg += "Please Choose StartRowNo <br/>"
        }
        if (Validate.empty(self.totRow())) {
            errMsg += "Please Choose TotalRow <br/>"
        }
        if (errMsg == "") {
            return true
        } else {
            alert('error', errMsg)
            return false
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
        self.startRowNo('');
        self.totRow('');

        $('#fileupload').val('');
        $('#tblUsers').DataTable().clear()
        $('#tblUsers').DataTable().destroy();
        self.TotalRecord('');
        self.TotalCurrentQty('');        
        self.TotalLockQty('');

    }

}
$(document).ready(function () {

    ko.applyBindings(new CreateIAFFile());

});