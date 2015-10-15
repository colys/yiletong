/*2015-08-22 18:10*/
// call a immediate funciton，prevent global namespace from being polluted.
(function () {
    // 这个initializing变量用于标识当前是否处于类的初始创建阶段，下面会继续详述
    var initializing = false,
    // 这是一个技巧性的写法，用于检测当前环境下函数是否能够序列化
    // 附一篇讨论函数序列化的文章：http://www.cnblogs.com/ziyunfei/archive/2012/12/04/2799603.html
    // superPattern引用一个正则对象，该对象用于验证被验证函数中是否有使用_super方法
        superPattern = /xyz/.test(function () { xyz; }) ? /\b_super\b/ : /.*/;

    Object.subClass = function (properties) {
        // 当前对象（父类）的原型对象
        var _super = this.prototype;

        // initializing = true表示当前处于类的初始创建阶段。
        // this构造函数里会判断initializing的状态，如果为false则不执行Init方法。
        // 事实上这也是非常需要的，因为在这个时候，我们需要的只是一个干净的虚构的构造函数，完全不需要其执行init函数，以避免污染。init方法只有在当前类被实例化的时候才需要被执行，而当前正执行继承行为，不应该执行Init方法。
        initializing = true;
        // 当前对象（父类）的一个实例对象
        var proto = new this();
        // 初始创建阶段完成，置initializing为false
        initializing = false;

        // 在properties里提供的属性，作为当前对象（父类）实例的公共属性，供其子类实例共享；
        // 在properties里提供的方法，作为当前对象（父类）实例的公共方法，供其子类实例共享。
        for (var name in properties) {
            proto[name] = typeof properties[name] == 'function' && //检测当前提供的是否为函数
                          typeof _super[name] == 'function' && //检测当前提供的函数名是否已经存在于父类的原型对象中，如果是，则需要下面的操作，以保证父类中的方法不会被覆盖且可以以某种方式被调用，如果否，则直接将该函数赋值为父类实例的方法
                          superPattern.test(properties[name]) ? //检测当前提供的函数内是否使用了_super方法，如果有使用_super方法，则需要下面的操作，以保证父类中的方法不会被覆盖且可以以某种方式被调用，如果没有用到_super方法，则直接将该函数赋值为父类实例的方法，即使父类原型中已经拥有同名方法（覆盖）

                // 使用一个马上执行的函数，返回一个闭包，这样每个闭包引用的都是各自的name和fn。
                (function (name, fn) {
                    return function () {
                        // 首先将执行方法的当前对象（子类的实例化对象）的_super属性保存到tmp变量里。
                        // 这是非常必要的， 因为this永远指向当前正在被调用的对象。
                        // 当C继承B，B继承A，而A\B\C均有一个dance方法且B\C的dance方法均使用了this._super来引用各自父类的方法时，下面这句操作就显得非常重要了。它使得在方法调用时，this._super永远指向“当前类”的父类的原型中的同名方法，从而避免this._super被随便改写。
                        var tmp = this._super;

                        // 然后将父类的原型中的同名方法赋值给this._super，以便子类的实例化对象可以在其执行name方法时通过this._super使用对应的父类原型中已经存在的方法
                        this._super = _super[name];

                        // 执行创建子类时提供的函数，并通过arguments传入参数
                        var ret = fn.apply(this, arguments);

                        // 将tmp里保存的_super属性重新赋值回this._super中
                        this._super = tmp;

                        // 返回函数的执行结果
                        return ret;
                    };
                })(name, properties[name]) :
                properties[name];
        }

        // 内部定义个名叫Class的类，构造函数内部只有一个操作：执行当前对象中可能存在的init方法
        // 这样做的原因：新建一个类（闭包），可以防止很多干扰（详细可对比JS高级设计第三版）
        function Class() {
            // 如果不是正在实现继承，并且当前类的init方法存在，则执行init方法
            // 每当subClass方法执行完毕后，都会返回这个Class构造函数，当用户使用new 方法时，就会执行这里面的操作
            // 本质：每次调用subClass都新建一个类（闭包）
            if (!initializing && this.init) {
                // 这是子类的初始化方法，里面可以定义子类的私有属性，公共属性请在上方所述处添加
                this.init.apply(this, arguments);
            }
        }

        // 重写Class构造函数的prototype，使其不再指向了Class原生的原型对象，而是指向了proto，即当前对象（类）的一个实例
        // 本质：一个类的原型是另一个类的实例（继承）
        Class.prototype = proto;
        // 为什么要重写Class的构造函数？因为这个Class函数，它原来的constructor指向的是Function对象，这里修正它的指向，使其指向自己。
        Class.constructor = Class;
        // 就是这个操作，使得每次调用subClass都会新生命的Class对象，也拥有subClass方法，可以继续被继承下去
        // 本质：使得每次继承的子类都拥有被继承的能力
        Class.subClass = arguments.callee;
        // 返回这个内部新定义的构造函数（闭包）
        return Class;
    };
})();



