var status_list;
function parseTableJson(jsonStr) {    
	getIsHoldDay();
//    var json = [];
//    status_list.empty();
//    var terminal = null;
//    $("#hidden_div table tbody tr").each(function () {
//        var item = {};
//        $(this).find("td").each(function () {
//            var fieldName = this.getAttribute("field");
//            item[fieldName] = this.innerText;
//        });
//        json[json.length] = item;
//        terminal = item.tid;
//    })
	status_list.empty();
	var json ;
	eval('json = '+jsonStr );
	if(json==null){
	 	status_list.html("json is null！");
        return;
	}
	if(json.rows==null){
	 	status_list.html("json rows is null！"+ jsonStr);
        return;
	}
	json = json.rows;
    if (json.length == 0) {
        status_list.html("没有刷卡数据！");
        return;
    }
    var terminal = json[0].tid;
    var customerInfo = null;
    var newJson =[];
    try{
    	//获取客户信息
    	var queryData=clientEx.execQuery("*","customers","terminal='"+ terminal +"'",null);
    	if(queryData.length == 1)  customerInfo = queryData[0];
    	if(customerInfo==null){ throw("获取客户信息is null, terminal："+terminal); }
    	if(!customerInfo.discount || isNaN(customerInfo.discount)) { throw("获取客户信息discount is null or NaN,terminal："+terminal); }
    	if(!customerInfo.tixianfei || isNaN(customerInfo.tixianfei)){ throw("获取客户信息tixianfei is null or NaN, terminal："+terminal); }
    	if(customerInfo.tixianfeiEles==null || isNaN(customerInfo.tixianfeiEles)){ throw("获取客户信息tixianfeiEles is null or NaN, terminal："+terminal); }

	    for(var i=json.length-1;i > -1 ;i--){
	    	var item =json[i];
	    	var localItem = convertToLocal(item,customerInfo);
	    	//判断记录是否存在,存在的话忽略,不存在则插入
	    	var whereSql ="terminal='" + item.tid + "' and time='" + localItem.time + "' and tradeName='" + item.trname + "' and tradeMoney=" + localItem.tradeMoney ;
	    	var existCount = clientEx.execQuery("count(0) cc", "transactionLogs",whereSql , null);	    	
	    	if(existCount[0].cc > 0){ continue;}
	    	//alert(whereSql);
	        //计算手续费
	    	localItem.shanghuName = customerInfo.shanghuName;	    	
	    	calcMoney(customerInfo,localItem);
	    	localItem.id = clientEx.getNexVal("transactionLogs");	    	
	    	localItem.status=0; 

	    	var inserDBJson=[{ table:"transactionLogs" ,action: 0 ,fields:localItem}];
	    	clientEx.execDb(inserDBJson);
	    	newJson[newJson.length] = localItem;
	    	$("<div>" + localItem.time.substr(11,8) + " , " + item.trname + " : " + item.amt + "</div>").appendTo(status_list);
	    	addTerminalView(localItem);
	    }
	}catch(e){
	    if(typeof(e)=="string") onError("保存数据时发生异常,请赶快处理！" + e);
	    else onError("保存数据时发生异常,请赶快处理！" + e.message);
	    return;
	}
    if (newJson.length > 0) {
        //是否有结算
        var maxId = 0;
        try {
            $(newJson).each(function () {
                if (this.tradeName == "批上送结束(平账)" ||  this.tradeName == "批上送结束(不平账)") {
                	$("#settle_status_list").empty();
                    maxId = this.id;
                    //get prev jieshuang date
                    var whereSql = "terminal='" + this.terminal + "' and Status=0 and tradeName in ('批上送结束(平账)','批上送结束(不平账)') and resultCode='00' and time < '"+ this.time +"'";
                    var prevData = clientEx.execQuery("max(time) prevTime ","transactionLogs",whereSql, null);
                    if (prevData == null || prevData.length == 0) throw ("查询上次结算数据失败");
                    var prevTime= prevData[0].prevTime;
                    //sum log of (消费)
                    var whereSql = "terminal='" + this.terminal + "' and Status=0 and isValid = 1 and resultCode='00'";
                    if(prevTime ==null || prevTime == '')
                    	whereSql+=" and time < '"+ this.time +"'";
                    else 
                    	whereSql+=" and time between '"+prevTime+"' and '"+ this.time +"'";                   

                    var transData = clientEx.execQuery("count(0) batchCount,sum(tradeMoney) tradeMoney,sum(discountMoney) discountMoney,sum(tixianfeiMoney) tixianfeiMoney", "transactionLogs", whereSql, null);                    
                    if (transData == 0 || transData.length == 0) throw ("查询transactionLogs数据为空");
                    var sumData = transData[0];
                    if (sumData.batchCount == 0){setSumStatus("发起了结算，但是没有交易记录"+ whereSql);return;}
                    var sumid = clientEx.getNexVal("transactionSum");
                    sumData.id = sumid;
                    sumData.finallyMoney = sumData.tradeMoney - sumData.discountMoney - sumData.tixianfeiMoney;
                    sumData.status = 0;
                    sumData.createDate = (new Date()).Format("yyyy-MM-dd hh:mm:ss");
                    sumData.terminal = this.terminal;
                    whereSql = "("+whereSql+") or id ="+ this.id;
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
    $("#settle_status_list").prepend("<div>" + str + "</div>");
}

function setBankStatus(batnum,str){
	var div = $("#bank_status_list #bank"+batnum);
	if(div.length ==0) div = $("<div id='bank"+batnum+"' >" + str + "</div>").appendTo("#bank_status_list");
	div.html(batnum+": "+str);
}
var isHoldDay;
//是否节假日
function getIsHoldDay(){
	isHoldDay= null;
	var today = (new Date()).Format("yyyy-MM-dd");
	alert(today);
	$.ajax({
		url:'http://www.easybots.cn/api/holiday.php?d='+today,
		dataType:'json',
		ansy:false,
		error:function(e1,e2){alert(JSON.stringify( e1));alert(e2);},
		success:function(d){
				isHoldDay= parseInt( d[today]) > 0;
		}
	});
	if(isHoldDay ==null) throw ("getIsHoldDay web services is exception happen");
}

function calcMoney (customerInfo,localItem){
	if(customerInfo.IsFengDing && !customerInfo.fengding)  throw("封顶机没配置封顶金额");
	if(isHoldDay) customerInfo.tixianfei+=customerInfo.tixianfeiEles;//节假日多收手续费
	localItem.discountMoney= (localItem.tradeMoney*customerInfo.discount* 0.01).toFixed(2);
	if(!customerInfo.IsFengDing ||  localItem.discountMoney < customerInfo.fengding){
		//pos ji
		localItem.discountMoney= (localItem.tradeMoney*customerInfo.discount* 0.01).toFixed(2);
		var totalSxf =( localItem.tradeMoney*(customerInfo.discount + customerInfo.tixianfei)* 0.01).toFixed(2)
		//t0
		localItem.tixianfeiMoney= totalSxf  - localItem.discountMoney;
	}else{		
		//feng ding
		localItem.discountMoney = customerInfo.fengding;
		localItem.tixianfeiMoney = (localItem.tradeMoney*customerInfo.tixianfei* 0.01).toFixed(2);
	}
	localItem.finallyMoney = localItem.tradeMoney - localItem.discountMoney - localItem.tixianfeiMoney;
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

function convertToLocal(netLog,customerInfo) {
    var time ;
    var dayStr =netLog.tdate;
    var timeStr = netLog.stime;
    var minute = timeStr.substr(2, 2);
    if (minute > 59) minute = 59;
    var second= timeStr.substr(4, 2);
    if (second > 59) second = 59;
    time = dayStr.substr(0, 4) + '-' + dayStr.substr(4, 2) + '-' + dayStr.substr(6, 2) + " " + timeStr.substr(0, 2) + ":" + minute + ":" + second;
    //交易结果代码
    var resultCode = netLog.rspcode;
    //if(!resultCode){ throw "交易结果代码获取方式有变！"; }
    var money = netLog.amt;
    var isValid = 0;
    if(netLog.trname.indexOf('冲正')> -1){ money = -1 * money;isValid=1;}
    else if(netLog.trname=="消费") isValid=1;
    if(netLog.rspcode != '00') isValid = 0;	
    //alert(resultCode+netLog.trname);
    return { terminal: netLog.tid, time: time, tradeName: netLog.trname,
     tradeMoney: money, 
     results: netLog.rspmsg,resultCode: resultCode, faren: customerInfo.faren, timeStr: netLog.tdate+" "+netLog.stime,isValid:isValid};
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
      var newRow= $('<tr><td>'+ item.time +'</td><td>'+ item.tradeName +'</td><td>'+ item.tradeMoney +'</td><td>'+ item.results +'</td></tr>');
      viewTableBody.prepend(newRow);
	
}
