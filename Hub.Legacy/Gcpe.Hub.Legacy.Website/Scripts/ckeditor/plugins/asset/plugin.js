
( function() {
    CKEDITOR.plugins.add( 'asset', {
        requires: 'dialog,fakeobjects',
        icons: 'asset',
        hidpi: true,
        onLoad: function () {
            CKEDITOR.addCss( 'img.cke_asset' +
                '{' +
                    'background-image: url(' + CKEDITOR.getUrl( this.path + 'images/placeholder.png' ) + ');' +
                    'background-position: center center;' +
                    'background-repeat: no-repeat;' +
                    'border: 1px solid #a9a9a9;' +
                    'align: center;' +
                    'width: 100%;' +
                    'height: 80px;' +
                '}'
                );
        },
        init: function( editor ) {
            var pluginName = 'asset';

            CKEDITOR.dialog.add('asset', function (editor) {
                return {
                    title: 'Asset Properties',
                    minWidth: 600,
                    minHeight: 50,
                    onShow: function () {
                        
                        this.fakeImage = this.assetNode = null;
                        var selected = this.getSelectedElement();
                        if (selected && selected.data('cke-real-element-type') && selected.data('cke-real-element-type') == 'asset') {
                            this.fakeImage = selected;
                            this.assetNode = editor.restoreRealElement(this.fakeImage);
                            this.setupContent(this.assetNode);
                        }
                    },
                    onOk: function () {
                        var assetNode = this.fakeImage ? this.assetNode : new CKEDITOR.dom.element('asset');
                        this.commitContent(assetNode, {}, {});

                        var newFakeImage = editor.createFakeElement(assetNode, 'cke_asset', 'asset', true);

                        if (this.fakeImage) {
                            newFakeImage.replace(this.fakeImage);
                            //editor.getSelection().selectElement( newFakeImage );
                        } else {
                            editor.insertElement(newFakeImage);
                        }
                    },
                    contents: [{
                        id: 'info',
                        label: 'Asset',
                        elements: [{
                            type: 'text',
                            label: "URL",
                            required: true,
                            validate: CKEDITOR.dialog.validate.regex(/^https:\/\/[^ "]+$/, 'Please type the asset URL starting with https://'), //only https and disallow spaces
                            setup: function (assetNode) {
                                this.setValue(assetNode.getHtml());
                            },
                            commit: function (assetNode) {
                                assetNode.setHtml(this.getValue());
                            }
                        }]
                    }]
                };
            });


            editor.addCommand( pluginName, new CKEDITOR.dialogCommand( pluginName, {
                allowedContent: 'asset',
                requiredContent: 'asset'
            } ) );

            editor.ui.addButton && editor.ui.addButton('asset', {
                label: 'Asset',
                command: pluginName,
                toolbar: 'insert,180'
            });

            editor.on( 'doubleclick', function( evt ) {
                var element = evt.data.element;
                if ( element.is( 'img' ) && element.data( 'cke-real-element-type' ) == 'asset' )
                    evt.data.dialog = 'asset';
            } );

            if ( editor.addMenuItems ) {
                editor.addMenuItems( {
                    asset: {
                        label: 'Asset',
                        command: pluginName,
                        group: 'image'
                    }
                } );
            }

            //we can enable this if there is interest in having context menu to open the dialog
            //if ( editor.contextMenu ) {
            //	editor.contextMenu.addListener( function( element ) {
            //		if ( element && element.is( 'img' ) && element.data( 'cke-real-element-type' ) == 'asset' )
            //			return { asset: CKEDITOR.TRISTATE_OFF };
            //	} );
            //}
        },
        afterInit: function( editor ) {
            var dataProcessor = editor.dataProcessor,
                dataFilter = dataProcessor && dataProcessor.dataFilter;

            if ( dataFilter ) {
                dataFilter.addRules( {
                    elements: {
                        asset: function( element ) {
                            return editor.createFakeParserElement( element, 'cke_asset', 'asset', true );
                        }
                    }
                } );
            }

            // Prevent extra ghost lines to be added when an asset is not within a <p>aragraph
            var blockTags = ['p', 'ul'];
            for (var i = 0; i < blockTags.length; i++) {
                dataProcessor.writer.setRules(blockTags[i], {
                    breakBeforeOpen: false, // Do not insert a line break before a tag
                    breakAfterClose: false  // Do not insert a line break after the closing tag
                });
            }
        }
    } );
} )();
