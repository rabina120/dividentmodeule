
var Validate = {};
var flag = false;
getUrlParamVal = function (key) {

    var result = new RegExp(key + "=([^&]*)", "i").exec(window.location.search);
    return result && unescape(result[1]) || "";
};
Validate.empty = function (string) {

    if (string == undefined) {
        string = "";
    }

    // return string.replace(/\s+/g, '').length == 0;
    return string.length == 0;
}

Validate.validateType = function (string, type) {
    let contactTypes = JSON.parse(localStorage.getItem('contactTypes'));
    let typeName;
    if (typeof type === 'number')
        typeName = contactTypes.find(x => x.CTypeId == type).CTypeName.toLowerCase();
    else if (typeof type === 'string')
        typeName = type.toLowerCase();
    else
        throw 'validation type must be either a number (contact type) or string';
    let regexes = {
        'phone': /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/,
        'email': /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
        'telegram': /^\d+/,
    };

    try {
        return string && regexes[typeName].test(string);
    } catch (e) {
        throw 'validation type invalid';
    }
};

Validate.email = function (string) {
    var pattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    if (!Validate.empty(string)) {
       return !(pattern.test(string.trim()));
                //if (pattern.test(string))
                //    return false;
                //else
                //    return true;
    }
    else {
        return true;
    }
}
Validate.url = function (string) {

    var pattern = /((?:https?\:\/\/|www\.)(?:[-a-z0-9]+\.)*[-a-z0-9]+.*)/i;

    if (!Vempty(string)) {
        if (pattern.test(string))
            return false;
        else
            return true;
    }
    else {
        return true;
    }
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
function nospaces(t) {

    if (t.value.match(/\s/g)) {

        jAlert('Sorry, you are not allowed to enter any spaces');

        t.value = t.value.replace(/\s/g, '');

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

function isAlphabet(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if ((charCode > 64 && charCode < 91) || (charCode > 96 && charCode < 123)) {
        return true;
    }
    return false;
}

function isPhone(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    let allowedChars = '0123456789+-'.split('').map(x => x.charCodeAt(0));
    return allowedChars.includes(charCode);
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46) {
        return false;
    }
    return true;
}

function isNumberAndDev(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function validatePriceRange(minPrice, maxPrice) {
    if ((minPrice != undefined || minPrice != "") && (maxPrice != undefined || maxPrice != "")) {
        if (parseInt(minPrice) > parseInt(maxPrice))
            return false
        else
            return true;
    }
}
VDate = function (string) {
    var pattern = /([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))/;

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



//Checks if the textbox is empty or undefined or has spaces
function VEmpty(input) {
    if (!input) {
        return true;
    } else {
        return !input.toString().trim();
    }
}

//Allows numbers and - symbol
function isDate(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

//Allows only unicode text to be entered in the textbox
function convert_to_unicode(item) {     
    var array_one = new Array(
        "ç", "˜", "m", "'m", "]m", "Fmf", "Fm",

        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",

        "k|m", "em", "km", "Qm", "qm", "N˜",
        "¡", "¢", "!", "@", "$", ">", "?", "B", "I", "Q", "ß",
        "q", "„", "<", "•", ">", "§", "°", "¶", "¿", "Å",
        "Ë", "Ì", "Í", "Î", "Ý", "å",
        "6«", "7«", "8«", "9«",

        "Ø", "|",

        "8Þ", "9Þ",

        "S", "s", "V", "v", "U", "u", "£", "#", "ª",
        "R", "r", "%", "H", "h", "‰", "´", "~", "`",

        "^", "&", "*", "(", ")",
        "T", "t", "Y", "y", "b", "W", "w", "G", "g",

        "K", "k", "ˆ", "A", "a", "E", "e", "D", "d",
        "o", "/", "N", "n", "J", "j", "Z", "z", "i", ":", ";", "X", "x",

        "cf‘", "c‘f", "cf}", "cf]", "cf", "c", "O{", "O", "pm", "p", "C", "P]", "P",

        "f‘", "\"", "'", "+", "f", "[", "\\", "]", "}", "F", "L", "M",

        "्ा", "्ो", "्ौ", "अो", "अा", "आै", "आे", "ाो", "ाॅ", "ाे",
        "ंु", "ेे", "अै", "ाे", "अे", "ंा", "अॅ", "ाै", "ैा", "ंृ",
        "ँा", "ँू", "ेा", "ंे")
    var array_two = new Array(

      
        "ॐ", "ऽ", "फ", "m'", "m]", "mfF", "mF",

        "०", "१", "२", "३", "४", "५", "६", "७", "८", "९",

        "फ्र", "झ", "फ", "क्त", "क्र", "ल",
        "ज्ञ्", "द्घ", "ज्ञ", "द्द", "द्ध", "श्र", "रु", "द्य", "क्ष्", "त्त", "द्म",
        "त्र", "ध्र", "ङ्घ", "ड्ड", "द्र", "ट्ट", "ड्ढ", "ठ्ठ", "रू", "हृ",
        "ङ्ग", "त्र", "ङ्क", "ङ्ख", "ट्ठ", "द्व",
        "ट्र", "ठ्र", "ड्र", "ढ्र",

        "्य", "्र",

        "ड़", "ढ़",

        "क्", "क", "ख्", "ख", "ग्", "ग", "घ्", "घ", "ङ",
        "च्", "च", "छ", "ज्", "ज", "झ्", "झ", "ङ", "ञ्",

        "ट", "ठ", "ड", "ढ", "ण्",
        "त्", "त", "थ्", "थ", "द", "ध्", "ध", "न्", "न",

        "प्", "प", "फ्", "ब्", "ब", "भ्", "भ", "म्", "म",
        "य", "र", "ल्", "ल", "व्", "व", "श्", "श", "ष्", "स्", "स", "ह्", "ह",

        "ऑ", "ऑ", "औ", "ओ", "आ", "अ", "ई", "इ", "ऊ", "उ", "ऋ", "ऐ", "ए",

        "ॉ", "ू", "ु", "ं", "ा", "ृ", "्", "े", "ै", "ँ", "ी", "ः",

        "", "े", "ै", "ओ", "आ", "औ", "ओ", "ो", "ॉ", "ो",
        "ुं", "े", "अ‍ै", "ो", "अ‍े", "ां", "अ‍ॅ", "ौ", "ौ", "ृं",
        "ाँ", "ूँ", "ो", "ें")    
    var array_one_length = array_one.length;   

    var modified_substring = item.value;

    //****************************************************************************************
    //  Break the long text into small bunches of max. max_text_size  characters each.
    //****************************************************************************************
    var text_size = item.value.length;

    var processed_text = '';  //blank

    //**********************************************
    //    alert("text size = "+text_size);
    //**********************************************

    var sthiti1 = 0; var sthiti2 = 0; var chale_chalo = 1;

    var max_text_size = 6000;

    while (chale_chalo == 1) {
        sthiti1 = sthiti2;

        if (sthiti2 < (text_size - max_text_size)) {
            sthiti2 += max_text_size;
            while (item.value.charAt(sthiti2) != ' ') { sthiti2--; }
        }
        else { sthiti2 = text_size; chale_chalo = 0 }

        //   alert(" sthiti 1 = "+sthiti1); alert(" sthit 2 = "+sthiti2) 

        var modified_substring = item.value.substring(sthiti1, sthiti2);

        Replace_Symbols();

        processed_text += modified_substring;


        //****************************************************************************************
        //  Breaking part code over
        //****************************************************************************************
        //  processed_text = processed_text.replace( /mangal/g , "SUCHI-DEV-708 " ) ;   

        //document.getElementById("unicode_text").value = processed_text  ;
        item.value = processed_text;
    }       

    var remaining_text = item.value;
    var processed_text = "";  //blank initially

    var position_of_current_opening_bracket = 0;
    var position_of_next_closing_bracket = 1;

    var idx = remaining_text.indexOf("<p ")  // search starting from paragraphs. then search for sanskrit 99.
    idx = remaining_text.indexOf("Sanskrit 99", idx)
    var idx2 = 0  //  position_of_current_opening_bracket  ">"
    var idx3 = 0  //  position of "/span"
    var idx4 = 0  //  postion of "span" only , ie span without "/"

    while (idx != -1)    // while-01 loop     ;  while "Sanskrit 99"  is found..
    {
        idx2 = remaining_text.indexOf(">", idx)
        idx3 = remaining_text.indexOf("/span", idx2)
        idx4 = remaining_text.indexOf("span", idx2)


        while (idx4 < idx3)     // this loop to take care of  nested span.
        {
            idx4 = remaining_text.indexOf("span", idx3 + 4);
            idx3 = remaining_text.indexOf("/span", idx3 + 4);
        }


        var modified_substring = remaining_text.substring(idx2, idx3);
        modified_substring = modified_substring.replace(/>/g, ">>");
        processed_text = processed_text + remaining_text.substring(0, idx2) + modified_substring + "/span";

        remaining_text = remaining_text.substring(idx3 + 5);  //remaining_text excludes "/span"
        idx = remaining_text.indexOf("Sanskrit 99");

    } //end of outermost while-01


    processed_text = processed_text + remaining_text;


    // -----------------------------

    //  This section for taking care of paragraphs marked class = MsoBodyText  or class = MsoBodyText

    remaining_text = processed_text;
    processed_text = "";  //blank initially

    var position_of_start_of_paragraph = 0;
    var position_of_end_of_paragraph = 1;

    position_of_start_of_paragraph = remaining_text.indexOf("<p ");


    while (position_of_start_of_paragraph != -1)  //search for <p  in the remaining_text
    {

        position_of_start_of_paragraph = remaining_text.indexOf("<p ");
        position_of_end_of_paragraph = remaining_text.indexOf("/p>");

        modified_substring = remaining_text.substring(position_of_start_of_paragraph + 3, position_of_end_of_paragraph);


        if (modified_substring.indexOf("MsoBodyText") != -1) {
            modified_substring = modified_substring.replace(/>/g, ">>");    // repace all ">" with ">>" in this paragraph

            idx = modified_substring.indexOf("font-family");  // in Mybodytext, whereever font-family is found, it means it is roman text.
            idx2 = 0;  // position of ">>"
            idx3 = 0;  // position of "/span"
            idx4 = 0;  // position of "span" only without "/"

            while (idx != -1)     // again change ">>" to ">" only those which occure immediately after font-family:"Times new Roman"
            {

                idx2 = modified_substring.indexOf(">>", idx);
                idx3 = modified_substring.indexOf("/span", idx2);
                idx4 = modified_substring.indexOf("span", idx2);


                while (idx4 < idx3) {
                    idx4 = modified_substring.indexOf("span", idx3 + 4);
                    idx3 = modified_substring.indexOf("/span", idx3 + 4);
                }

                modified_substring = modified_substring.substring(0, idx2) + (modified_substring.substring(idx2, idx3)).replace(/>>/g, ">") + modified_substring.substring(idx3);
                idx = modified_substring.indexOf("font-family", idx3);

            } // end of while inner loop
        } // end of if statement

        processed_text = processed_text + remaining_text.substring(0, position_of_start_of_paragraph + 3) + modified_substring + "/p>";
        remaining_text = remaining_text.substring(position_of_end_of_paragraph + 3);
        position_of_start_of_paragraph = remaining_text.indexOf("<p ");

    } // end of outer while loop

    processed_text = processed_text + remaining_text;


    // ------------------------

    // Now do actual font conversion  of text occuring between  all the  pairs  >>  and  <

    remaining_text = processed_text; processed_text = "";

    idx2 = remaining_text.indexOf(">>");



    while (idx2 != -1)    // while-01 loop (checks if ">>" is still present
    {

        position_of_current_opening_bracket = remaining_text.indexOf(">>", idx2)
        position_of_next_closing_bracket = remaining_text.indexOf("<", position_of_current_opening_bracket)

        modified_substring = remaining_text.substring(position_of_current_opening_bracket + 2, position_of_next_closing_bracket);

        processed_text = processed_text + remaining_text.substring(0, position_of_current_opening_bracket + 1);   // ">" included by using +1 here
        remaining_text = remaining_text.substring(position_of_next_closing_bracket + 1);  //remaining_text excludes the closing bracket


        Replace_Symbols();   // call the subroutine and replace the legacy symbols  with corresponding Unicode.


        processed_text = processed_text + modified_substring + "<";

        idx2 = remaining_text.indexOf(">>");


    } //end of outermost while-01



    processed_text = processed_text + remaining_text;



    // do follwing conversions which are still left  or  were done incorrectly due to unavoidable reasons.

    // processed_text = processed_text.replace( /Sanskrit 99/g , "mangal" ) ;   

    // processed_text = processed_text.replace( /ृलतष/g , "ं" )   ;  
    // processed_text = processed_text.replace( /ृटुखतष/g , "घ" )  ;
    // processed_text = processed_text.replace( /ृामपष/g , "ृ" )  ;
    // processed_text = processed_text.replace( /ृगतष/g , ":" )  ;
    // processed_text = processed_text.replace( /ृनबसपष/g , "/&nbsp" )  ; 
    // processed_text = processed_text.replace( /ाॅ/g , "ॉ" )  ; 


    // now put the processed text in the output box finally.

    //document.getElementById("unicode_text").value = processed_text  
    item.value = processed_text

    //} // end of else loop for HTML case


    // --------------------------------------------------


    function Replace_Symbols() {

        //substitute array_two elements in place of corresponding array_one elements

        if (modified_substring != "")  // if stringto be converted is non-blank then no need of any processing.
        {
            for (input_symbol_idx = 0; input_symbol_idx < array_one_length; input_symbol_idx++) {               
                idx = 0;  // index of the symbol being searched for replacement

                while (idx != -1) //while-00
                {

                    modified_substring = modified_substring.replace(array_one[input_symbol_idx], array_two[input_symbol_idx])
                    idx = modified_substring.indexOf(array_one[input_symbol_idx])

                } // end of while-00 loop
                // alert(" end of while loop")
            } // end of for loop
            // alert(" end of for loop")

            // alert(" modified substring2 = "+modified_substring)
            //*******************************************************
            var position_of_i = modified_substring.indexOf("l")

            while (position_of_i != -1)  //while-02
            {
                var charecter_next_to_i = modified_substring.charAt(position_of_i + 1)
                var charecter_to_be_replaced = "l" + charecter_next_to_i
                modified_substring = modified_substring.replace(charecter_to_be_replaced, charecter_next_to_i + "ि")
                position_of_i = modified_substring.search(/l/, position_of_i + 1) // search for i ahead of the current position.

            } // end of while-02 loop

            //**********************************************************************************
            // End of Code for Replacing four Special glyphs
            //**********************************************************************************

            // following loop to eliminate 'chhotee ee kee maatraa' on half-letters as a result of above transformation.

            var position_of_wrong_ee = modified_substring.indexOf("ि्")

            while (position_of_wrong_ee != -1)  //while-03
            {
                var consonent_next_to_wrong_ee = modified_substring.charAt(position_of_wrong_ee + 2)
                var charecter_to_be_replaced = "ि्" + consonent_next_to_wrong_ee
                modified_substring = modified_substring.replace(charecter_to_be_replaced, "्" + consonent_next_to_wrong_ee + "ि")
                position_of_wrong_ee = modified_substring.search(/ि्/, position_of_wrong_ee + 2) // search for 'wrong ee' ahead of the current position. 

            } // end of while-03 loop

            // following loop to eliminate 'chhotee ee kee maatraa' on half-letters as a result of above transformation.

            var position_of_wrong_ee = modified_substring.indexOf("िं्")

            while (position_of_wrong_ee != -1)  //while-03
            {
                var consonent_next_to_wrong_ee = modified_substring.charAt(position_of_wrong_ee + 3)
                var charecter_to_be_replaced = "िं्" + consonent_next_to_wrong_ee
                modified_substring = modified_substring.replace(charecter_to_be_replaced, "्" + consonent_next_to_wrong_ee + "िं")
                position_of_wrong_ee = modified_substring.search(/िं्/, position_of_wrong_ee + 3) // search for 'wrong ee' ahead of the current position. 

            } // end of while-03 loop


            // Eliminating reph "Ô" and putting 'half - r' at proper position for this.
            set_of_matras = "ा ि ी ु ू ृ े ै ो ौ ं : ँ ॅ"
            var position_of_R = modified_substring.indexOf("{")

            while (position_of_R > 0)  // while-04
            {
                probable_position_of_half_r = position_of_R - 1;
                var charecter_at_probable_position_of_half_r = modified_substring.charAt(probable_position_of_half_r)


                // trying to find non-maatra position left to current O (ie, half -r).

                while (set_of_matras.match(charecter_at_probable_position_of_half_r) != null)  // while-05
                {
                    probable_position_of_half_r = probable_position_of_half_r - 1;
                    charecter_at_probable_position_of_half_r = modified_substring.charAt(probable_position_of_half_r);

                } // end of while-05


                charecter_to_be_replaced = modified_substring.substr(probable_position_of_half_r, (position_of_R - probable_position_of_half_r));
                new_replacement_string = "र्" + charecter_to_be_replaced;
                charecter_to_be_replaced = charecter_to_be_replaced + "{";
                modified_substring = modified_substring.replace(charecter_to_be_replaced, new_replacement_string);
                position_of_R = modified_substring.indexOf("{");

            } // end of while-04
           
            // updated by dk raut--------
            var preValue = modified_substring.substr(0, modified_substring.length - 1);
            var lastValue = modified_substring.substr(modified_substring.length - 1);

            if (lastValue == '.') {
                var lastValueModify = '।'
                modified_substring = preValue + lastValueModify;
            }

            //------------------------------

            modified_substring = modified_substring.replace(/=/g, ".");
            modified_substring = modified_substring.replace(/_/g, ")");
            modified_substring = modified_substring.replace(/Ö/g, "=");
            modified_substring = modified_substring.replace(/Ù/g, ";");
            modified_substring = modified_substring.replace(/…/g, "‘");
            modified_substring = modified_substring.replace(/Ú/g, "’");
            modified_substring = modified_substring.replace(/Û/g, "!");
            modified_substring = modified_substring.replace(/Ü/g, "%");
            modified_substring = modified_substring.replace(/æ/g, "“");
            modified_substring = modified_substring.replace(/Æ/g, "”");
            modified_substring = modified_substring.replace(/±/g, "+");
            modified_substring = modified_substring.replace(/-/g, "(");
            modified_substring = modified_substring.replace(/</g, "?");

        } // end of IF  statement  meant to  supress processing of  blank  string.

    } // end of the function  Replace_Symbols

}




//Allows only unicode text to be entered in the textbox
function convert_to_unicode_with_value(item) {
    if (item == null || item == undefined) {
        return;
    }
    var array_one = new Array(
        "ç", "˜", "m", "'m", "]m", "Fmf", "Fm",

        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",

        "k|m", "em", "km", "Qm", "qm", "N˜",
        "¡", "¢", "!", "@", "$", ">", "?", "B", "I", "Q", "ß",
        "q", "„", "<", "•", ">", "§", "°", "¶", "¿", "Å",
        "Ë", "Ì", "Í", "Î", "Ý", "å",
        "6«", "7«", "8«", "9«",

        "Ø", "|",

        "8Þ", "9Þ",

        "S", "s", "V", "v", "U", "u", "£", "#", "ª",
        "R", "r", "%", "H", "h", "‰", "´", "~", "`",

        "^", "&", "*", "(", ")",
        "T", "t", "Y", "y", "b", "W", "w", "G", "g",

        "K", "k", "ˆ", "A", "a", "E", "e", "D", "d",
        "o", "/", "N", "n", "J", "j", "Z", "z", "i", ":", ";", "X", "x",

        "cf‘", "c‘f", "cf}", "cf]", "cf", "c", "O{", "O", "pm", "p", "C", "P]", "P",

        "f‘", "\"", "'", "+", "f", "[", "\\", "]", "}", "F", "L", "M",

        "्ा", "्ो", "्ौ", "अो", "अा", "आै", "आे", "ाो", "ाॅ", "ाे",
        "ंु", "ेे", "अै", "ाे", "अे", "ंा", "अॅ", "ाै", "ैा", "ंृ",
        "ँा", "ँू", "ेा", "ंे")
    var array_two = new Array(


        "ॐ", "ऽ", "फ", "m'", "m]", "mfF", "mF",

        "०", "१", "२", "३", "४", "५", "६", "७", "८", "९",

        "फ्र", "झ", "फ", "क्त", "क्र", "ल",
        "ज्ञ्", "द्घ", "ज्ञ", "द्द", "द्ध", "श्र", "रु", "द्य", "क्ष्", "त्त", "द्म",
        "त्र", "ध्र", "ङ्घ", "ड्ड", "द्र", "ट्ट", "ड्ढ", "ठ्ठ", "रू", "हृ",
        "ङ्ग", "त्र", "ङ्क", "ङ्ख", "ट्ठ", "द्व",
        "ट्र", "ठ्र", "ड्र", "ढ्र",

        "्य", "्र",

        "ड़", "ढ़",

        "क्", "क", "ख्", "ख", "ग्", "ग", "घ्", "घ", "ङ",
        "च्", "च", "छ", "ज्", "ज", "झ्", "झ", "ङ", "ञ्",

        "ट", "ठ", "ड", "ढ", "ण्",
        "त्", "त", "थ्", "थ", "द", "ध्", "ध", "न्", "न",

        "प्", "प", "फ्", "ब्", "ब", "भ्", "भ", "म्", "म",
        "य", "र", "ल्", "ल", "व्", "व", "श्", "श", "ष्", "स्", "स", "ह्", "ह",

        "ऑ", "ऑ", "औ", "ओ", "आ", "अ", "ई", "इ", "ऊ", "उ", "ऋ", "ऐ", "ए",

        "ॉ", "ू", "ु", "ं", "ा", "ृ", "्", "े", "ै", "ँ", "ी", "ः",

        "", "े", "ै", "ओ", "आ", "औ", "ओ", "ो", "ॉ", "ो",
        "ुं", "े", "अ‍ै", "ो", "अ‍े", "ां", "अ‍ॅ", "ौ", "ौ", "ृं",
        "ाँ", "ूँ", "ो", "ें")
    var array_one_length = array_one.length;

    var modified_substring = item;

    //****************************************************************************************
    //  Break the long text into small bunches of max. max_text_size  characters each.
    //****************************************************************************************
    var text_size = item.length;

    var processed_text = '';  //blank

    //**********************************************
    //    alert("text size = "+text_size);
    //**********************************************

    var sthiti1 = 0; var sthiti2 = 0; var chale_chalo = 1;

    var max_text_size = 6000;

    while (chale_chalo == 1) {
        sthiti1 = sthiti2;

        if (sthiti2 < (text_size - max_text_size)) {
            sthiti2 += max_text_size;
            while (item.charAt(sthiti2) != ' ') { sthiti2--; }
        }
        else { sthiti2 = text_size; chale_chalo = 0 }

        //   alert(" sthiti 1 = "+sthiti1); alert(" sthit 2 = "+sthiti2) 

        var modified_substring = item.substring(sthiti1, sthiti2);

        Replace_Symbols();

        processed_text += modified_substring;


        //****************************************************************************************
        //  Breaking part code over
        //****************************************************************************************
        //  processed_text = processed_text.replace( /mangal/g , "SUCHI-DEV-708 " ) ;   

        //document.getElementById("unicode_text").value = processed_text  ;
        item = processed_text;
    }

    var remaining_text = item;
    var processed_text = "";  //blank initially

    var position_of_current_opening_bracket = 0;
    var position_of_next_closing_bracket = 1;

    var idx = remaining_text.indexOf("<p ")  // search starting from paragraphs. then search for sanskrit 99.
    idx = remaining_text.indexOf("Sanskrit 99", idx)
    var idx2 = 0  //  position_of_current_opening_bracket  ">"
    var idx3 = 0  //  position of "/span"
    var idx4 = 0  //  postion of "span" only , ie span without "/"

    while (idx != -1)    // while-01 loop     ;  while "Sanskrit 99"  is found..
    {
        idx2 = remaining_text.indexOf(">", idx)
        idx3 = remaining_text.indexOf("/span", idx2)
        idx4 = remaining_text.indexOf("span", idx2)


        while (idx4 < idx3)     // this loop to take care of  nested span.
        {
            idx4 = remaining_text.indexOf("span", idx3 + 4);
            idx3 = remaining_text.indexOf("/span", idx3 + 4);
        }


        var modified_substring = remaining_text.substring(idx2, idx3);
        modified_substring = modified_substring.replace(/>/g, ">>");
        processed_text = processed_text + remaining_text.substring(0, idx2) + modified_substring + "/span";

        remaining_text = remaining_text.substring(idx3 + 5);  //remaining_text excludes "/span"
        idx = remaining_text.indexOf("Sanskrit 99");

    } //end of outermost while-01


    processed_text = processed_text + remaining_text;


    // -----------------------------

    //  This section for taking care of paragraphs marked class = MsoBodyText  or class = MsoBodyText

    remaining_text = processed_text;
    processed_text = "";  //blank initially

    var position_of_start_of_paragraph = 0;
    var position_of_end_of_paragraph = 1;

    position_of_start_of_paragraph = remaining_text.indexOf("<p ");


    while (position_of_start_of_paragraph != -1)  //search for <p  in the remaining_text
    {

        position_of_start_of_paragraph = remaining_text.indexOf("<p ");
        position_of_end_of_paragraph = remaining_text.indexOf("/p>");

        modified_substring = remaining_text.substring(position_of_start_of_paragraph + 3, position_of_end_of_paragraph);


        if (modified_substring.indexOf("MsoBodyText") != -1) {
            modified_substring = modified_substring.replace(/>/g, ">>");    // repace all ">" with ">>" in this paragraph

            idx = modified_substring.indexOf("font-family");  // in Mybodytext, whereever font-family is found, it means it is roman text.
            idx2 = 0;  // position of ">>"
            idx3 = 0;  // position of "/span"
            idx4 = 0;  // position of "span" only without "/"

            while (idx != -1)     // again change ">>" to ">" only those which occure immediately after font-family:"Times new Roman"
            {

                idx2 = modified_substring.indexOf(">>", idx);
                idx3 = modified_substring.indexOf("/span", idx2);
                idx4 = modified_substring.indexOf("span", idx2);


                while (idx4 < idx3) {
                    idx4 = modified_substring.indexOf("span", idx3 + 4);
                    idx3 = modified_substring.indexOf("/span", idx3 + 4);
                }

                modified_substring = modified_substring.substring(0, idx2) + (modified_substring.substring(idx2, idx3)).replace(/>>/g, ">") + modified_substring.substring(idx3);
                idx = modified_substring.indexOf("font-family", idx3);

            } // end of while inner loop
        } // end of if statement

        processed_text = processed_text + remaining_text.substring(0, position_of_start_of_paragraph + 3) + modified_substring + "/p>";
        remaining_text = remaining_text.substring(position_of_end_of_paragraph + 3);
        position_of_start_of_paragraph = remaining_text.indexOf("<p ");

    } // end of outer while loop

    processed_text = processed_text + remaining_text;


    // ------------------------

    // Now do actual font conversion  of text occuring between  all the  pairs  >>  and  <

    remaining_text = processed_text; processed_text = "";

    idx2 = remaining_text.indexOf(">>");



    while (idx2 != -1)    // while-01 loop (checks if ">>" is still present
    {

        position_of_current_opening_bracket = remaining_text.indexOf(">>", idx2)
        position_of_next_closing_bracket = remaining_text.indexOf("<", position_of_current_opening_bracket)

        modified_substring = remaining_text.substring(position_of_current_opening_bracket + 2, position_of_next_closing_bracket);

        processed_text = processed_text + remaining_text.substring(0, position_of_current_opening_bracket + 1);   // ">" included by using +1 here
        remaining_text = remaining_text.substring(position_of_next_closing_bracket + 1);  //remaining_text excludes the closing bracket


        Replace_Symbols();   // call the subroutine and replace the legacy symbols  with corresponding Unicode.


        processed_text = processed_text + modified_substring + "<";

        idx2 = remaining_text.indexOf(">>");


    } //end of outermost while-01



    processed_text = processed_text + remaining_text;



    // do follwing conversions which are still left  or  were done incorrectly due to unavoidable reasons.

    // processed_text = processed_text.replace( /Sanskrit 99/g , "mangal" ) ;   

    // processed_text = processed_text.replace( /ृलतष/g , "ं" )   ;  
    // processed_text = processed_text.replace( /ृटुखतष/g , "घ" )  ;
    // processed_text = processed_text.replace( /ृामपष/g , "ृ" )  ;
    // processed_text = processed_text.replace( /ृगतष/g , ":" )  ;
    // processed_text = processed_text.replace( /ृनबसपष/g , "/&nbsp" )  ; 
    // processed_text = processed_text.replace( /ाॅ/g , "ॉ" )  ; 


    // now put the processed text in the output box finally.

    //document.getElementById("unicode_text").value = processed_text  
    item = processed_text

    return item
    //} // end of else loop for HTML case


    // --------------------------------------------------


    function Replace_Symbols() {

        //substitute array_two elements in place of corresponding array_one elements

        if (modified_substring != "")  // if stringto be converted is non-blank then no need of any processing.
        {
            for (input_symbol_idx = 0; input_symbol_idx < array_one_length; input_symbol_idx++) {
                idx = 0;  // index of the symbol being searched for replacement

                while (idx != -1) //while-00
                {

                    modified_substring = modified_substring.replace(array_one[input_symbol_idx], array_two[input_symbol_idx])
                    idx = modified_substring.indexOf(array_one[input_symbol_idx])

                } // end of while-00 loop
                // alert(" end of while loop")
            } // end of for loop
            // alert(" end of for loop")

            // alert(" modified substring2 = "+modified_substring)
            //*******************************************************
            var position_of_i = modified_substring.indexOf("l")

            while (position_of_i != -1)  //while-02
            {
                var charecter_next_to_i = modified_substring.charAt(position_of_i + 1)
                var charecter_to_be_replaced = "l" + charecter_next_to_i
                modified_substring = modified_substring.replace(charecter_to_be_replaced, charecter_next_to_i + "ि")
                position_of_i = modified_substring.search(/l/, position_of_i + 1) // search for i ahead of the current position.

            } // end of while-02 loop

            //**********************************************************************************
            // End of Code for Replacing four Special glyphs
            //**********************************************************************************

            // following loop to eliminate 'chhotee ee kee maatraa' on half-letters as a result of above transformation.

            var position_of_wrong_ee = modified_substring.indexOf("ि्")

            while (position_of_wrong_ee != -1)  //while-03
            {
                var consonent_next_to_wrong_ee = modified_substring.charAt(position_of_wrong_ee + 2)
                var charecter_to_be_replaced = "ि्" + consonent_next_to_wrong_ee
                modified_substring = modified_substring.replace(charecter_to_be_replaced, "्" + consonent_next_to_wrong_ee + "ि")
                position_of_wrong_ee = modified_substring.search(/ि्/, position_of_wrong_ee + 2) // search for 'wrong ee' ahead of the current position. 

            } // end of while-03 loop

            // following loop to eliminate 'chhotee ee kee maatraa' on half-letters as a result of above transformation.

            var position_of_wrong_ee = modified_substring.indexOf("िं्")

            while (position_of_wrong_ee != -1)  //while-03
            {
                var consonent_next_to_wrong_ee = modified_substring.charAt(position_of_wrong_ee + 3)
                var charecter_to_be_replaced = "िं्" + consonent_next_to_wrong_ee
                modified_substring = modified_substring.replace(charecter_to_be_replaced, "्" + consonent_next_to_wrong_ee + "िं")
                position_of_wrong_ee = modified_substring.search(/िं्/, position_of_wrong_ee + 3) // search for 'wrong ee' ahead of the current position. 

            } // end of while-03 loop


            // Eliminating reph "Ô" and putting 'half - r' at proper position for this.
            set_of_matras = "ा ि ी ु ू ृ े ै ो ौ ं : ँ ॅ"
            var position_of_R = modified_substring.indexOf("{")

            while (position_of_R > 0)  // while-04
            {
                probable_position_of_half_r = position_of_R - 1;
                var charecter_at_probable_position_of_half_r = modified_substring.charAt(probable_position_of_half_r)


                // trying to find non-maatra position left to current O (ie, half -r).

                while (set_of_matras.match(charecter_at_probable_position_of_half_r) != null)  // while-05
                {
                    probable_position_of_half_r = probable_position_of_half_r - 1;
                    charecter_at_probable_position_of_half_r = modified_substring.charAt(probable_position_of_half_r);

                } // end of while-05


                charecter_to_be_replaced = modified_substring.substr(probable_position_of_half_r, (position_of_R - probable_position_of_half_r));
                new_replacement_string = "र्" + charecter_to_be_replaced;
                charecter_to_be_replaced = charecter_to_be_replaced + "{";
                modified_substring = modified_substring.replace(charecter_to_be_replaced, new_replacement_string);
                position_of_R = modified_substring.indexOf("{");

            } // end of while-04

            // updated by dk raut--------
            var preValue = modified_substring.substr(0, modified_substring.length - 1);
            var lastValue = modified_substring.substr(modified_substring.length - 1);

            if (lastValue == '.') {
                var lastValueModify = '।'
                modified_substring = preValue + lastValueModify;
            }

            //------------------------------

            modified_substring = modified_substring.replace(/=/g, ".");
            modified_substring = modified_substring.replace(/_/g, ")");
            modified_substring = modified_substring.replace(/Ö/g, "=");
            modified_substring = modified_substring.replace(/Ù/g, ";");
            modified_substring = modified_substring.replace(/…/g, "‘");
            modified_substring = modified_substring.replace(/Ú/g, "’");
            modified_substring = modified_substring.replace(/Û/g, "!");
            modified_substring = modified_substring.replace(/Ü/g, "%");
            modified_substring = modified_substring.replace(/æ/g, "“");
            modified_substring = modified_substring.replace(/Æ/g, "”");
            modified_substring = modified_substring.replace(/±/g, "+");
            modified_substring = modified_substring.replace(/-/g, "(");
            modified_substring = modified_substring.replace(/</g, "?");

        } // end of IF  statement  meant to  supress processing of  blank  string.

    } // end of the function  Replace_Symbols

}

//Allows date to be entered in 1974.10.13 format
function ValidateDate(date) {
    var dateregx = /^\d{4}\.(0[1-9]|1[012])\.(0[1-9]|[12][0-9]|3[01])$/;    
    if(date.match(dateregx))
    {
        return true;
    } 
        else
    {
        return false;
    }
    
}
function ValidateEmail(email) {
    var regx = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/ ;
    return (regx.test(email))    
}
function DateCompare(firstDate, secondDate) {

    if (firstDate > secondDate) {
        return false;
    }
    else
        return true;
};

function formValidation(form) {
    if (!form.checkValidity()) {
        let invalid_input = form.querySelectorAll('input:invalid, select:invalid')[0];
        $(':required').blur();
        invalid_input.focus();
    }
    return (form.checkValidity());
}
