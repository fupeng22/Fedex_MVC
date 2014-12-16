$(function () {
    var _$_datagrid = $("#DG_XRayScanResult");

    var _$_ddlPostion = $("#ddlPostion");

    _$_ddlPostion.combotree('loadData', [
    {
        id: -99,
        text: '---所有部门---'
    },
    {
        id: "X0",
        text: '二楼X光机一'
    },
    {
        id: "X1",
        text: '二楼X光机二'
    },
    {
        id: "X2",
        text: '二楼X光机三'
    },
    {
        id: "X3",
        text: '二楼X光机四'
    },
    {
        id: "X4",
        text: '二楼X光机五'
    },
    {
        id: "X5",
        text: '二楼X光机六'
    },
    {
        id: "X6",
        text: '一楼安检(化工品)'
    },
    {
        id: "X7",
        text: '一楼大X光机一'
    },
    {
        id: "X8",
        text: '一楼大X光机二'
    }]);

    _$_ddlPostion.combotree("setValue", "-99");

    var PrintURL = "";
    var QueryURL = "/QueryDetail/GetData?ddlPostion=" + encodeURI(_$_ddlPostion.combotree("getValues").join(',')) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val());

    $("#btnQuery").click(function () {
        $("#lblPostion").html(_$_ddlPostion.combotree('tree').tree('getSelected').text);
        $("#lblStartTime").html($("#inputBeginDate").val());
        $("#lblEndTime").html($("#inputEndDate").val());
        QueryURL = "/QueryDetail/GetData?ddlPostion=" + encodeURI(_$_ddlPostion.combotree("getValues").join(',')) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnReset").click(function () {
        _$_ddlPostion.combotree("setValue", "-99");
        $("#inputBeginDate").val("");
        $("#inputEndDate").val("");
        $("#btnQuery").click();
    });

    $("#btnPrint").click(function () {
        PrintURL = "/QueryDetail/Print?ddlPostion=" + encodeURI(_$_ddlPostion.combotree("getValues").join(',')) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/QueryDetail/Excel?ddlPostion=" + encodeURI(_$_ddlPostion.combotree("getValues").join(',')) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
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
        idField: 'CLID',
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
                    { field: 'CargoBC', title: '子运单号', width: 200, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CargoName', title: '品名', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CargoQuantity', title: '数量', width: 150, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CargoIDCN', title: '鉴定书编号', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ScanUserNumber', title: '安检员', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'OpenOrNot', title: '开箱', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CheckResultsDescription', title: '查验结果', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CheckUserNumber', title: '开箱员', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    }
        //                    ,
        //                    { field: 'printFirstPickGoods', title: '操作', width: 120,
        //                        formatter: function (value, rowData, rowIndex) {
        //                            return "<a href='#' class='printFirstPickGoods_cls' wbID='" + rowData.wbID + "'>打印快件提货单</a>";
        //                        }
        //                    }
				]],
        pagination: true,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
        toolbar: "#toolBar",
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
});
