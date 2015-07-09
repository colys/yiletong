
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
        $("<div>" + item.tdate + " , " + item.trname + " : " + item.amt + "</div>").appendTo(status_list);
    })
    if (json.length == 0) {
        status_list.html("没有刷卡数据！");
    }
    return JSON.stringify(json);
}

function setStatus(str) {
    $("#status").html(str);
}
