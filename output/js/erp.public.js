(function ($) {
$.ajaxSetup({async:false});

$.fn.entityTable=function(options){
	//queryTables用来多表关联查询
	var settings = $.extend({table:null,queryTables:null,queryCols:null,primary:"id",markDelete:true,markColName:"state",onRowCreated:null}, options);
	if(!settings.table) {alert("table name not define"); return;}
	if(!settings.editCols) {alert("editCols not define"); return;}
	if(!settings.displayName) settings.displayName = settings.table;
	if(this[0].tagName!="TABLE") {alert("not table element");return;}
	if(!settings.queryTables) settings.queryTables =  settings.table;
	if(!settings.queryCols) settings.queryCols =  "*";
	var editItemLi;
	_jthis=$(this);
	_modal = $('<div class="modal fade" style="width:auto" id="product_editor" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">'+
	'<div class="modal-dialog">'+
      '<div class="modal-content">'+
         '<div class="modal-header">'+
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times; </button>'+
            '<h4 class="modal-title" id="myModalLabel"> 模态框（Modal）标题</h4>'+
         '</div>'+
		 '<div class="modal-body"><div class="moal-content-groups"></div>'+
		 '<div class="modal-footer">'+
            '<button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>'+
            '<button type="button" class="btn btn-primary">提交更改</button>'+
         '</div>'+
	'</div></div></div>').appendTo(document.body);
	var mould_edit = _modal.find(".moal-content-groups");
	
	this.Query=function(filter, selectedCode, load){	
		return eventHandle.LoadTemplateList(filter, selectedCode, load);
	}
	
	this.ShowCreateDialog = function(){
		editItemLi=null;
		_modal.modal("show");
		eventHandle.LoadTemplateDetails();
	}
	
	var eventHandle={
		LoadTemplateList:function(filter, selectedCode, load) {
            var data = null;
             whereSql ="";
             if(settings.markDelete) whereSql= settings.markColName+" > 0"; 
             if(filter){if(whereSql.length==0) whereSql = filter; else  whereSql+=" and "+ filter;}			 
             if(selectedCode && selectedCode.length > 0) { whereSql= settings.primary +"='"+ selectedCode +"'"  }             
            var products = clientEx.execQuery(settings.queryCols,settings.queryTables,whereSql,settings.primary +" desc ")
            if (load) {
                mould_list_con = _jthis.find("tbody").empty();
                $.each(products, function () {
                    eventHandle.CreateAMould(this);
                })
            }
            return products;
        },
	     LoadTemplateDetails:function() {
			 product = null;
			 if(editItemLi){
				var templateCode = editItemLi.attr("code");
				var data = eventHandle.LoadTemplateList("", templateCode, false);
				if(data.length ==0) { showTip("找不到编号为"+ templateCode +"的商品数据！");return; }
				product = data[0];
			 }
            var mould_edit = _modal.find(".modal-body") //$("#mould_edit_temp").clone().show().appendTo(editItemLi);
            //var con = mould_edit.find(".mould_this_con").empty();
			$.each(settings.editCols,function(){
				$(this.fields).each(function(){
					switch(this.type){
						case "pic":
							break;
						default:							
							if(!product){
								mould_edit.find("#"+this.colName).val("").parent().attr("val","");								
							}else{
								mould_edit.find("#"+this.colName).val(product[this.colName]).parent().attr("val",product[this.colName]);
							} 
							break;
					}
					
				});
			});
			

        },
		CreateAMould:function(data, insertAtFirst) {
            mould_list=$('<tr code =' + data.id + ' ></tr>'); 
			_jthis.find("thead th").each(function(){
				switch($(this).attr("colName")){
					case "#":
						mould_list.append("<td>"+ _jthis.find("tr").length +"</td>");
					break;
					case "#action":
						var actionTd= $('<td><a class="edit">编辑<a>  <a class="delete">删除</a></td>');
						actionTd.find(".edit").click(function(){$(this).closest("tr").trigger("dblclick");});
						actionTd.find(".delete").click(function(){
							if(confirm("您确认要删除吗？")){
								var id = $(this).closest("tr").attr("code");								
								var delJson;								
								
								if(settings.markDelete) {
									var fields = {};
									fields[settings.markColName]=0;
									delJson =[{ table:settings.table ,action: 1, fields:fields, where:settings.primary+"="+id }]
									 
								}
								else delJson =[{ table:settings.table ,action: 2 ,where:settings.primary+"="+id }]
								if(clientEx.execDb(delJson)){
									eventHandle.LoadTemplateList("", "", true);
								}
							}
						})
						mould_list.append(actionTd);
					break;
					default:
					mould_list.append("<td></td>");
					break;
				}
			});
			
			eventHandle.setListItemText(mould_list,data);
            if (insertAtFirst) mould_list_con.prepend(mould_list);
            else mould_list.appendTo(mould_list_con);
            mould_list.dblclick(function(){
				editItemLi=$(this);
                _modal.modal("show");
				eventHandle.LoadTemplateDetails();
            })            
            if(settings.onRowCreated) settings.onRowCreated(mould_list);
            return mould_list;
        },
		setListItemText:function(tr,data){
			_jthis.find("thead th").each(function(i){
				var colName=$(this).attr("colName");
				var td = $(tr.children()[i]);
				switch(colName){
					case "#":						
						break;
					case "#action":
						break;
					default:
						td.text( data[colName] );
						break;
				}
			});
		 
		},
		editorOnLeave:function(){
			var editItem = $(this).parent();
                
                    errStr = null;
                    var inputText = $(this).val().trim();
                    if (inputText == "") errStr="请数据一个名称！";
                    else if (inputText.length > 50) errStr="名称不能这么长,50个字以内！"
                    if(errStr){
                        showTipNew(errStr);
                        highlight(editItem);
                        $(this).focus();
                        return;
                    }
                    $(this).attr("EditFlag", 1);
                    editItem.attr("val", inputText);
                
		},
		SpanWitEvent:function(sender, placeholder) {
            if (sender.hasClass("editor")) { return; }
            var oldVal = sender.attr("val");
            if (!oldVal) oldVal = sender.text();
            var inputObj = $('<input class="modal-editor-input"  type="text"  value="' + oldVal + '" placeholder="' + (placeholder ? placeholder : '请输入名称！') + '" >');
            var reWidth = sender.attr("editorWidth");
            if (reWidth != null) { inputObj.css("width", reWidth); }
            sender.addClass("editor").empty().append(inputObj);
            inputObj.blur(function () {
                var editItem = $(this).parent();
                if (editItem.hasClass("editor")) {
                    errStr = null;
                    var inputText = $(this).val().trim();
                    if (inputText == "") errStr="请数据一个名称！";
                    else if (inputText.length > 50) errStr="名称不能这么长,50个字以内！"
                    if(errStr){
                        showTipNew(errStr);
                        highlight(editItem);
                        $(this).focus();
                        return;
                    }
                    editItem.attr("EditFlag", 1);
                    $(this).remove();
                    $(editItem).attr("val", inputText).removeClass("editor").html(inputText);
                }
            });
            inputObj.focus().select();
        },
		SaveEdit:function(){		 
			var editInput = _modal.find(".editor_row input[EditFlag=1]");
			//if (editInput.length == 0) { return false; }
			var newEntity={};
			//newEntity[settings.primary] = editItemLi.attr("code");
			editInput.each(function(){
				newEntity[this.id]= this.value;
			})
			editInput = _modal.find(".picbox[EditFlag=1]");
			editInput.each(function(){
				newEntity[this.id]= this.getAttribute("img");
			})
			var hasVal = false;
			for(var i in newEntity){hasVal = true;break;}
			if(!hasVal){return false;}
			if(!editItemLi) {
				newEntity[settings.primary] =  clientEx.getNexVal(settings.table) ;
				if(settings.defaults) newEntity= $.extend(newEntity,settings.defaults);
				if(settings.markDelete && !newEntity[settings.markColName]){
					newEntity[settings.markColName] =1;
				}
				var updateJson=[{ table:settings.table ,action: 0 ,fields:newEntity}];
			}
			else{
				var updateJson=[{ table:settings.table ,action: 1 ,fields:newEntity,where: settings.primary+" ="+ editItemLi.attr("code") }]
			}			
			return clientEx.execDb(updateJson);
			
		}
	}
	
	$.each(settings.editCols,function(){
		var group = $('<div class="panel panel-info">'+
            '<div class="panel-heading"><h3 class="panel-title">'+this.group+'</h3></div>'+
            '<div class="panel-body"></div>'+
          '</div>').appendTo(mould_edit).find(".panel-body");
		$(this.fields).each(function(){
			if(this.type==null) this.type="text";
			switch(this.type){
				case "text":
					var editorRow=$('<div class="editor_row " >'+                                
                                    '<label>'+this.display+'：</label>'+
                                    '<input id="'+ this.colName +'" />'+                                
                            '</div>').appendTo(group);
					editorRow.find("input").blur(eventHandle.editorOnLeave);
				break;
				case "date":
				break;
				case "pic":
					var str='<div class="row picbox" id="'+this.colName+'">';
					if(!this.number)this.number=1;
					var colsm='col-xs-'+Math.floor(12/this.number);
					for(var i=0;i<this.number;i++){	
						//str+='<span class="'+colsm+' img-con"><img class="img-responsive img-rounded" src="images/chooseImg.jpg" /><cite >'+this.display+(i+1)+'<cite></span>';
						str+='<div class="'+colsm+' img-con thumbnail"><img class="carousel-inner img-rounded" src="images/chooseImg.jpg" /><cite >'+this.display+(i+1)+'<cite></div>';
					}		
					str+='</div>';
					var jPic=$(str).appendTo(group);
					jPic.find("."+colsm).click(function(){						
						var file= clientEx.openPictureDialog();
						if(file){
							file="file:///"+file.replace("\\","\\");
							$(this).find("img").attr("src",file).attr("hasVal",1);
							var picStr="";
							var imgCon=$(this).closest(".row");
							imgCon.find("img[hasVal=1]").each(function(){picStr+=","+ this.src; });
							picStr= picStr.substring(1);
							//alert(picStr);
							imgCon.attr("img",picStr).attr("EditFlag",1);
							
						}
					})
				break;
			}
		})
		
	})
	

	
	$(this).each(function(){
		
		_modal.modal("hide").find('.btn-primary').click(function(){			
			_modal.modal("hide");
			if(eventHandle.SaveEdit()==false) return;
			//更新列表	
			if(editItemLi){
				product = eventHandle.LoadTemplateList("",editItemLi.attr("code") , false)[0];			
				eventHandle.setListItemText(editItemLi,product);
			}else{
				eventHandle.LoadTemplateList("","",true);
			}
		});
		//eventHandle.LoadTemplateList("","",true);
	})
	
	return this;
}

//修复模态框不居中的bug
$.fn.modal.Constructor.prototype.adjustDialog = function () {
    var modalIsOverflowing = this.$element[0].scrollHeight > document.documentElement.clientHeight

    this.$element.css({
      paddingLeft:  !this.bodyIsOverflowing && modalIsOverflowing ? this.scrollbarWidth : '',
      paddingRight: this.bodyIsOverflowing && !modalIsOverflowing ? this.scrollbarWidth : ''
    })
    // 是弹出框居中。。。
    var $modal_dialog = $(this.$element[0]).find('.modal-dialog');
    var m_top = ( $(window).height() - $modal_dialog.height() )/2;
    $modal_dialog.css({'margin': m_top + 'px auto'});
  }

})(jQuery);


