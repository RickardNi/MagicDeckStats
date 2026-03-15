window.DragDropHelper = {
    initializeDragDrop: function (dropZoneElement, fileInputElement, dotNetObjectReference) {
        if (!dropZoneElement) {
            console.error('Drop zone element not found');
            return;
        }

        // Prevent default drag behaviors
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropZoneElement.addEventListener(eventName, preventDefaults, false);
            document.body.addEventListener(eventName, preventDefaults, false);
        });

        // Highlight drop zone when item is dragged over it
        ['dragenter', 'dragover'].forEach(eventName => {
            dropZoneElement.addEventListener(eventName, () => {
                dotNetObjectReference.invokeMethodAsync('OnDragEnter');
            }, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            dropZoneElement.addEventListener(eventName, () => {
                dotNetObjectReference.invokeMethodAsync('OnDragLeave');
            }, false);
        });

        // Handle dropped files
        dropZoneElement.addEventListener('drop', (e) => {
            readAndSendFile(e.dataTransfer.files, dotNetObjectReference);
        }, false);

        // Click on drop zone opens file dialog
        dropZoneElement.addEventListener('click', () => {
            if (fileInputElement) {
                fileInputElement.click();
            }
        }, false);

        // Handle file input selection
        if (fileInputElement) {
            fileInputElement.addEventListener('change', (e) => {
                readAndSendFile(e.target.files, dotNetObjectReference);
                e.target.value = '';
            }, false);
        }

        function readAndSendFile(fileList, dotNetRef) {
            const files = Array.from(fileList);

            if (files.length > 0) {
                const file = files[0];

                if (!file.name.toLowerCase().endsWith('.json')) {
                    dotNetRef.invokeMethodAsync('OnInvalidFileType');
                    return;
                }

                const reader = new FileReader();
                reader.onload = function (event) {
                    // First send lightweight metadata so C# can show "Processing..." immediately
                    dotNetRef.invokeMethodAsync('OnFileReadStart', file.name, file.size).then(() => {
                        // Yield to browser to render the notification, then send the heavy payload
                        setTimeout(() => {
                            dotNetRef.invokeMethodAsync('OnFileContentReady', event.target.result);
                        }, 50);
                    });
                };
                reader.onerror = function () {
                    dotNetRef.invokeMethodAsync('OnFileReadError');
                };
                reader.readAsText(file);
            }
        }

        function preventDefaults(e) {
            e.preventDefault();
            e.stopPropagation();
        }
    },

    disposeDragDrop: function (dropZoneElement) {
        if (dropZoneElement) {
            // Remove all event listeners by cloning the element
            const newElement = dropZoneElement.cloneNode(true);
            dropZoneElement.parentNode.replaceChild(newElement, dropZoneElement);
        }
    }
};
