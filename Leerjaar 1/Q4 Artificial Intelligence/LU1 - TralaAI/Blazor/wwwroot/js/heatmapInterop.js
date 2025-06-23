window.heatmapInterop = {
    drawMap: function () {
        if (typeof h337 === "undefined") {
            console.error("h337 (heatmap.js) is not loaded yet.");
            return;
        }

        console.log("heatmapInterop.drawMap called");

        const container = document.getElementById("heatmapContainer");
        if (!container) {
            console.error("heatmapContainer not found.");
            return;
        }

        const width = container.offsetWidth;
        const height = container.offsetHeight;

        // Create heatmap instance
        const heatmapInstance = h337.create({
            container: container,
            radius: 40,
            maxOpacity: 0.6,
            minOpacity: 0,
            blur: 0.85
        });

        // Your percentage-based data points
        const rawData = [
            { x: 0.50, y: 0.55, value: 5 }, //mc               
            { x: 0.80, y: 0.65, value: 4 }, //avans 
            { x: 0.65, y: 0.90, value: 3 }, //suburban
            { x: 0.25, y: 0.10, value: 3 }  //bedrijven
        ];

        // Convert percentage positions to pixel values
        const pixelData = rawData.map(p => ({
            x: Math.round(p.x * width),
            y: Math.round(p.y * height),
            value: p.value
        }));

        // Build final data object
        const data = {
            max: 5,
            data: pixelData
        };

        heatmapInstance.setData(data);
    }
};

// Optional: auto-enable once everything is ready
window.addEventListener("load", () => {
    console.log("Window loaded, ready for JS interop.");
});

//avans = 0.80 0.65
//mcdonalds = 0.50 0.55
//kievietstraat = 0.65 0.90
//bedrijventerrein = 0.25 0.10
