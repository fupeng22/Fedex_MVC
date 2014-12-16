$(function () {
    var _$_datagrid = $("#DG_XRayScanResult");

    var PrintURL = "";
    var QueryURL = "/ScanStatistic/GetData?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val());

    $("#btnQuery").click(function () {
        QueryURL = "/ScanStatistic/GetData?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnReset").click(function () {
        $("#inputBeginDate").val($("#hid_inputBeginDate").val());
        $("#inputEndDate").val($("#hid_inputEndDate").val());
        $("#btnQuery").click();
    });

    $("#btnPrint").click(function () {
        PrintURL = "/ScanStatistic/Print?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/ScanStatistic/Excel?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        sortName: 'indexSort',
        sortOrder: 'asc',
        remoteSort: true,
        border: false,
        idField: 'indexSort',
        columns: [[
					{ field: 'scanPostion', title: '岗位', width: 300, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'sumCount', title: '总件数', width: 150, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'scanCount', title: '扫描件数', width: 150, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'scanRate', title: '扫描率', width: 150, sortable: true,
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
        pagination: false,
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
                case "scanpostion":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "scanPostion") {

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
