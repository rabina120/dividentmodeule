function formValidation(form) {
    if (!form.checkValidity()) {
        let invalid_input = form.querySelectorAll('input:invalid, select:invalid, textarea:invalid')[0];
        $(form.querySelectorAll(':required')).blur();
        invalid_input.focus();
    }
    return (form.checkValidity());
}

function CheckValidation(el, options = null) {
    let valid = '', errmsg = '', sxsmsg = '', msg = '';
    if (options) {
        valid = options.valid;
        errmsg = options.errmsg;
        sxsmsg = options.sxsmsg;
    } else {
        if (el.classList.contains('date-picker')) {
            valid = !el.required || el.value;
            errmsg = valid ? '' : 'Please select the date';
        } else {
            el.value = el.value.trim();
            valid = el.checkValidity();
            errmsg = el.validationMessage;
        }
    }
    if (valid) {
        el.classList.remove('validation-err');
        if (el.classList.contains('selectpicker'))
            el.parentElement.classList.remove('validation-err');
        msg = sxsmsg;
        if (sxsmsg) el.classList.add('validation-sxs');
    } else {
        el.classList.add('validation-err');
        el.classList.remove('validation-sxs');
        msg = errmsg;
    }
    let valEl = el.classList.contains('selectpicker') ? el.parentElement.nextElementSibling : el.nextElementSibling;
    valEl.hidden = valid && !sxsmsg;
    if (valEl.hidden)
        el.parentElement.classList.remove('form-val-group');
    else
        el.parentElement.classList.add('form-val-group');
    valEl.innerText = msg;
    return valid;
}

function ValidationBlurEvenHandler(e) {
    if (!e.classList.contains('date-picker') && e.type != 'file' && e.type != 'checkbox') {
        if (e.classList.contains("selectpicker") && !e.parentElement.nextElementSibling?.classList.contains("validation-msg")) {
            $('<label class="validation-msg" hidden></label>').insertAfter(e.parentElement);
        }
        else if (!e.classList.contains("selectpicker") && (!e.nextElementSibling || !e.nextElementSibling.classList.contains("validation-msg"))) {
            $('<label class="validation-msg" hidden></label>').insertAfter($(e));
        }
        CheckValidation(e);
    }
}

function addValidationEventHandler() {
    $('input, select, textarea').blur(e => ValidationBlurEvenHandler(e.target));
    $('input.date-picker').on('dateSelect', (e, x, y) => {
        if (!e.target.nextElementSibling || !e.target.nextElementSibling.classList.contains("validation-msg"))
            $('<label class="validation-msg" hidden></label>').insertAfter($(e.target));
        CheckValidation(e.target);
    });
}