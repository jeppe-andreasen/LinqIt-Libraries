var linqit = linqit || {};

(function (le, $, undefined) {
    le.selectItem = function($le, item)
    {
        if ($le[0].selectedItem != undefined)
            $($le[0].selectedItem).removeClass("selected");

        if (item == undefined)
        {
            $le[0].selectedItem = undefined;
            return;
        }
        item.addClass("selected");
        $le[0].selectedItem = item[0];

        $le.find("div.editor").show();
        var $ed = $le.find(".linqit-linkeditor");

        var value = item[0].linkValue;
        if (value == undefined)
            value = '<a type="internal">' + item.html() + '</a>';

        linqit.linkeditor.setValue($ed, value);
        linqit.linkeditor.showInput($ed);
    }

    le.addLink = function(child){
        var $le = $(child).closest('.linqit-linklisteditor');

        $item = $('<li>New Item</li>');
        $item[0].isTmp = true;
        $le.find('td.links ul').append($item);
        le.selectItem($le, $item);
        le.updateValue($le)
    }

    le.removeLink = function(child){
        var $le = $(child).closest('.linqit-linklisteditor');

        if ($le[0].selectedItem != undefined)
        {
            $($le[0].selectedItem).remove();
            $le[0].selectedItem = undefined;
        }
        $le.find("div.editor").hide();
        le.updateValue($le)
    }

    le.updateValue = function($le){
        var value = "";
        $le.find("td.links li").each(function(){
            if (this.linkValue != undefined)
                value += this.linkValue;
        });
        $le.children("input").val(value);
    }

    $(function() {
        $('.linqit-linklisteditor').each(function () {
            $le = $(this);

            // Load values 

            var hidden = $le.children("input");
            var values = $(hidden.val());
            var $ul = $le.find('td.links ul');
            values.each(function() {
                $a = $(this);
                var $li = $("<li></li>");
                $li.attr("class", $a.attr("type"));
                $li.html($a.html());
                $li[0].linkValue = $a.clone().wrap('<p>').parent().html();
                $ul.append($li);
            });

            $le.bind('valueChanged', '.linqit-linkeditor', function(event, editor, value){
                var $item = null;
                if ($le[0].selectedItem != undefined)
                    $item = $($le[0].selectedItem);
                else
                {
                    $item = $('<li></li>');
                    $le.find('td.links ul').append($item);
                }
                $item[0].linkValue = value;
                $item[0].isTmp = false;
                $item.html($(value).html());
                $item.attr("class", $(value).attr("type")); 
                le.selectItem($le, $item);
                le.updateValue($le)
            });

            $le.bind('updateCancelled', '.linqit-linkeditor', function(event, editor){
                if ($le[0].selectedItem != undefined && $le[0].selectedItem.isTmp == true)
                {
                    $($le[0].selectedItem).remove();
                    $le[0].selectedItem = undefined;
                }
                $le.find('div.editor').hide();
                le.updateValue($le)
            });
            
            $le.find(".sortable").sortable({ containment: 'parent', placeholder: "plh" });
            
            $le.on('click', '.sortable li', function() {
                le.selectItem($le, $(this));
            });
        });
    });
} (linqit.linklisteditor = linqit.linklisteditor || {}, jQuery));