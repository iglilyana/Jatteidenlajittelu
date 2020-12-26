using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Widgets;

/// @author Jenni Arovaara
/// @version 4.4.2019
/// <summary>
/// Pelissä lajitellaan jätteitä oikeisiin jäteastioihin. Peli vaikeutuu tason vaihtuessa.
/// </summary>
public class JatteidenLajittelu : PhysicsGame
{
    private int kenttaNro = -1;
    private readonly Roskalista roskat = new Roskalista();
    private readonly List<PhysicsObject> roskikset = new List<PhysicsObject>();
    private IntMeter pisteLaskuri;
    private IntMeter elamaLaskuri;
    private Label roskaNaytto;
    private readonly Random satunnaisuus = new Random();
    private Timer roskienTiputus;
    private const int ELAMAT = 3;
    private const int VALIKONVIIVE = 2;

    private class RoskaTag
    {
        public string Tyyppi;
        public string Nimi;
    }

    /// <summary>
    /// Asetetaan ikkunan ja pelin koko, asetetaan reunat ja luodaan törmäyskäsittelijä.
    /// Asetetaan roskien tiputukselle ajastin.
    /// Kutsutaan pistelaskuri ja näppäimet. Käynnistetään seuraava kenttä ja roskien lisäys.
    /// Asetetaan tekstinäytön sijainti, läpinäkyvyys, fontin koko, viestin kesto ja viestien kerralla näytettävä määärä.
    /// </summary>
    public override void Begin()
    {
        SetWindowSize(1024, 768);
        Level.Size = new Vector(1024, 768);
        Level.CreateHorizontalBorders();
        PhysicsObject alareuna = Level.CreateBottomBorder();
        AddCollisionHandler(alareuna, TuhotaanRoska);
        Camera.ZoomToLevel();

        roskienTiputus = new Timer(5, LisaaRoska);

        MessageDisplay.X = Level.Width / 2 - 100;
        MessageDisplay.Y = Level.Height / 2 - 50;
        MessageDisplay.BackgroundColor = Color.Transparent;
        MessageDisplay.Font = Font.DefaultHuge;
        MessageDisplay.MessageTime = new TimeSpan(0, 0, 1);
        MessageDisplay.MaxMessageCount = 1;

        roskaNaytto = new Label();
        roskaNaytto.X = Screen.Center.X;
        roskaNaytto.Y = Screen.Top - 100;
        roskaNaytto.Font = Font.DefaultLarge;
        roskaNaytto.TextColor = Color.DarkGray;
        roskaNaytto.Color = Color.Transparent;
        Add(roskaNaytto);

        roskat.ValittuMuuttui += PaivitaRoskaNaytto;

        LuoPistelaskurit();
        LuoNappaimet();
        AloitaPeli();
    }

    /// <summary>
    /// Pelin käynnistämiseen liittyvät toimenpiteet, jonka avulla saadaan aloitettua peli uudelleen pelin loputtua.
    /// </summary>
    private void AloitaPeli()
    {
        kenttaNro = -1;
        elamaLaskuri.Value = ELAMAT;
        SeuraavaKentta();
        LisaaRoska();
    }

    /// <summary>
    /// Luodaan kenttä. Asetetaan kentän väri liukuväriksi, asetetaan painovoima, aetetaan roskisten sijainti.
    /// Käynnistetään roskien tiputus.
    /// </summary>
    /// <param name="kenttaNro">Kentän numero eli taso.</param>
    private void LuoKentta(int kenttaNro)
    {
        Kentta kentta = Asetukset.Kentat[kenttaNro];
        Level.Background.CreateGradient(Color.ForestGreen, Color.LightGreen);
        Gravity = new Vector(0, kentta.Painovoima);
        roskienTiputus.Interval = kentta.RoskienTiheys;
        pisteLaskuri.Value = 0;
        pisteLaskuri.MaxValue = kentta.RoskienMaara;

        double jaettuKentta = Screen.Width / (kentta.Roskikset.Length+1);

        for (int i=0; i<kentta.Roskikset.Length; i++)
        {
            LuoRoskis(Screen.Left + jaettuKentta*(i+1), Screen.Bottom + 50, kentta.Roskikset[i]);
        }

        roskienTiputus.Start();
    }

    /// <summary>
    /// Määritellään mitä tapahtuu, kun siirrytään uuteen kenttään.
    /// Tyhjennetään roskien lista, tuhotaan ja siivotaan roskikset pois kentältä.
    /// Jos kentät on loppu, lopetetaan roskien tiputus ja näytetään teksti "Läpäisit pelin!".
    /// Muussa tapauksessa luodaan uusi kenttä.
    /// Nollataan pistelaskuri.
    /// </summary>
    private void SeuraavaKentta()
    {
        kenttaNro++;
        roskat.Tyhjenna();
        roskikset.ForEach(roskis => roskis.Destroy());
        roskikset.Clear();

        if (kenttaNro >= Asetukset.Kentat.Length)
        {
            roskienTiputus.Stop();
            Level.Background.CreateGradient(Color.MediumPurple, Color.HanPurple);
            MessageDisplay.Add("Hyvä!");
            MessageDisplay.Add("Läpäisit pelin!");
            Timer.SingleShot(VALIKONVIIVE, Valikko);
        }
        else
        {
            LuoKentta(kenttaNro);
            MessageDisplay.Add($"Taso {(kenttaNro + 1)}");
        }
    }

