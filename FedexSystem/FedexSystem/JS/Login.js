$(function () {
    var LoginURL = "/Login/ValidateLogin?UserName=";

    $(document).keydown(function (keycode) {
        if (keycode.keyCode == 13) {
            $("#btnLogin").click();
        }
    });

    $("#btnLogin").click(function () {
        var userName = $("#txtUserName").val();
        var Password = $("#txtPassword").val();

        if (userName == "" || Password == "") {
            $.messager.alert("提示", "请填写完整的登录信息(用户名、密码)", "error");
            return false;
        }

        $.ajax({
            type: "GET",
            url: LoginURL + encodeURI(userName) + "&Password=" + encodeURI(Password),
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                var JSONMsg = eval("(" + msg + ")");
                if (JSONMsg.result.toLowerCase() == 'ok') {
                    var strRetURL = "";
                    if ($("#hidRetURL").val() != "") {
                        strRetURL = $("#hidRetURL").val();
                    } else {
                        strRetURL = "/MainFrame/Index";
                    }
                    window.location = strRetURL;
                } else {
                    //reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                    $.messager.alert('操作提示', JSONMsg.message, 'error');
                }
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
    });

    $("#txtUserName").focus();
});