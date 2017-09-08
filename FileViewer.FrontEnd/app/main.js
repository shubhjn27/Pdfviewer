(function () {
    'use strict';
 
    angular.module('app').controller('Main', main);
 
    function main() {
        var vm = this;
        vm.docName = 'MyFile';

        initializeViewer();
    }

    function initializeViewer(){
        var viewerElement = document.getElementById("viewer");
        console.log("inside viewer");
        var libPath = "../Scripts/WebViewer/lib";
        var webViewer = new PDFTron.WebViewer({
            path: libPath,
            type: "html5",
            documentType: "pdf",
            streaming: true,
            initialDoc: "../Scripts/WebViewer/autoPDF.pdf" //viewerModel.ApplicationPath + '/FileItem/' + viewerModel.FileItemId + '/ViewerData/' + token
        }, viewerElement); 

        $("#viewer").on("documentLoaded", function () {
            console.log("document loaded");
        });

        $("#viewer").error(function () {
            console.log("document errored");            
        });   
    }
 
})();