using System;
using System.Text;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

/// @author Minea Nupponen, Emilia Rantonen
/// @version 19.11.2021
/// <summary>
/// Peli, jossa kuljetaan labyrintissa ja kerätään avaimia
/// Kuvat Pixabay sivustolta, joka toimii Pixabay License lisenssin alla.
/// </summary>

public class labyrintti : PhysicsGame
{
    private PhysicsObject hahmo;
    private IntMeter pistelaskuri;
    private static readonly Image avaimenKuva = LoadImage("avain.png");
    private static readonly Image seinanKuva = LoadImage("seinä2.png");
    private static readonly Image pahisKuva = LoadImage("pahis.png");


    private static readonly String[] taso1 = {
                  "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXYYX",
                  "X            X                  X",
                  "X            X                  X",
                  "X     X              X          X",
                  "X     X              X          X",
                  "X     XXXXXXXXXXXXXXXXXXXXXXXXXXX",
                  "X                               X",
                  "X                               X",
                  "XXXXXXXXXXXXXXXXXXXXXXXXXXXX    X",
                  "X                          X    X",
                  "X                          X    X",
                  "X             X     XXX    X    X",
                  "XXXXXXXXX     X      X     X    X",
                  "X       X     X      X          X",
                  "X  *    X     X      X          X",
                  "X       X     X      X          X",
                  "XXX     X     XXXXXXXXXXXXXXXXXXX",
                  "X       X     X *               X",
                  "X       X     X                 X",
                  "X    XXXX     XXXXXXXX   XXXXXXXX",
                  "X                 X             X",
                  "X                 X             X",
                  "XXXXXXXXXXXXX     X     X       X",
                  "X           X           X       X",
                  "X     p     X           X   *   X",
                  "X    XXX    XXXXXXXX    XXXXXXXXX",
                  "X                               X",
                  "X                               X",
                  "X O XXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                  };

    private static readonly String[] taso2 = {
                  "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXYYX",
                  "X             X           X     X",
                  "X             X           XXX   X",
                  "XXXXX   X     X   X   X   X     X",
                  "X       X         X   X   X     X",
                  "X       X         X   X   X   XXX",
                  "X   XXXXXXXXXXXXXXX   X   X     X",
                  "X   X>                X   X     X",
                  "X   X *               X   XXX   X",
                  "X   XXXXXXXXXXXXXXXXXXX         X",
                  "X   XVVVVVVVVVVVVVVVVVX         X",
                  "X   X                 X^^^^^^^^^X",
                  "X                     XXXXXXXXXXX",
                  "X             X   X             X",
                  "X   XXXXXXXXXXX   X             X",
                  "X        X        X    XXXXX    X",
                  "X        X        X    X * X    X",
                  "XXXXX    XXXXXXXXXX    X   X    X",
                  "X>             *  X    X        X",
                  "X>                X    X        X",
                  "XXXXXXXXXXXXXXXXXXX    XXXXXXXXXX",
                  "X V  V  X       X               X",
                  "X       X       X               X",
                  "X       X   X   X   XXXXXXXXX   X",
                  "X   X   X * X   X   X * X   X   X",
                  "X   X   XXXXX   X   X   X   X   X",
                  "X   X               X           X",
                  "X   X^              X           X",
                  "X O XXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                  };

    private static readonly String[] taso3 = {
                  "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXYYX",
                  "X             X           X     X",
                  "X   p         X           X     X",
                  "XXXXX   X     X   X   X   XXX   X",
                  "X       X         X   X   X     X",
                  "X       X         X   X   X     X",
                  "X   XXXXXXXXXXXXXXX   X   X   XXX",
                  "X   X>                X   X     X",
                  "X   X *               X   X     X",
                  "X   XXXXXXXXXXXXXXXXXXX   XXX   X",
                  "X   XVVVVVVVVVVVVVVVVVX         X",
                  "X   X                 X         X",
                  "X                     XXXXXXXXXXX",
                  "X             X   X             X",
                  "X   XXXXXXXXXXX   X             X",
                  "X        X        X    XXXXX    X",
                  "X        X   p    X    X * X    X",
                  "XXXXX    XXXXXXXXXX    X   X    X",
                  "X                 X    X        X",
                  "X *    p  p  p    X    X        X",
                  "XXXXXXXXXXXXXXXXXXX    XXXXXXXXXX",
                  "X V  V  X       X               X",
                  "X       X       X               X",
                  "X       X   X   X   XXXXXXXXX   X",
                  "X   X   X * X   X   X * X   X   X",
                  "X   X   XXXXX   X   X   X   X   X",
                  "X   X               X           X",
                  "X   X^              X           X",
                  "X O XXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                  };


    private static readonly string[][] tasolista = { taso1, taso2, taso3 };
    private static readonly int[] avaintenMaara = { 3, 5, 5 };

    private static readonly int tileWidth = 600 / tasolista[0].Length;
    private static readonly int tileHeight = 480 / tasolista[0].Length;

    private readonly IntMeter tasoNr = new IntMeter(0, 0, 3);

    private const double KIVEN_TN = 80;
    private const double PAHIKSEN_NOPEUS = 25;
    private const double KIVEN_KOKO = 11;
    private const double HAHMON_NOPEUS = 100;

    /// <summary>
    /// Aloitetaan peli
    /// </summary>
    public override void Begin()
    {

        AloitaUusiPeli();

    }


    /// <summary>
    /// Aloitetaan peli ensimmäisen kerran
    /// </summary>
    private void AloitaUusiPeli()
    {
        tasoNr.Value = 0;
        LuoTaso();
    }


    /// <summary>
    /// Aloitetaan uusi taso tai sama taso alusta
    /// </summary>
    private void LuoTaso()
    {
        ClearGameObjects();
        ClearControls();

        if (tasoNr.Value >= tasolista.Length)
        {
            MessageDisplay.Add("Läpäisit pelin!");
            ConfirmExit();
        }
        else
        {
            ClearAll();
            LuoKentta();
            Level.CreateBorders();
            Camera.Follow(hahmo);
            Camera.Zoom(3);

            AsetaOhjaimet();
            LuoPistelaskuri();
        }
    }


    /// <summary>
    /// Luodaan pelikenttä
    /// </summary>
    private void LuoKentta()
    {
        Level.Background.CreateGradient(Color.Gray, Color.White);
        int numero = tasoNr;
        string[] tasonKuva = tasolista[numero];

        ArvoKivet(tasonKuva);

        
        TileMap tiles = TileMap.FromStringArray(tasonKuva);
        tiles.SetTileMethod('X', LuoSeina);
        tiles.SetTileMethod('O', LuoHahmo, Color.Black);
        tiles.SetTileMethod('*', LuoAvain);
        tiles.SetTileMethod('Y', Maali, Color.Brown);
        tiles.SetTileMethod('^', LuoPiikki, Angle.FromDegrees(0), Color.Black);
        tiles.SetTileMethod('>', LuoPiikki, Angle.FromDegrees(-90), Color.Black);
        tiles.SetTileMethod('V', LuoPiikki, Angle.FromDegrees(180), Color.Black);
        tiles.SetTileMethod('p', Pahis);
        tiles.SetTileMethod('k', LuoKivi);
        tiles.Execute(tileWidth, tileHeight);
    }


    /// <summary>
    /// luodaan satunnaisia kiviä pelikentälle silmukan avulla
    /// </summary>
    /// <param name="tasonKuva">kertoo aliohjelmalle missä tasossa mennään</param>
    private void ArvoKivet(string[] tasonKuva)
    {
        for (int i = 0; i < tasonKuva.Length; i++)
        {
            for (int j = 0; j < tasonKuva[i].Length; j++)
            {
                if (tasonKuva[i][j] == ' ')
                {
                    if (RandomGen.NextDouble(0, 100) > KIVEN_TN)
                    {
                        tasonKuva[i] = UusiRivi(tasonKuva[i], j);
                    }
                }
            }
        }
    }
    
    
    /// <summary>
    /// Luodaan seinä
    /// </summary>
    /// <param name="paikka">seinän paikka</param>
    /// <param name="leveys">seinän leveys</param>
    /// <param name="korkeus">seinän korkeus</param>
    private void LuoSeina(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject seina = PhysicsObject.CreateStaticObject(leveys, korkeus);
        seina.Position = paikka;
        seina.Image = seinanKuva;
        Add(seina);
    }


    /// <summary>
    /// Luodaan pelaaja
    /// </summary>
    /// <param name="paikka">pelaajan lähtöpaikka</param>
    /// <param name="leveys">pelaajan leveys</param>
    /// <param name="korkeus">pelaajan korkeus</param>
    /// <param name="vari">pelaajan väri</param>
    private void LuoHahmo(Vector paikka, double leveys, double korkeus, Color vari)
    {
        hahmo = new PhysicsObject(leveys, leveys, Shape.Circle);
        hahmo.Position = paikka;
        hahmo.Color = vari;
        hahmo.Tag = "hahmo";
        AddCollisionHandler(hahmo, "avain", AvaimenKerays);
        AddCollisionHandler(hahmo, "maali", PaasitMaaliin);
        AddCollisionHandler(hahmo, "piikki", OsuitPiikkiin);
        AddCollisionHandler(hahmo, "pahis", OsuitPahikseen);
        Add(hahmo);
    }


    /// <summary>
    /// Luodaan avaimia
    /// </summary>
    /// <param name="paikka">avaimen paikka</param>
    /// <param name="leveys">avaimen leveys</param>
    /// <param name="korkeus">avaimen korkeus</param>
    private void LuoAvain(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject avain = PhysicsObject.CreateStaticObject(leveys * 2, leveys);
        avain.Position = paikka;
        avain.Image = avaimenKuva;
        avain.Tag = "avain";
        avain.IgnoresCollisionResponse = true;
        Add(avain);
    }


    /// <summary>
    /// Luodaan maali
    /// </summary>
    /// <param name="paikka">maalin paikka</param>
    /// <param name="leveys">maalin leveys</param>
    /// <param name="korkeus">maalin korkeus</param>
    /// <param name="vari">maalin väri</param>
    private void Maali(Vector paikka, double leveys, double korkeus, Color vari)
    {
        PhysicsObject maali = PhysicsObject.CreateStaticObject(leveys, korkeus);
        maali.Position = paikka;
        maali.Color = vari;
        maali.Tag = "maali";
        Add(maali);
    }


    /// <summary>
    /// Luodaan piikkejä pelikentälle.
    /// </summary>
    /// <param name="paikka">piikin paikka</param>
    /// <param name="leveys">piikin leveys</param>
    /// <param name="korkeus">piikin korkeus</param>
    /// <param name="kulma">mihin suuntaan piikki osoittaa</param>
    /// <param name="vari">piikin väri</param>
    private void LuoPiikki(Vector paikka, double leveys, double korkeus, Jypeli.Angle kulma, Color vari)
    {
        PhysicsObject piikki = PhysicsObject.CreateStaticObject(leveys / 2, leveys, Shape.Triangle);
        piikki.Angle = kulma;
        piikki.Position = paikka;
        piikki.Tag = "piikki";
        piikki.Color = vari;
        piikki.IgnoresCollisionResponse = true;
        Add(piikki);
    }


    /// <summary>
    /// Luodaan liikkuva pahis pelikentälle
    /// </summary>
    /// <param name="paikka">paikka, josta pahis lähtee liikkeelle</param>
    /// <param name="leveys">pahiksen leveys</param>
    /// <param name="korkeus">pahiksen korkeus/<param>
    private void Pahis(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject pahis = new PhysicsObject(leveys, korkeus);
        pahis.Image = pahisKuva;
        pahis.Tag = "pahis";
        pahis.Position = paikka;
        pahis.IgnoresCollisionResponse = true;
        Add(pahis);

        List<Vector> polku = new List<Vector>();
        Vector piste1 = new Vector(paikka.X-4*leveys, paikka.Y);
        Vector piste2 = new Vector(paikka.X+4*leveys, paikka.Y);
        polku.Add(piste1);
        polku.Add(piste2);
        PathFollowerBrain aivot = new PathFollowerBrain();
        aivot.Path = polku;
        aivot.Loop = true;
        aivot.Speed = PAHIKSEN_NOPEUS;
        pahis.Brain = aivot;
        
    }


    /// <summary>
    /// Luodaan uusi rivi, missä on kiven paikka.
    /// </summary>
    /// <param name="rivi">mille riville kivi luodaan</param>
    /// <param name="paikka">mille rivin paikalle kivi luodaan</param>
    /// <returns>palauttaa uuden rivin</returns>
    private static string UusiRivi(string rivi, int paikka)
    {
        StringBuilder uusi = new StringBuilder(rivi);
        uusi[paikka] = 'k';
        string uudempi = uusi.ToString();
        return uudempi;
    }

    /// <summary>
    /// Luodaan kiviä koristamaan pelikenttää.
    /// </summary>
    /// <param name="paikka">vektorin määrämä satunnainen paikka</param>
    private void LuoKivi(Vector paikka, double leveys, double korkeus)
    {
        GameObject kivi = new GameObject(KIVEN_KOKO, KIVEN_KOKO);
        kivi.Position = paikka;
        kivi.Shape = RandomGen.SelectOne<Shape>(Shape.Pentagon, Shape.Ellipse, Shape.Diamond, Shape.Hexagon);
        kivi.Color = RandomGen.SelectOne<Color>(Color.Gray, Color.LightGray);
        kivi.Tag = "kivi";

        Add(kivi, -1);
    }


    /// <summary>
    /// Asetetaan ohjaimet.
    /// </summary>
    private void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Up, ButtonState.Down, LiikutaHahmoa, "Hahmo liikkuu ylöspäin", new Vector(0, 1));
        Keyboard.Listen(Key.Up, ButtonState.Released, PysaytaHahmo, null);

        Keyboard.Listen(Key.Down, ButtonState.Down, LiikutaHahmoa, "Hahmo liikkuu alaspäin", new Vector(0, -1));
        Keyboard.Listen(Key.Down, ButtonState.Released, PysaytaHahmo, null);

        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaHahmoa, "Hahmo liikkuu oikealle", new Vector(1, 0));
        Keyboard.Listen(Key.Right, ButtonState.Released, PysaytaHahmo, null);

        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaHahmoa, "Hahmo liikkuu vasemmalle", new Vector(-1, 0));
        Keyboard.Listen(Key.Left, ButtonState.Released, PysaytaHahmo, null);


        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// Pelaaja liikkuu näppäimen osoittamaan suuntaan.
    /// </summary>
    /// <param name="suunta">vektorin määräämä suunta</param>
    private void LiikutaHahmoa(Vector suunta)
    {
        hahmo.Push(hahmo.Mass * suunta * HAHMON_NOPEUS);
    }


