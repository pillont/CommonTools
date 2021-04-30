namespace pillont.CommonTools.Core.Comparables.Dates
{
    /// <summary>
    /// date par default pour pas avoir besoin de le préciser dans tous les jsons
    /// alors que c est le comportement voulu quasiment tout le temps
    /// </summary>
    public enum DateType
    {
        /// <summary>
        /// compare les dates sans prendre en compte les heures
        ///
        /// VRAI : 12/12/2020 00:00:00 == 12/12/2020
        /// VRAI : 12/12/2020 00:00:00 == 12/12/2020 12:00:00
        /// </summary>
        Date = 0,

        /// <summary>
        /// compare les dates en prenant compte des heures
        ///
        /// VRAI : 12/12/2020 00:00:00 == 12/12/2020
        /// FAUX : 12/12/2020 00:00:00 == 12/12/2020 12:00:00
        /// </summary>
        DateTime = 1,
    }
}
