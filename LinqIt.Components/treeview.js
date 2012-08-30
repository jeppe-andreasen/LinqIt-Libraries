var linqit = linqit || {};

(function (tv, $, undefined) {

    tv.refreshNode = function (treeview, itemId) {
        var $t = treeview.jquery ? treeview : $(treeview);
        var $node = itemId != undefined ? findNode($t, itemId) : $($t[0].selectedNode);
        if ($node.length == 0)
            return;
        var $li = $node.closest("li");
        var provider = $t.attr("data-provider");
        var referenceId = $t.attr("data-referenceId");
        var html = callMethod('LinqIt.Components.LinqItTreeView, LinqIt.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','FetchNode', $node.attr("ref"),providerName,referenceId,true);
        $li.html(html);
    };

    tv.selectNode = function (treeview, itemId, suppressDialogs) {
        var $t = treeview.jquery ? treeview : $(treeview);

        // Unselect current node
        if ($t[0].selectedNode != undefined) {
            if (!suppressDialogs)
                $t.trigger("nodeUnselected", $t[0].selectedNode);
            $($t[0].selectedNode).removeClass("selected");
            $t[0].selectedNode = undefined;
        }

        if (itemId == '' || itemId == undefined)
            return;

        // Select node
        var $node = findNode($t, itemId);
        if ($node.length == 0)
        {
            // the node is node loaded or it does not exist. 
            // Try loading the tree path of the node
            var provider = $t.attr("data-provider");
            var referenceId = $t.attr("data-referenceId");
            var findResult = callMethod('LinqIt.Components.LinqItTreeView, LinqIt.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','FindNode', itemId, provider, referenceId);
            if (!findResult.found)
                return;

            var root = findNode($t, findResult.root).closest('li');
            root.html(findResult.html);
            root.find(".draggable").each(function () {
                if ($(this).draggable != undefined)
                    $(this).draggable({ helper: "clone", opacity: 0.7 });
            });
            $node = findNode($t, itemId);
        }
        $t[0].selectedNode = $node[0];
        $("#" + $t.attr("hiddenId")).val(itemId);
        $node.addClass("selected");
        $t.trigger("nodeSelected", $node[0]);
    };

    tv.getSelectedValue = function (treeview) {
        var $t = treeview.jquery ? treeview : $(treeview);
        if ($t[0].selectedNode != undefined)
            return $($t[0].selectedNode).attr("ref");
        else
            return '';
    };

    function findNode($treeview, itemId) {
        return $treeview.find('[ref="' + itemId + '"]');
    }

} (linqit.treeview = linqit.treeview || {}, jQuery));




$(document).ready(function () {
    $(".linqit-treeview").each(function (ix, treeview) {
        treeview.onNodeClicked = function ($node) {
            linqit.treeview.selectNode(treeview, $node.attr("ref"), false);
        }
    });
    $(document).on("click", ".linqit-treeview .toggler", function () {
        var link = $(this);
        link.toggleClass("expanded");
        var nextA = link.next();
        var ul = nextA.next();
        if (ul.length > 0)
            ul.toggle();
        else {
            var id = nextA.attr("ref");
            var treeview = link.closest(".linqit-treeview");
            var provider = treeview.attr("data-provider");
            var referenceId = treeview.attr("data-referenceId");
            if (referenceId == undefined)
                referenceId = '';

            var html = $(callMethod('LinqIt.Components.LinqItTreeView, LinqIt.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','FetchChildren', id,provider,referenceId));
            nextA.after(html);
            html.find(".draggable").each(function () {
                if ($(this).draggable != undefined)
                    $(this).draggable({ helper: "clone", opacity: 0.7 });
            });
        }
        return false;
    });
    $(document).on("click", ".linqit-treeview .node", function () {
        var treeview = $(this).closest(".linqit-treeview");
        treeview[0].onNodeClicked($(this));
    });

    $(".linqit-treeview").each(function () {
        var treeview = $(this);
        var contextMenuProvider = treeview.data("contextmenuprovider");
        if (contextMenuProvider != undefined) {
            var id = $(this).attr("id");
            $.contextMenu({
                selector: '#' + id + ' .node',
                appendTo: treeview.parent(),
                build: function ($trigger, e) {
                    return {
                        callback: function (key, options) {
                            treeview.trigger("onCommand", [key, options.$trigger.attr("ref")]);
                        },
                        items: getContextMenuItems($trigger.attr("ref"), contextMenuProvider)
                    };
                }
            });
        }
    });
});