    /// <summary>
    /// Hahmo pysähtyy, kun näppäintä ei paineta.
    /// </summary>
    private void PysaytaHahmo()
    {
        hahmo.Velocity = Vector.Zero;
    }


    /// <summary>
    /// Pelaaja kerää avaimen
    /// </summary>
    /// <param name="hahmo">pelajaa saa itselleen avaimen</param>
    /// <param name="avain">avain katoaa pelikentältä</param>
    private void AvaimenKerays(PhysicsObject hahmo, PhysicsObject avain)
    {
        MessageDisplay.Add("Keräsit avaimen");
        avain.Destroy();
        pistelaskuri.Value += 1;
        if (pistelaskuri == avaintenMaara[tasoNr.Value])
        {
            MessageDisplay.Add("Suunnista maaliin!");
        }
    }


    /// <summary>
    /// Pelaaja osuu piikkiin
    /// </summary>
    /// <param name="hahmo">pelaaja osuu piikkiin</param>
    /// <param name="piikki">piikki tuhoaa pelaajan</param>
    private void OsuitPiikkiin(PhysicsObject hahmo, PhysicsObject piikki)
    {
        LuoTaso();
    }


    /// <summary>
    /// Pelaaja joutuu alkuun, kun osuu pahikseen.
    /// </summary>
    /// <param name="hahmo">pelaaja osuu pahikseen</param>
    /// <param name="pahis">pahis tuhoaa pelaajan</param>
    private void OsuitPahikseen(PhysicsObject hahmo, PhysicsObject pahis)
    {
        LuoTaso();
    }


