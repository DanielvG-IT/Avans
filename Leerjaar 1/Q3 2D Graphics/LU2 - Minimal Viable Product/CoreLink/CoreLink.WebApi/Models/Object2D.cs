using System;
using System.ComponentModel.DataAnnotations;

namespace CoreLink.WebApi.Models
{
    /// <summary>
    /// Represents a 2D object in the environment.
    /// </summary>
    public class Object2D
    {
        /// <summary>
        /// Gets or sets the unique identifier for the object.
        /// </summary>
        [Key]
        public Guid id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the environment the object belongs to.
        /// </summary>
        [Required]
        public Guid environmentId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the prefab used to create the object.
        /// </summary>
        [Required]
        public string? prefabId { get; set; }

        /// <summary>
        /// Gets or sets the X position of the object.
        /// </summary>
        [Required]
        public float positionX { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the object.
        /// </summary>
        [Required]
        public float positionY { get; set; }

        /// <summary>
        /// Gets or sets the X scale of the object.
        /// </summary>
        [Required]
        public float scaleX { get; set; }

        /// <summary>
        /// Gets or sets the Y scale of the object.
        /// </summary>
        [Required]
        public float scaleY { get; set; }

        /// <summary>
        /// Gets or sets the Z rotation of the object.
        /// </summary>
        [Required]
        public float rotationZ { get; set; }

        /// <summary>
        /// Gets or sets the sorting layer of the object.
        /// </summary>
        [Required]
        public int sortingLayer { get; set; }
    }
}