(function ($) {
$.ajaxSetup({async:false});


/* 
*单表维护插件
*triggerQuery 载入后自动查询
*submitUnChange 未修改的字段是否也写入json提交保存
*noSaveItemWarinig:没有任何修改点保存，是否要弹出提示
*defaults默认添加的查询与保存字段json{}
*事件:onRowCreated，onDetailLoad
*/
$.fn.entityTable=function(options){
    
    var settings = $.extend({triggerQuery:false,submitUnChange:false,noSaveItemWarinig:true, pageSize: 15, pagger: "#pagger" , title: null, table: null, orderby: null, permission: "undefind", queryCols: null, primary: "id", defaults: null, onRowCreated: null, onDetailLoad: null }, options);
    var total = 0;
    var pageIndex = settings.pagger ? 1 : null;
	if(!settings.table) {alert("table name not define"); return;}
	if(!settings.editCols) {alert("editCols not define"); return;}
	if(!settings.displayName) settings.displayName = settings.table;
	if (!this.length || this[0].tagName != "TABLE") { alert(this.selector + " is not exists or not a table element"); return; }
	if (!settings.queryCols) settings.queryCols = "*";
	if (!settings.title) settings.title = settings.table;
	var editItemLi;
	var lastFilter = null;
	var firstQuery = true;
	_jthis=$(this);
	_modal = $('<div class="modal fade" style="width:auto" id="product_editor" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">'+
	'<div class="modal-dialog">'+
      '<div class="modal-content">'+
         '<div class="modal-header">'+
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times; </button>'+
            '<h4 class="modal-title" id="myModalLabel"> '+ settings.title +'详情</h4>'+
         '</div>'+
		 '<div class="modal-body"><div class="moal-content-groups"></div>'+
		 '<div class="modal-footer">'+
            '<button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>'+
            '<button type="button" class="btn btn-primary">提交更改</button>'+
         '</div>'+
	'</div></div></div>').appendTo(document.body);
	var mould_edit = _modal.find(".moal-content-groups");
	
	this.Query = function (filter, selectedCode, load) {
	    firstQuery = true;
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
            whereSql = {};
            lastFilter = filter;
            if (filter) {
                whereSql = $.extend(whereSql, filter);
                //if (whereSql.length == 0) whereSql = filter; else whereSql += " and " + filter;
            }
            if (settings.defaults) whereSql = $.extend(whereSql, settings.defaults);
            if (selectedCode && selectedCode.length > 0) { whereSql = {}; whereSql[settings.primary] = selectedCode; }
		    //permission, fields, table, where, order
            var orderbyStr = null;
            if(settings.orderby) orderbyStr = settings.orderby;
            //else orderbyStr= settings.primary + " desc ";
            var queryResult = clientEx.execQuery(settings.permission, settings.queryCols, settings.table, whereSql, orderbyStr,pageIndex,settings.pageSize);
            if (!queryResult || !queryResult.result) return;            
            if (load) {
                if (queryResult.total > -1) total = Math.ceil(queryResult.total / settings.pageSize);
                
                mould_list_con = _jthis.find("tbody").empty();
                $.each(queryResult.result, function () {
                    eventHandle.CreateAMould(this);
                })
                if (settings.pagger && total > 1) {//分页
                    if (typeof ($.fn.bootstrapPaginator) == "undefined") throw ("bootstrap paginator没有引用!")
                    if ($(settings.pagger).length == 0) {
                        settings.pagger = "#autogene_pagger";
                        $("<div id='autogene_pagger' ></div>").appendTo(_jthis.parent());
                    }
                    if (firstQuery) {
                        $(settings.pagger).bootstrapPaginator({
                            currentPage: pageIndex, totalPages: total, numberOfPages: 5,alignment:"right",
                            itemTexts: function (type, page, current) {
                                switch (type) {
                                    case "first":
                                        return "第一页";
                                    case "prev":
                                        return "上一页";
                                    case "next":
                                        return "下一页";
                                    case "last":
                                        return "最后一页";
                                    case "page":
                                        return  page;
                                }
                            },
                            onPageChanged: function (event, oldPage, newPage) {
                                pageIndex = newPage;
                                eventHandle.LoadTemplateList(lastFilter, null, true);
                            }
                        });
                    }
                }
                firstQuery = false;
            }
            return queryResult;
        },
	     LoadTemplateDetails:function() {
			 product = null;
			 if(editItemLi){
			     var templateCode = editItemLi.attr("code");
			     var product = clientEx.getObject(settings.permission, settings.table, templateCode);
			     if(!product) { showTip("找不到编号为"+ templateCode +"的商品数据！");return; }				
			 }
            var mould_edit = _modal.find(".modal-body") //$("#mould_edit_temp").clone().show().appendTo(editItemLi);
            //var con = mould_edit.find(".mould_this_con").empty();
			$.each(settings.editCols,function(){
			    $(this.fields).each(function () {		     

			        var editor = GetEditorHandle(this.type);
			        var val;
			        if (product && product[this.colName]!=null) {
			            val = product[this.colName];
			        } else val="";
			        var sender =editor.getByColName(this.colName);
			        editor.val(sender, val);
			        editor.attrOld(sender,val);
					
				});
			});
			if (settings.onDetailLoad) settings.onDetailLoad(product);

        },
	     CreateAMould: function (data, insertAtFirst) {
	         if (!data[settings.primary]) throw ("主键值为空，如果主键不是ID请在options中设置primary的值，如果是则检查服务端实体主键配置");
            mould_list=$('<tr code =' + data[settings.primary] + ' ></tr>'); 
			_jthis.find("thead th").each(function(){
				switch($(this).attr("colName")){
					case "#":
						mould_list.append("<td>"+ _jthis.find("tr").length +"</td>");
					break;
					case "#action":
					    var actionTd = $('<td class="action"><a class="edit">编辑</a><a class="delete">删除</a></td>');
						actionTd.find(".edit").click(function(){$(this).closest("tr").trigger("dblclick");});
						actionTd.find(".delete").click(function(){
							if(confirm("您确认要删除吗？")){
								var id = $(this).closest("tr").attr("code");									
								if (clientEx.removeObject(settings.permission,settings.table,id)!=null) {
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
            if (settings.editCols && settings.editCols.length > 0) {
                mould_list.dblclick(function () {
                    editItemLi = $(this);
                    _modal.modal("show");
                    eventHandle.LoadTemplateDetails();
                })
            }
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
		SaveEdit: function () {
		    var errorMsg = null;
		    var newEntity = {};
		    $.each(settings.editCols, function () {
		        $(this.fields).each(function () {
		            var editor = GetEditorHandle(this.type);
		            var sender = editor.getByColName(this.colName);
		            var value = editor.val(sender);                    
		            if(this.type!='checkbox'&& this.requre &&( value==null || value=="" )){
		                errorMsg = this.display + "为空！";
		            }
		            else if (settings.submitUnChange || editor.hasChange(sender)) newEntity[this.colName] = value;
		        });
		    });
		    if (errorMsg) { showTip(errorMsg); return false;}
			if(!editItemLi) {
				//newEntity[settings.primary] =  clientEx.getNexVal(settings.table) ;
				if(settings.defaults) newEntity= $.extend(newEntity,settings.defaults);				
				//var updateJson=[{ table:settings.table ,action: 0 ,fields:newEntity}];
			}
			else {
			    newEntity[settings.primary] = editItemLi.attr("code");

				//var updateJson=[{ table:settings.table ,action: 1 ,fields:newEntity,where: settings.primary+" ="+ editItemLi.attr("code") }]
			}
			var n = 0;
			for (var i in newEntity) {
			    n++;
			}
			if (n < 2) { if(settings.noSaveItemWarinig) showTip("没有任何的修改，不需要保存"); return;}
			return clientEx.saveObject(settings.permission, settings.table, newEntity);
			
		}
	}
	
	$.each(settings.editCols,function(){
		var group = $('<div class="panel panel-info">'+
            '<div class="panel-heading"><h3 class="panel-title">'+this.group+'</h3></div>'+
            '<div class="panel-body '+ this.groupClass +'"></div>'+
          '</div>').appendTo(mould_edit).find(".panel-body");
		$(this.fields).each(function () {
		    if (this.type == null) this.type = "text";

		    var editor = GetEditorHandle(this.type);		    
		    var editorJHtml = editor.create(this);
		    group.append(editorJHtml);
		    //switch(this.type){
		    //	case "text":
		    //		var editorRow=$('<div class="editor_row " >'+                                
		    //                        '<label>'+this.display+'：</label>'+
		    //                        '<input id="'+ this.colName +'" />'+                                
		    //                '</div>').appendTo(group);
		    //		//editorRow.find("input").blur(eventHandle.editorOnLeave);
		    //	break;
		    //	case "date":
		    //	break;
		    //	case "pic":
		    //		var str='<div class="row picbox" id="'+this.colName+'">';
		    //		if(!this.number)this.number=1;
		    //		var colsm='col-xs-'+Math.floor(12/this.number);
		    //		for(var i=0;i<this.number;i++){	
		    //			//str+='<span class="'+colsm+' img-con"><img class="img-responsive img-rounded" src="images/chooseImg.jpg" /><cite >'+this.display+(i+1)+'<cite></span>';
		    //			str+='<div class="'+colsm+' img-con thumbnail"><img class="carousel-inner img-rounded" src="images/chooseImg.jpg" /><cite >'+this.display+(i+1)+'<cite></div>';
		    //		}		
		    //		str+='</div>';
		    //		var jPic=$(str).appendTo(group);
		    //		jPic.find("."+colsm).click(function(){						
		    //			var file= clientEx.openPictureDialog();
		    //			if(file){
		    //				file="file:///"+file.replace("\\","\\");
		    //				$(this).find("img").attr("src",file).attr("hasVal",1);
		    //				var picStr="";
		    //				var imgCon=$(this).closest(".row");
		    //				imgCon.find("img[hasVal=1]").each(function(){picStr+=","+ this.src; });
		    //				picStr= picStr.substring(1);
		    //				//alert(picStr);
		    //				imgCon.attr("img",picStr).attr("EditFlag",1);

		    //			}
		    //		})
		    //	break;
		    //}
		});
		
	})
	

	
	$(this).each(function(){
		
		_modal.modal("hide").find('.btn-primary').click(function(){						
		    if (eventHandle.SaveEdit() == false) return;
		    _modal.modal("hide");
			//更新列表	
		    if (editItemLi) {
		        var product = clientEx.getObject(settings.permission, settings.table, editItemLi.attr("code"));				
				eventHandle.setListItemText(editItemLi,product);
			}else{
				eventHandle.LoadTemplateList("","",true);
			}
		});
		//eventHandle.LoadTemplateList("","",true);
	})
	if (settings.triggerQuery) eventHandle.LoadTemplateList(null, null, true);//触发查询  
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

/*=================================表格=====================================*/
window.onresize = function () {
    var width = $(window).width();
    $('table thead th:hidden').show();
    $('table tbody td:hidden').show();
    var hideThs = $('table thead th').filter(function (index) { return $(this).attr("hide") > width; });
    hideThs.each(function () {
        var index = $(this).index();
        $(this).hide();
        $(this).parent().parent().next().find("tr td:eq(" + index + ")").hide();
    });
}


/*=================================编辑框定义=====================================*/
var inputEditors = [];
function GetEditorHandle(type) {
    if (inputEditors[type] != null) return inputEditors[type];
    var editor= null;
    switch (type) {        
        case "date":
            editor = new dateEditor();
            break;
        case "pic":
            editor = new imageEditor();
            break;
        case "dropdown":
            editor = new dropdownEditor();
            break;
        case "checkbox":
            editor = new checkboxEditor();
            break;
        default:
            editor = new normalEditor();
            break;
    }
    inputEditors[type] = editor;
    return editor;
}
var inputEditor = Object.subClass({
    
    create: function (field) { },
    getPlaceholder: function (field) {
        return field.placeholder ? field.placeholder : field.display;
    },
    val: function (sender, value) {
        if (value!=null) sender.find("input").val(value);
        else return sender.find("input").val();
    },
    getByColName: function (colName) {
        return $("#"+colName+"_con");
    },
    attrOld: function (sender, value) {
        sender.find("input").attr("old", value);
    },
    hasChange: function (sender) {
        var input= sender.find("input");
        return input.attr("old") != this.val(sender);
    }
 
});

var normalEditor = inputEditor.subClass({    
    create: function (field) {
        var sender= $('<div class="editor editor_normal" id="' + field.colName + '_con">' +
                                    '<label>' + field.display + '：</label>' +
                                    '<input id="' + field.colName + '" placeholder="' + this.getPlaceholder(field) + '" />' +
                            '</div>');
        return sender;
    }
    
});

var checkboxEditor = inputEditor.subClass({
    create: function (field) {
        var sender = $('<div class="editor editor_checkbox" id="' + field.colName + '_con">' +
                                    '<label>' + field.display + '：</label>' +
                                    '<input type="checkbox" id="' + field.colName + '" />' +
                            '</div>');
        return sender;
        
    },
    val: function (sender,value) {
        if (value!=null) { if (value == "") value = false; sender.find("input")[0].checked = value; }
        else return sender.find("input")[0].checked;
    },
    attrOld: function (sender, value) {
        sender.find("input").attr("old", value);
    }
   
});

var dropdownEditor = inputEditor.subClass({
    create: function (field) {
        var sender = $('<div class="editor editor_date" id="' + field.colName + '_con" >' +
                                  '<label>' + field.display + '：</label>' +
                 '</div>');
        var select = $(' <select class="selectpicker"  id="' + field.colName + '" > </select>').appendTo(sender);
        if (field.url) {
            $.getJSON(field.url, function (d) { field.data = d; });
        } else if (field.table) {
            var whereJson = {};
            if (field.filterField) { whereJson = $.extend(whereJson, field.filterField); }//可以设置下拉框查询表按字段过滤掉（json）            
            if (!field.displayField) throw ("displayField未定义");
            if (!field.valueField) throw ("valueField未定义");
            var displayArr = field.displayField.split(',');

            field.data = clientEx.execQuery(null, field.displayField + " ," + field.valueField + " value", field.table, whereJson, null);
            if (field.data) {
                for (var i = 0; i < field.data.length; i++) {
                    var str = field.data[i][displayArr[0]]
                    for (var j = 1; j < displayArr.length; j++) {
                        str += ' - ' + field.data[i][displayArr[j]];
                    }
                    field.data[i].text = str;
                }
            }
        }
        if (field.data) {
            $(field.data).each(function () {
                $('<option value="' + this.value + '">' + this.text + '</option>').appendTo(select);
            })
        }
        if (!field.requre) {
            //非必选
            select.prepend('<option  value="" >--请选择--</option>');
        }
        return sender;
    },
    val: function (sender, value) {
        if (value != null) {
            if (value == "") sender[0].selectedIndex = 0;
            else {
                sender.find("select").val(value);
            }
        }
        else {
            var val = sender.find("select").val();
            if (val == "") val = null;
            return val;
        }
    }, attrOld: function (sender, value) {
        sender.find("select").attr("old", value);
    },
    hasChange: function (sender) {
        var input = sender.find("select");
        return input.attr("old") != input.val();
    }
});

var imageEditor = inputEditor.subClass({   
    create: function (field) {     
        var str = '<div   id="' + field.colName + '_con"  class="img-con thumbnail"><img class="carousel-inner img-rounded" src="images/chooseImg.jpg" /><cite >' + field.display + '<cite></div>';
        var sender = $(str);
        return sender;
    },
    val: function (value) {

    }, attrOld: function (sender, value) {
        
    }
});

var dateEditor = inputEditor.subClass({
    create: function (field) {
        var sender = $('<div class="editor editor_date" id="' + field.colName + '_con" >' +
                                  '<label>' + field.display + '：</label>' +
                                  '<input type="text" class="form_datetime" placeholder="' + this.getPlaceholder(field) + '" id="' + field.colName + '" />' +
                          '</div>');
        this.sender.find(".form_datetime").datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });
        return sender;
    }
});


/*===========================clientEx===========================================*/
var clientEx;


var clientHttpNet = {
    serviceUrl: "http://192.168.211.114:8735/ServiceHost/ArgumentDiscuss/json/",
    getViewHTML: function (fileName) {
        var html = "没找到";
        $.get("views/" + fileName, null, function (res) {
            html = res;
        });
        return html;
    },
    getNexVal: function (table) {
        var nextVal = 0;
        $.get(clientHttpNet.serviceUrl + "GetNextVal", { table: table }, function (res) {
            if (res.error) { showTip(res.error); return; }
            nextVal = res.Result;
        });
        return nextVal;
    },
    getObject: function (permission, objName, primaryVal) {
        var data = "permission=" + permission + "&entityName=" + objName + "&primaryVal=" + primaryVal;
        var result;
        jQuery.getJSON(clientHttpNet.serviceUrl + "DoGetOne", data, function (d) {
            if (d.Message) { showTip(d.Message); return; }
            if (!d.Result) { throw ("DoGetOne返回结果为空，请检查返回值，必需包含Result和Message属性"); }
            result = d.Result;
        })
        return result;
    },
    saveObject: function (permission, objName, obj) {
        var data = { permission: permission, entityName: objName, entityJson:JSON.stringify(obj) };
        var result;
        jQuery.post(clientHttpNet.serviceUrl + "DoSave", data, function (d) {
            if (d.Message) { showTip(d.Message); return; }
            result = d.Result;
        },'json')
        return result;
    },
    removeObject: function (permission, objName, id) {
        var data = { permission: permission, entityName: objName, primaryID: id };
        var result;
        jQuery.post(clientHttpNet.serviceUrl + "DoDelete", data, function (d) {
            if (d.Message) { showTip(d.Message); return; }
            result = d.Result;
        }, 'json')
        return result;
    },
    execQuery: function (permission, fields, table, where, order,pageIndex,pageSize) {
        if (!permission) permission = "";
        if (!order) order = "";
        if (!fields) fields = "*";
        var data = "permission=" + permission + "&queryField=" + fields + "&entityName=" + table + "&whereField=" + JSON.stringify(where) + "&orderBy=" + order;
        if (pageIndex) {
            data+="&pageIndex="+pageIndex+"&pageSize="+ pageSize;
        }
        var val = { result: null, total: -1 };
        jQuery.getJSON(clientHttpNet.serviceUrl + "DoQuery", data, function (d) {
            if (d.Message) { showTip(d.Message); return; }
            if (!d.Result) { throw ("DoQuery返回结果为空，请检查返回值，必需包含Result和Message属性"); }
            val.result = d.Result;
            val.total = d.Total;
        })
        return pageIndex ? val : val.result;
    }, execDb: function (jsonArr) {
        var jsonArrStr = JSON.stringify(jsonArr);
        var returnVal;
        $.post(clientHttp.serviceUrl + "ExecuteSqlite", { data: jsonArrStr }, function (res) {
            if (res.error) { showTip(res.error); return; }
            returnVal = res.Result;
        }, "json");
        return returnVal;
    }, openPictureDialog: function () {
        str = TeaErp.uploadFile();
        return str;
    }, getTableName: function (table) {
        var tableName = 0;
        $.get(clientHttp.serviceUrl + "GetTableName", { table: table }, function (res) {
            if (res.error) { showTip(res.error); return; }
            tableName = res.Result;
        });
        return tableName;
    }
}


var clientPc = {    
getViewHTML: function (fileName) {
		return window.external.GetView(fileName);
	},
	getNexVal:function(table){
//		if(!window.external.getNextVal) return null;
		var str =window.external.getNextVal(table);
		if(str==null) throw ("getNexVal("+table+")异常，可能是数据库连接问题。");
		var jsonResult;
		eval('jsonResult='+str);
		if(jsonResult.Message){alert(jsonResult.Message);return null;}		
		return jsonResult.Result;
	},
	execQuery: function (permission, fields,table,where,order) {		
		if(where=="") where=null;
		var str = window.external.execQuery(table,fields,where,order);
		if(str==null) throw ("查询"+table+"异常。");
		var jsonResult;
		eval('jsonResult='+str);
		if(jsonResult.Message){alert(jsonResult.Message);return null;}
		return jsonResult.Result;
	},execDb: function (jsonArr) {
		//if(!window.external.execDb) return null;		
		for(var i=0;i<jsonArr.length;i++){
			var fieldNameArr=[];
			var fieldValueArr=[];
			var fields=jsonArr[i].fields;
			for(var name in fields){
				fieldNameArr[fieldNameArr.length]= name;
				fieldValueArr[fieldValueArr.length]= fields[name];
			}
			jsonArr[i].fields = fieldNameArr;
			jsonArr[i].values = fieldValueArr;
		}
		var jsonArrStr = JSON.stringify(jsonArr);
		var str= window.external.execDb(jsonArrStr);
		if(str==null) throw ("执行execDb异常。");
		var jsonResult;
		eval('jsonResult='+str);
		if(jsonResult.Message){alert(jsonResult.Message);return null;}
		return jsonResult.Result;
	},openPictureDialog:function(){
		str= window.external.uploadFile();
		return str;
	},getTableName:function(table){
		//if(!window.external.getTableName)return null;
	    var str = window.external.getTableName(table);
	    if (str == null) throw ("getTableName("+table+")异常。");
		var jsonResult;
		eval('jsonResult='+str);
		if(jsonResult.Message){alert(jsonResult.Message);return null;}
		return jsonResult.Result;
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


var clientHttpPhp = {
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
	    return nextVal;
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
/*==============================扩展功能======================================*/
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

//获取地址栏参数json,WEB用
function getUrlJson() {
    var url = window.location.href;
    var pos = url.indexOf('?');
    if (pos == -1) return null;
    var paras = url.substring(pos + 1);
    var paraArr = paras.split('&');
    var base64Json = null;
    for (var i = 0; i < paraArr.length; i++) {
        var pos2 = paraArr[i].indexOf("plus=");
        if (pos2 > -1) { base64Json = paraArr[i].substring(pos2 + 5); }
    }
    if (base64Json == null) return null;
    base64Json = window.base64.decode(base64Json);
    var json;
    eval('json=' + base64Json);
    return json;
}