var clientEx;

var clientPc = {    
getViewHTML: function (fileName) {
		return window.external.GetView(fileName);
	},
	getNexVal:function(table){
//		if(!window.external.getNextVal) return null;
		return window.external.getNextVal(table);
	},
	execQuery: function (fields,table,where,order) {		
		if(where=="") where=null;
		var jsonStr = window.external.execQuery(table,fields,where,order);
		return eval(jsonStr);
	},execDb: function (jsonArr) {
		//if(!window.external.execDb) return null;
		var fieldNameArr=[];
		var fieldValueArr=[];
		for(var i=0;i<jsonArr.length;i++){
			var fields=jsonArr[i].fields;
			for(var name in fields){
				fieldNameArr[fieldNameArr.length]= name;
				fieldValueArr[fieldValueArr.length]= fields[name];
			}

			jsonArr[i].fields = fieldNameArr;
			jsonArr[i].values = fieldValueArr;
		}

		var jsonArrStr = JSON.stringify(jsonArr);
		return window.external.execDb(jsonArrStr);
	},openPictureDialog:function(){
		str= window.external.uploadFile();
		return str;
	},getTableName:function(table){
		//if(!window.external.getTableName)return null;
		return window.external.getTableName(table);
	}        
}

var clientPy = {
	getViewHTML: function (fileName) {
		return TeaErp.GetView(fileName);
	},
	getNexVal:function(table){
		return TeaErp.getNextVal(table);
	},
	execQuery: function (fields,table,where,order) {
		if(where=="") where=null;
		var jsonStr = TeaErp.execQuery(table,fields,where,order);
		return eval(jsonStr);
	},execDb: function (jsonArr) {
		var jsonArrStr = JSON.stringify(jsonArr);
		return TeaErp.execDb(jsonArrStr);
	},openPictureDialog:function(){
		str= TeaErp.uploadFile();
		return str;
	},getTableName:function(table){
		return TeaErp.getTableName(table);
	}
}


