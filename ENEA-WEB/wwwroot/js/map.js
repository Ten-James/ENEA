let map;

window.loadMapScript = function (locations) {
    console.log("Loading map script...");
    console.log("Locations:", locations);
    if (!map) {
        console.log("Initializing map...");
        map = L.map('map').setView([0, 0], 2);

        // Add OpenStreetMap tile layer
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);
    }

    for (let i = 0; i < locations.length; i++) {
        const location = locations[i];
        console.log("Adding marker for location:", location);
        const marker = L.marker([location.latitude, location.longitude])
            .addTo(map)
            .bindPopup(location.name);

        marker.on('click', function () {
            // You can either use the direct navigation:
            // window.location.href = `/ChargerGroup/${location.id}`;

            // Or use a short delay to allow popup to show before navigation
            setTimeout(() => {
                window.location.href = `/ChargerGroups/${location.id}`;
            }, 300);
        });
    }

    if (locations.length > 0) {
        const bounds = [];
        for (let i = 0; i < locations.length; i++) {
            bounds.push([locations[i].latitude, locations[i].longitude]);
        }
        map.fitBounds(bounds);
    }
}