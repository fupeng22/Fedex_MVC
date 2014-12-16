var timer = null;

function enableTime(b) {
    if (b) {
        timer = setInterval(function () {
            $("#btnRefreshAllPanels").click();
        }, $("#selectInterval").val() * 1000);
    } else {
        if (timer) {
            clearInterval(timer);
        }
    }
}

var settime = null;
function redraw() {
    self.parent.$("#layout_Main").layout('resize');
    //$('#mainLayout').layout('panel', 'north').panel('resize', { width: $('#mainLayout').width() });
    //$('#mainLayout').layout('panel', 'center').panel('resize', { width: $('#mainLayout').width() });
    //$('#mainLayout').layout('resize');
    //self.parent.$("#layout_Main").layout('resize');
}

$(function () {
    $(window).resize(function () {
        if (settime != null)
            clearTimeout(settime);
        settime = setTimeout("redraw()", 2000);
    });

    var _$_datagrid_CP_X0 = $("#DG_CP_X0");
    var _$_datagrid_CP_X1 = $("#DG_CP_X1");
    var _$_datagrid_CP_X2 = $("#DG_CP_X2");
    var _$_datagrid_CP_X3 = $("#DG_CP_X3");
    var _$_datagrid_CP_X4 = $("#DG_CP_X4");
    var _$_datagrid_CP_X5 = $("#DG_CP_X5");
    var _$_datagrid_CP_X6 = $("#DG_CP_X6");
    var _$_datagrid_CP_X7 = $("#DG_CP_X7");
    var _$_datagrid_CP_X8 = $("#DG_CP_X8");
    var _$_datagrid_CP_X9 = $("#DG_CP_X9");
    var _$_datagrid_CP_X10 = $("#DG_CP_X10");
    var _$_datagrid_CP_X11 = $("#DG_CP_X11");

    var LoadHistoryWorkingInfoURL = "/XRayDutyReal/LoadHistoryWorkingInfo?inputDate=" + encodeURI($("#inputDate").val()) + "&CPID=";

    var LoadXRayWorkingInfoURL = "/XRayDutyReal/LoadXRayWorkingInfo?CPs=";

    var LoadXRayTypeNameURL = "/XRayDutyReal/LoadXRayWorkingInfoByXRayType?XRayType=";

    var panels = [[0, "DIV_CP_X0", "X0"], [1, "DIV_CP_X1", "X1"], [2, "DIV_CP_X2", "X2"], [3, "DIV_CP_X3", "X3"], [4, "DIV_CP_X4", "X4"], [5, "DIV_CP_X5", "X5"], [6, "DIV_CP_X6", "X6"], [7, "DIV_CP_X7", "X7"], [8, "DIV_CP_X8", "X8"], [9, "DIV_CP_X9", "X9"], [10, "DIV_CP_X10", "X10"], [11, "DIV_CP_X11", "X11"]];
    for (var i = 0; i < panels.length; i++) {
        var title = "";
        $.ajax({
            type: "POST",
            url: LoadXRayTypeNameURL + encodeURI(panels[i][2]),
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                var JSONMsg = eval("(" + msg + ")");
                if (JSONMsg.result.toLowerCase() == 'ok') {
                    title = JSONMsg.XRayTypeName;
                } else {
                    title = JSONMsg.XRayTypeName;
                }
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });

        $("#" + panels[i][1]).panel({
            width: 270,
            height: 190,
            title: title,
            tools: "#panelTool_CP_X" + panels[i][0]
        });
    }

    var datagrids = [[_$_datagrid_CP_X0, "toolbar_CP_X0", 0], [_$_datagrid_CP_X1, "toolbar_CP_X1", 1], [_$_datagrid_CP_X2, "toolbar_CP_X2", 2], [_$_datagrid_CP_X3, "toolbar_CP_X3", 3], [_$_datagrid_CP_X4, "toolbar_CP_X4", 4], [_$_datagrid_CP_X5, "toolbar_CP_X5", 5], [_$_datagrid_CP_X6, "toolbar_CP_X6", 6], [_$_datagrid_CP_X7, "toolbar_CP_X7", 7], [_$_datagrid_CP_X8, "toolbar_CP_X8", 8], [_$_datagrid_CP_X9, "toolbar_CP_X9", 9], [_$_datagrid_CP_X10, "toolbar_CP_X10", 10], [_$_datagrid_CP_X11, "toolbar_CP_X11", 11]];
    for (var i = 0; i < datagrids.length; i++) {
        LoadHistoryWorkingInfoURL = "/XRayDutyReal/LoadHistoryWorkingInfo?inputDate=" + encodeURI($("#inputDate").val()) + "&CPID=" + encodeURI(datagrids[i][2]);
        datagrids[i][0].datagrid({
            iconCls: 'icon-save',
            nowrap: true,
            autoRowHeight: false,
            autoRowWidth: false,
            striped: true,
            collapsible: true,
            url: LoadHistoryWorkingInfoURL,
            sortName: 'wlWorkStart',
            sortOrder: 'desc',
            remoteSort: true,
            border: false,
            loadMsg: '',
            idField: 'wlID',
            columns: [[
            			{ field: 'UserName', title: '员工', width: 70, sortable: true,
            			    sorter: function (a, b) {
            			        return (a > b ? 1 : -1);
            			    }
            			},
            			{ field: 'wlWorkStartDes', title: '登入时间', width: 60, sortable: true,
            			    sorter: function (a, b) {
            			        return (a > b ? 1 : -1);
            			    }
            			},
                        { field: 'wlWorkEndDes', title: '登出时间', width: 60, sortable: true,
                            sorter: function (a, b) {
                                return (a > b ? 1 : -1);
                            }
                        },
                            { field: 'wlUseTimeDes', title: '工作时长', width: 70, sortable: true,
                                sorter: function (a, b) {
                                    return (a > b ? 1 : -1);
                                },
                                formatter: function (value, rowData, rowIndex) {
                                    var sRet = "";
                                    if (parseFloat(rowData.wlUseTimeDes) > 45) {
                                        sRet = rowData.wlUseTimeDes + "<img  src='../../images/warning.png'/>";
                                    } else {
                                        sRet = rowData.wlUseTimeDes;
                                    }
                                    return sRet;
                                }
                            }
            		]],
            toolbar: "#" + datagrids[i][1]
        });
    }

    var panelTools = ["btn_panelTool_CP_X0", "btn_panelTool_CP_X1", "btn_panelTool_CP_X2", "btn_panelTool_CP_X3", "btn_panelTool_CP_X4", "btn_panelTool_CP_X5", "btn_panelTool_CP_X6", "btn_panelTool_CP_X7", "btn_panelTool_CP_X8", "btn_panelTool_CP_X9", "btn_panelTool_CP_X10", "btn_panelTool_CP_X11"];
    for (var i = 0; i < panelTools.length; i++) {
        $("#" + panelTools[i]).click(function () {
            RefreshPanels([$(this).attr("iwitchcp")]);
        });
    }

    $("#btnRefreshAllPanels").click(function () {
        RefreshPanels([0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]);
    });

    $("#chkEnableTime").click(function () {
        enableTime($(this).attr("checked"));
    });

    $("#selectInterval").change(function () {
        enableTime($("#chkEnableTime").attr("checked"));
    });

    $("#btnFullScreen").click(function () {
        self.parent.$("#layout_Main").layout("collapse", "north");
        self.parent.$("#layout_Main").layout("collapse", "west");
    });

    $("#btnResetScreen").click(function () {
        self.parent.$("#layout_Main").layout("expand", "north");
        self.parent.$("#layout_Main").layout("expand", "west");
    });

    //传入X光机查验点数组，数组中各个元素为X光机编号，如:[0,1,2,3,4]
    function RefreshPanels(CPs) {
        //刷新给定X光机的当前员工工作状态
        var bOK = false;
        $.ajax({
            type: "POST",
            url: LoadXRayWorkingInfoURL + encodeURI(CPs.join(',')),
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                var JSONMsg = eval("(" + msg + ")");
                var rows = null;
                if (JSONMsg.result.toLowerCase() == 'ok') {
                    bOK = true;
                    rows = JSONMsg.rows;
                    for (var i = 0; i < rows.length; i++) {
                        $("#span_CP_X" + rows[i].iWhichCP + "_CurrentEmp").html(rows[i].UserNameDes);
                        $("#span_CP_X" + rows[i].iWhichCP + "_HasWorkedLong").html(rows[i].WorkingLong);
                        $("#span_CP_X" + rows[i].iWhichCP + "_StartWorkTime").html(rows[i].StartTime);
                        $("#span_CP_X" + rows[i].iWhichCP + "_NeedEndWorkTime").html(rows[i].EndTime);
                    }
                } else {
                    reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                }
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
        if (bOK) {
            for (var i = 0; i < CPs.length; i++) {
                //获取各个X光机当前工作员工信息
                LoadHistoryWorkingInfoURL = "/XRayDutyReal/LoadHistoryWorkingInfo?inputDate=" + encodeURI($("#inputDate").val()) + "&CPID=" + encodeURI(CPs[i]);

                for (var j = 0; j < datagrids.length; j++) {
                    if (datagrids[j][2] == CPs[i]) {
                        $.extend(datagrids[j][0].datagrid("options"), {
                            url: LoadHistoryWorkingInfoURL
                        });
                        datagrids[j][0].datagrid("reload");
                    }
                }

            }
        }
    }
    $("#btnRefreshAllPanels").click();

//    $("#btnTest").click(function () {
//        //console.info($("#tab_Content", window.parent.window.document).tabs('getSelected'));
//        //console.info($(window.parent.window.document).find("#tab_Content"));
//        console.info(window.parent.window.dd());
//    });
});