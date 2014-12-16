$(function () {
    var _$_datagrid = $("#DG_MainQueryResult");
    var _$_ddlXRayType = $('#ddlXRayType');

    var GetAllXRayTypeInfoURL = "/MainFrame/GetAllXRayTypeInfo";

    _$_ddlXRayType.combotree({
        url: GetAllXRayTypeInfoURL,
        valueField: 'id',
        textField: 'text',
        onLoadSuccess: function (node, data) {
            _$_ddlXRayType.combotree("setValues", ["-99"]);
        }
    });

    var PrintSubWayBill = "";
    var GetXRayOnDutyDetailURL = "/OnDutyStatisticByUser/GetXRayOnDutyDetail?inputDate=";

    var PrintURL = "";
    var QueryURL = "/OnDutyStatisticByUser/GetData?inputBeginDate=" + encodeURI($("#txtBeginD").val()) + "&inputEndDate=" + encodeURI($("#txtEndD").val())+"&ddlXRayType=" + encodeURI(_$_ddlXRayType.combotree("getValues").join(','));

    $("#btnQuery").click(function () {
        QueryURL = "/OnDutyStatisticByUser/GetData?inputBeginDate=" + encodeURI($("#txtBeginD").val()) + "&inputEndDate=" + encodeURI($("#txtEndD").val())+"&ddlXRayType=" + encodeURI(_$_ddlXRayType.combotree("getValues").join(','));
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 20); //延迟100毫秒执行，时间可以更短
    });

    $("#btnReset").click(function () {
        $("#txtBeginD").val("");
        $("#txtEndD").val("");
        _$_ddlXRayType.combotree("setValue", "-99");
        $("#btnQuery").click();
    });

    $("#btnPrint").click(function () {
        PrintURL = "/OnDutyStatisticByUser/PrintXRayOnDutyInfoInfo?inputBeginDate=" + encodeURI($("#txtBeginD").val()) + "&inputEndDate=" + encodeURI($("#txtEndD").val())+"&ddlXRayType=" + encodeURI(_$_ddlXRayType.combotree("getValues").join(',')) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            var div_PrintDlg = self.parent.$("#dlg_GlobalPrint");
            div_PrintDlg.show();
            var PrintDlg = null;

//           div_PrintDlg.find("#p").show();
//            div_PrintDlg.find("#frmPrintURL").load(function(){
//                div_PrintDlg.find("#p").hide();
//            });
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

        PrintURL = "/OnDutyStatisticByUser/ExcelXRayOnDutyInfoInfo?inputBeginDate=" + encodeURI($("#txtBeginD").val()) + "&inputEndDate=" + encodeURI($("#txtEndD").val())+"&ddlXRayType=" + encodeURI(_$_ddlXRayType.combotree("getValues").join(','))+ "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        sortName: 'XRayTypeDes',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'cId',
        view: detailview,
        singleSelect: true,
        showFooter: true,
        detailFormatter: function (index, row) {
            return '<div style="padding:2px"><center><span style="color:red;font-weight:bold;">[<span id="XRay_On_Duty_' + index + '"></span>]到岗信息</span></center><table  id="ddv-' + index + '"></table></div></br>';
        },
        columns: [[
                    { field: 'XRayTypeDes', title: 'X光机', width: 300
                    },
					{ field: 'dDateDes', title: '日期', width: 120
					},
					{ field: 'LoginCount', title: '当日登入人次', width: 300, align: "center",
					    formatter: function (value, rowData, rowIndex) {
                        if (rowData.LoginCount!="-1") {
                                return "<center><a href='#' class='load_XRayOnDutyDetail' rowIndex='" + rowIndex + +" XRayTypeDes='"+rowData.XRayTypeDes+"' XRayType='" + rowData.XRayType + "' dDateDes='" + rowData.dDateDes + "'>" + rowData.LoginCount + "</a></center>";
                            }else{
                                return "当天无登录";
                            } 
					    }
					}
				]],
        pagination: true,
        pageSize: 15,
        pageList: [15,20,25,30,35,40,45,50],
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
        },
        onLoadSuccess: function (data) {
            delete $(this).datagrid('options').queryParams['id'];
            for (var i = 0; i < data.rows.length; i++) {
            if ( data.rows[i].LoginCount!="-1") {
                    _$_datagrid.datagrid("expandRow", i);
                }
                
            }

            var allload_XRayOnDutyDetailLnk = $(".load_XRayOnDutyDetail");
            $.each(allload_XRayOnDutyDetailLnk, function (i, item) {
                $(item).click(function () {
                    var _$_XRayType = $(item).attr("XRayType");
                    var _$_dDateDes = $(item).attr("dDateDes");
                    var _$_XRayTypeDes= $(item).attr("XRayTypeDes");
                    var _$_rowIndex= $(item).attr("rowIndex");

                    var _$_subDataGrid = $('#ddv-' + _$_rowIndex);
                    $("#XRay_On_Duty_" + _$_rowIndex).html("光机("+_$_XRayTypeDes+")"+_$_dDateDes);

                    var expander = _$_datagrid.datagrid('getExpander', _$_rowIndex);
                    _$_datagrid.datagrid("expandRow", _$_rowIndex);
                    window.setTimeout(function () {
                        $.extend(_$_subDataGrid.datagrid("options"), {
                            url: GetXRayOnDutyDetailURL + encodeURI(_$_dDateDes) + "&ddlXRayType="+encodeURI(_$_XRayType),
                        });
                        _$_subDataGrid.datagrid("reload");
                    },10);
                   
                });
            });
        },
        onClickRow: function (index, row) {
            var expander = _$_datagrid.datagrid('getExpander', index);
            if (row.LoginCount!="-1") {
                if (expander.hasClass('datagrid-row-collapse')) {
                    _$_datagrid.datagrid("collapseRow", index);
                } else {
                    _$_datagrid.datagrid("expandRow", index);
                }
            } 
        },
        onExpandRow: function (index, row) {
            var _$_subDataGrid = $('#ddv-' + index);
            $("#XRay_On_Duty_" + index).html("光机("+row.XRayTypeDes+")"+row.dDateDes);
            _$_subDataGrid.datagrid({
                    url: GetXRayOnDutyDetailURL + encodeURI(row.dDateDes) + "&ddlXRayType="+encodeURI(row.XRayType),
                    fitColumns: true,
                    loadMsg: '',
                    height: 'auto',
                    iconCls: 'icon-save',
                    nowrap: true,
                    autoRowHeight: false,
                    autoRowWidth: false,
                    striped: true,
                    collapsible: true,
                    sortName: 'wlWorkStart',
                    sortOrder: 'desc',
                    remoteSort: true,
                    border: true,
                    idField: 'wlID',
                    columns: [[
					            { field: 'UserName', title: '员工', width: 150, sortable: true,
					                sorter: function (a, b) {
					                    return (a > b ? 1 : -1);
					                }
					            },
                                { field: 'wlWorkStartDes', title: '登入时间', width: 120, sortable: true,
					                sorter: function (a, b) {
					                    return (a > b ? 1 : -1);
					                }
					            },
                                { field: 'wlWorkEndDes', title: '登出时间', width: 120, sortable: true,
					                sorter: function (a, b) {
					                    return (a > b ? 1 : -1);
					                }
					            },
                                { field: 'wlUseTimeDes', title: '用时', width: 120, sortable: true,
					                sorter: function (a, b) {
					                    return (a > b ? 1 : -1);
					                }
					            }
				            ]],
                    pagination: true,
                    toolbar: [{
                        id: 'btnPrint',
                        text: '打印',
                        disabled: false,
                        iconCls: 'icon-print',
                        handler: function () {
                        var _$_url=_$_subDataGrid.datagrid('options').url;
                            PrintSubWayBillInfo(_$_subDataGrid, row.wbID,_$_url.substring(_$_url.indexOf("InOutType"),_$_url.length).split("&")[0].split("=")[1]);
                        }
                    }, '-', {
                        id: 'btnExcel',
                        text: '导出',
                        disabled: false,
                        iconCls: 'icon-excel',
                        handler: function () {
                        var _$_url=_$_subDataGrid.datagrid('options').url;
                            ExcelSubWayBillInfo(_$_subDataGrid, row.wbID,_$_url.substring(_$_url.indexOf("InOutType"),_$_url.length).split("&")[0].split("=")[1]);
                        }
                    }],
                    onResize: function () {
                        _$_datagrid.datagrid('fixDetailRowHeight', index);
                    },
                    onLoadSuccess: function () {
                        setTimeout(function () {
                            _$_datagrid.datagrid('fixDetailRowHeight', index);
                        }, 0);
                    },
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
                    },
                });
                _$_datagrid.datagrid('fixDetailRowHeight', index);
        }
    });

    function PrintSubWayBillInfo(dg, wbID,InOutType) {
        PrintSubWayBill = "/OnDutyStatisticByUser/PrintSubWayBillInfo?txtWBID=" + encodeURI(wbID) +"&InOutType="+InOutType+ "&strSwbSerialNum=" + encodeURI($("#txtSubWayBillCode").val()) + "&order=" + dg.datagrid("options").sortOrder + "&sort=" + dg.datagrid("options").sortName + "&page=1&rows=10000000";
        if (dg.datagrid("getData").rows.length > 0) {
            var div_PrintDlg = self.parent.$("#dlg_GlobalPrint");
            div_PrintDlg.show();
            var PrintDlg = null;
            div_PrintDlg.find("#frmPrintURL").attr("src", PrintSubWayBill);
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
    }

    function ExcelSubWayBillInfo(dg, wbID,InOutType) {
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

        PrintURL = "/OnDutyStatisticByUser/ExcelSubWayBillInfo?txtWBID=" + encodeURI(wbID) +"&InOutType="+InOutType+ "&strSwbSerialNum=" + encodeURI($("#txtSubWayBillCode").val()) + "&order=" + dg.datagrid("options").sortOrder + "&sort=" + dg.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (dg.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:200px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "wbserialnum":
                    break;
                case "wbid":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "wbSerialNum") {

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
