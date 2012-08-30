var linqit = linqit || {};

(function (ed, $, undefined) {
    ed.selectTab = function(tab){
        ed.showEditor(tab);
    }
    ed.showEditor = function (child, type) {
        var $child = $(child);
        var $e = $child.closest('.linqit-linkeditor');
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
        var $fieldset = $form.find("fieldset.active");
        $fieldset.removeClass("active");
        
        var values = {};
        values.type = $fieldset.attr("class");

        switch(values.type) {
            case "internal":
                values.text = $form.find('input[id$="txtInternalText"]').val();
                values.target = $form.find('select[id$="ddlInternalTarget"]').val();
                values.title = $form.find('input[id$="txtInternalTooltip"]').val();
                values.class = $form.find('input[id$="txtInternalClass"]').val();
                var internalTree = $form.find('ul[id$="internalTree"]');
                values.itemId = linqit.treeview.getSelectedValue(internalTree);
                var anchor = $form.find('input[id$="txtInternalAnchor"]').val();
                var query = $form.find('input[id$="txtInternalQueryString"]').val();
                values.href = callMethod('LinqIt.Libraries.WebTest.Components.LinkEditor, LinqIt.Libraries.WebTest','GetInternalUrl', values.itemId, query, anchor);
                break;
            case "external":
                values.href = $form.find('input[id$="txtExternalPath"]').val();
                values.text = $form.find('input[id$="txtExternalText"]').val();
                values.target = $form.find('select[id$="ddlExternalTarget"]').val();
                values.title = $form.find('input[id$="txtExternalTooltip"]').val();
                values.class = $form.find('input[id$="txtExternalClass"]').val();
                break;
            case "media":
                var mediaTree = $form.find('ul[id$="mediaTree"]');
                values.itemId = linqit.treeview.getSelectedValue(mediaTree);
                values.text = $form.find('input[id$="txtMediaText"]').val();
                values.target = $form.find('select[id$="ddlMediaTarget"]').val();
                values.title = $form.find('input[id$="txtMediaTooltip"]').val();
                values.class = $form.find('input[id$="txtMediaClass"]').val();
                values.href = callMethod('LinqIt.Libraries.WebTest.Components.LinkEditor, LinqIt.Libraries.WebTest','GetMediaUrl', values.itemId);
                break;
            case "mailto":
                values.href = "mailto:" + $form.find('input[id$="txtMailToPath"]').val();
                var subject = $form.find('input[id$="txtMailToSubject"]').val();
                if (subject != '')
                    values.href += "#" + subject;

                values.text = $form.find('input[id$="txtMailToText"]').val();
                values.title = $form.find('input[id$="txtMailToTooltip"]').val();
                values.class = $form.find('input[id$="txtMailToClass"]').val();
                break;
            case "javascript":
                values.href = "javascript:" + $form.find('textarea[id$="txtJavascriptCode"]').val();
                values.text = $form.find('input[id$="txtJavascriptText"]').val();
                values.title = $form.find('input[id$="txtJavascriptTooltip"]').val();
                values.class = $form.find('input[id$="txtJavascriptClass"]').val();
                break;
            case "anchor":
                values.href = "#" + $form.find('input[id$="txtAnchorTarget"]').val();
                values.text = $form.find('input[id$="txtAnchorText"]').val();
                values.title = $form.find('input[id$="txtAnchorTooltip"]').val();
                values.class = $form.find('input[id$="txtAnchorClass"]').val();
                break;
        }
        
        var value = '<a';
        for (var key in values)
        {
            if (key == 'text' || values[key] == '')
                continue;
            value += ' ' + key + '="' + values[key] + '"';
        }
        value += '>' + values.text + '</a>';

        var $e = $form.closest(".linqit-linkeditor");
        getHiddenInput($e).val(value);
        $e.trigger("valueChanged", [$e[0], value]);
        ed.showInfo($e);
    }

    ed.cancelUpdate = function(child){
        var $e = $(child).closest(".linqit-linkeditor");
        ed.showInfo($e);
        $e.trigger("updateCancelled", $e[0]);
    }

    ed.clearValue = function(child){
        var $e = $(child).closest(".linqit-linkeditor");
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
                $form.find('select[id$="ddlInternalTarget"]').val($value.attr("target"));
                $form.find('input[id$="txtInternalTooltip"]').val($value.attr("title"));
                $form.find('input[id$="txtInternalClass"]').val($value.attr("class"));
                
                var internalTree = $form.find('ul[id$="internalTree"]');
                $value.itemId = linqit.treeview.selectNode(internalTree, $value.attr("itemId"), true);
                var href = $value.attr('href');
                if (href != "" && href != undefined)
                {
                    var parts = href.split('#');
                    $form.find('input[id$="txtInternalAnchor"]').val(parts.length > 1 ? parts[1] : "");
                    parts = parts[0].split('?');
                    $form.find('input[id$="txtInternalQueryString"]').val(parts.length > 1 ? parts[1] : "");                    
                }
                break;
            case "external":
                $form.find('input[id$="txtExternalPath"]').val($value.attr('href'));
                $form.find('input[id$="txtExternalText"]').val($value.html());
                $form.find('select[id$="ddlExternalTarget"]').val($value.attr('target'));
                $form.find('input[id$="txtExternalTooltip"]').val($value.attr('title'));
                $form.find('input[id$="txtExternalClass"]').val($value.attr('class'));
                break;
            case "media":
                var mediaTree = $form.find('ul[id$="mediaTree"]');
                $value.itemId = linqit.treeview.selectNode(mediaTree, $value.attr('itemId'), true);
                $form.find('input[id$="txtMediaText"]').val($value.html());
                $form.find('select[id$="ddlMediaTarget"]').val($value.attr('target'));
                $form.find('input[id$="txtMediaTooltip"]').val($value.attr('title'));
                $form.find('input[id$="txtMediaClass"]').val($value.attr('class'));
                break;
            case "mailto":
                var href = $value.attr('href');
                if (href != '')
                {
                    var parts = href.replace("mailto:", "").split('#');
                    $form.find('input[id$="txtMailToPath"]').val(parts[0]);
                    $form.find('input[id$="txtMailToSubject"]').val(parts.length > 1? parts[1] : "");
                }
                $form.find('input[id$="txtMailToText"]').val($value.html());
                $form.find('input[id$="txtMailToTooltip"]').val($value.attr('title'));
                $form.find('input[id$="txtMailToClass"]').val($value.attr('class'));
                break;
            case "javascript":
                $form.find('textarea[id$="txtJavascriptCode"]').val($value.attr('href').replace("javascript:",""));
                $form.find('input[id$="txtJavascriptText"]').val($value.html());
                $form.find('input[id$="txtJavascriptTooltip"]').val($value.attr('title'));
                $form.find('input[id$="txtJavascriptClass"]').val($value.attr('class'));
                break;
            case "anchor":
                $form.find('input[id$="txtAnchorTarget"]').val($value.attr("href").replace("#",""));
                $form.find('input[id$="txtAnchorText"]').val($value.html());
                $form.find('input[id$="txtAnchorTooltip"]').val($value.attr('title'));
                $form.find('input[id$="txtAnchorClass"]').val($value.attr('class'));
                break;
        }
        
    }
    ed.clearForm = function($form) {
        $form.find('select').val('');
        $form.find('textarea').val('');
        $form.find('input[type=text]').val('');
        var internalTree = $form.find('ul[id$="internalTree"]');
        linqit.treeview.selectNode(internalTree, '', true);
        var mediaTree = $form.find('ul[id$="mediaTree"]');
        linqit.treeview.selectNode(mediaTree, '', true);
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
            info += '<a class="create" href="#" onclick="linqit.linkeditor.showInputc(this); return false;"><span>Set Link</span></a>';
        }
        else {
            $value = $(value);
            info = 'Link: <span class="linktext">' + $value.html() + '</span> (<span class="linktype">' + $value.attr("type") + "</span>)";
            info += '<a class="change" href="#" onclick="linqit.linkeditor.showInputc(this); return false;"><span>Change</span></a>';
            info += '<a class="clear" href="#" onclick="linqit.linkeditor.clearValue(this); return false;"><span>Clear</span></a>';
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
        ed.showInput($(child).closest('.linqit-linkeditor'));
    }
    

    function getHiddenInput($editor){
        return $editor.children('input');
    }

    $(function(editor) {
        $('.linqit-linkeditor').each(function(){
            ed.showInfo(this);
        });
    });


} (linqit.linkeditor = linqit.linkeditor || {}, jQuery));