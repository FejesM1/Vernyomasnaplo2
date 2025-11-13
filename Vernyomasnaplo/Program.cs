using System;
using System.Collections.Generic;
using System.IO;

namespace Vernyomasnaplo
{
    internal class Program
    {
        static bool fut = true;
        static string bejelentkezettFelhasznalo = "";
        static bool bejelentkezve = false;
        static bool kivalasztva = false;
        static string[] menupontok = { "Regisztrálás", "Bejelentkezés", "Adatok hozzáadása", "Adatok módosítása", "Adatok megjelenítése", "Adat törlése", "Beállítások", "Kilépés" };
        static Action[] fuggvenyek = { Regisztral, Bejelentkezes, AdatokHozzaadasa, Modosit, Megjelenit, Torol, Beallit, Kilep };
        static int aktualis_menu_pont = 0;
        static int menupontok_szama = menupontok.Length;

        static ConsoleColor[] szinek = {
            ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Yellow,
            ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Gray, ConsoleColor.Black
        };

        static string[] szinek_neve = {
            "Zöld","Piros","Kék","Sárga","Cian","Magenta","Szürke","Fekete"
        };

        static int szinek_szama = szinek.Length;
        static int alapszin = 0;
        static int alaphatter = 7;

        // 🔹 Fájlok elérési útja a projekt gyökerében
        static readonly string adatokFile = "Adatok.txt";
        static readonly string felhasznalokFile = "Felhaszn.txt";

        static List<string> adatok = new List<string>();

