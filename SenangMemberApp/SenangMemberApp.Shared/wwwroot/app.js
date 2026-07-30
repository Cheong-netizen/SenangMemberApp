window.addAppointmentIdToInput = function (shortCode) {
    const input = document.getElementById('chat-user-input');
    if (!shortCode || !input || input.disabled) return;

    const selectedCodes = input.value.split(',').map(code => code.trim()).filter(Boolean);
    if (!selectedCodes.some(code => code.toLowerCase() === shortCode.toLowerCase())) {
        selectedCodes.push(shortCode);
        input.value = selectedCodes.join(', ');
        input.dispatchEvent(new Event('input', { bubbles: true }));
        input.dispatchEvent(new Event('change', { bubbles: true }));
    }

    input.focus();
};

document.addEventListener('click', function (event) {
    const button = event.target.closest('.btn-add-appt');
    if (!button) return;

    const shortCode = button.dataset.appointmentShortCode;
    window.addAppointmentIdToInput(shortCode);
});

window.chatScroll = {
    initialize(containerId, buttonId) {
        const container = document.getElementById(containerId);
        const button = document.getElementById(buttonId);
        if (!container || !button) return false;

        if (container.__chatScrollHandler) {
            container.removeEventListener('scroll', container.__chatScrollHandler);
        }

        const updateButton = () => {
            const distanceFromBottom =
                container.scrollHeight - container.scrollTop - container.clientHeight;
            button.classList.toggle('visible', distanceFromBottom > 96);
        };

        container.__chatScrollHandler = updateButton;
        container.addEventListener('scroll', updateButton, { passive: true });
        updateButton();
        return true;
    },

    scrollToBottom(containerId, smooth) {
        const container = document.getElementById(containerId);
        if (!container) return;

        container.scrollTo({
            top: container.scrollHeight,
            behavior: smooth ? 'smooth' : 'auto'
        });
    }
};

window.audioRecorder = (() => {
    let mediaRecorder;
    let mediaStream;
    let audioChunks = [];

    const stopTracks = () => {
        mediaStream?.getTracks().forEach(track => track.stop());
        mediaStream = undefined;
    };

    const getSupportedMimeType = () => {
        if (typeof MediaRecorder === 'undefined' || typeof MediaRecorder.isTypeSupported !== 'function') {
            return undefined;
        }

        return [
            'audio/webm;codecs=opus',
            'audio/webm',
            'audio/mp4'
        ].find(mimeType => MediaRecorder.isTypeSupported(mimeType));
    };

    return {
        async start() {
            if (mediaRecorder?.state === 'recording') {
                return true;
            }

            if (!navigator.mediaDevices?.getUserMedia || typeof MediaRecorder === 'undefined') {
                return false;
            }

            try {
                mediaStream = await navigator.mediaDevices.getUserMedia({ audio: true });
                audioChunks = [];

                const mimeType = getSupportedMimeType();
                mediaRecorder = mimeType
                    ? new MediaRecorder(mediaStream, { mimeType })
                    : new MediaRecorder(mediaStream);

                mediaRecorder.addEventListener('dataavailable', event => {
                    if (event.data.size > 0) {
                        audioChunks.push(event.data);
                    }
                });

                mediaRecorder.start();
                return true;
            } catch {
                stopTracks();
                mediaRecorder = undefined;
                return false;
            }
        },

        async stopAndGetBase64() {
            if (!mediaRecorder || mediaRecorder.state === 'inactive') {
                stopTracks();
                mediaRecorder = undefined;
                return '';
            }

            return new Promise(resolve => {
                const recorder = mediaRecorder;
                let isSettled = false;

                const finish = value => {
                    if (isSettled) return;
                    isSettled = true;
                    clearTimeout(stopTimeout);
                    cleanup();
                    resolve(value);
                };

                const handleStop = async () => {
                    try {
                        const mimeType = recorder.mimeType || audioChunks[0]?.type || 'audio/webm';
                        const apiMimeType = mimeType.split(';', 1)[0].trim().toLowerCase() || 'audio/webm';
                        const blob = new Blob(audioChunks, { type: mimeType });
                        const base64Data = await new Promise((resolveData, reject) => {
                            const reader = new FileReader();
                            reader.onloadend = () => resolveData(reader.result?.toString().split(',')[1] ?? '');
                            reader.onerror = reject;
                            reader.readAsDataURL(blob);
                        });

                        // The API uses new MediaTypeHeaderValue(mimeType), which accepts
                        // the media type but rejects recorder parameters such as
                        // ";codecs=opus". The bytes remain WebM/Opus; only the HTTP
                        // content-type value sent to the API is normalized.
                        finish(JSON.stringify({ base64Data, mimeType: apiMimeType }));
                    } catch {
                        finish('');
                    }
                };

                const handleError = () => {
                    finish('');
                };

                const cleanup = () => {
                    recorder.removeEventListener('stop', handleStop);
                    recorder.removeEventListener('error', handleError);
                    stopTracks();
                    if (mediaRecorder === recorder) {
                        mediaRecorder = undefined;
                    }
                    audioChunks = [];
                };

                // Some browsers occasionally fail to emit "stop". Never leave the
                // Blazor UI awaiting this promise indefinitely.
                const stopTimeout = setTimeout(() => finish(''), 10000);

                recorder.addEventListener('stop', handleStop);
                recorder.addEventListener('error', handleError);

                try {
                    recorder.requestData();
                } catch {
                    // Some MediaRecorder implementations do not support an
                    // explicit data flush; stopping still emits the final data.
                }

                try {
                    recorder.stop();
                } catch {
                    finish('');
                }
            });
        }
    };
})();
