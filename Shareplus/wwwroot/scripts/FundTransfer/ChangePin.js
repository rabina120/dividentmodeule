var changeviewmodel = function () {
    var self = this;
    self.OldPin = ko.observable('');
    self.NewPin = ko.observable('');
    self.NewCurrentPin = ko.observable('');

    self.UpdatePin = function () {
        // Validate inputs before making the AJAX call
        if (!self.OldPin() && !self.NewPin() == !self.NewCurrentPin()) {
            alert("All fields are required.");
            return;
        }
        else if (self.NewPin() == self.NewCurrentPin()) {
            alert("All fields are required.");
        }
        else {
            alert("Incorrect Pin.Pin Should be Matched");
        }

        $.ajax({
            type: "POST",
            url: '/FundTransfer/ChangePin/ChangePin',
            datatype: "json",
            data: {
                OldPin: self.OldPin(),
                NewPin: self.NewPin(),
                NewCurrentPin: self.NewCurrentPin()
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (response) {
                if (response.isSuccess==true) {
                    alert("Pin updated successfully!");
                } else {
                    alert(response.responseMessage || "Failed to update Pin. Please try again.");
                }
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText, status, error);
                alert("An error occurred while updating the Pin.");
            }
        });
    };
};

$(document).ready(function () {
    ko.applyBindings(new changeviewmodel());
});
