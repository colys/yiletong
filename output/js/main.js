function parseTableJson() {    
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
	    	var existCount = clientEx.execQuery("count(0) cc","transaction_log","terminal='"+ item.tid +"' and time='"+item.tdate+"' and trade_name='"+item.trname+"' and money='"+ item.amt +"' ",null);
	    	if(existCount[0].cc > 0) continue;
	    	var localItem = convertToLocal(item);
	    	var inserDBJson=[{ table:"transaction_log" ,action: 0 ,fields:localItem}];
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
	var date= (new Date()).Format("yyyyMMdd");
	var where= " time between '"+date+" 000000' and '"+date+" 235959' ";
	var todayAllData=clientEx.execQuery("*","transaction_log",where,null);
	if(todayAllData==null || todayAllData.length ==0) return;
	for(var i=0;i< todayAllData.length;i++){
		addTerminalView(todayAllData[i]);
	}
}

function convertToLocal(netLog){
	return { terminal: netLog.tid ,time:netLog.tdate,trade_name:netLog.trname, money:netLog.amt ,results:netLog.rap, faren:netLog.lpName };
}

function addTerminalView(item){

    var tview=$("#termianls").find("#view_"+ item.terminal);

	if(tview.length ==0){		
		 tview=$('<div class="col-sm-4">'+
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
      	var viewTable=$('<table><thead><th>时间</th><th>交易名称</th><th>金额</th><th>结果</th></thead></table>').appendTo(panelBody);
      	viewTableBody=$('<tbody></tbody>').appendTo(viewTable);
      }
      var newRow= $('<tr><td>'+ item.time +'</td><td>'+ item.trade_name +'</td><td>'+ item.money +'</td><td>'+ item.results +'</td></tr>').appendTo(viewTableBody);
	
}



Date.prototype.Format = function(fmt)   
{ //author: meizz   
  var o = {   
    "M+" : this.getMonth()+1,                 //月份   
    "d+" : this.getDate(),                    //日   
    "h+" : this.getHours(),                   //小时   
    "m+" : this.getMinutes(),                 //分   
    "s+" : this.getSeconds(),                 //秒   
    "q+" : Math.floor((this.getMonth()+3)/3), //季度   
    "S"  : this.getMilliseconds()             //毫秒   
  };   
  if(/(y+)/.test(fmt))   
    fmt=fmt.replace(RegExp.$1, (this.getFullYear()+"").substr(4 - RegExp.$1.length));   
  for(var k in o)   
    if(new RegExp("("+ k +")").test(fmt))   
  fmt = fmt.replace(RegExp.$1, (RegExp.$1.length==1) ? (o[k]) : (("00"+ o[k]).substr((""+ o[k]).length)));   
  return fmt;   
} 

$(function(){
	init();
});