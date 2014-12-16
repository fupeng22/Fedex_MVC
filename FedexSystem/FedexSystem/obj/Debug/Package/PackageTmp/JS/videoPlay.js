
function XrayPlay(xrayContent) {
  if(xrayContent=="a")
  {
   alert("未找见设备！");
   
  }else{
    var xrayArr = [];
    xrayArr = xrayContent.split(';');
    if (xrayArr[0] != "") {
        var cameInfo = document.getElementById("CameraInfo").value;

        
        //拆分日期
//        var date = xrayArr[3].split(' ');
//        var dateArr = date[0].split('-');
//        if (dateArr[1] <= 9) {
//            dateArr[1] = '0' + dateArr[1];
//        }
//        if (dateArr[2] <= 9) {
//            dateArr[2] = '0' + dateArr[2];
//        }
//        var timeArr = date[1].split(':');
//        var dateTimeStr = dateArr[0] + dateArr[1] + dateArr[2] + timeArr[0] + timeArr[1] + timeArr[2];

        var xRayStr = '&DeviceID=' + xrayArr[0] + '&CameraID=' + xrayArr[1] + '&CameraName=' + xrayArr[2] + '&RecTime=' + xrayArr[3] + '&TagName=' + xrayArr[4] + '&RecLocation=1';
        cameInfo = cameInfo.replace(/;/g, '&');

        var total = cameInfo + xRayStr;
// alert(total);
        var strWidth = "800px";
        var strHeight = "600px";
        var robj = showModalDialog(total, window, "dialogHeight='" + strHeight + "';dialogWidth='" + strWidth + "';status=no;scroll=no;help=no");
    }
    else {
        alert("未找见录像点，图像回放失败！");
    }
    }
}

function ImageShow(url) {
   //  alert(url);
  var dUrl = encodeURI(url);
//  alert(dUrl);
  // window.open('dUrl', 'newwindow', 'height=600, width=800, top=100, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no');
  var strWidth = "800px";
  var strHeight = "600px";
  var robj = showModalDialog(dUrl, window, "dialogHeight='" + strHeight + "';dialogWidth='" + strWidth + "';status=no;scroll=no;help=no");
}


function OpenPlay(openContent) {
    if (openContent != "") {
        var cameInfo = document.getElementById("CameraInfo").value;
        cameInfo = cameInfo.replace(/;/g, '&');
        var total = cameInfo + openContent;
        var strWidth = "800px";
        var strHeight = "600px";
        var robj = showModalDialog(total, window, "dialogHeight='" + strHeight + "';dialogWidth='" + strWidth + "';status=no;scroll=no;help=no");
    }
    else {
        alert("未找见录像点，图像回放失败！");
    }
}



//建立XML配置文档
function createXML(chn) {
    var doc = new ActiveXObject("Microsoft.XMLDOM"); //ie5.5+,CreateObject("Microsoft.XMLDOM") 
    var header = doc.createProcessingInstruction("xml", "version='1.0' encoding='utf-8'"); //创建文件头信息
    doc.appendChild(header);
    var root = doc.createElement("Parament"); //根节点
    var matrixCode = doc.createElement("MatrixCode"); //云台矩阵
    matrixCode.text = "";
    var monitorID = doc.createElement("MonitorID");
    monitorID.text = "0";
    var cameraId = doc.createElement("CameraID"); //监控点id
    cameraId.text = chn; //++
    var cameraName = doc.createElement("CameraName"); //监控点名称
    cameraName.text = "";
    var ip = doc.createElement("DeviceIP"); //所属设备ip
    ip.text = "127.0.0.1";
    var port = doc.createElement("DevicePort"); //所属设备通信端口
    port.text = "8000";
    var type = doc.createElement("DeviceType"); //设备类型
    type.text = "";
    var user = doc.createElement("User"); //用户名
    user.text = "";
    var psw = doc.createElement("Password"); //密码
    psw.text = "";
    var channel = doc.createElement("ChannelNum"); //通道号，从0开始
    channel.text = "";
    var proto = doc.createElement("ProtocolType"); //协议类型，0代表tcp，1代表udp
    proto.text = "0";
    var stream = doc.createElement("StreamType"); //码流类型，0表示主码流，1表示子码流
    stream.text = "0";

    var RegionID = doc.createElement("RegionID"); //区域id
    RegionID.text = "";
    var ControlUnitID = doc.createElement("ControlUnitID"); //控制单元id
    ControlUnitID.text = "";
    root.appendChild(matrixCode);
    root.appendChild(monitorID);
    root.appendChild(cameraId);
    root.appendChild(cameraName);
    root.appendChild(ip);
    root.appendChild(port);
    root.appendChild(type);
    root.appendChild(user);
    root.appendChild(psw);
    root.appendChild(channel);
    root.appendChild(proto);
    root.appendChild(stream);
    root.appendChild(RegionID);
    root.appendChild(ControlUnitID);
    doc.appendChild(root);
    return doc;
}

//设置图片本地保存路径、格式、最小磁盘空间等参数
function set_img(obj) {
    var path = "C:\\Better\\Snapshot\\Preview\\";
    var type = 0;
    var min_size = 10;
    obj.SetCapturParam(path, type); //设置图片保存路径和格式
    var retval = obj.SetPicDiskMinSize(min_size); //设置保存图片磁盘空间最小值，成功返回0
    if (0 != retval) {
        alert("C盘空间不足！\n磁盘剩余空间必须大于10MB，才能保证截图存放成功。");
    }
}

//设置录像本地保存路径、文件大小、最小磁盘空间等参数
function set_record(obj) {
    var path = "C:\\Better\\Clip\\";
    var size = 64;
    var min_size = 100;
    obj.SetRecordParam(path, size); //设置录像保存路径和大小
    var retval = obj.SetRecordDiskMinSize(min_size); //设置保存录像磁盘空间最小值，成功返回0
    if (0 != retval) {
        alert("C盘空间不足！\n磁盘剩余空间必须大于100MB，才能保证录像剪辑存放成功。");
    }
}

//注册到CAG
var isRegister = false;
function register_cag(obj, cag_ip, cag_port, user, password) {
    //var cag_ip = "61.50.189.91";	//++
    //var cag_port = "6610";			//++
    //var user = "guest";				//++
    //var password = "52130465";		//++
    var result = obj.Register_CagByUserInfo(cag_ip, cag_port, user, password); //向cag注册用户
    if (0 !== result) {
        alert("cag服务器注册失败！");
    }
    else {
        isRegister = true;
        //alert("cag注册成功！");
    }
}

//开始预览
function play(obj, chn) {
    if (!isRegister) {
        alert("cag服务器注册失败！");
        return;
    }
    var win_num = obj.GetSelWnd(); //当前选中窗口
    var xmldoc = createXML(chn); //得到xml格式的配置参数文件
    //在win_num窗口开始预览，预览是否成功由预览事件通知
    var retval = obj.StartTask_Preview_InWnd(xmldoc.xml, win_num);
    if (0 != retval) alert("预览失败！");
}

//停止预览
function stop(obj) {
    var win_num = obj.GetSelWnd(); //当前选中窗口
    var retval = obj.stopRealPlay(win_num);
    if (retval != 0) {
        alert("关闭预览失败！");
    }
}	