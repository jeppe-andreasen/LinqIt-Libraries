var linqit = linqit || {};

(function (ed, $, undefined) {
    ed.selectTab = function(tab){
        ed.showEditor(tab);
    }
    ed.showEditor = function (child, type) {
        var $child = $(child);
        var $e = $child.closest('.linqit-imageeditor');
        $e.find('ul.tabs a').removeClass("active");
        var $form = $e.find('.editor-form');
        $form.find("fieldset").removeClass("active");
        if (type == undefined)
            type = $child.data("type");
        $form.find("fieldset." + type).addClass("active");
        $e.find('ul.tabs a[data-type="' + type + '"]').addClass("active");
        return false;
    }
    ed.updateValue = function(child) {
        var $form = $(child).closest('.editor-form');
        var $e = $form.closest(".linqit-imageeditor");
        var $fieldset = $form.find("fieldset.active");
        $fieldset.removeClass("active");
        
        var values = {};
        values.type = $fieldset.attr("class");

        switch(values.type) {
            case "internal":
                values.alt = $form.find('input[id$="txtInternalText"]').val();

                var internalTree = $form.find('ul[id$="internalTree"]');
                values.itemId = linqit.treeview.getSelectedValue(internalTree);
                
                var provider = $e.attr("data-provider");
                var referenceId = $e.attr("data-referenceId");

                var properties = callMethod('LinqIt.Components.LinqItImageEditor, LinqIt.Components','GetImageProperties', provider, referenceId, values.itemId);
                values.src = properties.url;
                
                break;
            case "external":
                values.src = $form.find('input[id$="txtExternalPath"]').val();
                values.alt = $form.find('input[id$="txtExternalText"]').val();
                break;
        }
        
        var value = '<img';
        for (var key in values)
        {
            if (values[key] == '')
                continue;
            value += ' ' + key + '="' + values[key] + '"';
        }
        value += '>';

        
        getHiddenInput($e).val(value);
        $e.trigger("valueChanged", [$e[0], value]);
        ed.showInfo($e);
    }

    ed.cancelUpdate = function(child){
        var $e = $(child).closest(".linqit-imageeditor");
        ed.showInfo($e);
        $e.trigger("updateCancelled", $e[0]);
    }

    ed.clearValue = function(child){
        var $e = $(child).closest(".linqit-imageeditor");
        getHiddenInput($e).val('');
        ed.showInfo($e);
    }

    ed.setValue = function(editor, value){
        var $e = editor.jquery ? editor : $(editor);
        getHiddenInput($e).val(value);
        var $form = $e.find('.editor-form');
        ed.clearForm($form);
        if (value == '')
        {
            ed.showEditor($form[0], 'internal');
            return;
        }
        var $value = $(value);
        var type = $value.attr("type");
        ed.showEditor($form[0],type);

        $form.find('input[id$="Text"]').val($value.html());

        switch (type) {
            case "internal":
                $form.find('input[id$="txtInternalText"]').val($value.html());
                var internalTree = $form.find('ul[id$="internalTree"]');
                $value.itemId = linqit.treeview.selectNode(internalTree, $value.attr("itemId"), true);
                break;
            case "external":
                $form.find('input[id$="txtExternalPath"]').val($value.attr('src'));
                $form.find('input[id$="txtExternalText"]').val($value.attr('alt'));
                $form.find('fieldset.external .previewbox img').attr("src", $value.attr('src'));
                break;
        }
        
    }
    ed.clearForm = function($form) {
        $form.find('select').val('');
        $form.find('textarea').val('');
        $form.find('input[type=text]').val('');
        $form.find('.previewbox img').attr('src','');
        $form.find('.previewbox img').attr('alt','');
        var internalTree = $form.find('ul[id$="internalTree"]');
        linqit.treeview.selectNode(internalTree, '', true);
    }
    ed.showDefault = function(editor, value){
        var $e = editor.jquery ? editor : $(editor);
        getHiddenInput($e).val(value);
        ed.showInfo($e);
    }
    ed.showInfo = function(editor){
        var $e = editor.jquery ? editor : $(editor);
        var value = getHiddenInput($e).val();
        var $output = $e.children(".editor-output");

        var info = "";

        if (value == '') {
            info = 'Link: <span class="notset">Not set</span>';
            info += '<a class="create" href="#" onclick="linqit.imageeditor.showInputc(this); return false;"><span>Set Link</span></a>';
        }
        else {
            $value = $(value);
            info = 'Link: <span class="linktext">' + $value.html() + '</span> (<span class="linktype">' + $value.attr("type") + "</span>)";
            info += '<a class="change" href="#" onclick="linqit.imageeditor.showInputc(this); return false;"><span>Change</span></a>';
            info += '<a class="clear" href="#" onclick="linqit.imageeditor.clearValue(this); return false;"><span>Clear</span></a>';
        }
        $output.html(info);
        $e.children(".editor-output").show();
        $e.children(".editor-input").hide();
    }
    ed.showInput = function($e){
        ed.setValue($e, getHiddenInput($e).val());
        $e.children(".editor-output").hide();
        $e.children(".editor-input").show();
    }
    ed.showInputc = function(child){
        ed.showInput($(child).closest('.linqit-imageeditor'));
    }

    function getHiddenInput($editor){
        return $editor.children('input');
    }

    $(function() {
        $('.linqit-imageeditor').each(function(){
            ed.showInfo(this);

            $editor = $(this);

            var internalTree = $(this).find('ul[id$="internalTree"]');
            $editor.bind('nodeSelected', internalTree, function(event, node){
                var $e = $(this);
                var provider = $e.attr("data-provider");
                var referenceId = $e.attr("data-referenceId");
                var value = $(node).attr("ref");
                var properties = callMethod('LinqIt.Components.LinqItImageEditor, LinqIt.Components','GetImageProperties', provider, referenceId, value);
                var img = $e.find("fieldset.internal .previewbox img");
                img.attr("src", properties.url);
                img.attr("alt", properties.alt);
                var altbox = $e.find('input[id$="txtInternalText"]');
                if (altbox.val() == '')
                    altbox.val(properties.alt);
            });

            $editor.find('input[id$="txtExternalPath"]').blur(function(){
                var img = $editor.find("fieldset.external .previewbox img");
                img.attr("src", $(this).val());
            });
        });
    });


} (linqit.imageeditor = linqit.imageeditor || {}, jQuery));