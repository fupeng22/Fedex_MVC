$(function () {
    var _$_ddlDepart = $("#ddlDepart_Edit");
    var _$_ddlAuthority = $("#ddlAuthority_Edit");
    var LoadDepartTreeURL = "/DepartManagement/GetData?state=open";
    var LoadCheckPointTreeURL = "/CheckPoint/CreateCheckPointJSON";

    _$_ddlDepart.combotree({
        url: LoadDepartTreeURL,
        valueField: 'id',
        textField: 'text',
        onLoadSuccess: function () {
            _$_ddlDepart.combotree("setValue", $("#hid_ddlDepart").val());
        }
    });

    _$_ddlAuthority.combotree({
        url: LoadCheckPointTreeURL,
        valueField: 'id',
        textField: 'text'
    });

    $("#ddlSex_Edit").combobox({
        valueField: 'id',
        textField: 'text',
        data: [
            {
                id: "---请选择---",
                text: "---请选择---"
            },
        {
            id: 0,
            text: '男'
        },
        {
            id: 1,
            text: '女'
        }
        ],
        onLoadSuccess: function () {
            $("#ddlSex_Edit").combobox("setValue", $("#hid_ddlSex").val());
        }
    });

});
