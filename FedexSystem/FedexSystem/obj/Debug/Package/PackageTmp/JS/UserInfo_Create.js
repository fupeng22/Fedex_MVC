$(function () {
    var _$_ddlDepart = $("#ddlDepart");
    var _$_ddlAuthority = $("#ddlAuthority");
    var LoadDepartTreeURL = "/DepartManagement/GetData?state=open";
    var LoadCheckPointTreeURL = "/CheckPoint/CreateCheckPointJSON";

    _$_ddlDepart.combotree({
        url: LoadDepartTreeURL,
        valueField: 'id',
        textField: 'text'
    });

    _$_ddlAuthority.combotree({
        url: LoadCheckPointTreeURL,
        valueField: 'id',
        textField: 'text'
    });
});
