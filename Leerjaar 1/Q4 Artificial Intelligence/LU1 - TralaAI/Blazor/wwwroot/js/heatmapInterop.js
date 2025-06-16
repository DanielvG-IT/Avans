window.heatmapInterop = {
    renderHeatmap: function (dataPoints) {
        // Clear previous heatmap container content if any
        const container = document.getElementById('heatmapContainer');
        container.innerHTML = "";

        // Create heatmap instance
        var heatmapInstance = h337.create({
            container: container,
            radius: 40,
            maxOpacity: 0.6,
            minOpacity: 0,
            blur: 0.85,
            // Optional: gradient colors
            gradient: {
                // red for high intensity
                '.1': 'blue',
                '.5': 'lime',
                '.8': 'red'
            }
        });

        // Set heatmap data with max intensity and points
        heatmapInstance.setData({
            max: 10,
            data: dataPoints
        });
    },

    drawMap: function () {
        console.log("heatmapInterop.drawMap called");

        const container = document.getElementById("heatmapContainer");
        if (!container) {
            console.error("heatmapContainer not found");
            return;
        }

        const dataPoints = [
            { x: 100, y: 100, value: 5 },
            { x: 150, y: 120, value: 8 },
            { x: 200, y: 180, value: 10 },
            { x: 250, y: 150, value: 6 },
            { x: 300, y: 200, value: 7 },
            { x: 350, y: 250, value: 9 }
        ];

        this.renderHeatmap(dataPoints);
    }

};
