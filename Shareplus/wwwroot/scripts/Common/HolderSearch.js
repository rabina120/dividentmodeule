function HolderSearch(data)  {
    self.HolderText=data.shholderno+' '+data.fname+' '+data.lname
    self.HolderNo = data.shholderno;
}

var HolderSearch = function (){
    self.HolderSearch = ko.observableArray([]);
    self.HolderNo = ko.observable();
    self.HolderText = ko.observable();
    self.SelectedHolder = ko.observable();

}

$(document).ready(function () {
    ko.applyBindings(new HolderSearch());
});