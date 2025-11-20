
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Threading;

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
            Console.BackgroundColor = szinek[alaphatter];
            while (fut)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = szinek[alaphatter];

                Console.Clear();
               
                int startIndex = bejelentkezve ? 2 : 0;
                int endIndex = bejelentkezve ? menupontok.Length : 2;


                for (int i = startIndex; i < endIndex; i++)
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


                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Enter:

                        if (aktualis_menu_pont >= startIndex && aktualis_menu_pont < endIndex)
                            fuggvenyek[aktualis_menu_pont]();


                        if (bejelentkezve && aktualis_menu_pont < 2)
                            aktualis_menu_pont = 2;
                        break;

                    case ConsoleKey.UpArrow:
                        if (aktualis_menu_pont > startIndex)
                            aktualis_menu_pont--;
                        else
                        {
                            aktualis_menu_pont = endIndex - 1;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (aktualis_menu_pont < endIndex - 1)
                            aktualis_menu_pont++;
                        else
                        {
                            aktualis_menu_pont = 2;
                        }
                        break;

                    default:
                        Console.Beep();
                        break;
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
            Console.BackgroundColor = szinek[alaphatter];
            Console.ForegroundColor = szinek[alapszin];
            Console.Clear();
            Console.WriteLine("Regisztrálás:\n");
            Console.Write("Felhasználónév: ");
            string nev = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(nev))
            {
                Console.WriteLine("A felhasználónév nem lehet üres!");
                Console.ReadLine();
                return;
            }

            if (FelhasznaloLetezik(nev))
            {
                Console.WriteLine("Ez a felhasználónév már létezik!");
                Console.ReadLine();
                return;
            }
            Console.Write("Születési dátum: ");
            DateTime szul_datum = DateTime.Parse(Console.ReadLine());
            

            Console.Write("Jelszó: ");
            string jelszo = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(jelszo))
            {
                Console.WriteLine("A jelszó nem lehet üres!");
                Console.ReadLine();
                return;
            }
            File.AppendAllText(felhasznalokFile, $"{nev};{jelszo};{szul_datum}{Environment.NewLine}");
            File.AppendAllText(adatokFile, $"{nev}({Environment.NewLine}");
            adatok.Add($"{nev}(");
            Console.WriteLine("Sikeres regisztráció!");
            Console.ReadLine();
        }

        static void Bejelentkezes()
        {
            Console.BackgroundColor = szinek[alaphatter];
            Console.ForegroundColor = szinek[alapszin];
            Console.Clear();
            Console.WriteLine("Bejelentkezés:\n");

            Console.Write("Felhasználónév: ");
            string nev = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(nev))
            {
                Console.WriteLine("A felhasználónév nem lehet üres!");
                Console.ReadLine();
                return;
            }

            Console.Write("Jelszó: ");
            string jelszo = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(jelszo))
            {
                Console.WriteLine("A jelszó nem lehet üres!");
                Console.ReadLine();
                return;
            }

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
            Console.BackgroundColor = szinek[alaphatter];
            Console.ForegroundColor = szinek[alapszin];
            Console.Clear();
            bool talalat = false;
            string nev = "";
            int index = 0;
            for (int i = 0; i < adatok.Count; i++)
            {
                if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                {
                    nev = adatok[i].Split('(')[0];
                    talalat = true;
                    index = i;
                    break;
                }
            }
            if (talalat == true)
            {
                Console.Write("Adja meg a vérnyomását: ");
                int vernyomas = int.Parse(Console.ReadLine());
                Console.Write("Adja meg a pulzusát: ");
                int pulzus = int.Parse(Console.ReadLine());

                string keszadat = $"{vernyomas};{pulzus}";

                if (adatok[index].Split('(')[1] != "")
                {
                    adatok[index] += "|";
                    adatok[index] += keszadat;
                }
                else
                {
                    adatok[index] += keszadat;
                }
            }
            else
            {
                Console.WriteLine("Az Adatok.txt be nem került bele a felhasználó neved!");
                Console.ReadLine();
            }
            File.WriteAllText(adatokFile, "");
            for (int i = 0; i < adatok.Count; i++)
            {
                File.AppendAllText(adatokFile, adatok[i] + Environment.NewLine);
            }
            Console.WriteLine("Adat hozzáadva.");
            Console.ReadLine();
        }

        static void Modosit()
        {
            Console.BackgroundColor = szinek[alaphatter];
            Console.ForegroundColor = szinek[alapszin];
            Console.Clear();
            bool talalat = false;
            int index = 0;
            for (int i = 0; i < adatok.Count; i++)
            {
                if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                {
                    talalat = true;
                    index = i;
                    break;
                }
            }

            Console.WriteLine("Adatok megjelenítése:\n");

            int darab = adatok[index].Split('(')[1].Split('|').Count();
            int törles;

            if (talalat == true)
            {
                Console.WriteLine($"A felhasználó neve: {adatok[index].Split('(')[0]}\n");
                if (adatok[index].Split('(')[1] != "")
                {
                    for (int i = 0; i < darab; i++)
                    {
                        Console.Write($"A(z) {i + 1}. mérés eredménye vérnyomás : {adatok[index].Split('(')[1].Split('|')[i].Split(';')[0]} pulzus: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[1]}\n");
                    }
                }
            }

            Console.Write("\nAdja meg a módosítandó mérés sorszámát: ");
            törles = int.Parse(Console.ReadLine());
            Console.Write("Add meg vérnyomásod: ");
            int vernyomas = int.Parse(Console.ReadLine());
            Console.Write("Add meg a pulzusod: ");
            int pulzus = int.Parse(Console.ReadLine());
            var modositas = $"{vernyomas};{pulzus}";

            var start = adatok[index].IndexOf(adatok[index].Split('(')[1].Split('|')[törles - 1]);
            var vege = adatok[index].Split('(')[1].Split('|')[törles - 1].Length;

            adatok[index] = adatok[index].Remove(start, vege);

            adatok[index] = adatok[index].Insert(start, modositas);

            File.WriteAllText(adatokFile, "");
            for (int i = 0; i < adatok.Count; i++)
            {
                File.AppendAllText(adatokFile, adatok[i] + Environment.NewLine);
            }
            Console.WriteLine("Adat módosítva.");
            Console.ReadLine();
        }

        static void Megjelenit()
        {
            Console.BackgroundColor = szinek[alaphatter];
            Console.ForegroundColor = szinek[alapszin];
            Console.Clear();
            bool talalat = false;
            int index = 0;
            for (int i = 0; i < adatok.Count; i++)
            {
                if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                {
                    talalat = true;
                    index = i;
                    break;
                }
            }
            Console.WriteLine("Adatok megjelenítése:\n");

            if (talalat == true)
            {
                Console.WriteLine($"A felhasználó neve: {adatok[index].Split('(')[0]}\n");
                if (adatok[index].Split('(')[1] != "")
                {
                    int darab = adatok[index].Split('(')[1].Split('|').Count();
                    for (int i = 0; i < darab; i++)
                    {
                        Console.Write($"A(z) {i + 1}. mérés eredménye vérnyomás : {adatok[index].Split('(')[1].Split('|')[i].Split(';')[0]} pulzus: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[1]}\n");
                    }
                }
            }

            Console.WriteLine("\nNyomjon Entert a visszatéréshez.");
            Console.ReadLine();
        }

        static void Torol()
        {
            Console.BackgroundColor = szinek[alaphatter];
            Console.ForegroundColor = szinek[alapszin];
            Console.Clear();
            Console.ReadLine();
        }

        static void Beallit()
        {
            Console.BackgroundColor = szinek[alaphatter];
            Console.ForegroundColor = szinek[alapszin];
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
                        if (szin == -1)
                        {
                            break;
                        }
                        if (alaphatter != szin && alapszin != szin)
                        {
                            megy = false;
                            if (aktualis == 0)
                                alapszin = szin;
                            else
                                alaphatter = szin;
                        }
                   
                        break;

                    case ConsoleKey.DownArrow:
                        if (aktualis < kiirni.Length - 1)
                            aktualis++;
                        else
                        {
                            aktualis = 0;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (aktualis > 0)
                            aktualis--;
                        else
                        {
                            aktualis = kiirni.Length - 1;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        megy = false;
                        break;
                }
            }
        }

        static int Szinvalaszto()
        {



            Console.BackgroundColor = szinek[alaphatter];
            Console.ForegroundColor = szinek[alapszin];


            ConsoleColor[] szinek_kicsi = new ConsoleColor[szinek.Length - 1];
            string[] szinek_neve_kicsi = new string[szinek_neve.Length - 1];
            int jo_index = 0;
            for (int i = 0; i < szinek.Length; i++)
            {
                if (i == alapszin || i==alaphatter)
                    continue; // ezt a színt kihagyjuk

                szinek_kicsi[jo_index] = szinek[i];
                szinek_neve_kicsi[jo_index] = szinek_neve[i];
                jo_index++;
            }


            bool kivalaszt = true;
            int akt_szin_szama = 0;

            while (kivalaszt)
            {
                Console.Clear();

                for (int i = 0; i < szinek_kicsi.Length; i++)
                {



                    if (i == akt_szin_szama)
                    {

                        Console.ForegroundColor = szinek_kicsi[i];
                        Console.WriteLine(szinek_neve_kicsi[i]);
                        Console.ForegroundColor = ConsoleColor.White;


                    }
                    else
                    {
                        Console.WriteLine(szinek_neve_kicsi[i]);
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
                        else
                        {
                            akt_szin_szama = szinek_szama - 3;
                        }
                        break;
                    case ConsoleKey.DownArrow:

                        if (akt_szin_szama < szinek_szama - 3)
                            akt_szin_szama++;
                        else
                        {
                            akt_szin_szama = 0;
                        }
                        break;
                    default:
                        Console.Beep();
                        break;
                    case ConsoleKey.LeftArrow:
                        kivalaszt = false;
                        return -1;

                }
            }

            int vissza= Array.IndexOf(szinek, szinek_kicsi[akt_szin_szama]);
            return vissza;

        }

        static void Kilep()
        {
            fut = false;
        }
    }
}