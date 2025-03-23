/// <reference lib="webworker" />

// Rzutujemy `self` na typ `any` albo na bardziej konkretny (ServiceWorkerGlobalScope).
// Wbudowany `self` z "lib.webworker.d.ts" to "WorkerGlobalScope", 
// ale w Service Workerze mamy jeszcze np. `clients`, `registration` itp.
const sw = self as any;

sw.addEventListener('install', (evt: any) => {
    console.log('[SW] Installing...');

    // evt.waitUntil(...) jest dostępne w "ExtendableEvent"
    evt.waitUntil(
        caches.open('pwa-cache-v1').then((cache) => {
            return cache.addAll([
                // Tutaj pliki do pre-cachowania, np. '/index.html'
            ]);
        })
    );
});

sw.addEventListener('activate', (evt: any) => {
    console.log('[SW] Activating...');
    evt.waitUntil(
        // W Service Workerze global scope to "sw" powyżej
        sw.clients.claim()
    );
});

sw.addEventListener('fetch', (evt: any) => {
    // Poniższy kod demonstruje prosty cache-then-network
    evt.respondWith(
        caches.match(evt.request).then((cachedResp: Response | undefined) => {
            return cachedResp || fetch(evt.request);
        })
    );
});

// (opcjonalnie) obsługa background fetch
sw.addEventListener('backgroundfetchsuccess', (event: any) => {
    console.log('[SW] Background fetch success:', event.registration.id);
});

// (opcjonalnie) obsługa backgroundfetchfail
sw.addEventListener('backgroundfetchfail', (event: any) => {
    console.log('[SW] Background fetch fail:', event.registration.id);
});
