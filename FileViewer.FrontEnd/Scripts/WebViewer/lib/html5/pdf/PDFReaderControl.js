(function(exports) {
    "use strict";

    // reference the parent ReaderControl
    exports.DesktopReaderControl = exports.ReaderControl;

    exports.ReaderControl = function(options) {
        var me = this;
        this.showFilePicker = options.showFilePicker;

        exports.DesktopReaderControl.call(this, options);
        me.fireError = function (type, msg, genericMsg) {
            console.warn('Error: ' + msg);
            me.fireEvent('error', [type, msg, genericMsg]);
        };

        this.pdfType = options.pdfType;
        this.initProgress();

        this.workerHandlers = {
            workerLoadingProgress: function(percentComplete) {
                me.fireEvent('workerLoadingProgress', percentComplete);
            },
            pnaclCrashError: function(msg, genericMsg) {
                me.fireError('PNaClCrashError', msg, genericMsg);
            },
            pnaclLoadError: function(msg, genericMsg) {
                me.fireError('PNaClLoadError', msg, genericMsg);
            },
            emsWorkerError: function(msg, genericMsg) {
                me.fireError('EmsWorkerError', msg, genericMsg);
            }
        };

        this.pdfTypePromise = (this.pdfType === 'auto') ? exports.CoreControls.getDefaultPdfBackendType() : Promise.resolve(this.pdfType);
        if (options.preloadWorker) {
            this.pdfTypePromise.then(function(pdfType) {
                var useEmscriptenWhileLoading = me.pdfType !== 'pnacl';

                exports.CoreControls.preloadPDFWorker(pdfType, me.workerHandlers, {
                    useEmscriptenWhileLoading: useEmscriptenWhileLoading,
                    autoSwap: false
                });
            });
        }

        this.filename = 'downloaded.pdf';

        // code to handle password requests from DocumentViewer
        var passwordDialog;
        var passwordInput;
        var passwordMessage;
        var showTextMessage;
        var finishedPassword;
        var tryingPassword;
        me.getPassword = function(passwordCallback) {
            // only allow a few attempts
            finishedPassword = me.passwordTries >= 3;
            tryingPassword = false;
            if (me.passwordTries === 0) {
                // first try so we create the dialog
                passwordDialog = $('<div>').attr({
                    'id': 'passwordDialog'
                });

                showTextMessage = $('<div style="color:red"></div>').appendTo(passwordDialog);

                passwordMessage = $('<label>').attr({
                    'for': 'passwordInput'
                })
                    .text('Enter the document password:')
                    .appendTo(passwordDialog);

                passwordInput = $('<input>').attr({
                    'type': 'password',
                    'id': 'passwordInput'
                }).keypress(function(e) {
                    if (e.which === 13) {
                        $(this).parent().next().find('#pass_ok_button').click();
                    }
                }).appendTo(passwordDialog);

                passwordDialog.dialog({
                    modal: true,
                    resizable: false,
                    closeOnEscape: false,
                    close: function () {
                        if (!tryingPassword) {
                            me.fireError('EncryptedFileError', "The document requires a valid password.", i18n.t('error.EncryptedUserCancelled'));
                        }
                    },
                    buttons: {
                        'OK': {click: function() {
                                if (!finishedPassword) {
                                    tryingPassword = true;
                                    passwordCallback(passwordInput.val());
                                }
                                $(this).dialog('close');
                            },
                            id: 'pass_ok_button',
                            text: 'OK'
                        },
                        'Cancel': function() {
                            $(this).dialog('close');
                        }
                    }
                });

            } else if (finishedPassword) {
                // attempts have been used
                me.fireError('EncryptedFileError', "The document requires a valid password.", i18n.t('error.EncryptedAttemptsExceeded'));
            } else {
                // allow another request for the password
                passwordInput.val('');
                showTextMessage.text('The Password is incorrect. Please make sure that Caps lock is not on by mistake, and try again.');
                passwordDialog.dialog('open');
            }

            ++(me.passwordTries);
        };

        me.onDocError = function (err) {
            me.fireError('PDFLoadError', err.message, i18n.t('error.PDFLoadError'));
        };
    };

    exports.ReaderControl.prototype = {
        // we are fine with using a larger max zoom (like 1000%) unlike XOD webviewer
        MAX_ZOOM: 10,
        MIN_ZOOM: 0.05,
        // PDF units are 72 points per inch so we need to adjust it to 96 dpi for window.print()
        printFactor: 96/72,
        /**
         * Initialize UI controls.
         * @ignore
         */
        initUI: function() {
            var me = this;

            exports.DesktopReaderControl.prototype.initUI.call(this);

            var downloadButton = $('<span></span>')
                .addClass('glyphicons disk_save')
                .attr({
                    id: 'downloadButton',
                    'data-i18n': '[title]controlbar.download'
                }).i18n();

            var $printParent = $('#printButton').parent();
            $printParent.append(downloadButton);

            if (this.showFilePicker) {
                var $filePicker = $('<label for="input-pdf" class="file-upload glyphicons folder_open"></label>' +
                    '<input id="input-pdf"  accept="application/pdf,.png,.jpg,.jpeg" type="file" class="input-pdf">')
                    .attr('data-i18n', '[title]controlbar.open')
                    .i18n();
                $printParent.append($filePicker);

                $filePicker.on('change', me.listener.bind(me));
            }
            var inProgress = false;

            $('#downloadButton').on('click', function () {
                var current_document = me.docViewer.getDocument();
                if (inProgress || !current_document) {
                    return;
                }
                inProgress = true;
                downloadButton.removeClass('disk_save');
                downloadButton.addClass('refresh');
                downloadButton.addClass('rotate-icon');

                var annotManager = me.docViewer.getAnnotationManager();
                var options = {"xfdfString": annotManager.exportAnnotations()};
                current_document.getFileData(options).then(function (data) {
                    inProgress = false;
                    downloadButton.removeClass('glyphicons-refresh');
                    downloadButton.removeClass('rotate-icon');
                    downloadButton.addClass('disk_save');
                    var arr = new Uint8Array(data); // should be removed soon
                    var blob = new Blob([arr], {
                        type: 'application/pdf'
                    });

                    saveAs(blob, me.getDownloadFilename(me.filename));
                    me.fireEvent('finishedSavingPDF');
                });
            });
        },

        getDownloadFilename: function(filename) {
            if (filename && filename.slice(-4).toLowerCase() !== '.pdf') {
                filename += '.pdf';
            }
            return filename;
        },

        /**
         * Loads a PDF document into the ReaderControl
         * @param {string} doc a resource URI to the document. The URI may be an http or blob URL.
         * @param loadOptions options to load the document
         * @param loadOptions.filename the filename of the document to load. Used in the export/save PDF feature.
         * @param loadOptions.customHeaders specifies custom HTTP headers in retrieving the resource URI.
         * @param loadOptions.workerTransportPromise optionally specifies a worker transport promise to be used
         */
        loadDocument: function (doc, options) {
            var me = this;
            this.showProgress();
            this.closeDocument();

            var pdfPartRetriever = new CoreControls.PartRetrievers.ExternalPdfPartRetriever(doc);
            if (options && options.customHeaders) {
                pdfPartRetriever.setCustomHeaders(options.customHeaders);
            }
            if (options && options.filename) {
                this.filename = options.filename;
            } else {
                this.parseFileName(doc);
            }

            pdfPartRetriever.on('documentLoadingProgress', function(e, loaded, total) {
                me.fireEvent('documentLoadingProgress', [loaded, total]);
            });
            pdfPartRetriever.on('error', function(e, type, message) {
                me.fireEvent('error', [type, message, i18n.t('error.load') + ': ' + message]);
            });

            this.loadAsync(me.docId, pdfPartRetriever, options.workerTransportPromise);
        },

        loadLocalFile: function(file, options) {
            var me = this;
            this.showProgress();
            this.closeDocument();

            var partRetriever = new CoreControls.PartRetrievers.LocalPdfPartRetriever(file);
            partRetriever.on('documentLoadingProgress', function(e, loaded, total) {
                me.fireEvent('documentLoadingProgress', [loaded, total]);
            });

            // load the document into the viewer
            this.loadAsync(window.readerControl.docId, partRetriever, options.workerTransportPromise);

            // get the filename so we can use it when downloading the file
            if (options.filename) {
                this.filename = options.filename;
            }
        },

        loadAsync: function (id, partRetriever, workerTransportPromise) {
            var me = this;

            this.pdfTypePromise.then(function(pdfType) {
                var extension = 'pdf';

                var filename = me.filename;
                if (filename) {
                    var extensionStartIndex = filename.lastIndexOf('.');
                    if (extensionStartIndex !== -1) {
                        extension = filename.slice(extensionStartIndex + 1).toLowerCase() || 'pdf';
                    }
                }

                var options = {
                    type: 'pdf',
                    docId: id,
                    extension: extension,
                    getPassword: me.getPassword,
                    onError: me.onDocError,
                    l: me.l,
                    pdfBackendType: pdfType,
                    workerHandlers: me.workerHandlers
                };
                if (workerTransportPromise) {
                    options.workerTransportPromise = workerTransportPromise;
                }
                me.docViewer.setRenderBatchSize(2);
                me.docViewer.setViewportRenderMode(true);
                me.passwordTries = 0;
                me.hasBeenClosed = false;
                me.docViewer.loadAsync(partRetriever, options);
            });
        },

        initProgress: function () {
            var me = this;
            this.$progressBar = $('<div id="pdf-progress-bar"><div class="progress-text"></div><div class="progress-bar"><div style="width:0%">&nbsp;</div><span>&nbsp;</span></div></div>');
            $('body').append(this.$progressBar);

            var viewerLoadedDeferred = new $.Deferred();
            var documentLoadedDeferred = new $.Deferred();
            this.$progressBar.find('.progress-text').text(i18n.t('initialize.default'));
            this.$progressBar.find('.progress-bar div').css({ width: 100 + '%' });

            $(document).on('workerLoadingProgress', function (e, progress) {
                var failed = me.$progressBar.hasClass('document-failed');
                var pro_per = Math.round(progress * 100);
                var finished = progress >= 1 && !failed;

                if (pro_per > 0 && !failed && !finished) {
                    me.$progressBar.find('.progress-text').text(i18n.t('initialize.pnacl') + pro_per + '%');
                }
                me.$progressBar.find('.progress-bar div').css({ width: pro_per + '%' });
                if (progress >= 1 && !failed) {
                    viewerLoadedDeferred.resolve();
                    me.$progressBar.find('.progress-text').text(i18n.t('initialize.default') + ' ' + pro_per + '%');
                }
            }).on('documentLoadingProgress', function (e, bytesLoaded, bytesTotal) {
                var loadedPercent = -1;
                if (bytesTotal > 0) {
                    loadedPercent = Math.round(bytesLoaded / bytesTotal * 100);
                }

                if (viewerLoadedDeferred.state() !== 'pending') {
                    //viewer is already, so show document progress

                    if (loadedPercent >= 0) {
                        if (!me.$progressBar.hasClass('document-failed')) {
                            me.$progressBar.find('.progress-text').text(i18n.t('initialize.loadDocument') + loadedPercent + '%');
                        }
                        me.$progressBar.find('.progress-bar div').css({ width: loadedPercent + '%' });
                    } else {
                        var kbLoaded = Math.round(bytesLoaded / 1024);
                        if (!me.$progressBar.hasClass('document-failed')) {
                            me.$progressBar.find('.progress-text').text(i18n.t('initialize.loadDocument') + kbLoaded + 'KB');
                            me.$progressBar.find('.progress-bar div').css({ width: '100%' });
                        }
                    }
                }

                if (bytesLoaded === bytesTotal) {
                    documentLoadedDeferred.resolve();
                }
            });

            $(document).on('documentLoaded', function () {
                //viewer ready
                if (!me.$progressBar.hasClass('document-failed')) {
                    me.$progressBar.fadeOut();
                    clearTimeout(me.initialProgressTimeout);
                }
            });

            this.onError = function (e, type, msg, userMsg) {
                me.$progressBar.find('.progress-text').text(userMsg);
                me.$progressBar.addClass('document-failed');
                me.$progressBar.show();
                clearTimeout(me.initialProgressTimeout);
            };

            me.$progressBar.hide();

        },

        showProgress: function () {
            var me = this;
            this.$progressBar.hide();

            this.initialProgressTimeout = setTimeout(function() {
                me.$progressBar.fadeIn('slow');
                // need to make sure that the document failed class has been removed
                // since we are now loading a new document
                me.$progressBar.removeClass('document-failed');
            }, 1000);
        },

        parseFileName: function(fullPath) {
            var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
            var filename = fullPath.substring(startIndex);
            if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
                this.filename = filename.substring(1);
            }
            else {
                this.filename = filename;
            }
        },

        listener: function (e) {
            var files = e.target.files;
            if (files.length === 0) {
                return;
            }
            this.loadLocalFile(files[0], {
                filename: this.parseFileName(document.getElementById('input-pdf').value)
            });
        },

        closeDocument: function() {
            exports.DesktopReaderControl.prototype.closeDocument.call(this);
            this.$progressBar.removeClass('document-failed');
            this.$progressBar.hide();
        }
    };

    exports.ReaderControl.prototype = $.extend({}, exports.DesktopReaderControl.prototype, exports.ReaderControl.prototype);

})(window);

$('#slider').addClass('hidden-lg');
$('#searchControl').parent().addClass('hidden-md');
$('#control').css('min-width', 680);
$('head').append($('<link rel="stylesheet" type="text/css" />').attr('href', 'pdf/PDFReaderControl.css'));

//# sourceURL=PDFReaderControl.js