    /// <summary>
    /// Luodaan näppäimet, joilla peli toimii.
    /// Näppäinten avulla voi lopettaa pelin, laittaa pelin tauolla tai katsoa näppäinohjeet.
    /// Liikutetaan roskaa nuolinäppäimillä sivuille ja tiputetaan roska välilyönnillä.
    /// </summary>
    private void LuoNappaimet()
    {
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä näppäinohjeet");
        Keyboard.Listen(Key.P, ButtonState.Pressed, Tauko, "Pysäytä peli");

        Keyboard.Listen(Key.Space, ButtonState.Pressed, PudotaRoska, "Tiputa roska");
        Keyboard.Listen(Key.Left, ButtonState.Pressed, LiikutaRoskaa, "Liikuta roskaa vasemmalle", new Vector(-150, 0));
        Keyboard.Listen(Key.Right, ButtonState.Pressed, LiikutaRoskaa, "Liikuta roskaa oikealle", new Vector(150,0));
    }

    /// <summary>
    /// Pelaajan painaessa P-näppäintä, menee peli paussille. Pelaajan painaessa P-näppäintä, jatkaa peli siitä mihin pelaaja jäi.
    /// </summary>
    private void Tauko()
    {
        IsPaused = !IsPaused;
    }

    /// <summary>
    /// Valikko pelin loputtua antaa pelaajalle mahdollisuuden aloittaa pelin alusta tai sulkea pelin.
    /// </summary>
    private void Valikko()
    {
        MultiSelectWindow valikko = new MultiSelectWindow("Kokeile uudestaan", "Pelaa uudestaan", "Lopeta");
        Add(valikko);
        valikko.AddItemHandler(0, AloitaPeli);
        valikko.AddItemHandler(1, Exit);
    }

    /// <summary>
    /// Luodaan pistelaskuri ja elämien määrä. Pistelaskuri laskee kuinka monta roskaa on osunut roskikseen.
    /// Elämälaskuri laskee kuinka monta elämää on jäljellä.
    /// Asetetaan fontin koko, laskureiden sijainti, määritellään laskurin tausta läpinäkyväksi.
    /// </summary>
    private void LuoPistelaskurit()
    {
        pisteLaskuri = new IntMeter(0);
        
        ProgressBar pistePalkki = new ProgressBar(150, 15, pisteLaskuri);
        pistePalkki.X = Screen.Right - 100;
        pistePalkki.Y = Screen.Top - 100;
        pistePalkki.Color = Color.LightGray;
        pistePalkki.BarColor = Color.DarkForestGreen;
        pistePalkki.Tag = "laskuri";
        Add(pistePalkki);

        pisteLaskuri.UpperLimit += SeuraavaKentta;

        elamaLaskuri = new IntMeter(ELAMAT);

        Label elamaNaytto = new Label();
        elamaNaytto.X = Screen.Left + 100;
        elamaNaytto.Y = Screen.Top - 100;
        elamaNaytto.Font = Font.DefaultLarge;
        elamaNaytto.TextColor = Color.Black;
        elamaNaytto.Color = Color.Transparent;
        elamaNaytto.Title = "Elämät";
        elamaNaytto.Tag = "laskuri";

        elamaLaskuri.MinValue = 0;
        elamaLaskuri.LowerLimit += ElamatLoppu;
        elamaNaytto.BindTo(elamaLaskuri);
        Add(elamaNaytto);
    }

    /// <summary>
    /// Päivitetään roskanäyttöön roskan nimi.
    /// </summary>
    private void PaivitaRoskaNaytto()
    {
        roskaNaytto.Text = ((RoskaTag)roskat.Valittu?.Tag)?.Nimi ?? "";
    }

    /// <summary>
    /// Luodaan roskis. Asetetaan roskiksen tyyppi, kuva, koko ja sijainti. 
    /// Tehdään roskiksesta staattinen ja lisätään törmäyskäsittelijä.
    /// </summary>
    /// <param name="peli"></param>
    /// <param name="x">Roskiksen x-koordinaatti.</param>
    /// <param name="y">Roskiksen y-koordinaatti.</param>
    private void LuoRoskis(double x, double y, string tyyppi)
    {
        Image kuva = LoadImage(tyyppi);
        PhysicsObject roskis = new PhysicsObject(kuva.Width / 3, kuva.Height / 3, Shape.FromImage(kuva));
        roskis.Position = new Vector(x, y);
        roskis.Tag = tyyppi;
        roskis.Image = kuva;
        roskis.MakeStatic();
        roskikset.Add(roskis);
        Add(roskis, 1);
        AddCollisionHandler(roskis, TuhotaanRoska);
    }

