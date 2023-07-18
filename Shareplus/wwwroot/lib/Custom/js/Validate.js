var Validate = {};
var flag = false;

Validate.empty = function (string) {

    if (string == undefined) {
        string = "";
    }

    // return string.replace(/\s+/g, '').length == 0;
    return string.length == 0;
}
Validate.email = function (string) {
    var pattern = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;

    if (!Validate.empty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}

Validate.url = function (string) {

    var pattern = /((?:https?\:\/\/|www\.)(?:[-a-z0-9]+\.)*[-a-z0-9]+.*)/i;

    if (!Validate.empty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}
Validate.date = function (string, preutc) {
    var date = Date.parse(string);
    if (isFinite(date)) {
        return true;
    }
    if (preutc) {
        var now = new Date();
        string = string.replace(/\d{4}/, now.getFullYear());
        date = Date.parse(string);
        return isFinite(date);
    }
    return false;
}
Validate.zip = function (string, plus4) {
    var pattern = plus4 ? /^\d{5}-\d{4}$/ : /^\d{5}$/;

    if (!Validate.empty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}
Validate.phone = function (string) {

    /*
    valid phone numbers examples
    (+351) 282 43 50 50
    90191919908
    555-8909
    001 6867684
    001 6867684x1
    1 (234) 567-8901
    1-234-567-8901 x1234
    1-234-567-8901 ext1234
    1-234 567.89/01 ext.1234
    1(234)5678901x1234
    (123)8575973
    (0055)(123)8575973
    */
    // var pattern = /^\(?([0-9]{3})\)?[-.\s]?([0-9]{3})[-.\s]?([0-9]{4})$/.test(string);
    var pattern = /^(?:(?:\(?(?:00|\+)([1-4]\d\d|[1-9]\d?)\)?)?[\-\.\ \\\/]?)?((?:\(?\d{1,}\)?[\-\.\ \\\/]?){0,})(?:[\-\.\ \\\/]?(?:#|ext\.?|extension|x)[\-\.\ \\\/]?(\d+))?$/i;
    if (!Validate.empty(string)) {
        if (pattern.test(string)) {

            return false;
        }
        else
            return true;
    }
    else {
        return true;
    }
}
Validate.creditCard = function (string) {
    var valid = /^[\d-\s]$/.test(string);
    if (!valid) {
        return false;
    }
    return Validate.luhn(string);
}
Validate.luhn = function (string) {
    var numeric = string.replace(/\d+/g, '');
    var digits = numeric.split('');
    var count = digits.length;
    var parity = count % 2;
    var total = 0;
    for (var i = 0; i < count; i++) {
        var digit = digits[i];
        if ((i % 2) == parity) {
            digit *= 2;
            if (digit > 9) {
                digit -= 9;
            }
        }
        total += digit;
    }
    return (total % 10) == 0;
}
Validate.integer = function (string) {

    var pattern = /^\-?\d+$/;

    if (!Validate.empty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}
Validate.numeric = function (string) {

    var pattern = /^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/;

    if (!Validate.empty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}
Validate.currency = function (string, us) {

    var pattern = /^\$-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/;

    if (!Validate.empty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}
Validate.ip = function (string) {
    var pattern = /^((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){3}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})$/;

    if (!Validate.empty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}

Validate.alpha = function (string) {

    var pattern = /^[a-z]$/i;

    if (!Validate.empty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}
Validate.alphaNumeric = function (string) {

    var pattern = /^[a-z0-9]$/i;

    if (!Validate.empty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}
Validate.lowercase = function (string) {

    if (!Validate.empty(string)) {

        if (string.toLowerCase() == string)
            return true;
        else
            return false;
    }
    else {
        return true;
    }
}
Validate.uppercase = function (string) {

    if (!Validate.empty(string)) {

        if (string.toUpperCase() == string)
            return true;
        else
            return false;
    }
    else {
        return true;
    }
}
Validate.min = function (string, length) {

    if (!Validate.empty(string)) {


        if (string.length >= length)
            return true;
        else
            return false;
    }
    else {
        return true;
    }

}
Validate.max = function (string, length) {

    if (!Validate.empty(string)) {

        if (string.length <= length)
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}
Validate.between = function (string, min, max) {

    if (!Validate.empty(string)) {
        if (string.length >= min && string.length <= max)
            return false;
        else
            return true;

    }
    else {
        return true;
    }

}