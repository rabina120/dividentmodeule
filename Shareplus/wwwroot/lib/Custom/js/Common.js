/* theme */
var gvKonamiPattern = ['ArrowUp', 'ArrowUp', 'ArrowDown', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'ArrowLeft', 'ArrowRight', 'b', 'a'];
var gvKonamiCurrent = 0;
var gvDarkMode = parseInt(localStorage.getItem('dark-mode')) || 0;

var alert = function (title, content) {
    let theme = 'info';
    if (title && title.toLowerCase().indexOf('warning') > -1) theme = 'warning';
    if (title && title.toLowerCase().indexOf('error') > -1) theme = 'error';
    if (title && title.toLowerCase().indexOf('success') > -1) theme = 'success';
    toastr[theme](content);
};

var jAlert = function (title, content, cb) {
    let theme = 'blue';
    if (title && title.toLowerCase().indexOf('warning') > -1) theme = 'red';
    if (title && title.toLowerCase().indexOf('error') > -1) theme = 'dark_red';
    if (title && title.toLowerCase().indexOf('success') > -1) theme = 'green';
    $.jAlert({
        'title': title,
        'content': content,
        'theme': theme,
        'showAnimation': 'fadeIn',
        'hideAnimation': 'fadeOut',
        'animationTimeout': 100,
        'btns': { 'text': 'close', 'theme': theme },
        'onClose': cb,
        "positionClass": "toast-top-center",
    });
};


function convertDate(inputFormat) {
    function pad(s) { return (s < 10) ? '0' + s : s; }
    var d = new Date(inputFormat)
    return [d.getFullYear(), pad(d.getMonth() + 1), pad(d.getDate())].join('-')
}

//for converting to yyyy-mm-dd
function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

//for adding days
addDays = function (days) {
    var date = new Date();
    date.setDate(date.getDate() + days);
    return date;
}

function convertToFixTwo(data) {
    return data.toFixed(4)
}

function postReq(url, data, constr, arr, options) {
    //debugger
    let fnClear = options?.fnClear || null,
        cb = options?.cb || null;
    redir = options?.redir || null;

    $.ajaxSetup({
        async: false, beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        }
    });
    return $.post(url, data, function (res, status, xhr) {
        res = JSON.parse(res);
        if (res.hasError) {
            alert("Warning!", res.Message);
        } else {
            if (!res.IsSuccess)
                alert("Warning!", res.Message);
            else {
                if (!redir) {
                    if (!arr) {
                        if (constr) {
                            constr(res.OutputParam);
                        }
                        alert("Success!", res.Message);
                    }
                    else {
                        if (res.Message) {
                            alert('Success!', res.Message);
                        }
                        arr(res.ResponseData);
                    }
                    try {
                        if (fnClear) fnClear();
                    } catch (e) {
                        console.error('fnClear must be a function.');
                    }
                    try {
                        if (cb) cb();
                    } catch (e) {
                        console.error('cb must be a function.');
                    }
                } else {
                    localStorage.setItem('initialMessage', res.Message);
                    window.location = redir;
                }
            }
        }

        //})        .done(() => {
        //$('#loader').hide();
    }).fail((xhr, status, message) => {
        jAlert(status, message);
        //$('#loader').hide();
    });
}

function postReqMsg(url, data, constr, arr, options) {



    let fnClear = options?.fnClear || null,
        cb = options?.cb || null;
    redir = options?.redir || null;

    $.ajaxSetup({
        async: false, beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        }
    });

    return $.post(url, data, function (res, status, xhr) {
        res = JSON.parse(res);
        if (res.hasError) {
            alert("Warning!", res.Message);
        } else {
            if (!res.IsSuccess)
                alert("Warning!", res.Message);
            else {
                if (!redir) {
                    if (!arr) {
                        if (constr) {
                            constr(res.OutputParam);
                        }
                        alert("Success!", res.Message);
                    }
                    else {
                        if (res.Message) {
                            alert('Success!', res.Message);
                        }
                        arr(res);
                    }
                    try {
                        if (fnClear) fnClear();
                    } catch (e) {
                        console.error('fnClear must be a function.');
                    }
                    try {
                        if (cb) cb();
                    } catch (e) {
                        console.error('cb must be a function.');
                    }
                } else {
                    localStorage.setItem('initialMessage', res.Message);
                    window.location = redir;
                }
            }
        }

        //})        .done(() => {
        //$('#loader').hide();
    }).fail((xhr, status, message) => {
        jAlert(status, message);
        //$('#loader').hide();
    });
}
function postReqMessage(url, data, constr, arr, options) {



    let fnClear = options?.fnClear || null,
        cb = options?.cb || null;
    redir = options?.redir || null;

    $.ajaxSetup({
        async: false, beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        }
    });

    return $.post(url, data, function (res, status, xhr) {
        res = JSON.parse(res);
        if (res.hasError) {
            alert("Warning!", res.Message);
        } else {
            if (!res.IsSuccess)
                alert("Warning!", res.Message);
            else {
                if (!redir) {
                    if (!arr) {
                        if (constr) {
                            constr(res.OutputParam);
                        }
                        alert("Success!", res.Message);
                    }
                    else {
                        if (res.Message) {
                            alert('Success!', res.Message);
                        }
                        arr(res);
                    }
                    try {
                        if (fnClear) fnClear();
                    } catch (e) {
                        console.error('fnClear must be a function.');
                    }
                    try {
                        if (cb) cb();
                    } catch (e) {
                        console.error('cb must be a function.');
                    }
                } else {
                    localStorage.setItem('initialMessage', res.Message);
                    window.location = redir;
                }
            }
        }

        //})        .done(() => {
        //$('#loader').hide();
    }).fail((xhr, status, message) => {
        jAlert(status, message);
        //$('#loader').hide();
    });
}