    /// <summary>
    /// Luodaan roska. Asetetaan roskan tyyppi, kuva, koko ja sijainti.
    /// </summary>
    /// <param name="x">Roskan aloituspisteen x-koordinaatti</param>
    /// <param name="y">Roskan aloituspisteen y-koordinaatti</param>
    /// <param name="tyyppi">Roskan tyyppi</param>
    private PhysicsObject LuoRoska(double x, double y, string tyyppi)
    {
        int kuvanNro = satunnaisuus.Next(1, Asetukset.Roskat[tyyppi].Count + 1);
        string kuvanNimi = $"{tyyppi}-{kuvanNro.ToString().PadLeft(3, '0')}";
        Image kuva = LoadImage(kuvanNimi);
        string roskanNimi = Asetukset.Roskat[tyyppi][kuvanNro - 1];
        PhysicsObject roska = new PhysicsObject(kuva.Width/2, kuva.Height/2, Shape.FromImage(kuva));
        roska.Position = new Vector(x, y);
        roska.Tag = new RoskaTag
        {
            Tyyppi = tyyppi,
            Nimi = roskanNimi
        };
        roska.LinearDamping = 0.95;
        roska.AngularVelocity = satunnaisuus.NextDouble() - 0.5;
        roska.Image = kuva;
        Add(roska, -1);
        return roska;
    }

    /// <summary>
    /// Laitetaan roskia tippumaan satunnaisesta kohtaa ruutua. Arvotaan myös roskan tyyppi satunnaisesti.
    /// </summary>
    private void LisaaRoska()
    {
        string roskanTyyppi = Asetukset.Kentat[kenttaNro]
            .Roskikset[satunnaisuus.Next(Asetukset.Kentat[kenttaNro].Roskikset.Length)];
        PhysicsObject roska = LuoRoska(
            Screen.Left + satunnaisuus.NextDouble()*(Screen.Width -100) +50,
            Screen.Top -50,
            roskanTyyppi);
        roskat.Lisaa(roska);
    }

    /// <summary>
    /// Liikutetaan roskaa sivusuunnassa.
    /// </summary>
    /// <param name="suunta">Roskan liikutuksen suunta.</param>
    private void LiikutaRoskaa(Vector suunta)
    {
        if (roskat.Valittu == null) return;
        roskat.Valittu.Hit(suunta);
    }

    /// <summary>
    /// Pudotetaan roska painamalla välilyöntiä.
    /// </summary>
    private void PudotaRoska()
    {
        if (roskat.Valittu == null) return;
        roskat.Valittu.LinearDamping = 1;
        roskat.Valittu.Hit(new Vector(0, -1000));
        roskat.Poista(roskat.Valittu);
    }

    /// <summary>
    /// Luodaan aliohjelma TuhotaanRoska, jossa kerrotaan mitä tapahtuu, kun roska osuu roskikseen.
    /// Jos roska osuu oikeaan roskikseen, lisätään pistelaskurin arvoa ja lisätään teksti "Hienoa!".
    /// Jos roska osuu väärään roksikseen, vähennetään elämä ja lisätään teksti "Väärin!".
    /// Tuhotaan roska.
    /// </summary>
    /// <param name="roska">Roska</param>
    /// <param name="roskis">Roskis</param>
    private void TuhotaanRoska(PhysicsObject roskis, PhysicsObject roska)
    {
        if (((RoskaTag)roska.Tag).Tyyppi == (string)roskis.Tag)
        {
            pisteLaskuri.Value++;
            MessageDisplay.TextColor = Color.Black;
            MessageDisplay.Add("Hienoa!");
        }
        else
        {
            elamaLaskuri.Value--;
            if (elamaLaskuri.Value != 0)
            {
                MessageDisplay.TextColor = Color.Black;
                MessageDisplay.Add("Väärin!");
            }
        }
        roskat.Poista(roska);
        roska.Destroy();
    }

    /// <summary>
    /// Määritellään mitä tapahtuu, kun elämät loppuuu.
    /// Laitetaan ruudulle teksti "Peli loppui!". Pysäytetään roskien tiputus ja tyhjennetään roskien lista.
    /// </summary>
    private void ElamatLoppu()
    {
        Level.Background.CreateGradient(Color.Red, Color.DarkRed);
        MessageDisplay.Font = Font.DefaultHuge;
        MessageDisplay.TextColor = Color.LightPink;
        MessageDisplay.Add("Peli loppui!");
        roskienTiputus.Stop();
        roskat.Tyhjenna();
        Timer.SingleShot(VALIKONVIIVE, Valikko);
    }
}
