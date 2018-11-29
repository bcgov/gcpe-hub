/*!
* <copyright file="BlockBlobUpload.js" company="Microsoft Corporation">
*  Copyright 2011 Microsoft Corporation
* </copyright>
* Licensed under the MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (the "License"); 
* You may not use this file except in compliance with the License. 
*/

/*global Node, FormData, $, document, setTimeout, request, window*/
var uploader;
var jqxhr;
var maxRetries = 3;
var retryAfterSeconds = 5;
var operationType = {
    "METADATA_SEND": 0,
    "CANCELLED": 1,
    "RESUME_UPLOAD": 2,
    "METADATA_FAILED": 3,
    "FILE_NOT_SELECTED": 4,
    "UNSUPPORTED_BROWSER": 5,
    "ZERO_BYTE_FILE": 6
};

var ChunkedUploader = {
    constructor: function (controlElements) {
        this.files = controlElements.fileControl.files;
        this.fileControl = controlElements.fileControl;
        this.statusLabel = controlElements.statusLabel;
        this.progressElement = controlElements.progressElement;
        this.uploadButton = controlElements.uploadButton;
        this.cancelButton = controlElements.cancelButton;
        this.refreshButton = controlElements.refreshButton;
        this.totalBlocks = controlElements.totalBlocks;
        this.completedFilesBlocks = 0;
        this.releaseNumber = controlElements.releaseNumber;
        this.url = controlElements.url;
        this.error = false;
        this.uploadedFileIndex = -1;
        this.key = controlElements.key;
        this.path = controlElements.path;
        this.blockLength = controlElements.blockLength;
    },
    isElementNode: function (node) {
        return !!(node.nodeType && node.nodeType === Node.ELEMENT_NODE);
    },
    clearChildren: function (node) {
        if (this.isElementNode(node)) {
            while (node.firstChild) {
                node.removeChild(node.firstChild);
            }
        }
    },
    displayStatusMessage: function (message) {
        this.clearChildren(this.statusLabel);
        if (message) {
            this.statusLabel.appendChild(document.createTextNode(message));
        }
    },
    initializeUpload: function () {
        this.displayStatusMessage('');
        //this.uploadButton.setAttribute('disabled', 'disabled');
        this.fileControl.setAttribute('disabled', 'disabled');
        //this.cancelButton.removeAttribute('disabled');
    },
    resetControls: function () {
        this.progressElement.setAttribute('hidden', 'hidden');
       // this.cancelButton.setAttribute('disabled', 'disabled');
        this.fileControl.removeAttribute('disabled');
        //this.uploadButton.removeAttribute('disabled');
        this.fileControl.value = '';
    },
    displayLabel: function (operation) {
        switch (operation) {
            case operationType.METADATA_SEND:
                this.displayStatusMessage('Sending file metadata to server. Please wait..');
                break;
            case operationType.CANCELLED:
                this.displayStatusMessage('File upload has been cancelled.');
                break;
            case operationType.RESUME_UPLOAD:
                this.displayStatusMessage('Error encountered during upload. Resuming upload..');
                break;
            case operationType.METADATA_FAILED:
                this.displayStatusMessage('Failed to send file meta data. Retry after some time.');
                break;
            case operationType.FILE_NOT_SELECTED:
                this.displayStatusMessage('Please select a file to upload.');
                break;
            case operationType.UNSUPPORTED_BROWSER:
                this.displayStatusMessage("Your browser does not support this functionality.");
                break;
            case operationType.ZERO_BYTE_FILE:
                this.displayStatusMessage("File should not be empty.");
                break;
        }
    },
    uploadError: function (message) {
        this.displayStatusMessage('The file could not be uploaded ' + (message ? 'because ' + message : '') + '. Operation aborted.');
        if (jqxhr !== null) {
            jqxhr.abort();
        }
    },
    renderProgress: function (blocksCompleted) {
        this.completedFilesBlocks += blocksCompleted; 
        if (this.completedFilesBlocks > this.totalBlocks) this.completedFilesBlocks = this.totalBlocks;
        var percentCompleted = Math.floor(this.completedFilesBlocks * 100 / this.totalBlocks);
        this.progressElement.removeAttribute('hidden');
        this.progressElement.setAttribute('value', percentCompleted.toString());
        this.displayStatusMessage("Completed: " + percentCompleted + '%');
    },
    refreshPage: function () {
        this.refreshButton.click();
    }
};

var cancelUpload = function () {
    if (jqxhr !== null) {
        jqxhr.abort();
    }
};

