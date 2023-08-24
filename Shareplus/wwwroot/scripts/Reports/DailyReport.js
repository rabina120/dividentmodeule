﻿var DailyReport = function () {
    self.DailyReportDate = ko.observable()
    self.DailyReportDate(formatDate(new Date))
    self.GenerateReport = function (data) {
        
            if (!Validate.empty(self.DailyReportDate())) {
                Openloader()
                
                $.ajax({
                    url: '/Reports/DailyReport/GenerateReport',
                    type: "post",
                    data: {  'DailyReportDate': self.DailyReportDate() },
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: (result) => {
                        if (result.isSuccess) {
                            var byteArray = Uint8Array.from(atob(result.responseData), c => c.charCodeAt(0)); // Decode base64 data
                            var pdfBlob = new Blob([byteArray], { type: 'application/pdf' });
                            var x = window.open();
                            if (x) {
                                x.document.open();
                                x.document.write('<iframe width="100%" height="100%" src="' + URL.createObjectURL(pdfBlob) + '"></iframe>');
                                x.document.title = result.message;
                                x.document.close();
                            } else {
                                alert('warning', 'Failed to open file.');
                            }
                        }
                        else {
                            alert('error', result.message);
                        }

                    }, error: function (error) {
                        alert('error', error.message)
                    },
                    complete: () => {

                        Closeloader()
                    }
                })

            }
            else {
                alert('error', 'Please Select Date !!!');
            }
        
    }

}



$(document).ready(function () {
    $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });

    ko.applyBindings(new DailyReport())
});
