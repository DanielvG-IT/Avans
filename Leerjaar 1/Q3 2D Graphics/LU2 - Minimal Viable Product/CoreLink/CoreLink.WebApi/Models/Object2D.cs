namespace CoreLink.WebApi.Models
{
    public class Object2D
    {
        public Guid Id { get; set; }
        public int EnvironmentId;
        public required string PrefabId;
        public required float PositionX;
        public required float PositionY;
        public required float ScaleX;
        public required float ScaleY;
        public required float RotationZ;
        public required int SortingLayer;
    }
}
