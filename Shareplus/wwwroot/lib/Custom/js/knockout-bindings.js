var _ko = {};
_ko.mapTask = function (from, to) {
    for (key in to()) {
        if (ko.isObservable(to()[key]))
            try {
                to()[key](from()[key]());
            } catch (e) {
                if (!e.message.includes('computed'))
                    throw e;
            }
    }
};

_ko.toJS = function (vm) {
    return JSON.parse(ko.toJSON(vm));
};

_ko.serialize = function (vm) {
    let obj = {};
    for (key in vm) {
        if (ko.isObservable(vm[key]))
            obj[key] = ko.utils.unwrapObservable(vm[key]);
    }
    return ko.toJS(obj);
};

_ko.genBuilder = function (arr, obs, constr, modal = null, clearVal = null) {
    let builder = {}, form = null;
    builder.editing = ko.observable(false);
    builder.current = ko.observable();
    builder.showModal = function () {
        if (modal) {
            $('#' + modal).modal('show');
        }
    }

    builder.add = function () {
        form = document.getElementById(modal).querySelector('form');
        let validation = form ? formValidation(form) : obs().Validate();
        if (validation) {
            if (!builder.editing()) {
                arr.push(obs());
            } else {
                _ko.mapTask(obs, builder.current);
            }
            if (modal) {
                $('#' + modal).modal('hide');
            }
            builder.clear();
        }
        else if (!form) {
            jAlert('Please enter all the required information denoted by (*)!!');
        }
    };

    builder.edit = function (data) {
        builder.current(data);
        obs(new constr(ko.toJS(data)));
        obs().Action(data.Action() == 'A' ? 'A' : 'E');
        builder.editing(true);
        if (modal) {
            $('#' + modal).modal('show');
        }
    };

    builder.delete = function (data) {
        if (data.Action() == 'A')
            arr.remove(data);
        else
            data.Action('D');
    };

    builder.clear = function () {
        obs(new constr(clearVal || { Action: 'A' }));
        builder.current(null);
        builder.editing(false);
    };

    return builder;
};

ko.subscribable.fn.trimmed = function () {
    return ko.computed({
        read: function () {
            return (ko.utils.unwrapObservable(this) || '').trim();
        },
        write: function (value) {
            this((value || '').trim());
            this.valueHasMutated();
        },
        owner: this
    }).extend({ rateLimit: 100, notify: 'always' });
};

ko.subscribable.fn.date = function () {
    return ko.computed({
        read: function () {
            return (ko.utils.unwrapObservable(this) || '').substring(0, 10);
        },
        write: function (value) {
            this((value || '').substring(0, 10));
            this.valueHasMutated();
        },
        owner: this
    }).extend({ rateLimit: 100, notify: 'always' });
};

ko.bindingHandlers.disableInputs = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        element.querySelectorAll('input, textarea, select, button').forEach(x => {
            x.disabled = ko.utils.unwrapObservable(valueAccessor());
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        element.querySelectorAll('input, textarea, select, button').forEach(x => {
            x.disabled = ko.utils.unwrapObservable(valueAccessor());
        });
    }
};

ko.bindingHandlers.barChart = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var ct = element.getContext('2d');
        CreateBarGraph(ct, ko.utils.unwrapObservable(valueAccessor()), "Total Count",
            ko.utils.unwrapObservable(allBindings.get("chartTitle")),
            ko.utils.unwrapObservable(allBindings.get("onClick")));
    },
    //update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
    //    var ct = element.getContext('2d');
    //    CreateBarGraph(ct, ko.utils.unwrapObservable(valueAccessor()), "Total Count",
    //        ko.utils.unwrapObservable(allBindings.get("chartTitle")),
    //        ko.utils.unwrapObservable(allBindings.get("onClick")));
    //}
}
ko.bindingHandlers.stackedBarChart = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var ct = element.getContext('2d');
        CreateStackedBarGraph(ct, ko.utils.unwrapObservable(valueAccessor()), "Total Count",
            ko.utils.unwrapObservable(allBindings.get("chartTitle")),
            ko.utils.unwrapObservable(allBindings.get("onClick")));
    },
    //update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
    //    var ct = element.getContext('2d');
    //    CreateBarGraph(ct, ko.utils.unwrapObservable(valueAccessor()), "Total Count",
    //        ko.utils.unwrapObservable(allBindings.get("chartTitle")),
    //        ko.utils.unwrapObservable(allBindings.get("onClick")));
    //}
}
ko.bindingHandlers.pieChart = {
    init: function (element, valueAccessor, allBindings) {
        var ct = element.getContext('2d');
        CreatePieChart(ct, ko.utils.unwrapObservable(valueAccessor()),
            ko.utils.unwrapObservable(allBindings.get("chartTitle")),
            ko.utils.unwrapObservable(allBindings.get("onClick")));
    }
    //update: function (element, valueAccessor, allBindings) {
    //    var ct = element.getContext('2d');
    //    CreatePieChart(ct, ko.utils.unwrapObservable(valueAccessor()),
    //        ko.utils.unwrapObservable(allBindings.get("chartTitle")));
    //}
}

