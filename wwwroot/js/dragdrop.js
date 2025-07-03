window.DragDropHelper = {
    initializeDragDrop: function (dropZoneElement, dotNetObjectReference) {
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
            const files = Array.from(e.dataTransfer.files);
            
            if (files.length > 0) {
                const file = files[0];
                
                // Check if it's a JSON file
                if (!file.name.toLowerCase().endsWith('.json')) {
                    dotNetObjectReference.invokeMethodAsync('OnInvalidFileType');
                    return;
                }

                // Read the file content
                const reader = new FileReader();
                reader.onload = function(event) {
                    const fileContent = event.target.result;
                    const fileInfo = {
                        name: file.name,
                        size: file.size,
                        content: fileContent
                    };
                    dotNetObjectReference.invokeMethodAsync('OnFileDropped', fileInfo);
                };
                reader.onerror = function() {
                    dotNetObjectReference.invokeMethodAsync('OnFileReadError');
                };
                reader.readAsText(file);
            }
        }, false);

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