    /// <summary>
    /// Pelaaja pääsi maaliin
    /// </summary>
    /// <param name="hahmo"><pelaaja osuu maaliin/param>
    /// <param name="maali"><taso loppuu kun osuu maaliin, jos on tarpeeksi avaimia/param>
    private void PaasitMaaliin(PhysicsObject hahmo, PhysicsObject maali)
    {
        if (pistelaskuri == avaintenMaara[tasoNr.Value])
        {
            MessageDisplay.Add("Läpäisit tason!! :D");
            tasoNr.Value++;
            LuoTaso();
        }
        else MessageDisplay.Add("Kerää loputkin avaimet!!");
    }


    /// <summary>
    /// Luo pistelaskurin peliin
    /// </summary>
    private void LuoPistelaskuri()
    {
        pistelaskuri = new IntMeter(0);

        Label pistenaytto = new Label();
        pistenaytto.IntFormatString = "Avaimia kerätty: {0:D1}/"+ avaintenMaara[tasoNr.Value];
        pistenaytto.X = Screen.Left + 100;
        pistenaytto.Y = Screen.Top - 100;
        pistenaytto.TextColor = Color.Black;
        pistenaytto.Color = Color.White;

        pistenaytto.BindTo(pistelaskuri);
        Add(pistenaytto);
    }


}


