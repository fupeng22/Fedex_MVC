$(function () {
    var _$_TVDepart = $("#TVDepart");
    var dlg_CreateCompany = $("#dlg_CreateCompany");
    var dlg_Create = $("#dlg_Create");
    var dlg_Update = $("#dlg_Update");

    var CreateCompany_dlg = null;
    var Create_dlg = null;
    var Update_dlg = null;

    var QueryURL = "/DepartManagement/GetDataWithUsers?state=open";

    var TestExist = "/DepartManagement/TestExistDepartName?departName=";
    var InsertCompany = "/DepartManagement/InsertTopDepartment?departName=";
    var InsertDepart = "/DepartManagement/InsertDepartment";
    var UpdateDepart = "/DepartManagement/UpdateDepartment";
    var TestExistInOther = "/DepartManagement/TestAlreadyUsedDepartName?departName=";
    var DeleteDepart = "/DepartManagement/DeleDepartment?departId=";
    var ExportURL = "/DepartManagement/SaveExcel";
    var IsDepartHasEmployee = "/DepartManagement/TestHasUsed?departId=";

    _$_TVDepart.tree({
        url: QueryURL, 
        onClick: function (node) {
            _$_TVDepart.tree('toggle', node.target);
        },
        onDblClick: function (node) {
            if (node.id.substring(0, 5) == "Users") {

            } else {
                Update();
            }
        },
        onContextMenu: function (e, node) {
            if (node.id.substring(0, 5) == "Users") {

            } else {
                e.preventDefault();
                _$_TVDepart.tree('select', node.target);
                $('#mm').menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });
            }
        }
    });

    CreateCompany_dlg = dlg_CreateCompany.dialog({
        buttons: [{
            text: '保 存',
            iconCls: 'icon-ok',
            handler: function () {
                var newCompanyName = $("#txtCompanyName").val();
                newCompanyName = $.trim(newCompanyName);
                if (newCompanyName && newCompanyName != "") {
                    $.ajax({
                        type: "GET",
                        url: TestExist + encodeURI(newCompanyName),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            var jsonMsg = eval("(" + msg + ")");
                            if (jsonMsg.result == "ok") {
                                $.ajax({
                                    type: "GET",
                                    url: InsertCompany + encodeURI(newCompanyName),
                                    data: "",
                                    async: true,
                                    cache: false,
                                    beforeSend: function (XMLHttpRequest) {

                                    },
                                    success: function (msg) {
                                        var JSONMsg = eval("(" + msg + ")");
                                        if (JSONMsg.result.toLowerCase() == 'ok') {
                                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                            CreateCompany_dlg.dialog('close');
                                            Query();
                                        } else {
                                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                                        }
                                    },
                                    complete: function (XMLHttpRequest, textStatus) {

                                    },
                                    error: function () {

                                    }
                                });
                            } else {
                                reWriteMessagerAlert("提示", jsonMsg.message, "error");
                                return false;
                            }
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });
                } else {
                    reWriteMessagerAlert('操作提示', '请填写完整的顶级部门名称!', 'error');
                    return false;
                }
            }
        }, {
            text: '关 闭',
            iconCls: 'icon-cancel',
            handler: function () {
                CreateCompany_dlg.dialog('close');
            }
        }],
        title: '添加顶级部门信息',
        modal: true,
        resizable: true,
        cache: false,
        closed: true,
        left: 50,
        top: 30,
        width: 400,
        height: 150
    });

    Create_dlg = $('#dlg_Create').dialog({
        buttons: [{
            text: '保 存',
            iconCls: 'icon-ok',
            handler: function () {
                var subDepartName = $("#txtSubDepart_Create").val();
                var ParentDepartName = $("#span_ParentDepart_Create").html();
                var ParentDepartId = $("#hid_ParentDepartId").val();
                subDepartName = $.trim(subDepartName);
                if (subDepartName && subDepartName != "") {
                    $.ajax({
                        type: "GET",
                        url: TestExist + encodeURI(subDepartName),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            var jsonMsg = eval("(" + msg + ")");
                            if (jsonMsg.result == "ok") {
                                $.ajax({
                                    type: "GET",
                                    url: InsertDepart + "?departName=" + encodeURI(subDepartName) + "&parentDepartId=" + encodeURI(ParentDepartId),
                                    data: "",
                                    async: false,
                                    cache: false,
                                    beforeSend: function (XMLHttpRequest) {

                                    },
                                    success: function (msg) {
                                        var JSONMsg = eval("(" + msg + ")");
                                        if (JSONMsg.result.toLowerCase() == 'ok') {
                                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                            Create_dlg.dialog('close');
                                            Query();
                                        } else {
                                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                                        }
                                    },
                                    complete: function (XMLHttpRequest, textStatus) {

                                    },
                                    error: function () {

                                    }
                                });
                            } else {
                                reWriteMessagerAlert("提示", jsonMsg.message, "error");
                                return false;
                            }
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });
                } else {
                    reWriteMessagerAlert('操作提示', '请填写完整的下级部门名称!', 'error');
                    return false;
                }
            }
        }, {
            text: '关 闭',
            iconCls: 'icon-cancel',
            handler: function () {
                Create_dlg.dialog('close');
            }
        }],
        title: '添加下级部门',
        modal: true,
        resizable: true,
        cache: false,
        closed: true,
        left: 50,
        top: 30,
        width: 400,
        height: 150
    });

    Update_dlg = $('#dlg_Update').dialog({
        buttons: [{
            text: '保 存',
            iconCls: 'icon-ok',
            handler: function () {
                var newDepartName = $("#txtNewDepartName").val();
                var oldDepartName = $("#span_OldDepartName").html();
                var departId = $("#hid_DepartId").val();
                newDepartName = $.trim(newDepartName);
                if (newDepartName == oldDepartName) {
                    reWriteMessagerAlert('操作提示', '新部门与旧部门名称一致,不需要修改!', 'error');
                    return false;
                } else {
                    if (newDepartName && newDepartName != "") {
                        $.ajax({
                            type: "GET",
                            url: TestExistInOther + encodeURI(newDepartName) + "&departId=" + encodeURI(departId),
                            data: "",
                            async: true,
                            cache: false,
                            beforeSend: function (XMLHttpRequest) {

                            },
                            success: function (msg) {
                                var jsonMsg = eval("(" + msg + ")");
                                if (jsonMsg.result == "ok") {
                                    $.ajax({
                                        type: "GET",
                                        url: UpdateDepart + "?newDepartName=" + encodeURI(newDepartName) + "&departId=" + encodeURI(departId),
                                        data: "",
                                        async: true,
                                        cache: false,
                                        beforeSend: function (XMLHttpRequest) {

                                        },
                                        success: function (msg) {
                                            var JSONMsg = eval("(" + msg + ")");
                                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                                Create_dlg.dialog('close');
                                                Query();
                                            } else {
                                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                                            }
                                        },
                                        complete: function (XMLHttpRequest, textStatus) {

                                        },
                                        error: function () {

                                        }
                                    });
                                } else {
                                    reWriteMessagerAlert("提示", jsonMsg.message, "error");
                                    return false;
                                }
                            },
                            complete: function (XMLHttpRequest, textStatus) {

                            },
                            error: function () {

                            }
                        });
                    } else {
                        reWriteMessagerAlert('操作提示', '请填写完整的新部门名称!', 'error');
                        return false;
                    }
                }
            }
        }, {
            text: '关 闭',
            iconCls: 'icon-cancel',
            handler: function () {
                Update_dlg.dialog('close');
            }
        }],
        title: '修改部门信息',
        modal: true,
        resizable: true,
        cache: false,
        closed: true,
        left: 50,
        top: 30,
        width: 400,
        height: 150
    });

    dlg_CreateCompany.dialog("close");
    dlg_Create.dialog("close");
    dlg_Update.dialog("close");

    $("#btnQuery").click(function () {
        Query();
    });

    $("#btnAddCompany").click(function () {
        AppendCompany();
    });

    $("#btnAdd").click(function () {
        Append();
    });

    $("#btnUpdate").click(function () {
        Update();
    });

    $("#btnDelete").click(function () {
        Remove();
    });

    $("#btnExpandAll").click(function () {
        expandAll();
    });

    $("#btnCollapseAll").click(function () {
        collapseAll();
    });

    $("#btnExport").click(function () {
        ExportExcel();
    });

    $("#btnPrint").click(function () {
        print();
    });

    $("#divQuery").click(function () {
        Query();
    });

    $("#divAddCompany").click(function () {
        AppendCompany();
    });

    $("#divAdd").click(function () {
        Append();
    });

    $("#divUpdate").click(function () {
        Update();
    });

    $("#divDelete").click(function () {
        Remove();
    });

    $("#divExpand").click(function () {
        expandAll();
    });

    $("#divCollapse").click(function () {
        collapseAll();
    });

    $("#divExport").click(function () {
        ExportExcel();
    });

    $("#divPrint").click(function () {
        print();
    });

    function Query() {
        var node = null;
        if (node) {
            _$_TVDepart.tree('reload', node.target);
        } else {
            _$_TVDepart.tree('reload');
        }
    }

    function AppendCompany() {
        $("#txtCompanyName").val("");
        $("#txtCompanyName").focus();
        CreateCompany_dlg = dlg_CreateCompany.dialog({
            buttons: [{
                text: '保 存',
                iconCls: 'icon-ok',
                handler: function () {
                    var newCompanyName = $("#txtCompanyName").val();
                    newCompanyName = $.trim(newCompanyName);
                    if (newCompanyName && newCompanyName != "") {
                        $.ajax({
                            type: "GET",
                            url: TestExist + encodeURI(newCompanyName),
                            data: "",
                            async: false,
                            cache: false,
                            beforeSend: function (XMLHttpRequest) {

                            },
                            success: function (msg) {
                                var jsonMsg = eval("(" + msg + ")");
                                if (jsonMsg.result == "ok") {
                                    $.ajax({
                                        type: "GET",
                                        url: InsertCompany + encodeURI(newCompanyName),
                                        data: "",
                                        async: true,
                                        cache: false,
                                        beforeSend: function (XMLHttpRequest) {

                                        },
                                        success: function (msg) {
                                            var JSONMsg = eval("(" + msg + ")");
                                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                                CreateCompany_dlg.dialog('close');
                                                Query();
                                            } else {
                                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                                            }
                                        },
                                        complete: function (XMLHttpRequest, textStatus) {

                                        },
                                        error: function () {

                                        }
                                    });
                                } else {
                                    reWriteMessagerAlert("提示", jsonMsg.message, "error");
                                    return false;
                                }
                            },
                            complete: function (XMLHttpRequest, textStatus) {

                            },
                            error: function () {

                            }
                        });
                    } else {
                        reWriteMessagerAlert('操作提示', '请填写完整的顶级部门名称!', 'error');
                        return false;
                    }
                }
            }, {
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    CreateCompany_dlg.dialog('close');
                }
            }],
            title: '添加顶级部门信息',
            modal: true,
            resizable: true,
            cache: false,
            closed: true,
            left: 50,
            top: 30,
            width: 400,
            height: 150
        });

        dlg_CreateCompany.dialog("open");
    }

    function Append() {
        var node = _$_TVDepart.tree('getSelected');
        if (node) {
            if (node.id.substring(0, 5) == "Users") {
                reWriteMessagerAlert('操作提示', '不允许在用户下面添加子部门!', 'error');
                return false;
            }
        } else {
            reWriteMessagerAlert('操作提示', "请先选择需要添加子部门的上级部门!", 'error');
            return false;
        }

        $("#span_ParentDepart_Create").html(node.text);
        $("#hid_ParentDepartId").val(node.id);
        $("#txtSubDepart_Create").val("");

        $("#txtSubDepart_Create").focus();

        Create_dlg = $('#dlg_Create').dialog({
            buttons: [{
                text: '保 存',
                iconCls: 'icon-ok',
                handler: function () {
                    var subDepartName = $("#txtSubDepart_Create").val();
                    var ParentDepartName = $("#span_ParentDepart_Create").html();
                    var ParentDepartId = $("#hid_ParentDepartId").val();
                    subDepartName = $.trim(subDepartName);
                    if (subDepartName && subDepartName != "") {
                        $.ajax({
                            type: "GET",
                            url: TestExist + encodeURI(subDepartName),
                            data: "",
                            async: false,
                            cache: false,
                            beforeSend: function (XMLHttpRequest) {

                            },
                            success: function (msg) {
                                var jsonMsg = eval("(" + msg + ")");
                                if (jsonMsg.result == "ok") {
                                    $.ajax({
                                        type: "GET",
                                        url: InsertDepart + "?departName=" + encodeURI(subDepartName) + "&parentDepartId=" + encodeURI(ParentDepartId),
                                        data: "",
                                        async: false,
                                        cache: false,
                                        beforeSend: function (XMLHttpRequest) {

                                        },
                                        success: function (msg) {
                                            var JSONMsg = eval("(" + msg + ")");
                                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                                Create_dlg.dialog('close');
                                                Query();
                                            } else {
                                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                                            }
                                        },
                                        complete: function (XMLHttpRequest, textStatus) {

                                        },
                                        error: function () {

                                        }
                                    });
                                } else {
                                    reWriteMessagerAlert("提示", jsonMsg.message, "error");
                                    return false;
                                }
                            },
                            complete: function (XMLHttpRequest, textStatus) {

                            },
                            error: function () {

                            }
                        });
                    } else {
                        reWriteMessagerAlert('操作提示', '请填写完整的下级部门名称!', 'error');
                        return false;
                    }
                }
            }, {
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    Create_dlg.dialog('close');
                }
            }],
            title: '添加下级部门',
            modal: true,
            resizable: true,
            cache: false,
            closed: true,
            left: 50,
            top: 30,
            width: 400,
            height: 150
        });

        $('#dlg_Create').dialog("open");
    }

    function Update() {
        var node = _$_TVDepart.tree('getSelected');
        if (node) {
            if (node.id.substring(0, 5) == "Users") {
                reWriteMessagerAlert('操作提示', '不允许修改用户!', 'error');
                return false;
            }
        } else {
            reWriteMessagerAlert('操作提示', '请先选择需要修改的部门!', 'error');
            return false;
        }

        $("#span_OldDepartName").html(node.text);
        $("#hid_DepartId").val(node.id);
        $("#txtNewDepartName").val(node.text);

        $("#txtNewDepartName").focus();

        Update_dlg = $('#dlg_Update').dialog({
            buttons: [{
                text: '保 存',
                iconCls: 'icon-ok',
                handler: function () {
                    var newDepartName = $("#txtNewDepartName").val();
                    var oldDepartName = $("#span_OldDepartName").html();
                    var departId = $("#hid_DepartId").val();
                    newDepartName = $.trim(newDepartName);
                    if (newDepartName == oldDepartName) {
                        reWriteMessagerAlert('操作提示', '新部门与旧部门名称一致,不需要修改!', 'error');
                        return false;
                    } else {
                        if (newDepartName && newDepartName != "") {
                            $.ajax({
                                type: "GET",
                                url: TestExistInOther + encodeURI(newDepartName) + "&departId=" + encodeURI(departId),
                                data: "",
                                async: true,
                                cache: false,
                                beforeSend: function (XMLHttpRequest) {

                                },
                                success: function (msg) {
                                    var jsonMsg = eval("(" + msg + ")");
                                    if (jsonMsg.result == "ok") {
                                        $.ajax({
                                            type: "GET",
                                            url: UpdateDepart + "?newDepartName=" + encodeURI(newDepartName) + "&departId=" + encodeURI(departId),
                                            data: "",
                                            async: true,
                                            cache: false,
                                            beforeSend: function (XMLHttpRequest) {

                                            },
                                            success: function (msg) {
                                                var JSONMsg = eval("(" + msg + ")");
                                                if (JSONMsg.result.toLowerCase() == 'ok') {
                                                    reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                                    Update_dlg.dialog('close');
                                                    Query();
                                                } else {
                                                    reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                                                }
                                            },
                                            complete: function (XMLHttpRequest, textStatus) {

                                            },
                                            error: function () {

                                            }
                                        });
                                    } else {
                                        reWriteMessagerAlert("提示", jsonMsg.message, "error");
                                        return false;
                                    }
                                },
                                complete: function (XMLHttpRequest, textStatus) {

                                },
                                error: function () {

                                }
                            });
                        } else {
                            reWriteMessagerAlert('操作提示', '请填写完整的新部门名称!', 'error');
                            return false;
                        }
                    }
                }
            }, {
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    Update_dlg.dialog('close');
                }
            }],
            title: '修改部门信息',
            modal: true,
            resizable: true,
            cache: false,
            closed: true,
            left: 50,
            top: 30,
            width: 400,
            height: 150
        });

        $('#dlg_Update').dialog("open");
    }

    function Remove() {
        var node = _$_TVDepart.tree('getSelected');
        if (node) {
            if (node.id.substring(0, 5) == "Users") {
                reWriteMessagerAlert('操作提示', '不允许删除用户!', 'error');
                return false;
            }

            var b = _$_TVDepart.tree('isLeaf', node.target);
            if (!b) {
                reWriteMessagerAlert('操作提示', '此部门有下级部门，无法删除，请先将其下属部门删除!', "操作提示", 'error');
                return false;
            } else {
                reWriteMessagerConfirm('提示', '您确定需要删除此部门吗?', function (r) {
                    if (r) {
                        $.ajax({
                            type: "GET",
                            url: IsDepartHasEmployee + encodeURI(node.id),
                            data: "",
                            async: false,
                            cache: false,
                            beforeSend: function (XMLHttpRequest) {

                            },
                            success: function (msg) {
                                var jsonMsg = eval("(" + msg + ")");
                                if (jsonMsg.result == "ok") {
                                    $.ajax({
                                        type: "GET",
                                        url: DeleteDepart + encodeURI(node.id),
                                        data: "",
                                        async: false,
                                        cache: false,
                                        beforeSend: function (XMLHttpRequest) {

                                        },
                                        success: function (msg) {
                                            var JSONMsg = eval("(" + msg + ")");
                                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                                Query();
                                            } else {
                                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                                            }
                                        },
                                        complete: function (XMLHttpRequest, textStatus) {

                                        },
                                        error: function () {

                                        }
                                    });
                                } else {
                                    alert(jsonMsg.message);
                                    //reWriteMessagerAlert("提示", jsonMsg.message, "error");
                                    return false;
                                }
                            },
                            complete: function (XMLHttpRequest, textStatus) {

                            },
                            error: function () {

                            }
                        });
                    }
                });
            }
        } else {
            reWriteMessagerAlert('操作提示', '请选择需要删除的部门<br>(<font style="color:red;font-weight:bold">只允许删除不带子部门的部门</font>)!', 'error');
            return false;
        }
    }

    function collapseAll() {
        var node = _$_TVDepart.tree('getSelected');
        if (node) {
            _$_TVDepart.tree('collapseAll', node.target);
        } else {
            _$_TVDepart.tree('collapseAll');
        }
    }

    function expandAll() {
        var node = _$_TVDepart.tree('getSelected');
        if (node) {
            _$_TVDepart.tree('expandAll', node.target);
        } else {
            _$_TVDepart.tree('expandAll');
        }
    }
});