ko.bindingHandlers.doughnutChart = {
    init: function (element, valueAccessor, allBindings) {
        var ct = element.getContext('2d');
        CreateDoughnutChart(ct, ko.utils.unwrapObservable(valueAccessor()),
            ko.utils.unwrapObservable(allBindings.get("chartTitle")),
            ko.utils.unwrapObservable(allBindings.get("onClick")));
    }
}
ko.bindingHandlers.fileUpload = {
    init: function (element, valueAccessor) {
        $(element).change(function () {
            element.files[0] = [];
            var e = element;
            if (e.files[0]) {
                var files = e.files ? e.files : [];
                if (!files.length || !window.FileReader) return;
                if (!e.files[0].type.match('image.*') && e.files[0].type != 'application/pdf') {
                    jAlert('The uploaded file must be an image or a pdf!', 'Error!!!');
                    return;
                }

                var reader = new FileReader();
                reader.readAsDataURL(files[0]);
                reader.onloadend = function (event) {
                    var dataUri = event.target.result;
                    imgfile = dataUri;
                    valueAccessor()({
                        FileName: e.files[0].name,
                        File: imgfile,
                        // File: e.files[0],
                    });
                };
            }
            // valueAccessor()(element.files[0]);
        });
    },
    update: function (element, valueAccessor) {
        if (ko.unwrap(valueAccessor()) === null) {
            $(element).wrap('<form>').closest('form').get(0).reset();
            $(element).unwrap();
        }
    }
};

ko.components.register('file-upload', {
    viewModel: function (params) {
        var self = this;
        self.File = params.value;
        self.Title = params.title;
        self.DocPreview = function (abc) {
            let wdth = screen.width / 2, hght = screen.height / 2;
            let pdfWindow = window.open('', '_blank', 'width=' + wdth + ', height=' + hght + ',scrollbars=0');
            pdfWindow.document.write('<iframe width=\'100%\' height=\'100%\' src=\'' + encodeURI(abc.File().File) + '\'></iframe>');
        };

    },
    template:
        `
            <div class="form-group">
                <label data-bind="text: Title"></label>
                <label class="file-upload" title="upload">
                    <input style="display: none;" type="file" data-bind="fileUpload: File, attr: {class: 'reqDoc'}" />
                    <i class="fa fa-cloud-upload-alt fa-3x"></i>
                </label>
            </div>
            <a class="preview" data-bind="click: DocPreview" title="click to preview">
                <img id="docImg" alt="" upImage" data-bind="attr: { src: File().File }"
                        onerror="if(this.src != 'error.jpg') this.src = '/lib/custom/images/pdf.png';"
                        class="DocPreview" data-content="Preview Uploaded File!!!" />
            </a>
        `
});

ko.components.register('file-uploads', {
    viewModel: function (params) {
        var self = this;
        self.File = params.value;
        self.Title = params.title;
        self.DocPreview = function (abc) {
            let wdth = screen.width / 2, hght = screen.height / 2;
            let pdfWindow = window.open('', '_blank', 'width=' + wdth + ', height=' + hght + ',scrollbars=0');
            pdfWindow.document.write('<iframe width=\'100%\' height=\'100%\' src=\'' + encodeURI(abc.File().File) + '\'></iframe>');
        };

    },
    template:
        `
            <div>
                  <label class = "filetitle" data-bind="text: Title">title</label>
                  <label class="file-upload" title="upload">
                    <input style="display: none;" type="file" data-bind="fileUpload: File, attr: {class: 'reqDoc'}" />
                    <i class="fa fa-cloud-upload-alt fa-3x"></i>
                  </label>
                  <a class="preview" data-bind="click:DocPreview" title="click to preview">
                    <img id="docImg" alt="" upImage" data-bind="attr: { src: File().File || File().FilePath}"onerror="if(this.src != 'error.jpg') 
                        this.src = '/lib/custom/images/pdf.png';"class="DocPreview" data-content="Preview Uploaded File!!!" />
                  </a>
           </div>
        `
});

ko.bindingHandlers.select2 = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        // This will be called once when the binding is first applied to an element,
        // and again whenever any observables/computeds that are accessed change
        // Update the DOM element based on the supplied values here.
    }
};


ko.bindingHandlers.executeOnEnter = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
};