window.heatmapInterop = {
  drawMap: function (points) {
    if (typeof h337 === "undefined") {
      console.error("h337 (heatmap.js) is not loaded yet.");
      return;
    }

    console.log("heatmapInterop.drawMap called with points:", points);

    const container = document.getElementById("heatmapContainer");
    if (!container) {
      console.error("heatmapContainer not found.");
      return;
    }

    // Use getBoundingClientRect() to account for zoom level and actual rendered size
    const rect = container.getBoundingClientRect();
    const width = rect.width;
    const height = rect.height;

    // Clear previous heatmap instance if necessary
    if (container._heatmapInstance) {
      container._heatmapInstance.setData({ max: 0, data: [] });
      container._heatmapInstance = null;
    }

    // Create heatmap instance
    const heatmapInstance = h337.create({
      container: container,
      radius: 40,
      maxOpacity: 0.6,
      minOpacity: 0,
      blur: 0.85,
    });

    // Convert percentage positions to pixel values
    const pixelData = points.map((p) => ({
      x: Math.round(p.x * width),
      y: Math.round(p.y * height),
      value: p.Value,
    }));

    // Use the created heatmap instance
    const heatmap = heatmapInstance;
    console.log("min:", minValue, "max:", maxValue);

    heatmap.setData({
      max: maxValue,
      min: minValue,
      data: pixelData,
    });

    // Store the instance for reuse/reset later
    container._heatmapInstance = heatmapInstance;
  },
};

// Optional: auto-enable once everything is ready
window.addEventListener("load", () => {
  console.log("Window loaded, ready for JS interop.");
});

// Redraw heatmap on window resize to handle zoom or container size changes
window.addEventListener("resize", () => {
  const container = document.getElementById("heatmapContainer");
  if (!container || !container._heatmapPoints) return;

  // Redraw the map with the stored points
  window.heatmapInterop.drawMap(container._heatmapPoints);
});

// Example: you should call this before drawing to store the points
window.heatmapInterop.setPoints = function (points) {
  const container = document.getElementById("heatmapContainer");
  if (container) {
    container._heatmapPoints = points;
  }
};
