var status_list;
function parseTableJson() {    
    var json = [];
    status_list.empty();
    var terminal = null;
    $("#hidden_div table tbody tr").each(function () {
        var item = {};
        $(this).find("td").each(function () {
            var fieldName = this.getAttribute("field");
            item[fieldName] = this.innerText;
        });
        json[json.length] = item;
        terminal = item.tid;
    })
    if (json.length == 0) {
        status_list.html("没有刷卡数据！");
        return;
    }
    var customerInfo = null;
    var newJson =[];
    try{
    	//获取客户信息
    	var queryData=clientEx.execQuery("*","customers","terminal='"+ terminal +"'",null);
    	if(queryData.length == 1)  customerInfo = queryData[0];
    	if(customerInfo==null){ throw("获取客户信息is null, terminal："+terminal); }
    	if(!customerInfo.discount || isNaN(customerInfo.discount)) { throw("获取客户信息discount is null or NaN,terminal："+terminal); }
    	if(!customerInfo.tixianfei || isNaN(customerInfo.tixianfei)){ throw("获取客户信息tixianfei is null or NaN, terminal："+terminal); }
    	if(!customerInfo.tixianfeiEles || isNaN(customerInfo.tixianfeiEles)){ throw("获取客户信息tixianfeiEles is null or NaN, terminal："+terminal); }

	    for(var i=0;i<json.length;i++){
	    	var item =json[i];
	    	//判断记录是否存在,存在的话忽略,不存在则插入
	    	var existCount = clientEx.execQuery("count(0) cc", "transactionLogs", "terminal='" + item.tid + "' and timeStr='" + item.tdate + "' and tradeName='" + item.trname + "' and tradeMoney=" + item.amt + " ", null);	    	
	    	if(existCount[0].cc > 0) continue;
	    	var localItem = convertToLocal(item);
	        //计算手续费
	    	localItem.shanghuName = customerInfo.shanghuName;	    	
	    	localItem.discountMoney =getCustomerShouXuFeiPos(customerInfo,localItem.tradeMoney);
	    	localItem.tixianfeiMoney = getCustomerShouXuFei(customerInfo, localItem.tradeMoney);
	    	localItem.finallyMoney = localItem.tradeMoney - localItem.discountMoney - localItem.tixianfeiMoney;
	    	localItem.id = clientEx.getNexVal("transactionLogs");	    	
	    	localItem.status=0; 

	    	var inserDBJson=[{ table:"transactionLogs" ,action: 0 ,fields:localItem}];
	    	clientEx.execDb(inserDBJson);
	    	newJson[newJson.length] = localItem;
	    	$("<div>" + localItem.time.substr(11,8) + " , " + item.trname + " : " + item.amt + "</div>").appendTo(status_list);
	    	addTerminalView(localItem);
	    }
	}catch(e){
	    onError("保存数据时发生异常,请赶快处理！" + e.message);
	    return;
	}
    if (newJson.length > 0) {
        //是否有结算
        var maxId = 0;
        try {
            $(newJson).each(function () {
                if (this.tradeName == "批上送结束(平账)") {
                	$("#settle_status_list").empty();
                    maxId = this.id;
                    var whereSql = "terminal='" + this.terminal + "' and Status=0 and tradeName='IC卡批上送通知交易(联机平账)' and resultCode='00' and time< '"+ this.time +"'";
                    var transData = clientEx.execQuery("count(0) batchCount,sum(tradeMoney) tradeMoney,sum(discountMoney) discountMoney,sum(tixianfeiMoney) tixianfeiMoney", "transactionLogs", whereSql, null);                    
                    if (transData == 0 || transData.length == 0) throw ("查询transactionLogs数据为空");
                    var sumData = transData[0];
                    if (sumData.batchCount == 0) return;
                    var sumid = clientEx.getNexVal("transactionSum");
                    sumData.id = sumid;
                    sumData.finallyMoney = sumData.tradeMoney - sumData.discountMoney - sumData.tixianfeiMoney;
                    sumData.status = 0;
                    sumData.terminal = this.terminal;
                    var inserDBJson = [{ table: "transactionlogs", action: 1, fields: { sumid: sumid, Status: 1 }, where: whereSql }, { table: "transactionSum", action: 0, fields: sumData }];
                    clientEx.execDb(inserDBJson);
                }
            });
        } catch (e) {
            onError("用户发起了结算，但汇总交易数据失败！" + e.message);
            return;
        }

    }
    //return JSON.stringify(newJson);
}


