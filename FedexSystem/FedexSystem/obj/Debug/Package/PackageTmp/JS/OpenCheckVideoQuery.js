﻿$(function () {
    var _$_datagrid = $("#DG_OpenCheckResult");
    var _$_ddlCheckResult = $("#ddlCheckResult");

    var VideoRePlay_ShowPic_Dlg = null;
    var VideoRePlay_ShowPic_Dlg_URL = "";

    _$_ddlCheckResult.combotree('loadData', [
    {
        id: -99,
        text: '---请选择(可多选)---'
    },
    {
        id: 0,
        text: '忽略'
    },
    {
        id: 1,
        text: '放行'
    },
    {
        id: 2,
        text: '扣留'
    }]);

    _$_ddlCheckResult.combotree("setValue", "-99");

    var PrintURL = "";
    var QueryURL = "/OpenCheckVideoQuery/GetData?txtCode=" + encodeURI($("#txtCode").val()) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&hidSearchType=" + encodeURI($("#hidSearchType").val()) + "&ddlCheckResult=" + encodeURI(_$_ddlCheckResult.combotree("getValues").join(',')) + "&txtCP=" + encodeURI($("#txtCP").val());

    $("#btnQuery").click(function () {
        $("#hidSearchType").val(1);
        QueryURL = "/OpenCheckVideoQuery/GetData?txtCode=" + encodeURI($("#txtCode").val()) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&hidSearchType=" + encodeURI($("#hidSearchType").val()) + "&ddlCheckResult=" + encodeURI(_$_ddlCheckResult.combotree("getValues").join(',')) + "&txtCP=" + encodeURI($("#txtCP").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#GbtnQuery").click(function () {
        $("#hidSearchType").val(0);
        QueryURL = "/OpenCheckVideoQuery/GetData?txtCode=" + encodeURI($("#txtCode").val()) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&hidSearchType=" + encodeURI($("#hidSearchType").val()) + "&ddlCheckResult=" + encodeURI(_$_ddlCheckResult.combotree("getValues").join(',')) + "&txtCP=" + encodeURI($("#txtCP").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnReset").click(function () {
        $("#txtCode").val("");
        $("#txtCP").val("");
        $("#btnQuery").click();
    });

    $("#GbtnReset").click(function () {
        $("#inputBeginDate").val("");
        $("#inputEndDate").val("");
        $("#txtGCode").val("");
        $("#txtGOperator").val("");
        $("#GbtnQuery").click();
        //console.info(_$_datagrid.datagrid("getPager")); 
        //        var p = _$_datagrid.datagrid('getPager');
        //        $(p).pagination({
        //            pageindex: 2
        //        });
        //        _$_datagrid.datagrid('reload');
    });

    $("#btnPrint").click(function () {
        PrintURL = "/OpenCheckVideoQuery/Print?txtCode=" + encodeURI($("#txtCode").val()) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&hidSearchType=" + encodeURI($("#hidSearchType").val()) + "&ddlCheckResult=" + encodeURI(_$_ddlCheckResult.combotree("getValues").join(',')) + "&txtCP=" + encodeURI($("#txtCP").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/OpenCheckVideoQuery/Excel?txtCode=" + encodeURI($("#txtCode").val()) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&hidSearchType=" + encodeURI($("#hidSearchType").val()) + "&ddlCheckResult=" + encodeURI(_$_ddlCheckResult.combotree("getValues").join(',')) + "&txtCP=" + encodeURI($("#txtCP").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        sortName: 'CKID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'CKID',
        columns: [[
					{ field: 'CargoBC', title: '货物条码', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'CargoName', title: '货物名称', width: 250, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'CheckCP', title: '岗位识别号', width: 70, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'UserName', title: '操作员姓名', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CheckResultsDescription', title: '检查结果', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CheckDescription', title: '结果详细描述', width: 200, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CheckBeginTime', title: '扫描开始时间', width: 122, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CheckEndTime', title: '扫描结束时间', width: 122, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                   { field: 'VideoRePlay', title: '录像回放', width: 80,
                       formatter: function (value, rowData, rowIndex) {
                           return "<a href='#' class='btn_VideoRePlay' VideoRePlay='" + rowData.VideoRePlay + "'>回放</a>";
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
        },
        onLoadSuccess: function (data) {

            var allVideoRePlayBtn = $(".btn_VideoRePlay");
            $.each(allVideoRePlayBtn, function (i, item) {
                var VideoRePlay = $(item).attr("VideoRePlay");
                $(item).click(function () {
                    VideoRePlay_ShowPic(0, VideoRePlay, "");
                });

            });
        }
    });

    function VideoRePlay_ShowPic(iType, VideoRePlay, ShowPic) {
        switch (iType) {
            case 0: //录像回放
                var xrayArr = [];
                var total = "";
                xrayArr = VideoRePlay.split(';');
                if (xrayArr[0] != "") {
                    var cameInfo = $("#hidCameraInfo").val();

                    var xRayStr = '&DeviceID=' + xrayArr[0] + '&CameraID=' + xrayArr[1] + '&CameraName=' + xrayArr[2] + '&RecTime=' + xrayArr[3] + '&TagName=' + xrayArr[4] + '&RecLocation=1';
                    cameInfo = cameInfo.replace(/;/g, '&');

                    total = cameInfo + xRayStr;
                }
                VideoRePlay_ShowPic_Dlg_URL = total;
                VideoRePlay_ShowPic_Dlg = $('#dlg_VideoRePlay').dialog({
                    buttons: [{
                        text: '关 闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            VideoRePlay_ShowPic_Dlg.dialog('close');
                        }
                    }],
                    title: '录像回放',
                    href: VideoRePlay_ShowPic_Dlg_URL,
                    modal: true,
                    resizable: true,
                    cache: false,
                    left: 50,
                    top: 10,
                    width: 1020,
                    height: 480,
                    closed: true
                });
                $('#dlg_VideoRePlay').dialog("open");
                break;
            case 1: //查看图像
                VideoRePlay_ShowPic_Dlg_URL = ShowPic;
                VideoRePlay_ShowPic_Dlg = $('#dlg_VideoRePlay').dialog({
                    buttons: [{
                        text: '关 闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            VideoRePlay_ShowPic_Dlg.dialog('close');
                        }
                    }],
                    title: '查看图像',
                    href: VideoRePlay_ShowPic_Dlg_URL,
                    modal: true,
                    resizable: true,
                    cache: false,
                    left: 50,
                    top: 10,
                    width: 1020,
                    height: 480,
                    closed: true
                });
                $('#dlg_VideoRePlay').dialog("open");
                break;
            default:
                break;
        }
    }

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