function getReq(url, data, constr, arr, options) {
    let fnClear = options?.fnClear || null,
        redir = options?.redir || null;
    $.ajax({
        dataType: 'json',
        async: false,
        url,
        data,
        contentType: 'application/json; charset= utf-8',
        success: function (result, status, xhr) {
            // console.log(xhr.status);
            //var res = JSON.parse(result);
            var res = result;
            if (!res.IsSuccess)
                alert("Warning!", res.Message);
            else {
                if (!redir) {
                    if (!arr && result.Message) {
                        alert("Success!", res.Message, redir);
                    } else if (constr) {
                        try {
                            arr(res.ResponseData.map(x => new constr(x)));
                        } catch (e) {
                            arr(new constr(res.ResponseData));
                        }
                    } else {
                        if (res.Message) {
                            alert(res.Message, 'Success!', redir);
                        }
                        arr(res.ResponseData);
                    }
                    if (fnClear) fnClear();
                } else {
                    localStorage.setItem('initialMessage', res.Message);
                    window.location = redir;
                }
            }

        }
    });
}



function toggleTheme() {
    document.body.classList.toggle('dark-mode');
    gvDarkMode = 1 - gvDarkMode;
    localStorage.setItem('dark-mode', gvDarkMode);
}

function popupFeedbackModal() {
    $('#ModalFeedback').modal('show');
}

function saveFeedback() {
    postReq('/Common/Dashboard/SaveFeedback', { feedback: document.getElementById('txtFeedback').value });
    $('#ModalFeedback').modal('hide');
}

function dispChangeLog(log) {
    console.log(log);
}

function checkVersion() {
    let currVersion;
    try {
        currVersion = document.getElementById('version-no').innerText;
    } catch (e) {
        return;
    }
    let lastVersion = localStorage.getItem('versionNo');
    console.log(lastVersion, currVersion);
    if (lastVersion != currVersion) {
        // localStorage.setItem('versionNo', currVersion);
        getReq('/Common/Dashboard/GetChangeLog', { version: lastVersion }, null, dispChangeLog);
    }
}

function showInitialToast() {
    let initialMessage = localStorage.getItem('initialMessage');
    if (initialMessage) {
        alert('success', initialMessage);
        localStorage.setItem('initialMessage', '');
    }
}

keyHandler = e => {
    if (gvKonamiPattern.indexOf(e.key) < 0 || event.key != gvKonamiPattern[gvKonamiCurrent]) {
        gvKonamiCurrent = 0;
        return;
    }
    gvKonamiCurrent++;
    if (gvKonamiPattern.length === gvKonamiCurrent) {
        gvKonamiCurrent = 0;
        toggleTheme();
    }
};
 function getConnectedCompany() {
    //var isConnected = false;
    $.ajax({
        type: "post",
        url: '/Common/Company/GetConnectedCompany',

        datatype: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (result) {
            return result.IsSuccess;
        },
        error: function () {
            return false;
        },
        //complete: (result) => {
        //    return result.isSuccess;
        //}
    })
}

