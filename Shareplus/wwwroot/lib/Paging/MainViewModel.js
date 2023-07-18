function mainViewModel() {
   
    self.PageSize = ko.observable(2);
    self.AllData = ko.observableArray([{ name: "yes 1" }, { name: "yes 2" }, { name: "yes 3" }, { name: "yes 4" }, { name: "yes 5" }, { name: "yes 6" }, { name: "yes 7" }, { name: "yes 8" }, { name: "yes 9" }, { name: "yes 10" }, { name: "yes 11" }]);
    self.AllData([])
    self.PagaData = ko.observableArray();

    

    self.Paging = ko.observable(new PagingVm({
        pageSize: self.PageSize(),
        totalCount: self.AllData().length,
    }));

    self.pageSizeSubscription = self.PageSize.subscribe(function (newPageSize) {
        self.Paging().Update({ PageSize: newPageSize, TotalCount: self.AllData().length, CurrentPage: self.Paging().CurrentPage() });
        self.RenderAgain();
    });

    self.currentPageSubscription = self.Paging().CurrentPage.subscribe(function (newCurrentPage) {
        self.RenderAgain();
    });

    self.RenderAgain = function () {
        var result = [];
        var startIndex = (self.Paging().CurrentPage() - 1) * self.Paging().PageSize();
        var endIndex = self.Paging().CurrentPage() * self.Paging().PageSize();

        for (var i = startIndex; i < endIndex; i++) {
            if (i < self.AllData().length)
                result.push(self.AllData()[i]);
        }
        self.PagaData(result);
    }

    self.dispose = function () {
        self.currentPageSubscription.dispose();
        self.pageSizeSubscription.dispose();
    }
}

$(function () {
    var vm = mainViewModel();
    ko.applyBindings(vm);
    vm.RenderAgain();
});