function setStatus(str) {
    $("#status").html(str);
}

function setSumStatus(str) {
    $("#settle_status").html(str);
    $("#settle_status_list").empty();
}

function appendSumLog(str) {
    $("#settle_status_list").append("<div>" + str + "</div>");
}

function setBankStatus(batnum,str){
	var div = $("#bank_status_list #bank"+batnum);
	if(div.length ==0) div = $("<div>" + str + "</div>").appendTo("#bank_status_list");
	div.html(batnum+": "+str);
}

function getCustomerShouXuFeiPos(customerInfo,money){
	//节假日
	return (money*customerInfo.discount* 0.01).toFixed(2);
}

function getCustomerShouXuFei(customerInfo,money){
	//节假日
	return (money*customerInfo.tixianfei* 0.01).toFixed(2);
}


function init(){	
    status_list = $("#status_list");
    var today =new Date();
    var date= today.Format("yyyy-MM-dd");
    var prevdate=  today.AddDays(-1).Format("yyyy-MM-dd");
    var where= " time between '"+prevdate+" 23:00:00' and '"+date+" 23:59:59' ";//前一天11到今天11点
    try{
	    var todayAllData=clientEx.execQuery("*","transactionLogs",where,null);
	    setStatus("未开启监控！");	
	    if (todayAllData != null) {
	        for (var i = 0; i < todayAllData.length; i++) {
	            addTerminalView(todayAllData[i]);
	        }
	        window.external.InitOK();
	    }
    }catch(e){
    	alert(e);
    }
	
}

function onError(msg){	
	status_list.html(msg);
	window.external.onError(msg);

}

function convertToLocal(netLog) {
    var time ;
    var arr = netLog.tdate.split(' ');
    var dayStr = arr[0];
    var timeStr = arr[1];
    var minute = timeStr.substr(2, 2);
    if (minute > 59) minute = 59;
    var second= timeStr.substr(4, 2);
    if (second > 59) second = 59;
    time = dayStr.substr(0, 4) + '-' + dayStr.substr(4, 2) + '-' + dayStr.substr(6, 2) + " " + timeStr.substr(0, 2) + ":" + minute + ":" + second;
    //交易结果代码
    var pos = netLog.rap.indexOf('(');
    var resultCode = null;
    if(pos!=-1){
    	var endPos= netLog.rap.indexOf(')');
    	if(endPos>0) resultCode = netLog.rap.substr(pos+1,endPos-1);
    }
	if(!resultCode){ throw "交易结果代码获取方式有变！"; }
    return { terminal: netLog.tid, time: time, tradeName: netLog.trname, tradeMoney: netLog.amt, results: netLog.rap,resultCode: resultCode, faren: netLog.lpName, timeStr: netLog.tdate};
}

function addTerminalView(item){

    var tview=$("#termianls").find("#view_"+ item.terminal);

	if(tview.length ==0){		
		 tview=$('<div class="col-sm-6">'+
	        	 '<div id="view_'+ item.terminal +'" class="panel panel-default">'+
			     '       <div class="panel-heading">'+
			     '         <h3 class="panel-title">['+ item.terminal +"] "+ item.faren +'</h3>'+
			     '       </div>'+
			     '       <div class="panel-body">没有刷卡!</div>'+
		         ' </div>'+
	      ' </div>').appendTo("#termianls");
      }
      var viewTableBody = tview.find(".panel-body table tbody");
      if(viewTableBody.length ==0){
      	var  panelBody = tview.find(".panel-body").empty();
      	var viewTable=$('<table class="table table-striped"><thead><th>时间</th><th>交易名称</th><th>金额</th><th>结果</th></thead></table>').appendTo(panelBody);
      	viewTableBody=$('<tbody></tbody>').appendTo(viewTable);
      }
      var newRow= $('<tr><td>'+ item.time +'</td><td>'+ item.tradeName +'</td><td>'+ item.tradeMoney +'</td><td>'+ item.results +'</td></tr>').appendTo(viewTableBody);
	
}
