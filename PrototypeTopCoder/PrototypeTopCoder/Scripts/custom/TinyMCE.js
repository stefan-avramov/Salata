var fileBrowser = function (fieldName, url, type, win) {
    // Depends on having the uploadUrl variable declared globally !!!
    var cmsURL = uploadUrl;
    if (cmsURL.indexOf("?") < 0) {
        //add the type as the only query parameter
        cmsURL = cmsURL + "?type=" + type;
    }
    else {
        //add the type as an additional query parameter
        // (PHP session ID is now included if there is one at all)
        cmsURL = cmsURL + "&type=" + type;
    }

    tinyMCE.activeEditor.windowManager.open({
        file: cmsURL,
        title: 'My File Browser',
        width: 400,  // Your dimensions may differ - toy around with them!
        height: 300,
        resizable: "yes",
        inline: "yes",  // This parameter only has an effect if you use the inlinepopups plugin!
        close_previous: "no"
    }, {
        window: win,
        input: fieldName
    });
    return false;
}

tinyMCE.init({
    mode: 'textareas', file_browser_callback: fileBrowser, width: 400, height: 400, theme: 'advanced',
    theme_advanced_toolbar_location: "top",
    theme_advanced_toolbar_align: "left",
    theme_advanced_statusbar_location: "bottom",
    theme_advanced_resizing: true,
    plugins: 'inlinepopups',
    encoding: 'xml',
    setup: function (ed) {
        ed.onSaveContent.add(function (ed, o) {
            o.content = o.content.replace(/&#39/g, "&apos");
        });
    }
});