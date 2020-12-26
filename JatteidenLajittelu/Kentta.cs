/// @author Jenni Arovaara
/// @version 4.4.2019
/// <summary>
/// Kentän määritykset.
/// </summary>
internal class Kentta
{
    /// <summary>
    /// Taulukko kentän roskiksista.
    /// </summary>
    public string[] Roskikset;

    /// <summary>
    /// Tiheys sekunteina, jolla kentän roskat tippuu.
    /// </summary>
    public double RoskienTiheys;

    /// <summary>
    /// Painovoiman voimakkuus, jolla kentän roskat tippuu.
    /// </summary>
    public int Painovoima;

    /// <summary>
    /// Roskien oikeaan roskikseen osumisten määrä, joka kentän läpipäisyyn vaaditaan.
    /// </summary>
    public int RoskienMaara;
}
