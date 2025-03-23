self.addEventListener("install", (event) => {
    console.log("Service Worker zainstalowany");
    self.skipWaiting();
});

self.addEventListener("activate", (event) => {
    console.log("Service Worker aktywowany");
    self.clients.claim();
});

// Obsługa wiadomości z Blazora
self.addEventListener("message", async (event) => {
    if (event.data && event.data.type === "UPLOAD_CHUNK") {
        await saveChunkToIndexedDB(event.data);
        await tryUploadChunks();
    }
});

self.addEventListener("message", async (event) => {
    if (event.data && event.data.type === "RESUME_UPLOAD") {
        console.log("Wznawianie przesyłania fragmentów...");
        await tryUploadChunks();
    }
});


// Zapisujemy fragmenty do IndexedDB, aby przetrwały zamknięcie strony
async function saveChunkToIndexedDB(data) {
    const db = await openDB();
    const tx = db.transaction("chunks", "readwrite");
    await tx.store.put(data);
    await tx.done;
}

// Przesyłanie fragmentów z IndexedDB
async function tryUploadChunks() {
    const db = await openDB();
    const tx = db.transaction("chunks", "readonly");
    const chunks = await tx.store.getAll();

    for (const data of chunks) {
        try {
            await uploadChunk(data);
            const txDelete = db.transaction("chunks", "readwrite");
            await txDelete.store.delete(data.chunkIndex);
            await txDelete.done;
        } catch (error) {
            console.error(`Błąd przesyłania fragmentu ${data.chunkIndex}:`, error);
        }
    }
}

// Przesyłanie fragmentu
async function uploadChunk(data) {
    console.log("Przesyłanie fragmentu:", data.chunkIndex);

    let chunkBlob = new Blob([new Uint8Array(data.chunk)], { type: "application/octet-stream" });

    const formData = new FormData();
    formData.append("chunk", chunkBlob, data.fileName);
    formData.append("uploadId", data.uploadId);
    formData.append("destinationPath", data.destinationPath);
    formData.append("chunkIndex", data.chunkIndex);
    formData.append("totalChunks", data.totalChunks);

    const response = await fetch(data.apiUrl, {
        method: "POST",
        body: formData,
        headers: {
            "Authorization": `Bearer ${data.token}`
        }
    });

    if (!response.ok) {
        throw new Error(`Błąd przesyłania fragmentu: ${response.status}`);
    }

    console.log(`Fragment ${data.chunkIndex}/${data.totalChunks} przesłany`);
}

// Otwieranie IndexedDB
async function openDB() {
    return await idb.openDB("UploadDB", 1, {
        upgrade(db) {
            db.createObjectStore("chunks", { keyPath: "chunkIndex" });
        }
    });
}
