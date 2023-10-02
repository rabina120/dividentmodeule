function User(data) {
    var self = this;
    if (data != undefined) {
        self.UserId = ko.observable(data.UserId);
        self.UserName = ko.observable(data.UserName);
    }
}

function Menu(data) {
    var self = this;
    if (data != undefined) {
        self.menuId = ko.observable(data.menuId);
        self.menuText = ko.observable(data.menuText);
        self.menuRole = ko.observable(data.menuRole);
        self.parentId = ko.observable(data.parentId);
    }
}
function Role(data) {
    var self = this;
    if (data != undefined) {
        self.RoleId = ko.observable(data.roleId);
        self.RoleName = ko.observable(data.roleName);
        self.IsActive = ko.observable(data.isActive);
    }
}

var UserRightsViewModel = function () {
   
   

    //Disabling Child Menu List on Load
    $('.menu-child').css('display', 'none');


    //role
    self.SelectedRole = ko.observable();
    self.RoleList = ko.observableArray([])

    //Users 
    self.UserId = ko.observable();
    self.UserName = ko.observable();
    self.UserList = ko.observableArray([]);
    self.SelectedUser = ko.observable();

    //Parent Menu List
    ParentMenuList = ko.observableArray([]);
    //Child Menu List
    self.ChildMenuList = ko.observableArray([]);

    self.ChildMenuListForChecker = ko.observableArray([]);
    self.ChildMenuListForMaker = ko.observableArray([]);

    //Menu
    SelectedParentMenu = ko.observableArray([]);
    SelectedChildrenMenu = ko.observableArray([]);
    self.menuText = ko.observable()
    self.MenuList = ko.observableArray()
    self.menuRole = ko.observable()
    //User Menu List
    self.UserMenuList = ko.observableArray([]);
    self.SelectedChildrenMenuToAdd = ko.observableArray([]);

    //Add Update Button
    self.ButtonValue = ko.observable();

    //varaiable for sub menu on parent click 
    var submenu;
    var selectedChildren;

    //Validation for user rights
    self.Validation = function () {
        errMsg = "";
        if (Validate.empty(self.SelectedRole())) {
            errMsg += "Please Select A Role. <br/>";
        }
        if (Validate.empty(self.SelectedParentMenu()) || Validate.empty(self.SelectedChildrenMenu())) {
            errMsg += "Please Select Menu Rights. <br/>";
        }
        if (errMsg !== "") {

            toastr.error(errMsg);
            return false;
        }
        else {
            return true;
        }
    }

    //All Menu List from database
    self.GetMenuList = function () {
        $.ajax({
            type: "post",
            url: '/Common/Menu/GetMenuList',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.hasError) {
                    alert('error',result.message)
                } else {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Menu(item);
                        });
                        self.MenuList(mappedTasks);
                        //console.log(result.responseData.filter(x => {
                        //    return (x.level == 1 && x.menuRole != 'A')
                        //}));
                        var mappedTasks = $.map(result.responseData, function (item) {
                            if (item.level == 1 && item.menuRole != 'A') {
                                return new Menu(item);
                            }

                        });
                        self.ParentMenuList(mappedTasks);

                        submenu = '';
                        submenu = $.map(ko.toJS(self.SelectedParentMenu()), function (item) {
                            return ko.toJS(self.MenuList()).filter(x => x.parentId == item);
                        });
                        self.ChildMenuListForChecker(submenu);
                        self.ChildMenuListForMaker(submenu);
                    }
                }
                
            },
            error: function (error) {
                alert("error", error)
            }
        });

    }
    self.GetMenuList()
    //Menu that user has access to
    self.GetUserList = function () {
        $.ajax({
            type: "post",
            url: '/Security/UserDetails/GetAllUsers',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                result = JSON.parse(result);
                        var mappedTasks = $.map(result.data, function (item) {
                            return new User(item);
                        });
                        self.UserList(mappedTasks);
                    
                
               
            },
            error: function (error) {
                alert("error", error)
            }
        });
    }
    self.GetUserList();


    self.GetRoleList = function () {
        $.ajax({
            type: "post",
            url: '/Security/UserDetails/GetAllRoles',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {

                if (result.hasError) {
                    alert('error', result.message)
                } else {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Role(item);
                        });
                        self.RoleList(mappedTasks);
                    } else {
                        alert('warning', result.message)
                    }
                }
                
            },
            error: function (error) {
                console.log("error", error)
            }
        });
    }
    self.GetRoleList();



    self.SelectedRole.subscribe(function () {

        SelectedParentMenu([]);
        SelectedChildrenMenu([]);
        selectedChildren = '';

        $.ajax({
            type: "post",
            url: '/Common/Menu/GetMenuByRole',
            data: { 'RoleID': self.SelectedRole() },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.hasError) {
                    alert('error', result.message)
                } else {
                    if (result.isSuccess) {
                        if (result.responseData.length > 0) {
                            result.responseData = result.responseData.filter(x => x.menuRole != "A")
                            var selectedParentMenuId = $.map(ko.toJS(result.responseData), function (item) {
                                if (item.level == 1) {
                                    SelectedParentMenu.push(item.menuId)
                                    submenu = '';
                                    selectedChildren = $.map(ko.toJS(result.responseData), function (item) {
                                        return ko.toJS(self.MenuList()).filter(x => x.menuId == item.menuId);
                                    });

                                    submenu = $.map(ko.toJS(self.SelectedParentMenu()), function (item) {
                                        return ko.toJS(self.MenuList()).filter(x => x.parentId == item).map(function (item) {
                                            return item.menuId;
                                        })
                                    });

                                    if (selectedChildren.length > 0) {
                                        var mappedTasks = $.map(submenu, function (item) {
                                            selectcheck = $.map(selectedChildren, function (item1) {
                                                if (item1.menuId == item) {
                                                    return item
                                                }
                                            });
                                            return selectcheck
                                        });
                                    } else {
                                        self.SelectedChildrenMenu([])
                                        selectedChildren = ''
                                    }

                                    self.SelectedChildrenMenu(mappedTasks);
                                    self.SelectedChildrenMenuToAdd(mappedTasks);
                                    //Enabling Child Menu List on Load
                                    $('.menu-child').css('display', 'block');
                                }
                            });
                        } else {
                            SelectedParentMenu([]);
                            SelectedChildrenMenu([]);
                            selectedChildren = '';
                        }
                    }
                }
                
            },
            error: function (error) {
                alert("failed", error)
            }
        });
    });

    
    self.ShowPDFReport = () => {
        self.ShowReport('pdf')
    }
    self.ShowExcelReport = () => {
        self.ShowReport('excel')
    }
    self.ShowReport = (type) => {
        $.ajax({
            url: '/Security/UserRight/RoleReport', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            }, data: { 'RoleID': self.SelectedRole(), 'actionType': type },
            type: 'post',
            success: function (result) {
                if (result.hasError) {
                    alert('error', result.message)
                } else {
                    if (result.isSuccess) {
                        var fileName = result.message;
                        var a = document.createElement("a");
                        a.href = "data:application/octet-stream;base64," + result.responseData;
                        a.download = fileName;
                        a.click();
                    }
                    else {
                        alert('error', result.message);
                    }


                }


            },
            error: function (error) {

            }
        });
    }

    self.SelectedUser.subscribe(function () {
        SelectedParentMenu([]);
        SelectedChildrenMenu([]);
        selectedChildren = '';

        $.ajax({
            type: "post", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            url: '/Common/Menu/GetMenuByUser',
            data: { 'UserId': self.SelectedUser() },
            datatype: "json",
            success: function (result) {
                if (result.hasError) {
                    alert('error', result.message)
                } else {
                    if (result.isSuccess) {
                        if (result.responseData.length > 0) {
                            var selectedParentMenuId = $.map(ko.toJS(result.responseData), function (item) {
                                if (item.level == 1) {
                                    SelectedParentMenu.push(item.menuId)
                                    submenu = '';
                                    selectedChildren = $.map(ko.toJS(result.responseData), function (item) {
                                        return ko.toJS(self.MenuList()).filter(x => x.menuId == item.menuId);
                                    });

                                    submenu = $.map(ko.toJS(self.SelectedParentMenu()), function (item) {
                                        return ko.toJS(self.MenuList()).filter(x => x.parentId == item).map(function (item) {
                                            return item.menuId;
                                        })
                                    });

                                    if (selectedChildren.length > 0) {
                                        var mappedTasks = $.map(submenu, function (item) {
                                            selectcheck = $.map(selectedChildren, function (item1) {
                                                if (item1.menuId == item) {
                                                    return item
                                                }
                                            });
                                            return selectcheck
                                        });
                                    } else {
                                        self.SelectedChildrenMenu([])
                                        selectedChildren = ''
                                    }

                                    self.SelectedChildrenMenu(mappedTasks);
                                    self.SelectedChildrenMenuToAdd(mappedTasks);
                                    //Enabling Child Menu List on Load
                                    $('.menu-child').css('display', 'block');
                                }
                            });
                        } else {
                            SelectedParentMenu([]);
                            SelectedChildrenMenu([]);
                            selectedChildren = '';
                        }
                    }
                }
                
            },
            error: function (error) {
                console.log("error", error)
            }
        });
    });

    self.SelectedChildrenMenu.subscribe(function () {
        selectedChildren = self.SelectedChildrenMenu();
    });
    //Load CHild navigation after parent menu is selected
    self.SelectedParentMenu.subscribe(function (props) {

        submenu = '';
        submenu = $.map(ko.toJS(self.SelectedParentMenu()), function (item) {
            return ko.toJS(self.MenuList()).filter(x => x.parentId == item)
        });

        var submenuForChecker = submenu.filter(x => x.menuRole == 'C')
        var submenuForMaker = submenu.filter(x => x.menuRole == 'M')

        var mappedTasks = $.map(submenu, function (item) {
            selectcheck = $.map(selectedChildren, function (item1) {
                if (item1 == item.menuId) {
                    return item1
                }
            });
            return selectcheck
        });
        self.ChildMenuListForChecker(submenuForChecker);
        self.ChildMenuListForMaker(submenuForMaker);
        submenu = $.map(ko.toJS(self.SelectedParentMenu()), function (item) {
            return ko.toJS(self.MenuList()).filter(x => x.parentId == item)
        });
        var mappedTasks = $.map(submenu, function (item) {
            selectcheck = $.map(selectedChildren, function (item1) {
                if (item1 == item.menuId) {
                    return item1
                }
            });
            return selectcheck
        });

        SelectedChildrenMenu(mappedTasks);
        //Enabling Child Menu List on Load
        $('.menu-child').css('display', 'block');
    });

    //Update Menu Rights to user
    self.UpdateRights = function () {

       
        if (self.Validation()) {
            var menuToAdd;
            var parentMenuList = (ko.toJS(self.SelectedParentMenu()));
            childrenMenuList = ko.toJS(self.SelectedChildrenMenu());
            menuToAdd = parentMenuList.concat(childrenMenuList);
           
            postReq('/Security/UserRight/AddRightsByRole', { RoleId: self.SelectedRole(), menuList: menuToAdd, addUpdate: "U" }, null, null, { redir: '/Security/UserRight/' });
        }
    }



}

$(document).ready(function () {
    ko.applyBindings(new UserRightsViewModel());
});