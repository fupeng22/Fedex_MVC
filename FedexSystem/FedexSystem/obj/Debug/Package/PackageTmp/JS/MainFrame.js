﻿//function dd() {
//    $('#tab_Content').tabs('select', 0);
//}

$(function () {
    var defaultTabTitle = "使用指南";
    var LogOutURL = "/Login/LogOut";
    var GetDateTimeURL = "/Util/getDatetime";

    var bGetDateOK = false;
    var iDifferent = 0;

    tabClose();
    tabCloseEven();

    function addTab(subtitle, url, icon) {
        if (!$('#tab_Content').tabs('exists', subtitle)) {
            $('#tab_Content').tabs('add', {
                title: subtitle,
                content: createFrame(url),
                closable: true,
                icon: icon
            });
        } else {
            $('#tab_Content').tabs('select', subtitle);
            //$('#mm-tabupdate').click();
            var currTab = $('#tab_Content').tabs('getSelected');
            var url = $(currTab.panel('options').content).attr('src');
            if (url != undefined && currTab.panel('options').title != defaultTabTitle) {
                $('#tab_Content').tabs('update', {
                    tab: currTab,
                    options: {
                        content: createFrame(url)
                    }
                })
            }
        }
        tabClose();
    }

    function createFrame(url) {
        var s = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:99%;"></iframe>';
        return s;
    }

    function tabClose() {
        /*双击关闭TAB选项卡*/
        $(".tabs-inner").dblclick(function () {
            var subtitle = $(this).children(".tabs-closable").text();
            $('#tab_Content').tabs('close', subtitle);
        })
        /*为选项卡绑定右键*/
        $(".tabs-inner").bind('contextmenu', function (e) {
            $('#mm').menu('show', {
                left: e.pageX,
                top: e.pageY
            });

            var subtitle = $(this).children(".tabs-closable").text();

            $('#mm').data("currtab", subtitle);
            $('#tab_Content').tabs('select', subtitle);
            return false;
        });
    }
    //绑定右键菜单事件
    function tabCloseEven() {
        //刷新
        $('#mm-tabupdate').click(function () {
            var currTab = $('#tab_Content').tabs('getSelected');
            var url = $(currTab.panel('options').content).attr('src');
            $('#tab_Content').tabs('update', {
                tab: currTab,
                options: {
                    content: createFrame(url)
                }
            })
        })
        //关闭当前
        $('#mm-tabclose').click(function () {
            var currtab_title = $('#mm').data("currtab");
            $('#tab_Content').tabs('close', currtab_title);
        })
        //全部关闭
        $('#mm-tabcloseall').click(function () {
            $('.tabs-inner span').each(function (i, n) {
                var t = $(n).text();
                if (t == defaultTabTitle) {
                    return;
                }
                $('#tab_Content').tabs('close', t);
            });
        });
        //关闭除当前之外的TAB
        $('#mm-tabcloseother').click(function () {
            $('#mm-tabcloseright').click();
            $('#mm-tabcloseleft').click();
        });
        //关闭当前右侧的TAB
        $('#mm-tabcloseright').click(function () {
            var nextall = $('.tabs-selected').nextAll();
            if (nextall.length == 0) {
                //msgShow('系统提示','后边没有啦~~','error');
                //alert('后边没有啦~~');
                return false;
            }
            nextall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                if (t != defaultTabTitle) {
                    $('#tab_Content').tabs('close', t);
                }
            });
            return false;
        });
        //关闭当前左侧的TAB
        $('#mm-tabcloseleft').click(function () {
            var prevall = $('.tabs-selected').prevAll();
            if (prevall.length == 0) {
                //alert('到头了，前边没有啦~~');
                return false;
            }
            prevall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                if (t != defaultTabTitle) {
                    $('#tab_Content').tabs('close', t);
                }
            });
            return false;
        });

        //退出
        $("#mm-exit").click(function () {
            $('#mm').menu('hide');
        })
    }

    //设置导航菜单栏的事件
    $("#btnXRayVideoQuery").click(function () {
        addTab($(this).text(), "/XRayVideoQuery/Index", "icon-reload");
    });

    $("#btnOpenCheckVideoQuery").click(function () {
        addTab($(this).text(), "/OpenCheckVideoQuery/Index", "icon-reload");
    });

    $("#btnQueryDetail").click(function () {
        addTab($(this).text(), "/QueryDetail/Index", "icon-reload");
    });

    $("#btnScanStatistic").click(function () {
        addTab($(this).text(), "/ScanStatistic/Index", "icon-reload");
    });

    $("#btnDepartManagement").click(function () {
        addTab($(this).text(), "/DepartManagement/Index", "icon-reload");
    });

    $("#btnUserInfo").click(function () {
        addTab($(this).text(), "/UserInfo/Index", "icon-reload");
    });

    $("#btnQueryInterface").click(function () {
        addTab($(this).text(), "/QueryInterface/Index", "icon-reload");
    });

    $("#btnXRayDutyReal").click(function () {
        addTab($(this).text(), "/XRayDutyReal/Index", "icon-reload");
    });

    $("#btnOnDutyStatisticByXRay").click(function () {
        addTab($(this).text(), "/OnDutyStatisticByXRay/Index", "icon-reload");
    });

    $("#btnOnDutyStatisticByUser").click(function () {
        addTab($(this).text(), "/OnDutyStatisticByUser/Index", "icon-reload");
    });

    $("#btnNewQueryInterface").click(function () {
        addTab($(this).text(), "/NewQueryInterface/Index", "icon-reload");
    });
    //    $('#btnXRayVideoQuery').tooltip({
    //        position: 'right',
    //        content: '<span style="color:#fff">This is the tooltip message.</span>',
    //        onShow: function () {
    //            $(this).tooltip('tip').css({
    //                backgroundColor: '#666',
    //                borderColor: '#666'
    //            });
    //        }
    //    });

    //    var myViewModel = {
    //        personName: ko.observable('Bob'),
    //        personAge: ko.observable(123)
    //    };
    //    ko.applyBindings(myViewModel);


    //    $("#btnChange").click(function () {
    //        myViewModel.personName("我们");
    //    });

    $("#btnRelogin").click(function () {
        $.messager.confirm("操作提示", "您确定需要注销吗?", function (ok) {
            var bLogOutOK = false;
            if (ok) {
                $.ajax({
                    type: "GET",
                    url: LogOutURL,
                    data: "",
                    async: false,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var msg = eval("(" + msg + ")");
                        if (msg.result.toLowerCase() == "ok") {
                            bLogOutOK = true;
                        } else {
                            $.messager.alert('操作提示', msg.message, 'error');
                            bLogOutOK = false;
                        }
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });
            }
            if (bLogOutOK) {
                window.location = "/Login/Index";
            }
        });
    });

    setInterval(function () {
        if (!bGetDateOK) {
            $.ajax({
                type: "GET",
                url: GetDateTimeURL,
                data: "",
                async: true,
                cache: false,
                beforeSend: function (XMLHttpRequest) {

                },
                success: function (msg) {
                    $("#lblCurrentDate").html(new Date(Date.parse(msg.replace(/-/g, "/"))).toLongDateTimeString());
                    bGetDateOK = true;

                    var date = new Date();
                    var now = "";
                    now = date.getFullYear() + "-";
                    now = now + (date.getMonth() + 1) + "-";  //取月的时候取的是当前月-1如果想取当前月+1就可以了
                    now = now + date.getDate() + " ";
                    now = now + date.getHours() + ":";
                    now = now + date.getMinutes() + ":";
                    now = now + date.getSeconds() + "";

                    msg = new Date(Date.parse(msg.replace(/-/g, "/")));
                    now = new Date(Date.parse(now.replace(/-/g, "/")));

                    iDifferent = dateDiff(now, msg, "s");

                },
                complete: function (XMLHttpRequest, textStatus) {

                },
                error: function () {

                }
            });
        } else {
            var date = new Date();
            var now = "";
            now = date.getFullYear() + "-";
            now = now + (date.getMonth() + 1) + "-";  //取月的时候取的是当前月-1如果想取当前月+1就可以了
            now = now + date.getDate() + " ";
            now = now + date.getHours() + ":";
            now = now + date.getMinutes() + ":";
            now = now + date.getSeconds() + "";

            now = new Date(Date.parse(now.replace(/-/g, "/")));
            $("#lblCurrentDate").html(new Date(Date.parse(dateCon(DateFormat(now, "yyyy-MM-dd HH:mm:ss"), iDifferent).replace(/-/g, "/"))).toLongDateTimeString());
        }

    }, 1000);

   
});