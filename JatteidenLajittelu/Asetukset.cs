using System.Collections.Generic;

/// @author Jenni Arovaara
/// @version 4.4.2019
/// <summary>
/// Pelin asetukset.
/// </summary>
internal static class Asetukset
{
    const string BIO = "bio";
    const string LASI = "lasi";
    const string METALLI = "metalli";
    const string MUOVI = "muovi";
    const string PANTTI = "pantti";
    const string KARTONKI = "kartonki";
    const string PAPERI = "paperi";
    const string SEKA = "seka";

    /// <summary>
    /// Määritellään kentät.
    /// </summary>
    public static Kentta[] Kentat = new Kentta[]{
        new Kentta
        {
            Roskikset = new string[] { BIO, LASI },
            RoskienTiheys = 5,
            Painovoima = -150,
            RoskienMaara = 10,
        },
        new Kentta
        {
            Roskikset = new string[] { BIO, LASI, METALLI },
            RoskienTiheys = 4,
            Painovoima = -180,
            RoskienMaara = 15
        },
        new Kentta
        {
            Roskikset = new string[] { BIO, LASI, METALLI, PAPERI },
            RoskienTiheys = 3,
            Painovoima = -220,
            RoskienMaara = 20
        },
        new Kentta
        {
            Roskikset = new string[] { BIO, LASI, METALLI, PAPERI, KARTONKI },
            RoskienTiheys = 3,
            Painovoima = -280,
            RoskienMaara = 25
        },
        new Kentta
        {
            Roskikset = new string[] { BIO, LASI, METALLI, PAPERI, KARTONKI, SEKA },
            RoskienTiheys = 2.5,
            Painovoima = -320,
            RoskienMaara = 25
        },
        new Kentta
        {
            Roskikset = new string[] { BIO, LASI, METALLI, PAPERI, KARTONKI, MUOVI },
            RoskienTiheys = 2.5,
            Painovoima = -360,
            RoskienMaara = 30
        },
        new Kentta
        {
            Roskikset = new string[] { BIO, LASI, METALLI, PAPERI, KARTONKI, MUOVI, PANTTI },
            RoskienTiheys = 2.5,
            Painovoima = -400,
            RoskienMaara = 35
        }
    };

    /// <summary>
    /// Määritellään roskatyypit ja roskien nimet.
    /// </summary>
    public static Dictionary<string, List<string>> Roskat = new Dictionary<string, List<string>>()
    {
        { BIO, new List<string> {"banaani", "talouspaperi", "omena", "leipä", "perunankuoret"} },
        { LASI, new List<string> {"pantiton lasipullo", "lasipurkki", "pantiton lasipullo", "pantiton lasipullo", "lasipurkki"} },
        { KARTONKI, new List<string> {"maitopurkki", "maitopurkki", "maitopurkki", "jogurttipurkki", "jogurttipurkki" } },
        { METALLI, new List<string> {"säilykepurkki", "kattila", "vääntynyt haarukka", "veitsi", "foliorasia"} },
        { PANTTI, new List<string> {"tölkki", "pantillinen vesipullo", "tölkki", "tölkki", "pantillinen vichypullo"} },
        { PAPERI, new List<string> {"sanomalehti", "kirjekuori", "piirustus", "sudokulehti", "ohjevihkonen"} },
        { SEKA, new List<string> {"hehkulamppu", "laastari", "kakka", "tupakka", "tuhka"} },
        { MUOVI, new List<string> {"jogurttipikari", "muovipussi", "muovirasia", "jogurttipikari", "voirasia"} }
    };
}
