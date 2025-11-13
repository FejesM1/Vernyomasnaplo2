using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Vernyomasnaplo
{
    internal class Program
    {
        static bool fut = true;
        static string bejelentkezettFelhasznalo = "";
        static bool bejelentkezve = false;
        static bool kivalasztva = false;
        static string[] menupontok = { "Regisztrálás", "Bejelentkezés", "Adatok hozzáadása", "Adatok módosítása", "Adatok megjelenítése", "Adat törlése", "Beállítások", "Kilépés" };
        static Action[] fuggvenyek = { Regisztral, Bejelentkezes, AdatokHozzaadasa, Modosit, Megjelenit, Torol, Beallit, Kilep, };
        static int aktualis_menu_pont = 0;
        static int menupontok_szama = menupontok.Length;
        static ConsoleColor[] szinek = {
                ConsoleColor.Green,
                ConsoleColor.Red,
                ConsoleColor.Blue,
                ConsoleColor.Yellow,

                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.Gray,
                ConsoleColor.Black
            };
        static string[] szinek_neve = {
                    "Zöld",
                    "Piros",
                    "Kék",
                    "Sárga",

                    "Cian",
                    "Magenta",
                    "Szürke",
                    "Fekete"
                };
        static int szinek_szama = szinek.Length;
        static int alapszin = 0;
        static int alaphatter = 7;

        static void Main(string[] args)
        {
            adatok.Clear();
            foreach (var sor in File.ReadAllLines("Adatok.txt"))
            {
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
                            if (aktualis_menu_pont > 0)
                            {
                                aktualis_menu_pont--;
                            }
                            break;

                        case ConsoleKey.DownArrow:
                            if (aktualis_menu_pont < menupontok_szama - 1)
                            {
                                aktualis_menu_pont++;
                            }
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
                    Console.WriteLine("Hiba történt!");
                    Console.WriteLine("Szeretné látni a hibát?");

                }
            }

        }
        static bool FelhasznaloLetezik(string nev)
        {
            if (!File.Exists("Felhaszn.txt")) return false;

            foreach (var sor in File.ReadAllLines("Felhaszn.txt"))
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

            File.AppendAllText("Felhaszn.txt", $"{nev};{jelszo}\n");
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

            if (!File.Exists("Felhaszn.txt"))
            {
                Console.WriteLine("Még nincs regisztrált felhasználó!");
                Console.ReadLine();
                return;
            }

            bool sikeres = false;
            foreach (var sor in File.ReadAllLines("Felhaszn.txt"))
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

        static void Modosit()
        {
            Console.Clear();
            int bekeres;
            string adat;

            for (int i = 0; i < adatok.Count; i++)
            {
                string[] mezok = adatok[i].Split(';');
                Console.WriteLine($"Vérnyomás: {mezok[0]}, Pulzus: {mezok[1]}");
            }

            Console.Write("Add a módosítandó sornak a számát: ");
            bekeres = int.Parse(Console.ReadLine()) - 1;
            Console.Write("Adja meg a vérnyomását: ");
            int bekeres2 = int.Parse(Console.ReadLine());
            Console.Write("Adja meg a pulzusát: ");
            int bekeres3 = int.Parse(Console.ReadLine());
            var keszadat = bekeres2 + ";" + bekeres3;

            if (bekeres < adatok.Count && bekeres >= 0)
            {
                adatok.RemoveAt(bekeres);
                adatok.Insert(bekeres, keszadat);
                File.WriteAllText("Adatok.txt", "");
                for (int i = 0; i < adatok.Count; i++)
                {
                    adat = adatok[i];
                    File.WriteAllText("Adatok.txt", adat + Environment.NewLine);
                }
                Console.WriteLine("Adat módosítva.");
            }
            else
            {
                Console.WriteLine("Nincs ilyen sorszámú adat!");
                Console.ReadLine();
            }
        }
        static List<string> adatok = new List<string>();
        static void Megjelenit()
        {
            Console.Clear();
            if (!File.Exists("Adatok.txt"))
            {
                Console.WriteLine("Az Adatok.txt nem található!");
            }
            else
            {
                Console.WriteLine("Adatok megjelenítése\n");
                for (int i = 0; i < adatok.Count; i++)
                {
                    string[] mezok = adatok[i].Split(';');
                    Console.WriteLine($"Vérnyomás: {mezok[0]}, Pulzus: {mezok[1]}");
                }
                Console.WriteLine("\nNyomjon le egy billentyűt a kilépéshez.");
                Console.ReadLine();
            }
        }
        static void AdatokHozzaadasa()
        {
            Console.Clear();
            Console.WriteLine("Adatok hozzáadása");
            Console.ReadLine();
        }
        static void Torol()
        {
            Console.Clear();
            Console.WriteLine("töröl");
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

                        Console.ForegroundColor = szinek[aktualis];
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
                            {
                                alapszin = szin;
                            }
                            else
                            {
                                alaphatter = szin;
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Nem lehet a betű és a háttér azonos színű! Enterre tovább!!!");
                            Console.ReadLine();
                        }


                        break;

                    case ConsoleKey.DownArrow:
                        if (aktualis < kiirni.Length - 1)
                        {
                            aktualis++;
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        if (aktualis > 0)
                        {
                            aktualis--;
                        }
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

                Console.WriteLine();
                for (int i = 0; i < szinek.Length; i++)
                {
                    if (i == akt_szin_szama)
                    {

                        Console.ForegroundColor = szinek[akt_szin_szama];
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
                        {
                            akt_szin_szama--;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (akt_szin_szama < szinek_szama - 1)
                        {
                            akt_szin_szama++;
                        }
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