function getMenu() {
    Openloader();
    $.ajax({
        url: '/Common/Menu',
        success: function (result) {
            if (result.isSuccess) {
                var html = "";
                i = 0;
                //result.ResponseData = JSON.parse(result)
                var anchorDisabled = "anchorEnabled";
               
               
                //if (!result.isValid) {
                //    anchorDisabled = "anchorDisabled"
                //} else {
                //    anchorDisabled = "anchorEnabled"
                //}
                if (result.isToken) {
                    $('.dropdown-item.password').hide();
                }
                else {
                    $('.dropdown-item.password').show();
                }
                $.each(result.responseData, function (index, item) {


                    if (item.level == 1) {
                        html += '<hr class="sidebar-divider">' +
                            '<li class="nav-item">' +
                            '<a class="nav-link collapsed ' + anchorDisabled+'" data-toggle="collapse" data-target="#' + item.collapseTarget + '" aria-expanded="true" aria-controls="collapseBootstrap"> ' +
                            '<i class="' + item.fontAwesome + '"></i>'
                            + '<span>' + item.menuText + '</span>'
                            + ' </a><div id="' + item.collapseTarget + '" class="collapse" aria-labelledby="headingBootstrap" data-parent="#accordionSidebar" style=""><div class="bg-white py-2 collapse-inner rounded">'

                    }

                    if (item.hasChild = true) {
                        var submenu = result.responseData.filter(x => x.parentId == item.menuId);
                        $.each(submenu, function (index1, item1) {

                            html += '<a class="collapse-item"  href="' + item1.mUrl + '"><i class="' + item1.fontAwesome + '"></i>  <span>' + item1.menuText + '</span></a>'

                        });
                        html += '</div></div > '
                        html += '</li>'

                    }

                });
                $(".menuList").html(html);
            } else {
                toastr.error(result.message)
            }
        },
        error: function (error) {
            toastr.error(error)
        },
        complete: () => {
            Closeloader();
        }
    });

}



//used for big files but not working 
function postReqFile(url, data, constr, arr, options) {



    let fnClear = options?.fnClear || null,
        cb = options?.cb || null;
    redir = options?.redir || null;

    $.ajaxSetup({
        contentType: false,
        processData: false,
        beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            }
    });

    return $.post(url, data, function (res, status, xhr) {
        console.log(res,'response');
        //res = JSON.parse(res);
        if (res.hasError) {
            alert("Warning!", res.Message);
        } else {
            if (!res.isSuccess)
                alert("Warning!", "Some Issue Occured");
            else {
                arr(res);
                if (!redir) {
                    if (!arr) {
                        if (constr) {
                            constr(res.OutputParam);
                        }
                        alert("Success!", res.Message);
                    }
                    else {
                        if (res.Message) {
                            alert('Success!', res.Message);
                        }
                        
                    }
                    try {
                        if (fnClear) fnClear();
                    } catch (e) {
                        console.error('fnClear must be a function.');
                    }
                    try {
                        if (cb) cb();
                    } catch (e) {
                        console.error('cb must be a function.');
                    }
                } else {
                    localStorage.setItem('initialMessage', res.Message);
                    window.location = redir;
                }
            }
        }

        //})        .done(() => {
        Closeloader();
        //$('#loader').hide();
    }).fail((xhr, status, message) => {
        jAlert(status, message);
        Closeloader();
        //$('#loader').hide();
    });
}



function isNumberKey(evt) {

    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    } else {
        return true;
    }
};
function isCopy(evt) {
    if (event.ctrlKey == true && (event.which == '118' || event.which == '86')) {
        return false;
        event.preventDefault();
    }
    else {
        return true;
    }
};

var LogoForPDF = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAKwAAABOCAYAAAC9vf1RAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyNpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMTQ4IDc5LjE2NDAzNiwgMjAxOS8wOC8xMy0wMTowNjo1NyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIDIxLjAgKFdpbmRvd3MpIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOkU4NzJDMEE4RTdBRDExRUNCOTFFRjczRkY0RTUyOTFDIiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOkU4NzJDMEE5RTdBRDExRUNCOTFFRjczRkY0RTUyOTFDIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6RTg3MkMwQTZFN0FEMTFFQ0I5MUVGNzNGRjRFNTI5MUMiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6RTg3MkMwQTdFN0FEMTFFQ0I5MUVGNzNGRjRFNTI5MUMiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz7PHTVZAAAUcklEQVR42uxdB3wU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";

$(document).ready(() => {


    showInitialToast();
    $('.fa-caret-down').click(function (event) {
        var stat = event.target.parentElement.nextElementSibling.style.display == 'block';
        if (stat) {
            event.target.parentElement.nextElementSibling.style.display = 'none';
        }
        else {
            event.target.parentElement.nextElementSibling.style.display = 'block';
        }
    });
    

});
