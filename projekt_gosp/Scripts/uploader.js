
function fileUpload() {

}

fileUpload.prototype.addRemoveFileListener = function (button) {
    $(button).on('click', function () {
        var row = $(this).parents('.row').eq(0);
        $(row).remove();
    });
};

fileUpload.prototype.addFileThumbnail = function (row, html) {
    $(row).append(html);
    var button = $(row).find('.icon');
    this.addRemoveFileListener(button);
};

fileUpload.prototype.createFileInput = function (fileDiv, callback) {
    var html = "<div class='row fileUpload fileUploadFix'>" +
                    "<input name='file' type='file'/>" +
                "</div>";

    $(fileDiv).append(html); // add new file input with its div

    var fileInput = $(fileDiv).find('.row').eq(-1).find("input[type='file']"); // find this file input

    var self = this;

    $(fileInput).on('change', function (e) {
        self.fileUploadHandler(e, this, callback); // file handler
    });

    //asasddsadasdas
    $(fileInput).on('focus', function (e) {
        self.fileUploadHandler(e, this, callback); // file handler
    });

    $(fileInput).trigger('click'); // open file choose dialog
};


//todo: information about files size 
fileUpload.prototype.fileUploadHandler = function (e, fileInput, callback) {
    var row = $(fileInput).parents('.row').eq(0);

    if (!e.target.files) {
        $(row).remove();
        return 0;
    }

    var file = e.target.files[0]; // FileList object

    if (typeof file === "undefined" || file === null) {
        $(row).remove();
        return 0;
    }

    //if file is not a image/video then return 0
    //if (!file.type.match('image.*') && !file.type.match('video.*')) 
    if (!file.type.match('image.*')) {
        alert("you can upload only images");
        $(row).remove();
        return 0;
    }

    var name = e.target.files[0].name;
    console.log(file.type);
    // for non supporting file reader browsers and also for videos
    // if (!window.FileReader || file.type.match('video.*')
    if (!window.FileReader) {

        var html = name + "<img class='icon' src='/Media/remove.png'>";
        this.addFileThumbnail(row, html);

    } else {
        var self = this;
        var reader = new FileReader();
        reader.readAsDataURL(file);

        reader.addEventListener('progress', function (evt) {
            if (evt.lengthComputable) {
                var percentComplete = Math.round((evt.loaded / evt.total) * 100) + "%";
                console.log(percentComplete);
            }
        }, false);

        reader.onload = function (e) {
            if (typeof callback === 'undefined') {
                var html = "<img class='uploadThumbnail' src=\"" + e.target.result + "\">" + "<br/>"
                                    + name + "<img class='icon' src='/Media/remove.png'>";
                self.addFileThumbnail(row, html);
            } else {
                callback(e.target.result);
            }
        }

    };
};

// simple ajax call
function ajaxCall(type, url, data, callbackOnSucces, callbackOnError, additionalData, divToPlaceProgressBar) {
    $.ajax({
        type: type,
        url: url,
        data: data,
        cache: false,
        processData: false,
        contentType: false,
        xhr: function () {
            var xhr = new window.XMLHttpRequest();

            if (divToPlaceProgressBar != null && typeof divToPlaceProgressBar !== 'undefined') {
                //upload progress
                xhr.upload.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        var percentComplete = Math.round((evt.loaded / evt.total) * 100) + "%";
                        console.log(percentComplete);
                        $(divToPlaceProgressBar).find("#progressBar").css("width", percentComplete);
                        $(divToPlaceProgressBar).find("#progressBar").text(percentComplete);
                    }
                }, false);
            }
            return xhr;
        },
        beforeSend: function () {
            if (divToPlaceProgressBar != null && typeof divToPlaceProgressBar !== 'undefined') {
                var progress = '<div id="progressDiv" class="progress">' +
                                    '<div id="progressBar" class="progress-bar progress-bar-info progress-bar-striped" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;">' +
                                        '0%' +
                                    '</div>' +
                                '</div>';
                $(divToPlaceProgressBar).append(progress);
            }
        },
        complete: function () {
            if (divToPlaceProgressBar != null && typeof divToPlaceProgressBar !== 'undefined') {
                $(divToPlaceProgressBar).find("#progressDiv").remove();
            }
        },
        error: function () {
            if (callbackOnError !== null) {
                callbackOnError();
            }
        },
        success: function (data) {
            if (additionalData === null) {
                callbackOnSucces(data);
            } else {
                callbackOnSucces(data, additionalData);
            }
        }
    });
}