var clientHttp = {
    serviceUrl: "../services/index.php",
	getViewHTML: function (fileName) {
		var html = "没找到";
		$.get("views/"+fileName,null,function(res){
			html = res;
		});
		return html;
	},
	getNexVal: function (table) {
	    var nextVal = 0;
	    $.get(clientHttp.serviceUrl + "?method=GetNextVal", { table: table }, function (res) {
	        if (res.error) { showTip(res.error); return; }
	        nextVal = res.result;
	    });
	},
	execQuery: function (fields,table,where,order) {
	    var jsonStr = 0;
	    $.getJSON(clientHttp.serviceUrl + "?method=QuerySqlite", { table: table, "columns": fields,"where":where,"orderby":order }, function (res) {
	        if (res.error) { showTip(res.error); return;}
	        jsonStr = res.result;
	    });
		return eval(jsonStr);
	},execDb: function (jsonArr) {
	    var jsonArrStr = JSON.stringify(jsonArr);
	    var returnVal;
	    $.post(clientHttp.serviceUrl + "?method=ExecuteSqlite", { data: jsonArrStr }, function (res) {
	        if (res.error) { showTip(res.error); return; }
	        returnVal = res.result;
	    },"json");
	    return returnVal;
	},openPictureDialog:function(){
		str= TeaErp.uploadFile();
		return str;
	},getTableName:function(table){
	    var tableName = 0;
	    $.get(clientHttp.serviceUrl + "?method=GetTableName", { table: table }, function (res) {
	        if (res.error) { showTip(res.error); return; }
	        tableName = res.result;
	    });
	    return tableName;
	}
}


