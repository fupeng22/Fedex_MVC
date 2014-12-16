$(function () {
    var _$_datagrid_UserInfo = $("#DG_UserInfo");
    var _$_datagrid = $("#DG_XRayScanResult");
    var _$_TVDepart = $("#TVDepart");
    var _$_ddlPostion = $("#ddlPostion");
    var _$_ddlCargoProperty = $("#ddlCargoProperty");
    var _$_ddlinputDateRange = $("#inputDateRange");

    var userIds = "";

    var viewType = "";

    var QueryAllProduceScanLogHeaderURL = "/NewQueryInterface/getAllProduceScanLogHeaderInfo";

    _$_ddlinputDateRange.combobox({
        url: QueryAllProduceScanLogHeaderURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null,
        onLoadSuccess: function () {
            _$_ddlinputDateRange.combobox("setValue", "-99");
        }
    });

    _$_ddlPostion.combotree('loadData',
    [{
        id: "'-99'",
        text: '---所有X光机---'
    },
    {
        id: "'X0;%'",
        text: '二楼X光机一'
    },
    {
        id: "'X1;%'",
        text: '二楼X光机二'
    },
    {
        id: "'X2;%'",
        text: '二楼X光机三'
    },
    {
        id: "X3;%'",
        text: '二楼X光机四'
    },
    {
        id: "'X4;%'",
        text: '二楼X光机五'
    },
    {
        id: "'X5;%'",
        text: '二楼X光机六'
    },
    {
        id: "'X6;%'",
        text: '一楼安检(化工品)'
    },
    {
        id: "'X7;%'",
        text: '一楼大X光机一'
    },
    {
        id: "'X8;%'",
        text: '一楼大X光机二'
    }]);

    _$_ddlCargoProperty.combotree('loadData',
    [{
        id: -99,
        text: '---所有类型---'
    },
    {
        id: "0",
        text: '出口普货'
    },
    {
        id: "1",
        text: '出口危险品'
    },
    {
        id: "2",
        text: '进口普货'
    },
    {
        id: "3",
        text: '进口危险品'
    }]);

    _$_ddlPostion.combotree("setValue", "'-99'");
    _$_ddlCargoProperty.combotree("setValue", "-99");

    if ($("#rdViewType_1").attr("checked")) {
        viewType = "1";
    } else if ($("#rdViewType_2").attr("checked")) {
        viewType = "2";
    }

    //    switch ($("#hid_IsEmployee").val()) {
    //        case "0":

    //            break;
    //        case "1":
    //            $("#rdViewType_1").hide();
    //            $("#rdViewType_2").hide();
    //            $("#span_ViewType1").hide();
    //            $("#span_ViewType2").hide();

    //            viewType = "2";
    //            break;
    //        default:
    //            break;
    //    }

    var QueryURL = ""; // "/NewQueryInterface/GetData?ddlPostion=" + encodeURI(_$_ddlPostion.combotree("getValues").join(',')) + "&ddlCargoProperty=" + encodeURI(_$_ddlCargoProperty.combotree("getValues").join(',')) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&rdViewType=" + encodeURI(viewType) + "&userIds=" + encodeURI(userIds);

    var QueryURL_Depart = "/DepartManagement/GetData?state=open";

    var QueryURL_UserInfo = "/NewQueryInterface/GetData_UserInfo?strDeptId=-99";

    $("#btnQuery").click(function () {
        userIds = getSeleUserId();

        if ($("#rdViewType_1").attr("checked")) {
            viewType = "1";
        } else if ($("#rdViewType_2").attr("checked")) {
            viewType = "2";
        }

        if (userIds == "") {
            reWriteMessagerAlert('操作提示', '请选择需要查询的用户!', 'error');
            return false;
        }

        if (_$_ddlinputDateRange.combobox("getValue") == "-99") {
            reWriteMessagerAlert('操作提示', '请选择时间段!', 'error');
            return false;
        }

        QueryURL = "/NewQueryInterface/GetData?ddlPostion=" + encodeURI(_$_ddlPostion.combotree("getValues").join(',')) + "&ddlCargoProperty=" + encodeURI(_$_ddlCargoProperty.combotree("getValues").join(',')) + "&inputDateRange=" + encodeURI(_$_ddlinputDateRange.combobox("getValue")) + "&rdViewType=" + encodeURI(viewType) + "&userIds=" + encodeURI(userIds) + "&hid_IsEmployee=" + encodeURI($("#hid_IsEmployee").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 100); //延迟100毫秒执行，时间可以更短
    });

    $("#btnPrint").click(function () {
        if (_$_ddlinputDateRange.combobox("getValue") == "-99") {
            reWriteMessagerAlert('操作提示', '请选择时间段!', 'error');
            return false;
        }
        PrintURL = "/NewQueryInterface/Print?ddlPostion=" + encodeURI(_$_ddlPostion.combotree("getValues").join(',')) + "&ddlCargoProperty=" + encodeURI(_$_ddlCargoProperty.combotree("getValues").join(',')) + "&inputDateRange=" + encodeURI(_$_ddlinputDateRange.combobox("getValue")) + "&rdViewType=" + encodeURI(viewType) + "&userIds=" + encodeURI(userIds) + "&hid_IsEmployee=" + encodeURI($("#hid_IsEmployee").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            var div_PrintDlg = self.parent.$("#dlg_GlobalPrint");
            div_PrintDlg.show();
            var PrintDlg = null;
            div_PrintDlg.find("#frmPrintURL").attr("src", PrintURL);
            PrintDlg = div_PrintDlg.window({
                title: '打印',
                href: "",
                modal: true,
                resizable: true,
                minimizable: false,
                collapsible: false,
                cache: false,
                closed: true,
                width: 900,
                height: 500
            });
            div_PrintDlg.window("open");

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可打印", "error");
            return false;
        }

    });

    $("#btnExcel").click(function () {
        var browserType = "";
        if ($.browser.msie) {
            browserType = "msie";
        }
        else if ($.browser.safari) {
            browserType = "safari";
        }
        else if ($.browser.mozilla) {
            browserType = "mozilla";
        }
        else if ($.browser.opera) {
            browserType = "opera";
        }
        else {
            browserType = "unknown";
        }

        if (_$_ddlinputDateRange.combobox("getValue") == "-99") {
            reWriteMessagerAlert('操作提示', '请选择时间段!', 'error');
            return false;
        }
        PrintURL = "/NewQueryInterface/Excel?ddlPostion=" + encodeURI(_$_ddlPostion.combotree("getValues").join(',')) + "&ddlCargoProperty=" + encodeURI(_$_ddlCargoProperty.combotree("getValues").join(',')) + "&inputDateRange=" + encodeURI(_$_ddlinputDateRange.combobox("getValue")) + "&rdViewType=" + encodeURI(viewType) + "&userIds=" + encodeURI(userIds) + "&hid_IsEmployee=" + encodeURI($("#hid_IsEmployee").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }

    });

    _$_TVDepart.tree({
        url: QueryURL_Depart,
        onClick: function (node) {
            //_$_TVDepart.tree('check', node.target);
            QueryURL_UserInfo = "/NewQueryInterface/GetData_UserInfo?strDeptId=" + encodeURI(node.id);
            window.setTimeout(function () {
                $.extend(_$_datagrid_UserInfo.datagrid("options"), {
                    url: QueryURL_UserInfo
                });
                _$_datagrid_UserInfo.datagrid("reload");
            }, 10); //延迟100毫秒执行，时间可以更短
        }
    });

    _$_datagrid_UserInfo.datagrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        url: QueryURL_UserInfo,
        sortName: 'UserNumber',
        sortOrder: 'asc',
        remoteSort: true,
        border: false,
        idField: 'UserID',
        columns: [[
                        {
                            field: "cb", title: "", width: 10, checkbox: true
                        },
    					{ field: 'UserNumber', title: '账号', width: 80, sortable: true,
    					    sorter: function (a, b) {
    					        return (a > b ? 1 : -1);
    					    }
    					},
    					{ field: 'UserName', title: '姓名', width: 80, sortable: true,
    					    sorter: function (a, b) {
    					        return (a > b ? 1 : -1);
    					    }
    					},
                        { field: 'UserID', title: '标识', width: 0, hidden: true
                        }
    				]],
        pagination: false,
        onLoadSuccess: function (data) {
            _$_datagrid_UserInfo.datagrid("selectAll");
        }
    });

    _$_datagrid.datagrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'CLID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'cId',
        columns: [[
					{ field: 'ScanTime', title: '扫描时间', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'FlightNumber', title: '航班号', width: 100, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'CargoBC', title: '子运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CargoName', title: '品名', width: 250, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CargoQuantity', title: '数量', width: 50, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CargoIDCN', title: '鉴定书编号', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ScanUserNumber', title: '安检员', width: 60, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'OpenOrNot', title: '开箱', width: 50, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                        ,
                        formatter: function (value, rowData, rowIndex) {
                            //return "<input type='button' class='handle_ReInStore' value='入库' wbfID='" + rowData.WbfID + "'/>" + "<input type='button' class='handle_ReOutStore' value='出库' wbfID='" + rowData.WbfID + "'/>";
                            //return "<a href='#' class='handle_ReInStore' Wbf_wbID='" + rowData.wbID + "' Wbf_swbID='" + rowData.swbID + "' wbfID='" + rowData.WbfID + "'>入仓</a>" + "&nbsp;&nbsp;" + "<a href='#' class='handle_ReOutStore'  Wbf_wbID='" + rowData.wbID + "' Wbf_swbID='" + rowData.swbID + "'  wbfID='" + rowData.WbfID + "'>出仓</a>";
                            switch (rowData.OpenOrNot) {
                                case "0":
                                    return "<img src='../../images/EmptyCircle.png'/>";
                                    break;
                                case "1":
                                    return "<img src='../../images/SolidCircle.png'/>";
                                    break;
                                default:
                                    return "未知"
                                    break;
                            }
                        }
                    },
                    { field: 'CheckResultsDescription', title: '查验结果', width: 60, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CheckUserNumber', title: '开箱员', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    }
				]],
        pagination: true,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
        onHeaderContextMenu: function (e, field) {
            e.preventDefault();
            if (!$('#tmenu').length) {
                createColumnMenu();
            }
            $('#tmenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        },
        onSortColumn: function (sort, order) {
            //_$_datagrid.datagrid("reload");
        }
    });

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "cargobc":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "CargoBC") {

                } else {
                    if (item.iconCls == 'icon-ok') {
                        _$_datagrid.datagrid('hideColumn', $(item.text).attr("id"));
                        tmenu.menu('setIcon', {
                            target: item.target,
                            iconCls: 'icon-empty'
                        });
                    } else {
                        _$_datagrid.datagrid('showColumn', $(item.text).attr("id"));
                        tmenu.menu('setIcon', {
                            target: item.target,
                            iconCls: 'icon-ok'
                        });
                    }
                }
            }
        });
    }

    function getSeleUserId() {
        var selects = _$_datagrid_UserInfo.datagrid("getSelections");
        var ids = [];
        for (var i = 0; i < selects.length; i++) {
            ids.push(selects[i].UserID);
        }
        return ids.join(",");
    }
})

