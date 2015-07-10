﻿function parseTableJson() {    
    var json = [];
    var status_list = $("#status_list").empty();
    $("#hidden_div table tbody tr").each(function () {
        var item = {};
        $(this).find("td").each(function () {
            var fieldName = this.getAttribute("field");
            item[fieldName] = this.innerText;
        });
        json[json.length] = item;

    })
    if (json.length == 0) {
        status_list.html("没有刷卡数据！");
    }
    var newJson =[];
    try{
	    for(var i=0;i<json.length;i++){
	    	var item =json[i];
	    	//判断记录是否存在,存在的话忽略,不存在则插入
	    	var existCount = clientEx.execQuery("count(0) cc","transactionLogs","terminal='"+ item.tid +"' and time='"+item.tdate+"' and tradeName='"+item.trname+"' and tradeMoney="+ item.amt +" ",null);
	    	if(existCount[0].cc > 0) continue;
	    	var localItem = convertToLocal(item);
	    	localItem.id= clientEx.getNexVal("transactionLogs");
	    	var inserDBJson=[{ table:"transactionLogs" ,action: 0 ,fields:localItem}];
	    	clientEx.execDb(inserDBJson);
	    	newJson[newJson.length]=item;
	    	$("<div>" + item.tdate + " , " + item.trname + " : " + item.amt + "</div>").appendTo(status_list);
	    	addTerminalView(localItem);
	    }
	}catch(e){
		status_list.html("保存数据时发生异常,请赶快处理！"+ e.message);
	}
	if(newJson.length > 0){
		//触发通知
	}
    return JSON.stringify(newJson);
}

function setStatus(str) {
    $("#status").html(str);
}


function init(){	
	var today =new Date();
	var date= today.Format("yyyyMMdd");
	var prevdate=  today.AddDays(-1).Format("yyyyMMdd");
	var where= " time between '"+prevdate+" 230000' and '"+date+" 999999' ";//前一天11到今天11点
	var todayAllData=clientEx.execQuery("*","transactionLogs",where,null);
	setStatus("未开启监控！");
	if(todayAllData==null || todayAllData.length ==0) return;
	for(var i=0;i< todayAllData.length;i++){
		addTerminalView(todayAllData[i]);
	}
}

function convertToLocal(netLog){
	return { terminal: netLog.tid ,time:netLog.tdate,tradeName:netLog.trname, tradeMoney:netLog.amt ,results:netLog.rap, faren:netLog.lpName };
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

$.customers = function (options) {
    var settings = $.extend({ viewOnly: null, reportType: 1, defaultSize: 10, viewMore: 5, personCode: null }, options);
    var entityTable_ops = {
        table: "customers"
        , editCols: [
            {
                group: "基础属性", fields: [
                   { display: '终端', colName: "terminal" },
                   { display: '法人', colName: "faren" },
                   { display: '商户号', colName: "shanghuNo" },
                   { display: '商户名称', colName: "shanghuName" },
                   { display: '联系电话', colName: "tel" },
                   { display: 'MCC码', colName: "mcc" },
                   { display: '扣率', colName: "discount" },
                   { display: '提现费', colName: "tixianfei" },
                   { display: '封顶', colName: "fengding" }
                ]
            }]
        , defaults: { status: 1 }
        , markDelete: true
        , markColName: "status"
    };
    var entityTable = $("#listTable").entityTable(entityTable_ops);
    entityTable.Query(null, null, true);


    $("#addnew").click(function () {
        entityTable.ShowCreateDialog();
    })

    return this;
}