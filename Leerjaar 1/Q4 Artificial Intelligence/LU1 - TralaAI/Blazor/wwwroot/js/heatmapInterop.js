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
            { x: 0.42, y: 0.50, value: 4 },  // 42% from left, 50% from top
            { x: 0.60, y: 0.42, value: 2 },
            { x: 0.30, y: 0.58, value: 3 }
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
