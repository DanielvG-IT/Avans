namespace Api.Models.Enums.DTO
{
    public enum LitterType
    {
        AluminiumFoil,
        BottleCap,
        Bottle,
        BrokenGlass,
        Can,
        Carton,
        Cigarette,
        Cup,
        Lid,
        OtherLitter,
        OtherPlastic,
        Paper,
        PlasticBagWrapper,
        PlasticContainer,
        PopTab,
        Straw,
        StyrofoamPiece,
        UnlabelledLitter,
        // Added new items if not duplicates
        PlasticBag_Wrapper, // "Plastic bag - wrapper" (distinct from PlasticBagWrapper)
        Karton // "Karton" (distinct from Carton)
    }
}