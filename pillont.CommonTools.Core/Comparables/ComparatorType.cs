namespace pillont.CommonTools.Core.Comparables
{
    /// <summary>
    /// s'applique de la valeur vers le filtre
    /// EXEMPLE : valeur 12 / filter 14 => LessThan : true
    /// </summary>
    public enum ComparatorType
    {
        Equal = 0,
        LessThan = 1,
        GreaterThan = 2,

        LessThanOrEqual = 3,
        GreaterThanOrEqual = 4,
    }
}