var sendFile = function (file, blockLength) {
    var start = 0,
        end = Math.min(blockLength, file.size),
        incrementalIdentifier = 1,
        retryCount = 0,
        sendNextChunk, fileChunk;

    uploader.displayStatusMessage();
    sendNextChunk = function () {
        fileChunk = new FormData();

        if (file.slice) {
            fileChunk.append('Slice', file.slice(start, end));
        }
        else if (file.webkitSlice) {
            fileChunk.append('Slice', file.webkitSlice(start, end));
        }
        else if (file.mozSlice) {
            fileChunk.append('Slice', file.mozSlice(start, end));
        }
        else {
            uploader.displayLabel(operationType.UNSUPPORTED_BROWSER);
            return;
        }
        jqxhr = $.ajax({
            async: true,
            url: uploader.url + "/UploadBlock?id=" + incrementalIdentifier + "&filename=" + file.name,
            data: fileChunk,
            cache: false,
            contentType: false,
            processData: false,
            type: 'POST',
            dataType: "json",
            error: function (request, error) {
                var nextTry;
                if (error !== 'abort' && retryCount < maxRetries) {
                    ++retryCount;
                    nextTry = setTimeout(sendNextChunk, retryAfterSeconds * 1000);
                }

                if (error === 'abort') {
                    uploader.displayLabel(operationType.CANCELLED);
                    uploader.resetControls();
                    uploader = null;
                    isSuccess = false;
                }
                else {
                    if (retryCount === maxRetries) {
                        uploader.uploadError(request.responseText);
                        uploader.resetControls();
                        uploader = null;
                        clearTimeout(nextTry);
                        isSuccess = false;
                    }
                    else {
                        uploader.displayLabel(operationType.RESUME_UPLOAD);
                    }
                }
                
                return;
            },
            success: function (notice) {
                if(!notice.error)
                    uploader.renderProgress(1);
                if (notice.error || notice.isLastBlock) {
                    uploader.displayStatusMessage(notice.message);
                    if (!notice.error && notice.isLastBlock && uploader.files.length > uploader.uploadedFileIndex + 1)
                    {
                        sendNextFile();
                        return;
                    }
                    
                   uploader.resetControls();
                    if(!notice.error)
                        uploader.refreshPage();
                    uploader = null; 
                    return;
                }
                
                ++incrementalIdentifier;
                start = (incrementalIdentifier - 1) * blockLength;
                end = Math.min(incrementalIdentifier * blockLength, file.size);
                retryCount = 0;
                sendNextChunk();
            }
        });
    };

    sendNextChunk();

};

var sendNextFile = function () {
    (uploader.uploadedFileIndex)++;
    var file = uploader.files[uploader.uploadedFileIndex];
    if (typeof file === "undefined" || file.size <= 0) {
        uploader.displayLabel(typeof uploader.files === "undefined" ? operationType.FILE_NOT_SELECTED : operationType.ZERO_BYTE_FILE);
        uploader.refreshPage();
        uploader.resetControls();
        return false;
    }

    uploader.progressElement.setAttribute('value', '0');
    uploader.displayLabel(operationType.METADATA_SEND);
    $.ajax({
        type: "POST",
        async: true,
        contentType: "application/json; charset=utf-8",
        url: uploader.url + "/PrepareMetadata",
        data: JSON.stringify({
            'key': document.getElementById(uploader.key).value,
            'path': document.getElementById(uploader.path).value,
            'blocksCount': Math.ceil(file.size / uploader.blockLength),
            'fileName': file.name,
            'fileSize': file.size,
            'chunkSize': uploader.blockLength
        }),
        dataType: "json",
        error: function () {
            uploader.displayLabel(operationType.METADATA_FAILED);
            uploader.resetControls();
        },
        success: function (data) {
            if (data.d.error != true) {
                if (uploader) {
                    sendFile(file, uploader.blockLength);
                }
            } else {
                uploader.displayStatusMessage(data.d.message);
                uploader.resetControls();
                uploader = null;
            }
        }
    });
};

function startUpload(url, fileElementId, blockLength, uploadProgressElement, statusLabel, uploadButton, cancelButton, key, path, refreshButton) {
    Object.freeze(operationType);
    uploader = Object.create(ChunkedUploader); 
    if (!window.FileList) {
        uploader.statusLabel = document.getElementById(statusLabel);
        uploader.displayLabel(operationType.UNSUPPORTED_BROWSER);
        return;
    }

    uploader.constructor({
        "fileControl": document.getElementById(fileElementId),
        "statusLabel": document.getElementById(statusLabel),
        "progressElement": document.getElementById(uploadProgressElement),
        "uploadButton": document.getElementById(uploadButton),
        "cancelButton": document.getElementById(cancelButton),
        "refreshButton": document.getElementById(refreshButton),
        "blockLength": blockLength,
        "totalBlocks": 0,
        "url": url,
        "key": key,
        "path": path
    });

    uploader.initializeUpload();
    if (uploader.files == null || uploader.files.length <= 0) {
        uploader.displayLabel(operationType.FILE_NOT_SELECTED);
        //uploader.refreshPage();
        uploader.resetControls();
        return;
    }
    if (typeof uploader.files === "undefined" || uploader.files.length <= 0) {
        uploader.displayLabel(typeof uploader.files === "undefined" ? operationType.FILE_NOT_SELECTED : operationType.ZERO_BYTE_FILE);
        //uploader.refreshPage();
        uploader.resetControls();
        return;
    }

    for (i = 0; i < uploader.files.length; i++) {
        blocks = Math.ceil(uploader.files[i].size / blockLength)
        uploader.totalBlocks += blocks == 0 ? 1 : blocks;

    }
    uploader.progressElement.setAttribute('value', '0');

    sendNextFile();

}