var clientAndroid = {
	
}

if (clientEx == null) clientEx = clientPc;

//日期扩展
Date.prototype.AddMonths = function (n) {
    if (n == 0) return this;
    var dtstr = this.Format("yyyy-MM-dd");
    var s = dtstr.split("-");
    var yy = parseInt(s[0]); var mm = parseInt(s[1] - 1); var dd = parseInt(s[2]);
    var dt = new Date(yy, mm, dd);
    dt.setMonth(dt.getMonth() + n);
    if ((dt.getYear() * 12 + dt.getMonth()) > (yy * 12 + mm + n)) {
        dt = new Date(dt.getYear(), dt.getMonth(), 0);
    }
    return dt;

}

Date.prototype.AddDays = function (n) {
    return new Date(this.getTime() + n * 24 * 60 * 60 * 1000);
}


function strToDate(str) {
    if (str == null) { alert("要转换的日期为空"); return; }
    if (str.length < 18) str = str + " 00:00:00"
    var tempStrs = str.split(" ");
    var dateStrs = tempStrs[0].split("-");
    var year = parseInt(dateStrs[0], 10);
    var month = parseInt(dateStrs[1], 10) - 1;
    var day = parseInt(dateStrs[2], 10);
    var timeStrs = tempStrs[1].split(":");
    var hour = parseInt(timeStrs[0], 10);
    var minute = parseInt(timeStrs[1], 10);
    var second = parseInt(timeStrs[2], 10);
    var date = new Date(year, month, day, hour, minute, second);
    return date;
}

Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1,                 //月份 
        "d+": this.getDate(),                    //日 
        "h+": this.getHours(),                   //小时 
        "m+": this.getMinutes(),                 //分 
        "s+": this.getSeconds(),                 //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds()             //毫秒 
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

//字符串扩展
String.prototype.replaceAll = function (str1, str2) {
    return this.replace(new RegExp(str1, 'gm'), str2);
}

String.prototype.trim = function () {
    return this.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
}

function showTip(str) {
    alert(str);
}

function showTipNew(str) {
    alert(str);
}


var hightlightObj = null;
function highlight(obj, times) {
    hightlightObj = obj;
    if (!times) times = 4
    hightlightObj.toggleClass("hightlight_border");
    times = times - 1;
    if (times > 0) setTimeout("highlight(hightlightObj," + times + ")", 250);
}