        static void Main(string[] args)
        {
            // 🔹 Fájlok létrehozása, ha nem léteznek
            if (!File.Exists(adatokFile)) File.Create(adatokFile).Close();
            if (!File.Exists(felhasznalokFile)) File.Create(felhasznalokFile).Close();

            // 🔹 Adatok beolvasása
            adatok.Clear();
            foreach (var sor in File.ReadAllLines(adatokFile))
            {
                if (!string.IsNullOrWhiteSpace(sor))
                    adatok.Add(sor);
            }

            while (fut)
            {
                Console.BackgroundColor = szinek[alaphatter];
                try
                {
                    Console.Clear();
                    for (int i = 0; i < menupontok_szama; i++)
                    {
                        if (aktualis_menu_pont == i)
                        {
                            Console.ForegroundColor = szinek[alapszin];
                            Console.WriteLine(menupontok[i]);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.WriteLine(menupontok[i]);
                        }
                    }

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Enter:
                            kivalasztva = true;
                            break;
                        case ConsoleKey.UpArrow:
                            if (aktualis_menu_pont > 0) aktualis_menu_pont--;
                            break;
                        case ConsoleKey.DownArrow:
                            if (aktualis_menu_pont < menupontok_szama - 1) aktualis_menu_pont++;
                            break;
                        default:
                            Console.Beep();
                            break;
                    }

                    if (kivalasztva)
                    {
                        fuggvenyek[aktualis_menu_pont]();
                        kivalasztva = false;
                        aktualis_menu_pont = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.Clear();
                    Console.WriteLine("Hiba történt!");
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }

        // 🔸 Felhasználó létezés ellenőrzés
        static bool FelhasznaloLetezik(string nev)
        {
            foreach (var sor in File.ReadAllLines(felhasznalokFile))
            {
                var adatok = sor.Split(';');
                if (adatok.Length > 0 && adatok[0] == nev)
                    return true;
            }
            return false;
        }

        static void Regisztral()
        {
            Console.Clear();
            Console.WriteLine("Regisztrálás:\n");
            Console.Write("Felhasználónév: ");
            string nev = Console.ReadLine();

            if (FelhasznaloLetezik(nev))
            {
                Console.WriteLine("Ez a felhasználónév már létezik!");
                Console.ReadLine();
                return;
            }

            Console.Write("Jelszó: ");
            string jelszo = Console.ReadLine();

            File.AppendAllText(felhasznalokFile, $"{nev};{jelszo}{Environment.NewLine}");
            Console.WriteLine("Sikeres regisztráció!");
            Console.ReadLine();
        }

        static void Bejelentkezes()
        {
            Console.Clear();
            Console.WriteLine("Bejelentkezés:\n");

            Console.Write("Felhasználónév: ");
            string nev = Console.ReadLine();
            Console.Write("Jelszó: ");
            string jelszo = Console.ReadLine();

            bool sikeres = false;
            foreach (var sor in File.ReadAllLines(felhasznalokFile))
            {
                var adatok = sor.Split(';');
                if (adatok.Length >= 2 && adatok[0] == nev && adatok[1] == jelszo)
                {
                    sikeres = true;
                    break;
                }
            }

            if (sikeres)
            {
                bejelentkezve = true;
                bejelentkezettFelhasznalo = nev;
                Console.WriteLine("Sikeres bejelentkezés!");
            }
            else
            {
                Console.WriteLine("Hibás felhasználónév vagy jelszó!");
            }
            Console.ReadLine();
        }

        static void AdatokHozzaadasa()
        {
            Console.Clear();
            
            for(int i = 0; i < adatok.Count; i++)
            {
                string[] mezo = adatok[i].Split(';');
                if (mezo[0] == bejelentkezettFelhasznalo)
                {
                    Console.Write("Adja meg a vérnyomását: ");
                    int vernyomas = int.Parse(Console.ReadLine());
                    Console.Write("Adja meg a pulzusát: ");
                    int pulzus = int.Parse(Console.ReadLine());
                    string keszadat = $"{vernyomas};{pulzus};";
                    if (mezo[1] != "")
                    {
                        adatok.Insert(adatok.Count, "|");
                    }
                    adatok.Insert(i, keszadat);
                    for (int j = 0; j < adatok.Count; j++)
                    {
                        File.AppendAllText(adatokFile, adatok[j]);
                    }
                    Console.WriteLine("Adat hozzáadva.");
                    Console.ReadLine();
                }
            }

        }

        static void Modosit()
        {
            Console.Clear();

            if (adatok.Count == 0)
            {
                Console.WriteLine("Nincs módosítható adat!");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < adatok.Count; i++)
            {
                string[] mezok = adatok[i].Split(';');
                Console.WriteLine($"{i + 1}. Vérnyomás: {mezok[1]}, Pulzus: {mezok[2]}");
            }

            Console.Write("Add meg a módosítandó sorszámot: ");
            int index = int.Parse(Console.ReadLine()) - 1;

            if (index < 0 || index >= adatok.Count)
            {
                Console.WriteLine("Érvénytelen sorszám!");
                Console.ReadLine();
                return;
            }

            Console.Write("Új vérnyomás: ");
            int ujVernyomas = int.Parse(Console.ReadLine());
            Console.Write("Új pulzus: ");
            int ujPulzus = int.Parse(Console.ReadLine());

            adatok[index] = $"{ujVernyomas};{ujPulzus}";

            File.WriteAllLines(adatokFile, adatok);

            Console.WriteLine("Adat módosítva.");
            Console.ReadLine();
        }

        static void Megjelenit()
        {
            Console.Clear();
            Console.WriteLine("Adatok megjelenítése:\n");

            if (adatok.Count == 0)
            {
                Console.WriteLine("Nincs adat a listában.");
            }
            else
            {
                foreach (var sor in adatok)
                {
                    var mezok = sor.Split(';');
                    Console.WriteLine($"Vérnyomás: {mezok[1]}, Pulzus: {mezok[2]}");
                }
            }

            Console.WriteLine("\nNyomjon Entert a visszatéréshez.");
            Console.ReadLine();
        }

        static void Torol()
        {
            Console.Clear();
            Console.ReadLine();
        }

        static void Beallit()
        {
            string[] kiirni = { "Betű", "Háttér" };
            bool megy = true;
            int aktualis = 0;
            while (megy)
            {
                Console.Clear();

                for (int i = 0; i < kiirni.Length; i++)
                {
                    if (aktualis == i)
                    {
                        Console.ForegroundColor = szinek[alapszin];
                        Console.WriteLine(kiirni[i]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine(kiirni[i]);
                    }
                }

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Enter:
                        int szin = Szinvalaszto();
                        if (alaphatter != szin && alapszin != szin)
                        {
                            megy = false;
                            if (aktualis == 0)
                                alapszin = szin;
                            else
                                alaphatter = szin;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Nem lehet a betű és a háttér azonos színű!");
                            Console.ReadLine();
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (aktualis < kiirni.Length - 1)
                            aktualis++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (aktualis > 0)
                            aktualis--;
                        break;
                }
            }
        }

        static int Szinvalaszto()
        {
            bool kivalaszt = true;
            int akt_szin_szama = 0;
            while (kivalaszt)
            {
                Console.Clear();

                for (int i = 0; i < szinek.Length; i++)
                {
                    if (i == akt_szin_szama)
                    {
                        Console.ForegroundColor = szinek[i];
                        Console.WriteLine(szinek_neve[i]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine(szinek_neve[i]);
                    }
                }

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Enter:
                        kivalaszt = false;
                        break;
                    case ConsoleKey.UpArrow:
                        if (akt_szin_szama > 0)
                            akt_szin_szama--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (akt_szin_szama < szinek_szama - 1)
                            akt_szin_szama++;
                        break;
                    default:
                        Console.Beep();
                        break;
                }
            }
            return akt_szin_szama;
        }

        static void Kilep()
        {
            fut = false;
        }
    }
}
