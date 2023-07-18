
function koPaginator(data) {
    var self = this;
    var pageSize = 50;
    var PageNo = 1;
    //let fnFilter = data.fnFilter || (x => true);
    if (!data.CallBack) {
        throw 'specify a callback function.';
        return;
    }

    self.PostUrl = ko.observable(data.PostUrl);
    //  self.DataFromServer = ko.observable(null);
    self.PageNo = ko.observable(1);
    self.GoToPageNo = ko.observable("");
    self.PageSize = ko.observable(pageSize);
    self.PageSize.subscribe(x => {
        var change = false;
        if (parseInt(pageSize) < parseInt(self.PageSize())) {

            change = parseInt(pageSize) < parseInt(self.TotalRecords()) ? true : false;
        } else {
            change = parseInt(self.PageSize()) < parseInt(self.TotalRecords()) ? true : false;
        }

        if (change) {
            if (self.PageSize() != pageSize) {
                pageSize = self.PageSize();
                self.PageNo(1);
                self.GetData();
            }
        }
    });
    self.TotalRecords = ko.observable(data.TotalRecords || 0);
    self.ShowFrom = ko.computed(function () {
        if (self.TotalRecords() > 0)
            return ((self.PageNo() - 1) * self.PageSize() + 1);
        else
            return 0;
    });
    self.ShowTo = ko.computed(function () {
        var showTo = self.PageNo() * self.PageSize();
        if (showTo < self.TotalRecords())
            return (self.PageNo() * self.PageSize());
        else
            return self.TotalRecords();
    });
    self.GoToPage = function () {
        if (!self.GoToPageNo()?.trim()) {
            self.GoToPageNo(null);
            return;
        }
        if (isNaN(self.GoToPageNo())) {
            self.GoToPageNo(null);
            jAlert("Please enter numeric value");
            return;
        }
        var gotoPage = parseInt(self.GoToPageNo());
        if (gotoPage == 0) {
            self.GoToPageNo(null);
            jAlert("Page number is start from 1 to ...");
            return;
        }
        if (gotoPage < 1) {
            gotoPage = gotoPage * (-1);
        }
        if (gotoPage - 1 >= (self.TotalRecords() / self.PageSize())) {
            jAlert("Page not found");
            self.GoToPageNo(null);
        }
        else {
            self.PageNo(gotoPage);
            self.GoToPageNo(gotoPage);
            self.GetData();
        }
    }
    self.PreviousPage = function () {
        var pageNo = ko.toJS(self.PageNo()) - 1;
        if (pageNo < 1) {
            jAlert("Page not found");
        }

        else {
            self.PageNo(pageNo);
            self.GetData();
        }

    }
    self.NextPage = function () {
        var pageNo = ko.toJS(self.PageNo()) + 1;
        if (self.TotalRecords() == self.ShowTo()) {
            jAlert("Page not found");
        }
        else {
            self.PageNo(pageNo);
            self.GetData();
        }

    }
    self.FetchData = function () {
        self.PageNo(1);
        self.GoToPageNo("");
        self.GetData();

    }
    self.GetData = function () {

        if (self.PostUrl()) {
            $.ajaxSetup({
                async: false, beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                }
            });
            /*$('#loader').show();*/
            var paginationData = {
                pageNo: self.PageNo(),
                pageSize: self.PageSize(),
            };
            /* Object.assign(paginationData);*/
            Object.assign(paginationData, ko.toJS(data.FilterParams));
            return $.post(data.PostUrl, paginationData,
                res => {
                    var result = JSON.parse(res);
                    self.TotalRecords(result.TotalRecords);
                    data.CallBack(result.ResponseData);
                    //  HEnabledDisabledButton();
                }).then(() => {

                    $('#loader').hide();
                });

        } else if (data.Records().length > 0) {
            console.log('rec', data.Records());
            //console.log('fnfil', data.fnFilter);
            let Records = data.Records();
            self.TotalRecords(Records.length);
            var from = (self.PageNo() - 1) * self.PageSize();
            var to = from + self.PageSize();
            if (to > self.TotalRecords()) {
                to = self.TotalRecords();
            }
            var result = [];
            for (var i = from; i < to; i++) {
                result.push(Records[i]);
            }
            data.CallBack(result);
        }
    }

    self.ClearControl = function () {
        let Records = [];
        self.TotalRecords(Records.length);
        var from = (self.PageNo() - 1) * self.PageSize();
        var to = from + self.PageSize();
        if (to > self.TotalRecords()) {
            to = self.TotalRecords();
        }
        var result = [];

        data.CallBack(result)

    }
}

ko.components.register('pagination', {
    viewModel: function (params) {
        var self = this;
        let addObj = ko.utils.unwrapObservable(params.value);
        self.GoToPageNo = addObj.GoToPageNo;
        self.GoToPage = addObj.GoToPage;
        self.ShowFrom = addObj.ShowFrom;
        self.ShowTo = addObj.ShowTo;
        self.TotalRecords = addObj.TotalRecords;
        self.PreviousPage = addObj.PreviousPage;
        self.NextPage = addObj.NextPage;
        self.PageNo = addObj.PageNo;
        self.PageSize = addObj.PageSize;
        /*self.Parameter = JSON.parse(localStorage.getItem('LocStorage')).selectedOption || "Param";*/
    },
    //afterBind: function (componentInfo) {
    //    console.log($(componentInfo.element));
    //},
    template:
        `<div class="row col-12 mt-5" data-bind='visible: TotalRecords() > 0' >
                            <div class="col-sm-12 col-md-4" style="margin-top: 7px;">
                                <div class="pagination pull-left" role="status" aria-live="polite">
                                    <span style="margin-right:5px">Showing</span> 
                                    <span style="margin-right:5px" data-bind="text: ShowFrom "></span>
                                    <span style="margin-right:5px">to</span> 
                                    <span style="margin-right:5px" data-bind="text: ShowTo "></span>
                                    <span style="margin-right:5px"> of</span> 
                                    <span style="margin-right:5px" data-bind="text: TotalRecords"></span>
                                    <span style="margin-right:5px">entries</span> 
                                </div>
                            </div>
                            <div class="col-sm-12 col-md-4">
                              <ul class="pagination pull-right">
                                    <li class="page-item" id="previousPage"><a class="page-link" data-bind="click:PreviousPage" href="#">Previous</a></li>
                                    <li class="page-item"><a class="page-link" data-bind="text:PageNo" href="#">1</a></li>
                                    <li class="page-item" id="nextPage"><a class="page-link" data-bind= "click:NextPage" href="#">Next</a></li>
                              </ul>
                           </div>

                                  <div class="col-md-2 ">
                                    <select name="example_length" aria-controls="example" data-bind="value:PageSize" class="input-sm form-control  pagination">
                                        <option value="10">10 / page</option>
                                        <option value="100">100 / page</option>
                                        <option value="200">200 / page</option>
                                        <option value="500">500 / page</option>
                                    </select>
                            </div>

                            <div class="col-sm-12 col-md-2">
                          <div class="input-group margin-top-20">
                                <input type="text" class="form-control" placeholder="" name="search" data-bind="value:GoToPageNo">
                                         <div class="input-group-btn ml-3">
                                             <button class="btn btn-primary" data-bind="click:GoToPage">Go To</button>
                                          </div>
                                         </div>
                         </div>
                            </div>`
    //,
    //afterRender: function (el) {
    //    console.log(el);
    //}
});